using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class DrawingService
    {
        public void Drawing(int width, int height, byte[][] datas, byte[] buffer)
        {
            for (int i = 0, j = 0; i < height; i++, j += 2)
            {
                Buffer.BlockCopy(datas[0], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[3], i * width, buffer, j * width + width, width);
            }

            var doubleHeight = height * 2;

            for (int i = height, j = height * 4; i < height * 2; i++, j += 2)
            {
                Buffer.BlockCopy(datas[0], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[3], i * width, buffer, j * width + width, width);
            }

            for (int i = height * 2, j = height * 8; i < height * 3; i++, j += 2)
            {
                Buffer.BlockCopy(datas[0], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[3], i * width, buffer, j * width + width, width);
            }

            for (int i = 0, j = height * 2; i < height; i++, j += 2)
            {
                Buffer.BlockCopy(datas[1], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[2], i * width, buffer, j * width + width, width);
            }
            
            for (int i = 0, j = height * 6; i < height; i++, j += 2)
            {
                Buffer.BlockCopy(datas[1], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[2], i * width, buffer, j * width + width, width);
            }

            for (int i = 0, j = height * 10; i < height; i++, j += 2)
            {
                Buffer.BlockCopy(datas[1], i * width, buffer, j * width, width);
                Buffer.BlockCopy(datas[2], i * width, buffer, j * width + width, width);
            }
        }
    }
}
