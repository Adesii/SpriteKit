using System;
using System.Text.Json.Serialization;
using Sandbox;

namespace SpriteKit.Asset;

[Library( "sprite" )]
[GameResource( "Sprite Asset", "sprite", "" )]
public class SpriteAsset : AreaAsset<SpriteArea>
{
	public static T Get<T>( string FilePath ) where T : GameResource
	{
		return All[FilePath.NormalizeFilename().ToLower()] as T;

	}
}
