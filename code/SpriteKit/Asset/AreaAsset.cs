using System.Collections.Generic;
using Sandbox;

namespace SpriteKit.Asset;

public class AreaAsset : GameResource
{
	public static Dictionary<string, GameResource> All = new();

	public virtual void PostInGameLoad()
	{

	}
	public virtual void PostInGameReload()
	{

	}

}

public class AreaAsset<T> : AreaAsset where T : AreaInfo
{
	public List<T> SpriteAreas { get; set; } = new();
	[HideInEditor]
	public Dictionary<string, T> SpriteAreasByName = new();
	protected override void PostLoad()
	{

		All[ResourcePath.ToLower().NormalizeFilename()] = this;
		if ( !Game.InGame ) return;
		PostInGameLoad();
	}

	public override void PostInGameLoad()
	{
		Log.Info( $"Loading sprite asset {ResourceName}" );
		foreach ( var area in SpriteAreas )
		{
			area.LoadTextures();
			SpriteAreasByName[area.Name.ToLower()] = area;
		}
	}
	protected override void PostReload()
	{
		if ( !Game.InGame ) return;
		PostInGameReload();
	}

	public override void PostInGameReload()
	{
		Log.Info( $"Reloading sprite asset {ResourceName}" );
		foreach ( var area in SpriteAreas )
		{
			area.LoadTextures();
			SpriteAreasByName[area.Name.ToLower()] = area;
		}
		Event.Run( "spriteassets_changed", ResourceId );
	}
}
