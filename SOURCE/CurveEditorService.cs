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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KSPCurveBuilder;

/// <summary>
/// Encapsulates core curve editing business logic.
/// </summary>
public class CurveEditorService(List<FloatString4> points, BindingList<FloatString4> bindingList)
{
    private readonly List<FloatString4> _points = points ?? throw new ArgumentNullException(nameof(points));
    private readonly BindingList<FloatString4> _bindingList = bindingList ?? throw new ArgumentNullException(nameof(bindingList));

    public IReadOnlyList<FloatString4> Points => _points.AsReadOnly();
    public List<FloatString4> PointsInternal => _points;

    public event EventHandler? PointsChanged;
    public event EventHandler? SilentPointsChanged;

    public FloatCurveStandalone? CreateCurveFromPoints()
    {
        if (_points.Count == 0) return null;

        var curve = new FloatCurveStandalone();
        foreach (var point in _points)
        {
            if (point != null)
                curve.Add(point.Time, point.Value, point.InTangent, point.OutTangent);
        }
        return curve;
    }

    public void ClearPoints()
    {
        _points.Clear();
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddPoint(FloatString4 point)
    {
        if (point == null) return;
        _points.Add(point);
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpdatePoint(int index, FloatString4 newPoint)
    {
        if (index < 0 || index >= _points.Count) return;
        if (newPoint == null) return;

        _points[index] = newPoint;
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpdatePointSilently(int index, FloatString4 newPoint)
    {
        if (index < 0 || index >= _points.Count) return;
        if (newPoint == null) return;

        _points[index] = newPoint;
        _bindingList.ResetBindings();
        SilentPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SortByTimeSilently()
    {
        _points.Sort();
        _bindingList.ResetBindings();
        SilentPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemovePoint(int index)
    {
        if (index < 0 || index >= _points.Count) return;

        _points.RemoveAt(index);
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SmoothTangents()
    {
        var curve = CreateCurveFromPoints();
        if (curve != null)
        {
            curve.SmoothTangents();
            LoadFromCurve(curve);
        }
    }

    public void LoadFromCurve(FloatCurveStandalone curve)
    {
        _points.Clear();
        if (curve?.Curve?.keys != null && curve.Curve.keys.Length > 0)
        {
            foreach (var key in curve.Curve.keys)
            {
                if (key != null)
                    _points.Add(new FloatString4(key));
            }
        }
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void LoadFromPoints(IEnumerable<FloatString4> points)
    {
        _points.Clear();
        if (points != null)
        {
            _points.AddRange(points);
        }
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SortByTime()
    {
        _points.RemoveAll(p => p is null);

        if (_points.Count > 0)
        {
            _points.Sort();
        }

        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public string SerializeToText()
    {
        return string.Join(Environment.NewLine,
            _points.Select(p => p.ToKeyString("key")));
    }

    public void DeserializeFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var newPoints = new List<FloatString4>();
        foreach (string line in text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            var result = CurveParser.TryParseKeyString(trimmedLine);
            if (result.Success && result.Point != null)
            {
                newPoints.Add(result.Point);
            }
        }

        _points.Clear();
        _points.AddRange(newPoints);
        _bindingList.ResetBindings();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }
}