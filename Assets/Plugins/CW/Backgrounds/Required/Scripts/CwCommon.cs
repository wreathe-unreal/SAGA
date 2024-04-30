using UnityEngine;

namespace CW.Backgrounds
{
    /// <summary>This class contains some useful methods used by this asset.</summary>
	internal static class CwCommon
	{
		public const string HelpUrlPrefix = "https://carloswilkes.com/Documentation/Backgrounds#";

		public const string ComponentMenuPrefix = "Space Backgrounds/CW ";

		public const string GameObjectMenuPrefix = "GameObject/CW/Backgrounds/";

		private static Cubemap whiteCubemap;

		public static Cubemap WhiteCubemap
		{
			get
			{
				if (whiteCubemap == null)
				{
					whiteCubemap = new Cubemap(1, TextureFormat.Alpha8, false);

					whiteCubemap.SetPixel(CubemapFace.PositiveX, 0, 0, Color.white);
					whiteCubemap.SetPixel(CubemapFace.NegativeX, 0, 0, Color.white);
					whiteCubemap.SetPixel(CubemapFace.PositiveY, 0, 0, Color.white);
					whiteCubemap.SetPixel(CubemapFace.NegativeY, 0, 0, Color.white);
					whiteCubemap.SetPixel(CubemapFace.PositiveZ, 0, 0, Color.white);
					whiteCubemap.SetPixel(CubemapFace.NegativeZ, 0, 0, Color.white);

					whiteCubemap.Apply();
				}

				return whiteCubemap;
			}
		}
	}
}