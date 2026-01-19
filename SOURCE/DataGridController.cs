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
using System.Windows.Forms;

namespace KSPCurveBuilder;

/// <summary>
/// Handles DataGridView cell editing and drag-to-edit functionality.
/// Restores original performance: no binding updates during drag.
/// </summary>
public class DataGridController
{
    private readonly DataGridView _grid;
    private readonly CurveEditorService _editorService;
    private readonly PictureBox _pictureBox;

    private bool _ignoreChanges = false;
    private bool _isDragging = false;
    private int _dragRowIndex = -1;
    private int _dragColumnIndex = -1;
    private Point _dragStartPos;

    public event EventHandler? CellValueChanged;

    // CORRECT: Classic constructor
    public DataGridController(DataGridView grid, CurveEditorService editorService, PictureBox pictureBox)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _pictureBox = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));

        _grid.CellValueChanged += OnCellValueChanged;
        _grid.CellClick += OnCellClick;
        _grid.CellValidating += OnCellValidating;
        _grid.MouseDown += OnMouseDown;
        _grid.MouseMove += OnMouseMove;
        _grid.MouseUp += OnMouseUp;
        _grid.CellMouseEnter += OnCellMouseEnter;
        _grid.CellMouseLeave += OnCellMouseLeave;
        _grid.DataError += (s, e) => { e.ThrowException = false; e.Cancel = false; };
    }

    private void OnCellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_ignoreChanges || e?.RowIndex < 0 || e?.ColumnIndex < 0) return;
        if (_editorService.Points.Count <= e.RowIndex) return;
        if (_grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null) return;

        var oldPoint = _editorService.Points[e.RowIndex];
        object cellValue = _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

        if (!float.TryParse(cellValue.ToString(), out float parsedValue))
        {
            _grid.Rows[e.RowIndex].ErrorText = "Must be a valid number";
            return;
        }

        _grid.Rows[e.RowIndex].ErrorText = "";

        FloatString4 newPoint;
        switch ((DataGridColumn)e.ColumnIndex)
        {
            case DataGridColumn.Time:
                newPoint = oldPoint.WithTime(parsedValue);
                break;
            case DataGridColumn.Value:
                newPoint = oldPoint.WithValue(parsedValue);
                break;
            case DataGridColumn.InTangent:
                newPoint = oldPoint.WithTangents(parsedValue, oldPoint.OutTangent);
                break;
            case DataGridColumn.OutTangent:
                newPoint = oldPoint.WithTangents(oldPoint.InTangent, parsedValue);
                break;
            default:
                return;
        }

        _editorService.UpdatePoint(e.RowIndex, newPoint);

        if (e.ColumnIndex == (int)DataGridColumn.Time)
        {
            _editorService.SortByTime();
        }

        CellValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e?.RowIndex < 0 || e?.ColumnIndex != (int)DataGridColumn.RemoveButton) return;

        if (MessageBox.Show("Delete this point?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            _editorService.RemovePoint(e.RowIndex);
            CellValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (e?.RowIndex < 0 || e?.ColumnIndex < 0 || e.ColumnIndex > 3) return;
        if (_editorService.Points.Count <= e.RowIndex) return;

        if (!float.TryParse(e.FormattedValue?.ToString() ?? "", out _))
        {
            var point = _editorService.Points[e.RowIndex];
            float originalValue = e.ColumnIndex switch
            {
                0 => point.Time,
                1 => point.Value,
                2 => point.InTangent,
                3 => point.OutTangent,
                _ => 0f
            };

            _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
            _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }
        else
        {
            _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }

        e.Cancel = false;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e?.Button != MouseButtons.Left) return;
        if (_grid.IsCurrentCellInEditMode)
        {
            _grid.EndEdit();
            return;
        }

        var hit = _grid.HitTest(e.X, e.Y);
        if (hit.Type == DataGridViewHitTestType.Cell &&
            hit.ColumnIndex >= 0 && hit.ColumnIndex <= 3 &&
            hit.RowIndex >= 0 && hit.RowIndex < _editorService.Points.Count)
        {
            _isDragging = true;
            _dragRowIndex = hit.RowIndex;
            _dragColumnIndex = hit.ColumnIndex;
            _dragStartPos = e.Location;

            _grid.ClearSelection();
            _grid.Capture = true;
            _grid.Cursor = Cursors.HSplit;
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDragging || _dragRowIndex < 0) return;

        var screenPos = _grid.PointToScreen(e.Location);
        var screenBounds = Screen.FromControl(_grid).Bounds;
        if (WarpCursorAtScreenEdges(screenPos, screenBounds))
        {
            _dragStartPos = _grid.PointToClient(Cursor.Position);
            return;
        }

        var oldPoint = _editorService.Points[_dragRowIndex];
        float currentValue = _dragColumnIndex switch
        {
            0 => oldPoint.Time,
            1 => oldPoint.Value,
            2 => oldPoint.InTangent,
            3 => oldPoint.OutTangent,
            _ => 0f
        };

        float mouseDelta = _dragStartPos.Y - e.Y;
        float valueChange = CalculateDragValueChange(currentValue, mouseDelta, _dragColumnIndex);
        float newValue = ClampDragValue(currentValue + valueChange, _dragColumnIndex);

        FloatString4 newPoint;
        switch (_dragColumnIndex)
        {
            case 0:
                newPoint = oldPoint.WithTime(newValue);
                break;
            case 1:
                newPoint = oldPoint.WithValue(newValue);
                break;
            case 2:
                newPoint = oldPoint.WithTangents(newValue, oldPoint.OutTangent);
                break;
            case 3:
                newPoint = oldPoint.WithTangents(oldPoint.InTangent, newValue);
                break;
            default:
                return;
        }

        var points = _editorService.PointsInternal;
        points[_dragRowIndex] = newPoint;

        if (_dragColumnIndex == 0)
        {
            points.Sort();
        }

        _pictureBox.Refresh();

        _dragStartPos = e.Location;
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;

            var points = _editorService.PointsInternal;
            if (_dragRowIndex >= 0 && _dragRowIndex < points.Count)
            {
                _editorService.UpdatePoint(_dragRowIndex, points[_dragRowIndex]);
            }

            _dragRowIndex = -1;
            _grid.Capture = false;
            _grid.Cursor = Cursors.Default;

            CellValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCellMouseEnter(object? sender, DataGridViewCellEventArgs e)
    {
        if (e?.ColumnIndex >= 0 && e.ColumnIndex <= 3 &&
            e.RowIndex >= 0 && !_isDragging)
        {
            _grid.Cursor = Cursors.HSplit;
        }
    }

    private void OnCellMouseLeave(object? sender, DataGridViewCellEventArgs e)
    {
        if (!_isDragging)
        {
            _grid.Cursor = Cursors.Default;
        }
    }

    private bool WarpCursorAtScreenEdges(Point screenPos, Rectangle screenBounds)
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

    private float CalculateDragValueChange(float baseValue, float mouseDeltaY, int columnIndex)
    {
        Func<float, float> getDragRate = (v) =>
            v < 0.001f ? Constants.DRAG_MIN_RATE : v / (v + Constants.DRAG_REFERENCE) * Constants.MAX_RATE_MULTIPLIER;

        float columnMult = (columnIndex == (int)DataGridColumn.InTangent ||
                            columnIndex == (int)DataGridColumn.OutTangent) ? Constants.TANGENT_MULTIPLIER : 1.0f;

        float speedMultiplier = DragSpeedCalculator.GetSpeedMultiplier();
        float rawChange = mouseDeltaY * Constants.DRAG_SENSITIVITY * columnMult * speedMultiplier;
        float currentRate = getDragRate(Math.Abs(baseValue));

        return rawChange * currentRate;
    }

    private float ClampDragValue(float value, int columnIndex) => columnIndex switch
    {
        0 => Math.Max(0f, Math.Min(value, 1e6f)),
        1 => Math.Max(-1e6f, Math.Min(value, 1e6f)),
        2 or 3 => Math.Max(-1e4f, Math.Min(value, 1e4f)),
        _ => value
    };
}