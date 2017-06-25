using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.iOS;
using UIKit;

namespace FeatureSamplesForiOS
{
	public partial class PopupsViewController : DetailViewController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MapView.MapUnit = GeoUnit.Meter;
			MapView.UseOpenStreetMapAsBaseMap();
			MapView.ZoomToFullBound();

			MapView.MapTap += (sender, e) =>
			{
				Popup popup = new Popup(e.WorldLocation);
				UILabel labelView = new UILabel();
				labelView.Lines = 2;
				labelView.Font = UIFont.FromName("ARIAL", 12);
				labelView.Text = $"x:{e.WorldLocation.X.ToString("N")} \r\n y:{e.WorldLocation.Y.ToString("N")}";
				popup.Content.AddSubview(labelView);
				labelView.SizeToFit();
				popup.SizeToFit();

				MapView.Placements.Add(popup);
			};
		}
	}
}

