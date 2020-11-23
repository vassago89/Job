using Net.Framework.Data.ImageDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Data.Recorder
{
    public interface IRecorder<TData>
    {
        int Width { get; }
        int Height { get; }
        int Channels { get; }

        void Intialize(int width, int height, int channels);

        void Start(string path);
        void Enqueue(TData[] datas);
        void Stop(double frameRate);
    }
}
