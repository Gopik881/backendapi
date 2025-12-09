using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Queries.ResetLinkTokenValidation
{
    public record ResetLinkTokenCommand(string Token) : IRequest<bool>;

    public class ResetLinkTokenCommandHandler : IRequestHandler<ResetLinkTokenCommand, bool>
    {
        private readonly IUsersRepository _usersRepository;

        public ResetLinkTokenCommandHandler(IUsersRepository usersRepository, ICryptoService cryptoService)
        {
            _usersRepository = usersRepository;
        }

        public async Task<bool> Handle(ResetLinkTokenCommand request, CancellationToken cancellationToken)
        {
            return await _usersRepository.ValidateResetLinkToken(request.Token);
        }
    }
}
