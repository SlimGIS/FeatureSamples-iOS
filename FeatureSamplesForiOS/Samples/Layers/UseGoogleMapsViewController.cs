using System;
using CoreGraphics;
using SlimGis.MapKit.Controls;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using SlimGis.MapKit.Layers;
using UIKit;

namespace FeatureSamplesForiOS
{
	public partial class UseGoogleMapsViewController : DetailViewController
	{
		static readonly string[] googleMapsOptions = Enum.GetNames(typeof(GoogleMapsType));
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;

			GoogleMapsLayer googleMapsLayer = new GoogleMapsLayer(GoogleMapsType.RoadMap, "AIzaSyC2CYaFRRi-Caf24Lq_2xu5seA3EzLWKv0");

			LayerOverlay layerOverlay = new LayerOverlay();
			layerOverlay.Layers.Add(googleMapsLayer);
			MapView.Overlays.Add(layerOverlay);

			// We can also use the code below to add a google maps to our map.
			// Map1.UseGoogleMapsAsBaseMap(GoogleMapsType.RoadMap, "Your api key", "Your client id");

			MapView.ZoomToFullBound();

			UISegmentedControl segmentedControl = new UISegmentedControl(googleMapsOptions);
			segmentedControl.Center = new CGPoint(View.Center.X, 100);
			segmentedControl.BackgroundColor = UIColor.White;
			segmentedControl.SelectedSegment = 0;
			segmentedControl.AutoresizingMask = UIViewAutoresizing.All ^ UIViewAutoresizing.FlexibleTopMargin ^ UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(segmentedControl);

			segmentedControl.ValueChanged += (sender, e) =>
			{
				googleMapsLayer.MapsType = (GoogleMapsType)Enum.Parse(typeof(GoogleMapsType), googleMapsOptions[segmentedControl.SelectedSegment]);
                MapView.Refresh(RefreshType.Redraw);
			};
		}
	}
}

