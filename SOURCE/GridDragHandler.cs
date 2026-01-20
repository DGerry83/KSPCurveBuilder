/* 
* KSPCurveBuilder - A standalone float curve editing tool.
* 
* This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
* Logic from that original project is used here and throughout.
* 
* Original work copyright © 2015 Sarbian (https://github.com/sarbian     ).
* Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/     ).
* 
* This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
* See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html     or the LICENSE file for full terms.
*/

#nullable enable

using System;
using System.Drawing;
using System.Windows.Forms;

namespace KSPCurveBuilder;

/// <summary>
/// Handles drag-to-edit functionality for DataGridView cells.
/// </summary>
public sealed class GridDragHandler
{
    private readonly DataGridView _grid;
    private readonly CurveEditorService _editorService;

    private bool _isMouseDown = false;
    private bool _isDragging = false;
    private int _dragRowIndex = -1;
    private int _dragColumnIndex = -1;
    private Point _dragStartPos;
    private FloatString4? _originalPoint;
    private FloatString4? _currentDragPoint;
    private float _lastDragValue;
    private const int DRAG_THRESHOLD = 2;

    public bool IsDragging => _isDragging;

    public event EventHandler<PointDraggedEventArgs>? DragCompleted;
    public event EventHandler? DragAborted;

    public GridDragHandler(DataGridView grid, CurveEditorService editorService)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
    }

    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        var hit = _grid.HitTest(e.X, e.Y);
        if (hit.Type == DataGridViewHitTestType.Cell &&
            hit.ColumnIndex >= 0 && hit.ColumnIndex <= 3 &&
            hit.RowIndex >= 0 && hit.RowIndex < _editorService.Points.Count)
        {
            _isMouseDown = true;
            _dragRowIndex = hit.RowIndex;
            _dragColumnIndex = hit.ColumnIndex;
            _dragStartPos = e.Location;
            _originalPoint = _editorService.Points[_dragRowIndex];
            _currentDragPoint = _originalPoint;
            _lastDragValue = GetValueFromPoint(_originalPoint, _dragColumnIndex);
            _grid.Cursor = Cursors.HSplit;
        }
    }

    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isMouseDown || _dragRowIndex < 0 || _currentDragPoint == null) return;

        if (!_isDragging)
        {
            float distance = Math.Abs(e.X - _dragStartPos.X) + Math.Abs(e.Y - _dragStartPos.Y);
            if (distance < DRAG_THRESHOLD) return;

            _isDragging = true;
            _grid.Capture = true;
            _dragStartPos = e.Location;
        }

        var screenPos = _grid.PointToScreen(e.Location);
        var screenBounds = Screen.FromControl(_grid).Bounds;
        if (WarpCursorAtScreenEdges(screenPos, screenBounds))
        {
            _dragStartPos = _grid.PointToClient(Cursor.Position);
            return;
        }

        float mouseDelta = _dragStartPos.Y - e.Y;
        float valueChange = CalculateDragValueChange(_lastDragValue, mouseDelta, _dragColumnIndex);
        float newValue = ClampDragValue(_lastDragValue + valueChange, _dragColumnIndex);

        FloatString4 newPoint = CreateNewPoint(_currentDragPoint, _dragColumnIndex, newValue);

        _currentDragPoint = newPoint;
        _lastDragValue = newValue;

        // CRITICAL: Only update internal list and PictureBox - NO direct grid cell manipulation
        UpdatePointTemporarily(_dragRowIndex, _currentDragPoint);

        // NO DragCompleted event during drag - only on mouse up

        _dragStartPos = e.Location;
    }

    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (!_isMouseDown) return;

        // FINALIZE: Only fire DragCompleted once at the end
        if (_isDragging && _originalPoint != null && _currentDragPoint != null)
        {
            DragCompleted?.Invoke(this, new PointDraggedEventArgs(
                _dragRowIndex, _originalPoint, _currentDragPoint
            ));
        }
        // Otherwise it was just a click - grid enters edit mode naturally

        Cleanup();
    }

    private void UpdatePointTemporarily(int index, FloatString4 point)
    {
        var points = _editorService.PointsInternal;
        if (index >= 0 && index < points.Count)
        {
            points[index] = point;

            // Only trigger PictureBox update, no grid events
            _editorService.TriggerSilentPointsChanged();

            if (IsTimeColumn(_dragColumnIndex))
            {
                points.Sort();
                _dragRowIndex = points.IndexOf(point);
            }
        }
    }

    private void Cleanup()
    {
        _isMouseDown = false;
        _isDragging = false;
        _dragRowIndex = -1;
        _dragColumnIndex = -1;
        _originalPoint = null;
        _currentDragPoint = null;
        _lastDragValue = 0f;
        _grid.Capture = false;
        _grid.Cursor = Cursors.Default;
    }

    private static float GetValueFromPoint(FloatString4 point, int columnIndex) => columnIndex switch
    {
        0 => point.Time,
        1 => point.Value,
        2 => point.InTangent,
        3 => point.OutTangent,
        _ => 0f
    };

    private static FloatString4 CreateNewPoint(FloatString4 basePoint, int columnIndex, float newValue) => columnIndex switch
    {
        0 => basePoint with { Time = newValue },
        1 => basePoint with { Value = newValue },
        2 => basePoint with { InTangent = newValue },
        3 => basePoint with { OutTangent = newValue },
        _ => basePoint
    };

    private static bool IsTimeColumn(int columnIndex) => columnIndex == 0;

    private static bool WarpCursorAtScreenEdges(Point screenPos, Rectangle screenBounds)
    {
        if (screenPos.Y <= screenBounds.Top + Constants.DRAG_EDGE_THRESHOLD)
        {
            Cursor.Position = new(screenPos.X, screenBounds.Bottom - Constants.DRAG_WARP_DISTANCE);
            return true;
        }
        if (screenPos.Y >= screenBounds.Bottom - Constants.DRAG_EDGE_THRESHOLD)
        {
            Cursor.Position = new(screenPos.X, screenBounds.Top + Constants.DRAG_WARP_DISTANCE);
            return true;
        }
        return false;
    }

    private static float CalculateDragValueChange(float baseValue, float mouseDeltaY, int columnIndex)
    {
        Func<float, float> getDragRate = v =>
        {
            float absV = Math.Max(Math.Abs(v), 1e-10f);
            float rate = absV / (absV + Constants.DRAG_REFERENCE) * Constants.MAX_RATE_MULTIPLIER;
            return Math.Max(Constants.DRAG_MIN_RATE, rate);
        };

        float columnMult = (columnIndex == 2 || columnIndex == 3) ? Constants.TANGENT_MULTIPLIER : 1.0f;
        float speedMultiplier = DragSpeedCalculator.GetSpeedMultiplier();
        float rawChange = mouseDeltaY * Constants.DRAG_SENSITIVITY * columnMult * speedMultiplier;

        return rawChange * getDragRate(baseValue);
    }

    private static float ClampDragValue(float value, int columnIndex) => columnIndex switch
    {
        0 => Math.Max(0f, Math.Min(value, 1e6f)),
        1 => Math.Max(-1e6f, Math.Min(value, 1e6f)),
        2 or 3 => Math.Max(-1e4f, Math.Min(value, 1e4f)),
        _ => value
    };
}