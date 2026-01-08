# Release Process for dotnet-core-uninstall Tool

This document outlines the complete release process for the dotnet-core-uninstall tool.

## Prerequisites

Before starting the release process, ensure you have:
- Access to the internal Azure DevOps pipeline `dotnet-cli-lab`
- Permissions to create releases on the [GitHub releases page](https://github.com/dotnet/cli-lab/releases)
- Access to communicate with the vendor team for manual signoff

## Release Steps

### 1. Identify the Build

1. Navigate to the internal Azure DevOps pipeline named `dotnet-cli-lab`
2. Locate the build you want to release
   - Ensure the build has completed successfully
   - Verify all build artifacts are available
   - Note the build number for reference

### 2. Collect Installer Artifacts

From the selected build's artifacts node, grab the installer links for:

- **macOS ARM64**: `drop-osx-arm64` artifact containing the `.tar.gz` file
- **macOS x64**: `drop-osx-x64` artifact containing the `.tar.gz` file
- **Windows MSI**: `drop-windows` artifact containing the `.msi` file

**Important**: Download or copy the direct links to these artifacts for the next steps.

### 3. Vendor Team Handoff for Manual Signoff

1. Send the installer artifacts to the vendor team for manual signoff
2. Wait for confirmation from the vendor team that all artifacts have been reviewed and approved
3. Address any issues or concerns raised during the signoff process

### 4. Determine the Version Number

The version number for the release can be found in the build's binary log (binlog):

1. Download the binlog from the build artifacts
2. Open the binlog file using a binlog viewer
3. Locate the version information (format: `Major.Minor.Patch`, e.g., `1.7.0`)
4. Record this version number for the next steps

**Alternative**: The version can also be determined from the `eng/Versions.props` file in the commit that triggered the build.

### 5. Create a Git Tag

Create a git tag matching the version number from the build:

```bash
# Checkout the commit that was built
git checkout <commit-sha-from-build>

# Create and push the tag
git tag <version>
git push origin <version>
```

Example:
```bash
git checkout abc1234
git tag 1.7.0
git push origin 1.7.0
```

### 6. Create a Draft Release on GitHub

1. Go to the [GitHub releases page](https://github.com/dotnet/cli-lab/releases)
2. Click "Draft a new release"
3. Copy content from the most recent release as a template

### 7. Update the Release Details

Update the following in the draft release:

#### a. Release Title
- Update the title to match the version number (e.g., "dotnet-core-uninstall v1.7.0")

#### b. Tag
- Select or enter the tag you created in Step 5 (e.g., `1.7.0`)
- Ensure the tag points to the correct commit from the build

#### c. Release Notes
Update the release notes to include:
- A summary of what's being fixed or changed in this release
- List of notable bug fixes
- List of new features (if any)
- Any breaking changes or important notices

#### d. Commit Diff Link
Update the commit diff link to compare the previous release tag to the new release tag:

```
**Full Changelog**: https://github.com/dotnet/cli-lab/compare/<previous-tag>...<new-tag>
```

Example:
```
**Full Changelog**: https://github.com/dotnet/cli-lab/compare/1.6.0...1.7.0
```

#### e. Upload Artifacts
Attach the following artifacts to the release:
- Windows MSI file (from `drop-windows` artifact)
- macOS ARM64 tar.gz file (from `drop-osx-arm64` artifact)
- macOS x64 tar.gz file (from `drop-osx-x64` artifact)

GitHub will automatically generate source code archives (zip and tar.gz).

### 8. Get Review

1. Save the draft release (do not publish yet)
2. Share the draft release link with the team for review
3. Request review from at least one team member with release permissions
4. Address any feedback on the release notes or artifacts

### 9. Publish the Release

Once the review is approved:

1. Return to the draft release
2. Make any final adjustments based on review feedback
3. Click "Publish release"
4. Verify the release appears on the releases page
5. Notify relevant stakeholders that the release is live

## Post-Release Verification

After publishing:

1. Verify the release appears correctly on the [releases page](https://github.com/dotnet/cli-lab/releases)
2. Test downloading and installing artifacts from at least one platform
3. Update any related documentation or announcements
4. Monitor for any issues reported by early adopters

## Notes

- Always use semantic versioning (Major.Minor.Patch)
- Keep release notes clear and user-focused
- Ensure all artifacts are properly signed before release
- Maintain consistency in release note formatting with previous releases

## Troubleshooting

### Build artifacts are missing
- Check that the build completed successfully in the pipeline
- Verify you have the correct permissions to access the artifacts
- Contact the build team if artifacts are not being generated

### Tag already exists
- Use `git tag -d <tag>` to delete the local tag
- Use `git push origin --delete <tag>` to delete the remote tag
- Recreate the tag with the correct commit

### Release notes are unclear
- Reference previous releases for formatting examples
- Focus on user-facing changes
- Include links to issues or PRs for detailed information

## Additional Resources

- [GitHub Releases Documentation](https://docs.github.com/en/repositories/releasing-projects-on-github)
- [Semantic Versioning](https://semver.org/)
- [dotnet-core-uninstall README](./README.md)
