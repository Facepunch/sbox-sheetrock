
internal partial class TapeMeasure : BaseTool
{

	public struct TapeInfo
	{
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }

		public TapeInfo( TraceResult tr )
		{
			Position = tr.EndPosition.SnapToGrid( 1 );
			Normal = tr.Normal;
		}
	}

	[Net, Predicted]
	public TapeInfo TapeStart { get; set; }
	[Net, Predicted]
	public TapeInfo TapeStop { get; set; }
	[Net, Predicted]
	public bool Measuring { get; set; }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( !Pawn.IsValid() ) return;

		if ( Measuring )
		{
			UpdateTape();

			var length = (int)(TapeStop.Position - TapeStart.Position).Length;

			DebugOverlay.Circle( TapeStart.Position, Rotation.LookAt( TapeStart.Normal ), 0.25f, Color.Green );
			DebugOverlay.Circle( TapeStop.Position, Rotation.LookAt( TapeStop.Normal ), 0.25f, Color.Green );
			DebugOverlay.Line( TapeStart.Position, TapeStop.Position );
			DebugOverlay.Text( length.ToString(), TapeStop.Position + Vector3.Up * 2 );
		}

		if ( !Measuring && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			StartTape();
		}

		if ( Measuring && Input.Released( InputButton.PrimaryAttack ) )
		{
			ReleaseTape();
		}
	}

	private void StartTape()
	{
		var tr = Pawn.TraceEyes();
		if ( !tr.Hit )
		{
			return;
		}

		TapeStart = new TapeInfo( tr );
		Measuring = true;
	}

	private void UpdateTape()
	{
		var tr = Pawn.TraceEyes();
		if ( !tr.Hit )
		{
			return;
		}

		TapeStop = new TapeInfo( tr );
	}

	private void ReleaseTape()
	{
		Measuring = false;
	}

	public override void CreateHudElements()
	{
		base.CreateHudElements();

		Local.Hud.AddChild<TapeMeasureHud>();
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		Local.Hud.ChildrenOfType<TapeMeasureHud>()?.FirstOrDefault()?.Delete();
	}

}
