using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Hamscoffee
{
    public class resizeimg
    {
        public Image resize(Image img, int w, int h)
        {
            Bitmap b = new Bitmap(img, w, h);
            Graphics g = Graphics.FromImage((Image)b);
            g.DrawImage(img, 0, 0, w, h);
            g.Dispose();
            return (Image)b;
        }
    }
}