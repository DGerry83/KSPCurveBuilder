/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/).
 * 
 * This file is part of Curve Editor, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html or the LICENSE file for full terms.
 */

namespace KSPCurveBuilder
{
    partial class KSPCurveBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                titleFont?.Dispose();
                curvePen?.Dispose();
                gridPen?.Dispose();
                pointPen?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            curveView = new System.Windows.Forms.PictureBox();
            curveText = new System.Windows.Forms.TextBox();
            buttonNewCurve = new System.Windows.Forms.Button();
            buttonSmooth = new System.Windows.Forms.Button();
            buttonCopy = new System.Windows.Forms.Button();
            buttonPaste = new System.Windows.Forms.Button();
            buttonAddNode = new System.Windows.Forms.Button();
            checkBoxSort = new System.Windows.Forms.CheckBox();
            dataPointEditor = new System.Windows.Forms.DataGridView();
            buttonResetZoom = new System.Windows.Forms.Button();
            presetDropdown = new System.Windows.Forms.ComboBox();
            presetNameTextbox = new System.Windows.Forms.TextBox();
            buttonSavePreset = new System.Windows.Forms.Button();
            buttonDeletePreset = new System.Windows.Forms.Button();
            buttonUndo = new System.Windows.Forms.Button();
            buttonRedo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)curveView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataPointEditor).BeginInit();
            SuspendLayout();
            // 
            // curveView
            // 
            curveView.BackColor = System.Drawing.Color.Black;
            curveView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            curveView.Location = new System.Drawing.Point(20, 22);
            curveView.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            curveView.Name = "curveView";
            curveView.Size = new System.Drawing.Size(730, 574);
            curveView.TabIndex = 0;
            curveView.TabStop = false;
            // 
            // curveText
            // 
            curveText.Location = new System.Drawing.Point(599, 610);
            curveText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            curveText.Multiline = true;
            curveText.Name = "curveText";
            curveText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            curveText.Size = new System.Drawing.Size(293, 196);
            curveText.TabIndex = 1;
            // 
            // buttonNewCurve
            // 
            buttonNewCurve.Location = new System.Drawing.Point(759, 22);
            buttonNewCurve.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonNewCurve.Name = "buttonNewCurve";
            buttonNewCurve.Size = new System.Drawing.Size(133, 58);
            buttonNewCurve.TabIndex = 2;
            buttonNewCurve.Text = "New Curve";
            // 
            // buttonSmooth
            // 
            buttonSmooth.Location = new System.Drawing.Point(599, 890);
            buttonSmooth.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonSmooth.Name = "buttonSmooth";
            buttonSmooth.Size = new System.Drawing.Size(133, 58);
            buttonSmooth.TabIndex = 3;
            buttonSmooth.Text = "Smooth";
            // 
            // buttonCopy
            // 
            buttonCopy.Location = new System.Drawing.Point(759, 820);
            buttonCopy.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new System.Drawing.Size(133, 58);
            buttonCopy.TabIndex = 4;
            buttonCopy.Text = "Copy";
            // 
            // buttonPaste
            // 
            buttonPaste.Location = new System.Drawing.Point(759, 92);
            buttonPaste.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonPaste.Name = "buttonPaste";
            buttonPaste.Size = new System.Drawing.Size(133, 58);
            buttonPaste.TabIndex = 5;
            buttonPaste.Text = "Paste";
            // 
            // buttonAddNode
            // 
            buttonAddNode.Location = new System.Drawing.Point(599, 820);
            buttonAddNode.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonAddNode.Name = "buttonAddNode";
            buttonAddNode.Size = new System.Drawing.Size(133, 58);
            buttonAddNode.TabIndex = 6;
            buttonAddNode.Text = "Add Node";
            // 
            // checkBoxSort
            // 
            checkBoxSort.AutoSize = true;
            checkBoxSort.Location = new System.Drawing.Point(759, 568);
            checkBoxSort.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            checkBoxSort.Name = "checkBoxSort";
            checkBoxSort.Size = new System.Drawing.Size(71, 29);
            checkBoxSort.TabIndex = 7;
            checkBoxSort.Text = "Sort";
            // 
            // dataPointEditor
            // 
            dataPointEditor.ColumnHeadersHeight = 34;
            dataPointEditor.Location = new System.Drawing.Point(20, 610);
            dataPointEditor.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            dataPointEditor.MultiSelect = false;
            dataPointEditor.Name = "dataPointEditor";
            dataPointEditor.RowHeadersWidth = 62;
            dataPointEditor.Size = new System.Drawing.Size(570, 358);
            dataPointEditor.TabIndex = 8;
            // 
            // buttonResetZoom
            // 
            buttonResetZoom.Location = new System.Drawing.Point(759, 498);
            buttonResetZoom.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonResetZoom.Name = "buttonResetZoom";
            buttonResetZoom.Size = new System.Drawing.Size(133, 58);
            buttonResetZoom.TabIndex = 9;
            buttonResetZoom.Text = "Reset Zoom";
            buttonResetZoom.Click += buttonResetZoom_Click;
            // 
            // presetDropdown
            // 
            presetDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            presetDropdown.FormattingEnabled = true;
            presetDropdown.Location = new System.Drawing.Point(758, 310);
            presetDropdown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            presetDropdown.Name = "presetDropdown";
            presetDropdown.Size = new System.Drawing.Size(134, 33);
            presetDropdown.TabIndex = 10;
            // 
            // presetNameTextbox
            // 
            presetNameTextbox.Location = new System.Drawing.Point(758, 270);
            presetNameTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            presetNameTextbox.Name = "presetNameTextbox";
            presetNameTextbox.Size = new System.Drawing.Size(111, 31);
            presetNameTextbox.TabIndex = 11;
            // 
            // buttonSavePreset
            // 
            buttonSavePreset.Location = new System.Drawing.Point(758, 221);
            buttonSavePreset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonSavePreset.Name = "buttonSavePreset";
            buttonSavePreset.Size = new System.Drawing.Size(88, 41);
            buttonSavePreset.TabIndex = 12;
            buttonSavePreset.Text = "Save";
            buttonSavePreset.UseVisualStyleBackColor = true;
            // 
            // buttonDeletePreset
            // 
            buttonDeletePreset.Location = new System.Drawing.Point(758, 352);
            buttonDeletePreset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonDeletePreset.Name = "buttonDeletePreset";
            buttonDeletePreset.Size = new System.Drawing.Size(88, 41);
            buttonDeletePreset.TabIndex = 13;
            buttonDeletePreset.Text = "Delete";
            buttonDeletePreset.UseVisualStyleBackColor = true;
            // 
            // buttonUndo
            // 
            buttonUndo.Location = new System.Drawing.Point(740, 914);
            buttonUndo.Name = "buttonUndo";
            buttonUndo.Size = new System.Drawing.Size(73, 34);
            buttonUndo.TabIndex = 14;
            buttonUndo.Text = "Undo";
            buttonUndo.UseVisualStyleBackColor = true;
            // 
            // buttonRedo
            // 
            buttonRedo.Location = new System.Drawing.Point(819, 914);
            buttonRedo.Name = "buttonRedo";
            buttonRedo.Size = new System.Drawing.Size(73, 34);
            buttonRedo.TabIndex = 15;
            buttonRedo.Text = "Redo";
            buttonRedo.UseVisualStyleBackColor = true;
            // 
            // KSPCurveBuilder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(912, 985);
            Controls.Add(buttonRedo);
            Controls.Add(buttonUndo);
            Controls.Add(buttonDeletePreset);
            Controls.Add(buttonSavePreset);
            Controls.Add(presetNameTextbox);
            Controls.Add(presetDropdown);
            Controls.Add(buttonResetZoom);
            Controls.Add(curveView);
            Controls.Add(curveText);
            Controls.Add(buttonNewCurve);
            Controls.Add(buttonSmooth);
            Controls.Add(buttonCopy);
            Controls.Add(buttonPaste);
            Controls.Add(buttonAddNode);
            Controls.Add(checkBoxSort);
            Controls.Add(dataPointEditor);
            Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            Name = "KSPCurveBuilder";
            Text = "KSP Curve Builder";
            ((System.ComponentModel.ISupportInitialize)curveView).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataPointEditor).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox curveView;
        private System.Windows.Forms.TextBox curveText;
        private System.Windows.Forms.Button buttonNewCurve;
        private System.Windows.Forms.Button buttonSmooth;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonAddNode;
        private System.Windows.Forms.CheckBox checkBoxSort;
        private System.Windows.Forms.DataGridView dataPointEditor;
        private System.Windows.Forms.Button buttonResetZoom;
        private System.Windows.Forms.ComboBox presetDropdown;
        private System.Windows.Forms.TextBox presetNameTextbox;
        private System.Windows.Forms.Button buttonSavePreset;
        private System.Windows.Forms.Button buttonDeletePreset;
        private System.Windows.Forms.Button buttonUndo;
        private System.Windows.Forms.Button buttonRedo;
    }
}

