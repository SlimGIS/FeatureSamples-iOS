using System.Threading.Tasks;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class GetConvexHullViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileSource dataSource = new ShapefileSource("SampleData/countries-900913.shp");
			dataSource.Open();
			Feature highlightFeature = dataSource.GetFeatureById("1", RequireColumnsType.None);

			MemoryLayer resultLayer = new MemoryLayer { Name = "ResultLayer" };
			resultLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#553F8CB4"), GeoColors.White));
			MapView.AddDynamicLayers("ResultOverlay", resultLayer);

			MemoryLayer highlightLayer = new MemoryLayer();
			highlightLayer.Features.Add(highlightFeature);
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D")));
			MapView.AddStaticLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(highlightFeature);

			ExecuteAsyncFunc = async () =>
			{
				if (resultLayer.Features.Count == 0)
				{
					Feature bufferedFeature = await Task.Run(() => new Feature(highlightFeature.Geometry.GetConvexHull()));
					resultLayer.Features.Add(bufferedFeature);
					MapView.Refresh("ResultOverlay");
				}
			};
		}
	}
}

