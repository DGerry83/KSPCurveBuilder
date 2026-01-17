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
            this.curveView = new System.Windows.Forms.PictureBox();
            this.curveText = new System.Windows.Forms.TextBox();
            this.buttonNewCurve = new System.Windows.Forms.Button();
            this.buttonSmooth = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonAddNode = new System.Windows.Forms.Button();
            this.checkBoxSort = new System.Windows.Forms.CheckBox();
            this.dataPointEditor = new System.Windows.Forms.DataGridView();
            this.buttonResetZoom = new System.Windows.Forms.Button();
            this.presetDropdown = new System.Windows.Forms.ComboBox();
            this.presetNameTextbox = new System.Windows.Forms.TextBox();
            this.buttonSavePreset = new System.Windows.Forms.Button();
            this.buttonDeletePreset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.curveView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataPointEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // curveView
            // 
            this.curveView.BackColor = System.Drawing.Color.Black;
            this.curveView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.curveView.Location = new System.Drawing.Point(18, 18);
            this.curveView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.curveView.Name = "curveView";
            this.curveView.Size = new System.Drawing.Size(657, 460);
            this.curveView.TabIndex = 0;
            this.curveView.TabStop = false;
            // 
            // curveText
            // 
            this.curveText.Location = new System.Drawing.Point(539, 488);
            this.curveText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.curveText.Multiline = true;
            this.curveText.Name = "curveText";
            this.curveText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.curveText.Size = new System.Drawing.Size(264, 158);
            this.curveText.TabIndex = 1;
            // 
            // buttonNewCurve
            // 
            this.buttonNewCurve.Location = new System.Drawing.Point(683, 18);
            this.buttonNewCurve.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonNewCurve.Name = "buttonNewCurve";
            this.buttonNewCurve.Size = new System.Drawing.Size(120, 46);
            this.buttonNewCurve.TabIndex = 2;
            this.buttonNewCurve.Text = "New Curve";
            // 
            // buttonSmooth
            // 
            this.buttonSmooth.Location = new System.Drawing.Point(539, 712);
            this.buttonSmooth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSmooth.Name = "buttonSmooth";
            this.buttonSmooth.Size = new System.Drawing.Size(120, 46);
            this.buttonSmooth.TabIndex = 3;
            this.buttonSmooth.Text = "Smooth";
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(683, 656);
            this.buttonCopy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(120, 46);
            this.buttonCopy.TabIndex = 4;
            this.buttonCopy.Text = "Copy";
            // 
            // buttonPaste
            // 
            this.buttonPaste.Location = new System.Drawing.Point(683, 74);
            this.buttonPaste.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(120, 46);
            this.buttonPaste.TabIndex = 5;
            this.buttonPaste.Text = "Paste";
            // 
            // buttonAddNode
            // 
            this.buttonAddNode.Location = new System.Drawing.Point(539, 656);
            this.buttonAddNode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAddNode.Name = "buttonAddNode";
            this.buttonAddNode.Size = new System.Drawing.Size(120, 46);
            this.buttonAddNode.TabIndex = 6;
            this.buttonAddNode.Text = "Add Node";
            // 
            // checkBoxSort
            // 
            this.checkBoxSort.AutoSize = true;
            this.checkBoxSort.Location = new System.Drawing.Point(683, 454);
            this.checkBoxSort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxSort.Name = "checkBoxSort";
            this.checkBoxSort.Size = new System.Drawing.Size(65, 24);
            this.checkBoxSort.TabIndex = 7;
            this.checkBoxSort.Text = "Sort";
            // 
            // dataPointEditor
            // 
            this.dataPointEditor.ColumnHeadersHeight = 34;
            this.dataPointEditor.Location = new System.Drawing.Point(18, 488);
            this.dataPointEditor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataPointEditor.MultiSelect = false;
            this.dataPointEditor.Name = "dataPointEditor";
            this.dataPointEditor.RowHeadersWidth = 62;
            this.dataPointEditor.Size = new System.Drawing.Size(513, 286);
            this.dataPointEditor.TabIndex = 8;
            // 
            // buttonResetZoom
            // 
            this.buttonResetZoom.Location = new System.Drawing.Point(683, 398);
            this.buttonResetZoom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonResetZoom.Name = "buttonResetZoom";
            this.buttonResetZoom.Size = new System.Drawing.Size(120, 46);
            this.buttonResetZoom.TabIndex = 9;
            this.buttonResetZoom.Text = "Reset Zoom";
            this.buttonResetZoom.Click += new System.EventHandler(this.buttonResetZoom_Click);
            // 
            // presetDropdown
            // 
            this.presetDropdown.FormattingEnabled = true;
            this.presetDropdown.Location = new System.Drawing.Point(682, 248);
            this.presetDropdown.Name = "presetDropdown";
            this.presetDropdown.Size = new System.Drawing.Size(121, 28);
            this.presetDropdown.TabIndex = 10;
            // 
            // presetNameTextbox
            // 
            this.presetNameTextbox.Location = new System.Drawing.Point(682, 216);
            this.presetNameTextbox.Name = "presetNameTextbox";
            this.presetNameTextbox.Size = new System.Drawing.Size(100, 26);
            this.presetNameTextbox.TabIndex = 11;
            // 
            // buttonSavePreset
            // 
            this.buttonSavePreset.Location = new System.Drawing.Point(682, 177);
            this.buttonSavePreset.Name = "buttonSavePreset";
            this.buttonSavePreset.Size = new System.Drawing.Size(79, 33);
            this.buttonSavePreset.TabIndex = 12;
            this.buttonSavePreset.Text = "Save";
            this.buttonSavePreset.UseVisualStyleBackColor = true;
            //this.buttonSavePreset.Click += new System.EventHandler(this.buttonSavePreset_Click);
            // 
            // buttonDeletePreset
            // 
            this.buttonDeletePreset.Location = new System.Drawing.Point(682, 282);
            this.buttonDeletePreset.Name = "buttonDeletePreset";
            this.buttonDeletePreset.Size = new System.Drawing.Size(79, 33);
            this.buttonDeletePreset.TabIndex = 13;
            this.buttonDeletePreset.Text = "Delete";
            this.buttonDeletePreset.UseVisualStyleBackColor = true;
            // 
            // KSPCurveBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 788);
            this.Controls.Add(this.buttonDeletePreset);
            this.Controls.Add(this.buttonSavePreset);
            this.Controls.Add(this.presetNameTextbox);
            this.Controls.Add(this.presetDropdown);
            this.Controls.Add(this.buttonResetZoom);
            this.Controls.Add(this.curveView);
            this.Controls.Add(this.curveText);
            this.Controls.Add(this.buttonNewCurve);
            this.Controls.Add(this.buttonSmooth);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonPaste);
            this.Controls.Add(this.buttonAddNode);
            this.Controls.Add(this.checkBoxSort);
            this.Controls.Add(this.dataPointEditor);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "KSPCurveBuilder";
            this.Text = "KSP Curve Builder";
            ((System.ComponentModel.ISupportInitialize)(this.curveView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataPointEditor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

