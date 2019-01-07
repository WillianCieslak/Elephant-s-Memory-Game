using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElephantsMemory
{
    //Object Square, extends from PictureBox

    class Square : PictureBox
    {
        private int speed = 0;
        private int size = 0;
        private Random random;

        public Square()
        {
            random = new Random();
            size = random.Next(10,30);
            this.Size = new Size(size, size);
            this.BackColor = DefineColor();
            this.Top = random.Next(0,800);
            this.Left = 0;
            this.speed = random.Next(1,5);
        }

        //Update the width
        public void Update(Panel panel)
        {
            this.Width += size;
        }

        //Generate random color
        private System.Drawing.Color DefineColor()
        {
            int max = byte.MaxValue + 1; // 256
            int r = random.Next(max);
            int g = random.Next(max);
            int b = random.Next(max);
            System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);

            return c;
        }
    }
}
