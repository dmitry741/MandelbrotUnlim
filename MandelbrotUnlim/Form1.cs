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

        private int _zoom = 0;
        private readonly double cStep = 10;

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

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (_zoom >= 0)
            {
                LongFloat dx = (_xmax - _xmin) * LongFloat.FromDouble(1.0 / cStep);
                LongFloat dy = (_ymax - _ymin) * LongFloat.FromDouble(1.0 / cStep);

                _xmin += dx;
                _xmax -= dx;
                _ymin += dy;
                _ymax -= dy;

                _zoom++;
            }
            else
            {
                LongFloat csOne = LongFloat.FromDouble(cStep + 1);
                LongFloat div = LongFloat.FromDouble(1.0 / (cStep + 2));

                LongFloat xmax = (csOne * _xmax + _xmin) * div;
                LongFloat xmin = (csOne * _xmin + _xmax) * div;
                LongFloat ymax = (csOne * _ymax + _ymin) * div;
                LongFloat ymin = (csOne * _ymin + _ymax) * div;

                _xmin = xmin;
                _xmax = xmax;
                _ymin = ymin;
                _ymax = ymax;

                _zoom--;
            }

            UpdateFractal();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (_zoom <= 0)
            {
                LongFloat dx = (_xmax - _xmin) * LongFloat.FromDouble(1.0 / cStep);
                LongFloat dy = (_ymax - _ymin) * LongFloat.FromDouble(1.0 / cStep);

                _xmin -= dx;
                _xmax += dx;
                _ymin -= dy;
                _ymax += dy;

                _zoom--;
            }
            else
            {
                LongFloat csOne = LongFloat.FromDouble(cStep - 1);
                LongFloat div = LongFloat.FromDouble(1.0 / (cStep - 2));

                LongFloat xmax = (csOne * _xmax - _xmin) * div;
                LongFloat xmin = (csOne * _xmin - _xmax) * div;
                LongFloat ymax = (csOne * _ymax - _ymin) * div;
                LongFloat ymin = (csOne * _ymin - _ymax) * div;

                _xmin = xmin;
                _xmax = xmax;
                _ymin = ymin;
                _ymax = ymax;

                _zoom++;
            }

            UpdateFractal();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            LongFloat lfStep = LongFloat.FromDouble(1.0 / cStep);
            LongFloat s = (_xmax - _xmin) * lfStep;

            _xmax -= s;
            _xmin -= s;

            UpdateFractal();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            LongFloat lfStep = LongFloat.FromDouble(1.0 / cStep);
            LongFloat s = (_xmax - _xmin) * lfStep;

            _ymax += s;
            _ymin += s;

            UpdateFractal();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            LongFloat lfStep = LongFloat.FromDouble(1.0 / cStep);
            LongFloat s = (_xmax - _xmin) * lfStep;

            _ymax -= s;
            _ymin -= s;

            UpdateFractal();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            LongFloat lfStep = LongFloat.FromDouble(1.0 / cStep);
            LongFloat s = (_xmax - _xmin) * lfStep;

            _xmax += s;
            _xmin += s;

            UpdateFractal();
        }
    }
}
