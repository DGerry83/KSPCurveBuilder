/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/ ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html  or the LICENSE file for full terms.
 */

#nullable enable

using System;
using System.Globalization;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Immutable view model that holds curve keyframe data (time, value, tangents).
    /// Uses computed string properties for display - single source of truth.
    /// </summary>
    [Serializable]
    public sealed class FloatString4 : IComparable<FloatString4>
    {
        // SINGLE SOURCE OF TRUTH: Only store numeric values
        private readonly float _time;
        private readonly float _value;
        private readonly float _inTangent;
        private readonly float _outTangent;

        /// <summary>Gets the time value.</summary>
        public float Time => _time;

        /// <summary>Gets the value at this keyframe.</summary>
        public float Value => _value;

        /// <summary>Gets the incoming tangent.</summary>
        public float InTangent => _inTangent;

        /// <summary>Gets the outgoing tangent.</summary>
        public float OutTangent => _outTangent;

        /// <summary>Gets the formatted string representation of time.</summary>
        public string TimeString => FormatNumber(_time, Formatting.TIME_DECIMAL_PLACES);

        /// <summary>Gets the formatted string representation of value.</summary>
        public string ValueString => FormatNumber(_value, Formatting.VALUE_DECIMAL_PLACES);

        /// <summary>Gets the formatted string representation of inTangent.</summary>
        public string InTangentString => FormatNumber(_inTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);

        /// <summary>Gets the formatted string representation of outTangent.</summary>
        public string OutTangentString => FormatNumber(_outTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);

        /// <summary>Constructs a new FloatString4 with default values (0,0,0,0).</summary>
        public FloatString4() : this(0f, 0f, 0f, 0f) { }

        /// <summary>Constructs a new FloatString4 with specified values.</summary>
        public FloatString4(float time, float value, float inTangent, float outTangent)
        {
            // Validate inputs
            CurveValidator.ValidateFloat(time, nameof(time));
            CurveValidator.ValidateFloat(value, nameof(value));
            CurveValidator.ValidateFloat(inTangent, nameof(inTangent));
            CurveValidator.ValidateFloat(outTangent, nameof(outTangent));

            _time = time;
            _value = value;
            _inTangent = inTangent;
            _outTangent = outTangent;
        }

        /// <summary>Constructs from a MyKeyframe.</summary>
        public FloatString4(MyKeyframe keyframe) : this(
            keyframe?.Time ?? throw new ArgumentNullException(nameof(keyframe)),
            keyframe.Value,
            keyframe.InTangent,
            keyframe.OutTangent)
        { }

        /// <summary>Compares by time value for sorting.</summary>
        public int CompareTo(FloatString4? other)
        {
            if (other == null) return 1;
            return _time.CompareTo(other._time);
        }

        /// <summary>Converts to a MyKeyframe.</summary>
        public MyKeyframe ToKeyframe()
        {
            return new MyKeyframe(Time, Value, InTangent, OutTangent);
        }

        /// <summary>Creates a copy with a new time value.</summary>
        public FloatString4 WithTime(float newTime)
        {
            return new FloatString4(newTime, Value, InTangent, OutTangent);
        }

        /// <summary>Creates a copy with a new value.</summary>
        public FloatString4 WithValue(float newValue)
        {
            return new FloatString4(Time, newValue, InTangent, OutTangent);
        }

        /// <summary>Creates a copy with new tangents.</summary>
        public FloatString4 WithTangents(float newInTangent, float newOutTangent)
        {
            return new FloatString4(Time, Value, newInTangent, newOutTangent);
        }    

        /// <summary>Formats number with specified format, removing ".0" from integer values.</summary>
        public static string FormatNumber(float number, string format)
        {
            if (string.IsNullOrEmpty(format))
                format = "G";

            string result = number.ToString(format, CultureInfo.InvariantCulture);
            if (format.StartsWith("F") && result.EndsWith(".0"))
                return result.Substring(0, result.Length - 2);
            return result;
        }

        /// <summary>Serializes to key string format.</summary>
        public string ToKeyString(string keyName = "key")
        {
            if (string.IsNullOrEmpty(keyName))
                keyName = "key";

            return string.Format(CultureInfo.InvariantCulture, "{0} = {1} {2} {3} {4}", keyName,
                FormatNumber(Time, Formatting.TIME_DECIMAL_PLACES),
                FormatNumber(Value, Formatting.VALUE_DECIMAL_PLACES),
                FormatNumber(InTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES),
                FormatNumber(OutTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES));
        }

        public override string ToString()
        {
            return string.Format("FloatString4: Time={0}, Value={1}, InTan={2}, OutTan={3}",
                Time, Value, InTangent, OutTangent);
        }
    }
}