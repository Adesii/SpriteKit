using System.Linq;
using Sandbox;
using SpriteKit.Asset;
using SpriteKit.SceneObjects;

namespace SpriteKit.Entities;

[Library( "prop_sprite" )]
public partial class ModelSprite : Entity
{
	[Property, Net]
	public SpriteAsset SpriteAsset { get; set; }

	private SpriteSceneObject SpriteSceneObject;
	public SpriteSceneObject SceneObject => SpriteSceneObject;

	public SceneWorld TargetSceneWorld { get; set; }
	public enum TrackingMode
	{
		Billboard,
		RotatingBillboard,
		WorldSpace,
		VerticalBillboard,
	}
	public enum FacingDirection
	{
		Left,
		Right
	}


	public TrackingMode Tracking { get; set; } = TrackingMode.Billboard;

	[Net] public bool OneShotAnimation { get; set; } = false;
	[Net, Change] protected FacingDirection LookDirection { get; set; } = FacingDirection.Right;

	[Net, Change] public Color TintColor { get; set; } = Color.Transparent;
	[Net, Change] public float TintAmount { get; set; } = 0;

	public string CurrentAnimation { get; set; }
	private SpriteArea _activeSpriteArea { get; set; }
	public SpriteArea ActiveSpriteArea
	{
		get
		{
			if ( _activeSpriteArea == null && SpriteAsset != null )
			{
				_activeSpriteArea = SpriteAsset.SpriteAreas[0];
			}
			return _activeSpriteArea;
		}
		set
		{
			_activeSpriteArea = value;
			if ( SpriteSceneObject != null && SpriteSceneObject.IsValid() )
			{
				SpriteSceneObject.Frame = 0;
			}
		}
	}


	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
		/* if ( !string.IsNullOrEmpty( SpritePath ) )
		{
			SpriteAsset = SpriteAsset.Get<SpriteAsset>( SpritePath.NormalizeFilename() );
		} */
	}

	public void ReloadSprite()
	{
		if ( Game.IsClient ) return;
		/* if ( !string.IsNullOrEmpty( SpritePath ) )
		{
			SpriteAsset = SpriteAsset.Get<SpriteAsset>( SpritePath.NormalizeFilename() );
		} */
	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		SpriteChange();
	}
	protected void SpriteChange()
	{
		if ( SpriteAsset == null ) return;
		if ( SpriteSceneObject.IsValid() && SpriteAsset.SpriteAreasByName != null )
		{
			if ( string.IsNullOrEmpty( CurrentAnimation ) )
			{
				ActiveSpriteArea = SpriteAsset.SpriteAreasByName.FirstOrDefault().Value;
			}
			else if ( SpriteAsset.SpriteAreasByName.TryGetValue( CurrentAnimation.ToLower(), out var area ) )
			{
				ActiveSpriteArea = area;
			}
			SpriteSceneObject.Init();
		}
	}
	[ConCmd.Client]
	public static void RebuildSprites()
	{
		foreach ( var item in Entity.All.OfType<ModelSprite>() )
		{
			item.SpriteChange();
		}
	}
	[Event( "spriteassets_changed" )]
	protected void DetectSpriteChange( int id )
	{
		if ( SpriteAsset == null || id == SpriteAsset.ResourceId )
		{
			SpriteChange();
		}
	}
	[Event.Client.Frame]
	protected virtual void Think()
	{
		if ( Game.IsServer ) return;
		if ( SpriteSceneObject.IsValid() )
		{

			SpriteSceneObject.Update();
		}
		else
		{
			if ( SpriteAsset != null )
				SpriteSceneObject = new( TargetSceneWorld.IsValid() ? TargetSceneWorld : Game.SceneWorld, this );
		}
	}

	public void SetAnimation( string Name )
	{
		if ( Game.IsServer )
		{
			SetAnimClient( Name );
		}
		else
		{
			SetAnim( Name );
		}

	}
	[ClientRpc]
	private void SetAnimClient( string Name )
	{
		SetAnim( Name );
	}
	private void SetAnim( string Name )
	{
		if ( SpriteAsset == null || Name.ToLower() == ActiveSpriteArea.Name.ToLower() ) return;
		if ( SpriteAsset.SpriteAreasByName.TryGetValue( Name.ToLower(), out var area ) )
		{
			ActiveSpriteArea = area;
			CurrentAnimation = Name;
		}
		else
		{
			Log.Warning( $"Animation {Name} not found" );
		}
	}
	public void SetFacingDirection( bool FacingRight )
	{
		LookDirection = FacingRight ? FacingDirection.Right : FacingDirection.Left;
	}



	public void AnimationFinished()
	{
		if ( OneShotAnimation )
		{
			DeleteThis( this.NetworkIdent );
		}
	}
	[ConCmd.Server]
	private static void DeleteThis( int entityid )
	{
		FindByIndex( entityid )?.Delete();
	}

	//Change Callbacks
	private void OnLookDirectionChanged()
	{
		if ( SpriteSceneObject.IsValid() )
		{
			SpriteSceneObject.Facing = LookDirection;
		}
	}
	private void OnTintColorChanged()
	{
		if ( SpriteSceneObject.IsValid() )
		{
			SpriteSceneObject.TintColor = TintColor;
		}
	}
	private void OnTintAmountChanged()
	{
		if ( SpriteSceneObject.IsValid() )
		{
			SpriteSceneObject.TintAmount = TintAmount;
		}
	}
}
