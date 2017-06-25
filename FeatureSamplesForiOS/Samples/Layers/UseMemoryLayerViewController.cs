using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class UseMemoryLayerViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileSource dataSource = new ShapefileSource("SampleData/countries-900913.shp");
			Feature tempFeature = dataSource.GetFeatureById("1", RequireColumnsType.None);

			MemoryLayer memoryLayer = new MemoryLayer { Name = "ResultLayer" };
			memoryLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D"), GeoColors.White));
			memoryLayer.Features.Add(tempFeature);

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(memoryLayer);
			MapView.Overlays.Add(layerOverlay);

			MapView.ZoomTo(tempFeature);
		}
	}
}

