# Security, Secret Management & Leak Prevention

## 1. Zero Hardcoding Mandate
- **Absolute Value Isolation:** Never write API keys, database connection strings, AWS IAM secret hashes, encryption tokens, or private third-party identifiers directly inside source code or layout metadata.
- **Dynamic Runtime Extraction:** All variable values must be pulled entirely from `process.env`.
- **Client Bundle Separation:** Only prefix environment variables with `NEXT_PUBLIC_` if they are structurally mandatory for client-side evaluation. All cloud database access parameters, serverless lambda keys, or internal API tokens must omit the prefix to prevent them from slipping into public client distribution scripts.
- **Git Shielding:** Never modify, bypass, or remove items from `.gitignore` to pass environment variables.

## 2. Multi-Tenant Resource Isolation
- **Tenant Context Verification:** Every database query, API route mutation, or record retrieval targeting the application database layer must pass explicit session authentication checks. Never rely on sequential, guessable, or predictable resource IDs.
- **Parameterized Scopes:** All operations writing data to DynamoDB or cloud storage backends must explicitly pass through parameterized validation checks to block unauthorized horizontal data bleed.
