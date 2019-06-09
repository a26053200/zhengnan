using System;
using UnityEngine;
using System.Collections.Generic;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/7 1:01:11</para>
    /// </summary> 
    public class ResourceDetail
    {
        public bool isOpen = false;
        public bool isClick = false;

        public string Name;
        public string MD5;
        public List<Resource> resources;

        public ResourceDetail(string md5, string name)
        {
            MD5 = md5;
            Name = name;
        }
    }
}

