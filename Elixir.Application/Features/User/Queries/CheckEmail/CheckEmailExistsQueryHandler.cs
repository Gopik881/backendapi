using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Queries.CheckEmail;

public record CheckEmailExistsQuery(string Email) : IRequest<bool>;
public class CheckEmailExistsQueryHandler : IRequestHandler<CheckEmailExistsQuery, bool>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICryptoService _cryptoService;
    public CheckEmailExistsQueryHandler(IUsersRepository usersRepository,ICryptoService cryptoService) 
    {
        _cryptoService = cryptoService;
        _usersRepository = usersRepository;
    }
    public async Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
    {
        return await _usersRepository.ExistsUserByEmailAsync(request.Email, _cryptoService.GenerateIntegerHashForString(request.Email));
        
    }
}
