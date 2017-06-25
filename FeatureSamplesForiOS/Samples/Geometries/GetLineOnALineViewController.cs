using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class GetLineOnALineViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();
			GeoBound currentBound = new GeoBound(2171997.6512, 8356849.2669, 3515687.9933, 11097616.86);

			MemoryLayer baselineLayer = new MemoryLayer { Name = "BaseLineLayer" };
			baselineLayer.Styles.Add(new LineStyle(GeoColors.White, 8));
			baselineLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#88FAB04D"), 4));
			baselineLayer.Features.Add(CreateLineFeature(currentBound));
			MapView.AddStaticLayers(baselineLayer);

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#9903A9F4"), 4));
			MapView.AddDynamicLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(currentBound);

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
				await MoveLine(highlightLayer, baseline, percentageRatio, i);
			}
		}

		async Task MoveLine(MemoryLayer highlightLayer, GeoLine baseline, double percentageRatio, int i)
		{
			GeoLinearBase line = await Task.Run(() => baseline.GetSegmentation((float)percentageRatio * i));
			highlightLayer.Features.Clear();

			if (line != null) highlightLayer.Features.Add(new Feature(line));
			MapView.Refresh("HighlightOverlay");

			await Task.Run(() => NSThread.SleepFor(0.05));
		}

		Feature CreateLineFeature(GeoBound bound)
		{
			GeoLine line = new GeoLine();

			double startX = bound.MinX + bound.Width * .15;
			double endX = bound.MinX + bound.Width * .85;
			double centerY = bound.GetCentroid().Y;
			double height = bound.Height * .25;
			int segmentCount = 30;
			double segmentHorizontalLength = (endX - startX) / segmentCount;

			for (int i = 0; i < segmentCount; i++)
			{
				double x = startX + segmentHorizontalLength * i;
				double y = Math.Sin(Math.PI * 2 * i / segmentCount) * height + centerY;
				line.Coordinates.Add(new GeoCoordinate(x, y));
			}

			return new Feature(line);
		}
	}
}

