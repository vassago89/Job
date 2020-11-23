using Net.Framework.Helper.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core
{
    public class SystemConfig : Writable<SystemConfig>
    {
        public double LedMax { get; set; }
        public double Lazer660Max { get; set; }
        public double Lazer760Max { get; set; }

        public SystemConfig()
        {
            LedMax = 1.000;
            Lazer660Max = 1.500;
            Lazer760Max = 2.500;
        }
    }
}
