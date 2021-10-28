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
    public partial class FormMain : Form
    {
        private readonly IPuzzle _puzzle;
        public FormMain(IPuzzle puzzle)
        {
            _puzzle = puzzle;
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e) {}

        private void PB_Click(object sender, EventArgs e)
        {
            _puzzle.TranslationSegment(sender as PictureBox);
        }
       
        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;          
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var folderPath = fbd.SelectedPath;
                string searchPattern = "*.*";
                _puzzle.Images = Directory.GetFiles(folderPath, searchPattern).Select(Image.FromFile).ToArray();
                _puzzle.CreatePictureSegments(this, ClientSize, PB_Click);

                                
            }
        }
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            _puzzle.RefreshPicture();
        }

        private void toolStripButtonImage_Click(object sender, EventArgs e)
        {
            var FormImage = new FormImage
            {
                NumRows = _puzzle.Rows,
                NumCollums = _puzzle.Collums
            };

            if (FormImage.ShowDialog() == DialogResult.OK) { }
        }
        
        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            var FormProperties = new FormProperties
            {
                NumRows = _puzzle.Rows,
                NumCollums = _puzzle.Collums
            };

            if (FormProperties.ShowDialog() == DialogResult.OK)
            {
                if (FormProperties.NumRows * FormProperties.NumCollums >= _puzzle.Images.Count())
                {
                    _puzzle.SetNumberRowCollum(FormProperties.NumRows, FormProperties.NumCollums);
                    _puzzle.CreatePictureSegments(this, ClientSize, PB_Click);
                }
            }
        }  
        
        private async void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            await Task.Run(() => HelpPicture());
        }

        private void HelpPicture()
        {
            var imageMask = new List<IImagePositionSearch>();
            foreach (var img in _puzzle.Images)
            {
                imageMask.Add(new ImagePositionSearch(img, Array.IndexOf(_puzzle.Images, img)));
            }

            var FormHelp = new FormHelp(new AutoPuzzle(imageMask, _puzzle.Rows, _puzzle.Collums));
                if (FormHelp.ShowDialog() == DialogResult.OK) { }            
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            _puzzle.CorrectSizeSegments(ClientSize);
        }
            
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _puzzle.CorrectSizeSegments(ClientSize);
        }
    }
}
