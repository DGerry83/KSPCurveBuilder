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
using System.Globalization;

namespace KSPCurveBuilder;

/// <summary>
/// Immutable view model that holds curve keyframe data (time, value, tangents).
/// Uses computed string properties for display - single source of truth.
/// </summary>
[Serializable]
public sealed class FloatString4 : IComparable<FloatString4>
{
    private readonly float _time;
    private readonly float _value;
    private readonly float _inTangent;
    private readonly float _outTangent;

    public float Time => _time;
    public float Value => _value;
    public float InTangent => _inTangent;
    public float OutTangent => _outTangent;

    public string TimeString => FormatNumber(_time, Formatting.TIME_DECIMAL_PLACES);
    public string ValueString => FormatNumber(_value, Formatting.VALUE_DECIMAL_PLACES);
    public string InTangentString => FormatNumber(_inTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);
    public string OutTangentString => FormatNumber(_outTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);

    public FloatString4() : this(0f, 0f, 0f, 0f) { }

    public FloatString4(float time, float value, float inTangent, float outTangent)
    {
        CurveValidator.ValidateFloat(time, nameof(time));
        CurveValidator.ValidateFloat(value, nameof(value));
        CurveValidator.ValidateFloat(inTangent, nameof(inTangent));
        CurveValidator.ValidateFloat(outTangent, nameof(outTangent));

        _time = time;
        _value = value;
        _inTangent = inTangent;
        _outTangent = outTangent;
    }

    public FloatString4(MyKeyframe keyframe) : this(
        keyframe?.Time ?? throw new ArgumentNullException(nameof(keyframe)),
        keyframe.Value,
        keyframe.InTangent,
        keyframe.OutTangent)
    { }

    public int CompareTo(FloatString4? other)
    {
        if (other == null) return 1;
        return _time.CompareTo(other._time);
    }

    public MyKeyframe ToKeyframe() => new(Time, Value, InTangent, OutTangent);

    public FloatString4 WithTime(float newTime) => new(newTime, Value, InTangent, OutTangent);

    public FloatString4 WithValue(float newValue) => new(Time, newValue, InTangent, OutTangent);

    public FloatString4 WithTangents(float newInTangent, float newOutTangent) =>
        new(Time, Value, newInTangent, newOutTangent);

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