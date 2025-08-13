using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        int x = 50;                 // Position x 
        int y = 50;                 // Position y
        int delta_x = 5;            // Starting speed (horisontal right)
        int delta_y = 0;            // Snake is only supposed to move horisontal, so y must be 0 to not move. Only one way at a time.
        int foodX, foodY;           // horisontal and vertical coordinates for the food on the gameboard. (0, 0)
        int score = 0;              // Counting variable for game points

        bool gameOver = false;      // The game is running when false. Game is over when variable is set to true
        bool gameStarted = false;   // Flag used to show startscreen. Will be set to true to run game

        const int cell = 20;                // One cell = 20 pixels (size)
        const int scoreBoardSection = 40;   // 40 pixels to give room for both score text and snake/food wont be drawn over text

        Random rand = new Random(); // Creates a random object to generate random numbers for the game. Used for spawning food at random locations on the gameboard

        List<Point> snake = new List<Point>(); // list with Point-objects (each Point have X and Y values -> coordinates to a segment -> 1 segment is one body joint)

        private readonly System.Windows.Forms.Timer GameTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Timer = WinForms-timer. One Tick each 100 ms: GamerTime() have 10 ticks per second
        public Form1()
        {
            InitializeComponent(); // Creates the window
            this.ClientSize = new Size(800, 600); // Window size object - length: 800, height: 600
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            this.MaximizeBox = false;
            GameTimer.Tick += GameLoop; // Connects the timer Tick-event to method GameLoop
            //GameTimer.Start(); // Starting timer
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

            int topY = scoreBoardSection; // 
            int bottomY = this.ClientSize.Height - cell;

            if (newX < 0)
            {
                newX = this.ClientSize.Width - cell;
            } else if (newX >= this.ClientSize.Width)
            {
                newX = 0;
            }
            if (newY < topY)
            {
                newY = bottomY;
            } else if (newY > bottomY)
            {
                newY = topY;
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
                    Invalidate();
                    //MessageBox.Show("Game Over!"); // Feedback in a popup window - game over
                    //MessageBox.Show("Restart Game. Press R"); // Popup window for reset game, press R key on keyboard
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
            if (!gameStarted)
            {
                DrawMessageInCenter(e.Graphics, "Press ENTER to start");
                return;
            }
            else if (gameOver)
            {
                DrawMessageInCenter(e.Graphics, "Game Over\n" + "Press R to reset");
                return;
            }

            base.OnPaint(e);

            foreach (var segment in snake)
            {
                e.Graphics.FillRectangle(Brushes.Green, segment.X, segment.Y, cell, cell); // Drawng a green snake
            }

            e.Graphics.FillRectangle(Brushes.Red, foodX, foodY, cell, cell); // Drawing red food
            e.Graphics.DrawString($"Score: {score}", SystemFonts.DefaultFont, Brushes.Black, 10, 10); // Showing score at game screen
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted && e.KeyCode == Keys.Enter)
            {
                gameStarted = true;
                GameTimer.Start();
            }

            if (gameOver && e.KeyCode == Keys.R)
            {
                ResetGame();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Right: // Right key
                    delta_x = cell; delta_y = 0; // Moving 5 pixels to the right x direction, 0 y direction (each tick)
                    break;

                case Keys.Left: // Left key
                    delta_x = -cell; delta_y = 0; // Moving 5 pixels to the left x direction, 0 y direction (each tick)
                    break;

                case Keys.Up: // Up key
                    delta_x = 0; delta_y = -cell; // Moving 5 pixels up y direction, 0 x direction (each tick)
                    break;

                case Keys.Down: // Down key
                    delta_x = 0; delta_y = cell; // Moving 5 pixels down y direction, 0 x direction (each tick)
                    break;
            }
        }
        private void ResetGame()
        {
            snake.Clear(); // Removes all elements from the list - snake head and body

            snake.Add(new Point(x, y));
            
            delta_x = 5; // Resetting the snake speed back to 5 pixels -> right movement along x-axis
            delta_y = 0; // Want y to be zero

            score = 0; // resetting score back to zero at game reset (from game over)
            
            SpawnFood(); // Calling method to reset food spawning and positioning
            
            gameOver = false;
            
            GameTimer.Start(); // Restarting game -> Starts the timer
        }

            private void SpawnFood()
            {
                int maxX = (this.ClientSize.Width / cell) - 1; // x-direction
                int maxY = (this.ClientSize.Height / cell) - 1; // y-direction

                int minRow = scoreBoardSection / cell; 

                Point newFood;

                bool onSnake;

            do
            {
                onSnake = false;

                int fx = rand.Next(0, maxX);
                int fy = rand.Next(minRow, maxY);

                newFood = new Point(fx * cell, fy * cell);

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

        private void DrawMessageInCenter(Graphics g, string message)
        {
            using var fontMessage = new Font("Arial", 24);
            SizeF fontSize = g.MeasureString(message, fontMessage);
            float positionWidthX = (ClientSize.Width - fontSize.Width) / 2;
            float positionHeightY = (ClientSize.Height - fontSize.Height) / 2;

            g.DrawRectangle(Pens.Black,
                positionWidthX - 10,
                positionHeightY - 10,
                fontSize.Width + 20,
                fontSize.Height + 20
                );
            g.DrawString(message, 
                fontMessage, 
                Brushes.Black, 
                positionWidthX, 
                positionHeightY
                );

        }
    }
}
