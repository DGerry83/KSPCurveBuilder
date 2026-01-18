# KSPCurveBuilder
A standalone Windows desktop application for editing and visualizing float curves, with support for saving and loading custom presets.
Based on Amazing Curve Editor by sarbian (c)2015 (https://github.com/sarbian)

## Features

*   **Visual Curve Editing**: Interactive graph with pan and zoom controls.
*   **Direct Point Manipulation**: Edit curve keys (Time, Value, In/Out Tangents) by typing directly in the cells or by click-dragging on the cells.
*   **Preset Management**:
    *   Built-in curve presets (Default, Linear, Ease In, Ease Out, Smooth Start).
    *   Save, load, rename, and delete custom user presets.
*   **Data Portability**: Copy and paste curve data as plain-text key strings for easy sharing.
*   **Tangent Smoothing**: One-click button to automatically smooth curve tangents.

## Prerequisites

*   [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework) or later.
*   Windows 7/8/10/11.
*   (For Development) Visual Studio 2022 or later with the .NET desktop development workload.

## Getting Started

### Installation
1.  Download the latest `KSPCurveBuilder.zip` from the [Releases](https://github.com/DGerry83/KSPCurveBuilder/releases) page.
2.  Extract the ZIP file to a folder of your choice.
3.  Run `KSPCurveBuilder.exe`.

## Usage Guide

### Basic Editing
1.  **Add Points**: Click the "Add Node" button to insert a new key.
2.  **Edit Points**: Modify values directly in the data grid or click and drag on cells vertically to smoothly drag values up/down.
3.  **Delete Points**: Click the "X" button on a row in the data grid.
4.  **Pan Graph**: Click and drag on the graph view.
5.  **Zoom Graph**: Mouse-wheel zooms the graph view, click "Reset Zoom" to reset.

### Working with Presets
*   **Load a Preset**: Select a curve from the "Preset" dropdown menu.
*   **Save a Preset**: Modify a curve, enter a name in the textbox above the dropdown, and click "Save".

### Copy/Paste Curves
*   **Copy**: The textbox at the bottom shows the curve data as `key` strings. Click the "Copy" button to copy this text.
*   **Paste**: Pastes valid key data from the clipboard into the editor(eg. from ksp config files)

## License and Attribution

This project is released under the **GNU General Public License, version 2 (GPL-2.0)**.

**Copyright Notice:**
*   Original concept and foundational code for a float curve editor © 2015 sarbian (https://github.com/sarbian).
*   Modifications, reimplementation, and new code © 2026 DGerry83(https://github.com/DGerry83).

This standalone application is a significant reimplementation and extension of those original ideas.

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, **version 2 of the License**.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html> or the [LICENSE](LICENSE) file in this repository.
