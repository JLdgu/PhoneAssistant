# Copilot Instructions for PhoneAssistant

## Repository Overview
Phone Assistant is a .NET C# WPF desktop application for administering mobile phones and tablets. The repository also contains several .NET C# console utilities.

## Development Environment
Windows is required to build and develop the WPF application. Visual Studio 2022 or Visual Studio Code with the C# extension are supported options. Use PowerShell for repository build scripts.
When setting up a new worktree, configure `PhoneAssistant.WPF` as the startup project.

## Code Style and Conventions
- Treat `.editorconfig` and established repository patterns as the source of truth. Use 4-space indentation for C#, 2 spaces for XAML/XML, Allman braces, PascalCase for public members, and `I`-prefixed interface names.
- Avoid `this.` unless needed, prefer explicit types over `var` for built-in types, and use file-scoped namespaces in new or updated C# files.
- Declare new interfaces alongside their implementing class in that class's source file; do not create separate interface files.

## Testing
- Use TUnit for .NET tests. Do not add or migrate to another test framework unless explicitly requested.
- UAT means User Access Testing; API tests may be run against the UAT service without asking for permission.
- STG means Staging; ask the user for permission before running any API tests against the STG service.
- Before changing tests or test configuration, inspect the relevant target framework, SDK settings, test projects, package versions, CI commands, and existing conventions. Use APIs and runner configuration supported by the TUnit version in use; consult its official setup guidance rather than guessing.
- Preserve existing test intent, coverage, setup, teardown, categories, timeouts, and relevant parallelization. Keep changes focused on testing; do not change production behavior merely to make tests pass or weaken assertions.
- Keep tests discoverable and runnable through the repository's documented commands and CI. Configure the runner only when needed, preserving unrelated settings in `global.json`.
- Run the narrowest relevant tests, followed by any broader required checks. Report commands run and any failures or environment limitations.
- To filter TUnit tests, use `--treenode-filter` with the format `/<Assembly>/<Namespace>/<Class>/<Test>`; `*` matches any node. Use the repository's documented test command.
- If a requested test change conflicts with the existing setup, explain the conflict and make the smallest compatible change.
