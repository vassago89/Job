using Matrox.MatroxImagingLibrary;
using Net.Framework.Device.ImageDevices;
using Net.Framework.Matrox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Device.Matrox
{
    public enum MatroxImageGrabberType
    {
        SOLIOS
    }
    
    public class MatroxImageGrabberInfo : IImageGrabberInfo
    {
        public int SystemNo { get; set; }
        public MatroxImageGrabberType Type { get; set; }
    }

    public class MatroxImageGrabber : IImageGrabber
    {
        MIL_ID _system = 0;
        public MIL_ID System => _system;

        private MatroxImageGrabberInfo _info;
        public IImageGrabberInfo Info => _info;

        private string GetSystemDescriptor()
        {
            switch (_info.Type)
            {
                case MatroxImageGrabberType.SOLIOS:
                    return MIL.M_SYSTEM_SOLIOS;
                default:
                    return string.Empty;
            }
        }

        public bool Initialize(IImageGrabberInfo deviceInfo)
        {
            try
            {
                if (_system > 0)
                    return false;

                _info = deviceInfo as MatroxImageGrabberInfo;
                
                if (MIL.MsysAlloc(MatroxApplicationHelper.Application, GetSystemDescriptor(), 0, MIL.M_DEFAULT, ref _system) == MIL.M_NULL)
                    return false;

                MatroxObjectPool.Add(this, 1);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        public void Dispose()
        {
            if (_system <= 0)
                return;

            MIL.MsysFree(_system);
        }
    }
}
