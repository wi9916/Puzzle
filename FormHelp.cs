using Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class FormHelp : Form
    {
        private readonly IAutoPuzzle _autoPuzzle;
        public FormHelp()
        {
            InitializeComponent();          
        }
        public FormHelp(IAutoPuzzle autoPuzzle)
        {
            _autoPuzzle = autoPuzzle;
            InitializeComponent();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {           
                _autoPuzzle.StartCreatePuzzle();
                _autoPuzzle.CreatePictureSegments(this, ClientSize);
        }
        
        private void FormHelp_SizeChanged(object sender, EventArgs e)
        {
            _autoPuzzle.CreatePictureSegments(this, ClientSize);            
        }
    }
}
