# Security, Save Verification & Anti-Tampering Protocols

## 1. Zero Local Plaintext Mandate
- **Save File Isolation:** Never write persistent player profile progress, transaction variables, or unlocking keys to local disk streams (`Application.persistentDataPath`) in plaintext JSON or standard binary serialization formats.
- **Cryptographic Obfuscation:** All local save loops must utilize AES-256 or equivalent hardware-compatible encryption wrappers. Use custom salt strings derived from unique device identifiers (`SystemInfo.deviceUniqueIdentifier`).
- **Memory Tampering Mitigation:** Critical runtime variables (e.g., player health, virtual currencies, inventory balances) must not sit as naked primitives in memory. Wrap them in basic bitwise obfuscation layers or anti-cheat variable frameworks to block runtime memory scanners.

## 2. Server Boundary & Secret Protection
- **No In-Editor Key Baking:** Never hardcode cloud endpoint tokens, production encryption keys, or Unity Gaming Services (UGS) developer credentials into Monobehaviour inspectors or script constants.
- **External Secret Injection:** Pull configuration hashes dynamically at runtime via secure text assets, server-driven configuration streams, or local runtime command-line arguments.
- **Network Validation:** Enforce strict cryptographic validation (HTTPS/SSL pinning) on all outgoing network payload streams communicating with external backend APIs or state layers.