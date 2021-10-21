using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle.Interfaces
{
    public interface IPuzzle
    {
        Image[] Images { get; set; }      
        PictureBox[] Segments { get; }
        int Rows { get; }
        int Collums { get; }
        void SetNumberRowCollum(int row, int collum);

        void CorrectSizeSegments(Size clientSize);

        void CreatePictureSegments(Control control, Size clientSize, Action<object, EventArgs> pB_Click);

        void RefreshPicture();

        void TranslationSegment(PictureBox segment);
    }
}

