using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TryMetaRandom
{
  public partial class Form1 : Form
  {

    private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, true, ScalingType.Accurate);
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, true, ScalingType.Fast); 
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, true, ScalingType.None);
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, false , ScalingType.Accurate);

    private static readonly NearestNeighbourPictureBox NearestNeighbourPictureBox1 = new NearestNeighbourPictureBox();
    private static readonly List<Bitmap> Bitmaps = new List<Bitmap>();
    private static readonly List<Bitmap> Blends = new List<Bitmap>();
    private static readonly Random Rand = new Random(Config.Seed);
    private static int _currentIndex;
    private static bool _viewingBlend = true;

    public Form1()
    {
      InitializeComponent();

      ReplacePictureBoxWithNearestNeighborPictureBox();

      for (var i = 0; i < Config.Depth; ++i)
      {
        switch (Config.Scaling)
        {
          case ScalingType.None:
          case ScalingType.Accurate:
            Bitmaps.Add(GenerateNoise_NoDownScale(i));
            break;
          case ScalingType.Fast:
            Bitmaps.Add(GenerateNoise(i));
            break;
        }
      }

      Blends.Add(Bitmaps[0]);
      for (var i = 1; i <= Bitmaps.Count; ++i)
      {
        Blends.Add(GetMetaImage(i));
      }
      Blends.RemoveAt(0);

      btn_inspect.Enabled = false;
      UpdatePictureBox();
    }

    private static Bitmap GenerateNoise(int scaleFactor)
    {
      ++scaleFactor;
      scaleFactor *= scaleFactor;

      var bmp = new Bitmap(Config.Width / scaleFactor, Config.Height /scaleFactor);

      for (var row = 0; row < bmp.Height; ++row)
      {
        for (var col = 0; col < bmp.Width; ++col)
        {
          if (Config.Rgb)
          {
            var colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
            bmp.SetPixel(col, row, colour);
            
          }
          else
          {
            var c = Rand.Next(256);
            var colour = Color.FromArgb(c, c, c);
            bmp.SetPixel(col, row, colour);
          }
        }
      }

      return bmp;
    }

    private static Bitmap GenerateNoise_NoDownScale(int scaleFactor)
    {
      ++scaleFactor;
      scaleFactor *= scaleFactor;

      var bmp = new Bitmap(Config.Width, Config.Height);

      for (var row = 0; row < bmp.Height; row += scaleFactor)
      {
        for (var col = 0; col < bmp.Width; col += scaleFactor)
        {
          Color colour;
          if (Config.Rgb)
          {
            colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
          }
          else
          {
            var c = Rand.Next(256);
            colour = Color.FromArgb(c, c, c);
          }

          for (var x = col; x < col + scaleFactor && x < bmp.Height; ++x)
          {
            for (var y = row; y < row + scaleFactor && y < bmp.Width; ++y)
            {
              bmp.SetPixel(x, y, colour);
            }
          }
        }
      }

      if (Config.Scaling == ScalingType.Accurate)
      {
        var blurAmount = scaleFactor / Math.Sqrt(Config.Depth);
        return GaussianBlur.Blur(bmp, (int)blurAmount);
      }

      return bmp;
    }

    private static Bitmap GetMetaImage(int depth)
    {
      var baseImage = new Bitmap(Blends[depth-1]);

      using (var g = Graphics.FromImage(baseImage))
      {
        var colorMatrix = new ColorMatrix { Matrix33 = Config.MetaStrength };
        var imageAttributes = new ImageAttributes();
        imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        for (var i = 1; i < depth; ++i)
        {
          var overlayImage = Bitmaps[i];

          g.DrawImage(
            overlayImage,
            new Rectangle(0, 0, baseImage.Width, baseImage.Height),
            0,
            0,
            overlayImage.Width,
            overlayImage.Height,
            GraphicsUnit.Pixel,
            imageAttributes);
        }
      }

      return baseImage;
    }

    private void btn_previous_Click(object sender, EventArgs e)
    {
      _currentIndex = _currentIndex > 0 ? _currentIndex - 1 : Blends.Count - 1;
      btn_inspect.Enabled = _currentIndex > 0;
      UpdatePictureBox();
    }

    private void btn_next_Click(object sender, EventArgs e)
    {
      _currentIndex = _currentIndex < Blends.Count - 1 ? _currentIndex + 1 : 0;
      btn_inspect.Enabled = _currentIndex > 0;
      UpdatePictureBox();
    }

    private void btn_inspect_Click(object sender, EventArgs e)
    {
      _viewingBlend = !_viewingBlend;
      UpdatePictureBox();
    }

    private void UpdatePictureBox()
    {
      NearestNeighbourPictureBox1.Image = _viewingBlend ? Blends[_currentIndex] : Bitmaps[_currentIndex];
      //btn_previous.Enabled = _currentIndex > 0;
      //btn_next.Enabled = _currentIndex < Bitmaps.Count - 1;
    }

    private void ReplacePictureBoxWithNearestNeighborPictureBox()
    {
      NearestNeighbourPictureBox1.Location = pictureBox1.Location;
      NearestNeighbourPictureBox1.Size = pictureBox1.Size;
      NearestNeighbourPictureBox1.Anchor = pictureBox1.Anchor;
      NearestNeighbourPictureBox1.SizeMode = pictureBox1.SizeMode;
      NearestNeighbourPictureBox1.BackColor = Color.Black;
      Controls.Add(NearestNeighbourPictureBox1);
      Controls.Remove(pictureBox1);
    }
  }
}
