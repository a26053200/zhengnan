//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/30 11:22:53
// 

using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.UI;

namespace Framework
{
    public static class LuaHelper
    {
        public static void AddButtonClick(GameObject go, LuaFunction func)
        {
            Button btn = go.GetComponent<Button>();
            if(btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(delegate()
                {
                    func.Call(go);
                });
            }
        }

        public static void RemoveButtonClick(GameObject go, LuaFunction func)
        {
            Button btn = go.GetComponent<Button>();
            if (btn)
                btn.onClick.RemoveAllListeners();
        }

        public static Button[] GetChildrenButtons(GameObject go)
        {
            Button[] btns = go.GetComponentsInChildren<Button>();
            return btns;
        }

        public static LuaMonoBehaviour AddLuaMonoBehaviour(GameObject go, string componentName, string funName,LuaFunction func)
        {
            LuaMonoBehaviour luaMonoBehaviour = AddLuaComponent<LuaMonoBehaviour>(go, componentName);
            luaMonoBehaviour.SetBehaviour(funName, func);
            return luaMonoBehaviour;
        }

        static T AddLuaComponent<T>(GameObject go, string componentName) where T : LuaComponent
        {
            T[] olds = go.GetComponents<T>();
            for (int i = 0; i < olds.Length; i++)
            {
                if (olds[i].componentName == componentName)
                {
                    //Debug.LogError(string.Format("The LuaComponent name '{0}' has already added!", componentName));
                    return olds[i];
                }
            }
            T t = go.AddComponent<T>();
            t.componentName = componentName;
            return t;
        }
    }
}

