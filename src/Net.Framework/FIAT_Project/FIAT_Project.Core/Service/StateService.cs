using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Service
{
    public class StateService
    {
        public double CpuUsage => _cpuUsage.NextValue();
        public double MemoryUsage => _memoryUsage.NextValue();
        public IEnumerable<DriveInfo> DriveInfos { get; }

        PerformanceCounter _cpuUsage;
        PerformanceCounter _memoryUsage;
        
        public StateService()
        {
            _cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryUsage = new PerformanceCounter("Memory", "Available MBytes");
            DriveInfos = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed);
        }
    }
}
