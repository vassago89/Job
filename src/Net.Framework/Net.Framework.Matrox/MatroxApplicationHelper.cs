using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public class MatroxApplicationHelper : IDisposable
    {
        private MIL_ID _application = -1;
        public MIL_ID Application => _application;
        
        public void Initilize()
        {
            if (_application > 0)
                return;

            MIL.MappAlloc(MIL.M_DEFAULT, ref _application);
        }

        public void Dispose()
        {
            if (_application < 0)
                return;

            MIL.MappFree(_application);
        }
    }
}
