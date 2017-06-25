using System;
using CoreGraphics;
using SlimGis.MapKit.Controls;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using UIKit;

namespace FeatureSamplesForiOS
{
	public partial class UseBingMapsViewController : DetailViewController
	{
		static readonly string[] bingMapsOptions = Enum.GetNames(typeof(BingMapsType));

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;

			BingMapsLayer bingMapsLayer = new BingMapsLayer(BingMapsType.Road, "AswP3q2unPpwB7h5xK-DbPvN_2ZbjQqGeQ18dp3HfSn3IKon4dLM3-IDX3jswTMd");
			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(bingMapsLayer);
			MapView.Overlays.Add(layerOverlay);

			// We can also use the code below to add a google maps to our map.
			// Map1.UseBingMapsAsBaseMap(BingMapsType.Road, "Your BingMaps Key");

			MapView.ZoomToFullBound();

			UISegmentedControl segmentedControl = new UISegmentedControl(bingMapsOptions);
			segmentedControl.Center = new CGPoint(View.Center.X, 100);
			segmentedControl.BackgroundColor = UIColor.White;
			segmentedControl.SelectedSegment = 0;
			segmentedControl.AutoresizingMask = UIViewAutoresizing.All ^ UIViewAutoresizing.FlexibleTopMargin ^ UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(segmentedControl);

			segmentedControl.ValueChanged += (sender, e) =>
			{
				bingMapsLayer.MapsType = (BingMapsType)Enum.Parse(typeof(BingMapsType), bingMapsOptions[segmentedControl.SelectedSegment]);
				MapView.Refresh(RefreshType.Redraw);
			};
		}
	}
}

