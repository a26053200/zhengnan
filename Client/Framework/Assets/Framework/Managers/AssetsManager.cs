using UnityEngine;
using UnityEditor;
using LuaInterface;
using System.Collections;
using System.IO;
using UnityEngine.UI;

using Object = UnityEngine.Object;
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

        // 通用资源
        public UnityEngine.Object LoadObject(string path)
        {
            return LoadAsset<UnityEngine.Object>(path);
        }
        public void LoadObjectAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<UnityEngine.Object>(path, callback);
        }

        //文本或者二进制文件
        public string LoadText(string path)
        {
            TextAsset textAsset = LoadAsset<TextAsset>(path);
            return textAsset.text;
        }
        public void LoadTextAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<TextAsset>(path, callback);
        }


        //Texture
        public Sprite LoadSingleSprite(string path)
        {
            return LoadAsset<Sprite>(path);
        }
        public void LoadSingleSpriteAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Sprite>(path, callback);
        }
        public Texture LoadTexture(string path)
        {
            return LoadAsset<Texture>(path);
        }
        public void LoadTextureAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Texture>(path, callback);
        }
        public Texture LoadTexture2D(string path)
        {
            return LoadAsset<Texture2D>(path);
        }
        public void LoadTexture2DAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Texture2D>(path, callback);
        }
        public Texture LoadTexture3D(string path)
        {
            return LoadAsset<Texture3D>(path);
        }
        public void LoadTexture3DAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Texture3D>(path, callback);
        }
        
        public GameObject LoadPrefab(string path)
        {
            return LoadAsset<GameObject>(path);
        }
        public void LoadPrefabAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<GameObject>(path, callback);
        }

        public Mesh LoadMesh(string path)
        {
            return LoadAsset<Mesh>(path);
        }
        public void LoadMeshAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Mesh>(path, callback);
        }
        
        public Material LoadMaterial(string path)
        {
            return LoadAsset<Material>(path);
        }
        public void LoadMaterialAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Material>(path, callback);
        }

        public AnimationClip LoadAnimationClip(string path)
        {
            return LoadAsset<AnimationClip>(path);
        }
        public void LoadAnimationClipAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<AnimationClip>(path, callback);
        }

        public RuntimeAnimatorController LoadAnimatorController(string path)
        {
            return LoadAsset<RuntimeAnimatorController>(path);
        }
        public void LoadRuntimeAnimatorControllerAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<RuntimeAnimatorController>(path, callback);
        }

        public Shader LoadShader(string path)
        {
            return LoadAsset<Shader>(path);
        }
        public void LoadShaderAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<Shader>(path, callback);
        }

        public AudioClip LoadAudioClip(string path)
        {
            return LoadAsset<AudioClip>(path);
        }
        public void LoadAudioClipAsync(string path, LuaFunction callback)
        {
            LoadAssetAsync<AudioClip>(path, callback);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <returns>T</returns>
        T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
            {
                //加载bundle;
                string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
                //Logger.Info("Load Asset:'{0}' ", assetPath);
                AssetBundle assetBundle = resLoader.GetBundleByAssetPath(assetPath);
                T t = assetBundle.LoadAsset<T>(assetPath);
                if (t)
                {
                    //Logger.Info("Asset:'{0}' has loaded", path);
                    return t;
                }
                else
                {
                    Logger.LogError("Asset:'{0}' has not found", path);
                    return default(T);
                }
            }
            else
            {
#if UNITY_EDITOR
                T t = AssetDatabase.LoadAssetAtPath(EDITOT_MODE_ROOT_PATH + path, typeof(T)) as T;
                if (t == default(T))
                {
                    if(File.Exists(EDITOT_MODE_ROOT_PATH + path))
                        Logger.LogError("Asset:'{0}' has not found", path);
                    else
                        Logger.LogError("Asset:'{0}' file has not exists", path);
                }
                return t;
#else
                return null;
#endif
            }
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        void LoadAssetAsync<T>(string path, LuaFunction luaFunc) where T : UnityEngine.Object
        {
            StartCoroutine(DoLoadAssetAsyncCo<T>(path, luaFunc));
        }

        IEnumerator DoLoadAssetAsyncCo<T>(string path, LuaFunction luaFunc) where T : UnityEngine.Object
        {
            if(luaFunc == null)
            {
                Logger.LogError("Load Asset {0} Async must has callback function!", path);
                yield break;
            }
            if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
            {
                //加载bundle;
                string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
                //Logger.Info("Load Asset:'{0}' ", assetPath);
                yield return resLoader.AddLoadAssetBundleAsync(assetPath, delegate (AssetBundle assetBundle)
                {
                    if (assetBundle.isStreamedSceneAssetBundle)
                    {//场景Bundle
                        //Logger.Info("Scene bundle:'{0}' has loaded", path);
                        luaFunc.BeginPCall();
                        luaFunc.Push(Path.GetFileNameWithoutExtension(assetPath));
                        luaFunc.PCall();
                        luaFunc.EndPCall();
                    }
                    else
                    {
                        T t = assetBundle.LoadAsset<T>(assetPath);
                        if (t)
                        {
                            //Logger.Info("Asset:'{0}' has loaded", path);
                            luaFunc.BeginPCall();
                            luaFunc.Push(t);
                            luaFunc.PCall();
                            luaFunc.EndPCall();
                        }
                        else
                        {
                            Logger.LogError("Asset:'{0}' has not found", path);
                        }
                    }
                });
            }
            else if (GlobalConsts.isRunningInEditor)
            {
#if UNITY_EDITOR
                yield return new WaitForEndOfFrame();
                T t = AssetDatabase.LoadAssetAtPath(EDITOT_MODE_ROOT_PATH + path, typeof(T)) as T;
                if (t == default(T))
                    Logger.LogError("Asset:'{0}' has not found", path);
                luaFunc.BeginPCall();
                luaFunc.Push(t);
                luaFunc.PCall();
                luaFunc.EndPCall();
#endif
            }
        }

        public AsyncOperation GetBundleRequest(string path)
        {
            string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
            if (resLoader && resLoader.bundleLoader)
                return resLoader.bundleLoader.GetAssetBundleCreateRequest(assetPath);
            else 
                return null;
        }

        #region 加载 Sprite 特殊处理

        private string GetSpritePrefabPath(string path)
        {
            int len = path.LastIndexOf('/');
            string dirName = Path.GetFileName(path.Substring(0, len));
            string fileName = Path.GetFileNameWithoutExtension(path);
            string spritePrefabPath = GlobalConsts.SpritePrefabDir + dirName + "/" + fileName + ".prefab";
            return spritePrefabPath;
        }
        //Sprite
        public Sprite LoadSprite(string path)
        {
            if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
            {
                string spritePrefabPath = GetSpritePrefabPath(path);
                //Debug.LogFormat("Load Sprite Prefab: {0} -- ({1})", path, spritePrefabPath);
                GameObject prefab = LoadPrefab(spritePrefabPath);
                return prefab.GetComponent<Image>().sprite;
            }else
            {
#if UNITY_EDITOR
                Texture2D texture2D = LoadAsset<Texture2D>(path);
                return Sprite.Create(texture2D, new Rect(0,0,texture2D.width, texture2D.height), new Vector2());
#else
                return null;
#endif
            }
        }

        public void LoadSpriteAsync(string path, LuaFunction luaFunc)
        {
            if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
            {
                string spritePrefabPath = GetSpritePrefabPath(path);
                StartCoroutine(resLoader.AddLoadAssetBundleAsync(spritePrefabPath, delegate(AssetBundle assetBundle)
                {
                    GameObject prefab = assetBundle.LoadAsset<GameObject>(spritePrefabPath);
                    Sprite sp = prefab.GetComponent<Image>().sprite;

                    luaFunc.BeginPCall();
                    luaFunc.Push(sp);
                    luaFunc.PCall();
                    luaFunc.EndPCall();
                }));
                //Debug.LogFormat("Async Load Sprite Prefab: {0} -- ({1})", path, spritePrefabPath);
            }
            else
            {
#if UNITY_EDITOR
                LoadAssetAsync<Sprite>(path, luaFunc);
#endif
            }
        }
        #endregion

        public void UnloadAssetBundle(string path, bool unloadAllDependence, bool unloadAllLoadedObjects)
        {
            string assetPath = (GlobalConsts.ResRootDir + path).ToLower();
            if(resLoader && resLoader.bundleLoader)
                resLoader.bundleLoader.UnloadAssetBundle(assetPath, unloadAllDependence,unloadAllLoadedObjects);
        }
    }
}

