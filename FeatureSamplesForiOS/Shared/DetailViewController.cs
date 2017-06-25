using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using SlimGis.MapKit.iOS;
using UIKit;

namespace FeatureSamplesForiOS
{
	[InheritedExport]
	public partial class DetailViewController : UIViewController
	{
		bool executeButtonInitialized;
		UIBarButtonItem executeButton;
		Func<Task> executeAsyncFunc;
		Action executeAction;

		public Action DetailButtonClick { get; set; }

		public Func<Task> ExecuteAsyncFunc
		{
			get { return executeAsyncFunc; }
			set
			{
				executeAsyncFunc = value;
				InitExecuteButton();
			}
		}

		public Action ExecuteAction
		{
			get { return executeAction; }
			set
			{
				executeAction = value;
				InitExecuteButton();
			}
		}

		public MapView MapView { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			View.BackgroundColor = UIColor.FromRGBA(240 / 255f, 236 / 255f, 227 / 255f, 1);

			UIBarButtonItem detailButton = new UIBarButtonItem(UIImage.FromBundle("detail30x30"), UIBarButtonItemStyle.Plain, OnDetailItemClick);
			detailButton.TintColor = UIColor.Black;
			NavigationItem.SetLeftBarButtonItem(detailButton, true);

			MapView = new MapView(View.Frame);
			MapView.AutoresizingMask = UIViewAutoresizing.All;
			View.AddSubview(MapView);
		}

		protected void InitExecuteButton()
		{
			if (!executeButtonInitialized && (ExecuteAction != null || ExecuteAsyncFunc != null))
			{
				executeButton = new UIBarButtonItem("GO", UIBarButtonItemStyle.Plain, OnGoItemClick);
				executeButton.TintColor = UIColor.Black;
				NavigationItem.SetRightBarButtonItems(new[] { executeButton }, true);
				executeButtonInitialized = true;
			}
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		async void OnGoItemClick(object sender, EventArgs e)
		{
			executeButton.Enabled = false;
			ExecuteAction?.Invoke();
			if (ExecuteAsyncFunc != null) await ExecuteAsyncFunc();

			executeButton.Enabled = true;
		}

		void OnDetailItemClick(object sender, EventArgs e)
		{
			DetailButtonClick?.Invoke();
		}
	}
}