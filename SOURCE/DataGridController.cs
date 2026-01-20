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
* See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html      or the LICENSE file for full terms.
*/

#nullable enable

using System;
using System.Windows.Forms;

namespace KSPCurveBuilder;

/// <summary>
/// Handles DataGridView cell editing and delegates drag operations.
/// </summary>
public sealed class DataGridController
{
    private readonly DataGridView _grid;
    private readonly CurveEditorService _editorService;
    private readonly GridDragHandler _dragHandler;

    private bool _ignoreChanges = false;

    public event EventHandler? CellValueChanged;
    public event EventHandler<PointRemovedEventArgs>? PointRemoved;
    public event EventHandler<GridCellEditedEventArgs>? GridCellEdited;

    public DataGridController(DataGridView grid, CurveEditorService editorService, PictureBox pictureBox)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _ = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));

        ConfigureGridBehavior();

        _dragHandler = new GridDragHandler(grid, editorService);

        WireUpEvents();
        CreateGridColumns();

        BindDataSource();
    }

    private void WireUpEvents()
    {
        _grid.CellValueChanged += OnCellValueChanged;
        _grid.CellFormatting += OnCellFormatting;
        _grid.CellClick += OnCellClick;
        _grid.CellValidating += OnCellValidating;
        _grid.MouseDown += _dragHandler.OnMouseDown;
        _grid.MouseMove += _dragHandler.OnMouseMove;
        _grid.MouseUp += _dragHandler.OnMouseUp;
        _grid.DataError += (s, e) => { e.ThrowException = false; e.Cancel = false; };

        _dragHandler.DragCompleted += OnDragCompleted;
    }

    private void ConfigureGridBehavior()
    {
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.CellSelect; // Allows cell editing
        _grid.RowHeadersVisible = false;
        _grid.ReadOnly = false; // Explicitly allow editing
        _grid.EditMode = DataGridViewEditMode.EditOnEnter; // Start editing on single click
    }
    private void BindDataSource()
    {
        _grid.DataSource = null;
        _grid.DataSource = _editorService.PointsInternal; // Bind directly to internal list
    }

    private void OnCellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e == null || _ignoreChanges || e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (_editorService.Points.Count <= e.RowIndex) return;

        var cellValue = _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
        if (string.IsNullOrEmpty(cellValue)) return;

        var oldPoint = _editorService.Points[e.RowIndex];

        if (!float.TryParse(cellValue, out float parsedValue))
        {
            _grid.Rows[e.RowIndex].ErrorText = "Must be a valid number";
            return;
        }

        _grid.Rows[e.RowIndex].ErrorText = "";

        FloatString4 newPoint = e.ColumnIndex switch
        {
            0 => oldPoint with { Time = parsedValue },
            1 => oldPoint with { Value = parsedValue },
            2 => oldPoint with { InTangent = parsedValue },
            3 => oldPoint with { OutTangent = parsedValue },
            _ => oldPoint
        };

        GridCellEdited?.Invoke(this, new GridCellEditedEventArgs(e.RowIndex, oldPoint, newPoint));

        if (e.ColumnIndex == 0)
        {
            _editorService.SortByTime();
        }

        CellValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e == null || e.RowIndex < 0 || e.ColumnIndex != 4) return; // RemoveButton column

        if (MessageBox.Show("Delete this point?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            PointRemoved?.Invoke(this, new PointRemovedEventArgs(e.RowIndex));
        }
    }

    private void OnCellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (e == null || e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex > 3) return;

        string? formattedValue = e.FormattedValue?.ToString();
        if (!float.TryParse(formattedValue ?? "", out _))
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

    private void OnDragCompleted(object? sender, PointDraggedEventArgs e)
    {
        GridCellEdited?.Invoke(this, new GridCellEditedEventArgs(e.Index, e.OldPoint, e.NewPoint));
    }

    private void CreateGridColumns()
    {
        _grid.Columns.Clear();

        AddTextColumn("Time", nameof(FloatString4.Time), Constants.UI.COLUMN_WIDTH_TIME);
        AddTextColumn("Value", nameof(FloatString4.Value), Constants.UI.COLUMN_WIDTH_VALUE);
        AddTextColumn("InTan", nameof(FloatString4.InTangent), Constants.UI.COLUMN_WIDTH_TANGENT);
        AddTextColumn("OutTan", nameof(FloatString4.OutTangent), Constants.UI.COLUMN_WIDTH_TANGENT);
        AddButtonColumn("Remove", "X", Constants.UI.COLUMN_WIDTH_REMOVE);

        _grid.CellFormatting += OnCellFormatting;
    }

    private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e == null || e.RowIndex < 0 || e.ColumnIndex > 3) return;

        if (e.Value is not float value) return;

        string format = e.ColumnIndex switch
        {
            0 => Formatting.TIME_DECIMAL_PLACES,
            1 => Formatting.VALUE_DECIMAL_PLACES,
            2 or 3 => Formatting.TANGENT_SIGNIFICANT_FIGURES,
            _ => "G"
        };

        e.Value = FloatString4.FormatNumber(value, format);
        e.FormattingApplied = true;
    }

    private void AddTextColumn(string header, string property, int width)
    {
        var column = new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            DataPropertyName = property,
            Width = width,
            ReadOnly = false, // Explicitly make column editable
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };
        _grid.Columns.Add(column);
    }

    private void AddButtonColumn(string header, string text, int width)
    {
        _grid.Columns.Add(new DataGridViewButtonColumn
        {
            HeaderText = header,
            Text = text,
            UseColumnTextForButtonValue = true,
            Width = width,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
    }
}