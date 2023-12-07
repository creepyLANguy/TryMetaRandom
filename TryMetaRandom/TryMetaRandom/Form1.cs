using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TryMetaRandom
{
  public partial class Form1 : Form
  {
    private static readonly Settings Config = new Settings(200, 200, 111, 5, 0.5f, false, true);
    //private static Settings Config = new Settings(200, 200, 111, 5, 0.5f, true , true);

    private static readonly NearestNeighborPictureBox NearestNeighborPictureBox1 = new NearestNeighborPictureBox();
    private static readonly List<Bitmap> Bitmaps = new List<Bitmap>();
    private static readonly List<Bitmap> Blends = new List<Bitmap>();
    private static int _currentIndex;
    private static readonly Random Rand = new Random(Config.Seed);
    private static bool ViewingBlend = true;

    public Form1()
    {
      InitializeComponent();

      ReplacePictureBoxWithSpecialOne();

      for (var i = 0; i < Config.Depth; ++i)
      {
        Bitmaps.Add(Config.Downscale ? GetRandomBmp(i) : GetRandomBmp_NoDownScale(i));
      }

      Blends.Add(Bitmaps[0]);
      for (var i = 1; i <= Bitmaps.Count; ++i)
      {
        Blends.Add(GetMetaImage(i));
      }
      Blends.RemoveAt(0);

      _currentIndex = 0;
      btn_inspect.Enabled = false;
      UpdatePictureBox();
    }

    private static Bitmap GetRandomBmp(int scaleFactor)
    {
      ++scaleFactor;
      scaleFactor *= scaleFactor;

      var bmp = new Bitmap(Config.Width / scaleFactor, Config.Height /scaleFactor);

      for (var row = 0; row < bmp.Height; ++row)
      {
        for (var col = 0; col < bmp.Width; ++col)
        {
          if (Config.Greyscale)
          {
            var c = Rand.Next(256);
            var colour = Color.FromArgb(c, c, c);
            bmp.SetPixel(col, row, colour);
          }
          else
          {
            var colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
            bmp.SetPixel(col, row, colour);
          }
        }
      }

      return bmp;
    }

    private static Bitmap GetRandomBmp_NoDownScale(int scaleFactor)
    {
      ++scaleFactor;
      scaleFactor *= scaleFactor;

      var bmp = new Bitmap(Config.Width, Config.Height);

      for (var row = 0; row < bmp.Height; row += scaleFactor)
      {
        for (var col = 0; col < bmp.Width; col += scaleFactor)
        {
          Color colour;
          if (Config.Greyscale)
          {
            var c = Rand.Next(256);
            colour = Color.FromArgb(c, c, c);
          }
          else
          {
            colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
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

    private void UpdatePictureBox()
    {
      NearestNeighborPictureBox1.Image = ViewingBlend ? Blends[_currentIndex] : Bitmaps[_currentIndex];
      //btn_previous.Enabled = currentIndex > 0;
      //btn_next.Enabled = currentIndex < bitmaps.Count - 1;
    }

    private void ReplacePictureBoxWithSpecialOne()
    {
      NearestNeighborPictureBox1.Location = pictureBox1.Location;
      NearestNeighborPictureBox1.Size = pictureBox1.Size;
      NearestNeighborPictureBox1.Anchor = pictureBox1.Anchor;
      NearestNeighborPictureBox1.SizeMode = pictureBox1.SizeMode;
      NearestNeighborPictureBox1.BackColor = Color.Black;
      Controls.Add(NearestNeighborPictureBox1);
      Controls.Remove(pictureBox1);
    }

    internal class NearestNeighborPictureBox : PictureBox
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

    internal struct Settings
    {
      public int Width;
      public int Height;
      public int Seed;
      public int Depth;
      public float MetaStrength;
      public bool Greyscale;
      public bool Downscale;

      public Settings(int width, int height, int seed, int depth, float metaStrength, bool greyscale, bool downscale)
      {
        Width = width;
        Height = height;
        Seed = seed;
        Depth = depth;
        MetaStrength = metaStrength;
        Greyscale = greyscale;
        Downscale = downscale;
      }
    }

    private void btn_inspect_Click(object sender, EventArgs e)
    {
      ViewingBlend = !ViewingBlend;
      UpdatePictureBox();
    }
  }
}
