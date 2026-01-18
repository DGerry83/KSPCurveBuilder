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
using System.Collections.Generic;
using System.Linq;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Represents an animation curve defined by keyframes with time-value pairs and tangents.
    /// Supports evaluation, smoothing, and clamp/loop wrap modes.
    /// </summary>
    [Serializable]
    public class MyAnimationCurve
    {
        private List<MyKeyframe> _keys = new List<MyKeyframe>();
        private WrapMode _preWrapMode = WrapMode.ClampForever;
        private WrapMode _postWrapMode = WrapMode.ClampForever;

        public MyKeyframe[] keys
        {
            get { return _keys.ToArray(); }
        }

        public WrapMode preWrapMode
        {
            get { return _preWrapMode; }
            set { _preWrapMode = value; }
        }

        public WrapMode postWrapMode
        {
            get { return _postWrapMode; }
            set { _postWrapMode = value; }
        }
        /// <summary>Adds a keyframe to the curve.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for NaN, Infinity, or values > 1e9.</exception>
        public int AddKey(float time, float value)
        {
            return AddKey(new MyKeyframe(time, value, 0f, 0f));
        }
        /// <summary>Adds a keyframe with tangents.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for invalid keyframe properties.</exception>
        public int AddKey(MyKeyframe key)
        {
            // Check for duplicate/near-duplicate time
            int existingIndex = _keys.FindIndex(k => Math.Abs(k.Time - key.Time) < Constants.TIME_EPSILON);
            if (existingIndex >= 0)
            {
                _keys[existingIndex] = key;
                return existingIndex;
            }

            // Validate all properties
            CurveValidator.ValidateFloat(key.Time, nameof(key.Time));
            CurveValidator.ValidateFloat(key.Value, nameof(key.Value));
            CurveValidator.ValidateFloat(key.InTangent, nameof(key.InTangent));
            CurveValidator.ValidateFloat(key.OutTangent, nameof(key.OutTangent));

            // O(log n) binary search + O(n) insert
            int insertIndex = FindInsertionIndex(key.Time);
            _keys.Insert(insertIndex, key);
            return insertIndex;
        }
        /// <summary>Binary search to find where to insert a new keyframe.</summary>
        private int FindInsertionIndex(float time)
        {
            int low = 0;
            int high = _keys.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                float midTime = _keys[mid].Time;

                if (time < midTime)
                    high = mid - 1;
                else
                    low = mid + 1;
            }

            return low;
        }
        /// <summary>Smooths tangents at a middle keyframe.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Cannot smooth first/last keyframe.</exception>
        public void SmoothTangents(int index, float weight)
        {
            if (_keys.Count < 3 || index <= 0 || index >= _keys.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Cannot smooth endpoints or invalid index");
            }

            var prev = _keys[index - 1];
            var curr = _keys[index];
            var next = _keys[index + 1];

            float dtLeft = curr.Time - prev.Time;
            float dtRight = next.Time - curr.Time;

            if (dtLeft < Constants.TIME_EPSILON || dtRight < Constants.TIME_EPSILON)
            {
                throw new InvalidOperationException("Time values too close to calculate tangents");
            }

            float leftSlope = (curr.Value - prev.Value) / dtLeft;
            float rightSlope = (next.Value - curr.Value) / dtRight;

            curr.InTangent = Mathf.Lerp(leftSlope, rightSlope, weight);
            curr.OutTangent = curr.InTangent;
            _keys[index] = curr;
        }
        /// <summary>Evaluates curve at a time using Hermite interpolation.</summary>
        public float Evaluate(float time)
        {
            if (_keys.Count == 0) return 0f;
            if (_keys.Count == 1) return _keys[0].Value;

            if (time <= _keys[0].Time)
            {
                return _preWrapMode == WrapMode.Loop ? Evaluate(LoopTime(time)) : _keys[0].Value;
            }

            if (time >= _keys[_keys.Count - 1].Time)
            {
                return _postWrapMode == WrapMode.Loop ? Evaluate(LoopTime(time)) : _keys[_keys.Count - 1].Value;
            }

            // Binary search for the correct segment
            int index = FindSegmentIndex(time);
            return EvaluateSegment(index, time);
        }

        /// <summary>Finds the index of the keyframe that starts the segment containing the time value.</summary>
        private int FindSegmentIndex(float time)
        {
            // Precondition: _keys.Count >= 2 and _keys[0].Time <= time <= _keys[_keys.Count - 1].Time
            // Find smallest i such that time <= _keys[i + 1].Time

            int low = 0;
            int high = _keys.Count - 2; // Last valid segment index

            while (low < high)
            {
                int mid = low + (high - low) / 2;

                if (time <= _keys[mid + 1].Time)
                {
                    high = mid;
                }
                else
                {
                    low = mid + 1;
                }
            }

            return low;
        }

        private float EvaluateSegment(int index, float time)
        {
            if (index < 0 || index >= _keys.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Segment index {index} is invalid. Must be 0 to {_keys.Count - 2} for a curve with {_keys.Count} keys.");

            MyKeyframe k0 = _keys[index];
            MyKeyframe k1 = _keys[index + 1];
            float h = k1.Time - k0.Time;

            if (Mathf.Approximately(h, 0f))
            {
                // Time values are identical, return average
                return (k0.Value + k1.Value) * 0.5f;
            }

            float s = (time - k0.Time) / h;
            s = Math.Max(0f, Math.Min(1f, s)); // Clamp to prevent extrapolation errors

            float m0 = (k0.OutTangent == 0f && index > 0) ? AutoTangent(index, true) : k0.OutTangent;
            float m1 = (k1.InTangent == 0f && index < _keys.Count - 2) ? AutoTangent(index + 1, false) : k1.InTangent;

            return HermiteInterpolate(k0.Value, k1.Value, m0, m1, s, h);
        }

        private float AutoTangent(int index, bool isOut)
        {
            if (index == 0 || index == _keys.Count - 1) return 0f;

            var prev = _keys[index - 1];
            var curr = _keys[index];
            var next = _keys[index + 1];

            float dtLeft = curr.Time - prev.Time;
            float dtRight = next.Time - curr.Time;

            if (dtLeft < 0.0001f || dtRight < 0.0001f) return 0f;

            return 0.5f * ((curr.Value - prev.Value) / dtLeft + (next.Value - curr.Value) / dtRight);
        }

        private float HermiteInterpolate(float v0, float v1, float m0, float m1, float s, float h)
        {
            float s2 = s * s;
            float s3 = s2 * s;

            float h00 = 2 * s3 - 3 * s2 + 1;
            float h10 = s3 - 2 * s2 + s;
            float h01 = -2 * s3 + 3 * s2;
            float h11 = s3 - s2;

            return h00 * v0 + h10 * h * m0 + h01 * v1 + h11 * h * m1;
        }

        private float LoopTime(float time)
        {
            float range = _keys[_keys.Count - 1].Time - _keys[0].Time;
            if (Mathf.Approximately(range, 0f)) return _keys[0].Time;
            return _keys[0].Time + ((time - _keys[0].Time) % range + range) % range;
        }
    }
    /// <summary>
    /// A keyframe with time, value, in-tangent, and out-tangent.
    /// Properties validate input (no NaN, Infinity, or values > 1e9).
    /// </summary>
    [Serializable]
    public class MyKeyframe
    {
        private float _time;
        private float _value;
        private float _inTangent;
        private float _outTangent;

        private static float ValidateProperty(float value, string propertyName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(propertyName, $"{propertyName} cannot be NaN or Infinity");

            if (Math.Abs(value) > Constants.MAX_REASONABLE_VALUE)
                throw new ArgumentOutOfRangeException(propertyName, $"{propertyName} too large: {value}");

            return value;
        }

        public float Time
        {
            get => _time;
            set => _time = ValidateProperty(value, nameof(Time));
        }

        public float Value
        {
            get => _value;
            set => _value = ValidateProperty(value, nameof(Value));
        }

        public float InTangent
        {
            get => _inTangent;
            set => _inTangent = ValidateProperty(value, nameof(InTangent));
        }

        public float OutTangent
        {
            get => _outTangent;
            set => _outTangent = ValidateProperty(value, nameof(OutTangent));
        }

        public MyKeyframe() { }

        public MyKeyframe(float time, float value, float inTangent, float outTangent)
        {
            Time = time;
            Value = value;
            InTangent = inTangent;
            OutTangent = outTangent;
        }
    }

    public enum WrapMode
    {
        Once = 1,
        Loop = 2,
        PingPong = 4,
        Default = 0,
        ClampForever = 8,
        Clamp = 1
    }

    public static class Mathf
    {
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static bool Approximately(float a, float b) => Math.Abs(a - b) < 0.0001f;
        public static float Min(float a, float b) => Math.Min(a, b);
        public static float Max(float a, float b) => Math.Max(a, b);
    }
}