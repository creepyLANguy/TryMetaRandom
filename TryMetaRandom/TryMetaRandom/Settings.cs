namespace TryMetaRandom
{
  internal struct Settings
  {
    public int Width;
    public int Height;
    public int Seed;
    public int Depth;
    public float MetaStrength;
    public ColourMode ColourMode;
    public ScalingType Scaling;

    public Settings(int width, int height, int seed, int depth, float metaStrength, ColourMode colourMode, ScalingType scaling)
    {
      Width = width;
      Height = height;
      Seed = seed;
      Depth = depth;
      MetaStrength = metaStrength;
      ColourMode = colourMode;
      Scaling = scaling;
    }
  }
}