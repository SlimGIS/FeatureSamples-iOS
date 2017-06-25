using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;

namespace FeatureSamplesForiOS
{
	public partial class MarkersViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();
			MapView.ZoomToFullBound();

			MapView.MapTap += (sender, e) =>
			{
				Marker marker = new Marker(e.WorldLocation);
				MapView.Placements.Add(marker);
			};
		}
	}
}

