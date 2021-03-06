using CoreGraphics;
using Foundation;
using MediaPlayer;
using MonoTouch.Dialog;
using ObjCRuntime;
using System;
using System.Drawing;
using UIKit;

namespace FeatureSamplesForiOS
{
    public enum FlyOutPosition
    {
        Left = 0,
        Right
    };

    [Register("SilderViewController")]
    public class SliderViewController : UIViewController
    {
        private const float sidebarFlickVelocity = 1000.0f;
        private const int menuWidth = 240;
        private bool hideShadow;
        private UIButton closeButton;
        private FlyOutPosition position;
        private DialogViewController leftNavigation;
        private int selectedIndex;
        private UIView shadowView;
        private nfloat startX;
        private UIView statusImage;
        private UIViewController[] viewControllers;
        private bool isIos7;
        private bool isOpen;
        public event UITouchEventArgs ShouldReceiveTouch;
        public Action SelectedIndexChanged;

        public SliderViewController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        public SliderViewController(UITableViewStyle navigationStyle = UITableViewStyle.Plain)
        {
            Initialize(navigationStyle);
        }

        public FlyOutPosition Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                shadowView.Layer.ShadowOffset = new CGSize(Position == FlyOutPosition.Left ? -5 : 5, -1);
            }
        }

        public bool AlwaysShowLandscapeMenu { get; set; }

        public bool ForceMenuOpen { get; set; }

        public bool HideShadow
        {
            get { return hideShadow; }
            set
            {
                if (value == hideShadow) return;

                hideShadow = value;
                if (!hideShadow) shadowView.RemoveFromSuperview();
                else if (MainView != null)
                {
                    View.InsertSubviewBelow(shadowView, MainView);
                }
            }
        }

        public UIColor ShadowViewColor
        {
            get { return shadowView.BackgroundColor; }
            set { shadowView.BackgroundColor = value; }
        }

        public UIViewController CurrentViewController { get; set; }

        public UIView MainView
        {
            get { return CurrentViewController?.View; }
        }

        public RootElement NavigationRoot
        {
            get { return leftNavigation.Root; }
            set
            {
                EnsureInvokedOnMainThread(delegate { leftNavigation.Root = value; });
            }
        }

        public UITableView NavigationTableView
        {
            get { return leftNavigation.TableView; }
        }

        public UIViewController[] ViewControllers
        {
            get { return viewControllers; }
            set
            {
                EnsureInvokedOnMainThread(delegate
                {
                    viewControllers = value;
                    NavigationItemSelected(GetIndexPath(SelectedIndex));
                });
            }
        }

        public bool IsOpen
        {
            get
            {
                return Position == FlyOutPosition.Left ? MainView.Frame.X == menuWidth : MainView.Frame.X == -menuWidth;
            }
            set
            {
                isOpen = value;
                if (value) HideMenu();
                else ShowMenu();
            }
        }

        public bool ShouldStayOpen
        {
            get
            {
                bool isLandscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                return ForceMenuOpen || (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && AlwaysShowLandscapeMenu && isLandscape);
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex == value) return;

                selectedIndex = value;
                EnsureInvokedOnMainThread(delegate { NavigationItemSelected(value); });
            }
        }

        public bool DisableRotation { get; set; }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (DisableRotation) return toInterfaceOrientation == InterfaceOrientation;

            bool theReturn = CurrentViewController == null ? true : CurrentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
            return theReturn;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return CurrentViewController == null ? UIInterfaceOrientationMask.All : CurrentViewController.GetSupportedInterfaceOrientations();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            CGRect navFrame = View.Bounds;
            navFrame.Width = menuWidth;
            if (Position == FlyOutPosition.Right) navFrame.X = MainView.Frame.Width - menuWidth;
            if (leftNavigation.View.Frame != navFrame) leftNavigation.View.Frame = navFrame;
        }

        public override void ViewWillAppear(bool animated)
        {
            CGRect navFrame = leftNavigation.View.Frame;
            navFrame.Width = menuWidth;

            if (Position == FlyOutPosition.Right) navFrame.X = MainView.Frame.Width - menuWidth;
            navFrame.Location = PointF.Empty;
            leftNavigation.View.Frame = navFrame;

            View.BackgroundColor = NavigationTableView.BackgroundColor;
            SetViewSize();
            SetLocation(MainView.Frame);

            base.ViewWillAppear(animated);
        }

        private void Initialize(UITableViewStyle navigationStyle = UITableViewStyle.Plain)
        {
            DisableStatusBarMoving = true;
            statusImage = new UIView { ClipsToBounds = true }.SetAccessibilityId("statusbar");
            leftNavigation = new DialogViewController(navigationStyle, null);

            leftNavigation.OnSelection += NavigationItemSelected;
            CGRect navFrame = leftNavigation.View.Frame;
            navFrame.Width = menuWidth;
            if (Position == FlyOutPosition.Right) navFrame.X = MainView.Frame.Width - menuWidth;
            leftNavigation.View.Frame = navFrame;
            View.AddSubview(leftNavigation.View);

            var version = new Version(UIDevice.CurrentDevice.SystemVersion);
            isIos7 = version.Major >= 7;
            if (isIos7)
            {
                leftNavigation.TableView.TableHeaderView = new UIView(new CGRect(0, 0, 320, 22)) { BackgroundColor = UIColor.Clear };
                leftNavigation.TableView.SectionHeaderHeight = 50;
            }

            leftNavigation.TableView.TableFooterView = new UIView(new CGRect(0, 0, 100, 100)) { BackgroundColor = UIColor.Clear };
            leftNavigation.TableView.ScrollsToTop = false;

            shadowView = new UIView() { BackgroundColor = UIColor.White };
            shadowView.Layer.ShadowOffset = new CGSize(Position == FlyOutPosition.Left ? -5 : 5, -1);
            shadowView.Layer.ShadowColor = UIColor.Black.CGColor;
            shadowView.Layer.ShadowOpacity = .75f;

            closeButton = new UIButton();
            closeButton.TouchUpInside += delegate { HideMenu(); };
            AlwaysShowLandscapeMenu = true;
            View.AddGestureRecognizer(new OpenMenuGestureRecognizer(DragContentView, shouldReceiveTouch));
        }

        private void DragContentView(UIPanGestureRecognizer panGesture)
        {
            if (MainView == null) return;

            if (!HideShadow) View.InsertSubviewBelow(shadowView, MainView);

            leftNavigation.View.Hidden = false;
            CGRect frame = MainView.Frame;
            shadowView.Frame = frame;
            nfloat translation = panGesture.TranslationInView(View).X;

            if (panGesture.State == UIGestureRecognizerState.Began) startX = frame.X;
            else if (panGesture.State == UIGestureRecognizerState.Changed)
            {
                frame.X = translation + startX;
                if (Position == FlyOutPosition.Left)
                {
                    if (frame.X < 0) frame.X = 0;
                    else if (frame.X > menuWidth) frame.X = menuWidth;
                }
                else
                {
                    if (frame.X > 0) frame.X = 0;
                    else if (frame.X < -menuWidth) frame.X = -menuWidth;
                }
                SetLocation(frame);
            }
            else if (panGesture.State == UIGestureRecognizerState.Ended)
            {
                nfloat velocity = panGesture.VelocityInView(View).X;
                nfloat newX = translation + startX;
                bool show = Math.Abs(velocity) > sidebarFlickVelocity ? velocity > 0 : newX > (menuWidth / 2);
                if (Position == FlyOutPosition.Right)
                {
                    show = Math.Abs(velocity) > sidebarFlickVelocity ? velocity < 0 : newX < -(menuWidth / 2);
                }

                if (show) ShowMenu();
                else HideMenu();
            }
        }

        private bool shouldReceiveTouch(UIGestureRecognizer gesture, UITouch touch)
        {
            if (ShouldReceiveTouch != null) return ShouldReceiveTouch(gesture, touch);
            return true;
        }

        private void NavigationItemSelected(NSIndexPath indexPath)
        {
            int index = GetIndex(indexPath);
            NavigationItemSelected(index);
        }

        private void NavigationItemSelected(int index)
        {
            if (selectedIndex == index && CurrentViewController != null)
            {
                HideMenu();
                return;
            }

            selectedIndex = index;
            if (viewControllers == null || viewControllers.Length <= index || index < 0)
            {
                SelectedIndexChanged?.Invoke();
                return;
            }

            if (ViewControllers[index] == null)
            {
                SelectedIndexChanged?.Invoke();
                return;
            }

            if (!DisableStatusBarMoving) UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.Fade);

            CurrentViewController = ViewControllers[SelectedIndex];
            CGRect frame = View.Bounds;
            if (isOpen)
            {
                frame.X = Position == FlyOutPosition.Left ? menuWidth : -menuWidth;
                frame.Width = frame.Width - frame.X;
            }
            SetLocation(frame);
            View.AddSubview(MainView);
            AddChildViewController(CurrentViewController);
            HideMenu();
            SelectedIndexChanged?.Invoke();
        }

        private void ShowMenu()
        {
            if (MainView == null) return;

            EnsureInvokedOnMainThread(delegate
            {
                leftNavigation.View.Hidden = false;
                closeButton.Frame = MainView.Frame;
                shadowView.Frame = MainView.Frame;
                var statusFrame = statusImage.Frame;
                statusFrame.X = MainView.Frame.X;
                statusImage.Frame = statusFrame;
                View.AddSubview(closeButton);

                if (!HideShadow) View.InsertSubviewBelow(shadowView, MainView);

                UIView.BeginAnimations("slideMenu");
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseIn);
                UIView.SetAnimationDuration(.3);
                SetViewSize();
                CGRect frame = MainView.Frame;
                frame.X = Position == FlyOutPosition.Left ? menuWidth : -menuWidth;
                SetLocation(frame);
                SetViewSize();
                frame = MainView.Frame;
                shadowView.Frame = frame;
                closeButton.Frame = frame;
                statusFrame.X = MainView.Frame.X;
                statusImage.Frame = statusFrame;
                UIView.CommitAnimations();
                isOpen = true;
            });
        }

        private void SetViewSize()
        {
            CGRect frame = View.Bounds;
            if (isOpen) frame.Width -= menuWidth;
            if (MainView.Bounds == frame) return;

            MainView.Bounds = frame;
        }

        private void SetLocation(CGRect frame)
        {
            MainView.Layer.AnchorPoint = new CGPoint(.5f, .5f);
            frame.Y = 0;
            if (MainView.Frame.Location == frame.Location) return;

            frame.Size = MainView.Frame.Size;
            var center = new CGPoint(frame.Left + frame.Width / 2, frame.Top + frame.Height / 2);
            MainView.Center = center;
            shadowView.Center = center;

            if (Math.Abs(frame.X - 0) > float.Epsilon)
            {
                GetStatus();
                var statusFrame = statusImage.Frame;
                statusFrame.X = MainView.Frame.X;
                statusImage.Frame = statusFrame;
            }
        }

        private bool DisableStatusBarMoving { get; set; }

        private void GetStatus()
        {
            if (DisableStatusBarMoving || !isIos7 || statusImage.Superview != null) return;

            var image = CaptureStatusBarImage();
            if (image == null) return;

            View.AddSubview(statusImage);
            foreach (var view in statusImage.Subviews) view.RemoveFromSuperview();

            statusImage.AddSubview(image);
            statusImage.Frame = UIApplication.SharedApplication.StatusBarFrame;
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        private UIView CaptureStatusBarImage()
        {
            try
            {
                UIView screenShot = UIScreen.MainScreen.SnapshotView(false);
                return screenShot;
            }
            catch { return null; }
        }

        private void HideStatus()
        {
            if (!isIos7) return;

            statusImage.RemoveFromSuperview();
            UIApplication.SharedApplication.StatusBarHidden = false;
        }

        private void HideMenu()
        {
            if (MainView == null) return;

            EnsureInvokedOnMainThread(delegate
            {
                isOpen = false;
                leftNavigation.FinishSearch();
                closeButton.RemoveFromSuperview();
                shadowView.Frame = MainView.Frame;
                var statusFrame = statusImage.Frame;
                statusFrame.X = MainView.Frame.X;
                statusImage.Frame = statusFrame;
                UIView.Animate(.5, () =>
                    {
                        UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                        CGRect frame = View.Bounds;
                        frame.X = 0;
                        SetViewSize();
                        SetLocation(frame);
                        shadowView.Frame = frame;
                        statusFrame.X = 0;
                        statusImage.Frame = statusFrame;
                    }, HideComplete);
            });
        }

        [Export("animationEnded")]
        private void HideComplete()
        {
            HideStatus();
            shadowView.RemoveFromSuperview();
            leftNavigation.View.Hidden = true;
        }

        private void ResignFirstResponders(UIView view)
        {
            if (view.Subviews == null) return;

            foreach (UIView subview in view.Subviews)
            {
                if (subview.IsFirstResponder) subview.ResignFirstResponder();
                ResignFirstResponders(subview);
            }
        }

        public void ToggleMenu()
        {
            EnsureInvokedOnMainThread(delegate
            {
                if (!IsOpen && CurrentViewController != null && CurrentViewController.IsViewLoaded)
                {
                    ResignFirstResponders(CurrentViewController.View);
                }

                if (IsOpen) HideMenu();
                else ShowMenu();
            });
        }

        private int GetIndex(NSIndexPath indexPath)
        {
            int section = 0;
            int rowCount = 0;
            while (section < indexPath.Section)
            {
                rowCount += leftNavigation.Root[section].Count;
                section++;
            }
            return rowCount + indexPath.Row;
        }

        private NSIndexPath GetIndexPath(int index)
        {
            if (leftNavigation.Root == null) return NSIndexPath.FromRowSection(0, 0);

            int currentCount = 0;
            int section = 0;
            foreach (Section element in leftNavigation.Root)
            {
                if (element.Count + currentCount > index) break;

                currentCount += element.Count;
                section++;
            }

            int row = index - currentCount;
            return NSIndexPath.FromRowSection(row, section);
        }

        private void EnsureInvokedOnMainThread(Action action)
        {
            if (NSThread.Current.IsMainThread)
            {
                action();
                return;
            }
            BeginInvokeOnMainThread(action);
        }
    }

    internal static class Helpers
    {
        static readonly IntPtr selAccessibilityIdentifier_Handle = Selector.GetHandle("accessibilityIdentifier");
        public static UIView SetAccessibilityId(this UIView view, string id)
        {
            var nsId = NSString.CreateNative(id);
            return view;
        }
    }

    internal class OpenMenuGestureRecognizer : UIPanGestureRecognizer
    {
        public OpenMenuGestureRecognizer(Action<UIPanGestureRecognizer> callback, Func<UIGestureRecognizer, UITouch, bool> shouldReceiveTouch)
            : base(callback)
        {
            ShouldReceiveTouch += (sender, touch) =>
            {
                bool isMovingCell = touch.View.ToString().IndexOf("UITableViewCellReorderControl", StringComparison.InvariantCultureIgnoreCase) > -1;
                if (touch.View is UISlider || touch.View is MPVolumeView || isMovingCell) return false;
                return shouldReceiveTouch(sender, touch);
            };
        }
    }
}