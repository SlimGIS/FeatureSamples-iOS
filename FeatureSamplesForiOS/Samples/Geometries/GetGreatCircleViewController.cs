using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using UIKit;

namespace FeatureSamplesForiOS
{
	public partial class GetGreatCircleViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			GeoLine greatCircle = CreateGreatCircle();

			MemoryLayer baselineLayer = new MemoryLayer { Name = "BaseLineLayer" };
			baselineLayer.Styles.Add(new LineStyle(GeoColors.White, 8));
			baselineLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#88FAB04D"), 4));
			baselineLayer.Features.Add(new Feature(greatCircle));
			MapView.AddStaticLayers(baselineLayer);

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#9903A9F4"), 4));
			highlightLayer.Styles.Add(new IconStyle(new GeoImage(UIImage.FromBundle("airplane"))) { AspectRatio = 0.6667 });
			MapView.AddDynamicLayers("HighlightOverlay", highlightLayer);

			GeoBound bound = baselineLayer.GetBound();
			bound.ScaleUp(25);
			MapView.ZoomTo(bound);

			ExecuteAsyncFunc = ExecuteAsync;
		}

		async Task ExecuteAsync()
		{
			MemoryLayer baselineLayer = MapView.FindLayer<MemoryLayer>("BaseLineLayer");
			MemoryLayer highlightLayer = MapView.FindLayer<MemoryLayer>("HighlightLayer");
			GeoLine baseline = (GeoLine)baselineLayer.Features.First().Geometry;

			int times = 100;
			double percentageRatio = 100d / times;

			for (int i = 0; i <= times; i++)
			{
				await MoveLine(highlightLayer, baseline, percentageRatio, i);
			}

			for (int i = times; i >= 0; i--)
			{
				await MoveLine(highlightLayer, baseline, percentageRatio, i, true);
			}
		}

		GeoLine CreateGreatCircle()
		{
			GeoPoint point1 = new GeoPoint(-73.935242, 40.730610);
			GeoPoint point2 = new GeoPoint(2.294694, 48.858093);
			GeoMultiLine greatCircleInWgs84 = point1.GetGreatCircle(point2, 30);

			Projection projection = new Proj4Projection(SpatialReferences.GetWgs84(), SpatialReferences.GetSphericalMercator());
			GeoMultiLine greatCircleIn900913 = (GeoMultiLine)projection.ConvertToTarget(greatCircleInWgs84);
			return greatCircleIn900913.Lines[0];
		}

		async Task MoveLine(MemoryLayer highlightLayer, GeoLine baseline, double percentageRatio, int i, bool reverse = false)
		{
			GeoLine line = await Task.Run(() => (GeoLine)baseline.GetSegmentation((float)percentageRatio * i));
			highlightLayer.Features.Clear();

			if (line != null)
			{
				highlightLayer.Features.Add(new Feature(line));

				GeoCoordinate currentPoint = line.Coordinates.Last();
				highlightLayer.Features.Add(new Feature(new GeoPoint(currentPoint)));
				if (line.Coordinates.Count > 1)
				{
					GeoCoordinate previousPoint = line.Coordinates.ElementAt(line.Coordinates.Count - 2);
					double angle = Math.Atan2(currentPoint.Y - previousPoint.Y, currentPoint.X - previousPoint.X);
					float degree = (float)(angle * 180 / Math.PI);
					if (reverse) degree = degree + 180;
					highlightLayer.Styles.OfType<IconStyle>().First().Rotation = degree;
				}
			}

			MapView.Refresh("HighlightOverlay");
			await Task.Run(() => NSThread.SleepFor(0.05));
		}
	}
}

