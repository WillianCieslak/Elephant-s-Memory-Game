using System.Windows.Forms;
using System.Drawing;

namespace ElephantsMemory
{
    //Object Cards, extends from Button

    class Card : Button
    {
        private string type;
        private string frontImage;
        private int id;

        public Card()
        {
        }

        public Card(int id, string type, string frontImage)
        {            
            this.Size = new Size(120, 170);
            this.BackgroundImage = Image.FromFile("backcard.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.id = id;
            this.type = type;
            this.frontImage = frontImage;
        }

        public string Type { get => type; set => type = value; }
        public string FrontImage { get => frontImage; set => frontImage = value; }
    }
}
