﻿using System;
using System.Threading.Tasks;

using Net.Framework.Data.ImageDatas;

namespace Net.Framework.Device.ImageDevices
{
    public interface IImageDeviceInfo : IDeviceInfo
    {

    }
    
    public abstract class ImageDeviceInfo : IImageDeviceInfo
    {
        public ImageDeviceInfo()
        {
        }
    }
}