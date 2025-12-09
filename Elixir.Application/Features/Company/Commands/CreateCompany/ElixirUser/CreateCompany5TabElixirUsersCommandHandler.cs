using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabElixirUsersCommand(int companyId, int userId, List<Company5TabElixirUserDto> ElixirUsersDto) : IRequest<bool>;

public class CreateCompany5TabElixirUsersCommandHandler : IRequestHandler<CreateCompany5TabElixirUsersCommand, bool>
{
    private readonly IElixirUsersHistoryRepository _elixirUsersRepository;

    public CreateCompany5TabElixirUsersCommandHandler(IElixirUsersHistoryRepository elixirUsersRepository)
    {
        _elixirUsersRepository = elixirUsersRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabElixirUsersCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _elixirUsersRepository.Company5TabCreateElixirUserDataAsync(request.companyId, request.ElixirUsersDto, request.userId);

    }
}
