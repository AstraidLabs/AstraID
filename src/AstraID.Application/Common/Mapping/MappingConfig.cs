using Mapster;
using AstraID.Domain.Entities;
using AstraID.Application.Users.Queries.GetUserById;

namespace AstraID.Application.Common.Mapping;

public static class MappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, UserDto>()
              .Map(dest => dest.DisplayName, src => src.DisplayNameRaw);
    }
}
