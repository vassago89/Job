using Net.Framework.Data.ImageDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Data.Recorder
{
    public interface ICapturer<TData>
    {
        int Width { get; }
        int Height { get; }
        int Channels { get; }

        void Intialize(int width, int height, int channels);

        void Capture(TData[] datas, string path);
    }
}
