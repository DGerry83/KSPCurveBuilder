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
using System.Globalization;

namespace KSPCurveBuilder;

/// <summary>
/// Immutable record that holds curve keyframe data (time, value, tangents).
/// C# 14: field keyword with explicit constructors (no circular chaining).
/// </summary>
[Serializable]
public sealed record FloatString4 : IComparable<FloatString4>, IComparable
{
    // C# 14: field keyword creates backing fields automatically
    public float Time { get; init => field = CurveValidator.ValidateFloat(value, nameof(Time)); }
    public float Value { get; init => field = CurveValidator.ValidateFloat(value, nameof(Value)); }
    public float InTangent { get; init => field = CurveValidator.ValidateFloat(value, nameof(InTangent)); }
    public float OutTangent { get; init => field = CurveValidator.ValidateFloat(value, nameof(OutTangent)); }



    // Parameterless constructor for serialization support
    public FloatString4() : this(0f, 0f, 0f, 0f) { }

    // Main constructor - explicitly initializes properties
    public FloatString4(float time, float value, float inTangent, float outTangent)
    {
        Time = time;
        Value = value;
        InTangent = inTangent;
        OutTangent = outTangent;
    }

    // Constructor from MyKeyframe
    public FloatString4(MyKeyframe keyframe) : this(
        keyframe?.Time ?? throw new ArgumentNullException(nameof(keyframe)),
        keyframe.Value,
        keyframe.InTangent,
        keyframe.OutTangent)
    { }

    // Computed string properties
    public string TimeString => FormatNumber(Time, Formatting.TIME_DECIMAL_PLACES);
    public string ValueString => FormatNumber(Value, Formatting.VALUE_DECIMAL_PLACES);
    public string InTangentString => FormatNumber(InTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);
    public string OutTangentString => FormatNumber(OutTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);

// Single implementation - no explicit interface duplication
public int CompareTo(FloatString4? other)
{
    if (other == null) return 1;
    return Time.CompareTo(other.Time);
}

// Also implement non-generic for safety
public int CompareTo(object? obj)
{
    if (obj is not FloatString4 other) return 1;
    return CompareTo(other);
}

    public MyKeyframe ToKeyframe() => new(Time, Value, InTangent, OutTangent);

    public static string FormatNumber(float number, string format)
    {
        if (string.IsNullOrEmpty(format))
            format = "G";

        string result = number.ToString(format, CultureInfo.InvariantCulture);
        return format.StartsWith("F") && result.EndsWith(".0") ? result.Substring(0, result.Length - 2) : result;
    }

    public string ToKeyString(string keyName = "key") => string.Format(CultureInfo.InvariantCulture, "{0} = {1} {2} {3} {4}",
        keyName ?? "key",
        FormatNumber(Time, Formatting.TIME_DECIMAL_PLACES),
        FormatNumber(Value, Formatting.VALUE_DECIMAL_PLACES),
        FormatNumber(InTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES),
        FormatNumber(OutTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES));

    public override string ToString() =>
        string.Format("FloatString4: Time={0}, Value={1}, InTan={2}, OutTan={3}", Time, Value, InTangent, OutTangent);
}