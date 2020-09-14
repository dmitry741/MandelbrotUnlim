using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LongFloatLib;

namespace MandelbrotUnlim
{
    public partial class Form1 : Form
    {

        #region memebers

        private Bitmap _bitmap = null;
        private byte[] _rgbValues = null;
        private Palette _colors = null;

        const double _dxmin = -1.7;
        const double _dxmax = 1.7;
        const double _dymin = -1.7;
        const double _ydmax = 1.7;

        private LongFloat _xmin = LongFloat.FromDouble(_dxmin);
        private LongFloat _xmax = LongFloat.FromDouble(_dxmax);
        private LongFloat _ymin = LongFloat.FromDouble(_dymin);
        private LongFloat _ymax = LongFloat.FromDouble(_ydmax);

        private int _timeSpan;
        AbstractDynamicFractal _fractal = new MandelbrotFractal();

        #endregion

        #region private

        Palette CreateColdPalette()
        {
            Palette palette = new Palette();

            palette.AddBaseColor(Color.FromArgb(0, 0, 128));
            palette.AddBaseColor(Color.FromArgb(0, 0, 192));
            palette.AddBaseColor(Color.FromArgb(0, 255, 255));
            palette.AddBaseColor(Color.FromArgb(0, 255, 0));

            palette.AddBaseColor(Color.FromArgb(0, 128, 0));
            palette.AddBaseColor(Color.FromArgb(128, 128, 0));
            palette.AddBaseColor(Color.FromArgb(255, 255, 0));
            palette.AddBaseColor(Color.FromArgb(255, 0, 0));

            palette.CreatePalette();

            return palette;
        }

        Bitmap CreateBackground(int width, int height)
        {
            return (width > 0 || height > 0) ? new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb) : null;
        }

        Bitmap GetFractal()
        {
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = _bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bmpData.Stride;
            int bytes = Math.Abs(stride) * _bitmap.Height;

            FractalBitmap fbi = new FractalBitmap()
            {
                BitmapWidth = _bitmap.Width,
                BitmapHeight = _bitmap.Height,
                Xmin = _xmin,
                Xmax = _xmax,
                Ymin = _ymin,
                Ymax = _ymax
            };

            _timeSpan = fbi.GetBitmap(_fractal, _colors.Colors, stride, _rgbValues);

            System.Runtime.InteropServices.Marshal.Copy(_rgbValues, 0, bmpData.Scan0, bytes);
            _bitmap.UnlockBits(bmpData);

            return _bitmap;
        }

        void UpdateFractal()
        {
            pictureBox1.Image = GetFractal();
            lblElapsedTime.Text = string.Format("Время построения: {0} сек", _timeSpan);
        }

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _bitmap = CreateBackground(pictureBox1.Width, pictureBox1.Height);

            if (_bitmap != null)
            {
                Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
                System.Drawing.Imaging.BitmapData bmpData = _bitmap.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int stride = bmpData.Stride;
                int bytes = Math.Abs(stride) * _bitmap.Height;
                _rgbValues = new byte[bytes];
                _bitmap.UnlockBits(bmpData);
            }

            _colors = CreateColdPalette();

            UpdateFractal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _xmin = LongFloat.FromDouble(_dxmin);
            _xmax = LongFloat.FromDouble(_dxmax);
            _ymin = LongFloat.FromDouble(_dymin);
            _ymax = LongFloat.FromDouble(_ydmax);

            UpdateFractal();
        }
    }
}
