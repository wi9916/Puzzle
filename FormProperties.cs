using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class FormProperties : Form
    {
        public int NumRows;
        public int NumCollums;
        public FormProperties()
        {
            InitializeComponent();                      
        }               
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NumCollums = Convert.ToInt32(numericUpDown1.Value);          
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NumRows = Convert.ToInt32(numericUpDown2.Value);          
        }

        private void FormProperties_Load_1(object sender, EventArgs e)
        {
            numericUpDown1.Value = NumCollums;
            numericUpDown2.Value = NumRows;          
        }

        private void button1_Click(object sender, EventArgs e){}
    }
}
