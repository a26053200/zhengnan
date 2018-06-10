using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
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
#if UNITY_EDITOR
            GameObject prefab = LoadAsset_Editor<GameObject>(assetName);
#endif
            return prefab;
        }

        /// <summary>
        /// 编辑器环境下加载资源
        /// </summary>
        /// <returns></returns>
        T LoadAsset_Editor<T>(string path) where T : UnityEngine.Object
        {
            T tempTex = AssetDatabase.LoadAssetAtPath("Assets/" + path, typeof(T)) as T;
            return tempTex;
        }
    }
}

