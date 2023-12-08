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

    private static readonly NearestNeighbourPictureBox PictureBox = new NearestNeighbourPictureBox();
    private static readonly List<Bitmap> Noises = new List<Bitmap>();
    private static readonly List<Bitmap> Blends = new List<Bitmap>();
    private static readonly Random Rand = new Random(Config.Seed);
    private static int _currentIndex;
    private static bool _viewingBlend = true;

    public Form1()
    {
      InitializeComponent();

      GenerateNoises();

      GenerateBlends();

      PrepareUI();
    }

    private static void GenerateNoises()
    {
      for (var i = 0; i < Config.Depth; ++i)
      {
        switch (Config.Scaling)
        {
          case ScalingType.None:
          case ScalingType.Accurate:
            Noises.Add(GenerateNoise_NoDownScale(i));
            break;
          case ScalingType.Fast:
            Noises.Add(GenerateNoise_Fast(i));
            break;
        }
      }
    }

    private static Bitmap GenerateNoise_Fast(int scaleFactor)
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

    private static void GenerateBlends()
    {
      Blends.Add(Noises[0]);
      for (var i = 1; i <= Noises.Count; ++i)
      {
        Blends.Add(GenerateBlend(i));
      }
      Blends.RemoveAt(0);
    }

    private static Bitmap GenerateBlend(int depth)
    {
      var baseImage = new Bitmap(Blends[depth-1]);

      using (var g = Graphics.FromImage(baseImage))
      {
        var colorMatrix = new ColorMatrix { Matrix33 = Config.MetaStrength };
        var imageAttributes = new ImageAttributes();
        imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        for (var i = 1; i < depth; ++i)
        {
          var overlayImage = Noises[i];

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

    private void PrepareUI()
    {
      btn_inspect.Enabled = false;
      ReplacePictureBoxWithCustomComponent();
      UpdatePictureBox();
    }

    private void ReplacePictureBoxWithCustomComponent()
    {
      PictureBox.Location = pictureBox1.Location;
      PictureBox.Size = pictureBox1.Size;
      PictureBox.Anchor = pictureBox1.Anchor;
      PictureBox.SizeMode = pictureBox1.SizeMode;
      Controls.Add(PictureBox);
      Controls.Remove(pictureBox1);
    }

    private void UpdatePictureBox()
    {
      PictureBox.Image = _viewingBlend ? Blends[_currentIndex] : Noises[_currentIndex];
      //btn_previous.Enabled = _currentIndex > 0;
      //btn_next.Enabled = _currentIndex < Bitmaps.Count - 1;
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
  }
}
