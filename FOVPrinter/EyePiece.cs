using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOVPrinter
{
    public class EyePiece : FOVItem
    {
        public EyePiece()
        {
            useStandardEyePiece = false;
            standardEyePiece = new StandardEyePiece("", "", 25f, 52f);
            xCenter = 250;
            yCenter = 250;
            text = "SW 130 PDS\r\n25 mm Plössl";
            fontSize = 10;
            crosshair = true;
            radius = 15;
        }

        public bool useStandardEyePiece { get; set; }
        public StandardEyePiece standardEyePiece;
        public float radius { get; set; }   //  circle radius in mm

        public override void draw(Graphics g)
        {
            float x0 = xCenter - radius;
            float y0 = yCenter - radius;
            using (Pen p = new Pen(Color.Black))
            {
                g.DrawEllipse(p, x0, y0, radius * 2f, radius * 2f);

                if (this.crosshair)
                {
                    g.DrawLine(p, x0, y0 + radius, x0 + 2 * radius, y0 + radius);
                    g.DrawLine(p, x0 + radius, y0, x0 + radius, y0 + 2 * radius);
                }
            }

            if (text.Length != 0)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                using (Font f = new Font("Arial", fontSize))
                using (SolidBrush b = new SolidBrush(Color.Black))
                {
                    float width = radius * 2;
                    SizeF textSz = g.MeasureString(this.text, f);
                    RectangleF textRect = new RectangleF((x0 + radius - textSz.Width / 2f), (y0 + 2 * radius), textSz.Width, textSz.Height);
                    g.DrawString(text, f, b, textRect, sf);
                    //g.DrawRectangle(Pens.Black, textRect.Left, textRect.Top, textRect.Width, textRect.Height);
                }
            }
        }

    }
}
