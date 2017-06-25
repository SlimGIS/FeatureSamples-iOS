using System.Linq;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class UseGridFileViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			GridLayer gridLayer = new GridLayer("SampleData/gridfile-900913.grd");
			GridFileSource gridSource = (GridFileSource)gridLayer.Source;
			var cellValues = gridSource.GetDistinctCellValues();
			cellValues.Sort();

			ClassBreakStyle style = ClassBreakStyle.Create("CellValue", cellValues.Last(), cellValues.First(), 8, DimensionType.Area, GeoColorFamily.Hue, GeoColors.Red, 1f);
			style.Margin = 25;
			gridLayer.Styles.Add(style);

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(gridLayer);
			MapView.Overlays.Add(layerOverlay);

			MapView.ZoomTo(gridLayer.GetBound());
		}
	}
}

