using Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle
{
    public class Puzzle: IPuzzle
    {
        private PictureBox[] segments;
        private int rows = 5;
        private int collums = 5;
        private PictureBox setSegment;
        private bool choseSegment = false;
        public Image[] Images { get; set; }
        public PictureBox[] Segments { get => segments; }
        public int Rows { get => rows; }
        public int Collums { get => collums; }

        public Puzzle() { }

        public void SetNumberRowCollum(int row, int collum)
        {
            rows = row;
            collums = collum;
        }

        public void CorrectSizeSegments(Size clientSize)
        {
            if (segments == null) return;

            var oldwidth = segments[0].Width;
            var oldheight = segments[0].Height;

            var w = clientSize.Width / collums;
            var h = (clientSize.Height - 25) / rows;

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].Width = w;
                segments[i].Height = h;
                try
                {
                    var countX = segments[i].Left /= oldwidth;
                    var countY = segments[i].Top /= oldheight;
                    segments[i].Left = countX * w;
                    segments[i].Top = countY * h + 25;
                }
                catch (Exception)
                { }
                
            }
        }

        public void CreatePictureSegments(Control control, Size clientSize, Action<object, EventArgs> pB_Click)
        {
            if (Segments != null)
            {
                for (int i = 0; i < Segments.Length; i++)
                {
                    Segments[i].Dispose();
                }
                segments = null;
            }
            if (Images == null) Images = new Image[0];

            while (Images.Count() > rows * collums)
            {
                rows++;
                collums++;
            }
            segments = new PictureBox[collums * rows];

            var w = clientSize.Width / collums;
            var h = (clientSize.Height - 25) / rows;

            var countX = 0;
            var countY = 0;

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = new PictureBox
                {
                    Width = w,
                    Height = h,
                    Left = countX * w,
                    Top = countY * h + 25
                };

                countX++;
                if (countX == collums)
                {
                    countX = 0;
                    countY++;
                }

                try
                {
                    segments[i].Image = Images[i];
                }
                catch (Exception)
                {
                    segments[i].BackColor = Color.Black;
                }
                segments[i].Parent = control;
                segments[i].BorderStyle = BorderStyle.None;
                segments[i].SizeMode = PictureBoxSizeMode.StretchImage;
                segments[i].Show();
                segments[i].Click += new EventHandler(pB_Click);
            }

            RefreshPicture();
        }

        public void RefreshPicture()
        {
            Random rand = new Random(Environment.TickCount);

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].Visible = true;
                int temp = rand.Next(0, segments.Length);
                Point ptR = segments[temp].Location;
                Point ptI = segments[i].Location;
                segments[i].Location = ptR;
                segments[temp].Location = ptI;
            }
        }

        public void TranslationSegment(PictureBox segment)
        {
            if (choseSegment)
            {
                if (setSegment.Location == segment.Location)
                {
                    if (segment.Image != null)
                    {
                        segment.Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }
                }
                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i].Location == setSegment.Location)
                    {
                        Point ptemp = Segments[i].Location;
                        segments[i].Location = segment.Location;
                        segment.Location = ptemp;
                        choseSegment = false;
                        segments[i].BorderStyle = BorderStyle.None;
                        break;
                    }
                }
            }
            else
            {
                setSegment = segment;
                choseSegment = true;
                segment.BorderStyle = BorderStyle.Fixed3D;
            }
        }
    }
}
