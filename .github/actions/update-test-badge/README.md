# Update Test Results Badge Action

A reusable GitHub Action that updates test result badges by uploading test data to GitHub Gist files and displaying badge URLs for README files.

## Purpose

This action simplifies the process of maintaining dynamic test result badges by:

- Creating structured JSON data from test results
- Uploading the data to platform-specific files in a single GitHub Gist
- Providing ready-to-use badge URLs for documentation

## Usage

```yaml
- name: "Update Test Results Badge"
  uses: ./.github/actions/update-test-badge
  with:
    platform: "Linux"
    gist_id: "472c59b7c2a1898c48a29f3c88897c5a"
    filename: "test-results-linux.json"
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

## Gist Structure

This action uses a **single Gist** with **multiple files** for different platforms:

```
Gist ID: 472c59b7c2a1898c48a29f3c88897c5a
├── test-results-linux.json
├── test-results-windows.json
└── test-results-macos.json
```

## Inputs

| Input | Description | Required | Default |
|-------|-------------|----------|---------|
| `platform` | Platform name (Linux, Windows, macOS) | ✅ | - |
| `gist_id` | GitHub Gist ID for storing test results | ✅ | - |
| `filename` | Filename for platform-specific JSON (e.g., test-results-linux.json) | ✅ | - |
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

- **Gist File Update**: Updates the platform-specific file in the single Gist
- **Console Output**: Displays badge URLs ready for README usage
- **Debug Info**: Shows HTTP status and error details

## Generated JSON Format

The action creates JSON data in this format for each platform file:

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
- **File-specific updates**: Only updates the specific platform file, doesn't affect other platform data

## Integration with Badge API

This action is designed to work with the LocalStack .NET Client Badge API that:

- Reads from the updated Gist files
- Generates shields.io-compatible badge JSON
- Provides redirect endpoints to test result pages

## Matrix Integration Example

```yaml
env:
  BADGE_GIST_ID: "472c59b7c2a1898c48a29f3c88897c5a"

strategy:
  matrix:
    include:
      - os: ubuntu-22.04
        name: "Linux"
        filename: "test-results-linux.json"
      - os: windows-latest
        name: "Windows"
        filename: "test-results-windows.json"
      - os: macos-latest
        name: "macOS"
        filename: "test-results-macos.json"

steps:
  - name: "Update Test Results Badge"
    uses: ./.github/actions/update-test-badge
    with:
      platform: ${{ matrix.name }}
      gist_id: ${{ env.BADGE_GIST_ID }}
      filename: ${{ matrix.filename }}
      gist_token: ${{ secrets.GIST_SECRET }}
      # ... other inputs
```

## Required Setup

1. **Create single GitHub Gist** with platform-specific files:
   - `test-results-linux.json`
   - `test-results-windows.json`
   - `test-results-macos.json`
2. **Generate GitHub PAT** with `gist` scope
3. **Add to repository secrets** as `GIST_SECRET`
4. **Deploy Badge API** to consume the Gist data

## Badge URLs Generated

The action displays ready-to-use markdown for README files:

```markdown
[![Test Results (Linux)](https://your-api-domain/badge/tests/linux)](https://your-api-domain/redirect/tests/linux)
```

## Advantages of Explicit Filename Configuration

- ✅ **No String Manipulation**: Eliminates brittle string transformation logic
- ✅ **Declarative**: Filenames are explicitly declared in workflow configuration
- ✅ **Predictable**: No risk of unexpected filename generation
- ✅ **Reusable**: Action works with any filename structure
- ✅ **Debuggable**: Easy to see exactly what files will be created
- ✅ **Flexible**: Supports any naming convention without code changes

## Advantages of Single Gist Approach

- ✅ **Simplified Management**: One Gist to manage instead of three
- ✅ **Atomic Operations**: All platform data in one place
- ✅ **Better Organization**: Clear file structure with descriptive names
- ✅ **Easier Debugging**: Single location to check all test data
- ✅ **Cost Efficient**: Fewer API calls and resources

## Troubleshooting

**Common Issues:**

- **403 Forbidden**: Check `GIST_SECRET` permissions
- **404 Not Found**: Verify `gist_id` is correct
- **JSON Errors**: Ensure `jq` is available in runner
- **File Missing**: Gist files are created automatically on first update

**Debug Steps:**

1. Check action output for HTTP status codes
2. Verify Gist exists and is publicly accessible  
3. Confirm token has proper `gist` scope
4. Check individual file URLs: `https://gist.githubusercontent.com/{gist_id}/raw/test-results-{platform}.json`
