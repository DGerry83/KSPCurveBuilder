/* 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that project is used here and throughout
 * Modified and restructured by Karl Kreegland in 2026.
 * Licensed under the GNU General Public License v2.0.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace KSPCurveBuilder
{
    /// <summary>
    /// Main editor form for creating and editing float curves with support for
    /// saving/loading presets, real-time curve visualization, and interactive editing.
    /// Includes pan/zoom functionality for the graph view and drag-to-edit controls
    /// for curve points.
    /// </summary>
    public partial class KSPCurveBuilder : Form
    {
        // Data storage
        private List<FloatString4> points = new List<FloatString4>();
        private BindingList<FloatString4> pointsBindingList;

        // Drawing settings
        private Pen curvePen = new Pen(Color.LimeGreen, 2f);
        private Pen gridPen = new Pen(Color.FromArgb(60, 60, 60), 1f);
        private Pen pointPen = new Pen(Color.White, 2f);
        private Brush pointBrush = Brushes.White;
        private Font gridFont = new Font("Arial", 8);
        private readonly Font titleFont = new Font("Arial", 10, FontStyle.Bold);

        // UI state
        private float graphMinTime = 0;
        private float graphMaxTime = 100;
        private float graphMinValue = 0;
        private float graphMaxValue = 1;
        private bool ignoreDataGridViewChanges = false;

        // Mouse Drag Control
        private bool isDragging = false;
        private int dragRowIndex = -1;
        private int dragColumnIndex = -1;
        private Point dragStartPos;

        // Pan/Zoom Control
        private float zoomLevel = 1.0f;
        private PointF panCenter = new PointF(0.5f, 0.5f); // 0.5 = centered
        private Point panDragStart = Point.Empty;
        private bool isPanning = false;

        /// <summary>Initializes the form and sets up all controls.</summary>
        public KSPCurveBuilder()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // or FixedDialog
            this.MaximizeBox = false;
            this.Icon = global::KSPCurveBuilder.Properties.Resources.CurveIcon;
            this.Shown += new EventHandler(Form1_Shown);
            SetupDataGridView();
            SetupEvents();
            UpdateAll();
            presetDropdown.SelectedIndexChanged += PresetDropdown_SelectedIndexChanged;
        }

        private void SetupDataGridView()
        {
            // Configure DataGridView columns
            dataPointEditor.AutoGenerateColumns = false;
            dataPointEditor.AllowUserToAddRows = false;
            dataPointEditor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataPointEditor.RowHeadersVisible = false;

            // Add columns
            dataPointEditor.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Time",
                DataPropertyName = "Time",
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataPointEditor.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                DataPropertyName = "Value",
                Width = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataPointEditor.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "InTan",
                DataPropertyName = "InTangent",
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataPointEditor.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "OutTan",
                DataPropertyName = "OutTangent",
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Remove button column
            var removeColumn = new DataGridViewButtonColumn()
            {
                HeaderText = "Remove",
                Text = "X",
                UseColumnTextForButtonValue = true,
                Width = 70,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dataPointEditor.Columns.Add(removeColumn);

            // Create binding list
            pointsBindingList = new BindingList<FloatString4>(points);
            dataPointEditor.DataSource = pointsBindingList;
        }
        private void DataPointEditor_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var point = points[e.RowIndex];

            if (e.ColumnIndex == 0) e.Value = FloatString4.FormatNumber(point.Time, Formatting.TIME_DECIMAL_PLACES);
            if (e.ColumnIndex == 1) e.Value = FloatString4.FormatNumber(point.Value, Formatting.VALUE_DECIMAL_PLACES);
            if (e.ColumnIndex == 2) e.Value = FloatString4.FormatNumber(point.InTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);
            if (e.ColumnIndex == 3) e.Value = FloatString4.FormatNumber(point.OutTangent, Formatting.TANGENT_SIGNIFICANT_FIGURES);

            e.FormattingApplied = true;
        }
        private void DataPointEditor_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false; // Suppress dialog
            e.Cancel = false;         // Allow navigation
        }

        /// <summary>Attaches event handlers to all UI controls.</summary>
        private void SetupEvents()
        {
            // Button events
            buttonNewCurve.Click += ButtonNewCurve_Click;
            buttonSmooth.Click += ButtonSmooth_Click;
            buttonCopy.Click += ButtonCopy_Click;
            buttonPaste.Click += ButtonPaste_Click;
            buttonAddNode.Click += ButtonAddNode_Click;
            checkBoxSort.CheckedChanged += CheckBoxSort_CheckedChanged;

            // DataGridView events
            dataPointEditor.CellValueChanged += DataPointEditor_CellValueChanged;
            dataPointEditor.CellClick += DataPointEditor_CellClick;
            dataPointEditor.CellValidating += DataPointEditor_CellValidating;
            dataPointEditor.CellFormatting += DataPointEditor_CellFormatting;
            dataPointEditor.DataError += DataPointEditor_DataError;

            // PictureBox events
            curveView.Paint += CurveView_Paint;
            curveView.Resize += CurveView_Resize;

            // TextBox events
            curveText.TextChanged += CurveText_TextChanged;

            // CRITICAL: Configure DataGridView to allow drag
            dataPointEditor.MultiSelect = false;
            dataPointEditor.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Add mouse event handlers
            dataPointEditor.MouseDown += DataPointEditor_MouseDown;
            dataPointEditor.MouseMove += DataPointEditor_MouseMove;
            dataPointEditor.MouseUp += DataPointEditor_MouseUp;
            dataPointEditor.CellMouseEnter += DataPointEditor_CellMouseEnter;
            dataPointEditor.CellMouseLeave += DataPointEditor_CellMouseLeave;

            // Add Pan/Zoom mouse event handlers
            curveView.MouseWheel += CurveView_MouseWheel;
            curveView.MouseDown += CurveView_MouseDown;
            curveView.MouseMove += CurveView_MouseMove;
            curveView.MouseUp += CurveView_MouseUp;
            curveView.TabStop = true;
            //curveView.MouseEnter += (s, e) => curveView.Focus(); // Required for wheel events

            // Preset element events
            presetDropdown.SelectedIndexChanged += PresetDropdown_SelectedIndexChanged;
            buttonSavePreset.Click += buttonSavePreset_Click;
            buttonDeletePreset.Click += ButtonDeletePreset_Click;
        }

        #region Button Event Handlers

        private void ButtonNewCurve_Click(object sender, EventArgs e)
        {
            points.Clear();
            pointsBindingList.ResetBindings();
            UpdateAll();
        }

        private void ButtonSmooth_Click(object sender, EventArgs e)
        {
            // Rebuild curve from current points
            var curve = CreateCurveFromPoints();
            curve.SmoothTangents();
            UpdatePointsFromCurve(curve);

            // Refresh UI
            pointsBindingList.ResetBindings();
            UpdateAll();
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            if (curveText.Text.Length > 0)
            {
                Clipboard.SetText(curveText.Text);
                MessageBox.Show("Curve data copied to clipboard!", "Copy",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    MessageBox.Show("Clipboard is empty.", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string[] lines = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    MessageBox.Show("No valid lines found in clipboard.", "Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var validPoints = new List<FloatString4>();
                var errors = new List<string>();

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("key", StringComparison.OrdinalIgnoreCase))
                    {
                        var parseResult = FloatString4.TryParseKeyString(trimmedLine);
                        if (parseResult.Success)
                        {
                            validPoints.Add(parseResult.Point);
                        }
                        else
                        {
                            errors.Add($"Line '{trimmedLine}': {parseResult.ErrorMessage}");
                        }
                    }
                }

                if (validPoints.Count == 0)
                {
                    string errorMsg = $"No valid key lines found.{Environment.NewLine}{Environment.NewLine}Errors:{Environment.NewLine}";
                    errorMsg += string.Join(Environment.NewLine, errors.Take(5));
                    MessageBox.Show(errorMsg, "Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (errors.Count > 0)
                {
                    string warning = $"Parsed {validPoints.Count} valid points, but {errors.Count} lines had errors.";
                    warning += $"{Environment.NewLine}{Environment.NewLine}Continue with valid points only?";

                    if (MessageBox.Show(warning, "Paste Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                }

                // Check for duplicate times
                var duplicateGroups = validPoints.GroupBy(p => p.Time).Where(g => g.Count() > 1).ToList();
                if (duplicateGroups.Any())
                {
                    var dupMsg = $"Duplicate time values found:{Environment.NewLine}";
                    dupMsg += string.Join(Environment.NewLine, duplicateGroups.Select(g => $"Time {g.Key} appears {g.Count()} times"));
                    dupMsg += $"{Environment.NewLine}{Environment.NewLine}Continue anyway?";

                    if (MessageBox.Show(dupMsg, "Duplicate Times Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                }

                points.Clear();
                points.AddRange(validPoints);

                if (checkBoxSort.Checked)
                    points.Sort();

                pointsBindingList.ResetBindings();
                ResetZoom();
                UpdateAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to paste: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonAddNode_Click(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                // Add a new point after the last one
                var lastPoint = points.Last();
                var newPoint = new FloatString4(
                    lastPoint.Time + 10,  // Default: +10 time
                    lastPoint.Value,      // Same value
                    0, 0);                // Zero tangents

                points.Add(newPoint);
            }
            else
            {
                points.Add(new FloatString4(0, 0, 0, 0));
            }

            if (checkBoxSort.Checked)
                points.Sort();

            pointsBindingList.ResetBindings();
            UpdateAll();
        }
        /// <summary>
        /// Saves the current curve as a user preset. The preset name is taken from the
        /// preset name textbox and sanitized for filesystem safety. Built-in presets cannot
        /// be overwritten. After saving, the dropdown is refreshed and the new preset is
        /// automatically selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void buttonSavePreset_Click(object sender, EventArgs e)
        {
            string name = presetNameTextbox.Text.Trim();

            // Sanitize filename (remove invalid chars)
            name = string.Concat(name.Split(Path.GetInvalidFileNameChars()));

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Enter a preset name.");
                return;
            }

            // Check if preset exists (either built-in or user-saved)
            bool exists = false;

            // Check built-in presets
            if (BuiltInPresets.GetAll().Any(p => p.Name == name))
            {
                MessageBox.Show($"Cannot overwrite built-in preset '{name}'. Please choose a different name.");
                return;
            }

            // Check user presets
            exists = PresetManager.GetAvailablePresets().Contains(name);

            if (exists)
            {
                var result = MessageBox.Show($"Overwrite '{name}'?", "Confirm Overwrite",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No) return;
            }

            // Save it
            var preset = Preset.FromPoints(name, "User-created preset", points);
            PresetManager.SavePreset(preset);

            // Refresh dropdown
            LoadPresetsIntoDropdown();

            // Select the saved preset
            foreach (Preset item in presetDropdown.Items)
            {
                if (item.Name == name)
                {
                    presetDropdown.SelectedItem = item;
                    break;
                }
            }
        }
        /// <summary>
        /// Handles selection changes in the preset dropdown. When a preset is selected,
        /// this method updates the preset name textbox, loads the corresponding curve points
        /// into the editor, and refreshes the UI. Built-in and user presets are loaded
        /// differently but both result in <see cref="Preset"/> objects being processed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void PresetDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetDropdown.SelectedItem == null) return;

            // Get the selected Preset object
            var selectedPreset = presetDropdown.SelectedItem as Preset;
            if (selectedPreset == null) return;

            // Update the preset name textbox with the Preset's Name property
            presetNameTextbox.Text = selectedPreset.Name;

            // Load into editor
            points.Clear();
            points.AddRange(selectedPreset.Points);
            pointsBindingList.ResetBindings();
            UpdateAll();
        }
        /// <summary>
        /// Deletes a user preset after confirmation. Built-in presets are protected from deletion.
        /// After successful deletion, the dropdown is refreshed and the default preset (index 0)
        /// is automatically selected to maintain a valid UI state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void ButtonDeletePreset_Click(object sender, EventArgs e)
        {
            if (presetDropdown.SelectedItem == null) return;

            // Get the selected Preset object
            var selectedPreset = presetDropdown.SelectedItem as Preset;
            if (selectedPreset == null) return;

            // Don't delete built-in presets
            if (BuiltInPresets.GetAll().Any(p => p.Name == selectedPreset.Name))
            {
                MessageBox.Show("Cannot delete built-in presets.");
                return;
            }

            if (MessageBox.Show($"Delete '{selectedPreset.Name}'?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PresetManager.DeletePreset(selectedPreset.Name);
                LoadPresetsIntoDropdown();

                // Select the default preset (index 0) after deletion
                if (presetDropdown.Items.Count > 0)
                {
                    presetDropdown.SelectedIndex = 0;
                }
                else
                {
                    // If no presets left, clear the textbox
                    presetNameTextbox.Text = "";
                }
            }
        }
        /// <summary>
        /// Populates the preset dropdown with both built-in and user-defined presets.
        /// Built-in presets are loaded from <see cref="BuiltInPresets"/>, while user presets
        /// are loaded from disk via <see cref="PresetManager"/>. Each item in the dropdown
        /// is stored as a <see cref="Preset"/> object with DisplayMember set to show the Name.
        /// </summary>
        private void LoadPresetsIntoDropdown()
        {
            if (presetDropdown == null) return;

            presetDropdown.Items.Clear();

            // Load built-in presets as Preset objects
            var builtIns = BuiltInPresets.GetAll();
            foreach (var preset in builtIns)
            {
                presetDropdown.Items.Add(preset);
            }

            // Load user presets as Preset objects
            var userPresetNames = PresetManager.GetAvailablePresets();
            foreach (var name in userPresetNames)
            {
                var userPreset = PresetManager.LoadPreset(name);
                if (userPreset != null)
                {
                    presetDropdown.Items.Add(userPreset);
                }
            }

            // Set DisplayMember to show the name
            presetDropdown.DisplayMember = "Name";
        }

        private void ResetZoom()
        {
            zoomLevel = 1.0f;
            panCenter = new PointF(0.5f, 0.5f);
            UpdateGraphBounds();
            curveView.Invalidate();
        }
        private void buttonResetZoom_Click(object sender, EventArgs e)
        {
            ResetZoom();
        }

        private void CheckBoxSort_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSort.Checked)
            {
                points.Sort();
                pointsBindingList.ResetBindings();
                UpdateAll();
            }
        }

        #endregion

        #region DataGridView Event Handlers

        private void DataPointEditor_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (ignoreDataGridViewChanges || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var point = points[e.RowIndex];
            object cellValue = dataPointEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (cellValue == null) return;

            if (!float.TryParse(cellValue.ToString(), out float parsedValue))
            {
                dataPointEditor.Rows[e.RowIndex].ErrorText = "Must be a valid number";
                return;
            }

            dataPointEditor.Rows[e.RowIndex].ErrorText = "";

            bool timeChanged = false;
            switch ((DataGridColumn)e.ColumnIndex)
            {
                case DataGridColumn.Time:
                    point.Time = parsedValue;
                    timeChanged = true;
                    break;
                case DataGridColumn.Value:
                    point.Value = parsedValue;
                    break;
                case DataGridColumn.InTangent:
                    point.InTangent = parsedValue;
                    break;
                case DataGridColumn.OutTangent:
                    point.OutTangent = parsedValue;
                    break;
            }

            // Only sort if Time column changed
            if (checkBoxSort.Checked && timeChanged)
                points.Sort();

            pointsBindingList.ResetBindings();
            UpdateAll();
        }

        private void DataPointEditor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == (int)DataGridColumn.RemoveButton && e.RowIndex >= 0)
            {
                if (MessageBox.Show("Delete this point?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    points.RemoveAt(e.RowIndex);
                    pointsBindingList.ResetBindings();
                    UpdateAll();
                }
            }
        }

        private void DataPointEditor_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex > 3) return;

            if (!float.TryParse(e.FormattedValue.ToString(), out _))
            {
                // Revert to original value
                var point = points[e.RowIndex];
                float originalValue;
                switch (e.ColumnIndex)
                {
                    case 0: originalValue = point.Time; break;
                    case 1: originalValue = point.Value; break;
                    case 2: originalValue = point.InTangent; break;
                    case 3: originalValue = point.OutTangent; break;
                    default: originalValue = 0f; break;
                }

                dataPointEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                dataPointEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            }
            else
            {
                dataPointEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            }

            e.Cancel = false;
        }

        #endregion

        #region PictureBox Event Handlers

        private void CurveView_Paint(object sender, PaintEventArgs e)
        {
            if (points.Count == 0) return;
            var curve = CreateCurveFromPoints();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Black);

            var renderer = new CurveRenderer(e.Graphics, points, curve,
                graphMinTime, graphMaxTime, graphMinValue, graphMaxValue,
                curveView.Width, curveView.Height, zoomLevel);
            renderer.Render();
        }

        private void CurveView_Resize(object sender, EventArgs e)
        {
            curveView.Invalidate(); // Redraw on resize
        }

        #endregion

        #region TextBox Event Handlers

        private void CurveText_TextChanged(object sender, EventArgs e)
        {
            // Optional: Parse text box changes in real-time
            // Could add a "Apply Text" button instead
        }

        #endregion

        #region Core Update Methods
        /// <summary>Rebuilds curve, updates graph bounds, refreshes textbox and render.</summary>
        private void UpdateAll()
        {
            var curve = CreateCurveFromPoints(); // Use local curve
            UpdateGraphBounds();
            UpdateTextBox();
            curveView.Invalidate();
        }

        private FloatCurveStandalone CreateCurveFromPoints()
        {
            var curve = new FloatCurveStandalone();
            foreach (var point in points) curve.Add(point.Time, point.Value, point.InTangent, point.OutTangent);
            return curve;
        }

        private void UpdatePointsFromCurve(FloatCurveStandalone curve)
        {
            points.Clear();
            if (curve != null && curve.Curve.keys.Length > 0)
            {
                foreach (var key in curve.Curve.keys)
                {
                    points.Add(new FloatString4(key));
                }
            }
            if (checkBoxSort.Checked) points.Sort();
        }

        private void UpdateGraphBounds()
        {
            if (points.Count == 0)
            {
                graphMinTime = -50 / zoomLevel;
                graphMaxTime = 150 / zoomLevel;
                graphMinValue = -0.5f / zoomLevel;
                graphMaxValue = 1.5f / zoomLevel;

                float offsetX = (panCenter.X - 0.5f) * (graphMaxTime - graphMinTime);
                float offsetY = (panCenter.Y - 0.5f) * (graphMaxValue - graphMinValue);

                graphMinTime += offsetX;
                graphMaxTime += offsetX;
                graphMinValue += offsetY;
                graphMaxValue += offsetY;
                return;
            }

            float minTime = points.Min(p => p.Time);
            float maxTime = points.Max(p => p.Time);
            float minValue = points.Min(p => p.Value);
            float maxValue = points.Max(p => p.Value);

            float timePadding = (maxTime - minTime) * 0.1f;
            float valuePadding = (maxValue - minValue) * 0.1f;

            float dataWidth = (maxTime - minTime + 2 * timePadding);
            float dataHeight = (maxValue - minValue + 2 * valuePadding);

            dataWidth /= zoomLevel;
            dataHeight /= zoomLevel;

            float centerX = minTime + (maxTime - minTime) * panCenter.X;
            float centerY = minValue + (maxValue - minValue) * panCenter.Y;

            graphMinTime = centerX - dataWidth / 2;
            graphMaxTime = centerX + dataWidth / 2;
            graphMinValue = centerY - dataHeight / 2;
            graphMaxValue = centerY + dataHeight / 2;
        }

        private void UpdateTextBox()
        {
            ignoreDataGridViewChanges = true;
            try
            {
                curveText.Text = string.Join(Environment.NewLine, points.Select(p => p.ToKeyString("key")));
            }
            finally
            {
                ignoreDataGridViewChanges = false;
            }
        }

        #endregion

        #region Utility Methods

        //Mouse interaction for dragging on cells to change values
        #region MouseDragControls
        private void DataPointEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (dataPointEditor.IsCurrentCellInEditMode)
            {
                dataPointEditor.EndEdit();
                return;
            }

            // Don't start drag if cell has validation errors
            if (dataPointEditor.IsCurrentCellInEditMode) return;

            var hit = dataPointEditor.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.Cell &&
                hit.ColumnIndex >= 0 && hit.ColumnIndex <= 3 &&
                hit.RowIndex >= 0 && hit.RowIndex < points.Count)
            {
                isDragging = true;
                dragRowIndex = hit.RowIndex;
                dragColumnIndex = hit.ColumnIndex;
                dragStartPos = e.Location;

                dataPointEditor.ClearSelection();
                // DO NOT SET CurrentCell = null - causes InvalidOperationException
                dataPointEditor.Capture = true;
                dataPointEditor.Cursor = Cursors.HSplit;
            }
        }

        private void DataPointEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || dragRowIndex < 0) return;

            // DRAG CURVE FUNCTION
            Func<float, float> getDragRate = (v) =>
                v < 0.001f ? Constants.DRAG_MIN_RATE : v / (v + Constants.DRAG_REFERENCE) * Constants.MAX_RATE_MULTIPLIER;

            // Calculate change from LIVE value (not corrupted baseline)
            var point = points[dragRowIndex];
            float currentValue;
            switch (dragColumnIndex)
            {
                case 0: currentValue = point.Time; break;
                case 1: currentValue = point.Value; break;
                case 2: currentValue = point.InTangent; break;
                case 3: currentValue = point.OutTangent; break;
                default: currentValue = 0f; break;
            }

            float mouseDelta = dragStartPos.Y - e.Y;
            float columnMult = (dragColumnIndex == (int)DataGridColumn.InTangent ||
                                dragColumnIndex == (int)DataGridColumn.OutTangent) ?
                                Constants.TANGENT_MULTIPLIER : 1.0f;
            float rawChange = mouseDelta * Constants.DRAG_SENSITIVITY * columnMult;
            float currentRate = getDragRate(Math.Abs(currentValue));
            float valueChange = rawChange * currentRate;
            float newValue = currentValue + valueChange;
            newValue = ClampDragValue(newValue, dragColumnIndex);

            dragStartPos = e.Location;

            // Update point
            switch (dragColumnIndex)
            {
                case (int)DataGridColumn.Time:
                    point.Time = newValue;
                    if (checkBoxSort.Checked)
                    {
                        points.Sort();
                        pointsBindingList.ResetBindings();
                    }
                    break;
                case (int)DataGridColumn.Value:
                    point.Value = newValue;
                    break;
                case (int)DataGridColumn.InTangent:
                    point.InTangent = newValue;
                    break;
                case (int)DataGridColumn.OutTangent:
                    point.OutTangent = newValue;
                    break;
            }

            dataPointEditor.InvalidateCell(dragColumnIndex, dragRowIndex);

            // LIGHTWEIGHT update: only what drag needs
            UpdateGraphBounds();
            curveView.Invalidate();
        }



        private void DataPointEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                dragRowIndex = -1;
                dataPointEditor.Capture = false;
                dataPointEditor.Cursor = Cursors.Default;

                // Sync textbox with final state
                UpdateTextBox();
                curveView.Invalidate(); // Redraw one final time
            }
        }

        private void DataPointEditor_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.ColumnIndex <= 3 && e.RowIndex >= 0 && !isDragging)
            {
                dataPointEditor.Cursor = Cursors.HSplit; // Show drag cursor on hover
            }
        }

        private void DataPointEditor_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (!isDragging)
            {
                dataPointEditor.Cursor = Cursors.Default;
            }
        }

        private void DataPointEditor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Reset drag state after editing
            isDragging = false;
            dragRowIndex = -1;
        }

        private float ClampDragValue(float value, int columnIndex)
        {
            switch (columnIndex)
            {
                case 0: // Time
                    return Math.Max(0f, Math.Min(value, 1e6f));
                case 1: // Value
                    return Math.Max(-1e6f, Math.Min(value, 1e6f));
                case 2: // InTangent
                case 3: // OutTangent
                    return Math.Max(-1e4f, Math.Min(value, 1e4f));
                default:
                    return value;
            }
        }
        #endregion

        // Zoom/pan controls for graph
        #region ZoomPanControls

        private void CurveView_MouseWheel(object sender, MouseEventArgs e)
        {
            curveView.Focus();
            float oldZoom = zoomLevel;
            float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f;
            zoomLevel = Math.Max(0.1f, Math.Min(10.0f, zoomLevel * zoomFactor));

            if (Math.Abs(zoomLevel - oldZoom) > 0.001f)
            {
                float cursorTime = graphMinTime + (e.X / (float)curveView.Width) * (graphMaxTime - graphMinTime);
                float cursorValue = graphMaxValue - (e.Y / (float)curveView.Height) * (graphMaxValue - graphMinValue);

                float timeRatio = (cursorTime - graphMinTime) / (graphMaxTime - graphMinTime);
                float valueRatio = (graphMaxValue - cursorValue) / (graphMaxValue - graphMinValue);

                panCenter.X = panCenter.X + (timeRatio - 0.5f) * (1 - oldZoom / zoomLevel);
                panCenter.Y = panCenter.Y + (valueRatio - 0.5f) * (1 - oldZoom / zoomLevel);

                panCenter.X = Math.Max(0.01f, Math.Min(0.99f, panCenter.X));
                panCenter.Y = Math.Max(0.01f, Math.Min(0.99f, panCenter.Y));

                UpdateGraphBounds();
                curveView.Invalidate();
            }
        }

        private void CurveView_MouseDown(object sender, MouseEventArgs e)
        {
            curveView.Focus();
            if (e.Button == MouseButtons.Left)
            {
                isPanning = true;
                panDragStart = e.Location;
                curveView.Cursor = Cursors.SizeAll;
            }
        }

        private void CurveView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                float timePerPixel = (graphMaxTime - graphMinTime) / curveView.Width;
                float valuePerPixel = (graphMaxValue - graphMinValue) / curveView.Height;

                float deltaX = (e.X - panDragStart.X) * timePerPixel / zoomLevel;
                float deltaY = (e.Y - panDragStart.Y) * valuePerPixel / zoomLevel;

                panCenter.X -= deltaX / (graphMaxTime - graphMinTime);
                panCenter.Y += deltaY / (graphMaxValue - graphMinValue);

                panDragStart = e.Location;
                UpdateGraphBounds();
                curveView.Invalidate();
            }
        }


        private void CurveView_MouseUp(object sender, MouseEventArgs e)
        {
            isPanning = false;
            curveView.Cursor = Cursors.Cross;
        }
        #endregion
        #endregion

        /// <summary>
        /// Initializes the form after it's displayed. Loads presets into the dropdown,
        /// sets up the default curve, updates the preset name textbox, and ensures the
        /// default preset is selected in the dropdown. This is the main initialization
        /// point after the UI is ready.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            // Force dropdown population FIRST
            LoadPresetsIntoDropdown();

            // Initialize from Default preset (not hardcoded)
            var defaultPreset = BuiltInPresets.Default();
            points.Clear();
            points.AddRange(defaultPreset.Points);
            pointsBindingList.ResetBindings();

            // Update the textbox with the default preset name
            presetNameTextbox.Text = defaultPreset.Name;

            // Try to select the Default preset in dropdown
            bool foundDefault = false;
            foreach (Preset preset in presetDropdown.Items)
            {
                if (preset.Name == "Default")
                {
                    presetDropdown.SelectedItem = preset;
                    foundDefault = true;
                    break;
                }
            }

            // If not found, select first item
            if (!foundDefault && presetDropdown.Items.Count > 0)
            {
                presetDropdown.SelectedItem = presetDropdown.Items[0];
            }

            // UI update LAST
            UpdateAll();
        }
    }
}