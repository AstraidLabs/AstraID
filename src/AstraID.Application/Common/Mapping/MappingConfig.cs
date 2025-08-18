using System.Linq;
using Mapster;
using AstraID.Domain.Entities;
using AstraID.Application.Users.Queries.GetUserById;
using AstraID.Application.Clients.Queries.GetClientById;
using AstraID.Application.Consents.Queries.ListUserConsents;
using AstraID.Application.Sessions.Queries.ListActiveSessions;

namespace AstraID.Application.Common.Mapping;

public static class MappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, UserDto>()
              .Map(dest => dest.DisplayName, src => src.DisplayNameRaw);

        config.NewConfig<Client, ClientDto>()
              .Map(dest => dest.Type, src => src.Type.ToString())
              .Map(dest => dest.Scopes, src => src.Scopes.Select(s => s.Value))
              .Map(dest => dest.RedirectUris, src => src.RedirectUris.Select(r => r.Value))
              .Map(dest => dest.PostLogoutRedirectUris, src => src.PostLogoutRedirectUris.Select(r => r.Value));

        config.NewConfig<UserConsent, ConsentDto>()
              .Map(dest => dest.Scopes, src => src.Scopes.Select(s => s.Value));

        config.NewConfig<UserSession, SessionDto>()
              .Map(dest => dest.DeviceId, src => src.DeviceId.Value)
              .Map(dest => dest.IpAddress, src => src.Ip.Value)
              .Map(dest => dest.UserAgent, src => src.Agent.Value);
    }
}
