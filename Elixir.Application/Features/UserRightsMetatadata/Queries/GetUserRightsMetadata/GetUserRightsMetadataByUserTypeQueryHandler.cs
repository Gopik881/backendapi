using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using MediatR;

namespace Elixir.Application.Features.UserRightsMetatadata.Queries.GetUserRightsMetadata;

public record GetUserRightsMetadataByUserTypeQuery(int UserType) : IRequest<UserRightsMetedataResponseDto>;

public class GetUserRightsMetadataByUserTypeQueryHandler : IRequestHandler<GetUserRightsMetadataByUserTypeQuery, UserRightsMetedataResponseDto>
{
    public Task<UserRightsMetedataResponseDto> Handle(GetUserRightsMetadataByUserTypeQuery request, CancellationToken cancellationToken)
    {
        if (request.UserType == 1)
        {
            var generator = new UserRightsMetadataGenerator();
            return Task.FromResult(generator.GetUserRightsMetadataForUserType1());
        }
        else if(request.UserType == 2)
        {
            var generator = new UserRightsMetadataGenerator();
            return Task.FromResult(generator.GetUserRightsMetadataForUserType2());
        }
        else
        {
            throw new ArgumentException("Invalid user type");
        }
    }
}
