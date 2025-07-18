# Update Test Results Badge Action

A reusable GitHub Action that updates test result badges by uploading test data to GitHub Gists and displaying badge URLs for README files.

## Purpose

This action simplifies the process of maintaining dynamic test result badges by:

- Creating structured JSON data from test results
- Uploading the data to platform-specific GitHub Gists
- Providing ready-to-use badge URLs for documentation

## Usage

```yaml
- name: "Update Test Results Badge"
  uses: ./.github/actions/update-test-badge
  with:
    platform: "Linux"
    gist_id: "c149767013f99f00791256d9036ef71b"
    gist_token: ${{ secrets.GIST_SECRET }}
    test_passed: 1099
    test_failed: 0
    test_skipped: 0
    test_url_html: "https://github.com/owner/repo/runs/12345"
    commit_sha: ${{ github.sha }}
    run_id: ${{ github.run_id }}
    repository: ${{ github.repository }}
    server_url: ${{ github.server_url }}
```

## Inputs

| Input | Description | Required | Default |
|-------|-------------|----------|---------|
| `platform` | Platform name (Linux, Windows, macOS) | ✅ | - |
| `gist_id` | GitHub Gist ID for storing test results | ✅ | - |
| `gist_token` | GitHub token with gist permissions | ✅ | - |
| `test_passed` | Number of passed tests | ✅ | - |
| `test_failed` | Number of failed tests | ✅ | - |
| `test_skipped` | Number of skipped tests | ✅ | - |
| `test_url_html` | URL to test results page | ❌ | `''` |
| `commit_sha` | Git commit SHA | ✅ | - |
| `run_id` | GitHub Actions run ID | ✅ | - |
| `repository` | Repository in owner/repo format | ✅ | - |
| `server_url` | GitHub server URL | ✅ | - |
| `api_domain` | Badge API domain for URLs | ❌ | `your-api-domain` |

## Outputs

This action produces:

- **Gist Update**: Updates the specified Gist with test result JSON
- **Console Output**: Displays badge URLs ready for README usage
- **Debug Info**: Shows HTTP status and error details

## Generated JSON Format

The action creates JSON data in this format:

```json
{
  "platform": "Linux",
  "passed": 1099,
  "failed": 0,
  "skipped": 0,
  "total": 1099,
  "url_html": "https://github.com/owner/repo/runs/12345",
  "timestamp": "2025-01-16T10:30:00Z",
  "commit": "abc123def456",
  "run_id": "12345678",
  "workflow_run_url": "https://github.com/owner/repo/actions/runs/12345678"
}
```

## Error Handling

- **Non-essential**: Uses `continue-on-error: true` to prevent workflow failures
- **Graceful degradation**: Provides detailed error messages without stopping execution
- **HTTP status reporting**: Shows API response codes for debugging

## Integration with Badge API

This action is designed to work with the LocalStack .NET Client Badge API that:

- Reads from the updated Gists
- Generates shields.io-compatible badge JSON
- Provides redirect endpoints to test result pages

## Matrix Integration Example

```yaml
strategy:
  matrix:
    include:
      - os: ubuntu-22.04
        name: "Linux"
        gist_id: "c149767013f99f00791256d9036ef71b"
      - os: windows-latest
        name: "Windows"
        gist_id: "3640d86bbf37520844f737e6a76b4d90"
      - os: macos-latest
        name: "macOS"
        gist_id: "db58d93cf17ee5db079d06e3bfa4c069"

steps:
  - name: "Update Test Results Badge"
    uses: ./.github/actions/update-test-badge
    with:
      platform: ${{ matrix.name }}
      gist_id: ${{ matrix.gist_id }}
      gist_token: ${{ secrets.GIST_SECRET }}
      # ... other inputs
```

## Required Setup

1. **Create GitHub Gists** for each platform
2. **Generate GitHub PAT** with `gist` scope
3. **Add to repository secrets** as `GIST_TOKEN`
4. **Deploy Badge API** to consume the Gist data

## Badge URLs Generated

The action displays ready-to-use markdown for README files:

```markdown
[![Test Results (Linux)](https://your-api-domain/badge/tests/linux)](https://your-api-domain/redirect/tests/linux)
```

## Troubleshooting

**Common Issues:**

- **403 Forbidden**: Check `GIST_TOKEN` permissions
- **404 Not Found**: Verify `gist_id` is correct
- **JSON Errors**: Ensure `jq` is available in runner

**Debug Steps:**

1. Check action output for HTTP status codes
2. Verify Gist exists and is publicly accessible  
3. Confirm token has proper `gist` scope
4. Test Gist update manually with curl
