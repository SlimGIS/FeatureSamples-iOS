using System.Linq;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.Utilities;

namespace FeatureSamplesForiOS
{
	public partial class GetLineDistanceViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileLayer dataLayer = new ShapefileLayer("SampleData/streets-900913.shp");
			dataLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#55FAB04D"), 4));
			MapView.AddStaticLayers(dataLayer);

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Columns.Add(new FeatureColumn("Distance", UnifiedColumnType.Text));
			highlightLayer.Styles.Add(new LineStyle(GeoColor.FromHtml("#8800BCD4"), 8));

			LabelStyle distanceLabelStyle = new LabelStyle("Distance", new GeoFont(), GeoColor.FromHtml("#111111"), GeoColors.White);
			distanceLabelStyle.LineSegmentRatio = 100;
			highlightLayer.Styles.Add(distanceLabelStyle);
			MapView.AddDynamicLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(new GeoBound(-10880446.205, 3540612.6137, -10879623.9531, 3541519.945));
			MapView.MapTap += (sender, e) =>
			{
				Feature identifiedFeature = IdentifyHelper.Identify(dataLayer, e.WorldLocation, MapView.CurrentScale, MapView.MapUnit).FirstOrDefault();
				if (identifiedFeature != null)
				{
					double distanceInMeter = ((GeoLinearBase)identifiedFeature.Geometry).GetLength();
					identifiedFeature.FieldValues.Add("Distance", $"{distanceInMeter:N2} m");

					highlightLayer.Features.Clear();
					highlightLayer.Features.Add(identifiedFeature);

					MapView.Refresh("HighlightOverlay");
				}
			};
		}
	}
}

