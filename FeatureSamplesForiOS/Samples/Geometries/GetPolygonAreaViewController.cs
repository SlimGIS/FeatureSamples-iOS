using System.Linq;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.Utilities;

namespace FeatureSamplesForiOS
{
	public partial class GetPolygonAreaViewController : DetailViewController
	{
		ShapefileLayer dataLayer;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			dataLayer = new ShapefileLayer("SampleData/sections-900913.shp");
			dataLayer.Styles.Add(new FillStyle(GeoColors.Transparent, GeoColor.FromHtml("#99FAB04D"), 1));
			MapView.AddStaticLayers(dataLayer);

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Columns.Add(new FeatureColumn("Area", UnifiedColumnType.Text));
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#66FFFF00"), GeoColors.White));

			LabelStyle areaLabelStyle = new LabelStyle("Area", new GeoFont(), GeoColor.FromHtml("#111111"), GeoColors.White);
			highlightLayer.Styles.Add(areaLabelStyle);
			MapView.AddDynamicLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(new GeoBound(-10111509.577, 4582117.8006, -10099064.6735, 4598828.7496));
			MapView.MapTap += (sender, e) =>
			{
				Feature identifiedFeature = IdentifyHelper.Identify(dataLayer, e.WorldLocation, MapView.CurrentScale, MapView.MapUnit).FirstOrDefault();
				if (identifiedFeature != null)
				{
					double areaInSquareKilometer = ((GeoAreaBase)identifiedFeature.Geometry).GetArea(GeoUnit.Meter, AreaUnit.SquareKilometers);
					identifiedFeature.FieldValues.Add("Area", $"{areaInSquareKilometer:N2} sq. km");

					highlightLayer.Features.Clear();
					highlightLayer.Features.Add(identifiedFeature);
					MapView.Refresh("HighlightOverlay");
				}
			};
		}
	}
}

