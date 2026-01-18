# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [KSPCurveBuilder.csproj](#kspcurvebuildercsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | All require upgrade |
| Total NuGet Packages | 0 | All compatible |
| Total Code Files | 15 |  |
| Total Code Files with Incidents | 7 |  |
| Total Lines of Code | 2646 |  |
| Total Number of Issues | 1107 |  |
| Estimated LOC to modify | 1105+ | at least 41.8% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [KSPCurveBuilder.csproj](#kspcurvebuildercsproj) | net48 | 🟡 Medium | 0 | 1105 | 1105+ | ClassicWinForms, Sdk Style = False |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 0 | 0.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***0*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 969 | High - Require code changes |
| 🟡 Source Incompatible | 136 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2263 |  |
| ***Total APIs Analyzed*** | ***3368*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Forms | 969 | 87.7% | Windows Forms APIs for building Windows desktop applications with traditional Forms-based UI that are available in .NET on Windows. Enable Windows Desktop support: Option 1 (Recommended): Target net9.0-windows; Option 2: Add <UseWindowsDesktop>true</UseWindowsDesktop>; Option 3 (Legacy): Use Microsoft.NET.Sdk.WindowsDesktop SDK. |
| Windows Forms Legacy Controls | 267 | 24.2% | Legacy Windows Forms controls that have been removed from .NET Core/5+ including StatusBar, DataGrid, ContextMenu, MainMenu, MenuItem, and ToolBar. These controls were replaced by more modern alternatives. Use ToolStrip, MenuStrip, ContextMenuStrip, and DataGridView instead. |
| GDI+ / System.Drawing | 134 | 12.1% | System.Drawing APIs for 2D graphics, imaging, and printing that are available via NuGet package System.Drawing.Common. Note: Not recommended for server scenarios due to Windows dependencies; consider cross-platform alternatives like SkiaSharp or ImageSharp for new code. |
| Legacy Configuration System | 2 | 0.2% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Windows.Forms.Button | 88 | 8.0% | Binary Incompatible |
| T:System.Windows.Forms.DataGridView | 54 | 4.9% | Binary Incompatible |
| T:System.Windows.Forms.PictureBox | 38 | 3.4% | Binary Incompatible |
| T:System.Windows.Forms.ComboBox | 30 | 2.7% | Binary Incompatible |
| T:System.Windows.Forms.TextBox | 27 | 2.4% | Binary Incompatible |
| T:System.Drawing.Pen | 25 | 2.3% | Source Incompatible |
| T:System.Windows.Forms.DialogResult | 23 | 2.1% | Binary Incompatible |
| T:System.Windows.Forms.Padding | 22 | 2.0% | Binary Incompatible |
| T:System.Drawing.Graphics | 20 | 1.8% | Source Incompatible |
| T:System.Windows.Forms.MessageBoxButtons | 20 | 1.8% | Binary Incompatible |
| T:System.Drawing.Font | 19 | 1.7% | Source Incompatible |
| T:System.Windows.Forms.CheckBox | 18 | 1.6% | Binary Incompatible |
| T:System.Windows.Forms.MessageBoxIcon | 18 | 1.6% | Binary Incompatible |
| P:System.Windows.Forms.Control.Name | 15 | 1.4% | Binary Incompatible |
| T:System.Windows.Forms.DataGridViewAutoSizeColumnMode | 15 | 1.4% | Binary Incompatible |
| T:System.Windows.Forms.Control.ControlCollection | 14 | 1.3% | Binary Incompatible |
| P:System.Windows.Forms.Control.Controls | 14 | 1.3% | Binary Incompatible |
| M:System.Windows.Forms.Control.ControlCollection.Add(System.Windows.Forms.Control) | 14 | 1.3% | Binary Incompatible |
| P:System.Windows.Forms.Control.Size | 14 | 1.3% | Binary Incompatible |
| P:System.Windows.Forms.Control.Location | 14 | 1.3% | Binary Incompatible |
| T:System.Windows.Forms.MouseEventHandler | 14 | 1.3% | Binary Incompatible |
| P:System.Windows.Forms.Control.TabIndex | 13 | 1.2% | Binary Incompatible |
| T:System.Windows.Forms.MessageBox | 13 | 1.2% | Binary Incompatible |
| T:System.Drawing.Brush | 12 | 1.1% | Source Incompatible |
| T:System.Windows.Forms.Cursor | 12 | 1.1% | Binary Incompatible |
| M:System.Windows.Forms.Padding.#ctor(System.Int32,System.Int32,System.Int32,System.Int32) | 11 | 1.0% | Binary Incompatible |
| P:System.Windows.Forms.Control.Margin | 10 | 0.9% | Binary Incompatible |
| P:System.Windows.Forms.ButtonBase.Text | 9 | 0.8% | Binary Incompatible |
| M:System.Windows.Forms.MessageBox.Show(System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon) | 9 | 0.8% | Binary Incompatible |
| E:System.Windows.Forms.Control.Click | 8 | 0.7% | Binary Incompatible |
| M:System.Windows.Forms.Button.#ctor | 8 | 0.7% | Binary Incompatible |
| T:System.Windows.Forms.ComboBox.ObjectCollection | 8 | 0.7% | Binary Incompatible |
| P:System.Windows.Forms.ComboBox.Items | 8 | 0.7% | Binary Incompatible |
| P:System.Windows.Forms.DataGridViewCellEventArgs.RowIndex | 8 | 0.7% | Binary Incompatible |
| T:System.Windows.Forms.DataGridViewCellEventHandler | 8 | 0.7% | Binary Incompatible |
| T:System.Drawing.Brushes | 7 | 0.6% | Source Incompatible |
| P:System.Windows.Forms.ComboBox.SelectedItem | 7 | 0.6% | Binary Incompatible |
| P:System.Windows.Forms.TextBox.Text | 7 | 0.6% | Binary Incompatible |
| T:System.Windows.Forms.MouseEventArgs | 7 | 0.6% | Binary Incompatible |
| M:System.Windows.Forms.Control.Invalidate | 7 | 0.6% | Binary Incompatible |
| T:System.Drawing.Icon | 6 | 0.5% | Source Incompatible |
| M:System.Drawing.Pen.#ctor(System.Drawing.Color,System.Single) | 6 | 0.5% | Source Incompatible |
| T:System.Windows.Forms.Cursors | 6 | 0.5% | Binary Incompatible |
| P:System.Windows.Forms.Control.Cursor | 6 | 0.5% | Binary Incompatible |
| T:System.Windows.Forms.MouseButtons | 6 | 0.5% | Binary Incompatible |
| P:System.Windows.Forms.DataGridViewCellEventArgs.ColumnIndex | 6 | 0.5% | Binary Incompatible |
| P:System.Windows.Forms.CheckBox.Checked | 6 | 0.5% | Binary Incompatible |
| P:System.Windows.Forms.DataGridViewCellValidatingEventArgs.ColumnIndex | 6 | 0.5% | Binary Incompatible |
| T:System.Windows.Forms.DataGridViewRowCollection | 6 | 0.5% | Binary Incompatible |
| P:System.Windows.Forms.DataGridView.Rows | 6 | 0.5% | Binary Incompatible |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>⚙️&nbsp;KSPCurveBuilder.csproj</b><br/><small>net48</small>"]
    click P1 "#kspcurvebuildercsproj"

```

## Project Details

<a id="kspcurvebuildercsproj"></a>
### KSPCurveBuilder.csproj

#### Project Info

- **Current Target Framework:** net48
- **Proposed Target Framework:** net10.0-windows
- **SDK-style**: False
- **Project Kind:** ClassicWinForms
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 18
- **Number of Files with Incidents**: 7
- **Lines of Code**: 2646
- **Estimated LOC to modify**: 1105+ (at least 41.8% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["KSPCurveBuilder.csproj"]
        MAIN["<b>⚙️&nbsp;KSPCurveBuilder.csproj</b><br/><small>net48</small>"]
        click MAIN "#kspcurvebuildercsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 969 | High - Require code changes |
| 🟡 Source Incompatible | 136 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2263 |  |
| ***Total APIs Analyzed*** | ***3368*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 2 | 0.2% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |
| Windows Forms Legacy Controls | 267 | 24.2% | Legacy Windows Forms controls that have been removed from .NET Core/5+ including StatusBar, DataGrid, ContextMenu, MainMenu, MenuItem, and ToolBar. These controls were replaced by more modern alternatives. Use ToolStrip, MenuStrip, ContextMenuStrip, and DataGridView instead. |
| GDI+ / System.Drawing | 134 | 12.1% | System.Drawing APIs for 2D graphics, imaging, and printing that are available via NuGet package System.Drawing.Common. Note: Not recommended for server scenarios due to Windows dependencies; consider cross-platform alternatives like SkiaSharp or ImageSharp for new code. |
| Windows Forms | 969 | 87.7% | Windows Forms APIs for building Windows desktop applications with traditional Forms-based UI that are available in .NET on Windows. Enable Windows Desktop support: Option 1 (Recommended): Target net9.0-windows; Option 2: Add <UseWindowsDesktop>true</UseWindowsDesktop>; Option 3 (Legacy): Use Microsoft.NET.Sdk.WindowsDesktop SDK. |

