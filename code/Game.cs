
/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class MyGame : Sandbox.Game
{

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		var effects = Map.Camera.FindOrCreateHook<Sandbox.Effects.ScreenEffects>();
		effects.FilmGrain.Intensity = 0.02f + 0.01f * 0.3f;
		effects.FilmGrain.Response = 1.0f;
		effects.Sharpen = 0.25f + 0.01f * 0.15f;
		effects.Brightness = 0.5f;
		effects.ChromaticAberration.Scale = 0.01f;
		effects.Pixelation = 0.01f * 0.1f;
		effects.Vignette.Intensity = 0.8f;
		effects.Vignette.Roundness = 0.5f;
		effects.Vignette.Smoothness = 1f;
		effects.MotionBlur.Scale = .001f;
		effects.MotionBlur.Samples = 10;
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Pawn();
		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}
	}
}
