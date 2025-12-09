using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.ElixirUser;

public record ApproveCompany5TabElixirUsersCommand(int companyId, int userId, List<Company5TabElixirUserDto> ElixirUsersDto) : IRequest<bool>;

public class ApproveCompany5TabElixirUsersCommandHandler : IRequestHandler<ApproveCompany5TabElixirUsersCommand, bool>
{
    private readonly IElixirUsersRepository _elixirUsersRepository;

    public ApproveCompany5TabElixirUsersCommandHandler(IElixirUsersRepository elixirUsersRepository)
    {
        _elixirUsersRepository = elixirUsersRepository;
    }

    public async Task<bool> Handle(ApproveCompany5TabElixirUsersCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _elixirUsersRepository.Company5TabApproveElixirUserDataAsync(request.companyId, request.ElixirUsersDto, request.userId);

    }
}
