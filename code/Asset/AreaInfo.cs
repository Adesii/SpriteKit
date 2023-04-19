using System.Collections.Generic;
using Sandbox;

namespace SpriteKit.Asset;

public class AreaInfo
{
	public string Name { get; set; }
	public Rect Area { get; set; }
	public float XSubdivisions { get; set; }
	public float YSubdivisions { get; set; }
	[HideInEditor]
	public Color AreaColor { get; set; } = Color.Random;
	public float FrameRate { get; set; } = 12f;

	[ResourceType( "png" )]
	public string SpriteSheetPath { get; set; }
	[ResourceType( "png" )]
	public string SpriteSheetAlphaPath { get; set; }

	[ResourceType( "png" )]
	public string SpriteSheetNormalPath { get; set; }

	public static Dictionary<string, Texture> TextureList = new();
	[HideInEditor]
	public Texture SpriteSheetTexture
	{
		get
		{
			//if ( !string.IsNullOrEmpty( SpriteSheetPath ) && !TextureList.ContainsKey( SpriteSheetPath ) )
			LoadTextures();
			if ( TextureList.TryGetValue( SpriteSheetPath, out Texture val ) )
			{
				return val;
			}
			return null;
		}
	}
	[HideInEditor]
	public Texture SpriteSheetNormalTexture
	{
		get
		{
			//if ( !string.IsNullOrEmpty( SpriteSheetNormalPath ) && !TextureList.ContainsKey( SpriteSheetNormalPath ) )
			LoadTextures();
			if ( TextureList.TryGetValue( SpriteSheetNormalPath, out Texture val ) )
				return val;
			return null;
		}
	}
	[HideInEditor]
	public Texture SpriteSheetAlphaTexture
	{
		get
		{
			//if ( !string.IsNullOrEmpty( SpriteSheetAlphaPath ) && !TextureList.ContainsKey( SpriteSheetAlphaPath ) )
			LoadTextures();
			if ( TextureList.TryGetValue( SpriteSheetAlphaPath, out Texture val ) )
				return val;
			return null;
		}
	}

	public virtual void LoadTextures()
	{
		var textures = new Dictionary<string, Texture>();
		var SpriteSheetTexture = Texture.Load( FileSystem.Mounted, SpriteSheetPath );
		TextureList[SpriteSheetPath] = SpriteSheetTexture;
		if ( !string.IsNullOrEmpty( SpriteSheetAlphaPath ) )
		{
			var SpriteSheetAlphaTexture = Texture.Load( FileSystem.Mounted, SpriteSheetAlphaPath );
			TextureList[SpriteSheetAlphaPath] = SpriteSheetAlphaTexture;
		}

		if ( !string.IsNullOrEmpty( SpriteSheetNormalPath ) )
		{
			var SpriteSheetNormalTexture = Texture.Load( FileSystem.Mounted, SpriteSheetNormalPath, false );
			TextureList[SpriteSheetNormalPath] = SpriteSheetNormalTexture;
		}
	}
}
