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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KSPCurveBuilder;

public class CurveViewController : IDisposable
{
    private readonly PictureBox _pictureBox;
    private readonly CurveEditorService _editorService;
    private readonly Func<FloatCurveStandalone?> _getCurve;
    private readonly CurveRenderer _renderer = new();

    public float ZoomLevel { get; set; } = 1.0f;
    public PointF PanCenter { get; set; } = new(0.5f, 0.5f);
    public float GraphMinTime { get; private set; }
    public float GraphMaxTime { get; private set; }
    public float GraphMinValue { get; private set; }
    public float GraphMaxValue { get; private set; }

    private bool _isDraggingPoint = false;
    private int _draggedPointIndex = -1;
    private int _hoveredPointIndex = -1;
    private Point _panDragStart = Point.Empty;
    private bool _isPanning = false;
    private FloatString4? _originalPoint = null;

    public event EventHandler? DragStarted;
    public event EventHandler? DragEnded;
    public event EventHandler<PointDragEventArgs>? PointDragged;
    public event EventHandler? ViewChanged;

    // CORRECT: Classic constructor (not primary constructor body) for maximum compatibility
    public CurveViewController(PictureBox pictureBox, CurveEditorService editorService, Func<FloatCurveStandalone?> getCurve)
    {
        _pictureBox = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _getCurve = getCurve ?? throw new ArgumentNullException(nameof(getCurve));

        _pictureBox.Paint += OnPaint;
        _pictureBox.MouseWheel += OnMouseWheel;
        _pictureBox.MouseDown += OnMouseDown;
        _pictureBox.MouseMove += OnMouseMove;
        _pictureBox.MouseUp += OnMouseUp;
        _pictureBox.MouseClick += OnMouseClick;
        _pictureBox.Resize += OnResize;
        _pictureBox.Disposed += (s, e) => _renderer.Dispose();
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        if (e?.Graphics == null) return;

        var curve = _getCurve();
        var points = _editorService.Points;

        if (points.Count == 0)
        {
            UpdateGraphBounds();
            e.Graphics.Clear(Color.Black);
            return;
        }

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.Clear(Color.Black);

        UpdateGraphBounds();

        _renderer.UpdateState(points.ToList(), curve,
            GraphMinTime, GraphMaxTime, GraphMinValue, GraphMaxValue,
            _pictureBox.Width, _pictureBox.Height, ZoomLevel,
            _draggedPointIndex, _hoveredPointIndex);

        _renderer.Render(e.Graphics);
    }

    private void OnResize(object? sender, EventArgs e) => _pictureBox?.Invalidate();

    private float CalculateOptimalZoom(float minTime, float maxTime, float minValue, float maxValue)
    {
        var points = _editorService.Points;
        if (points.Count == 0) return 1.0f;

        float keyMin = points.Min(p => p.Value);
        float keyMax = points.Max(p => p.Value);
        float keyRange = keyMax - keyMin;

        if (keyRange < 0.001f) return 1.0f;

        float curveRange = maxValue - minValue;
        if (curveRange < 0.001f) return 1.0f;

        return Math.Max(0.1f, Math.Min(10.0f, keyRange / curveRange));
    }

    private PointF CalculateOptimalPan(float minTime, float maxTime, float minValue, float maxValue)
    {
        var points = _editorService.Points;
        if (points.Count == 0) return new(0.5f, 0.5f);

        float keyMin = points.Min(p => p.Value);
        float keyMax = points.Max(p => p.Value);
        float keyMid = (keyMin + keyMax) / 2f;
        float curveMid = (minValue + maxValue) / 2f;

        float panY = 0.5f + (curveMid - keyMid) / (keyMax - keyMin) * 0.5f;
        return new(0.5f, Math.Max(0.01f, Math.Min(0.99f, panY)));
    }

    public void UpdateGraphBounds()
    {
        var (minTime, maxTime, minValue, maxValue) = CalculateDataRange();

        if (maxTime - minTime < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            minTime -= Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO / 2f;
            maxTime += Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO / 2f;
        }
        if (maxValue - minValue < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            minValue -= Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO / 2f;
            maxValue += Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO / 2f;
        }

        PanCenter = new(
            Math.Max(0.01f, Math.Min(0.99f, PanCenter.X)),
            Math.Max(0.01f, Math.Min(0.99f, PanCenter.Y))
        );

        float timePadding = (maxTime - minTime) * 0.1f;
        float valuePadding = (maxValue - minValue) * 0.1f;

        float dataWidth = (maxTime - minTime + 2 * timePadding) / ZoomLevel;
        float dataHeight = (maxValue - minValue + 2 * valuePadding) / ZoomLevel;

        float centerX = minTime + (maxTime - minTime) * PanCenter.X;
        float centerY = minValue + (maxValue - minValue) * PanCenter.Y;

        GraphMinTime = centerX - dataWidth / 2;
        GraphMaxTime = centerX + dataWidth / 2;
        GraphMinValue = centerY - dataHeight / 2;
        GraphMaxValue = centerY + dataHeight / 2;

        if (Math.Abs(GraphMaxTime - GraphMinTime) < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            GraphMaxTime = GraphMinTime + Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO;
        }
        if (Math.Abs(GraphMaxValue - GraphMinValue) < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            GraphMaxValue = GraphMinValue + Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO;
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (_pictureBox == null) return;

        float oldZoom = ZoomLevel;
        float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f;
        ZoomLevel = Math.Max(0.1f, Math.Min(10.0f, ZoomLevel * zoomFactor));

        if (Math.Abs(ZoomLevel - oldZoom) > 0.001f)
        {
            float cursorTime = GraphMinTime + (e.X / (float)_pictureBox.Width) * (GraphMaxTime - GraphMinTime);
            float cursorValue = GraphMaxValue - (e.Y / (float)_pictureBox.Height) * (GraphMaxValue - GraphMinValue);

            float timeRatio = (cursorTime - GraphMinTime) / (GraphMaxTime - GraphMinTime);
            float valueRatio = (GraphMaxValue - cursorValue) / (GraphMaxValue - GraphMinValue);

            PanCenter = new(
                PanCenter.X + (timeRatio - 0.5f) * (1 - oldZoom / ZoomLevel),
                PanCenter.Y + (valueRatio - 0.5f) * (1 - oldZoom / ZoomLevel)
            );

            PanCenter = new(
                Math.Max(0.01f, Math.Min(0.99f, PanCenter.X)),
                Math.Max(0.01f, Math.Min(0.99f, PanCenter.Y))
            );

            ViewChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e?.Button == MouseButtons.Left)
        {
            if (_editorService.Points.Count == 0) return;
            int hitIndex = HitTestPoint(e.Location);
            if (hitIndex >= 0 && !_isPanning)
            {
                _isDraggingPoint = true;
                _draggedPointIndex = hitIndex;
                _hoveredPointIndex = hitIndex;
                _pictureBox.Cursor = Cursors.Hand;
                Cursor.Clip = _pictureBox.RectangleToScreen(_pictureBox.ClientRectangle);

                _originalPoint = _editorService.Points[hitIndex];
                DragStarted?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (!_isDraggingPoint)
            {
                _isPanning = true;
                _panDragStart = e.Location;
                _pictureBox.Cursor = Cursors.SizeAll;
            }
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (_isDraggingPoint && _draggedPointIndex >= 0)
        {
            float newTime = GraphMinTime + (e.X / (float)_pictureBox.Width) * (GraphMaxTime - GraphMinTime);
            float newValue = GraphMaxValue - (e.Y / (float)_pictureBox.Height) * (GraphMaxValue - GraphMinValue);

            newTime = Math.Max(0f, Math.Min(newTime, Constants.MAX_REASONABLE_VALUE));
            newValue = Math.Max(-Constants.MAX_REASONABLE_VALUE, Math.Min(newValue, Constants.MAX_REASONABLE_VALUE));

            if (_originalPoint != null)
            {
                var points = _editorService.PointsInternal;
                var newPoint = _originalPoint with { Time = newTime, Value = newValue };

                // NULL SAFETY CHECK - prevent corruption
                if (newPoint != null && _draggedPointIndex >= 0 && _draggedPointIndex < points.Count)
                {
                    points[_draggedPointIndex] = newPoint;
                    _pictureBox.Refresh();
                }
            }
        }
        else if (_isPanning)
        {
            float timePerPixel = (GraphMaxTime - GraphMinTime) / _pictureBox.Width;
            float valuePerPixel = (GraphMaxValue - GraphMinValue) / _pictureBox.Height;

            float deltaX = (e.X - _panDragStart.X) * timePerPixel / ZoomLevel;
            float deltaY = (e.Y - _panDragStart.Y) * valuePerPixel / ZoomLevel;

            PanCenter = new(
                PanCenter.X - deltaX / (GraphMaxTime - GraphMinTime),
                PanCenter.Y + deltaY / (GraphMaxValue - GraphMinValue)
            );

            _panDragStart = e.Location;
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            int newHoveredIndex = HitTestPoint(e.Location);
            if (newHoveredIndex != _hoveredPointIndex)
            {
                _hoveredPointIndex = newHoveredIndex;
                _pictureBox.Invalidate();
            }

            _pictureBox.Cursor = (_hoveredPointIndex >= 0) ? Cursors.Hand : Cursors.Cross;
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (_isDraggingPoint)
        {
            _isDraggingPoint = false;
            _draggedPointIndex = -1;
            _pictureBox.Cursor = Cursors.Cross;
            _hoveredPointIndex = -1;
            Cursor.Clip = Rectangle.Empty;

            DragEnded?.Invoke(this, EventArgs.Empty);
            _originalPoint = null;
        }
        else if (_isPanning)
        {
            _isPanning = false;
            _pictureBox.Cursor = Cursors.Cross;
        }
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e?.Button == MouseButtons.Right)
        {
            float time = GraphMinTime + (e.X / (float)_pictureBox.Width) * (GraphMaxTime - GraphMinTime);
            float newValue = GraphMaxValue - (e.Y / (float)_pictureBox.Height) * (GraphMaxValue - GraphMinValue);

            var newPoint = new FloatString4(time, newValue, 0f, 0f);
            _editorService.AddPoint(newPoint);
            DragEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    private int HitTestPoint(Point mousePos)
    {
        var points = _editorService.Points;
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            float x = (point.Time - GraphMinTime) * _pictureBox.Width / (GraphMaxTime - GraphMinTime);
            float y = _pictureBox.Height - ((point.Value - GraphMinValue) * _pictureBox.Height / (GraphMaxValue - GraphMinValue));

            float dx = mousePos.X - x;
            float dy = mousePos.Y - y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance <= Constants.Visual.HIT_TEST_RADIUS)
                return i;
        }
        return -1;
    }

    private (float minTime, float maxTime, float minValue, float maxValue) CalculateDataRange()
    {
        var points = _editorService.Points;

        if (points.Count == 0)
        {
            return (-0.5f, 9.5f, -0.5f, 1.5f);
        }

        float minTime = points.Min(p => p.Time);
        float maxTime = points.Max(p => p.Time);
        float minValue = points.Min(p => p.Value);
        float maxValue = points.Max(p => p.Value);

        var curve = _getCurve();
        if (curve != null && points.Count > 1)
        {
            curve.FindMinMaxValue(out minValue, out maxValue, out _, out _);
        }

        if (maxTime - minTime < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            minTime -= 0.5f;
            maxTime += 0.5f;
        }
        if (maxValue - minValue < Constants.Visual.MIN_RANGE_BEFORE_DIVIDE_BY_ZERO)
        {
            minValue -= 0.5f;
            maxValue += 0.5f;
        }

        return (minTime, maxTime, minValue, maxValue);
    }

    public void ResetZoom()
    {
        var (minTime, maxTime, minValue, maxValue) = CalculateDataRange();

        ZoomLevel = CalculateOptimalZoom(minTime, maxTime, minValue, maxValue);
        PanCenter = CalculateOptimalPan(minTime, maxTime, minValue, maxValue);

        UpdateGraphBounds();
        ViewChanged?.Invoke(this, EventArgs.Empty);
        _pictureBox?.Invalidate();
    }

    public void Dispose()
    {
        _renderer.Dispose();
    }
}

public class PointDragEventArgs(int index, FloatString4 newPoint) : EventArgs
{
    public int Index { get; } = index;
    public FloatString4 NewPoint { get; } = newPoint;
}