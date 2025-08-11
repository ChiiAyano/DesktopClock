using DesktopClock.ViewModels;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DesktopClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WindowOffset = 20;
        private static readonly CompositeDisposable CompositeDisposable = new();

        private readonly MainPageViewModel _viewModel;
        private readonly NotifyIcon _notifyIcon;

        public MainWindow(MainPageViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            InitializeComponent();

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = _viewModel.WindowTitle,
                Visible = true
            };

            CreateNotifyIcon();
            InitializeObservable();

            this.Loaded += OnMainWindowLoad;
        }

        private void OnMainWindowLoad(object sender, RoutedEventArgs e)
        {
            var hwnd = new HWND(new WindowInteropHelper(this).Handle);
            var extendedStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            // ウィンドウに対するクリック操作を透過
            _ = PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)(extendedStyle | WINDOW_EX_STYLE.WS_EX_TRANSPARENT));

            // ウィンドウを右下に配置
            var screen = Screen.PrimaryScreen;
            if (screen is null)
            {
                return;
            }

            var workingArea = screen.WorkingArea;
            var (width, height) = (this.Width, this.Height);

            this.Left = workingArea.Right - width - WindowOffset;
            this.Top = workingArea.Bottom - height - WindowOffset;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositeDisposable.Dispose();

            base.OnClosed(e);
        }

        private void InitializeObservable()
        {
            _viewModel.MessageStream
                .Subscribe(message =>
                {
                    if (message == MessengerMessage.HideWindowFrame)
                    {
                        HideWindowFrame();
                    }
                }).AddTo(CompositeDisposable);
        }

        private void CreateNotifyIcon()
        {
            // コンテキストメニュー作成
            var contextMenu = new ContextMenuStrip();
            var showWindowFrameItem = new ToolStripMenuItem("ウィンドウフレームを表示 (&S)");
            var closeMenuItem = new ToolStripMenuItem("終了 (&E)");

            showWindowFrameItem.Click += (_, _) => ShowWindowFrame();

            closeMenuItem.Click += (_, _) => System.Windows.Application.Current.Shutdown();

            contextMenu.Items.Add(showWindowFrameItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(closeMenuItem);
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowWindowFrame()
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            Show();
        }

        private void HideWindowFrame()
        {
            this.ShowInTaskbar = false;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
        }
    }
}