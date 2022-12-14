
partial class Pawn : Player
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();

		(Controller as WalkController).DefaultSpeed = 80;
		
		EnableHideInFirstPerson = true;

		Inventory = new BaseInventory( this );
		Inventory.Add( new TapeMeasure(), true );
	}

    public override void Simulate(Client cl)
    {
        base.Simulate(cl);

		if ( ActiveChild.IsValid() )
		{
            SimulateActiveChild( cl, ActiveChild );
        }

		var tr = TraceEyes();

		if( Input.Pressed( InputButton.Use ) && tr.Entity is IUsable use )
		{
			use.OnUse( this );
		}

		foreach ( var ent in Entity.All )
		{
			if ( ent is not IUsable u ) continue;

			var glow = ent.Components.GetOrCreate<Glow>();
			glow.Enabled = ent == tr.Entity;
			glow.Color = u.GlowColor;
		}
	}

    public override void PostCameraSetup( ref CameraSetup setup )
	{
		setup.ZNear = 0.1f;

		base.PostCameraSetup( ref setup );

		if ( setup.Viewer != null )
		{
			AddCameraEffects( ref setup );
		}
	}

	public TraceResult TraceEyes( float distance = 1000.0f )
	{
		return Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * distance )
			.WorldAndEntities()
			.Ignore( this )
			.Run();
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects( ref CameraSetup setup )
	{
		var speed = Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = Velocity.Normal.Dot( setup.Rotation.Forward );

		var left = setup.Rotation.Left;
		var up = setup.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		setup.Position += up * MathF.Sin( walkBob ) * speed * 2;
		setup.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( Velocity.Dot( setup.Rotation.Right ) * 0.11f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.3f;
		setup.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 4.0f );

		setup.FieldOfView = 105;
		setup.FieldOfView += fov;

	}

    public override float FootstepVolume()
    {
		return Velocity.Length.LerpInverse(0f, 150.0f);
    }

    private TimeSince timeSinceLastFootstep;
    public override void OnAnimEventFootstep(Vector3 pos, int foot, float volume)
    {
        if (!IsClient) return;
        if (timeSinceLastFootstep < 0.2f) return;

        timeSinceLastFootstep = 0;

        var tr = Trace.Ray(pos, pos + Vector3.Down * 20)
            .Radius(1)
            .Ignore(this)
            .Run();

        if (!tr.Hit) return;

        tr.Surface.DoFootstep(this, tr, foot, volume * FootstepVolume() * 10.0f);
    }

}
