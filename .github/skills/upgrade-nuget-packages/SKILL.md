---
name: upgrade-nuget-packages
description: Upgrade all repository NuGet packages to their latest stable versions, validate the full solution, and open a pull request.
---

# Upgrade NuGet packages

Use this workflow when asked to upgrade or update the repository's NuGet packages. Do not run it merely because the skill is installed.

## Prepare

1. Inspect the worktree and current branch. Do not overwrite or include unrelated changes. If uncommitted changes prevent safely switching branches, stop and ask the user how to proceed.
2. Identify the repository's default branch and fetch its latest state. Create a uniquely named `chore/upgrade-nuget-packages-*` branch from the updated remote default branch, without modifying an existing branch.
3. Inspect the current solution, project files, NuGet configuration and package version files. This repository currently uses `Directory.Packages.props` for centrally managed versions, and both projects target .NET 10. Check the actual files and installed SDK rather than relying on older README or Copilot instruction version references.

## Upgrade

1. Inventory all explicitly declared package versions, including central `PackageVersion` entries, project-level versions or overrides, and any other NuGet version files. Use the repository's configured package sources and credentials; never expose credentials in output or commit them.
2. Determine the latest **stable** version of each declared package from its permitted configured sources, including major releases. For example, use `dotnet package search <package-id> --exact-match --format json` and `dotnet package list --outdated` to investigate. Check source errors and package-source mappings; an incomplete or failed lookup is not evidence that a package is current. Do not select prereleases unless the user explicitly requests them.
3. Update all declarations to their latest stable versions, preserving central package management and consistent versions across projects. Update existing lock files if applicable. Transitive packages should be updated through normal dependency resolution or existing central pins, not promoted to direct dependencies solely to force an upgrade.
4. Make only changes necessary for the upgrade, including compatibility fixes and focused tests where required. Preserve the library's behavior, DI wiring, and distributed nonce handling. Do not silently leave a package behind because the newest version is incompatible: resolve the incompatibility or stop and report the blocker.
5. Restore and recheck the package inventory against the latest versions found. If every declaration was already current and no changes are needed, report that and stop without creating an empty commit or PR.

## Validate and publish

1. The integration tests in `tests/BlockChainTools.Tests/BlockChainToolsTests.cs` use Testcontainers and Redis. Check `docker info` and ensure a usable Docker daemon is available; do not skip integration tests or start or alter shared infrastructure without authorization. If a prerequisite or package source is unavailable, stop and report it.
2. Run the full CI-equivalent sequence from the repository root, and require every command to succeed:

   ```sh
   dotnet restore BlockChainTools.sln
   dotnet build BlockChainTools.sln --configuration Release --no-restore
   dotnet test BlockChainTools.sln --configuration Release --no-build --verbosity normal
   ```

   Confirm that the test runner actually discovered and executed tests, including the Redis integration tests. Fix upgrade-related failures and rerun the necessary checks; never create a PR if restore, build, or the full test suite fails or is skipped.
3. Review the diff for unrelated changes, secrets, and unintended dependency changes. Commit only the upgrade and required compatibility changes, following repository and session commit instructions.
4. Push the new branch. After the push succeeds, check for a repository PR template and follow it. Create a pull request targeting the default branch using the available PR creation mechanism, respecting any required user confirmation. Summarize the package upgrades, breaking or compatibility changes, and validation results. Report the PR or any publication blocker; do not claim a PR was created unless creation succeeds.
