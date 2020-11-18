using System;

namespace Net.Framework.Data.ImageDatas
{
    public interface IImageData
    {
        int Width { get; }
        int Height { get; }
        int Channels { get; }
    }

    public class ImageData<TData> : IImageData
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int Channels { get; protected set; }

        public TData[] Data { get; }
        
        public ImageData(int width, int height, int channels)
        {
            Width = width;
            Height = height;
            Channels = channels;

            Data = new TData[width * Height * channels];
        }
    }
}