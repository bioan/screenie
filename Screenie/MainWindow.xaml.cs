using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Controls;

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
            CaptureScreen();
            this.Activate();
        }

        private static BitmapSource CopyScreen()
        {
            using (var screenBmp = new System.Drawing.Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = System.Drawing.Graphics.FromImage(screenBmp))
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

        private void CaptureScreen()
        {
            this.Hide();
            System.Drawing.Bitmap screenshotBmp;
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
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ImageCanvas.Background = ib;
            }
            finally
            {
                DeleteObject(handle);
            }
            this.Show();
        }
#region Selection
        private bool isDragging = false;
        private Point mouseDownPos = new Point();
        private Point mouseUpPos = new Point();

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            mouseDownPos = e.GetPosition(ImageCanvas);

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(SelectionRect, mouseDownPos.X);
            Canvas.SetTop(SelectionRect, mouseDownPos.Y);
            SelectionRect.Width = 0;
            SelectionRect.Height = 0;

            // Make the drag selection box visible.
            SelectionRect.Visibility = Visibility.Visible;

        }

        private void ImageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            SelectionRect.Visibility = Visibility.Collapsed;

            Point mouseUpPos = e.GetPosition(ImageCanvas);

            //Mask Top
            Canvas.SetLeft(MaskTop, 0);
            Canvas.SetTop(MaskTop, 0);
            MaskTop.Width = SystemParameters.PrimaryScreenWidth;
            MaskTop.Height = Math.Min(mouseUpPos.Y, mouseDownPos.Y);

            //Mask Left
            Canvas.SetLeft(MaskLeft, 0);
            Canvas.SetTop(MaskLeft, Math.Min(mouseUpPos.Y, mouseDownPos.Y));

            MaskLeft.Width = Math.Min(mouseUpPos.X, mouseDownPos.X);
            MaskLeft.Height = Math.Abs(mouseUpPos.Y - mouseDownPos.Y);

            //Mask Right
            Canvas.SetLeft(MaskRight, Math.Max(mouseUpPos.X, mouseDownPos.X));
            Canvas.SetTop(MaskRight, Math.Min(mouseUpPos.Y, mouseDownPos.Y));

            MaskRight.Width = SystemParameters.PrimaryScreenWidth - Math.Max(mouseUpPos.X, mouseDownPos.X);
            MaskRight.Height = Math.Abs(mouseUpPos.Y - mouseDownPos.Y);

            //Mask Bottom
            Canvas.SetLeft(MaskBottom, 0);
            Canvas.SetTop(MaskBottom, Math.Max(mouseUpPos.Y, mouseDownPos.Y));
            MaskBottom.Width = SystemParameters.PrimaryScreenWidth;
            MaskBottom.Height = SystemParameters.PrimaryScreenHeight - Math.Max(mouseUpPos.Y, mouseDownPos.Y);
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point mousePos = e.GetPosition(ImageCanvas);

                Canvas.SetLeft(SelectionRect, Math.Min(mousePos.X, mouseDownPos.X));
                Canvas.SetTop(SelectionRect, Math.Min(mousePos.Y, mouseDownPos.Y));

                SelectionRect.Width = Math.Abs(mousePos.X - mouseDownPos.X);
                SelectionRect.Height = Math.Abs(mousePos.Y - mouseDownPos.Y);
            }
        }
#endregion
    }
}
