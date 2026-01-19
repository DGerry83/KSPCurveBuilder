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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSPCurveBuilder;

/// <summary>
/// Main form - coordinates UI with services while preserving original textbox-centric workflow.
/// </summary>
public partial class KSPCurveBuilder : Form
{
    private readonly List<FloatString4> _points = [];
    private readonly BindingList<FloatString4> _bindingList;
    private readonly CurveEditorService _editorService;
    private readonly CurveViewController _viewController;
    private readonly DataGridController _gridController;
    private readonly PresetService _presetService;
    private readonly UndoService _undoService;
    private bool _suppressUndoRecording = false;

    private string _preDragState = "";

    public KSPCurveBuilder()
    {
        InitializeComponent();
        this.Icon = new Icon(GetType(), "CurveIcon.ico");
        _bindingList = new BindingList<FloatString4>(_points);
        _editorService = new CurveEditorService(_points, _bindingList);
        _viewController = new CurveViewController(curveView, _editorService,
            () => _editorService.CreateCurveFromPoints());

        _gridController = new DataGridController(dataPointEditor, _editorService, curveView);

        _presetService = new PresetService();
        _undoService = new UndoService(curveText?.Text ?? "");

        WireUpEvents();
        SetupDataGridView();
        SetupPictureBox();
        //LoadPresetList();
        LoadDefaultPreset();

        SyncPointsToTextbox("Initial Load");
        UpdateUndoButtons();

        //if (presetDropdown != null)
        //{
        //    presetDropdown.SelectedIndexChanged += PresetDropdown_SelectedIndexChanged;
        //}
    }

    private void WireUpEvents()
    {
        _editorService.SilentPointsChanged += (s, e) => curveView?.Refresh();

        _editorService.PointsChanged += (s, e) =>
        {
            SyncPointsToTextbox("Edit");
            UpdateUndoState();
            curveView?.Invalidate();
        };

        _gridController.CellValueChanged += (s, e) =>
        {
            SyncPointsToTextbox("Edit");
            UpdateUndoState();
        };
        _undoService.StateChanged += (s, e) => UpdateUndoButtons();
        _viewController.ViewChanged += (s, e) => curveView?.Invalidate();
        _viewController.DragStarted += OnDragStarted;
        _viewController.DragEnded += OnDragEnded;
        _viewController.PointDragged += OnPointDragged;

        buttonNewCurve.Click += (s, e) => NewCurve();
        buttonSmooth.Click += (s, e) => SmoothTangents();
        buttonCopy.Click += (s, e) => CopyToClipboard();
        buttonPaste.Click += (s, e) => PasteFromClipboard();
        buttonAddNode.Click += (s, e) => AddNode();
        buttonSavePreset.Click += async (s, e) => await SavePresetAsync();
        buttonDeletePreset.Click += async (s, e) => await DeletePresetAsync();
        presetDropdown.SelectedIndexChanged += async (s, e) => await PresetDropdown_SelectedIndexChanged();
        buttonResetZoom.Click += (s, e) => _viewController.ResetZoom();
        checkBoxSort.CheckedChanged += (s, e) => ToggleSort();
        buttonUndo.Click += (s, e) => Undo();
        buttonRedo.Click += (s, e) => Redo();

        this.Load += async (s, e) => await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadPresetListAsync();
        // Keep UI responsive while presets load
    }

    private void OnDragStarted(object? sender, EventArgs e)
    {
        _preDragState = _editorService.SerializeToText();
    }

    private void OnDragEnded(object? sender, EventArgs e)
    {
        _bindingList.ResetBindings();

        var postDragState = _editorService.SerializeToText();
        if (postDragState != _preDragState)
        {
            _undoService.RecordAction(postDragState, "Move Point");
        }

        SyncPointsToTextbox("Move Point");
    }

    private void SetupDataGridView()
    {
        ConfigureGridBehavior();
        CreateGridColumns();
        BindDataSource();
    }

    private void ConfigureGridBehavior()
    {
        dataPointEditor.AutoGenerateColumns = false;
        dataPointEditor.AllowUserToAddRows = false;
        dataPointEditor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataPointEditor.RowHeadersVisible = false;
    }

    private void CreateGridColumns()
    {
        AddTextColumn("Time", "TimeString", 60);
        AddTextColumn("Value", "ValueString", 80);
        AddTextColumn("InTan", "InTangentString", 60);
        AddTextColumn("OutTan", "OutTangentString", 60);
        AddButtonColumn("Remove", "X", 70);
    }

    private void AddTextColumn(string header, string property, int width)
    {
        dataPointEditor.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            DataPropertyName = property,
            Width = width,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
    }

    private void AddButtonColumn(string header, string text, int width)
    {
        dataPointEditor.Columns.Add(new DataGridViewButtonColumn
        {
            HeaderText = header,
            Text = text,
            UseColumnTextForButtonValue = true,
            Width = width,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
    }

    private void BindDataSource()
    {
        dataPointEditor.DataSource = _bindingList;
    }

    private void SetupPictureBox()
    {
        typeof(PictureBox).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic,
            null, curveView, [true]);
    }

    private void LoadDefaultPreset()
    {
        var defaultPreset = _presetService.GetDefaultPreset();
        if (defaultPreset?.Points != null)
        {
            _editorService.LoadFromPoints(defaultPreset.Points);
            presetNameTextbox.Text = defaultPreset.Name;
            SyncPointsToTextbox("Load Preset");
            UpdateUndoState();
            _viewController.ResetZoom();
            curveView?.Invalidate();
        }
    }

    private async Task LoadPresetListAsync()
    {
        if (presetDropdown == null) return;

        presetDropdown.Enabled = false;

        try
        {
            var presets = await _presetService.GetAllPresetsAsync();
            presetDropdown.Items.Clear();
            foreach (var preset in presets)
            {
                presetDropdown.Items.Add(preset);
            }
            presetDropdown.DisplayMember = "Name";
        }
        finally
        {
            presetDropdown.Enabled = true;
        }
    }


    private void OnPointDragged(object? sender, PointDragEventArgs e)
    {
        _editorService.UpdatePoint(e.Index, e.NewPoint);
    }

    private void NewCurve()
    {
        _editorService.ClearPoints();
        SyncPointsToTextbox("New Curve");
        _viewController.ResetZoom();
        curveView?.Invalidate();
    }

    private void SmoothTangents()
    {
        _editorService.SmoothTangents();
        SyncPointsToTextbox("Smooth Tangents");
    }

    private void CopyToClipboard()
    {
        var text = curveText?.Text ?? "";
        if (!string.IsNullOrEmpty(text))
        {
            Clipboard.SetText(text);
            MessageBox.Show("Curve data copied to clipboard!", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void PasteFromClipboard()
    {
        try
        {
            string clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                MessageBox.Show("Clipboard is empty.", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _editorService.DeserializeFromText(clipboardText);
            SyncPointsToTextbox("Paste");
            _viewController.ResetZoom();
            curveView?.Invalidate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to paste: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddNode()
    {
        var points = _editorService.Points;
        FloatString4 newPoint;

        if (points.Count > 0)
        {
            var lastPoint = points[^1];
            newPoint = new FloatString4(lastPoint.Time + 10, lastPoint.Value, 0f, 0f);
        }
        else
        {
            newPoint = new FloatString4(0f, 0f, 0f, 0f);
        }

        _editorService.AddPoint(newPoint);

        if (checkBoxSort.Checked)
        {
            _editorService.SortByTime();
        }

        SyncPointsToTextbox("Add Node");
    }

    private async void SavePreset(object? sender, EventArgs e)
    {
        if (presetNameTextbox == null) return;

        string name = presetNameTextbox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Enter a valid preset name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            buttonSavePreset.Enabled = false;
            buttonSavePreset.Text = "Saving...";

            await _presetService.SavePresetAsync(name, "User-created preset", _points);

            presetNameTextbox.Text = "";
            await LoadPresetListAsync();

            var newlySaved = presetDropdown.Items.Cast<Preset>().FirstOrDefault(p => p.Name == name);
            if (newlySaved != null)
            {
                presetDropdown.SelectedItem = newlySaved;
            }

            curveView?.Invalidate();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            buttonSavePreset.Enabled = true;
            buttonSavePreset.Text = "Save";
        }
    }

    private async Task SavePresetAsync()
    {
        if (presetNameTextbox == null) return;

        string name = presetNameTextbox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Enter a valid preset name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            buttonSavePreset.Enabled = false;
            buttonSavePreset.Text = "Saving...";

            await _presetService.SavePresetAsync(name, "User-created preset", _points);

            presetNameTextbox.Text = "";
            await LoadPresetListAsync();

            var newlySaved = presetDropdown.Items.Cast<Preset>().FirstOrDefault(p => p.Name == name);
            if (newlySaved != null)
            {
                presetDropdown.SelectedItem = newlySaved;
            }

            curveView?.Invalidate();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            buttonSavePreset.Enabled = true;
            buttonSavePreset.Text = "Save";
        }
    }

    private async Task DeletePresetAsync()
    {
        if (presetDropdown?.SelectedItem is not Preset preset) return;

        if (MessageBox.Show($"Delete preset '{preset.Name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            buttonDeletePreset.Enabled = false;
            buttonDeletePreset.Text = "Deleting...";

            await _presetService.DeletePresetAsync(preset.Name);
            await LoadPresetListAsync();

            if (presetDropdown.Items.Count > 0)
            {
                presetDropdown.SelectedIndex = 0;
                if (presetDropdown.SelectedItem is Preset defaultPreset && defaultPreset.Points != null)
                {
                    _editorService.LoadFromPoints(defaultPreset.Points);
                    presetNameTextbox.Text = defaultPreset.Name;
                    SyncPointsToTextbox($"Load Preset: {defaultPreset.Name}");
                    curveView?.Invalidate();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            buttonDeletePreset.Enabled = true;
            buttonDeletePreset.Text = "Delete";
        }
    }

    private async Task PresetDropdown_SelectedIndexChanged()
    {
        if (presetDropdown?.SelectedItem is Preset preset && preset.Points != null)
        {
            presetDropdown.Enabled = false; // Prevent rapid changes

            try
            {
                _viewController.ZoomLevel = 1.0f;
                _viewController.PanCenter = new(0.5f, 0.5f);

                _editorService.LoadFromPoints(preset.Points);
                presetNameTextbox.Text = preset.Name;
                SyncPointsToTextbox($"Load Preset: {preset.Name}");

                _viewController.ResetZoom();
                curveView?.Invalidate();
            }
            finally
            {
                presetDropdown.Enabled = true;
            }
        }
    }

    private async void DeletePreset(object? sender, EventArgs e)
    {
        if (presetDropdown?.SelectedItem is not Preset preset) return;

        if (MessageBox.Show($"Delete preset '{preset.Name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            buttonDeletePreset.Enabled = false;
            buttonDeletePreset.Text = "Deleting...";

            await _presetService.DeletePresetAsync(preset.Name);
            await LoadPresetListAsync();

            if (presetDropdown.Items.Count > 0)
            {
                presetDropdown.SelectedIndex = 0;
                if (presetDropdown.SelectedItem is Preset defaultPreset && defaultPreset.Points != null)
                {
                    _editorService.LoadFromPoints(defaultPreset.Points);
                    presetNameTextbox.Text = defaultPreset.Name;
                    SyncPointsToTextbox($"Load Preset: {defaultPreset.Name}");
                    curveView?.Invalidate();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            buttonDeletePreset.Enabled = true;
            buttonDeletePreset.Text = "Delete";
        }
    }

    private void ToggleSort()
    {
        if (checkBoxSort.Checked)
        {
            _editorService.SortByTime();
            SyncPointsToTextbox("Sort");
        }
    }

    private void UpdateUndoState()
    {
        if (_suppressUndoRecording) return;
        var text = curveText?.Text ?? "";
        _undoService.RecordAction(text, "Edit");
    }

    private void UpdateUndoButtons()
    {
        buttonUndo.Enabled = _undoService.CanUndo;
        buttonRedo.Enabled = _undoService.CanRedo;
    }

    private void Undo()
    {
        _suppressUndoRecording = true;
        try
        {
            var state = _undoService.Undo();
            if (state != null)
            {
                _editorService.DeserializeFromText(state);
                curveView?.Invalidate();
            }
        }
        finally
        {
            _suppressUndoRecording = false;
        }
    }

    private void Redo()
    {
        _suppressUndoRecording = true;
        try
        {
            var state = _undoService.Redo();
            if (state != null)
            {
                _editorService.DeserializeFromText(state);
                curveView?.Invalidate();
            }
        }
        finally
        {
            _suppressUndoRecording = false;
        }
    }

    private void SyncPointsToTextbox(string actionName)
    {
        if (curveText == null) return;
        curveText.Text = _editorService.SerializeToText();
    }

    private async void PresetDropdown_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (presetDropdown?.SelectedItem is Preset preset && preset.Points != null)
        {
            presetDropdown.Enabled = false; // Prevent rapid changes

            try
            {
                _viewController.ZoomLevel = 1.0f;
                _viewController.PanCenter = new(0.5f, 0.5f);

                _editorService.LoadFromPoints(preset.Points);
                presetNameTextbox.Text = preset.Name;
                SyncPointsToTextbox($"Load Preset: {preset.Name}");

                _viewController.ResetZoom();
                curveView?.Invalidate();
            }
            finally
            {
                presetDropdown.Enabled = true;
            }
        }
    }
}