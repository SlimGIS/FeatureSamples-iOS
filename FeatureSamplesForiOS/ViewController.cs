using MonoTouch.Dialog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UIKit;

namespace FeatureSamplesForiOS
{
    public partial class ViewController : UIViewController
    {
        private SliderViewController navigation;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            navigation = new SliderViewController();
            navigation.Position = FlyOutPosition.Left;
            navigation.View.Frame = UIScreen.MainScreen.Bounds;
            View.AddSubview(navigation.View);
            AddChildViewController(navigation);

            SamplesList samplesList = SamplesList.LoadAsync();
            Collection<UIViewController> styleList = new Collection<UIViewController>();
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            var rootElement = new RootElement("Feature Samples");
            foreach(var sampleSection in samplesList.Sections)
            {
                Section section = new Section(sampleSection.Name);
                rootElement.Add(section);

                foreach(var sample in sampleSection.Samples)
                {
					string name = sample.Name;
					string image = "buffer";
					string className = sample.TypeName;

					UIImage icon = UIImage.FromBundle(image);
                    BadgeElement sampleElement = new BadgeElement(icon, name);
					section.Add(sampleElement);

					Type sampleType = currentAssembly.GetType("FeatureSamplesForiOS." + className);
					DetailViewController sampleController = (DetailViewController)Activator.CreateInstance(sampleType);

					sampleController.DetailButtonClick = navigation.ToggleMenu;
					sampleController.Title = name;
					UINavigationController mainController = new UINavigationController(sampleController);
					styleList.Add(mainController);
                }
            }

            navigation.ViewControllers = styleList.ToArray();
            navigation.NavigationRoot = rootElement;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}