using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
}
