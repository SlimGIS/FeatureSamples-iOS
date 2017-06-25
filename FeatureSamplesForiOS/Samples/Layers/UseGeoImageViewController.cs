using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.Utilities;

namespace FeatureSamplesForiOS
{
	public partial class UseGeoImageViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;

			GeoImage image = new GeoImage(@"SampleData/bing-aerial-900913.png");
			GeoSize size = image.GetSize();
			GeoImageLayer geoImageLayer = new GeoImageLayer(image, new WorldFile(GeoCommonHelper.GetMaxBound(GeoUnit.Meter), (float)size.Width, (float)size.Height));

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(geoImageLayer);
			MapView.Overlays.Add(layerOverlay);
			MapView.ZoomToFullBound();
		}
	}
}

