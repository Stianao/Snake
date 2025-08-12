using System;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        int x = 50; // position x
        int y = 50; // position y
        int dx = 5; // speed (right)
        int dy = 0;
        private readonly System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Timer = WinForms-timer
        public Form1()
        {
            InitializeComponent(); // Creates the window
            GameTimer.Tick += GameLoop; // Connects the timer Tick-event to method GameLoop
            GameTimer.Start(); // Starting timer
            this.KeyDown += KeyIsDown;
        }
        private void GameLoop(object sender, EventArgs e) // Runs every "tick"
        {
            x += dx;
            y += dy;
            Invalidate(); // Draw new window -> will trigger OnPaint-method
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.Green, x, y, 20, 20);
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // dx = delta X
                // dy = delta Y

                case Keys.Right:
                    dx = 5; dy = 0;
                    break;

                case Keys.Left:
                    dx = -5; dy = 0;
                    break;

                case Keys.Up:
                    dx = 0; dy = -5;
                    break;

                case Keys.Down:
                    dx = 0; dy = 5;
                    break;
            }
        }
    }
}
