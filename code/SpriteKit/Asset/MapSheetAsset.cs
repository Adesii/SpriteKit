using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sandbox;

namespace SpriteKit.Asset;

[Library( "msprite" )]
[GameResource( "Map Sprite Asset", "msprite", "" )]
public class MapSheetAsset : AreaAsset<MapSheetArea>
{
	public static Dictionary<string, MapSheetArea> BlockList = new();
	public override void PostInGameLoad()
	{
		BlockList ??= new();

		Log.Info( $"Loading Map asset {ResourceName}" );
		foreach ( var area in SpriteAreas )
		{
			//area.LoadTextures();
			BlockList[area.Name.ToLower()] = area;
			Event.Run( "MapArea.Loaded", area );
		}
	}
	public override void PostInGameReload()
	{
		BlockList ??= new();

		Log.Info( $"Reloading Map asset {ResourceName}" );
		foreach ( var area in SpriteAreas )
		{
			//area.LoadTextures();
			BlockList[area.Name.ToLower()] = area;
			Event.Run( "MapArea.Loaded", area );
		}
	}
	public static MapSheetArea GetBlockArea( string name )
	{
		return BlockList.GetValueOrDefault( name.ToLower() );
	}
}
