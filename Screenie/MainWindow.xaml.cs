using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;

namespace Screenie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public MainWindow()
        {
            HotKey _hotKey = new HotKey(Key.PrintScreen, KeyModifier.None, OnHotKeyHandler);
            this.ShowInTaskbar = false;
            InitializeComponent();
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            WindowState = WindowState.Maximized;
            this.Activate();
        }

        private static BitmapSource CopyScreen()
        {
            using (var screenBmp = new Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private void myTestKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                WindowState = WindowState.Minimized;
                return;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Bitmap screenshotBmp;
            screenshotBmp = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (System.Drawing.Graphics g= System.Drawing.Graphics.FromImage(screenshotBmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, screenshotBmp.Size);
            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = screenshotBmp.GetHbitmap();

                imageCapture.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                
            }
            finally
            {
                DeleteObject(handle);
            }
            this.Show();
        }
    }
}
