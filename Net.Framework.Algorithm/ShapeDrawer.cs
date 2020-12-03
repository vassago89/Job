using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Algorithm
{
    public class ShapeDrawer : IDisposable
    {
        Mat _src;

        public ShapeDrawer(int width, int height, int channels)
        {
            switch (channels)
            {
                case 1:
                    _src = new Mat(new Size(width, height), MatType.CV_8U);
                    break;
                case 3:
                    _src = new Mat(new Size(width, height), MatType.CV_8UC3);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public byte[] DrawRect(byte[] data, System.Drawing.Rectangle rect, bool isFilled, byte r = 255, byte g = 255, byte b = 255)
        {
            _src.SetArray(data);
            Cv2.Rectangle(_src, new Rect(rect.X, rect.Y, rect.Width, rect.Height), new Scalar(r, g, b), -1);
            _src.GetArray(out byte[] output);

            return output;
        }

        public byte[] DrawEllipse(byte[] data, System.Drawing.Rectangle rect, bool isFilled, float angle = 0, byte r = 255, byte g = 255, byte b = 255)
        {
            _src.SetArray(data);
            Cv2.Ellipse( _src, new RotatedRect(new Point2f(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), new Size2f(rect.Width, rect.Height), angle), new Scalar(r, g, b), -1);
            _src.GetArray(out byte[] output);

            return output;
        }

        public byte[] DrawPolygon(byte[] data, IEnumerable<System.Drawing.Point> points, bool isFilled, byte r = 255, byte g = 255, byte b = 255)
        {
            var pts = new List<Point>();
            foreach (var point in points)
                pts.Add(new Point(point.X, point.Y));

            _src.SetArray(data);

            if (isFilled)
                Cv2.FillPoly(_src, new Point[][] { pts.ToArray() }, new Scalar(r, g, b));
            else
                Cv2.Polylines(_src, new Point[][] { pts.ToArray() }, true, new Scalar(r, g, b));
            
            _src.GetArray(out byte[] output);

            return output;
        }

        public void Dispose()
        {
            _src.Dispose();
        }
    }
}
