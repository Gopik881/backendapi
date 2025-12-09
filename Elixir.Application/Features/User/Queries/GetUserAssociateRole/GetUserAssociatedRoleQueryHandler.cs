
//using Elixir.Application.Features.User.DTOs;
//using Elixir.Application.Interfaces.Persistance;
//using MediatR;

//namespace Elixir.Application.Features.User.Queries.GetUserAssociateRole;

//public record GetUserAssociatedRoleQuery(int RoleId) : IRequest<IEnumerable<UserRoleDto>>;
//public class GetUserAssociatedRoleQueryHandler : IRequestHandler<GetUserAssociatedRoleQuery, IEnumerable<UserRoleDto>>
//{
//    private readonly IRolesRepository _rolesRepository;
//    public GetUserAssociatedRoleQueryHandler(IRolesRepository rolesRepository)
//    {
//        _rolesRepository = rolesRepository;
//    }
//    public async Task<IEnumerable<UserRoleDto>> Handle(GetUserAssociatedRoleQuery request, CancellationToken cancellationToken)
//    {
//        return await _rolesRepository.GetUserAssociatedRoleAsync(request.RoleId);
//    }
//}
