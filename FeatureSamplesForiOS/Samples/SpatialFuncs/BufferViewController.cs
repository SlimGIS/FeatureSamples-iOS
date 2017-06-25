using System.Threading.Tasks;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class BufferViewController : DetailViewController
	{
		Feature highlightFeature;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileSource dataSource = new ShapefileSource("SampleData/countries-900913.shp");
			dataSource.Open();
			highlightFeature = dataSource.GetFeatureById("1", RequireColumnsType.None);

			MemoryLayer bufferedLayer = new MemoryLayer { Name = "BufferResultLayer" };
			bufferedLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#553F8CB4"), GeoColors.White));
			MapView.AddDynamicLayers("BufferedOverlay", bufferedLayer);

			MemoryLayer highlightLayer = new MemoryLayer();
			highlightLayer.Features.Add(highlightFeature);
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D")));
			MapView.AddStaticLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(highlightFeature);

			ExecuteAsyncFunc = ExecuteAsync;
		}

		async Task ExecuteAsync()
		{
			MemoryLayer bufferResultLayer = MapView.FindLayer<MemoryLayer>("BufferResultLayer");

			Feature bufferedFeature = await Task.Run(() => new Feature(highlightFeature.Geometry.GetBuffer(100000)));
			bufferResultLayer.Features.Clear();
			if (bufferedFeature.Geometry != null)
			{
				bufferResultLayer.Features.Add(bufferedFeature);
				MapView.Refresh("BufferedOverlay");
			}
		}
	}
}

