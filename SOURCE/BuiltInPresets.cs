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

using System.Collections.Generic;

namespace KSPCurveBuilder;

/// <summary>
/// Collection of commonly-used curve presets.
/// </summary>
public static class BuiltInPresets
{
    /// <summary>Gets all built-in presets.</summary>
    public static List<Preset> GetAll() =>
    [
        Default(),
        Linear(),
        EaseIn(),
        EaseOut(),
        SmoothStart(),
    ];

    /// <summary>The default curve that loads on startup.</summary>
    public static Preset Default()
    {
        var points = new[]
        {
            new FloatString4(0, 0, 0, 0.02f),
            new FloatString4(100, 1, 0.02f, 0)
        };
        return new Preset
        {
            Name = "Default",
            Description = "Default curve on startup",
            Points = [.. points]
        };
    }

    /// <summary>Linear curve: constant rate of change.</summary>
    private static Preset Linear()
    {
        var points = new[]
        {
            new FloatString4(0, 0, 0, 0.01f),
            new FloatString4(100, 1, 0.01f, 0)
        };
        return new Preset
        {
            Name = "Linear",
            Description = "Constant rate of change",
            Points = [.. points]
        };
    }

    /// <summary>Ease-in curve: starts slow, accelerates.</summary>
    private static Preset EaseIn()
    {
        var points = new[]
        {
            new FloatString4(0, 0, 0, 0),
            new FloatString4(100, 1, 0.005f, 0)
        };
        return new Preset
        {
            Name = "Ease In",
            Description = "Starts slow, speeds up",
            Points = [.. points]
        };
    }

    /// <summary>Ease-out curve: starts fast, decelerates.</summary>
    private static Preset EaseOut()
    {
        var points = new[]
        {
            new FloatString4(0, 0, 0, 0.005f),
            new FloatString4(100, 1, 0, 0)
        };
        return new Preset
        {
            Name = "Ease Out",
            Description = "Starts fast, slows down",
            Points = [.. points]
        };
    }

    /// <summary>Smooth start curve: very gradual acceleration.</summary>
    private static Preset SmoothStart()
    {
        var points = new[]
        {
            new FloatString4(0, 0, 0, 0),
            new FloatString4(50, 0.1f, 0.002f, 0.002f),
            new FloatString4(100, 1, 0.02f, 0)
        };
        return new Preset
        {
            Name = "Smooth Start",
            Description = "Very gradual acceleration",
            Points = [.. points]
        };
    }
}