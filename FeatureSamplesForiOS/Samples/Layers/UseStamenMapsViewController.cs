using System;
using CoreGraphics;
using SlimGis.MapKit.Controls;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using UIKit;

namespace FeatureSamplesForiOS
{
	public partial class UseStamenMapsViewController : DetailViewController
	{
		static readonly string[] stamenMapsOptions = Enum.GetNames(typeof(StamenMapType));

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			MapView.MapUnit = GeoUnit.Meter;

			StamenMapLayer stamenMapLayer = new StamenMapLayer();

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(stamenMapLayer);
			MapView.Overlays.Add(layerOverlay);

			// We can also use the code below to add a stamen maps to our map.
			// Map1.UseStamenMapAsBaseMap(StamenMapType.Toner);

			MapView.ZoomToFullBound();

			UISegmentedControl segmentedControl = new UISegmentedControl(stamenMapsOptions);
			segmentedControl.Center = new CGPoint(View.Center.X, 100);
			segmentedControl.BackgroundColor = UIColor.White;
			segmentedControl.SelectedSegment = 0;
			segmentedControl.AutoresizingMask = UIViewAutoresizing.All ^ UIViewAutoresizing.FlexibleTopMargin ^ UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(segmentedControl);

			segmentedControl.ValueChanged += (sender, e) =>
			{
				stamenMapLayer.MapType = (StamenMapType)Enum.Parse(typeof(StamenMapType), stamenMapsOptions[segmentedControl.SelectedSegment]);
				MapView.Refresh(RefreshType.Redraw);
			};
		}
	}
}

