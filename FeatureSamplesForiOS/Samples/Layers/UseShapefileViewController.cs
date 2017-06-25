using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class UseShapefileViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileLayer shapefileLayer = new ShapefileLayer("SampleData/countries-900913.shp");
			shapefileLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D"), GeoColors.White));

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(shapefileLayer);
			MapView.Overlays.Add(layerOverlay);

			MapView.ZoomToFullBound();
		}
	}
}

