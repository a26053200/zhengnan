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
    /// <summary>
    /// 资源卸载组别
    /// </summary>
    public enum AssetsUnloadGroup
    {
        /// <summary>
        /// 永不卸载
        /// </summary>
        Common,
        /// <summary>
        /// 当前场景卸载时卸载
        /// </summary>
        Scene,
    }
    public class AssetsManager : BaseManager
    {
        //编辑器模式下的加载根目录
        private const string EDITOT_MODE_ROOT_PATH = "Assets/Res/";

        public string LoadText(string path)
        {
            TextAsset textAsset = LoadAsset<TextAsset>(path);
            return textAsset.text;
        }

        public Sprite LoadSprite(string path)
        {
            return LoadAsset<Sprite>(path);
        }

        public Texture LoadTexture(string path)
        {
            return LoadAsset<Texture>(path);
        }

        public GameObject LoadPrefab(string path)
        {
            return LoadAsset<GameObject>(path);
        }

        public Material LoadMaterial(string path)
        {
            return LoadAsset<Material>(path);
        }
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <returns>T</returns>
        T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T tempTex = AssetDatabase.LoadAssetAtPath(EDITOT_MODE_ROOT_PATH + path, typeof(T)) as T;
            if (tempTex == default(T))
                Logger.LogError("Asset:'{0}' has not found", path);
            return tempTex;
#else
            //加载bundle
#endif
        }

    }
}

