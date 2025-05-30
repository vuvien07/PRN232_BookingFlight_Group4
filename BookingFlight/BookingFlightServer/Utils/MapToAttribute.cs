namespace BookingFlightServer.Utils
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MapToAttribute : Attribute
	{
	   public string TargetName { get; }
	   public MapToAttribute(string targetName) => TargetName = targetName;
	}
}
