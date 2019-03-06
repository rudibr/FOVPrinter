using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FOVPrinter
{
    public partial class EyepieceSetup : Form
    {
        private Dictionary<string, List<StandardEyePiece>> standardEyePieces;

        public EyePiece eyepiece { get; set; }
        public EyepieceSetup()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //  set values
            checkBoxEPFromList.Checked = eyepiece.useStandardEyePiece;

            numericUpDownFocalLength.Value = (decimal)eyepiece.standardEyePiece.focalLength;
            numericUpDownApparantFOV.Value = (decimal)eyepiece.standardEyePiece.apparantFOV;

            numericUpDownXCenter.Value = (decimal)eyepiece.xCenter;
            numericUpDownYCenter.Value = (decimal)eyepiece.yCenter;

            checkBoxCrosshair.Checked = eyepiece.crosshair;

            numericUpDownFontSize.Value = (decimal)eyepiece.fontSize;

            textBoxText.Text = eyepiece.text;

            standardEyePieces = new Dictionary<string, List<StandardEyePiece>>();
            foreach (StandardEyePiece ep in StandardEyePiece.standardEyePieces)
            {
                if (!standardEyePieces.ContainsKey(ep.brand))
                    standardEyePieces[ep.brand] = new List<StandardEyePiece>();
                standardEyePieces[ep.brand].Add(ep);
            }

            //  fill in brands combo box
            foreach (string brand in standardEyePieces.Keys)
                comboBoxBrand.Items.Add(brand);

            if (standardEyePieces.ContainsKey(eyepiece.standardEyePiece.brand))
            {
                comboBoxBrand.SelectedItem = eyepiece.standardEyePiece.brand;
                repopulateModelCombo();
                comboBoxModel.SelectedItem = eyepiece.standardEyePiece.model;
            }

            udateEPEnabeling();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            eyepiece.useStandardEyePiece = checkBoxEPFromList.Checked;
            if (checkBoxEPFromList.Checked)
            {
                if (comboBoxBrand.SelectedIndex != -1)
                {
                    eyepiece.standardEyePiece.brand = comboBoxBrand.SelectedItem.ToString();
                    if (comboBoxModel.SelectedIndex != -1)
                        eyepiece.standardEyePiece.model = comboBoxModel.SelectedItem.ToString();
                }
            }

            eyepiece.standardEyePiece.focalLength = (float)numericUpDownFocalLength.Value;
            eyepiece.standardEyePiece.apparantFOV = (float)numericUpDownApparantFOV.Value;

            eyepiece.xCenter = (float)numericUpDownXCenter.Value;
            eyepiece.yCenter = (float)numericUpDownYCenter.Value;

            eyepiece.crosshair = checkBoxCrosshair.Checked;

            eyepiece.fontSize = (float)numericUpDownFontSize.Value;

            eyepiece.text = textBoxText.Text;
        }

        private void checkBoxEPFromList_CheckedChanged(object sender, EventArgs e)
        {
            udateEPEnabeling();
        }

        private void udateEPEnabeling()
        {
            numericUpDownFocalLength.Enabled = !checkBoxEPFromList.Checked;
            numericUpDownApparantFOV.Enabled = !checkBoxEPFromList.Checked;

            comboBoxBrand.Enabled = checkBoxEPFromList.Checked;
            comboBoxModel.Enabled = checkBoxEPFromList.Checked;
        }

        private void comboBoxBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            repopulateModelCombo();
        }

        private void repopulateModelCombo()
        {
            comboBoxModel.Items.Clear();

            if (!standardEyePieces.ContainsKey(comboBoxBrand.SelectedItem.ToString()))
                return;

            //  fill in model combo box
            foreach (StandardEyePiece ep in standardEyePieces[comboBoxBrand.SelectedItem.ToString()])
                comboBoxModel.Items.Add(ep.model + " - FL: " + ep.focalLength.ToString() + " [mm] - AF: " + ep.apparantFOV.ToString() + " [°]");
        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!standardEyePieces.ContainsKey(comboBoxBrand.SelectedItem.ToString()))
                return;

            int modelIndex = comboBoxModel.SelectedIndex;
            if (modelIndex < 0)
                return;

            //  get data for the selected eyepiece
            StandardEyePiece ep = standardEyePieces[comboBoxBrand.SelectedItem.ToString()][modelIndex];
            numericUpDownApparantFOV.Value = (decimal)ep.apparantFOV;
            numericUpDownFocalLength.Value = (decimal)ep.focalLength;
        }
    }
}
