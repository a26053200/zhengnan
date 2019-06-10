using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/8 1:17:22</para>
    /// </summary> 
    public abstract class Resource 
    {
        public bool isUsedOpen = true;

        public string name;
        public string path;
        public int hashCode;
        public Object resObj { get; protected set; }
        public FileInfo fileInfo;

        public int errorNum;
        public int warnNum;

        public virtual void SetResObj(Object obj)
        {
            resObj = obj;
        }

        public abstract void OnResourceGUI();
    }
}

