#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Handles parsing of curve keyframe strings and files.
    /// </summary>
    public static class CurveParser
    {
        /// <summary>Parses a key string like "key = 1.0 2.5 0 0".</summary>
        public static (bool Success, string? ErrorMessage, FloatString4? Point) TryParseKeyString(string? keyString)
        {
            if (string.IsNullOrWhiteSpace(keyString))
            {
                return (false, "Input string is null or empty", null);
            }

            try
            {
                var parts = keyString.Split(new char[] { '=', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    return (false, "Invalid format: expected 'key = time value [inTangent] [outTangent]'", null);
                }

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

                return (true, "", new FloatString4(time, value, inTangent, outTangent));
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}", null);
            }
        }

        /// <summary>Parses multiple keyframe lines into a list.</summary>
        public static List<FloatString4> ParseLines(string[] lines)
        {
            var points = new List<FloatString4>();

            foreach (var line in lines)
            {
                var result = TryParseKeyString(line.Trim());
                if (result.Success && result.Point != null)
                {
                    points.Add(result.Point);
                }
            }

            return points;
        }
    }
}