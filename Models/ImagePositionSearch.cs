using Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    public class ImagePositionSearch: IImagePositionSearch
    {
        private int _width;
        private int _height;
        private int _id;       
        private Image _image;
        private List<Image> _imgSide = new List<Image>(4);
        public bool KnownPosition { get; set; }        
        public int Width { get => _width; }
        public int Height { get => _height; }
        public int Id { get => _id; }
        public Image Image { get => _image;}
        public List<Image> ImgSide { get => _imgSide;}

        public ImagePositionSearch() { }

        public ImagePositionSearch(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public ImagePositionSearch(Image img, int id)
        {
            KnownPosition = false;
            _image = img;
            _id = id;
            for (int i = 1; i < 5; i++)
                ImgSide.Add(GetSideImage(i));
        }

        public void SetXY(int width, int height)
        {
            _width = width;
            _height = height;
            KnownPosition = true;
        }

        private Image GetSideImage(int position)
        {
            var input = new Bitmap(Image);
            var pixel = new UInt32();
            Bitmap output;          
          
            if (position == 1 || position == 3)
            {
                output = new Bitmap(1, Image.Height);
            }
            else output = new Bitmap(Image.Width, 1);
           
            for (int i = 0; i < output.Width; i++)
            {
                for (int j = 0; j < output.Height; j++)
                {
                    switch (position)
                    {
                        case 1:
                            pixel = (UInt32)(input.GetPixel(0, j).ToArgb());
                            break;
                        case 2:
                            pixel = (UInt32)(input.GetPixel(i, 0).ToArgb());
                            break;
                        case 3:
                            pixel = (UInt32)(input.GetPixel(input.Width - 1, j).ToArgb());
                            break;
                        case 4:
                            pixel = (UInt32)(input.GetPixel(i, input.Height - 1).ToArgb());
                            break;
                    }
                    output.SetPixel(i, j, Color.FromArgb((int)pixel));
                }
            }
            return output;
        }
    }
}
