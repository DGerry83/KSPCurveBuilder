/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/ ).
 * 
 * This file is part of Curve Editor, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html  or the LICENSE file for full terms.
 */

using System.Collections.Generic;

namespace KSPCurveBuilder;

/// <summary>
/// A saved curve preset with name, description, and keyframe data.
/// </summary>
public class Preset
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<FloatString4> Points { get; set; } = [];

    /// <summary>
    /// Creates a preset from the current points list.
    /// </summary>
    public static Preset FromPoints(string name, string description, IEnumerable<FloatString4> points)
    {
        var preset = new Preset
        {
            Name = name,
            Description = description
        };

        foreach (var point in points)
        {
            preset.Points.Add(new FloatString4(point.Time, point.Value, point.InTangent, point.OutTangent));
        }

        return preset;
    }
}