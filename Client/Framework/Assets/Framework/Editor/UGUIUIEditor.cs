using Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UGUIUIEditor
{
    [MenuItem("GameObject/UI/List View")]
	static void CreateListView()
    {
        GameObject listViewObj = new GameObject("ListView",typeof(ListView));
        if(Selection.activeObject)
            listViewObj.transform.SetParent((Selection.activeObject as GameObject).transform);
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
        CreateButton(new Vector2(160, 40), new Color(13f / 255f, 95f / 255f, 68f / 255f, 1));
    }

    [MenuItem("GameObject/UI/Button Small")]
    static void CreateSmallButton()
    {
        CreateButton(new Vector2(128, 32), new Color(6f / 255f, 47f / 255f, 34f / 255f, 1));
    }

    static void CreateButton(Vector2 size,Color color)
    {
        GameObject btnObj = new GameObject("Button", typeof(Image));
        Image btnImg = btnObj.GetComponent<Image>();
        btnImg.color = color;
        Button btn = btnObj.AddComponent<Button>();
        GameObject btnText = new GameObject("Text", typeof(Text));
        btnText.transform.SetParent(btnObj.transform);
        if (Selection.activeObject)
            btnObj.transform.SetParent((Selection.activeObject as GameObject).transform);
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        btnRect.sizeDelta = size;
        btn.gameObject.transform.localScale = Vector3.one;

        Text text = btnText.GetComponent<Text>();
        RectTransform textRect = text.GetComponent<RectTransform>();
        text.alignment = TextAnchor.MiddleCenter;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMax = Vector3.one;
        textRect.anchorMin = Vector2.zero;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        text.text = "button";
        text.fontSize = 24;
        text.color = Color.white;
    }

}
