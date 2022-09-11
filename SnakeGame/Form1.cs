using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class frmSnake : Form
    {
        Random rand; // for generating random bounus in random place
        // 3 types of fields in a game: empty field, snake and bonus
        enum GameBoardFields
        {
            Free, 
            Snake,
            Bonus
        };
        
        // enum for controlling the snake
        enum Directions
        {
            Up,
            Down,
            Left,
            Right
        };
     
        // It is needed to know where the snake is (what coordinates it has)
        struct SnakeCoordinates
        {
            public int x;
            public int y;
        }
        // create game board
        GameBoardFields[,] gameBoardField;
        // snake coordinates
        SnakeCoordinates[] snakeXY;
        // snake length
        int snakeLength;
       
        // variable which keeps the direction to follow by the snake
        Directions direction;
        // Paint everything
        // Graphics - class which allows for painting on the surface
        Graphics g; // Graphics to klasa, sluzaca to rysowania po powierzchni 

        // Initialize variables in constructor
        public frmSnake()
        {
            InitializeComponent();
            gameBoardField = new GameBoardFields[11, 11]; // field 12x12 
            snakeXY = new SnakeCoordinates[100]; // place where the snake can be (coordinates
            // from 1 to 10 so field is 10x10=100, because 11 is for the walls of the game field)
            rand = new Random();
        }
        
        // ********** Logic of the game **********
        private void frmSnake_Load(object sender, EventArgs e)
        {
            picGameBoard.Image = new Bitmap(420, 420); // prepare pictureBox for showing images
            
            g = Graphics.FromImage(picGameBoard.Image); // take graphics from images
            g.Clear(Color.Black); // fill the background by the choosen color
            for (int i = 1; i <= 10; i++) // filling gameboard by walls
            {
                // top and bottom walls      
                g.DrawImage(imgList.Images[6], i*35, 0); // top wall (y=0) - it refers to list
                // with images (image 6 is an image of wall)
                g.DrawImage(imgList.Images[6], i*35, 385); // it's a bottom wall 420-35=385
                // (385 for discribing the range of wall)
            }
            for (int i = 0; i <= 11; i++) // corners of the walls so i<=11 
            {
                // left and right walls      
                g.DrawImage(imgList.Images[6], 0, i*35); // left wall (x = 0)
                g.DrawImage(imgList.Images[6], 385, i*35); // righ wall (x=385)
            }
            // Initialize the snake - indexes describe starting position (x,y) on the game field
            snakeXY[0].x = 5; // head
            snakeXY[0].y = 5; 
            snakeXY[1].x = 5; // first body part
            snakeXY[1].y = 6;
            snakeXY[2].x = 5; // second body part
            snakeXY[2].y = 7;
            // print it on the game field
            g.DrawImage(imgList.Images[5], 5*35, 5*35); // head
            g.DrawImage(imgList.Images[4], 5 * 35, 6 * 35); // first body part
            g.DrawImage(imgList.Images[4], 5 * 35, 7 * 35); // second body part
            // follow the snake
            gameBoardField[5, 5] = GameBoardFields.Snake;
            gameBoardField[5, 6] = GameBoardFields.Snake;
            gameBoardField[5, 7] = GameBoardFields.Snake;

            direction = Directions.Up;
            snakeLength = 3; // starting length of the snake
            // bonuses - every moment game generates 4 bonuses
            for (int i=0; i < 4; i++)
            {
                Bonus();
            }
        }
        // function generating bonuses
        private void Bonus()
        {
            int x, y;
            var imgIndex = rand.Next(0, 4); // draw the bonus (there're 4 different types)
            do
            {
                x = rand.Next(1, 10); // starts from x=1 because x=0 is a wall 
                y = rand.Next(1, 10); 
            }
            while (gameBoardField[x, y] != GameBoardFields.Free); // it's necessary to put
            // bonuses in a place where is no snake
            
            // print it on the game field
            gameBoardField[x, y] = GameBoardFields.Bonus;
            g.DrawImage(imgList.Images[imgIndex], x*35, y*35);
        }
        
        // obsluga klawiszy strzalek
        // klikajac na okno Form, w properties > Events > Key > Wpisuje nazwe dla Key Down i sie formuje funkcja 
        
        // controlling the snake
        private void frmSnake_KeyDown(object sender, KeyEventArgs e) // KeyEventArg describes is the key pressed
        {
            switch(e.KeyCode)
            {
                case Keys.Up:
                    direction = Directions.Up;
                    break;
                case Keys.Down:
                    direction = Directions.Down;
                    break;
                case Keys.Left:
                    direction = Directions.Left;
                    break;
                case Keys.Right:
                    direction = Directions.Right;
                    break;
            }
        }
        // timer > event > Tick
        private void timer_Tick(object sender, EventArgs e)
        {
            
            // delete last part of the snake because of its movement
            g.FillRectangle(Brushes.Black, snakeXY[snakeLength - 1].x * 35,
                snakeXY[snakeLength - 1].y * 35, 35, 35);
            gameBoardField[snakeXY[snakeLength - 1].x, snakeXY[snakeLength - 1].y] = GameBoardFields.Free;
            picGameBoard.Refresh();
            // move the snake in the selected movement
            for(int i = snakeLength; i >= 1; i--) // i>=1 allows to keep the snake's head
            {
                snakeXY[i].x = snakeXY[i - 1].x;
                snakeXY[i].y = snakeXY[i - 1].y;
            }
            g.DrawImage(imgList.Images[4], snakeXY[0].x * 35, snakeXY[0].y * 35);
            picGameBoard.Refresh();
            
            // Change the direction of the movement because of the pressed key
            switch (direction)
            {
                case Directions.Up:
                    // changes only y
                    snakeXY[0].y = snakeXY[0].y - 1;
                    break;
                case Directions.Down:
                    snakeXY[0].y = snakeXY[0].y + 1;
                    break;
                case Directions.Left:
                    // changes only x
                    snakeXY[0].x = snakeXY[0].x - 1;
                    break;
                case Directions.Right:
                    snakeXY[0].x = snakeXY[0].x + 1;
                    break;
            }
            // Has the snake hit the wall
            if (snakeXY[0].x < 1 || snakeXY[0].x > 10 || snakeXY[0].y < 1 || snakeXY[0].y > 10)
            {
                GameOver();
                picGameBoard.Refresh();
                return;
            }
            // Has the snake bitten itself
            if (gameBoardField[snakeXY[0].x, snakeXY[0].y] == GameBoardFields.Snake)
            {
                GameOver();
                picGameBoard.Refresh();
                return;
            }
            // sprawdzenie czy waz zjadl element dodajacy punkt (bonus)
            // Has the snake eaten bonus (adding the point)
            if (gameBoardField[snakeXY[0].x, snakeXY[0].y] == GameBoardFields.Bonus)
            {
                g.DrawImage(imgList.Images[4], snakeXY[snakeLength].x * 35,
                    snakeXY[snakeLength].y * 35); // image at index 4 is a snake's body
                // empty field needs to be filled by the snake's body
                gameBoardField[snakeXY[snakeLength].x, snakeXY[snakeLength].y] = GameBoardFields.Snake;
                snakeLength++;
                if(snakeLength < 96)
                    Bonus();
                // increase the score 
                this.Text = "Snake - score: " + snakeLength;
            }
            // Print snake's head
            g.DrawImage(imgList.Images[5], snakeXY[0].x * 35, snakeXY[0].y * 35);
            gameBoardField[snakeXY[0].x, snakeXY[0].y] = GameBoardFields.Snake;

            picGameBoard.Refresh();
        }
        // Snake moves into the selected direction in every tick of the timer

        // function GameOver
        private void GameOver()
        {
            // function has to stop the game so the timer must to be stopped
            timer.Enabled = false;
            MessageBox.Show("GAME OVER"); 
        }

        private void picGameBoard_Click(object sender, EventArgs e)
        {

        }
    }
}
