# KSPCurveBuilder .NET 10.0 Upgrade Tasks

## Overview

This document lists executable tasks to upgrade `KSPCurveBuilder` to .NET 10.0 on Windows. The execution includes prerequisites verification, a single atomic project conversion with compilation fixes, and automated test execution if test projects exist.

**Progress**: 3/3 tasks complete (100%) ![0%](https://progress-bar.xyz/100)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-01-18 00:09)*
**References**: Plan §Phase 0, Plan §Prerequisites

- [✓] (1) Verify required .NET 10 SDK is installed on the environment per Plan §Phase 0
- [✓] (2) Runtime/SDK version meets minimum requirements (**Verify**)
- [✓] (3) If a `global.json` file exists, confirm compatibility with .NET 10 or update per Plan §Phase 0
- [✓] (4) Check configuration/MSBuild files (e.g., `Directory.Build.props`, `Directory.Packages.props`) for compatibility with target SDK per Plan §Phase 0
- [✓] (5) Configuration files compatible with target version (**Verify**)

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-01-18 00:18)*
**References**: Plan §Phase 1, Plan §Project-by-Project Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Convert `KSPCurveBuilder.csproj` to SDK-style with `Sdk="Microsoft.NET.Sdk.WindowsDesktop"`, set `<TargetFramework>net10.0-windows</TargetFramework>` and `<UseWindowsForms>true</UseWindowsForms>` per Plan §Project-by-Project Plans
- [✓] (2) Migrate or remove duplicate assembly attributes from `Properties\AssemblyInfo.cs` and/or move them into the csproj as MSBuild properties per Plan §Project-by-Project Plans
- [✓] (3) Update startup/`Program.cs` patterns and any required WinForms initialization per Plan §Project-by-Project Plans
- [✓] (4) Add or update package references if build indicates missing APIs (e.g., `System.Drawing.Common`) per Plan §Package Update Reference
- [✓] (5) Restore dependencies (`dotnet restore`) per Plan §Phase 1
- [✓] (6) Build solution to identify compilation errors (`dotnet build`) per Plan §Phase 1
- [✓] (7) Fix all compilation errors found, addressing WinForms/System.Drawing breaking changes per Plan §Breaking Changes Catalog
- [✓] (8) Rebuild solution to verify fixes
- [✓] (9) Solution builds with 0 errors (**Verify**)
- [✓] (10) Commit all changes with message: "TASK-002: Convert project to net10.0-windows (SDK-style) and apply compatibility fixes"

### [✓] TASK-003: Run tests and validate upgrade *(Completed: 2026-01-18 00:20)*
**References**: Plan §Phase 2, Plan §Testing & Validation Strategy

- [✓] (1) Confirm presence of any test projects listed in the plan (Plan §Testing & Validation Strategy notes no test projects discovered)
- [✓] (2) If test projects exist, run tests (`dotnet test`) per Plan §Testing & Validation Strategy
- [✓] (3) Fix any test failures referencing Plan §Breaking Changes Catalog and re-run tests
- [✓] (4) All tests pass with 0 failures (**Verify**)
- [✓] (5) Commit test fixes if any with message: "TASK-003: Complete testing and validation"










