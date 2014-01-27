namespace CardDataLayer.Models
{
	public class PixelLocation
	{
		public int PointId { get; set; }
		public string OperatingSystemId { get; set; }
		public string ResolutionId { get; set; }
		public string LocationId { get; set; }
		public int? Top { get; set; }
		public int? Left { get; set; }
		public int? Bottom { get; set; }
		public int? Right { get; set; }
		public string ArrayValue { get; set; }
	}
}
