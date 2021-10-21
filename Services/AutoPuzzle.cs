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
    public class AutoPuzzle: IAutoPuzzle
    {
        private PictureBox[] segments;
        private int numRows;
        private int numCollums;
        private List<IImagePositionSearch> imageMask = new List<IImagePositionSearch>();

        public PictureBox[] Segments { get => segments; }
        public int NumRows { get => numRows; }
        public int NumCollums { get => numCollums; }
        public AutoPuzzle() { }       
        public AutoPuzzle(List<IImagePositionSearch> imageMask, int numRows, int numCollums)
        {
            this.numRows = numRows;
            this.numCollums = numCollums;
            this.imageMask = imageMask;
        }

        public void StartCreatePuzzle()
        {
            int imageId = 0;
            double serchAccuracy;
            double accuracy;
            Image imgBase, imgPrev;
            var serchAccuracyMin = new List<double>(2);
            var serchAccuracyAverage = new List<double>(2) { 0, 0 };

            imageMask[imageId].SetXY(0, 0);
            imgBase = imageMask[imageId].ImgSide[0];
            serchAccuracyMin.Add(LookPixel(imgBase, imageMask[0].ImgSide[2]));

            foreach (var img in imageMask)
            {
                imgPrev = img.ImgSide[2];
                accuracy = LookPixel(imgBase, imgPrev);
                serchAccuracyAverage[0] += accuracy;

                if (accuracy < serchAccuracyMin[0])
                    serchAccuracyMin[0] = accuracy;
            }

            imgBase = imageMask[imageId].ImgSide[2];
            serchAccuracyMin.Add(LookPixel(imgBase, imageMask[0].ImgSide[0]));

            foreach (var img in imageMask)
            {
                imgPrev = img.ImgSide[0];
                accuracy = LookPixel(imgBase, imgPrev);
                serchAccuracyAverage[1] += accuracy;

                if (accuracy < serchAccuracyMin[1])
                    serchAccuracyMin[1] = accuracy;
            }

            if (serchAccuracyMin[1] < serchAccuracyMin[0])
            {
                serchAccuracy = serchAccuracyAverage[1] / 4 / imageMask.Count();
            }
            else
            {
                serchAccuracy = serchAccuracyAverage[0] / 4 / imageMask.Count();
            }

            BaseSerch(imageId, serchAccuracy, new ImagePositionSearch(0, 0));

            var lostImage = imageMask.Where(a => a.KnownPosition == false);
            while (lostImage.Count() > 0)
            {
                LostImageSearch(lostImage.Last().Id);
            }
        }

        public void CreatePictureSegments(Control control,Size clientSize)
        {
            if (segments != null)
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    segments[i].Dispose();
                }
                segments = null;
            }
            var minW = imageMask.Min(a => a.Width);
            var minH = imageMask.Min(a => a.Height);
            var maxW = imageMask.Max(a => a.Width);
            var maxH = imageMask.Max(a => a.Height);

            numCollums = 0;
            for (int i = minW; i <= maxW; i++)
                numCollums++;

            numRows = 0;
            for (int i = minH; i <= maxH; i++)
                numRows++;

            segments = new PictureBox[numRows * numCollums];
            var segmentIndex = 0;
            var w = clientSize.Width / numRows;
            var h = clientSize.Height / numCollums;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCollums; j++)
                {
                    var imMask = from t in imageMask
                                 where t.Width - minW == j && t.Height - minH == i
                                 select t;

                    segments[segmentIndex] = new PictureBox
                    {
                        Width = w,
                        Height = h,
                        Left = i * w,
                        Top = j * h
                    };

                    if (imMask.Count() > 0)
                    {
                        segments[segmentIndex].Image = imMask.First().Image;
                    }
                    else
                    {
                        segments[segmentIndex].BackColor = Color.Black;
                    }

                    segments[segmentIndex].Parent = control;
                    segments[segmentIndex].BorderStyle = BorderStyle.None;
                    segments[segmentIndex].SizeMode = PictureBoxSizeMode.StretchImage;

                    segments[segmentIndex].Show();
                    segmentIndex++;
                }
            }
        }

        private Double LookPixel(Image imgBase, Image imgPrev)
        {
            Double change = 0;
            var bImage = new Bitmap(imgBase);
            var pImage = new Bitmap(imgPrev);
            for (int i = 0; i < bImage.Width; i++)
            {
                for (int j = 0; j < bImage.Height; j++)
                {
                    UInt32 pixel1 = (UInt32)(bImage.GetPixel(i, j).ToArgb());
                    UInt32 pixel2 = (UInt32)(pImage.GetPixel(i, j).ToArgb());

                    float R1 = (float)((pixel1 & 0x00FF0000) >> 16);
                    float G1 = (float)((pixel1 & 0x0000FF00) >> 8);
                    float B1 = (float)(pixel1 & 0x000000FF);

                    float R2 = (float)((pixel2 & 0x00FF0000) >> 16);
                    float G2 = (float)((pixel2 & 0x0000FF00) >> 8);
                    float B2 = (float)(pixel2 & 0x000000FF);
                    change += Math.Abs(R1 - R2) + Math.Abs(G1 - G2) + Math.Abs(B1 - B2);
                }
            }
            change = change / (bImage.Height + bImage.Width);
            return change;
        }

        private void LostImageSearch(int id)
        {
            var correntAccuracy = new List<double>(5) { 9000, 9000, 9000, 9000, 0 };
            var correntAccuracyId = new List<int>(4) { -1, -1, -1, -1 };
            foreach (var p in imageMask.Where(a => a.KnownPosition == true))
            {
                correntAccuracy[4] = LookPixel(imageMask[id].ImgSide[0], p.ImgSide[2]);
                if (correntAccuracy[4] < correntAccuracy[0])
                {
                    correntAccuracy[0] = correntAccuracy[4];
                    correntAccuracyId[0] = p.Id;
                }

                correntAccuracy[4] = LookPixel(imageMask[id].ImgSide[1], p.ImgSide[3]);
                if (correntAccuracy[4] < correntAccuracy[1])
                {
                    correntAccuracy[1] = correntAccuracy[4];
                    correntAccuracyId[1] = p.Id;
                }

                correntAccuracy[4] = LookPixel(imageMask[id].ImgSide[2], p.ImgSide[0]);
                if (correntAccuracy[4] < correntAccuracy[2])
                {
                    correntAccuracy[2] = correntAccuracy[4];
                    correntAccuracyId[2] = p.Id;
                }

                correntAccuracy[4] = LookPixel(imageMask[id].ImgSide[3], p.ImgSide[1]);
                if (correntAccuracy[4] < correntAccuracy[3])
                {
                    correntAccuracy[3] = correntAccuracy[4];
                    correntAccuracyId[3] = p.Id;
                }
            }
            correntAccuracy.RemoveAt(4);

            if (correntAccuracy.IndexOf(correntAccuracy.Min()) == 0)
                imageMask[id].SetXY(imageMask[correntAccuracyId[0]].Width, imageMask[correntAccuracyId[0]].Height + 1);

            if (correntAccuracy.IndexOf(correntAccuracy.Min()) == 1)
                imageMask[id].SetXY(imageMask[correntAccuracyId[1]].Width + 1, imageMask[correntAccuracyId[1]].Height);

            if (correntAccuracy.IndexOf(correntAccuracy.Min()) == 2)
                imageMask[id].SetXY(imageMask[correntAccuracyId[2]].Width, imageMask[correntAccuracyId[2]].Height - 1);

            if (correntAccuracy.IndexOf(correntAccuracy.Min()) == 3)
                imageMask[id].SetXY(imageMask[correntAccuracyId[3]].Width - 1, imageMask[correntAccuracyId[3]].Height);

            if (imageMask[id].KnownPosition == false)
                imageMask[id].KnownPosition = true;
        }

        private void BaseSerch(int imageId, double serchAccuracy, IImagePositionSearch _imgP)
        {
            imageMask[imageId].SetXY(_imgP.Width, _imgP.Height);

            ImagePositionSearch imgP;
            imgP = new ImagePositionSearch(imageMask[imageId].Width, imageMask[imageId].Height - 1);
            if (!imageMask.Any(_ => _.Width == imgP.Width && _.Height == imgP.Height))
                ChekImageS(imageId, imgP, serchAccuracy, 0, 2);

            imgP = new ImagePositionSearch(imageMask[imageId].Width - 1, imageMask[imageId].Height);
            if (!imageMask.Any(_ => _.Width == imgP.Width && _.Height == imgP.Height))
                ChekImageS(imageId, imgP, serchAccuracy, 1, 3);

            imgP = new ImagePositionSearch(imageMask[imageId].Width, imageMask[imageId].Height + 1);
            if (!imageMask.Any(_ => _.Width == imgP.Width && _.Height == imgP.Height))
                ChekImageS(imageId, imgP, serchAccuracy, 2, 0);

            imgP = new ImagePositionSearch(imageMask[imageId].Width + 1, imageMask[imageId].Height);
            if (!imageMask.Any(_ => _.Width == imgP.Width && _.Height == imgP.Height))
                ChekImageS(imageId, imgP, serchAccuracy, 3, 1);
        }

        private void ChekImageS(int pointBase, IImagePositionSearch imgP, double serchAccuracy, int xb, int xp)
        {
            var minAccuracy = serchAccuracy;
            var correntAccuracy = default(double);
            var pointNext = new int();

            for (int i = 0; i < imageMask.Count(); i++)
            {
                if (imageMask[i].KnownPosition)
                    continue;

                correntAccuracy = LookPixel(imageMask[pointBase].ImgSide[xb], imageMask[i].ImgSide[xp]);
                if (correntAccuracy < minAccuracy)
                {
                    foreach (var d in imageMask.Where(x => x != imageMask[i]))
                    {
                        if (LookPixel(d.ImgSide[xb], imageMask[i].ImgSide[xp]) < correntAccuracy)
                        {
                            correntAccuracy = 0;
                            break;
                        }
                    }
                    if (correntAccuracy != 0)
                    {
                        minAccuracy = correntAccuracy;
                        pointNext = i;
                    }
                }
            }

            if (pointNext != 0)
            {
                BaseSerch(pointNext, serchAccuracy, imgP);
            }
        }
    }
}
