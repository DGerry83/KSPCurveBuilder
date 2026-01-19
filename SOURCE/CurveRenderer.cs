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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace KSPCurveBuilder;

/// <summary>
/// Handles rendering the curve visualization to a Graphics surface.
/// Implements IDisposable to properly clean up GDI+ resources.
/// </summary>
public class CurveRenderer : IDisposable
{
    // Per-frame state (updated via UpdateState)
    private List<FloatString4> _points = [];
    private FloatCurveStandalone? _curve;
    private float _minTime, _maxTime, _minValue, _maxValue;
    private int _width, _height;
    private float _zoomLevel = 1.0f;
    private int _highlightedPointIndex = -1;
    private int _hoveredPointIndex = -1;

    // Persistent resources (created once, disposed once)
    private readonly Font _gridFont = new(Constants.Visual.GRID_FONT_NAME, Constants.Visual.GRID_FONT_SIZE);
    private readonly Font _titleFont = new(Constants.Visual.TITLE_FONT_NAME, Constants.Visual.TITLE_FONT_SIZE, Constants.Visual.TITLE_FONT_STYLE);
    private readonly Pen _curvePen = new(Color.LimeGreen, Constants.Visual.CURVE_PEN_WIDTH);
    private readonly Pen _gridPen = new(Color.FromArgb(60, 60, 60), Constants.Visual.GRID_PEN_WIDTH);
    private readonly Pen _pointPen = new(Color.White, Constants.Visual.POINT_PEN_WIDTH);
    private readonly Brush _pointBrush = Brushes.White;
    private bool _disposed = false;

    /// <summary>Updates all per-frame rendering state.</summary>
    public void UpdateState(List<FloatString4> points, FloatCurveStandalone? curve,
        float minTime, float maxTime, float minValue, float maxValue,
        int width, int height, float zoomLevel,
        int highlightedPointIndex = -1, int hoveredPointIndex = -1)
    {
        _points = points ?? [];
        _curve = curve;
        _minTime = float.IsNaN(minTime) || float.IsInfinity(minTime) ? 0f : minTime;
        _maxTime = float.IsNaN(maxTime) || float.IsInfinity(maxTime) ? 1f : maxTime;
        _minValue = float.IsNaN(minValue) || float.IsInfinity(minValue) ? 0f : minValue;
        _maxValue = float.IsNaN(maxValue) || float.IsInfinity(maxValue) ? 1f : maxValue;

        if (_maxTime <= _minTime)
            _maxTime = _minTime + 0.001f;
        if (_maxValue <= _minValue)
            _maxValue = _minValue + 0.001f;

        _width = Math.Max(1, width);
        _height = Math.Max(1, height);
        _zoomLevel = Math.Max(0.1f, Math.Min(10.0f, zoomLevel));
        _highlightedPointIndex = highlightedPointIndex;
        _hoveredPointIndex = hoveredPointIndex;
    }

    /// <summary>Renders the curve to the provided Graphics surface.</summary>
    public void Render(Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        if (_points.Count == 0) return;

        if (_width <= 0 || _height <= 0) return;
        if (float.IsNaN(_minTime) || float.IsNaN(_maxTime) ||
            float.IsNaN(_minValue) || float.IsNaN(_maxValue)) return;
        if (float.IsInfinity(_minTime) || float.IsInfinity(_maxTime) ||
            float.IsInfinity(_minValue) || float.IsInfinity(_maxValue)) return;

        float timeRange = _maxTime - _minTime;
        float valueRange = _maxValue - _minValue;
        if (timeRange <= float.Epsilon || valueRange <= float.Epsilon) return;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        DrawGrid(g);
        DrawCurve(g);
        DrawPoints(g);
        DrawHoverLabel(g);
        DrawLabels(g);
    }

    private void DrawGrid(Graphics g)
    {
        float timeRange = _maxTime - _minTime;
        float valueRange = _maxValue - _minValue;

        float timeStep = CalculateNiceStep(timeRange / Constants.GRID_LINES);
        float valueStep = CalculateNiceStep(valueRange / Constants.GRID_LINES);

        float firstTime = (float)Math.Ceiling(_minTime / timeStep) * timeStep;
        float firstValue = (float)Math.Ceiling(_minValue / valueStep) * valueStep;

        for (float time = firstTime; time <= _maxTime; time += timeStep)
        {
            float x = (time - _minTime) * _width / timeRange;
            if (IsCoordinateValid(x))
            {
                g.DrawLine(_gridPen, x, 0, x, _height);
                g.DrawString(time.ToString("F1", CultureInfo.InvariantCulture), _gridFont, Brushes.Gray, x, _height - Constants.Visual.GRID_LABEL_OFFSET_Y);
            }
        }

        for (float value = firstValue; value <= _maxValue; value += valueStep)
        {
            float y = _height - ((value - _minValue) * _height / valueRange);
            if (IsCoordinateValid(y))
            {
                g.DrawLine(_gridPen, 0, y, _width, y);
                g.DrawString(value.ToString("F2", CultureInfo.InvariantCulture), _gridFont, Brushes.Gray, Constants.Visual.GRID_LABEL_VALUE_OFFSET_X, y);
            }
        }
    }

    private void DrawCurve(Graphics g)
    {
        if (_curve?.Curve?.keys.Length < 2) return;

        var curvePoints = new List<PointF>();
        int samples = Math.Min(_width, Constants.MAX_SAMPLES);

        for (int i = 0; i <= samples; i++)
        {
            float time = _minTime + (i * (_maxTime - _minTime) / samples);
            float value = _curve.Evaluate(time);

            float x = (time - _minTime) * _width / (_maxTime - _minTime);
            float y = _height - ((value - _minValue) * _height / (_maxValue - _minValue));

            if (IsCoordinateValid(x) && IsCoordinateValid(y))
            {
                curvePoints.Add(new(x, y));
            }
        }

        if (curvePoints.Count >= 2)
        {
            g.DrawCurve(_curvePen, [.. curvePoints]);
        }
    }

    private void DrawPoints(Graphics g)
    {
        float timeRange = _maxTime - _minTime;
        float valueRange = _maxValue - _minValue;

        if (timeRange < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO ||
            valueRange < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO) return;

        for (int i = 0; i < _points.Count; i++)
        {
            bool isHighlighted = (i == _highlightedPointIndex);

            float x = (_points[i].Time - _minTime) * _width / timeRange;
            float y = _height - ((_points[i].Value - _minValue) * _height / valueRange);

            if (!IsCoordinateValid(x) || !IsCoordinateValid(y)) continue;

            Brush brush = isHighlighted ? Brushes.Yellow : _pointBrush;

            g.FillEllipse(brush, x - Constants.Visual.POINT_DRAW_RADIUS, y - Constants.Visual.POINT_DRAW_RADIUS, Constants.Visual.POINT_DRAW_SIZE, Constants.Visual.POINT_DRAW_SIZE);

            if (isHighlighted)
            {
                using Pen highlightPen = new(Color.Yellow, Constants.Visual.POINT_PEN_WIDTH);
                g.DrawEllipse(highlightPen, x - Constants.Visual.POINT_DRAW_RADIUS, y - Constants.Visual.POINT_DRAW_RADIUS, Constants.Visual.POINT_DRAW_SIZE, Constants.Visual.POINT_DRAW_SIZE);
            }
            else
            {
                g.DrawEllipse(_pointPen, x - Constants.Visual.POINT_DRAW_RADIUS, y - Constants.Visual.POINT_DRAW_RADIUS, Constants.Visual.POINT_DRAW_SIZE, Constants.Visual.POINT_DRAW_SIZE);
            }
        }
    }

    private void DrawLabels(Graphics g)
    {
        string title = $"Curve Editor - {_points.Count} point(s)";
        SizeF titleSize = g.MeasureString(title, _titleFont);
        float boxWidth = titleSize.Width + Constants.Visual.TITLE_BOX_PADDING_X;
        float boxHeight = titleSize.Height + Constants.Visual.TITLE_BOX_PADDING_Y;

        g.FillRectangle(Brushes.Black,
            Constants.Visual.TITLE_OFFSET_X - Constants.Visual.TITLE_BOX_PADDING_X,
            Constants.Visual.TITLE_OFFSET_Y - Constants.Visual.TITLE_BOX_PADDING_Y,
            boxWidth, boxHeight);
        g.DrawString(title, _titleFont, Brushes.White, Constants.Visual.TITLE_OFFSET_X, Constants.Visual.TITLE_OFFSET_Y);

        string zoomText = $"Zoom: {_zoomLevel:F1}x";
        SizeF textSize = g.MeasureString(zoomText, _gridFont);
        float labelX = _width - textSize.Width - Constants.Visual.LABEL_PADDING;
        float labelY = _height - textSize.Height - 30;
        g.DrawString(zoomText, _gridFont, Brushes.White, labelX, labelY);
    }

    private void DrawHoverLabel(Graphics g)
    {
        if (_hoveredPointIndex < 0 || _hoveredPointIndex >= _points.Count) return;

        var point = _points[_hoveredPointIndex];

        float x = (point.Time - _minTime) * _width / (_maxTime - _minTime) + Constants.HOVER_LABEL_OFFSET_X;
        float y = _height - ((point.Value - _minValue) * _height / (_maxValue - _minValue)) + Constants.HOVER_LABEL_OFFSET_Y;

        x = Math.Max(Constants.Visual.LABEL_PADDING, Math.Min(_width - 120, x));
        y = Math.Max(Constants.Visual.LABEL_PADDING, Math.Min(_height - 60, y));

        string label = $"Point {_hoveredPointIndex + 1}\n" +
                       $"Time: {FloatString4.FormatNumber(point.Time, "F2")}\n" +
                       $"Value: {FloatString4.FormatNumber(point.Value, "F3")}\n";

        SizeF textSize = g.MeasureString(label, _gridFont);
        float boxWidth = textSize.Width + 10;
        float boxHeight = textSize.Height + 8;

        g.FillRectangle(Brushes.Black, x, y, boxWidth, boxHeight);
        g.DrawRectangle(Pens.White, x, y, boxWidth, boxHeight);
        g.DrawString(label, _gridFont, Brushes.White, x + Constants.Visual.LABEL_PADDING, y + 4);
    }

    private float CalculateNiceStep(float rawStep)
    {
        if (rawStep <= 0) return 1f;
        float exponent = (float)Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
        float normalized = rawStep / exponent;

        return normalized switch
        {
            <= 1 => 1f * exponent,
            <= 2 => 2f * exponent,
            <= 5 => 5f * exponent,
            _ => 10f * exponent
        };
    }

    private bool IsCoordinateValid(float value) =>
        float.IsFinite(value) &&
        value >= Constants.Visual.MIN_RENDER_COORDINATE &&
        value <= Constants.Visual.MAX_RENDER_COORDINATE;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _gridFont?.Dispose();
                _titleFont?.Dispose();
                _curvePen?.Dispose();
                _gridPen?.Dispose();
                _pointPen?.Dispose();
            }
            _disposed = true;
        }
    }
}