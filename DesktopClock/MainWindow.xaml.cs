using DesktopClock.ViewModels;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
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

        private readonly MainPageViewModel _viewModel;
        private readonly NotifyIcon _notifyIcon;
        private readonly ToolStripMenuItem _windowClickableMenuItem = new("クリック有効 (&C)");
        private readonly ToolStripMenuItem _startupRegisterMenuItem = new("スタートアップ (&S)");
        private readonly ToolStripMenuItem _closeMenuItem = new("終了 (&E)");

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

            this.Loaded += OnMainWindowLoad;
        }

        private void OnMainWindowLoad(object sender, RoutedEventArgs e)
        {
            WindowClickTransparencyState(true);

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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            DragMove();

            base.OnMouseDown(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            base.OnClosing(e);
        }

        private void CreateNotifyIcon()
        {
            // コンテキストメニュー作成
            var contextMenu = new ContextMenuStrip();

            _windowClickableMenuItem.Click += (_, _) =>
            {
                if (IsWindowClickTransparent())
                {
                    WindowClickTransparencyState(false);
                    _windowClickableMenuItem.Checked = true;
                }
                else
                {
                    WindowClickTransparencyState(true);
                    _windowClickableMenuItem.Checked = false;
                }
            };

            _startupRegisterMenuItem.Click += (_, _) =>
            {
                if (_viewModel.IsStartupRegistered())
                {
                    _viewModel.UnregisterStartup();
                    _startupRegisterMenuItem.Checked = false;
                }
                else
                {
                    _viewModel.RegisterStartup();
                    _startupRegisterMenuItem.Checked = true;
                }
            };

            _closeMenuItem.Click += (_, _) => System.Windows.Application.Current.Shutdown();

            contextMenu.Items.Add(_windowClickableMenuItem);
            contextMenu.Items.Add(_startupRegisterMenuItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(_closeMenuItem);
            _notifyIcon.ContextMenuStrip = contextMenu;

            // 透過状態チェック
            _windowClickableMenuItem.Checked = IsWindowClickTransparent();

            // スタートアップ登録状態チェック
            _startupRegisterMenuItem.Checked = _viewModel.IsStartupRegistered();
        }

        private bool IsWindowClickTransparent()
        {
            var hwnd = new HWND(new WindowInteropHelper(this).Handle);
            var extendedStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            return (extendedStyle & WINDOW_EX_STYLE.WS_EX_TRANSPARENT) != 0;
        }

        private void WindowClickTransparencyState(bool enable)
        {
            var hwnd = new HWND(new WindowInteropHelper(this).Handle);
            var extendedStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            if (enable)
            {
                // ウィンドウのクリック透過を有効
                _ = PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)(extendedStyle | WINDOW_EX_STYLE.WS_EX_TRANSPARENT));
            }
            else
            {
                // ウィンドウのクリック透過を無効
                _ = PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)(extendedStyle & ~WINDOW_EX_STYLE.WS_EX_TRANSPARENT));
            }
        }
    }
}