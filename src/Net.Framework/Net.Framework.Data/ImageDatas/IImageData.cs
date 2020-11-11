using System;

namespace Net.Framework.Data.ImageDatas
{
    public interface IImageData
    {
        int WIdth { get; }
        int Height { get; }
    }

    public interface IImageData<T> : IImageData
    {
        T[] Data { get; }
    }
}