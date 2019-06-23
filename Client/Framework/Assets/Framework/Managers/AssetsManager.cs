using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using LuaInterface;
using System.Collections;
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

        private ResLoader resLoader;

        public override void Initialize()
        {
            resLoader = GameManager.GetResLoader();
        }


        public UnityEngine.Object LoadObject(string path)
        {
            return LoadAsset<UnityEngine.Object>(path);
        }

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

        public AnimationClip LoadAnimationClip(string path)
        {
            return LoadAsset<AnimationClip>(path);
        }

        public RuntimeAnimatorController LoadAnimatorController(string path)
        {
            return LoadAsset<RuntimeAnimatorController>(path);
        }

        public Shader LoadShader(string path)
        {
            return LoadAsset<Shader>(path);
        }

        public AudioClip LoadAudioClip(string path)
        {
            return LoadAsset<AudioClip>(path);
        }
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <returns>T</returns>
        T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T t = AssetDatabase.LoadAssetAtPath(EDITOT_MODE_ROOT_PATH + path, typeof(T)) as T;
            if (t == default(T))
                Logger.LogError("Asset:'{0}' has not found", path);
            return t;
#else
            //加载bundle;
            string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
            Logger.Log("Load Asset:'{0}' ", assetPath);
            AssetBundle assetBundle = resLoader.GetBundleByAssetPath(assetPath);
            T t = assetBundle.LoadAsset<T>(assetPath);
            if(t)
            {
                Logger.Log("Asset:'{0}' has found", path);
                return t;
            }
            else
            {
                Logger.LogError("Asset:'{0}' has not found", path);
                return default(T);
            }
            
#endif
        }


        /// <summary>
        /// 异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        void LoadAssetAsyn<T>(string path, LuaFunction luaFunc) where T : UnityEngine.Object
        {
            StartCoroutine(DoLoadAssetAsynCo<T>(path, luaFunc));
        }

        IEnumerator DoLoadAssetAsynCo<T>(string path, LuaFunction luaFunc) where T : UnityEngine.Object
        {
#if UNITY_EDITOR1
            yield return new WaitForEndOfFrame();
            T t = AssetDatabase.LoadAssetAtPath(EDITOT_MODE_ROOT_PATH + path, typeof(T)) as T;
            if (t == default(T))
                Logger.LogError("Asset:'{0}' has not found", path);
            if (luaFunc != null)
            {
                luaFunc.BeginPCall();
                luaFunc.Push(t);
                luaFunc.EndPCall();
            }
#else
            //加载bundle;
            string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
            Logger.Log("Load Asset:'{0}' ", assetPath);
            yield return resLoader.LoadAssetBundleAsync(assetPath, delegate(AssetBundle assetBundle)
            {
                T t = assetBundle.LoadAsset<T>(assetPath);
                if (t)
                {
                    Logger.Log("Asset:'{0}' has found", path);
                }
                else
                {
                    Logger.LogError("Asset:'{0}' has not found", path);
                }
            });
#endif

        }

    }
}

