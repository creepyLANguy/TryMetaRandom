namespace TryMetaRandom
{
  internal struct Settings
  {
    public int Width;
    public int Height;
    public int Seed;
    public int Depth;
    public float MetaStrength;
    public bool Rgb;
    public ScalingType Scaling;

    public Settings(int width, int height, int seed, int depth, float metaStrength, bool rgb, ScalingType scaling)
    {
      Width = width;
      Height = height;
      Seed = seed;
      Depth = depth;
      MetaStrength = metaStrength;
      Rgb = rgb;
      Scaling = scaling;
    }
  }
}