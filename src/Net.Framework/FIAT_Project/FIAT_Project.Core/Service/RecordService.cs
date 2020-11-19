using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;
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
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
        }

        public void Start()
        {
            _grabService.Grabbed += _grabService_Grabbed;
        }

        public void Stop()
        {
            foreach (var datas in dataLists)
            {
                EncodeImagesToH264(_width, _height, datas);
                datas.Clear();
            }
            
        }

        private void _grabService_Grabbed(int width, int height, byte[][] datas)
        {
            _width = width;
            _height = height;

            for (int i = 0; i < 3; i++)
            {
                dataLists[i].Add(datas[i]);
            }
        }

        private unsafe void EncodeImagesToH264(int width, int height, List<byte[]> datas)
        {
            var outputFileName = "out.MP4";
            var fps = 25;
            var sourceSize = new Size(width, height);
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_GRAY8;
            var destinationSize = sourceSize;
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (var fs = File.Open(outputFileName, FileMode.Create)) // be advise only ffmpeg based player (like ffplay or vlc) can play this file, for the others you need to go through muxing
                {
                    using (var vse = new VideoStreamEncoder(fs, fps, destinationSize, AVCodecID.AV_CODEC_ID_MPEG4))
                    {
                        var frameNumber = 0;
                        foreach (var bitmapData in datas)
                        {

                            for (int i = 0; i < bitmapData.Length; i++)
                                bitmapData[i] *= 20;

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                //var linesize = new int_array8 { [0] = width };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }

                            Console.WriteLine($"frame: {frameNumber}");
                            frameNumber++;
                        }
                    }
                }
            }
        }
    }
}
