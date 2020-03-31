//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/30 11:22:53
// 

using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using PathCreation;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Framework
{
    public static class LuaHelper
    {
        private static Vector3 tempVec3 = Vector3.zero;
        public static bool isNullObj(UnityEngine.Object obj)
        {
            return !obj;
        }
        
        public static Vector3 ScreenToCanvasPoint(Vector2 pos,Canvas uiCanvas, RectTransform parent = null)
        {
            Vector2 p = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, pos, uiCanvas.worldCamera, out p);
            tempVec3.Set(p.x, p.y, uiCanvas.GetComponent<RectTransform>().anchoredPosition3D.z);
            return tempVec3;
        }
        
        public static void AddScrollListOnItemRender(ScrollList list, LuaFunction OnItemRender)
        {
            list.onItemRender = delegate (int index, Transform item)
            {
                OnItemRender.Call(index, item);
            };
        }

        public static void AddScrollListOnScrollOver(ScrollList list, LuaFunction onScrollOver)
        {
            list.onScrollOver = delegate (int index)
            {
                onScrollOver.Call(index);
            };
        }
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

        public static void RemoveButtonClick(GameObject go)
        {
            if(go)
            {
                Button btn = go.GetComponent<Button>();
                if (btn)
                    btn.onClick.RemoveAllListeners();
            }
        }

        public static Button[] GetChildrenButtons(GameObject go, bool includeInactive = false)
        {
            Button[] btns = go.GetComponentsInChildren<Button>(includeInactive);
            return btns;
        }

        public static Component[] GetChildrenComponents(GameObject go, Type type, bool includeInactive = false)
        {
            Component[] coms = go.GetComponentsInChildren(type, includeInactive);
            return coms;
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

        public static int GetLayerMask(int layer)
        {
            return 1 << layer;
        }

        public static int Bit_Or(int bit1, int bit2)
        {
            return bit1 | bit2;
        }
        
        public static int Bit_And(int bit1, int bit2)
        {
            return bit1 & bit2;
        }
        
        public static Vector3[] GetBezierPath(PathCreator pc)
        {
            Vector3[] path = new Vector3[pc.bezierPath.NumPoints];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = pc.bezierPath[i];
            }

            return path;
        }
        /// <summary>
        /// 给GameObject 注册点击事件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="luaFunc"></param>
        /// <returns></returns>
        public static LuaFunction AddObjectClickEvent(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.PointerClick, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectPointerEnter(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.PointerEnter, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectPointerExit(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.PointerExit, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectPointerDown(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.PointerDown, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectPointerUp(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.PointerUp, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectBeginDrag(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.BeginDrag, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectDrag(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.Drag, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectDrop(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.Drop, luaFunc, passEvent);
        }
        public static LuaFunction AddObjectEndDrag(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            return EventHelper.AddObjectEvent(go, EventTriggerType.EndDrag, luaFunc, passEvent);
        }
        
        public static void RemoveObjectEvent(GameObject go, LuaFunction luaFunc, bool passEvent = false)
        {
            EventHelper.RemoveObjectEvent(go, luaFunc);
        }
        /// <summary>
        /// 判断是否按下(跨平台)
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerDown()
        {
            return Input.GetMouseButton(0) || (Input.touchCount > 0 && (int)Input.GetTouch(0).phase > -1);
        }
    }
}

