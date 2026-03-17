# Security Policy

## Supported Versions

We actively support the following versions of AgriscaleContainer:

| Version | Supported          | Notes                          |
| ------- | ------------------ | ------------------------------ |
| 1.x.x   | :white_check_mark: | Current stable release         |
| < 1.0   | :x:                | Pre-release, not supported     |

## Reporting a Vulnerability

We take the security of AgriscaleContainer seriously. If you discover a security vulnerability, please follow these steps:

### 1. **Do NOT** publicly disclose the vulnerability

Please do not create a public GitHub issue for security vulnerabilities.

### 2. Report privately

Send an email to: **cyrille_ahmed.midingoyi@cirad.fr**

Include the following information:
- Description of the vulnerability
- Steps to reproduce the issue
- Potential impact
- Suggested fix (if you have one)
- Your contact information

### 3. What to expect

- **Acknowledgment**: We will acknowledge receipt of your report within **48 hours**
- **Assessment**: We will assess the vulnerability and determine its severity within **5 business days**
- **Fix**: Critical vulnerabilities will be addressed in a security patch as soon as possible
- **Credit**: If you wish, we will credit you in the security advisory and changelog

## Security Considerations

### Container Infrastructure

AgriscaleContainer provides a build system and containerization framework. Security considerations include:

1. **Build Dependencies**: Ensure you trust all build dependencies (.NET SDK, compilers, etc.)
2. **Container Runtime**: Keep Docker/Singularity updated to the latest secure versions
3. **Sensitive Data**: Never include sensitive data (credentials, API keys) in container images

### Third-Party Models

The included crop models (APSIM, DSSAT, STICS) are maintained by their respective organizations:

- **APSIM Security**: Report to apsim@csiro.au
- **DSSAT Security**: Report via https://github.com/DSSAT/dssat-csm-os/security
- **STICS Security**: Contact INRAE directly

We are not responsible for security issues in third-party model code.

## Security Best Practices

When using AgriscaleContainer:

### For Users

1. **Verify Downloads**: Check checksums for downloaded releases
2. **Keep Updated**: Use the latest stable version
3. **Scan Images**: Scan container images for vulnerabilities before deployment
4. **Minimal Permissions**: Run containers with minimal required permissions
5. **Network Security**: Restrict network access as appropriate for your use case

### For Contributors

1. **Dependency Review**: Carefully review any new dependencies
2. **Code Review**: All PRs undergo security-focused code review
3. **License Compliance**: Ensure contributed code complies with licenses
4. **No Secrets**: Never commit secrets, API keys, or credentials
5. **Signed Commits**: Consider using GPG-signed commits

## Known Security Considerations

### Model Execution

Crop simulation models may:
- Read/write files in working directories
- Consume significant computational resources
- Process user-supplied input files

**Recommendation**: Run models with appropriate resource limits and in isolated environments when processing untrusted input.

### Build Process

The build process:
- Downloads and compiles code from submodules
- Executes build scripts
- May require elevated permissions for package installation

**Recommendation**: Review build scripts and only build from trusted sources.

## Vulnerability Disclosure Policy

When we address a security vulnerability:

1. **Patch Development**: We develop and test a fix
2. **Security Advisory**: We publish a GitHub Security Advisory
3. **Release**: We release a patched version
4. **Public Disclosure**: We publicly disclose the vulnerability after users have had time to update (typically 7-14 days)
5. **CVE**: For critical vulnerabilities, we may request a CVE identifier

## Dependencies and Supply Chain

### Direct Dependencies

- **.NET SDK**: Official Microsoft releases
- **Docker/Singularity**: Official container runtimes
- **Compilers**: GFortran from official repositories

### Transitive Dependencies

Monitor for vulnerabilities in:
- .NET package dependencies (watch for NuGet security advisories)
- System packages installed via apt/yum
- Git submodules (APSIM, DSSAT, STICS)

### Submodule Security

Third-party models are included as Git submodules or archives:
- **APSIM**: https://github.com/APSIMInitiative/ApsimX
- **DSSAT**: https://github.com/DSSAT/dssat-csm-os
- **STICS**: Vendored archive (JavaSticsInstall.zip)

We do not audit the security of third-party model code. Refer to their respective security policies.

## Automated Security Scanning

We use:
- GitHub Dependabot for dependency vulnerability alerts
- GitHub Advanced Security (when available)
- CodeQL static analysis (planned)

## Security Contact

For urgent security matters: **cyrille_ahmed.midingoyi@cirad.fr**

For general inquiries: https://github.com/YOUR_USERNAME/AgriscaleContainer/issues

---

**Last Updated**: March 17, 2026
