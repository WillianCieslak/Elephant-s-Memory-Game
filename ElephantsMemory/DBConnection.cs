using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ElephantsMemory
{
    class DBConnection
    {
        //Class responsible for opening connection with a database, making queries, updating and return values from the db
        //Using Singleton Pattern

        private static DBConnection instance = null;

        //Private Constructor.  
        private DBConnection()
        {

        }

        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnection();
                }
                return instance;
            }
        }

        //Method to create a connection
        private static OleDbConnection GetConnection()
        {
            string connectionString;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path = path + "\\Database\\ElephantsMemory.mdb";
            connectionString = @"Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path;

            return new OleDbConnection(connectionString);
        }

        //Method that returns the cards stored in the db
        public List<Card> LoadCards()
        {
            List<Card> cards = new List<Card>();
            OleDbConnection myConnection = GetConnection();

            string myQuery = "SELECT * FROM Card";
            OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);

            try
            {
                myConnection.Open();
                OleDbDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    Card card = new Card(int.Parse(myReader["id"].ToString()), myReader["type"].ToString(), myReader["front_image"].ToString());
                    cards.Add(card);
                }
                return cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in DBHandler", ex);
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }

        //Method that updates the size of the game chosen by the player
        public void UpdateSizeGame(int size)
        {
            OleDbConnection myConnection = GetConnection();
            OleDbCommand myCommand = new OleDbCommand(@"UPDATE number_of_cards
                                                    SET [size] = @size
                                                    WHERE [id] = @id", myConnection);
            myCommand.Parameters.AddWithValue("@size", size);
            myCommand.Parameters.AddWithValue("@id", 1);
            
            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                MessageBox.Show("Size updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in DBHandler" + ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

        //Returns the value to know how many cards are needed to start the game
        public int SizeGame()
        {
            int size = 0;

            OleDbConnection myConnection = GetConnection();
            string myQuery = "SELECT * FROM number_of_cards";
            OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);

            try
            {
                myConnection.Open();
                OleDbDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    size = (int.Parse(myReader["size"].ToString()));
                }

                return size;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in DBHandler", ex);
                return 0;
            }
            finally
            {
                myConnection.Close();
            }            
        }
    }
}
