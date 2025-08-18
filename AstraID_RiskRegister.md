# AstraID Server – Risk Register

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
|Domain entities coupled to EF Core|Medium|Medium|Introduce separate persistence models or abstractions|
|Missing UserInfo/logout endpoints|High|High|Implement remaining OpenID Connect specs and add integration tests|
|Outbox publisher reprocessing on failure|Medium|Medium|Add deduplication/idempotency keys or max-attempts policy|
|PII in logs|Medium|High|Configure Serilog filters; avoid logging sensitive fields|
|Weak secret management|Medium|High|Store Data Protection keys in external store; ensure secrets hashed and rotated|
|Insufficient tests|High|High|Establish CI pipeline executing unit/integration/OIDC E2E tests|
