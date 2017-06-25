using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;

namespace FeatureSamplesForiOS
{
	public partial class UseOpenStreetMapViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;

			OpenStreetMapLayer openStreetMapLayer = new OpenStreetMapLayer();

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(openStreetMapLayer);
			MapView.Overlays.Add(layerOverlay);

			// We can also use the code below to add a google maps to our map.
			// Map1.UseOpenStreetMapAsBaseMap();

			MapView.ZoomToFullBound();
		}
	}
}

