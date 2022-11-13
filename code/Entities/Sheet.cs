
[Library( "sheetrock_sheet" )]
[Solid]
[Display( Name = "Sheet", GroupName = "Materials", Description = "Sheetrock to cut sheets from" ), Category( "Tools" ), Icon( "landscape" )]
[HammerEntity]
internal class Sheet : BrushEntity, IUsable
{

	public Color GlowColor => Color.Orange;

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is not Pawn p ) 
			return false;

		if( IsClient )
		{
			Log.Error( "Enter sheetrock cutting mode" );
		}

		return false;
	}

}
