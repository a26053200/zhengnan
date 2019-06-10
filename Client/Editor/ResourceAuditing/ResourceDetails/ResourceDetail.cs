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

        public string Name { get; private set; }
        public string MD5 { get; private set; }
        public List<Resource> resources { get; protected set; }

        public int warnNum
        {
            get
            {
                int num = 0;
                for (int i = 0; i < resources.Count; i++)
                {
                    num += resources[i].warnNum;
                }
                return num;
            }
        }

        public int errorNum
        {
            get
            {
                int num = 0;
                for (int i = 0; i < resources.Count; i++)
                {
                    num += resources[i].errorNum;
                }
                return num;
            }
        }
        public ResourceDetail(string md5, string name)
        {
            MD5 = md5;
            Name = name;
        }
    }
}

