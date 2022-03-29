using System.Collections.Generic;

namespace SpriteKit.Asset;

public class AreaInfo
{
	public string Name { get; set; }
	public Rect Area { get; set; }
	public float XSubdivisions { get; set; }
	public float YSubdivisions { get; set; }
	public Color AreaColor { get; set; } = Color.Random;
	public float FrameRate { get; set; } = 12f;

	[ResourceType( "img" )]
	public string SpriteSheetPath { get; set; }

	[ResourceType( "img" )]
	public string SpriteSheetNormalPath { get; set; }

	[Skip]
	public static Dictionary<string, Texture> TextureList = new();
	public Texture SpriteSheetTexture => TextureList[SpriteSheetPath];
	public Texture SpriteSheetNormalTexture => TextureList[SpriteSheetNormalPath];

	public virtual void LoadTextures()
	{
		var textures = new Dictionary<string, Texture>();
		var SpriteSheetTexture = Sandbox.TextureLoader.Image.Load( FileSystem.Mounted, SpriteSheetPath, true );
		if ( !TextureList.ContainsKey( SpriteSheetPath ) )
			TextureList.Add( SpriteSheetPath, SpriteSheetTexture );
		if ( !string.IsNullOrEmpty( SpriteSheetNormalPath ) )
		{
			var SpriteSheetNormalTexture = Sandbox.TextureLoader.Image.Load( FileSystem.Mounted, SpriteSheetNormalPath, false );
			if ( !TextureList.ContainsKey( SpriteSheetNormalPath ) )
				TextureList.Add( SpriteSheetNormalPath, SpriteSheetNormalTexture );
		}
	}
}
