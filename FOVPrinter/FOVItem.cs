using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOVPrinter
{
    public abstract class FOVItem
    {
        public float xCenter { get; set; }
        public float yCenter { get; set; }
        public string text { get; set; }
        public float fontSize { get; set; }
        public bool crosshair { get; set; }
        public abstract void draw(Graphics g);
    }
}
