using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 비매샵테스트
{
    public partial class coloraddForm : Form
    {
        autoaimColorForm _frm;
        public coloraddForm()
        {
            InitializeComponent();
        }

        public coloraddForm(autoaimColorForm frm)
        {
            InitializeComponent();
            _frm = frm;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDown4.Value < numericUpDown1.Value)
            {
                numericUpDown4.Value = numericUpDown1.Value;
            }
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDown4.Value < numericUpDown1.Value)
            {
                numericUpDown1.Value = numericUpDown4.Value;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDown5.Value < numericUpDown2.Value)
            {
                numericUpDown5.Value = numericUpDown2.Value;
            }
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
             if(numericUpDown5.Value < numericUpDown2.Value)
            {
                numericUpDown2.Value = numericUpDown5.Value;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
             if(numericUpDown6.Value < numericUpDown3.Value)
            {
                numericUpDown6.Value = numericUpDown3.Value;
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
             if(numericUpDown6.Value < numericUpDown3.Value)
            {
                numericUpDown3.Value = numericUpDown6.Value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _frm.rgbChange(Convert.ToInt32(numericUpDown1.Value),Convert.ToInt32(numericUpDown4.Value),Convert.ToInt32(numericUpDown2.Value),Convert.ToInt32(numericUpDown5.Value),Convert.ToInt32(numericUpDown3.Value),Convert.ToInt32(numericUpDown6.Value));
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
