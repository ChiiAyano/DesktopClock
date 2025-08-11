using DesktopClock.ViewModels;
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
        private readonly MainPageViewModel _viewModel;

        public MainWindow(MainPageViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            InitializeComponent();

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
            var(width, height) = (this.Width, this.Height);

            this.Left = workingArea.Right - width - WindowOffset;
            this.Top = workingArea.Bottom - height - WindowOffset;
        }

        private void CreateNotificationIcon()
        {
            //var notification = new NotifyIcon
            //{
            //    Icon = SystemIcons.Application,
            //    Text = ,
            //}
}