using Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UGUIUIEditor
{
    [MenuItem("GameObject/UI/List View")]
	static void CreateListView()
    {
        GameObject listViewObj = new GameObject("ListView", typeof(RectTransform));
        RectTransform rect = listViewObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 200);
        Image img = listViewObj.AddComponent<Image>();
        Mask mask = listViewObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        GameObject contentObj = new GameObject("Content",typeof(RectTransform));
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentObj.transform.SetParent(listViewObj.transform);
        contentObj.transform.localScale = Vector3.one;
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(200, 200);
        ListView listView = contentObj.AddComponent<ListView>();
        ScrollRect scroll = listViewObj.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.content = listView.GetComponent<RectTransform>();
        scroll.viewport = listViewObj.GetComponent<RectTransform>();
        if (Selection.activeObject)
        {
            listViewObj.transform.SetParent((Selection.activeObject as GameObject).transform);
            listViewObj.transform.localScale = Vector3.one;
        }
    }

    [MenuItem("GameObject/UI/Scroll List")]
    static void CreateScrollList()
    {
        GameObject listViewObj = new GameObject("ScrollList", typeof(ScrollList));
        GameObject content = new GameObject("Content");
        content.transform.SetParent(listViewObj.transform);
        if (Selection.activeObject)
            listViewObj.transform.SetParent((Selection.activeObject as GameObject).transform);
    }
    [MenuItem("GameObject/UI/Button Normal")]
    static void CreateNormalButton()
    {
        CreateButton(new Vector2(128, 32), new Color(13f / 255f, 95f / 255f, 68f / 255f, 1));
    }

    [MenuItem("GameObject/UI/Button Small")]
    static void CreateSmallButton()
    {
        CreateButton(new Vector2(100, 24), new Color(6f / 255f, 47f / 255f, 34f / 255f, 1));
    }

    [MenuItem("GameObject/UI/Title Panel")]
    static void CreateTitlePanel()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Prefabs/UI/Common/TitlePanel.prefab");
        GameObject panel = GameObject.Instantiate(prefab);
        panel.name = "TitlePanel";
        if (Selection.activeObject)
        {
            panel.transform.SetParent((Selection.activeObject as GameObject).transform);
            panel.transform.localScale = Vector3.one;
        }
    }

    static void CreateButton(Vector2 size,Color color)
    {
        GameObject btnObj = new GameObject("Button", typeof(Image));
        Image btnImg = btnObj.GetComponent<Image>();
        btnImg.color = color;
        Button btn = btnObj.AddComponent<Button>();
        Text btnText = CreateText(Color.white, "button");
        btnText.transform.SetParent(btnObj.transform);
        if (Selection.activeObject)
            btnObj.transform.SetParent((Selection.activeObject as GameObject).transform);
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        btnRect.sizeDelta = size;
        btn.gameObject.transform.localScale = Vector3.one;

        RectTransform textRect = btnText.GetComponent<RectTransform>();
        btnText.alignment = TextAnchor.MiddleCenter;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMax = Vector3.one;
        textRect.anchorMin = Vector2.zero;
        textRect.pivot = new Vector2(0.5f, 0.5f);
    }
    static Text CreateText(Color color, string content = "new text", int size = 24)
    {
        GameObject textObj = new GameObject("Text", typeof(Text));
        Text text = textObj.GetComponent<Text>();
        //text.alignment = TextAnchor.MiddleCenter;
        text.text = content;
        text.fontSize = size;
        text.color = color;
        return text;
    }
}
