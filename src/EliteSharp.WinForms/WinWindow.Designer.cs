using System.Windows.Forms;

namespace EliteSharp.GDI;

#pragma warning disable CA1031 // Do not catch general exception types
public sealed partial class WinWindow
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
        if (disposing)
        {
            components?.Dispose();
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinWindow));
        screen = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)screen).BeginInit();
        SuspendLayout();
        // 
        // screen
        // 
        screen.BackColor = Color.Gray;
        screen.Location = new Point(0, 0);
        screen.Name = "screen";
        screen.Size = new Size(512, 512);
        screen.TabIndex = 1;
        screen.TabStop = false;
        // 
        // WinWindow
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Magenta;
        ClientSize = new Size(512, 512);
        Controls.Add(screen);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        MaximizeBox = false;
        MaximumSize = new Size(528, 551);
        MinimumSize = new Size(528, 551);
        Name = "WinWindow";
        KeyDown += GameWindow_KeyDown;
        KeyUp += GameWindow_KeyUp;
        ((System.ComponentModel.ISupportInitialize)screen).EndInit();
        ResumeLayout(false);
    }

    #endregion
    private PictureBox screen;
}
