using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// <para>游戏管理</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:18:45</para>
/// </summary> 
/// 
namespace Framework
{
    public class SceneManager : BaseManager
    {
        /// <summary>
		///  异步方式加载场景
		/// </summary>
		/// <param name="sceneName">加载的场景名称</param>
		/// <param name="func">加载完成Lua回调函数</param>
		public void LoadSceneAsync(string sceneName, LuaFunction func)
        {
            StartCoroutine(OnLoadSceneAnsyn(sceneName, func, LoadSceneMode.Single));
        }

        /// <summary>
        /// 协程方式异步加载场景
        /// </summary>
        /// <param name="sceneName">加载的场景名称</param>
        /// <param name="func">加载完成Lua回调函数</param>
        IEnumerator OnLoadSceneAnsyn(string sceneName, LuaFunction func, LoadSceneMode mode)
        {
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            yield return operation;
            if (func != null)
            {
                func.Call(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));
            }
        }
        /// <summary>
        /// 获取当前活动的场景对象
        /// </summary>
        /// <returns>The active scene.</returns>
        public Scene GetActiveScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        /// <summary>
		///  异步方式加载子场景
		/// </summary>
		/// <param name="sceneName">加载的场景名称</param>
		/// <param name="func">加载完成Lua回调函数</param>
		public void LoadSubSceneAsync(string sceneName, LuaFunction func)
        {
            StartCoroutine(OnLoadSceneAnsyn(sceneName, func, LoadSceneMode.Additive));
        }


        /// <summary>
        ///  异步方式卸载子场景
        /// </summary>
        /// <param name="sceneName">加载的场景名称</param>
        /// <param name="func">加载完成Lua回调函数</param>
        public void UnloadSubSceneAsync(string sceneName, LuaFunction func)
        {
            StartCoroutine(OnUnloadSceneAnsyn(sceneName, func));
        }


        /// <summary>
        /// 协程方式异步卸载场景
        /// </summary>
        /// <param name="sceneName">加载的场景名称</param>
        /// <param name="func">加载完成Lua回调函数</param>
        IEnumerator OnUnloadSceneAnsyn(string sceneName, LuaFunction func)
        {
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            yield return operation;
            if (func != null)
            {
                func.Call(sceneName);
            }
        }
        /// <summary>
        /// 查找指定场景的所有顶级对象
        /// </summary>
        public GameObject FindRootObjInScene(Scene scene, string name)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            for (int i = 0; i < rootObjects.Count; i++)
            {
                if (rootObjects[i].name == name)
                {
                    return rootObjects[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 遍历所有顶级对象，调用Lua函数
        /// </summary>
        public void ForEachRootObj(Scene scene, LuaFunction luaFunc)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            for (int i = 0; i < rootObjects.Count; i++)
            {
                luaFunc.BeginPCall();
                luaFunc.Push(rootObjects[i]);
                luaFunc.PCall();
                luaFunc.EndPCall();
            }
        }
    }
}

