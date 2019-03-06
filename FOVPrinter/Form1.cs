using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//  https://www.c-sharpcorner.com/article/printing-a-ruler-using-C-Sharp-and-gdi/

namespace FOVPrinter
{
    public partial class Form1 : Form
    {

        private EyePiece ep = new EyePiece();

        //  cached, time consuming to get
        private Rectangle pageRect;
        private Rectangle marginRect;

        public Form1()
        {
            InitializeComponent();

            printDocument1.PrintPage += PrintDocument1_PrintPage;

            getPrinterPageRects();
        }

        private void getPrinterPageRects()
        {
            pageRect = new Rectangle(0, 0, this.Width, this.Height);
            const int M = 25;
            marginRect = new Rectangle(M, M, this.Width - 2 * M, this.Height - 2 * M);

            PageSettings pageSettings = getPrinterPageSettings("");
            if (pageSettings == null)
                return;

            pageRect = new Rectangle(0, 0, pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);

            marginRect = new Rectangle(
                pageRect.Left + pageSettings.Margins.Left,
                pageRect.Top + pageSettings.Margins.Top,
                pageRect.Width - pageSettings.Margins.Left - pageSettings.Margins.Right,
                pageRect.Height - pageSettings.Margins.Top - pageSettings.Margins.Bottom);

            //  put page in landscape
            if (pageSettings.Landscape)
            {
                int w = pageRect.Width;
                pageRect.Width = pageRect.Height;
                pageRect.Height = w;

                w = marginRect.Width;
                marginRect.Width = marginRect.Height;
                marginRect.Height = w;
            }
        }
        private PageSettings getPrinterPageSettings(string printerName)
        {
            Cursor.Current = Cursors.WaitCursor;

            this.Refresh();
            this.update();

            PrinterSettings printerSettings;

            //  no printer name given, try getting default printer
            if (String.IsNullOrEmpty(printerName))
            {
                foreach (var printer in PrinterSettings.InstalledPrinters)
                {
                    printerSettings = new PrinterSettings();

                    printerSettings.PrinterName = printer.ToString();

                    if (printerSettings.IsDefaultPrinter)
                        return printerSettings.DefaultPageSettings;
                }

                return null;
            }

            //  get the printer settings
            printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printerName;
            return printerSettings.DefaultPageSettings;
        }

        private void SplitContainer1_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate(true);
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            PageSettings ps = e.PageSettings;
            //drawCircle(e.Graphics, RADIUS);
            ep.draw(e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(0.5f, 0.5f);

            base.OnPaint(e);

            //  show the page rectangle
            using (Pen blackPen = new Pen(Color.Black))
            using (Pen grayPen = new Pen(Color.LightGray))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(whiteBrush, pageRect);
                e.Graphics.DrawRectangle(blackPen, pageRect);
                e.Graphics.DrawRectangle(grayPen, marginRect);
            }

            ep.draw(e.Graphics);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
                getPrinterPageRects();
            }
        }

        private void buttonPrintPreview_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
            getPrinterPageRects();
        }

        private void buttonEyepiece_Click(object sender, EventArgs e)
        {
            EyepieceSetup epSetup = new EyepieceSetup();
            epSetup.eyepiece = this.ep;
            if (epSetup.ShowDialog() != DialogResult.OK)
                return;

            update();
            this.Invalidate();
        }

        public static float getFloat(string value, float defaultValue)
        {
            float result;

            //Try parsing in the current culture
            if (!float.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !float.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !float.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        private void update()
        {
            float telescopeFL = (float)numericUpDownTelescopeFL.Value;
            float chartScale = (float)numericUpDownChartScale.Value;

            float magnification = telescopeFL / ep.standardEyePiece.focalLength;

            float trueFOV = ep.standardEyePiece.apparantFOV / magnification;
            ep.radius = chartScale * trueFOV / 2f;   //  radius in mm

            //  convert to 100 * inch, the scale for printers
            ep.radius /= 25.4f;
            ep.radius *= 100f;

            Invalidate();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //  add version info to title text
            this.Text += " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";

            update();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            update();
            Invalidate();
        }

        private void numericUpDownChartScale_ValueChanged(object sender, EventArgs e)
        {
            update();
            Invalidate();
        }

        private void numericUpDownTelescopeFL_ValueChanged(object sender, EventArgs e)
        {
            update();
            Invalidate();
        }
    }
}
