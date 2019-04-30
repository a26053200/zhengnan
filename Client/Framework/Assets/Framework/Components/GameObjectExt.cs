using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// <para>GameObject Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/7/13 0:26:52</para>
/// </summary> 
public static class GameObjectExt
{
    //充值Transorm
    public static void ResetTransform(this GameObject obj)
    {
        if (obj)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }
    }

    //添加或者获得组件
    public static Component GetOrAddComponent(this GameObject obj, System.Type t)
    {
        Component cpt = obj.GetComponent(t);
        if (null == cpt)
        {
            cpt = obj.AddComponent(t);
        }
        return cpt;
    }

    //获取 RectTransform 组件
    public static RectTransform GetRect(this GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Debug.Log("RectTransform GetRectRectTransform GetRectRectTransform GetRect");
        return rect;
    }

    //获取组件
    public static Component GetCom(this GameObject obj, string comName)
    {
        Component cpt = obj.GetComponent(comName);
        return cpt;
    }

    //获取按钮
    public static Button GetButton(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Button btn = child.GetComponent<Button>();
            return btn;
        }
        return null;
    }


    //获取组件文本
    public static Text GetText(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Text text = child.GetComponent<Text>();
            return text;
        }
        return null;
    }

    //获取文本输入组件
    public static InputField GetInputField(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            InputField com = child.GetComponent<InputField>();
            return com;
        }
        return null;
    }

    //获取组件Image
    public static Image GetImage(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Image img = child.GetComponent<Image>();
            return img;
        }
        return null;
    }

    //获取组件Slider
    public static Slider GetSlider(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Slider slider = child.GetComponent<Slider>();
            return slider;
        }
        return null;
    }

    //获取组件CanvasGroup
    public static CanvasGroup GetCanvasGroup(this GameObject gameObject, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            return canvasGroup;
        }
        return null;
    }

    //设置组件Image Alpha
    public static void SetImageAlpha(this GameObject gameObject, float alpha, string path = null)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Color col = child.GetComponent<Image>().color;
            col.a = alpha;
            child.GetComponent<Image>().color = col;
        }
    }

    //设置按钮文本
    public static void SetButtonText(this GameObject gameObject, string path, string label)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            child.GetText("Text").text = label;
        }
    }

    //按节点路径查找子节点
    public static GameObject FindChild(this GameObject gameObject, string path)
    {
        if (string.IsNullOrEmpty(path))
            return gameObject;
        else
        {
            Queue<string> childQueue = new Queue<string>(path.Split('/'));
            Transform findChild = null;
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
            {
                if (childQueue.Peek() == child.name)
                {
                    childQueue.Dequeue();
                    if (childQueue.Count == 0)
                    {
                        findChild = child;
                        break;
                    }
                }
            }
            return findChild ? findChild.gameObject : null;
        }
    }

    //获取当前节点的完整路径
    public static string Path(this GameObject gameObject)
    {
        var path = "/" + gameObject.name;
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
            path = "/" + gameObject.name + path;
        }
        return path;
    }

    //获取根节点
    public static GameObject Root(this GameObject go)
    {
        var current = go;
        GameObject result;
        do
        {
            var trans = current.transform.parent;
            if (trans != null)
            {
                result = trans.gameObject;
                current = trans.gameObject;
            }
            else
            {
                result = current;
                current = null;
            }
        } while (current != null);
        return result;
    }

    //获取节点层级深度
    public static int Depth(this GameObject go)
    {
        var depth = 0;
        var current = go.transform;
        do
        {
            current = current.transform.parent;
            if (current != null)
            {
                depth++;
            }
        } while (current != null);
        return depth;
    }

    public static void SetLayer(this GameObject gameObject, string layer)
    {
        gameObject.layer = 1 << LayerMask.GetMask(layer);
    }

    /// <summary>
    /// 设置该节点及其所有子节点层级
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    public static void SetLayerRecursion(this GameObject gameObject, string layer)
    {
        gameObject.layer = 1 << LayerMask.GetMask(layer);
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursion(child.gameObject, layer);
        }
    }

    /// <summary>
    /// 设置该节点及其所有子节点tag
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    public static void SetTagRecursion(this GameObject gameObject, string tag)
    {
        gameObject.tag = tag;
        foreach (Transform child in gameObject.transform)
        {
            SetTagRecursion(child.gameObject, tag);
        }
    }
}

