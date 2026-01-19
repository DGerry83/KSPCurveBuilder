/* 
* KSPCurveBuilder - A standalone float curve editing tool.
* 
* This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
* Logic from that original project is used here and throughout.
* 
* Original work copyright © 2015 Sarbian (https://github.com/sarbian   ).
* Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/   ).
* 
* This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
* See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html    or the LICENSE file for full terms.
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KSPCurveBuilder;

/// <summary>
/// Handles preset loading, saving, and deletion with async operations.
/// </summary>
public class PresetService
{
    public async Task<Preset[]> GetAllPresetsAsync()
    {
        var builtIns = BuiltInPresets.GetAll() ?? [];
        var userNames = await PresetManager.GetAvailablePresetsAsync();
        var userPresets = new List<Preset>();

        foreach (var name in userNames)
        {
            var preset = await PresetManager.LoadPresetAsync(name);
            if (preset != null) userPresets.Add(preset);
        }

        return builtIns.Union(userPresets).ToArray();
    }

    public async Task SavePresetAsync(string name, string description, List<FloatString4> points)
    {
        name = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Enter a valid preset name.", nameof(name));

        var builtIns = BuiltInPresets.GetAll();
        if (builtIns != null && builtIns.Any(p => p.Name == name))
            throw new InvalidOperationException($"Cannot overwrite built-in preset '{name}'.");

        var preset = new Preset
        {
            Name = name,
            Description = description,
            Points = [.. points]
        };

        await PresetManager.SavePresetAsync(preset);
    }

    public Task DeletePresetAsync(string presetName)
    {
        var builtIns = BuiltInPresets.GetAll();
        if (builtIns != null && builtIns.Any(p => p.Name == presetName))
            throw new InvalidOperationException("Cannot delete built-in presets.");

        PresetManager.DeletePreset(presetName); // Delete is fast, keep sync
        return Task.CompletedTask;
    }

    public Preset? GetDefaultPreset() => BuiltInPresets.Default();
}