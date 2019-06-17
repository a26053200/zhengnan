﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 22:35:05</para>
    /// </summary> 
    public class BundleFileInfo
    {
        public string name = "";
        public string path = "";
        public string abName = "";
        public bool packDependencies;           //是否包含整个依赖
        public CompressType compressType;       //压缩类型
        public uint crc = 0;
        public long size = -1;
        public int priority;
    }
}
    
