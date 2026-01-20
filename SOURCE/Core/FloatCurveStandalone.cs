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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace KSPCurveBuilder;

/// <summary>
/// Wraps MyAnimationCurve with Save/Load from text and min/max finding.
/// </summary>
[Serializable]
public class FloatCurveStandalone
{
    private MyAnimationCurve fCurve = new();
    private float _minTime = float.MaxValue;
    private float _maxTime = float.MinValue;
    private const int findCurveMinMaxInterations = 100;

    public FloatCurveStandalone() { }

    public FloatCurveStandalone(MyKeyframe[] keyframes) : this()
    {
        foreach (var keyframe in keyframes)
        {
            Add(keyframe.Time, keyframe.Value, keyframe.InTangent, keyframe.OutTangent);
        }
    }

    public MyAnimationCurve Curve
    {
        get => fCurve;
        set
        {
            fCurve = value ?? throw new ArgumentNullException(nameof(value));
            UpdateMinMaxTimes();
        }
    }

    public float minTime => _minTime;
    public float maxTime => _maxTime;

    public void Add(float time, float value) => Add(time, value, 0f, 0f);

    public void Add(float time, float value, float inTangent, float outTangent)
    {
        var key = new MyKeyframe(time, value, inTangent, outTangent);
        fCurve.AddKey(key);
        _minTime = Math.Min(_minTime, time);
        _maxTime = Math.Max(_maxTime, time);
    }

    public float Evaluate(float time) => fCurve.Evaluate(time);

    /// <summary>Loads curve from text lines in "key = time value in out" format.</summary>
    public void Load(string[] lines)
    {
        fCurve = new MyAnimationCurve();
        _minTime = float.MaxValue;
        _maxTime = float.MinValue;

        foreach (string line in lines)
        {
            var result = CurveParser.TryParseKeyString(line.Trim());
            if (result.Success && result.Point != null)
            {
                MyKeyframe key = result.Point.ToKeyframe();
                fCurve.AddKey(key);
                _minTime = Math.Min(_minTime, key.Time);
                _maxTime = Math.Max(_maxTime, key.Time);
            }
            else
            {
                Debug.WriteLine($"FloatCurve: Failed to parse line '{line}': {result.ErrorMessage}");
            }
        }
    }

    /// <summary>Saves curve to text lines in "key = time value in out" format.</summary>
    public string[] Save()
    {
        var lines = new List<string>();
        MyKeyframe[] keys = fCurve.keys;

        foreach (var key in keys)
        {
            lines.Add(string.Format(CultureInfo.InvariantCulture, "key = {0} {1} {2} {3}",
                key.Time, key.Value, key.InTangent, key.OutTangent));
        }

        return lines.ToArray();
    }

	public void SmoothTangents()
	{
		if (fCurve?.keys == null || fCurve.keys.Length < 3) return;

		for (int i = 1; i < fCurve.keys.Length - 1; i++)
		{
			fCurve.SmoothTangents(i, 0f);
		}
	}

	private void UpdateMinMaxTimes()
    {
        _minTime = float.MaxValue;
        _maxTime = float.MinValue;

        foreach (MyKeyframe key in fCurve.keys)
        {
            _minTime = Math.Min(_minTime, key.Time);
            _maxTime = Math.Max(_maxTime, key.Time);
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

        if (fCurve?.keys.Length == 0)
            return;

        float timeStart = float.MaxValue;
        float timeEnd = float.MinValue;
        foreach (MyKeyframe key in fCurve.keys)
        {
            timeStart = Math.Min(key.Time, timeStart);
            timeEnd = Math.Max(key.Time, timeEnd);
        }

        float sampleStep = (timeEnd - timeStart) / findCurveMinMaxInterations;
        for (int i = 0; i < findCurveMinMaxInterations; i++)
        {
            float time = timeStart + i * sampleStep;
            float value = fCurve.Evaluate(time);

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