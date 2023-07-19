// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Renderer
{
    public partial class RenderForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                _graphics.Dispose();
                _bmp.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderForm));
            renderBox = new PictureBox();
            btnRender = new Button();
            comboObjects = new ComboBox();
            lblObject = new Label();
            numLocationZ = new NumericUpDown();
            lblLocationZ = new Label();
            ((System.ComponentModel.ISupportInitialize)renderBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLocationZ).BeginInit();
            SuspendLayout();
            // 
            // renderBox
            // 
            renderBox.BackColor = Color.LightGray;
            renderBox.ErrorImage = null;
            renderBox.InitialImage = null;
            renderBox.Location = new Point(12, 12);
            renderBox.Name = "renderBox";
            renderBox.Size = new Size(512, 512);
            renderBox.TabIndex = 0;
            renderBox.TabStop = false;
            // 
            // btnRender
            // 
            btnRender.Location = new Point(797, 449);
            btnRender.Name = "btnRender";
            btnRender.Size = new Size(75, 75);
            btnRender.TabIndex = 1;
            btnRender.Text = "Render";
            btnRender.UseVisualStyleBackColor = true;
            btnRender.Click += BtnRender_Click;
            // 
            // comboObjects
            // 
            comboObjects.FormattingEnabled = true;
            comboObjects.Location = new Point(638, 41);
            comboObjects.Name = "comboObjects";
            comboObjects.Size = new Size(103, 23);
            comboObjects.TabIndex = 2;
            comboObjects.SelectedIndexChanged += ComboPlanets_SelectedIndexChanged;
            // 
            // lblObject
            // 
            lblObject.AutoSize = true;
            lblObject.Location = new Point(565, 44);
            lblObject.Name = "lblObject";
            lblObject.Size = new Size(42, 15);
            lblObject.TabIndex = 3;
            lblObject.Text = "Object";
            // 
            // numLocationZ
            // 
            numLocationZ.Location = new Point(638, 70);
            numLocationZ.Name = "numLocationZ";
            numLocationZ.Size = new Size(103, 23);
            numLocationZ.TabIndex = 4;
            // 
            // lblLocationZ
            // 
            lblLocationZ.AutoSize = true;
            lblLocationZ.Location = new Point(569, 72);
            lblLocationZ.Name = "lblLocationZ";
            lblLocationZ.Size = new Size(63, 15);
            lblLocationZ.TabIndex = 5;
            lblLocationZ.Text = "Location Z";
            // 
            // RenderForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 536);
            Controls.Add(lblLocationZ);
            Controls.Add(numLocationZ);
            Controls.Add(lblObject);
            Controls.Add(comboObjects);
            Controls.Add(btnRender);
            Controls.Add(renderBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "RenderForm";
            Text = "Elite Sharp Renderer";
            ((System.ComponentModel.ISupportInitialize)renderBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLocationZ).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox renderBox;
        private Button btnRender;
        private ComboBox comboObjects;
        private Label lblObject;
        private NumericUpDown numLocationZ;
        private Label lblLocationZ;
    }
}
