# AstraID Server – Test Plan

## Unit Tests
| Area | Scenario | Expected Result |
|---|---|---|
|Domain|`AppUser.Register` emits `UserRegistered`|Domain event in `DomainEvents` collection|
|Domain|`Client.RotateSecret` deactivates previous secret|Active history entry replaced|
|Application|ValidationBehavior rejects invalid `RegisterUserCommand`|`ValidationException` thrown|
|Application|AuthorizationBehavior denies missing scopes|`UnauthorizedAccessException` thrown|

## Integration Tests
| Scenario | Steps | Pass/Fail Criteria |
|---|---|---|
|User registration API|POST `/api/users` with valid payload|201 Created and user persisted|
|Client secret rotation API|POST `/api/clients/{id}/rotate-secret`|204 No Content and secret history updated|
|Outbox publishing|Trigger domain event and run hosted service|Outbox row marked processed and handler invoked|

## End-to-End (OAuth/OpenID)
| Flow | Steps | Expected |
|---|---|---|
|Authorization Code + PKCE|1. GET `/connect/authorize` with code_challenge<br>2. POST `/connect/token` with code_verifier|Access token issued; refresh token returned|
|Client Credentials|POST `/connect/token` with `client_id`/`client_secret`|Access token issued|

### Pass/Fail Criteria
- All unit and integration tests execute without unhandled exceptions.
- For OAuth flows, responses follow RFC specifications (HTTP 302 for authorize, 200 with tokens for token endpoint) and scopes/claims match requests.
