using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Application.Features.User.Commands.BulkInsertUsers;

public record BulkInsertUsersCommand(List<UserBulkUploadDto> Users, string passwordResetUrl, string passwordResetHTMLTemplate, string FileName) : IRequest<BulkUploadStatusDto>;
public class BulkInsertUsersCommandHandler : IRequestHandler<BulkInsertUsersCommand, BulkUploadStatusDto>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICryptoService _cryptoService;
    private readonly IEmailService _emailService;
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly ISuperUsersRepository _superUsersRepository;
    public BulkInsertUsersCommandHandler(IUsersRepository usersRepository, ITelephoneCodeMasterRepository telephoneCodeMasterRepository, ICryptoService cryptoService,
        IEmailService emailService, IBulkUploadErrorListRepository bulkUploadErrorListRepository, ICountryMasterRepository countryMasterRepository, ISuperUsersRepository superUsersRepository)
    {
        _usersRepository = usersRepository;
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
        _cryptoService = cryptoService;
        _emailService = emailService;
        _bulkUploadErrorListRepository = bulkUploadErrorListRepository;
        _countryMasterRepository = countryMasterRepository;
        _superUsersRepository = superUsersRepository;
    }
    public async Task<BulkUploadStatusDto> Handle(BulkInsertUsersCommand request, CancellationToken cancellationToken)
    {
        //var fileSizeLimit = await _countryMasterRepository.GetBulkUploadFileSizeLimitMbAsync();
        //if (fileSizeLimit.HasValue && request.Users.Count > fileSizeLimit.Value * 1024 * 1024) //Check if the number of countries exceeds the file size limit in MB
        //{
        //    throw new ValidationException($"The number of currencies exceeds the file size limit of {fileSizeLimit.Value} MB.");
        //}

        bool IsBulkInsertSuccessful = false; //Initialize result to false
        Guid NewProcessId = Guid.NewGuid();
        BulkUploadStatusDto bulkUploadStatusDto = new BulkUploadStatusDto() { ProcessId = NewProcessId, FileName = request.FileName, ProcessedAt = DateTime.UtcNow };
        List<BulkUploadErrorList> errorLists = new List<BulkUploadErrorList>();
        List<UserBulkUploadDto> usersToInsert = new List<UserBulkUploadDto>();

        // Pseudocode plan:
        // 1. When building the dictionary, handle duplicate TelephoneCode values gracefully.
        // 2. Use GroupBy to ensure only one entry per TelephoneCode (e.g., first occurrence).
        // 3. Build the dictionary from the grouped result.

        var telephoneCodes = await _telephoneCodeMasterRepository.GetAllTelephoneCodesAsync();
        var existingTelephoneCodes = telephoneCodes
            .GroupBy(tc => tc.TelephoneCode)
            .ToDictionary(g => g.Key, g => g.First().TelephoneCodeId);

        var allExistingUsers = await _usersRepository.GetAllUsersEmailAsync();
        var ExistingSuperAdmin = await _superUsersRepository.GetSuperUserEmailAsync((int)Roles.SuperAdmin);
        var existingUsers = new HashSet<string>(allExistingUsers.Select(u => u.ToLower()));

        foreach (var user in request.Users)
        {
            //Add Email Hash
            user.EmailHash = _cryptoService.GenerateIntegerHashForString(user.Email.Trim()); //GenerateIntegerHashForString make sure that the email is lower cased and trimmed
            //Check if TelephoneCode is valid
            if (existingTelephoneCodes.TryGetValue(user.TelephoneCode, out int telephoneCodeId))
            {
                user.TelephoneCodeId = telephoneCodeId;
                if (existingUsers.Contains(user.Email.ToLower()))
                {
                    errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = user.RowId, ErrorField = "Email", ErrorMessage = $"Duplicate Entry for {user.Email}" });
                }
                else if (ExistingSuperAdmin != null && string.Equals(ExistingSuperAdmin, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = user.RowId, ErrorField = "Email", ErrorMessage = $"Email '{user.Email}' is reserved and cannot be used." });
                }
                else if (string.IsNullOrWhiteSpace(user.TelephonePhoneNumber) || !user.TelephonePhoneNumber.All(char.IsDigit))
                {
                    // Phone number must contain only integer digits
                    errorLists.Add(new BulkUploadErrorList()
                    {
                        ProcessId = NewProcessId,
                        RowId = user.RowId,
                        ErrorField = "PhoneNumber",
                        ErrorMessage = $"PhoneNumber '{user.TelephonePhoneNumber}' should contain only digits for Email '{user.Email}'"
                    });
                }
                else
                {
                    usersToInsert.Add(user);
                }
            }
            else
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = user.RowId, ErrorField = "Telephone Code", ErrorMessage = $"Invalid Telephone Code '{user.TelephoneCode}' for Email '{user.Email}'" });
            }
        }
        if (usersToInsert?.Any() == true) //If there are users to insert, call the repository method to insert them
        {
            IsBulkInsertSuccessful = await _usersRepository.BulkInsertUsersAsync(usersToInsert);

            // Run email sending in the background to avoid blocking the main process
            //_ = Task.Run(async () =>
            //{
            foreach (var user in usersToInsert)
            {
                var resetToken = _cryptoService.EncryptPasswordResetData(user.Email);
                var resetLink = $"{request.passwordResetUrl}?token={resetToken}";
                var mailHtmlBody = request.passwordResetHTMLTemplate
                    .Replace("{{FirstName}}", user.FirstName)
                    .Replace("{{LastName}}", user.LastName)
                    .Replace("{{RESET_PASSWORD_LINK}}", resetLink)
                    .Replace("{{RESET_TOKEN}}", resetToken);

                try
                {
                    var mailSendResponse = await _emailService.SendEmailAsync(new EmailRequestDto()
                    {
                        HtmlBody = mailHtmlBody,
                        Subject = "Welcome to the ElixirHr Application!",
                        To = user.Email,
                    });
                    if (mailSendResponse.IsSuccess)
                    {
                        await _usersRepository.SaveResetPasswordTokenAsync(_cryptoService.GenerateIntegerHashForString(user.Email), user.Email, resetToken);
                    }
                }
                catch
                {
                    // Optionally log the exception or handle failed emails here
                }
            }
            //});

            #region previous code for email sending
            //foreach (var user in usersToInsert)
            //{
            //    var resetToken = _cryptoService.EncryptPasswordResetData(user.Email);
            //    var resetLink = $"{request.passwordResetUrl}?token={resetToken}";
            //    var mailHtmlBody = request.passwordResetHTMLTemplate
            //        .Replace("{{FirstName}}", user.FirstName)
            //        .Replace("{{LastName}}", user.LastName)
            //        .Replace("{{RESET_PASSWORD_LINK}}", resetLink)
            //        .Replace("{{RESET_TOKEN}}", resetToken);

            //    var mailSendResponse = await _emailService.SendEmailAsync(new EmailRequestDto()
            //    {
            //        HtmlBody = mailHtmlBody,
            //        Subject = "Welcome to the ElixirHr Application!",
            //        To = user.Email,
            //    });
            //}
            #endregion
        }
        if (errorLists.Count > 0)
        {
            await _bulkUploadErrorListRepository.BulkInsertBulkUploadErrorListAsync(errorLists);
        }

        bulkUploadStatusDto.IsSuccess = false;
        if (IsBulkInsertSuccessful && errorLists.Count() == 0) bulkUploadStatusDto.IsSuccess = true;
        if (IsBulkInsertSuccessful && errorLists.Count() > 0) bulkUploadStatusDto.IsPartialSuccess = true;

        return bulkUploadStatusDto;
    }
}
