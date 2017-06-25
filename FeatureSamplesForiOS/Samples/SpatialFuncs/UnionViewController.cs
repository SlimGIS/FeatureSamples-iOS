using System.Threading.Tasks;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class UnionViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			GeoBound bound = new GeoBound(2171997.6512, 8356849.2669, 3515687.9933, 11097616.86);
			GeoPoint center = bound.GetCentroid();
			double x1 = bound.MinX + bound.Width * .25;
			double y = center.Y;
			double x2 = bound.MaxX - bound.Width * .25;
			double radius = bound.Width * 3 / 8;

			Feature feature1 = new Feature(new GeoEllipse(new GeoPoint(x1, y), radius));
			Feature feature2 = new Feature(new GeoEllipse(new GeoPoint(x2, y), radius));

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Features.Add(feature1);
			highlightLayer.Features.Add(feature2);
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D"), GeoColors.White));
			MapView.AddStaticLayers("HighlightOverlay", highlightLayer);

			MemoryLayer resultLayer = new MemoryLayer { Name = "ResultLayer" };
			resultLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#55FAB04D"), GeoColors.White));
			MapView.AddDynamicLayers("ResultOverlay", resultLayer);

			MapView.ZoomTo(bound);

			ExecuteAsyncFunc = async () =>
			{
				if (resultLayer.Features.Count == 0)
				{
					highlightLayer.Features.Clear();

					GeoAreaBase unionResult = await Task.Run(() => ((GeoAreaBase)feature1.Geometry).Union((GeoAreaBase)feature2.Geometry));
					resultLayer.Features.Add(new Feature(unionResult));
					MapView.Refresh("HighlightOverlay", "ResultOverlay");
				}
			};
		}
	}
}

