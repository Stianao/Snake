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
        int dx = 5; // starting speed (right)
        int dy = 0; // 
        const int cell = 20; // One cell = 20 pixels

        List<Point> snake = new List<Point>(); // list with Point-objects (each Point have X and Y values - coordinates to a segment)

        Random rand = new Random();
        int foodX, foodY;

        private readonly System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Timer = WinForms-timer
        public Form1()
        {
            InitializeComponent(); // Creates the window
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
                newX = this.ClientSize.Width - cell;
            } else if (newX >= this.ClientSize.Width)
            {
                newX = 0;
            }

            if (newY < 0)
            {
                newY = this.ClientSize.Height - cell;
            } else if (newY >= this.ClientSize.Height)
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
            if (newX < foodX + cell && 
                newX + cell > foodX &&
                newY < foodY + cell &&
                newY + cell > foodY) 
            {
                SpawnFood(); // if true; move food to new random location

                snake.Add(snake[snake.Count - 1]); // Adding a new tailjoint

                if (GameTimer.Interval > cell)
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
                e.Graphics.FillRectangle(Brushes.Green, segment.X, segment.Y, cell, cell);
            }
            
            e.Graphics.FillRectangle(Brushes.Red, foodX, foodY, cell, cell);

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
                int maxX = (this.ClientSize.Width / cell) - 1;
                int maxY = (this.ClientSize.Height / cell) - 1;

                foodX = rand.Next(0, this.ClientSize.Width / maxX) * cell;
                foodY = rand.Next(0, this.ClientSize.Height / maxY) * cell;

                Point newFood;
                bool onSnake;

            do
            {
                onSnake = false;
                int fx = rand.Next(0, maxX) * cell;
                int fy = rand.Next(0, maxY) * cell;
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
