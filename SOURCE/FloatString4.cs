/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/).
 * 
 * This file is part of Curve Editor, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html or the LICENSE file for full terms.
 */

using System;
using System.Linq;
using System.Globalization;

namespace KSPCurveBuilder
{
    /// <summary>
    /// View model that syncs numeric values with string representations for display/editing.
    /// </summary>
    [Serializable]
    public class FloatString4 : IComparable<FloatString4>
    {
        public float time;
        public float value;
        public float inTangent;
        public float outTangent;
        public string[] strings;

        public FloatString4()
        {
            time = 0f;
            value = 0f;
            inTangent = 0f;
            outTangent = 0f;
            strings = new string[] { "0", "0", "0", "0" };
        }

        public FloatString4(float t, float v, float inTan, float outTan)
        {
            time = t;
            value = v;
            inTangent = inTan;
            outTangent = outTan;
            UpdateStrings();
        }

        public FloatString4(MyKeyframe keyframe)
        {
            time = keyframe.Time;
            value = keyframe.Value;
            inTangent = keyframe.InTangent;
            outTangent = keyframe.OutTangent;
            UpdateStrings();
        }

        public int CompareTo(FloatString4 other)
        {
            if (other == null) return 1;
            return time.CompareTo(other.Time);
        }

        public void UpdateFloats()
        {
            float.TryParse(strings[0], NumberStyles.Float, CultureInfo.InvariantCulture, out time);
            float.TryParse(strings[1], NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float.TryParse(strings[2], NumberStyles.Float, CultureInfo.InvariantCulture, out inTangent);
            float.TryParse(strings[3], NumberStyles.Float, CultureInfo.InvariantCulture, out outTangent);
        }

        public void UpdateStrings()
        {
            strings = new string[]
            {
        FormatNumber(time, Formatting.TIME_DECIMAL_PLACES),
        FormatNumber(value, Formatting.VALUE_DECIMAL_PLACES),
        FormatNumber(inTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES),
        FormatNumber(outTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES)
            };
        }

        private string FormatSignificant(float number)
        {
            return number.ToString(Formatting.TANGENT_SIGNIFICANT_FIGURES, CultureInfo.InvariantCulture);
        }

        public MyKeyframe ToKeyframe()
        {
            return new MyKeyframe(time, value, inTangent, outTangent);
        }

        public float Time
        {
            get { return time; }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Time)} cannot be NaN or Infinity");
                time = value;
                UpdateStrings();
            }
        }

        public float Value
        {
            get { return value; }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Value)} cannot be NaN or Infinity");
                this.value = value;
                UpdateStrings();
            }
        }

        public float InTangent
        {
            get { return inTangent; }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(InTangent)} cannot be NaN or Infinity");
                inTangent = value;
                UpdateStrings();
            }
        }

        public float OutTangent
        {
            get { return outTangent; }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(OutTangent)} cannot be NaN or Infinity");
                outTangent = value;
                UpdateStrings();
            }
        }

        public static FloatString4 FromKeyString(string keyString)
        {
            var result = TryParseKeyString(keyString);
            return result.Point ?? new FloatString4();
        }
        /// <summary>Parses a key string like "key = 1.0 2.5 0 0". Returns success, error, and point.</summary>
        public static (bool Success, string ErrorMessage, FloatString4 Point) TryParseKeyString(string keyString)
        {
            try
            {
                string[] parts = keyString.Split(new char[] { '=', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    return (false, "Invalid format: expected 'key = time value [inTangent] [outTangent]'", null);
                }

                FloatString4 result = new FloatString4();
                var culture = CultureInfo.InvariantCulture;

                if (!float.TryParse(parts[1], NumberStyles.Float, culture, out float time))
                    return (false, $"Invalid time value: '{parts[1]}'", null);

                if (!float.TryParse(parts[2], NumberStyles.Float, culture, out float value))
                    return (false, $"Invalid value: '{parts[2]}'", null);

                float inTangent = 0f, outTangent = 0f;
                if (parts.Length > 3 && !string.IsNullOrEmpty(parts[3]))
                {
                    if (!float.TryParse(parts[3], NumberStyles.Float, culture, out inTangent))
                        return (false, $"Invalid inTangent: '{parts[3]}'", null);
                }

                if (parts.Length > 4 && !string.IsNullOrEmpty(parts[4]))
                {
                    if (!float.TryParse(parts[4], NumberStyles.Float, culture, out outTangent))
                        return (false, $"Invalid outTangent: '{parts[4]}'", null);
                }

                try
                {
                    CurveValidator.ValidateFloat(time, nameof(time));
                    CurveValidator.ValidateFloat(value, nameof(value));
                    CurveValidator.ValidateFloat(inTangent, nameof(inTangent));
                    CurveValidator.ValidateFloat(outTangent, nameof(outTangent));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return (false, ex.Message, null);
                }

                result.Time = time;
                result.Value = value;
                result.InTangent = inTangent;
                result.OutTangent = outTangent;
                result.UpdateStrings();
                return (true, "", result);
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}", null);
            }
        }
        /// <summary>Formats number with specified format, removing ".0" from integer values.</summary>
        public static string FormatNumber(float number, string format)
        {
            string result = number.ToString(format, CultureInfo.InvariantCulture);
            if (format.StartsWith("F") && result.EndsWith(".0"))
                return result.Substring(0, result.Length - 2);
            return result;
        }
        /// <summary>Serializes to key string format.</summary>
        public string ToKeyString(string keyName = "key")
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} = {1} {2} {3} {4}", keyName,
                FormatNumber(time, Formatting.TIME_DECIMAL_PLACES),
                FormatNumber(value, Formatting.VALUE_DECIMAL_PLACES),
                FormatNumber(inTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES),
                FormatNumber(outTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES));
        }

        public override string ToString()
        {
            return string.Format("FloatString4: Time={0}, Value={1}, InTan={2}, OutTan={3}",
                time, value, inTangent, outTangent);
        }
    }
}