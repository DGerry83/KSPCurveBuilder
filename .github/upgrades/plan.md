# Upgrade Plan: KSPCurveBuilder ? .NET 10.0 (windows)

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Risk Management & Mitigation](#risk-management--mitigation)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

Selected Strategy
**All-At-Once Strategy** — All projects in the solution (single project) will be upgraded simultaneously in a single atomic operation.

Rationale:
- Solution size: 1 project (small)
- Project type: Classic WinForms application targeting `net48` that must be converted to SDK-style and target `net10.0-windows`.
- Assessment findings: No NuGet packages declared; the assessment reports a high number of Windows Forms and System.Drawing API incompatibilities that will be identified and resolved during a single coordinated build pass.

Scope:
- Project: `C:\Users\Matt\source\repos\KSPCurveBuilder\KSPCurveBuilder.csproj`
- Target framework: `net10.0-windows`
- SDK: `Microsoft.NET.Sdk.WindowsDesktop` with `<UseWindowsForms>true</UseWindowsForms>`
- Branch: `upgrade-to-NET10` (created)

Key deliverable: A single atomic upgrade that results in the solution building under .NET 10 with updated project files, package references (if required), and resolved compilation issues.

## Migration Strategy

Approach: All-At-Once
- Convert the project to SDK-style and update all project-level settings and package references in one coordinated commit.
- Perform dependency updates, restore, build, and fix compilation errors as a single bounded operation (TASK-001).
- Run test-validation phase after the atomic upgrade (TASK-002).

Phases (logical):
- Phase 0: Prerequisites (SDK validation, global.json review)
- Phase 1: Atomic Upgrade (project file conversion, package updates, restore, build, fix compile errors)
- Phase 2: Test Validation (run tests and address failures)

Planned tasks (for execution stage):
- TASK-000: Prerequisites — validate SDK, handle pending changes, ensure correct branch
- TASK-001: Atomic framework + package upgrade — update all projects, update packages, restore, build and fix compilation errors (single pass)
- TASK-002: Test execution and validation — run tests and resolve test failures

## Detailed Dependency Analysis

Summary:
- Single-project solution; there are no project-to-project dependencies.
- The project is a Classic WinForms application and depends on Windows Forms and System.Drawing APIs.
- Assessment shows many API compatibility issues concentrated in WinForms and drawing APIs.

Implications:
- No dependency ordering constraints beyond converting the single project.
- All updates can be applied atomically in the scope of this one project.

## Project-by-Project Plans

### Project: `KSPCurveBuilder.csproj`

Current State:
- Path: `C:\Users\Matt\source\repos\KSPCurveBuilder\KSPCurveBuilder.csproj`
- Current Target Framework: `net48`
- SDK-style: False
- Project type: Classic WinForms
- Files: ~18, LOC: ~2646
- Assessment findings: ~969 binary-incompatible API usages and ~136 source-incompatible usages (see assessment)

Target State:
- Target Framework: `net10.0-windows`
- Project style: SDK-style with `Sdk="Microsoft.NET.Sdk.WindowsDesktop"`
- Property: `<UseWindowsForms>true</UseWindowsForms>`

Migration Steps (atomic operation):
1. Prerequisites (TASK-000):
   - Verify .NET 10 SDK is installed. If `global.json` is present, confirm compatibility or update accordingly.
   - Ensure working tree is clean on `upgrade-to-NET10` branch.
2. Convert `.csproj` to SDK-style:
   - Replace legacy project XML with SDK-style header: `<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">` and a `PropertyGroup` with `<TargetFramework>net10.0-windows</TargetFramework>` and `<UseWindowsForms>true</UseWindowsForms>`.
   - Preserve any custom MSBuild targets/imports and any non-standard file includes.
3. Assembly attributes and `AssemblyInfo`:
   - Inspect `Properties\AssemblyInfo.cs`. Remove attributes already provided by the SDK or migrate them to the csproj as MSBuild properties to avoid duplicate attribute generation.
4. Startup pattern & `Program.cs`:
   - Update startup to modern WinForms pattern used in .NET 6+ templates if needed:
     - `ApplicationConfiguration.Initialize();` then `Application.Run(new KSPCurveBuilder());`
   - Alternatively retain explicit calls to `Application.SetHighDpiMode(...)`, `Application.EnableVisualStyles()`, `Application.SetCompatibleTextRenderingDefault(false)` if required.
5. Designer & resources:
   - Address designer-generated code issues that surface after conversion. Fix fully-qualified type names or namespace changes if reported by the build.
6. Package/reference adjustments:
   - Assessment lists no NuGet packages. Add `System.Drawing.Common` if required by compilation for GDI+ usage, or retain framework assemblies if the SDK provides them.
   - Identify usage of removed legacy controls (StatusBar, MainMenu, MenuItem, ToolBar) and plan for replacements (ToolStrip, MenuStrip, ContextMenuStrip, DataGridView) if they exist.
7. Restore & build (atomic):
   - Run `dotnet restore` and `dotnet build`.
   - Fix all compilation errors discovered during the build as part of the atomic operation.
8. Validation after build:
   - Confirm `dotnet build` completes with 0 errors. Verify designer files compile and primary UI flows start without immediate runtime crashes.

Validation Checklist:
- [ ] `TargetFramework` set to `net10.0-windows` in csproj
- [ ] Project converted to SDK-style with `Microsoft.NET.Sdk.WindowsDesktop`
- [ ] `<UseWindowsForms>true</UseWindowsForms>` present
- [ ] Duplicate assembly attributes removed or migrated
- [ ] `dotnet build` completes with 0 errors
- [ ] Designer files compile without errors

## Package Update Reference

Summary:
- Assessment found no declared NuGet packages to update.
- After conversion, the following package additions are commonly required; include only if `dotnet build` indicates missing APIs:
  - `System.Drawing.Common` — Suggested when System.Drawing APIs are used extensively. Consider platform implications.

If any packages are added or updated during TASK-001, record exact current and target versions here.

## Breaking Changes Catalog

High-impact areas (from assessment):
- Windows Forms API binary incompatibilities across many controls and members (Button, DataGridView, PictureBox, ComboBox, TextBox, CheckBox, etc.).
- Control collections, layout properties, event handler signatures may require source changes.
- System.Drawing API changes; consider `System.Drawing.Common` or port to alternative graphics libraries if cross-platform is desired.
- Designer/resource serialization differences that may surface when upgrading designer files or opening forms in newer Visual Studio.
- Legacy controls removed in modern .NET WinForms: StatusBar, MainMenu, MenuItem, ToolBar, ContextMenu — replace with modern equivalents if present.

Remediation patterns:
- Replace removed controls with supported alternatives where feasible.
- Update event handler signatures and using directives to match new API surface.
- Open forms in Visual Studio to re-generate designer code if needed and fix any resource or type reference differences.

?? Many of these items will be discovered by the compiler or designer and require developer judgment during the atomic upgrade. Document each fix with file and code references.

## Testing & Validation Strategy

Automated tests:
- No test projects were discovered in the assessment. If tests exist, include them in TASK-002.

Validation after atomic upgrade:
- Automated: `dotnet build` must succeed with 0 errors.
- Automated: Run any unit/integration tests (if present) and require passing results.
- Manual: Start the WinForms app and exercise main flows (non-automatable, documented as recommended manual validation).

Artifacts to collect:
- Build log showing successful build
- Test run results (if applicable)
- A list of code changes applied to fix compilation and designer issues

## Risk Management & Mitigation

Top risks:
- High count of WinForms API incompatibilities (High)
- Designer and resource mismatches causing broken forms (Medium-High)

Mitigations:
- Work on branch `upgrade-to-NET10` and keep backups
- Apply atomic upgrade in single commit to simplify rollback
- Use latest Visual Studio and .NET 10 SDK when resolving designer issues
- Prioritize build fixes discovered in TASK-001 and isolate complex UI issues for pair debugging

Contingency:
- If blocking designer/runtime issues appear that cannot be resolved in the atomic pass, create targeted follow-up branches to refactor UI components and revert the main upgrade commit if needed.

## Complexity & Effort Assessment

Project complexity (relative):
- `KSPCurveBuilder.csproj` — Medium-High (WinForms-heavy code, designer sensitivity)

Indicators:
- LOC impacted: ~1105+ (assessment)
- API incompatibilities: ~1105 (binary + source)

## Source Control Strategy

All-At-Once guidelines:
- Use a single atomic commit for the core upgrade changes where feasible. That commit should include the converted `.csproj`, updated startup code, assembly attribute changes, package reference updates, and compilation fixes.

Suggested procedure:
1. Ensure working tree clean on branch `upgrade-to-NET10`.
2. Make all code and project file changes locally.
3. Commit all changes in a single commit with message: `Upgrade: Convert project to net10.0-windows (SDK-style) and apply compatibility fixes`.
4. Push branch and open a PR for review.

Rationale: A single atomic upgrade reduces intermediate inconsistent states and follows the All-At-Once approach.

## Success Criteria

The migration is complete when all of the following are true:
- Project targets `net10.0-windows`.
- Project file is SDK-style using `Microsoft.NET.Sdk.WindowsDesktop` and `<UseWindowsForms>true</UseWindowsForms>`.
- `dotnet build` completes with 0 errors for the solution.
- Designer files compile without errors and primary UI flows are validated (manual checks).
- Any added/updated NuGet packages are recorded and have no unresolved security vulnerabilities.

---

Notes & open questions:
- The assessment flagged many WinForms API incompatibilities. Specific code fixes will be discovered during the atomic build pass and must be resolved in the same operation.
- If additional NuGet packages are required after conversion, include them in TASK-001 and record exact versions in the Package Update Reference.
