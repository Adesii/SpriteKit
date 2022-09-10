using System.Collections.Generic;
using Sandbox;

namespace SpriteKit.Asset;

public class AreaAsset<T> : GameResource where T : AreaInfo
{
	public static Dictionary<string, GameResource> All = new();
	public List<T> SpriteAreas { get; set; } = new();
	[HideInEditor]
	public Dictionary<string, T> SpriteAreasByName = new();


	protected override void PostLoad()
	{
		base.PostLoad();
		Log.Info( $"Loading sprite asset {ResourceName}" );


		foreach ( var area in SpriteAreas )
		{
			area.LoadTextures();
			SpriteAreasByName[area.Name.ToLower()] = area;
		}

		All[ResourcePath.ToLower().NormalizeFilename()] = this;
	}
	protected override void PostReload()
	{
		base.PostReload();
		Log.Info( $"Reloading sprite asset {ResourceName}" );
		foreach ( var area in SpriteAreas )
		{
			area.LoadTextures();
			SpriteAreasByName[area.Name.ToLower()] = area;
		}
		Event.Run( "spriteassets_changed", ResourceId );
	}
}
