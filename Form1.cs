using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        int x = 50; // position x
        int y = 50; // position y
        int dx = 20; // speed (right)
        int dy = 0;

        List<Point> snake = new List<Point>(); // list with Point-objects (each Point have X and Y values - coordinates to a segment)

        Random rand = new Random();
        int foodX, foodY;

        private readonly System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Timer = WinForms-timer
        public Form1()
        {
            InitializeComponent(); // Creates the window
            const int cell = 20;
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            GameTimer.Tick += GameLoop; // Connects the timer Tick-event to method GameLoop
            GameTimer.Start(); // Starting timer
            this.KeyDown += KeyIsDown;
            this.DoubleBuffered = true;
            snake.Add(new Point(50, 50)); // Start position for the head
            SpawnFood();
        }
        private void GameLoop(object sender, EventArgs e) // Runs every "tick"
        {
            // Calculating new head for head
            int newX = snake[0].X + dx;
            int newY = snake[0].Y + dy;

            if (newX < 0)
            {
                newX = this.ClientSize.Width - 20;
            } else if (newX >= this.ClientSize.Width)
            {
                newX = 0;
            }

            if (newY < 0)
            {
                newY = this.ClientSize.Width - 20;
            } else if (newY >= this.ClientSize.Width)
            {
                newY = 0;
            }

            // Moving snake body backwards
            for (int i = snake.Count - 1; i > 0; i--)
            {
                snake[i] = snake[i - 1];
            }

            snake[0] = new Point(newX, newY);

            // Collision with food
            if (newX < foodX + 20 && 
                newX + 20 > foodX &&
                newY < foodY + 20 &&
                newY + 20 > foodY) 
            {
                SpawnFood(); // if true; move food to new random location

                snake.Add(snake[snake.Count - 1]); // Adding a new tailjoint

                if (GameTimer.Interval > 20)
                {
                    GameTimer.Interval -= 5;
                }
            }
            
            Invalidate(); // Draw new window -> will trigger OnPaint-method
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var segment in snake)
            {
                e.Graphics.FillRectangle(Brushes.Green, segment.X, segment.Y, 20, 20);
            }
            
            e.Graphics.FillRectangle(Brushes.Red, foodX, foodY, 20, 20);

            //e.Graphics.FillRectangle(Brushes.Green, x, y, 20, 20);
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
            private void SpawnFood()
            {
                int maxX = (this.ClientSize.Width / 20) - 1;
                int maxY = (this.ClientSize.Width / 20) - 1;

                foodX = rand.Next(0, this.ClientSize.Width / maxX) * 20;
                foodY = rand.Next(0, this.ClientSize.Width / maxY) * 20;

                Point newFood;
                bool onSnake;

            do
            {
                onSnake = false;
                int fx = rand.Next(0, maxX) * 20;
                int fy = rand.Next(0, maxY) * 20;
                newFood = new Point(fx, fy);

                foreach (var segment in snake)
                {
                    if (segment == newFood)
                    {
                        onSnake = true;
                        break;
                    }
                }
            }

            while (onSnake);

            foodX = newFood.X;
            foodY = newFood.Y;

            }
        
    }
}
