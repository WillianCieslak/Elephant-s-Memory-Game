using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ElephantsMemory
{
    public partial class Form : System.Windows.Forms.Form
    {
        private List<Square> squares; //List of PicturerBoxes that runs animation at the end of the game
        private AllCards allCards;    //Instance of AllCards class
        private Card card1;           //Instance of Card, used to compare with another card
        private Card card2;           //Instance of Card, used to compare with another card
        private Thread thread;        //Thread that will remove cards from the list if they match, leave them up and change their borders
        private Panel animation;      //Panel that goes over the Form when the game ends, List of squares will run in this panel
        private int timerForSquare;   //Timer that creates and adds a new square in the animation
        private int count = 0;        //Counter to see how many cards have been clicked, if > 1, count == 0
        private int restart;          //Counter that enables Button Exit and Restart to appear after a few seconds when the game ends  
        private int numberMoves;      //Counts how many moves the player had
        private int seconds;          //Counts seconds untill the player finishes the game
        private int size;             //Receives a value from db too see how many cards are needed to start the game
        private int compareCounter;   //Counter that leaves cards up for a few seconds before turning them down if they dont match
        private DateTime time;        //Object used to manipulate seconds

        //Initialization of the Form
        public Form()
        {
            InitializeComponent();
            squares = new List<Square>();
            time = new DateTime();
            compareCounter = 0;
            timerForSquare = 0;
            restart = 0;
            numberMoves = 0;
            seconds = 0;
        }

        //After Form was created load its content 
        //Return the size from the db
        //Based on the size add cards to the form
        //Start the seconds timer
        //Creates button click event for all the cards on the form
        private void Form_Load(object sender, EventArgs e)
        {
            size = DBConnection.Instance.SizeGame();
            allCards = new AllCards(size);

            for (int i = 0; i < allCards.randomCards.Count; i++)
            {
                flowLayoutPanel.Controls.Add(allCards.randomCards[i]);
                allCards.randomCards[i].Click += new System.EventHandler(this.Button_Click);
            }

            gameTimer.Start();
        }

        //Button Click event
        private void Button_Click(object sender, System.EventArgs e)
        {
            Card btn = sender as Card;
            btn.BackgroundImage = Image.FromFile(btn.FrontImage);
            CompareCards(btn);
        }

        //Compare select cards
        private void CompareCards(Card card)
        {            
            if (count == 0)
            {
                card1 = new Card();
                card1 = card;
                count++;

            } else if (count == 1)
            {
                numberMoves++;
                lblNumberMoves.Text = "Number of moves: " + numberMoves;

                card2 = new Card();
                card2 = card;
                count = 0;

                //If the same cards is clicked twice or the cards are different, turn them back down
                if (card == card1 || card1.Type != card2.Type)
                {
                    timer.Start();
                    
                } else
                {
                    RemoveCards(card1, card2);
                    if (allCards.randomCards.Count == 0)
                    {
                        //If there are no cards left, finish the game
                        gameTimer.Stop();
                        FinishGame();
                    }
                }                
            }
        }

        //Method to desable cards when they match
        private void RemoveCards(Card card1, Card card2)
        {
            allCards.randomCards.Remove(card1);
            allCards.randomCards.Remove(card2);

            thread = new Thread(() => CardAnimation(card1, card2));
            thread.Start();            
        }

        //Delegate from Thread
        void CardAnimation(Card card1, Card card2)
        {
            UpdateCard(card1, card2);
            thread.Abort();
            thread.Join();
        }

        //Invoker method with parameters
        void UpdateCard(Card card1, Card card2)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    UpdateCard(card1, card2);
                });
                return;
            }
            else
            {
                card1.BackgroundImage = Image.FromFile(card1.FrontImage);
                card2.BackgroundImage = Image.FromFile(card2.FrontImage);
                card1.Enabled = false;
                card2.Enabled = false;

                card1.FlatStyle = FlatStyle.Flat;
                card2.FlatStyle = FlatStyle.Flat;
                card1.FlatAppearance.BorderSize = 3;
                card2.FlatAppearance.BorderSize = 3;
                card2.FlatAppearance.BorderColor = Color.Blue;
                card1.FlatAppearance.BorderColor = Color.Blue;
            }
        }

        //Reduces opacity from the Form untill it disappears
        private void FinishGame()
        {
            this.Controls.Remove(flowLayoutPanel);

            for (double i = 0.1; i < 10; i++)
            {
                this.Opacity -= 0.1;
                this.Refresh();
            }            

            EndAnimation();            
        }

        //When the game ends creates a new PicturesBox and create some opacity effect to close and open the form
        private void EndAnimation()
        {
            animation = new Panel();
            animation.Size = new Size(this.Width, this.Height);
            animation.BackColor = Color.Brown;
            animation.Anchor = AnchorStyles.Top;
            this.Controls.Add(animation);

            for (double i = 0.1; i < 10; i++)
            {
                this.Opacity += 0.1;
                this.Refresh();
            }
            
            squareTimer.Start();

            lblNumberMoves.Location = new Point(20,20);
            lblTimer.Location = new Point(20,50);
            menuStrip.Visible = false;
        }

        //Timer that does the comparison and turn the cards back again if they dont match
        private void timer1_Tick(object sender, EventArgs e)
        {
            compareCounter++;
            if(compareCounter > 4)
            {
                for(int i = 0; i < allCards.randomCards.Count; i++)
                {
                    allCards.randomCards[i].BackgroundImage = Image.FromFile("backcard.png");
                }                

                compareCounter = 0;
                timer.Stop();
            }                            
        }

        //Adds a new square to the list
        private void AddSquare(Square square)
        {
            animation.Controls.Add(square);
            squares.Add(square);
        }

        //Timer that displays the squares for the animation when the game ends
        private void EndTimer(object sender, EventArgs e)
        {
            timerForSquare++;
            restart++;
            if(timerForSquare > 1)
            {
                AddSquare(new Square());
                timerForSquare = 0;
            }

            if(restart > 75)
            {
                btnExit.Visible = true;
                btnRestart.Visible = true;
            }

            for (int indx = 0; indx < squares.Count; indx++)
            {
                squares[indx].Update(animation);
            }
        }

        //Timer in seconds
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            lblTimer.Text = "Time: " + time.AddSeconds(seconds).ToString("mm:ss");
        }

        //Button Exit
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //Button Restart
        private void btnRestart_Click(object sender, EventArgs e)
        {
            Restart();
        }

        //StripMenu Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //StripMenu size 8
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            UpdateSize(8);
        }

        //StripMenu size 16
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            UpdateSize(16);
        }

        //StripMenu size 24
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            UpdateSize(24);
        }

        //Method called to update the size of the game
        private void UpdateSize(int size)
        {
            DBConnection.Instance.UpdateSizeGame(size);
            Restart();
        }

        //Restart the game
        private void Restart()
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            this.Close(); //to turn off current app
        }
    }
}
