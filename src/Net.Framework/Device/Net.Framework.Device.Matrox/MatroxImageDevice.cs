using Matrox.MatroxImagingLibrary;
using Net.Framework.Data.ImageDatas;
using Net.Framework.Device.ImageDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Device.Matrox
{
    public class MatroxImageDeviceInfo : ImageDeviceInfo
    {
        public int BufferSize { get; set; }
        public int DigitizerNo { get; set; }
        public string DcfPath { get; set; }

        public MatroxImageDeviceInfo() : base()
        {

        }
    }

    public class MatroxImageDeviceOnGrabber : IImageDeviceOnGrabber<MatroxImageDeviceInfo, MatroxImageGrabber>
    {
        private double _frameRate;
        public double FrameRate
        {
            get
            {
                MIL.MdigInquire(_digitizer, MIL.M_PROCESS_FRAME_RATE, ref _frameRate);
                return _frameRate;
            }
        }
        
        private MatroxImageDeviceInfo _info;
        public MatroxImageDeviceInfo Info => _info;

        private MatroxImageGrabber _grabber;
        public MatroxImageGrabber Grabber => _grabber;
        
        private MIL_ID _digitizer;

        private MIL_INT _pitch;
        public int Width => (int)_pitch;

        private MIL_INT _height;
        public int Height => (int)_height;

        private MIL_INT _channels;
        public int Channels => (int)_channels;

        private MIL_DIG_HOOK_FUNCTION_PTR _processingFunctionPtr { get; set; }
        
        private GCHandle _thisHandle;

        public event Action<IImageData> DeviceGrabbed;
        MIL_ID[] _buffers;


        public MIL_INT ProcessingFunction(MIL_INT hookType, MIL_ID hookId, IntPtr hookDataPtr)
        {
            MIL_ID modifiedBufferId = MIL.M_NULL;

            if (!IntPtr.Zero.Equals(hookDataPtr))
            {
                MIL.MdigGetHookInfo(hookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref modifiedBufferId);

                var imageData = new ImageData<byte>((int)_pitch, (int)_height, (int)_channels);

                //MIL.MbufExportSequence(saveAviMerged, M_AVI_MJPG, &UserHookDataPtr->MilRedBandSubImage, 1, UserHookDataPtr->m_FrameSet, M_WRITE);
                //MIL.MbufExportSequence("1.avi", MIL.M_DEFAULT, ref modifiedBufferId, 1, MIL.M_DEFAULT, MIL.M_OPEN);
                //MIL.MbufExportSequence("1.avi", MIL.M_DEFAULT, ref modifiedBufferId, 1, MIL.M_DEFAULT, MIL.M_WRITE);

                MIL.MbufGet(modifiedBufferId, imageData.Data);

                //MIL.MbufExportSequence("d:\\1.avi", MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, MIL.M_DEFAULT, MIL.M_OPEN);
                //MIL.MbufExportSequence("d:\\1.avi", MIL.M_DEFAULT, ref modifiedBufferId, 1, MIL.M_DEFAULT, MIL.M_WRITE);
                //MIL.MbufExportSequence("d:\\1.avi", MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 20, MIL.M_CLOSE);


                DeviceGrabbed?.Invoke(imageData);
            }

            return 0;
        }

        private MIL_INT GrabFrameEnd(MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr)
        {
            
            return MIL.M_NULL;
        }

        public bool Initialize(MatroxImageDeviceInfo info, MatroxImageGrabber grabber)
        {
            try
            {
                _info = info;
                _grabber = grabber;

                _thisHandle = GCHandle.Alloc(this);

                _processingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction);

                MIL.MdigAlloc(grabber.System, _info.DigitizerNo, _info.DcfPath, MIL.M_DEFAULT, ref _digitizer);

                MIL.MdigInquire(_digitizer, MIL.M_SIZE_X, ref _pitch);
                MIL.MdigInquire(_digitizer, MIL.M_SIZE_Y, ref _height);
                MIL.MdigInquire(_digitizer, MIL.M_SIZE_BAND, ref _channels);

                MIL.MdigControl(_digitizer, MIL.M_GRAB_MODE, MIL.M_ASYNCHRONOUS);
                MIL.MdigControl(_digitizer, MIL.M_GRAB_TIMEOUT, MIL.M_INFINITE);
                
                _buffers = new MIL_ID[info.BufferSize];

                for (int i = 0; i < info.BufferSize; i++)
                {
                    MIL.MbufAllocColor(grabber.System,
                        _channels,
                        (int)_pitch,
                        (int)_height,
                        8 + MIL.M_UNSIGNED,
                        MIL.M_IMAGE + MIL.M_GRAB,
                        ref _buffers[i]);
                }
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }

            return true;
        }

        public Task<IImageData> Grab()
        {
            throw new NotImplementedException();
        }

        public bool ContinuousGrab()
        {
            try
            {
                MIL.MdigProcess(_digitizer, _buffers, _info.BufferSize, MIL.M_START, MIL.M_DEFAULT, _processingFunctionPtr, GCHandle.ToIntPtr(_thisHandle));
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }

            return true;
        }

        public bool Stop()
        {
            try
            {
                MIL.MdigProcess(_digitizer, _buffers, _info.BufferSize, MIL.M_STOP, MIL.M_DEFAULT, _processingFunctionPtr, GCHandle.ToIntPtr(_thisHandle));
                //MIL.MdigInquire(_digitizer, MIL.M_PROCESS_FRAME_RATE, ref _frameRate);
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            MIL.MdigFree(_digitizer);

            foreach (var buffer in _buffers)
                MIL.MbufFree(buffer);
        }

        public void Exposure()
        {

        }
    }
        
}
