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
using static KSPCurveBuilder.DataGridController;

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

    private string _preDragState = "";
    private int _dragPointIndex = -1;
    private FloatString4? _dragStartPoint;

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

        // CHANGE: UndoService now requires editor service reference
        _undoService = new UndoService(_editorService);

        WireUpEvents();
        SetupDataGridView();
        SetupPictureBox();
        LoadDefaultPreset();

        SyncPointsToTextbox("Initial Load");
        UpdateUndoButtons();

        _ = InitializeAsync();
    }

    private void WireUpEvents()
    {
        SetupServiceEventHandlers();
        SetupViewControllerEventHandlers();
        SetupGridControllerEventHandlers();
        SetupDragEventHandlers();
        SetupTextBoxEventHandlers();
        SetupButtonEventHandlers();
        SetupOtherUIEventHandlers();
        SetupUndoRedoEventHandlers();

        presetDropdown.SelectedIndexChanged += OnPresetDropdownChanged;

        this.Load += async (s, e) => await InitializeAsync();
    }

    private void SetupDragEventHandlers()
    {
        _viewController.DragStarted += OnDragStarted;
        _viewController.DragEnded += OnDragEnded;
    }

    private void OnDragStarted(object? sender, EventArgs e)
    {
        _dragPointIndex = _viewController.CurrentlyDraggedPointIndex;
        _dragStartPoint = _viewController.CurrentlyDraggedPointOriginal;
    }

    private void OnDragEnded(object? sender, EventArgs e)
    {
        if (_dragPointIndex >= 0 && _dragStartPoint != null)
        {
            var points = _editorService.Points;
            if (_dragPointIndex < points.Count)
            {
                var newPoint = points[_dragPointIndex];

                // Only create command if the point actually changed
                if (!ArePointsEqual(newPoint, _dragStartPoint))
                {
                    ExecuteCommand(new MovePointCommand(_editorService, _dragPointIndex, newPoint, _dragStartPoint));
                }
            }
        }
        _dragPointIndex = -1;
        _dragStartPoint = null;
    }

    private bool ArePointsEqual(FloatString4 a, FloatString4 b)
    {
        if (a == null || b == null) return false;
        return Math.Abs(a.Time - b.Time) < 0.001f &&
               Math.Abs(a.Value - b.Value) < 0.001f &&
               Math.Abs(a.InTangent - b.InTangent) < 0.001f &&
               Math.Abs(a.OutTangent - b.OutTangent) < 0.001f;
    }
    private void SetupButtonEventHandlers()
    {
        buttonNewCurve.Click += (s, e) => ExecuteCommand(new ClearPointsCommand(_editorService, [.. _points]));
        buttonSmooth.Click += (s, e) => ExecuteCommand(new SmoothTangentsCommand(_editorService, [.. _points]));
        buttonCopy.Click += (s, e) => CopyToClipboard();
        buttonPaste.Click += (s, e) => ExecuteCommand(new LoadPointsCommand(_editorService, ParseClipboard()));
        buttonAddNode.Click += (s, e) => AddNode();
        buttonSavePreset.Click += async (s, e) => await SavePresetAsync();
        buttonDeletePreset.Click += async (s, e) => await DeletePresetAsync();
        buttonResetZoom.Click += (s, e) => _viewController.ResetZoom();
        buttonUndo.Click += (s, e) => _undoService.Undo();
        buttonRedo.Click += (s, e) => _undoService.Redo();
    }
    private void SetupOtherUIEventHandlers()
    {
        checkBoxSort.CheckedChanged += OnCheckBoxSortChanged;
        // Event subscriptions moved to Designer.cs - keep only non-Designer events here
        // (such as ToggleSort if not in Designer, but it likely is too)
    }


    private async Task InitializeAsync()
    {
        await LoadPresetListAsync();
        // Keep UI responsive while presets load
    }


    private void SetupDataGridView()
    {
        ConfigureGridBehavior();
        BindDataSource();
    }


    private void ConfigureGridBehavior()
    {
        dataPointEditor.AutoGenerateColumns = false;
        dataPointEditor.AllowUserToAddRows = false;
        dataPointEditor.RowHeadersVisible = false;
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
            if (checkBoxSort.Checked)
            {
                ExecuteCommand(new SortPointsCommand(_editorService, _editorService.Points.ToList()));
            }
            presetNameTextbox.Text = defaultPreset.Name;
            SyncPointsToTextbox("Load Preset");
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

        ExecuteCommand(new AddPointCommand(_editorService, newPoint));

        if (checkBoxSort.Checked)
        {
            _editorService.SortByTime();
        }

        SyncPointsToTextbox("Add Node");
    }
    private void OnGridControllerCellEdited(object? sender, GridCellEditedEventArgs e)
    {
        ExecuteCommand(new MovePointCommand(_editorService, e.RowIndex, e.NewPoint, e.OldPoint));
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
            await RefreshPresetDropdownAsync();

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
            await RefreshPresetDropdownAsync();
            await LoadPresetListAsync();

            if (presetDropdown.Items.Count > 0)
            {
                presetDropdown.SelectedIndex = 0;
                if (presetDropdown.SelectedItem is Preset defaultPreset && defaultPreset.Points != null)
                {
                    _editorService.LoadFromPoints(defaultPreset.Points);

                    // Apply sort if checkbox is checked
                    if (checkBoxSort.Checked)
                    {
                        ExecuteCommand(new SortPointsCommand(_editorService, _editorService.Points.ToList()));
                    }

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
    private void SetupTextBoxEventHandlers()
    {
        curveText.Leave += OnCurveTextLeave;
        curveText.KeyDown += OnCurveTextKeyDown;
    }

    private void OnCurveTextLeave(object? sender, EventArgs e)
    {
        ApplyTextboxChanges();
    }

    private void OnCurveTextKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            ApplyTextboxChanges();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void ApplyTextboxChanges()
    {
        var text = curveText.Text;
        var newPoints = ParsePointsFromText(text);
        var currentPoints = _editorService.Points.ToList();

        // Only create command if points actually changed
        if (!ArePointListsEqual(newPoints, currentPoints))
        {
            ExecuteCommand(new LoadPointsCommand(_editorService, newPoints));
        }
    }

    private bool ArePointListsEqual(List<FloatString4> list1, List<FloatString4> list2)
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!ArePointsEqual(list1[i], list2[i]))
                return false;
        }

        return true;
    }
    private void ExecuteCommand(ICommand command)
    {
        try
        {
            _undoService.ExecuteCommand(command);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Command failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private List<FloatString4> ParsePointsFromText(string text)
    {
        var points = new List<FloatString4>();
        if (string.IsNullOrWhiteSpace(text)) return points;

        foreach (string line in text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            var result = CurveParser.TryParseKeyString(trimmedLine);
            if (result.Success && result.Point != null)
            {
                points.Add(result.Point);
            }
        }
        return points;
    }

    private void CreateDragCommand()
    {
        if (string.IsNullOrEmpty(_preDragState)) return;

        var pointsAfter = _editorService.SerializeToText();
        if (pointsAfter != _preDragState)
        {
            var pointsBefore = ParsePointsFromText(_preDragState);
            ExecuteCommand(new LoadPointsCommand(_editorService, pointsBefore));
            _preDragState = "";
        }
    }
    private List<FloatString4> ParseClipboard()
    {
        string text = Clipboard.GetText();
        if (string.IsNullOrWhiteSpace(text)) return [];

        return text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => CurveParser.TryParseKeyString(line.Trim()))
            .Where(r => r.Success && r.Point != null)
            .Select(r => r.Point!)
            .ToList();
    }

    private void UpdateUndoButtons()
    {
        buttonUndo.Enabled = _undoService.CanUndo;
        buttonRedo.Enabled = _undoService.CanRedo;
    }
    private void SetupViewControllerEventHandlers()
    {
        _viewController.PointAdded += OnViewControllerPointAdded;
        _viewController.ViewChanged += (s, e) =>
        {
            curveView?.Invalidate();
            curveView?.Update(); // Force immediate update for smoother interaction
        };
    }
    private void SetupServiceEventHandlers()
    {
        // Visual refresh only - no textbox sync here
        _editorService.SilentPointsChanged += (s, e) =>
        {
            curveView?.Invalidate();
            curveView?.Update(); // Force immediate paint processing during drag
        };

        _editorService.PointsChanged += (s, e) =>
        {
            curveView?.Invalidate();
            curveView?.Update(); // Force immediate paint processing
        };
    }

    private void OnViewControllerPointAdded(object? sender, PointAddedEventArgs e)
    {
        var pointsBefore = _editorService.Points.ToList();

        ExecuteCommand(new AddPointCommand(_editorService, e.Point));
    }

    private void SetupGridControllerEventHandlers()
    {
        _gridController.PointRemoved += OnGridControllerPointRemoved;
        _gridController.GridCellEdited += OnGridControllerCellEdited;
    }

    private void OnGridControllerPointRemoved(object? sender, PointRemovedEventArgs e)
    {
        ExecuteCommand(new RemovePointCommand(_editorService, e.Index));
    }

    private void SetupUndoRedoEventHandlers()
    {
        _undoService.StateChanged += (s, e) =>
        {
            UpdateUndoButtons();
            curveView?.Invalidate();
            SyncPointsToTextbox("Undo/Redo");
        };
    }

    private void SyncPointsToTextbox(string actionName)
    {
        if (curveText == null) return;
        curveText.Text = _editorService.SerializeToText();
    }

    private async void OnPresetDropdownChanged(object? sender, EventArgs e)
    {
        if (presetDropdown?.SelectedItem is Preset preset && preset.Points != null)
        {
            presetDropdown.Enabled = false; // Prevent rapid changes

            try
            {
                _viewController.ZoomLevel = 1.0f;
                _viewController.PanCenter = new(0.5f, 0.5f);

                _editorService.LoadFromPoints(preset.Points);

                // Apply sort if checkbox is checked
                if (checkBoxSort.Checked)
                {
                    ExecuteCommand(new SortPointsCommand(_editorService, _editorService.Points.ToList()));
                }

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

    private void OnCheckBoxSortChanged(object? sender, EventArgs e)
    {
        if (checkBoxSort.Checked)
        {
            // Sort immediately when checkbox is enabled
            ExecuteCommand(new SortPointsCommand(_editorService, _editorService.Points.ToList()));
        }
    }
    /// <summary>Forces the preset dropdown to reload its items from the PresetService</summary>
    public async Task RefreshPresetDropdownAsync()
    {
        // Disable dropdown while loading
        presetDropdown.Enabled = false;

        try
        {
            // Clear items
            presetDropdown.Items.Clear();

            // Load presets (this is the async operation)
            var presets = await _presetService.GetAllPresetsAsync();

            // Repopulate
            foreach (var preset in presets)
            {
                presetDropdown.Items.Add(preset);
            }

            // Set display member AFTER adding items
            presetDropdown.DisplayMember = "Name";

            // Force UI to refresh
            presetDropdown.Invalidate();

            // Re-select default if nothing is selected
            if (presetDropdown.SelectedItem == null && presetDropdown.Items.Count > 0)
            {
                presetDropdown.SelectedIndex = 0;
            }
        }
        finally
        {
            presetDropdown.Enabled = true;
        }
    }
}