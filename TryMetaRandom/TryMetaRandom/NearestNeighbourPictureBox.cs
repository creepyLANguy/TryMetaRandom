using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TryMetaRandom
{
  internal class NearestNeighbourPictureBox : PictureBox
  {
    protected override void OnPaint(PaintEventArgs e)
    {
      if (Image == null)
      {
        return;
      }

      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      var destRect = new Rectangle(0, 0, Width, Height);
      e.Graphics.DrawImage(Image, destRect);
    }
  }
}