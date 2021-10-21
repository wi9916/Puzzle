using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Interfaces
{
    public interface IImagePositionSearch
    {
        bool KnownPosition { get; set; }
        int Width { get; }
        int Height { get; }
        int Id { get; }
        Image Image { get; }
        List<Image> ImgSide { get; }
        void SetXY(int width, int height);
    }
}

