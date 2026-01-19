/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian  ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/  ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html   or the LICENSE file for full terms.
 */

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KSPCurveBuilder;

/// <summary>
/// Handles preset loading, saving, and deletion.
/// </summary>
public class PresetService
{
    public Preset[] GetAllPresets()
    {
        var builtIns = BuiltInPresets.GetAll() ?? [];
        var userNames = PresetManager.GetAvailablePresets() ?? [];
        var userPresets = userNames.Select(PresetManager.LoadPreset)
                                  .Where(p => p != null)
                                  .ToArray()!;

        return builtIns.Union(userPresets).ToArray();
    }

    public void SavePreset(string name, string description, List<FloatString4> points)
    {
        name = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Enter a valid preset name.");
            return;
        }

        var builtIns = BuiltInPresets.GetAll();
        if (builtIns != null && builtIns.Any(p => p.Name == name))
        {
            MessageBox.Show($"Cannot overwrite built-in preset '{name}'.");
            return;
        }

        var preset = Preset.FromPoints(name, description, points);
        PresetManager.SavePreset(preset);
    }

    public void DeletePreset(string presetName)
    {
        var builtIns = BuiltInPresets.GetAll();
        if (builtIns != null && builtIns.Any(p => p.Name == presetName))
        {
            MessageBox.Show("Cannot delete built-in presets.");
            return;
        }

        PresetManager.DeletePreset(presetName);
    }

    public Preset? GetDefaultPreset() => BuiltInPresets.Default();
}