using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class RecordService
    {
        private GrabService _grabService;

        private int _width;
        private int _height;

        private List<byte[]>[] dataLists;

        public RecordService(GrabService grabService)
        {
            _grabService = grabService;
            dataLists = new List<byte[]>[3];
            for (int i = 0; i < 3; i++)
                dataLists[i] = new List<byte[]>();
            //FFmpegBinariesHelper.RegisterFFmpegBinaries();
        }

        public void Start()
        {
            _grabService.Grabbed += _grabService_Grabbed;
        }

        public void Stop()
        {
            foreach (var datas in dataLists)
            {
               
                datas.Clear();
            }
            
        }

        private void _grabService_Grabbed(int width, int height, byte[][] datas)
        {
            _width = width;
            _height = height;
        }
    }
}
