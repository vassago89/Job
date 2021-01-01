using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Algorithm
{
    public static class ImageHelper
    {
        public static T[] GetDataOfROI<T>(T[] data, int stride, int xMin, int yMin, int width, int height)
        {
            var dataOfROI = new T[width * height];

            for (int srcY = yMin, dstY = 0; dstY < height; srcY++, dstY++)
            {
                Array.Copy(data, srcY * stride + xMin, dataOfROI, dstY * width, width);
            }
            
            return dataOfROI;
        }

        public static void SetROIToSource<T>(T[] dataOfSource, T[] dataOfROI, int stride, int xMin, int yMin, int width, int height)
        {
            for (int srcY = yMin, dstY = 0; dstY < height; srcY++, dstY++)
            {
                Array.Copy(dataOfROI, dstY * width, dataOfSource, srcY * stride + xMin, width);
            }
        }
    }
}
