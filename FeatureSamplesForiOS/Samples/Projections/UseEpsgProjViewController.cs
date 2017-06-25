using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class UseEpsgProjViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Degree;

			ShapefileLayer featureLayer = new ShapefileLayer("SampleData/countries-wgs84.shp");
			featureLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#FAB04D"), GeoColors.White));
			MapView.AddStaticLayers("countries-wgs84", featureLayer);

			MapView.ZoomToFullBound();

			ExecuteAction = Execute;
		}

		void Execute()
		{
			if (MapView.MapUnit == GeoUnit.Meter) return;

			MapView.MapUnit = GeoUnit.Meter;
			FeatureLayer featureLayer = MapView.FindLayer<ShapefileLayer>("countries-wgs84");
			SpatialReference srsSource = SpatialReferences.GetWgs84();
			SpatialReference srsTarget = SpatialReference.ParseSrid(2163);
			featureLayer.Source.Projection = new Proj4Projection(srsSource, srsTarget);

			MapView.Overlays["countries-wgs84"].Invalidate();
			MapView.ZoomToFullBound();
		}
	}
}

