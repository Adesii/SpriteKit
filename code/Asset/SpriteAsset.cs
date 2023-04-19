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
		if ( All.TryGetValue( FilePath, out var res ) )
		{
			return (T)res;
		}
		if ( Sandbox.Internal.GlobalGameNamespace.ResourceLibrary.TryGet<T>( FilePath, out var res2 ) )
		{
			return res2;
		}

		return null;

	}
}
