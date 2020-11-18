using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public static class MatroxApplicationHelper
    {
        private static MIL_ID _application = -1;
        public static MIL_ID Application => _application;
        
        public static void Initilize()
        {
            if (_application > 0)
                return;

            MIL.MappAlloc("M_DEFAULT", MIL.M_DEFAULT, ref _application);
        }

        public static void Dispose()
        {
            if (_application < 0)
                return;

            MIL.MappFree(_application);
        }
    }
}
