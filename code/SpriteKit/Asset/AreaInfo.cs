using System.Collections.Generic;
using Sandbox;

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
	public string SpriteSheetAlphaPath { get; set; }

	[ResourceType( "img" )]
	public string SpriteSheetNormalPath { get; set; }

	[HideInEditor]
	public static Dictionary<string, Texture> TextureList = new();
	public Texture SpriteSheetTexture
	{
		get
		{
			if ( string.IsNullOrEmpty( SpriteSheetPath ) )
				return null;
			return TextureList[SpriteSheetPath];
		}
	}
	public Texture SpriteSheetNormalTexture
	{
		get
		{
			if ( string.IsNullOrEmpty( SpriteSheetNormalPath ) )
				return null;
			return TextureList[SpriteSheetNormalPath];
		}
	}
	public Texture SpriteSheetAlphaTexture
	{
		get
		{
			if ( string.IsNullOrEmpty( SpriteSheetAlphaPath ) )
				return null;
			return TextureList[SpriteSheetAlphaPath];
		}
	}

	public virtual void LoadTextures()
	{
		var textures = new Dictionary<string, Texture>();
		var SpriteSheetTexture = Sandbox.TextureLoader.Image.Load( FileSystem.Mounted, SpriteSheetPath, true );
		if ( !TextureList.ContainsKey( SpriteSheetPath ) )
			TextureList.Add( SpriteSheetPath, SpriteSheetTexture );
		if ( !string.IsNullOrEmpty( SpriteSheetAlphaPath ) )
		{
			var SpriteSheetAlphaTexture = Sandbox.TextureLoader.Image.Load( FileSystem.Mounted, SpriteSheetAlphaPath, true );
			if ( !TextureList.ContainsKey( SpriteSheetAlphaPath ) )
				TextureList.Add( SpriteSheetAlphaPath, SpriteSheetAlphaTexture );
		}

		if ( !string.IsNullOrEmpty( SpriteSheetNormalPath ) )
		{
			var SpriteSheetNormalTexture = Sandbox.TextureLoader.Image.Load( FileSystem.Mounted, SpriteSheetNormalPath, false );
			if ( !TextureList.ContainsKey( SpriteSheetNormalPath ) )
				TextureList.Add( SpriteSheetNormalPath, SpriteSheetNormalTexture );
		}
	}
}
