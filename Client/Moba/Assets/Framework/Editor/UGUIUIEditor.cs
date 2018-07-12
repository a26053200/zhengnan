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
}
