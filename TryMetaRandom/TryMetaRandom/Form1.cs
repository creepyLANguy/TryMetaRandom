using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TryMetaRandom
{
  enum ColourMode
  {
    RGB,
    PALETTE,
    GREYSCALE,
    BW
  }

  public partial class Form1 : Form
  {
    private static readonly List<Color> Palette = new List<Color>()
    {
      //Color.Red,
      //Color.Green,
      //Color.Blue,
      //Color.Black,
      //Color.Purple,
      Color.MediumPurple,
      Color.CornflowerBlue,
      Color.AliceBlue,
      Color.Pink
    };

    private static readonly Settings Config = new Settings(200, 200, 11, 7, 0.5f, ColourMode.PALETTE, ScalingType.Accurate);
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, ColourMode.RGB, ScalingType.Accurate);
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, ColourMode.RGB, ScalingType.Fast); 
    //private static readonly Settings Config = new Settings(200, 200, 11, 5, 0.5f, ColourMode.BW , ScalingType.Accurate);

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
          switch (Config.ColourMode)
          {
            case ColourMode.RGB:
            {
              var colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
              bmp.SetPixel(col, row, colour);
              break;
            }
            case ColourMode.PALETTE:
            {
              var colour = Palette[Rand.Next(Palette.Count)];
              bmp.SetPixel(col, row, colour);
              break;
            }
            case ColourMode.GREYSCALE:
            {
              var c = Rand.Next(256);
              var colour = Color.FromArgb(c, c, c);
              bmp.SetPixel(col, row, colour);
              break;
            }
            case ColourMode.BW:
            {
              var c = Rand.Next(2) == 0 ? 0 : 255;
              var colour = Color.FromArgb(c, c, c);
              bmp.SetPixel(col, row, colour);
              break;
            }
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
          switch (Config.ColourMode)
          {
            case ColourMode.RGB:
            { 
              colour = Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256));
              break;
            }
            case ColourMode.PALETTE:
            {
              colour = Palette[Rand.Next(Palette.Count)];
              bmp.SetPixel(col, row, colour);
              break;
            }
            case ColourMode.GREYSCALE:
            {
              var c = Rand.Next(256);
              colour = Color.FromArgb(c, c, c);
              break;
            }
            case ColourMode.BW:
            {
              var c = Rand.Next(2) == 0 ? 0 : 255;
              colour = Color.FromArgb(c, c, c);
              break;
            }
            default:
              throw new NotImplementedException();
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
