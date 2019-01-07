using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ElephantsMemory
{
    class AllCards 
    {
        //Class that generates a list of Cards

        private List<Card> allCards;
        private List<Card> selectedCards;
        public List<Card> randomCards;
        private Random random;

        public AllCards(int size)
        {
            selectedCards = new List<Card>();
            allCards = new List<Card>();
            randomCards = new List<Card>();
            random = new Random();
            
            LoadCards();
            ShuffleCards(size);
        }

        //Load all the cards from the Database and add to a List
        private void LoadCards()
        {
            allCards = DBConnection.Instance.LoadCards();
        }

        //Shuffle cards adding to another list based on how many cards the player wants
        private void ShuffleCards(int size)
        {
            randomCards = new List<Card>();
            random = new Random();
            allCards.Sort((x, y) => -x.Type.CompareTo(y.Type));

            for(int i = 0; i < size; i++)
            {
                selectedCards.Add(allCards[i]);
            }

            while (randomCards.Count < selectedCards.Count)
            {
                int index = random.Next(0, selectedCards.Count);

                if (!randomCards.Contains(selectedCards[index]))
                {
                    randomCards.Add(selectedCards[index]);
                }
            }
        }
    }
}
