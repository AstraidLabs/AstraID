# AstraID Server – Requirements Traceability

| Requirement / Goal | Implementation | Evidence |
|---|---|---|
|Emit domain events for user lifecycle|`AppUser` aggregate raises events|`UserRegistered`, `PasswordChanged` etc.【F:src/AstraID.Domain/Entities/AppUser.cs†L1-L112】【F:src/AstraID.Domain/Entities/AppUser.cs†L195-L210】|
|Capture domain events to outbox|SaveChanges interceptor|DomainEventsCollectorInterceptor writes outbox messages【F:src/AstraID.Infrastructure/Persistence/Interceptors/DomainEventsCollectorInterceptor.cs†L1-L44】|
|Publish outbox messages|Hosted service with `OutboxPublisher`|OutboxHostedService polling & dispatch【F:src/AstraID.Infrastructure/Messaging/Background/OutboxHostedService.cs†L1-L27】|
|Enforce Authorization via scopes/roles|`AuthorizationBehavior` + policies|Behavior checks scopes/roles; API policies【F:src/AstraID.Application/Behaviors/AuthorizationBehavior.cs†L1-L24】【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L24-L33】|
|Expose OAuth/OIDC endpoints|OpenIddict server config|Authorization/token/introspection endpoints and grant types【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L22-L37】|
|Provide user management API|Minimal endpoints for register/get|ApplicationEndpoints.Users maps routes to handlers【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Users.cs†L13-L34】|
|Provide client management API|Register, rotate secret, get client|ApplicationEndpoints.Clients handlers【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Clients.cs†L13-L40】|
|Health monitoring|Liveness and readiness checks|HealthChecksConfig mapping【F:src/AstraID.Api/Health/HealthChecksConfig.cs†L15-L23】|

---

See also: [Feature Inventory](AstraID_FeatureInventory.md) · [Capabilities](AstraID_Capabilities.md)
