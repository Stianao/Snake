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
        int delta_x = 5; // starting speed (right) delta x
        int delta_y = 0; // delta y
        const int cell = 20; // One cell = 20 pixels
        bool gameOver = false;
        int score = 0; // Counting variable for game points

        List<Point> snake = new List<Point>(); // list with Point-objects (each Point have X and Y values - coordinates to a segment)

        Random rand = new Random();
        int foodX, foodY;

        private readonly System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Timer = WinForms-timer
        public Form1()
        {
            InitializeComponent(); // Creates the window
            this.ClientSize = new Size(800, 600); // Window size object - length: 800, height: 600
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            this.MaximizeBox = false;
            GameTimer.Tick += GameLoop; // Connects the timer Tick-event to method GameLoop
            GameTimer.Start(); // Starting timer
            this.KeyDown += KeyIsDown;
            this.DoubleBuffered = true;
            snake.Add(new Point(50, 50)); // Start position for the head
            SpawnFood();
        }
        private void GameLoop(object sender, EventArgs e) // Method runs every "tick"
        {
            // Calculating new head for head. snake[0] is the head. First element in list.
            int newX = snake[0].X + delta_x; 
            int newY = snake[0].Y + delta_y;

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

            Point head = snake[0]; // head = snakes new head position

            // Collision check for head and body, from element 1 to element n
            // for-loop checking every "body joint" from index 1 and onwards
            // Point is a valuetype comparing X and Y

            for (int i = 1; i < snake.Count; i++)// starting at 1: We do not want to compare head (element 0) with itself
            {
                if (snake[i] == head) // Point compared to X and Y value-equality
                {
                    GameTimer.Stop(); // Stop the game - the timer stops
                    gameOver = true;
                    MessageBox.Show("Game Over!"); // Feedback in a popup window - game over
                    MessageBox.Show("Restart Game. Press R"); // Popup window for reset game, press R key on keyboard
                    return; // returns to cancel method from running food, drawing etc.
                }
            }

            // Collision with food
            if (newX < foodX + cell && 
                newX + cell > foodX &&
                newY < foodY + cell &&
                newY + cell > foodY) 
            {
                SpawnFood(); // if true -> move food to new random location
                
                score++; // Incrementing score

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
            e.Graphics.DrawString($"Score: {score}", SystemFonts.DefaultFont, Brushes.Black, 10, 10);
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver && e.KeyCode == Keys.R)
            {
                ResetGame();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Right:
                    delta_x = 5; delta_y = 0;
                    break;

                case Keys.Left:
                    delta_x = -5; delta_y = 0;
                    break;

                case Keys.Up:
                    delta_x = 0; delta_y = -5;
                    break;

                case Keys.Down:
                    delta_x = 0; delta_y = 5;
                    break;
            }
        }
        private void ResetGame()
        {
            snake.Clear(); // Removes all elements from the list - snake head and body

            snake.Add(new Point(x, y));
            
            delta_x = 5; // Resetting the snake speed back to 5 pixels -> right movement along x-axis
            delta_y = 0; // Want y to be zero

            score = 0; 
            
            SpawnFood(); // Calling method to reset food spawning and positioning
            
            gameOver = false;
            
            GameTimer.Start(); // Restarting game -> Starts the timer
        }

            private void SpawnFood()
            {
                int maxX = (this.ClientSize.Width / cell) - 1; // x-direction
                int maxY = (this.ClientSize.Height / cell) - 1; // y-direction

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

                foreach (var segment in snake) // segment = element in snake-list -> segment = body joint(cell) of the snake
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
