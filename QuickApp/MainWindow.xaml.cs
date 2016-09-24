using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using MahApps.Metro.Controls;
using System.Globalization;
using QuickApp.ViewModel;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Informer.Core;

namespace QuickApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel Model { get; set; }

        public static readonly RoutedEvent OpenMainWindowEvent = EventManager.RegisterRoutedEvent("OpenMainWindow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindow));
        public static readonly RoutedEvent CloseMainWindowEvent = EventManager.RegisterRoutedEvent("CloseMainWindow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindow));

        public double ScreenWidth { get { return System.Windows.Forms.Screen.AllScreens.Last().WorkingArea.X + System.Windows.Forms.Screen.AllScreens.Last().WorkingArea.Width; } }
        public double ScreenHeight { get { return System.Windows.Forms.Screen.AllScreens.Last().WorkingArea.Height; } }
        public double MainWindowsLeftClosed { get { return ScreenWidth; } }
        public double MainWindowsLeftOpened { get { return ScreenWidth - Model.State.WindowWidth; } }

        private static MainWindow _Instance;
        public static MainWindow Instance
        {
            get
            {
                return _Instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public enum GlobalWindowState
        {
            Undefined,
            Closing,
            Closed,
            Opening,
            Opened
        }

        public MainWindow()
        {
            InitializeComponent();

            Model = this.DataContext as MainWindowViewModel;

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            MainWindow._Instance = this;

            this.AllowsTransparency = true;
            this.Topmost = true;
            this.ShowInTaskbar = false;

            this.Height = ScreenHeight;
            //this.Width = 0;
            this.Top = 0;
            this.Left = ScreenWidth;

            Thread monitorMouseThread = new Thread(() => MonitorMouseThread());
            monitorMouseThread.IsBackground = true;
            monitorMouseThread.Start();

            var launchWindow = new LaunchWindow();
            launchWindow.Show();
        }

        public void MonitorMouseThread()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (System.Windows.Forms.Cursor.Position.X >= ScreenWidth - 1 && System.Windows.Forms.Cursor.Position.Y > ScreenHeight * 0.3)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.CurrentWindowState = GlobalWindowState.Opening;
                    });
                }
                else if ((this.CurrentWindowState == GlobalWindowState.Opened || this.CurrentWindowState == GlobalWindowState.Opening) && System.Windows.Forms.Cursor.Position.X < ScreenWidth - Model.State.WindowWidth)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.CurrentWindowState = GlobalWindowState.Closing;
                    });
                }
            }

            /*Thread.Sleep(1000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.CurrentWindowState = GlobalWindowState.Opening;
                
            });*/
        }

        public GlobalWindowState _CurrentWindowState = GlobalWindowState.Undefined;
        public GlobalWindowState CurrentWindowState
        {
            get
            {
                return _CurrentWindowState;
            }

            set
            {
                try
                {
                    switch (value)
                    {
                        case GlobalWindowState.Opening:
                            {
                                if (this.CurrentWindowState == GlobalWindowState.Closed || this.CurrentWindowState == GlobalWindowState.Undefined)
                                {
                                    lock (this)
                                    {
                                        Console.WriteLine("Opening");
                                        this._CurrentWindowState = GlobalWindowState.Opened;
                                        WindowUtilties.AnimateWindowSize(this, Model.State.WindowWidth, MainWindowsLeftOpened);
                                        RoutedEventArgs newEventArgs = new RoutedEventArgs(OpenMainWindowEvent);
                                        RaiseEvent(newEventArgs);
                                        AppsSlide1.Visibility = Visibility.Visible;

                                        ButtonAutomationPeer peer = new ButtonAutomationPeer(MoveAppsSlideFrom0To1);
                                        IInvokeProvider invokeProv =
                                          peer.GetPattern(PatternInterface.Invoke)
                                          as IInvokeProvider;
                                        invokeProv.Invoke();
                                    } 
                                }
                                break;
                            }
                        case GlobalWindowState.Closing:
                            {
                                if (this.CurrentWindowState == GlobalWindowState.Opened || this.CurrentWindowState == GlobalWindowState.Undefined)
                                {
                                    lock (this)
                                    {
                                        Console.WriteLine("Closing");
                                        this._CurrentWindowState = GlobalWindowState.Closed;
                                        //WindowUtilties.AnimateWindowSize(this, 0, MainWindowsLeftClosed);
                                        RoutedEventArgs newEventArgs = new RoutedEventArgs(CloseMainWindowEvent);
                                        RaiseEvent(newEventArgs);
                                        AppsSlide1.Visibility = Visibility.Hidden;

                                        ButtonAutomationPeer peer = new ButtonAutomationPeer(MoveAppsSlideFrom1To0);
                                        IInvokeProvider invokeProv =
                                          peer.GetPattern(PatternInterface.Invoke)
                                          as IInvokeProvider;
                                        invokeProv.Invoke();
                                    }
                                }
                                break;
                            }
                        case GlobalWindowState.Opened:
                            {
                                this._CurrentWindowState = GlobalWindowState.Opened;
                                break;
                            }
                        case GlobalWindowState.Closed:
                            {
                                this._CurrentWindowState = GlobalWindowState.Closed;
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public event RoutedEventHandler OpenMainWindow
        {
            add { AddHandler(OpenMainWindowEvent, value); }
            remove { RemoveHandler(OpenMainWindowEvent, value); }
        }

        public event RoutedEventHandler CloseMainWindow
        {
            add { AddHandler(CloseMainWindowEvent, value); }
            remove { RemoveHandler(CloseMainWindowEvent, value); }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public enum SpecialWindowHandles
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static readonly DependencyProperty WindowLeftAnimationProperty = DependencyProperty.Register("WindowLeftAnimation", typeof(double),
                                                                                                    typeof(MainWindow), new PropertyMetadata(OnWindowLeftAnimationChanged));

        private static void OnWindowLeftAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;

            if (window != null)
            {
                IntPtr handle = new WindowInteropHelper(window).Handle;
                var rect = new RECT();
                if (GetWindowRect(handle, ref rect))
                {
                    rect.X = (int)(double)e.NewValue;
                    rect.Y = (int)window.Top;

                    rect.Width = (int)window.ActualWidth;
                    rect.Height = (int)window.Height;  // double casting from object to double to int

                    SetWindowPos(handle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), rect.X, rect.Y, rect.Width, rect.Height, (uint)SWP.SHOWWINDOW);
                }
            }
        }

        public double WindowLeftAnimation
        {
            get { return (double)GetValue(WindowLeftAnimationProperty); }
            set { SetValue(WindowLeftAnimationProperty, value); }
        }

        public static readonly DependencyProperty WindowWidthAnimationProperty = DependencyProperty.Register("WindowWidthAnimation", typeof(double),
                                                                                                    typeof(MainWindow), new PropertyMetadata(OnWindowWidthAnimationChanged));

        private static void OnWindowWidthAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;

            if (window != null)
            {
                IntPtr handle = new WindowInteropHelper(window).Handle;
                var rect = new RECT();
                if (GetWindowRect(handle, ref rect))
                {
                    rect.X = (int)window.Left;
                    rect.Y = (int)window.Top;
                    var width = (int)(double)e.NewValue;
                    rect.Width = width;
                    rect.Height = (int)window.ActualHeight;

                    SetWindowPos(handle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), rect.X, rect.Y, rect.Width, rect.Height, (uint)SWP.SHOWWINDOW);
                }
            }
        }

        public double WindowWidthAnimation
        {
            get { return (double)GetValue(WindowWidthAnimationProperty); }
            set { SetValue(WindowWidthAnimationProperty, value); }
        }

        /// <summary>
        /// SetWindowPos Flags
        /// </summary>
        public static class SWP
        {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        private void MyMainWindow_Closing(object sender, CancelEventArgs e)
        {
            ApplicationManager.Instance.Applications.ForEach(app => app.StopApplication());
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public static class WindowUtilties
    {
        public static void AnimateWindowSize(this Window target, double newWidth, double newLeft)
        {
            int milSec = 100;

            var sb = new Storyboard { Duration = new Duration(new TimeSpan(0, 0, 0, 0, milSec)) };

            var aniWidth = new DoubleAnimationUsingKeyFrames();
            var aniLeft = new DoubleAnimationUsingKeyFrames();

            aniWidth.Duration = new Duration(new TimeSpan(0, 0, 0, 0, milSec));
            aniLeft.Duration = new Duration(new TimeSpan(0, 0, 0, 0, milSec));

            aniLeft.KeyFrames.Add(new EasingDoubleKeyFrame(target.Left, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 00))));
            aniLeft.KeyFrames.Add(new EasingDoubleKeyFrame(newLeft, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, milSec))));
            aniWidth.KeyFrames.Add(new EasingDoubleKeyFrame(target.ActualWidth, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 00))));
            aniWidth.KeyFrames.Add(new EasingDoubleKeyFrame(newWidth, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, milSec))));

            Storyboard.SetTarget(aniLeft, target);
            Storyboard.SetTargetProperty(aniLeft, new PropertyPath(MainWindow.WindowLeftAnimationProperty));

            Storyboard.SetTarget(aniWidth, target);
            Storyboard.SetTargetProperty(aniWidth, new PropertyPath(MainWindow.WindowWidthAnimationProperty));

            sb.Children.Add(aniWidth);
            sb.Children.Add(aniLeft);

            sb.Begin();
        }
    }
}
