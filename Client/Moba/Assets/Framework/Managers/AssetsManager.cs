using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>资源管理,资源加载和缓存</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 20:43:16</para>
/// </summary> 
/// 

namespace Framework
{
    public class AssetsManager : BaseManager
    {
        public GameObject LoadPrefab(string assetName)
        {
            GameObject prefab = Resources.Load<GameObject>(assetName);
            return prefab;
        }
    }
}

