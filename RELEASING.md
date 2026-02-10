# Release Process

This document describes the process for releasing new versions of the Ave.Extensions.ErrorPaths packages.

## Release with GitHub Tags

1. **Decide on a version number**:

   Follow [Semantic Versioning](https://semver.org/):
   - MAJOR version when you make incompatible API changes
   - MINOR version when you add functionality in a backward compatible manner
   - PATCH version when you make backward compatible bug fixes
   - Pre-release versions with suffixes like `-preview.1`, `-beta.2`, etc.

2. **Create and push a tag**:

   ```bash
   git tag 1.0.0
   git push origin 1.0.0
   ```

   For pre-release versions:
   ```bash
   git tag 1.0.0-preview.1
   git push origin 1.0.0-preview.1
   ```

3. **Monitor the release process**:

   The GitHub Actions workflow will automatically:
   - Build the code
   - Run tests
   - Generate NuGet packages with the proper version number
   - Create a GitHub Release
   - Attach NuGet packages to the release

4. **Publish to NuGet.org**:

   Trigger the "Publish to NuGet" workflow manually from the Actions tab.

## Release with Version Labels (for Pull Requests)

1. **Add version label to PR**:

   Add a label in the format `version/1.0.0` or `version/1.0.0-preview.1` to your PR.

2. **Verify build and packages**:

   The GitHub Actions workflow will:
   - Build the code with the specified version
   - Run tests
   - Generate NuGet packages as artifacts
   - Comment on the PR with a link to the artifacts

3. **Review packages**:

   Download the packages from the workflow artifacts and verify them before merging.

4. **After merging, create a release**:

   Follow the "Release with GitHub Tags" process above to create the official release.

## Configuring NuGet.org Publishing

To enable publishing to NuGet.org:

1. Generate a NuGet API key at https://www.nuget.org/account/apikeys

2. Add it as a repository secret in GitHub:
   - Go to your repository settings
   - Select "Secrets and variables" > "Actions"
   - Add a new secret named `NUGET_API_KEY` with your API key as the value

3. Trigger the "Publish to NuGet" workflow from the Actions tab after a release is created.
