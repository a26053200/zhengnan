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

    //获取组件
    public static Component GetCom(this GameObject obj, string comName)
    {
        Component cpt = obj.GetComponent(comName);
        return cpt;
    }

    //获取组件文本
    public static Button GetButton(this GameObject gameObject, string path)
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
    public static string GetText(this GameObject gameObject, string path)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Text text = child.GetComponent<Text>();
            return text.text;
        }
        return null;
    }

    //设置组件文本
    public static void SetText(this GameObject gameObject, string path, string text)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
            child.GetComponent<Text>().text = text;
    }

    //设置输入组件文本
    public static void SetInputField(this GameObject gameObject, string path, string text)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
            child.GetComponent<InputField>().text = text;
    }

    //获取组件Sprite
    public static Sprite GetSprite(this GameObject gameObject, string path)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            Image img = child.GetComponent<Image>();
            return img.sprite;
        }
        return null;
    }

    //设置组件Sprite
    public static void SetSprite(this GameObject gameObject, string path, Sprite sprite)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
            child.GetComponent<Image>().sprite = sprite;
    }

    //设置组件Slider
    public static void SetSlider(this GameObject gameObject, string path, float value)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
            child.GetComponent<Slider>().value = value;
    }

    //获取组件Slider值
    public static float GetSlider(this GameObject gameObject, string path)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
            return child.GetComponent<Slider>().value;
        else
            return 0;
    }

    //获取组件CanvasGroup
    public static CanvasGroup GetCanvasGroup(this GameObject gameObject, string path)
    {
        GameObject child = gameObject.FindChild(path);
        if (child)
        {
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            return canvasGroup;
        }
        return null;
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
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
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

    public static void SetLayerRecursion(this GameObject gameObject, string layer)
    {
        gameObject.layer = 1 << LayerMask.GetMask(layer);
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursion(child.gameObject, layer);
        }
    }

    public static void SetTagRecursion(this GameObject gameObject, string tag)
    {
        gameObject.tag = tag;
        foreach (Transform child in gameObject.transform)
        {
            SetTagRecursion(child.gameObject, tag);
        }
    }
}

