# AstraID Test Plan

## Unit Tests

| Area | Scenario | Expectation |
| ---- | -------- | ----------- |
| Options Validation | Non-HTTPS issuer | Validation fails |
| Options Validation | HTTP CORS origin in production | Validation fails |
| Connection Strings | Empty default connection string | Validation fails |
| Auth Options | Missing introspection secret | Validation fails |
| Auth Options | Missing certificate file | Validation fails |
| Password Policy | Weak passwords (length/complexity) | Error returned |
| Password Policy | Strong password | Accepted |
| Password History | Reused hash | Throws exception |
| Password History | New hash | Allowed |

## Integration Tests

| Area | Scenario | Expectation |
| ---- | -------- | ----------- |
| Discovery | `/.well-known/openid-configuration` | Returns issuer |
| JWKS | `/connect/jwks` | Returns key set |
| Security Headers | `/health/live` | Standard headers present |

## Security Tests

| Area | Scenario | Expectation |
| ---- | -------- | ----------- |
| Token tampering | Modify JWT payload without resigning | Validation fails |

## Load Tests (Skeleton)

Placeholder for future load tests targeting `/connect/token`.
