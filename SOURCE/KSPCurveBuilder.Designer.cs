/* 
 * KSPCurveBuilder - A standalone float curve editing tool.
 * 
 * This file is part of a project based on AmazingCurveEditor (Copyright (C) sarbian).
 * Logic from that original project is used here and throughout.
 * 
 * Original work copyright © 2015 Sarbian (https://github.com/sarbian ).
 * Modifications, restructuring, and new code copyright © 2026 DGerry83(https://github.com/DGerry83/ ).
 * 
 * This file is part of KSPCurveBuilder, free software under the GPLv2 license. 
 * See https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html  or the LICENSE file for full terms.
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
            curveView.Location = new System.Drawing.Point(13, 15);
            curveView.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            curveView.Name = "curveView";
            curveView.Size = new System.Drawing.Size(746, 574);
            curveView.TabIndex = 0;
            curveView.TabStop = false;
            // 
            // curveText
            // 
            curveText.Location = new System.Drawing.Point(591, 645);
            curveText.Margin = new System.Windows.Forms.Padding(4);
            curveText.Multiline = true;
            curveText.Name = "curveText";
            curveText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            curveText.Size = new System.Drawing.Size(308, 291);
            curveText.TabIndex = 1;
            // 
            // buttonNewCurve
            // 
            buttonNewCurve.Location = new System.Drawing.Point(794, 945);
            buttonNewCurve.Name = "buttonNewCurve";
            buttonNewCurve.Size = new System.Drawing.Size(105, 35);
            buttonNewCurve.TabIndex = 2;
            buttonNewCurve.Text = "New Curve";
            buttonNewCurve.UseVisualStyleBackColor = true;
            // 
            // buttonSmooth
            // 
            buttonSmooth.Location = new System.Drawing.Point(86, 945);
            buttonSmooth.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            buttonSmooth.Name = "buttonSmooth";
            buttonSmooth.Size = new System.Drawing.Size(90, 35);
            buttonSmooth.TabIndex = 3;
            buttonSmooth.Text = "Smooth";
            buttonSmooth.UseVisualStyleBackColor = true;
            // 
            // buttonCopy
            // 
            buttonCopy.Location = new System.Drawing.Point(591, 603);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new System.Drawing.Size(80, 35);
            buttonCopy.TabIndex = 4;
            buttonCopy.Text = "Copy";
            buttonCopy.UseVisualStyleBackColor = true;
            // 
            // buttonPaste
            // 
            buttonPaste.Location = new System.Drawing.Point(820, 603);
            buttonPaste.Name = "buttonPaste";
            buttonPaste.Size = new System.Drawing.Size(80, 35);
            buttonPaste.TabIndex = 5;
            buttonPaste.Text = "Paste";
            buttonPaste.UseVisualStyleBackColor = true;
            // 
            // buttonAddNode
            // 
            buttonAddNode.Location = new System.Drawing.Point(183, 945);
            buttonAddNode.Name = "buttonAddNode";
            buttonAddNode.Size = new System.Drawing.Size(110, 35);
            buttonAddNode.TabIndex = 6;
            buttonAddNode.Text = "Add Node";
            buttonAddNode.UseVisualStyleBackColor = true;
            // 
            // checkBoxSort
            // 
            checkBoxSort.AutoSize = true;
            checkBoxSort.Location = new System.Drawing.Point(13, 945);
            checkBoxSort.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            checkBoxSort.Name = "checkBoxSort";
            checkBoxSort.Size = new System.Drawing.Size(71, 29);
            checkBoxSort.TabIndex = 7;
            checkBoxSort.Text = "Sort";
            checkBoxSort.UseVisualStyleBackColor = true;
            checkBoxSort.Checked = true;
            //this.checkBoxSort.CheckedChanged += new System.EventHandler(this.OnCheckBoxSortChanged);

            // 
            // dataPointEditor
            // 
            dataPointEditor.ColumnHeadersHeight = 34;
            dataPointEditor.Location = new System.Drawing.Point(13, 603);
            dataPointEditor.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            dataPointEditor.MultiSelect = false;
            dataPointEditor.Name = "dataPointEditor";
            dataPointEditor.RowHeadersWidth = 62;
            dataPointEditor.Size = new System.Drawing.Size(570, 338);
            dataPointEditor.TabIndex = 8;
            // 
            // buttonResetZoom
            // 
            buttonResetZoom.Location = new System.Drawing.Point(766, 554);
            buttonResetZoom.Name = "buttonResetZoom";
            buttonResetZoom.Size = new System.Drawing.Size(129, 35);
            buttonResetZoom.TabIndex = 9;
            buttonResetZoom.Text = "Reset Zoom";
            buttonResetZoom.UseVisualStyleBackColor = true;
            // 
            // presetDropdown
            // 
            presetDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            presetDropdown.FormattingEnabled = true;
            presetDropdown.Location = new System.Drawing.Point(766, 93);
            presetDropdown.Name = "presetDropdown";
            presetDropdown.Size = new System.Drawing.Size(140, 33);
            presetDropdown.TabIndex = 10;
            //this.presetDropdown.SelectedIndexChanged += new System.EventHandler(this.OnPresetDropdownChanged);

            // 
            // presetNameTextbox
            // 
            presetNameTextbox.Location = new System.Drawing.Point(766, 14);
            presetNameTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            presetNameTextbox.Name = "presetNameTextbox";
            presetNameTextbox.Size = new System.Drawing.Size(140, 31);
            presetNameTextbox.TabIndex = 11;
            // 
            // buttonSavePreset
            // 
            buttonSavePreset.Location = new System.Drawing.Point(766, 52);
            buttonSavePreset.Name = "buttonSavePreset";
            buttonSavePreset.Size = new System.Drawing.Size(90, 35);
            buttonSavePreset.TabIndex = 12;
            buttonSavePreset.Text = "Save";
            buttonSavePreset.UseVisualStyleBackColor = true;
            // 
            // buttonDeletePreset
            // 
            buttonDeletePreset.Location = new System.Drawing.Point(766, 132);
            buttonDeletePreset.Name = "buttonDeletePreset";
            buttonDeletePreset.Size = new System.Drawing.Size(90, 35);
            buttonDeletePreset.TabIndex = 13;
            buttonDeletePreset.Text = "Delete";
            buttonDeletePreset.UseVisualStyleBackColor = true;
            // 
            // buttonUndo
            // 
            buttonUndo.Location = new System.Drawing.Point(431, 946);
            buttonUndo.Name = "buttonUndo";
            buttonUndo.Size = new System.Drawing.Size(73, 34);
            buttonUndo.TabIndex = 14;
            buttonUndo.Text = "Undo";
            buttonUndo.UseVisualStyleBackColor = true;
            // 
            // buttonRedo
            // 
            buttonRedo.Location = new System.Drawing.Point(508, 945);
            buttonRedo.Name = "buttonRedo";
            buttonRedo.Size = new System.Drawing.Size(75, 35);
            buttonRedo.TabIndex = 15;
            buttonRedo.Text = "Redo";
            buttonRedo.UseVisualStyleBackColor = true;
            // 
            // KSPCurveBuilder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(912, 986);
            Controls.Add(buttonRedo);
            Controls.Add(buttonUndo);
            Controls.Add(buttonDeletePreset);
            Controls.Add(buttonSavePreset);
            Controls.Add(presetNameTextbox);
            Controls.Add(presetDropdown);
            Controls.Add(buttonResetZoom);
            Controls.Add(dataPointEditor);
            Controls.Add(checkBoxSort);
            Controls.Add(buttonAddNode);
            Controls.Add(buttonPaste);
            Controls.Add(buttonCopy);
            Controls.Add(buttonSmooth);
            Controls.Add(buttonNewCurve);
            Controls.Add(curveText);
            Controls.Add(curveView);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            MaximizeBox = false;
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