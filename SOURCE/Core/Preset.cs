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

using System;
using System.Collections.Generic;

namespace KSPCurveBuilder;

/// <summary>
/// Immutable preset class with required init-only properties.
/// </summary>
public class Preset
{
    public required string Name { get; init; }
    public string Description { get; init; } = "";
    public List<FloatString4> Points { get; init; } = [];

    [Obsolete("Use object initializer syntax: new Preset { Name = ..., Points = [...] }")]
    public static Preset FromPoints(string name, string description, IEnumerable<FloatString4> points)
    {
        return new Preset
        {
            Name = name,
            Description = description,
            Points = [.. points]
        };
    }
}