using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;

namespace FeatureSamplesForiOS
{
	public partial class SpatialQueryViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			ShapefileLayer sectionLayer = new ShapefileLayer("SampleData/sections-900913.shp");
			sectionLayer.Styles.Add(new FillStyle(GeoColors.Transparent, GeoColor.FromHtml("#99FAB04D"), 1));
			MapView.AddStaticLayers("SectionOverlay", sectionLayer);

			GeoBound sectionBound = sectionLayer.GetBound();
			GeoBound queryArea = (GeoBound)sectionBound.Clone();
			queryArea.ScaleDown(60);

			MemoryLayer queryAreaLayer = new MemoryLayer { Name = "QueryAreaLayer" };
			queryAreaLayer.Styles.Add(new FillStyle(GeoColors.Transparent, GeoColor.FromHtml("#9900BCD4"), 4));
			queryAreaLayer.Features.Add(new Feature(queryArea));
			MapView.AddStaticLayers("SectionOverlay", queryAreaLayer);

			MemoryLayer highlightLayer = new MemoryLayer { Name = "HighlightLayer" };
			highlightLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#66FFFF00"), GeoColors.White));
			MapView.AddDynamicLayers("HighlightOverlay", highlightLayer);

			MapView.ZoomTo(sectionLayer.GetBound());

			ExecuteAsyncFunc = ExecuteAsync;
		}

		async Task ExecuteAsync()
		{
			ShapefileLayer sectionLayer = MapView.FindLayer<ShapefileLayer>("sections-900913");
			MemoryLayer highlightLayer = MapView.FindLayer<MemoryLayer>("HighlightLayer");
			MemoryLayer queryAreaLayer = MapView.FindLayer<MemoryLayer>("QueryAreaLayer");
			Geometry queryArea = queryAreaLayer.Features.First().Geometry;
			IEnumerable<Feature> resultFeatures = await Task.Run(() => sectionLayer.Source.SpatialQuery(queryArea, SpatialQueryMode.Intersecting));

			highlightLayer.Features.Clear();
			foreach (var feature in resultFeatures)
			{
				highlightLayer.Features.Add(feature);
			}

			MapView.Refresh("HighlightOverlay");
		}
	}
}

