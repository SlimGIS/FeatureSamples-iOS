using System.Threading.Tasks;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class DifferenceViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileSource dataSource = new ShapefileSource("SampleData/countries-900913.shp");
			dataSource.Open();
			Feature highlightFeature = dataSource.GetFeatureById("1", RequireColumnsType.None);

			GeoBound highlightBound = highlightFeature.GetBound();
			Feature highlightBoundFeature = new Feature(highlightBound);

			MemoryLayer highlightLayer = new MemoryLayer() { Name = "HighlightLayer" };
			highlightLayer.Features.Add(highlightFeature);
			highlightLayer.Features.Add(highlightBoundFeature);
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D")));
			highlightLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#00BCD4"), 4));
			MapView.AddStaticLayers("HighlightOverlay", highlightLayer);

			MemoryLayer resultLayer = new MemoryLayer { Name = "ResultLayer" };
			resultLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D"), GeoColors.White));
			MapView.AddDynamicLayers("ResultOverlay", resultLayer);

			MapView.ZoomTo(highlightFeature);

			ExecuteAsyncFunc = async () =>
			{
				if (resultLayer.Features.Count == 0)
				{
					highlightLayer.Features.Clear();

					Geometry difference = await Task.Run(() => highlightBoundFeature.Geometry.GetDifference(highlightFeature.Geometry));
					resultLayer.Features.Add(new Feature(difference));
					MapView.Refresh("HighlightOverlay", "ResultOverlay");
				}
			};
		}
	}
}

