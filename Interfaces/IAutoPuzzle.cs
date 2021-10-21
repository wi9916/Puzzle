using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle.Interfaces
{
    public interface IAutoPuzzle
    {
        PictureBox[] Segments { get; }
        int NumRows { get; }
        int NumCollums { get; }

        void StartCreatePuzzle();

        void CreatePictureSegments(Control control, Size clientSize);             
    }
}
