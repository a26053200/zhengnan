using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// <para>游戏管理</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:18:45</para>
/// </summary> 
/// 
namespace Framework
{
    public class GameManager : BaseManager
    {
        static Dictionary<string, BaseManager> s_mgrDict;

        public static void AddManager(BaseManager mgr)
        {
            if (s_mgrDict == null)
                s_mgrDict = new Dictionary<string, BaseManager>();
            s_mgrDict.Add(mgr.GetType().Name, mgr);
            mgr.Initialize();
        }

        static T GetManager<T>() where T : BaseManager
        {
            T t = default(T);
            return s_mgrDict[typeof(T).Name] as T;
        }

        public static GameManager GetGameManager()
        {
            return GetManager<GameManager>();
        }

        public static AssetsManager GetAssetsManager()
        {
            return GetManager<AssetsManager>();
        }

        public static SceneManager GetSceneManager()
        {
            return GetManager<SceneManager>();
        }

        public static MonoBehaviourManager GetMonoBehaviourManager()
        {
            return GetManager<MonoBehaviourManager>();
        }

        public static NetworkManager GetNetworkManager()
        {
            return GetManager<NetworkManager>();
        }

        public static ResLoader GetResLoader()
        {
            return GetManager<ResLoader>();
        }

        /// <summary>
        /// 设置分辨率（竖屏游戏，以宽度为基准）
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        public void SetResolution(int maxWidth)
        {
            Resolution[] resolutions = Screen.resolutions;
            //获取设备的分辨率
            float deviceWidth, deviceHeight;
            if (resolutions.Length > 0)
            {
                deviceWidth = resolutions[resolutions.Length - 1].width;
                deviceHeight = resolutions[resolutions.Length - 1].height;
            }
            else
            {
                deviceWidth = Screen.width;
                deviceHeight = Screen.height;
            }
            Debug.LogFormat("Device Width {0}, screen width {1}, maxWidth {2}, Resolution Length {3}",deviceWidth,Screen.width,maxWidth,resolutions.Length);
            //当设备分辨率大于最大指定分辨率时才改变
            if (deviceWidth > maxWidth)
            {
                float scale = maxWidth / deviceWidth;
                int height = Mathf.CeilToInt(deviceHeight * scale);
                int width = Mathf.CeilToInt(maxWidth);
                Screen.SetResolution(width, height, false);
                Debug.LogFormat("Set new resolution {0}x{1}, scale {2}", width, height, scale);
            }
        }

    }
}

