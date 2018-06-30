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
                func.Call((object)GetActiveScene());
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
    }
}

