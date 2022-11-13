
class BaseTool : BaseCarriable
{

    protected TraceResult TraceEyes()
	{
		return Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 1000f )
			.WorldAndEntities()
			.Ignore( Owner )
			.Run();
	}

}
