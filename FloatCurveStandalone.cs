/* 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that project is used here and throughout
 * Modified and restructured by Karl Kreegland in 2026.
 * Licensed under the GNU General Public License v2.0.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Wraps MyAnimationCurve with Save/Load from text and min/max finding.
    /// </summary>
    [Serializable]
    public class FloatCurveStandalone
    {
        private MyAnimationCurve fCurve;
        private float _minTime;
        private float _maxTime;

        private static readonly char[] delimiters = new char[] { ' ', ',', ';', '\t' };
        private const int findCurveMinMaxInterations = 100;

        public FloatCurveStandalone()
        {
            this.fCurve = new MyAnimationCurve();
            this._minTime = float.MaxValue;
            this._maxTime = float.MinValue;
            this.fCurve.postWrapMode = WrapMode.ClampForever;
            this.fCurve.preWrapMode = WrapMode.ClampForever;
        }

        public FloatCurveStandalone(MyKeyframe[] keyframes) : this()
        {
            for (int i = 0; i < keyframes.Length; i++)
            {
                this.Add(keyframes[i].Time, keyframes[i].Value, keyframes[i].InTangent, keyframes[i].OutTangent);
            }
        }

        public MyAnimationCurve Curve
        {
            get { return this.fCurve; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.fCurve = value;
                this.UpdateMinMaxTimes();
            }
        }

        public float minTime
        {
            get { return this._minTime; }
        }

        public float maxTime
        {
            get { return this._maxTime; }
        }

        public void Add(float time, float value)
        {
            this.fCurve.AddKey(time, value);
            this._minTime = Math.Min(this.minTime, time);
            this._maxTime = Math.Max(this.maxTime, time);
        }

        public void Add(float time, float value, float inTangent, float outTangent)
        {
            MyKeyframe key = new MyKeyframe(time, value, inTangent, outTangent);
            this.fCurve.AddKey(key);
            this._minTime = Math.Min(this.minTime, time);
            this._maxTime = Math.Max(this.maxTime, time);
        }

        public float Evaluate(float time)
        {
            return this.fCurve.Evaluate(time);
        }
        /// <summary>Loads curve from text lines in "key = time value in out" format.</summary>
        public void Load(string[] lines)
        {
            this.fCurve = new MyAnimationCurve();
            this._minTime = float.MaxValue;
            this._maxTime = float.MinValue;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                if (trimmedLine.StartsWith("key", StringComparison.OrdinalIgnoreCase))
                {
                    var parseResult = FloatString4.TryParseKeyString(trimmedLine);
                    if (parseResult.Success)
                    {
                        MyKeyframe key = parseResult.Point.ToKeyframe();
                        this.fCurve.AddKey(key);
                        this._minTime = Math.Min(this._minTime, key.Time);
                        this._maxTime = Math.Max(this._maxTime, key.Time);
                    }
                    else
                    {
                        Debug.WriteLine($"FloatCurve: Failed to parse line '{trimmedLine}': {parseResult.ErrorMessage}");
                    }
                }
            }
        }
        /// <summary>Saves curve to text lines in "key = time value in out" format.</summary>
        public string[] Save()
        {
            List<string> lines = new List<string>();
            MyKeyframe[] keys = this.fCurve.keys;

            for (int i = 0; i < keys.Length; i++)
            {
                lines.Add(string.Format(CultureInfo.InvariantCulture, "key = {0} {1} {2} {3}",
                    keys[i].Time,
                    keys[i].Value,
                    keys[i].InTangent,
                    keys[i].OutTangent));
            }

            return lines.ToArray();
        }
        /// <summary>Applies SmoothTangents to every keyframe in the curve.</summary>
        public void SmoothTangents()
        {
            MyKeyframe[] keys = this.fCurve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                this.fCurve.SmoothTangents(i, 0f);
            }
        }

        private void UpdateMinMaxTimes()
        {
            this._minTime = float.MaxValue;
            this._maxTime = float.MinValue;

            foreach (MyKeyframe key in this.fCurve.keys)
            {
                this._minTime = Math.Min(this._minTime, key.Time);
                this._maxTime = Math.Max(this._maxTime, key.Time);
            }
        }

        public void FindMinMaxValue(out float min, out float max)
        {
            FindMinMaxValue(out min, out max, out _, out _);
        }

        public void FindMinMaxValue(out float min, out float max, out float tMin, out float tMax)
        {
            min = float.MaxValue;
            max = float.MinValue;
            tMin = 0f;
            tMax = 0f;

            if (this.fCurve == null || this.fCurve.keys.Length == 0)
                return;

            // Find time bounds
            float timeStart = float.MaxValue;
            float timeEnd = float.MinValue;
            foreach (MyKeyframe key in this.fCurve.keys)
            {
                if (key.Time < timeStart) timeStart = key.Time;
                if (key.Time > timeEnd) timeEnd = key.Time;
            }

            float sampleStep = (timeEnd - timeStart) / findCurveMinMaxInterations;
            for (int i = 0; i < findCurveMinMaxInterations; i++)
            {
                float time = timeStart + i * sampleStep;
                float value = this.fCurve.Evaluate(time);

                if (value < min)
                {
                    min = value;
                    tMin = time;
                }
                if (value > max)
                {
                    max = value;
                    tMax = time;
                }
            }
        }
    }
}