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
        public int SystemID { get; set; }
        public int DigitizerNo { get; set; }
        public string DcfPath { get; set; }

        public MatroxImageDeviceInfo() : base()
        {

        }
    }

    public class MatroxImageDeviceOnGrabber : IImageDeviceOnGrabber<MatroxImageDeviceInfo, MatroxImageGrabber>
    {
        private MatroxImageDeviceInfo _info;
        public MatroxImageDeviceInfo Info => _info;

        private MatroxImageGrabber _grabber;
        public MatroxImageGrabber Grabber => _grabber;

        private IntPtr _handle;
        private MIL_ID _digitizer;

        private int _pitch;
        private int _height;
        private int _channels;

        private GCHandle _thisHandle;

        public event Action<IImageData> Grabbed;
        MIL_ID[] _buffers;


        MIL_INT ProcessingFunction(MIL_INT hookType, MIL_ID hookId, IntPtr hookDataPtr)
        {
            MIL_ID modifiedBufferId = MIL.M_NULL;

            if (!IntPtr.Zero.Equals(hookDataPtr))
            {
                MIL.MdigGetHookInfo(hookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref modifiedBufferId);

                var imageData = new ImageData<byte>(_pitch, _height, _channels);

                MIL.MbufGet(modifiedBufferId, imageData.Data);

                Grabbed(imageData);
            }

            return 0;
        }

        public bool Initialize(MatroxImageDeviceInfo info, MatroxImageGrabber grabber)
        {
            try
            {
                _info = info;
                _thisHandle = GCHandle.Alloc(this);

                MIL.MdigAlloc(grabber.System, _info.DigitizerNo, _info.DcfPath, MIL.M_DEFAULT, ref _digitizer);

                MIL.MdigInquire(_digitizer, MIL.M_SIZE_X, ref _pitch);
                MIL.MdigInquire(_digitizer, MIL.M_SIZE_Y, ref _height);
                MIL.MdigInquire(_digitizer, MIL.M_SIZE_BAND, ref _channels);

                MIL.MdigControl(_digitizer, MIL.M_GRAB_MODE, MIL.M_ASYNCHRONOUS);
                MIL.MdigControl(_digitizer, MIL.M_GRAB_TIMEOUT, MIL.M_INFINITE);

                _buffers = new MIL_ID[info.BufferSize];

                for (int i = 0; i < info.BufferSize; i++)
                {
                    MIL.MbufAlloc2d(grabber.System,
                        _pitch,
                        _height,
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
                MIL.MdigProcess(_digitizer, _buffers, _info.BufferSize, MIL.M_START, MIL.M_DEFAULT, new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction), GCHandle.ToIntPtr(_thisHandle));
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
                MIL.MdigProcess(_digitizer, _buffers, _info.BufferSize, MIL.M_STOP, MIL.M_DEFAULT, new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction), GCHandle.ToIntPtr(_thisHandle));
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
    }
        
}
