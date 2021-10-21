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
    public partial class FormImage : Form
    {
        public int NumRows;
        public int NumCollums;
        private Bitmap Picture;
        private string filePath = @"D:\puzzle";
        public FormImage()
        {
            InitializeComponent();
        }

        private void buttonSavePatch_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fileDear = new FolderBrowserDialog();
            if (fileDear.ShowDialog() == DialogResult.OK)
            {                
                filePath = fileDear.SelectedPath;
                SavePatch.Text = filePath;
            }
        }

        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            LoadPicture();
        }
        private void LoadPicture()
        {
            var ofDlg = new OpenFileDialog();

            ofDlg.Filter = "файлы картинок (*.bmp;*.jpg;*.jpeg;*.gif;)|";
            ofDlg.Filter += "*.bmp;*.jpg;*.jpeg;*.gif|All files (*.*)|*.*";
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;

            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                Picture = new Bitmap(ofDlg.FileName);
                pictureBox1.Image = Picture;               
            }
        }       

        private void FormImage_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = NumCollums;
            numericUpDown2.Value = NumRows;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NumCollums = Convert.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NumRows = Convert.ToInt32(numericUpDown2.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {           
            if(Picture != null)  CreatePictureSegments();
        }
        public void CreatePictureSegments()
        {
            var width = (int)((double)Picture.Width / NumCollums + 0.5);
            var height = (int)((double)Picture.Height / NumRows + 0.5);
            var bmps = new Bitmap[NumRows, NumCollums];

            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
            Directory.CreateDirectory(filePath);

            Random rand = new Random(Environment.TickCount);
            int temp = rand.Next(0, 1000000);

            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCollums; j++)
                {
                    bmps[i, j] = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(bmps[i, j]);
                    while (File.Exists(filePath + @"\" + temp.ToString() + ".jpg"))
                    {
                        temp = rand.Next(0, 1000000);
                    }
                    g.DrawImage(Picture, new Rectangle(0, 0, width, height)
                        , new Rectangle(j * width, i * height, width, height)
                        , GraphicsUnit.Pixel);
                    g.Dispose();
                    bmps[i, j].Save(filePath + @"\" + temp.ToString() + ".jpg");
                    bmps[i, j].Dispose();
                }
            }
        }
    }
}
