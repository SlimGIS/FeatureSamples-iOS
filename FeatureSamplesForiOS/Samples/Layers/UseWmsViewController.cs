using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;

namespace FeatureSamplesForiOS
{
	public partial class UseWmsViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();

			WmsLayer wmsLayer = new WmsLayer("https://ahocevar.com/geoserver/wms");
			wmsLayer.Name = "Wms";
			wmsLayer.Crs = "EPSG:900913";
			wmsLayer.Layers.Add("topp:states");
			wmsLayer.Parameters.Add("TILED", "TRUE");

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(wmsLayer);
			MapView.Overlays.Add(layerOverlay);

			MapView.ZoomToFullBound();
		}
	}
}

