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
            comboPlanets = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)renderBox).BeginInit();
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
            // comboPlanets
            // 
            comboPlanets.FormattingEnabled = true;
            comboPlanets.Location = new Point(609, 41);
            comboPlanets.Name = "comboPlanets";
            comboPlanets.Size = new Size(103, 23);
            comboPlanets.TabIndex = 2;
            comboPlanets.SelectedIndexChanged += ComboPlanets_SelectedIndexChanged;
            // 
            // frmRender
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 536);
            Controls.Add(comboPlanets);
            Controls.Add(btnRender);
            Controls.Add(renderBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmRender";
            Text = "Elite Sharp Renderer";
            ((System.ComponentModel.ISupportInitialize)renderBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox renderBox;
        private Button btnRender;
        private ComboBox comboPlanets;
    }
}
