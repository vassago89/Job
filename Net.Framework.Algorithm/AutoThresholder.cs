using Net.Framework.Algorithm.Enums;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Algorithm
{
    public class AutoThresholder
    {
        public byte[] AutoThreshold(EThresholdMethod method, byte[] data, byte[] mask = null, bool isInverse = false)
        {
            var binaryData = new byte[data.Length];
            var histo = GetHistogram(data, mask);
            switch (method)
            {
                case EThresholdMethod.Otsu:
                    return Binarize(data, Otsu(histo), isInverse);
                case EThresholdMethod.Li:
                    return Binarize(data, Li(histo), isInverse);
                case EThresholdMethod.Triangle:
                    return Binarize(data, Triangle(histo), isInverse);
            }

            return binaryData;
        }

        public byte[] Binarize(byte[] data, double value, bool isInverse = false)
        {
            var binData = new byte[data.Length];

            if (isInverse)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] > value)
                        binData[i] = byte.MaxValue;
                }
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] <= value)
                        binData[i] = byte.MaxValue;
                }
            }

            return binData;
        }

        public long[] GetHistogram(byte[] data, byte[] mask = null)
        {
            long[] histogram = new long[256];
            var size = data.Length;

            if (mask != null)
            {
                for (int i = 0; i < size; i++)
                {
                    if (mask[i] > 0)
                        histogram[data[i]]++;
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                    histogram[data[i]]++;
            }

            return histogram;
        }

        public double Li(long[] histogram)
        {
            double tolerance = 0.5f;

            long count = histogram.Sum();

            double mean = 0;
            double sum = 0;

            for (int ih = 0 + 1; ih < 256; ih++)
                sum += ih * histogram[ih];

            if (sum == 0)
                return 0;

            mean = sum / count;

            double sumBack;           /* sum of the background pixels at a given threshold */
            double sumObj;            /* sum of the object pixels at a given threshold */
            double numBack;           /* number of background pixels at a given threshold */
            double numObj;            /* number of object pixels at a given threshold */

            double meanBack;       /* mean of the background pixels at a given threshold */
            double meanObj;        /* mean of the object pixels at a given threshold */

            double newThreshold = mean;
            double oldThreshold = 0;
            double threshold = 0;
            
            do
            {
                oldThreshold = newThreshold;
                threshold = oldThreshold + 0.5f;

                /* Calculate the means of background and object pixels */

                /* Background */
                sumBack = 0;
                numBack = 0;
                for (int ih = 0; ih <= (int)threshold; ih++)
                {
                    sumBack += ih * histogram[ih];
                    numBack += histogram[ih];
                }

                meanBack = (numBack == 0 ? 0 : (sumBack / numBack));

                sumObj = 0;
                numObj = 0;
                for (int ih = (int)threshold + 1; ih < 256; ih++)
                {
                    sumObj += ih * histogram[ih];
                    numObj += histogram[ih];
                }

                meanObj = (numObj == 0 ? 0 : (sumObj / numObj));

                /* Calculate the new threshold: Equation (7) in Ref. 2 */
                double logDiff = Math.Log(meanBack) - Math.Log(meanObj);
                newThreshold = (meanBack - meanObj) / logDiff;

                /* 
                    Stop the iterations when the difference between the
                    new and old threshold values is less than the tolerance 
                */
            }
            while (Math.Abs(newThreshold - oldThreshold) > tolerance);

            return threshold;
        }


        public double Otsu(long[] histogram)
        {
            long total = histogram.Sum();

            double sum = 0;
            for (int i = 1; i < 256; ++i)
                sum += i * histogram[i];

            double sumB = 0;
            double wB = 0;
            double wF = 0;
            double mB;
            double mF;
            double max = 0.0;
            double between = 0.0;
            double threshold1 = 0.0;
            double threshold2 = 0.0;

            for (int i = 0; i < 256; i++)
            {
                wB += histogram[i];
                if (wB == 0)
                    continue;
                wF = total - wB;
                if (wF == 0)
                    break;
                sumB += i * histogram[i];
                mB = sumB / wB;
                mF = (sum - sumB) / wF;
                between = wB * wF * (mB - mF) * (mB - mF);
                if (between >= max)
                {
                    threshold1 = i;
                    if (between > max)
                    {
                        threshold2 = i;
                    }
                    max = between;
                }
            }

            return ((threshold1 + threshold2) / 2.0);
        }

        public double Triangle(long[] histogram)
        {
            int min = 0, max = 0, min2 = 0;
            double dmax = 0;

            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > 0)
                {
                    min = i;
                    break;
                }
            }
            if (min > 0) min--; // line to the (p==0) point, not to data[min]

            for (int i = 255; i > 0; i--)
            {
                if (histogram[i] > 0)
                {
                    min2 = i;
                    break;
                }
            }
            if (min2 < 255) min2++; // line to the (p==0) point, not to data[min]

            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > dmax)
                {
                    max = i;
                    dmax = histogram[i];
                }
            }
            // find which is the furthest side
            //IJ.log(""+min+" "+max+" "+min2);
            bool inverted = false;
            if ((max - min) < (min2 - max))
            {
                // reverse the histogram
                //IJ.log("Reversing histogram.");
                inverted = true;
                int left = 0;          // index of leftmost element
                int right = 255; // index of rightmost element
                while (left < right)
                {
                    // exchange the left and right elements
                    long temp = histogram[left];
                    histogram[left] = histogram[right];
                    histogram[right] = temp;
                    // move the bounds toward the center
                    left++;
                    right--;
                }
                min = 255 - min2;
                max = 255 - max;
            }

            if (min == max)
            {
                //IJ.log("Triangle:  min == max.");
                return min;
            }

            // describe line by nx * x + ny * y - d = 0
            double nx, ny, d;
            // nx is just the max frequency as the other point has freq=0
            nx = histogram[max];   //-min; // data[min]; //  lowest value bmin = (p=0)% in the image
            ny = min - max;
            d = Math.Sqrt(nx * nx + ny * ny);
            nx /= d;
            ny /= d;
            d = nx * min + ny * histogram[min];

            // find split point
            int split = min;
            double splitDistance = 0;
            for (int i = min + 1; i <= max; i++)
            {
                double newDistance = nx * i + ny * histogram[i] - d;
                if (newDistance > splitDistance)
                {
                    split = i;
                    splitDistance = newDistance;
                }
            }
            split--;

            if (inverted)
            {
                // The histogram might be used for something else, so let's reverse it back
                int left = 0;
                int right = 255;
                while (left < right)
                {
                    long temp = histogram[left];
                    histogram[left] = histogram[right];
                    histogram[right] = temp;
                    left++;
                    right--;
                }
                return (255 - split);
            }
            else
                return split;
        }
    }
}