using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/7 0:57:32</para>
    /// </summary> 
    public class ResourceTree<T> where T : ResourceDetail
    {
        //变量
        Dictionary<string, T> allResourceDetailDict;
        List<string> allAssetsPaths;

        Vector2 scrollerPos = Vector2.zero;

        public ResourceTree(Dictionary<string, T> allResourceDetailDict, List<string> allAssetsPaths)
        {
            this.allResourceDetailDict = allResourceDetailDict;
            this.allAssetsPaths = allAssetsPaths;
        }

        public void OnGUI()
        {
            if (allResourceDetailDict.Count > 0)
            {
                //List Header
                //EditorGUILayout.BeginHorizontal();
                {

                }
                //EditorGUILayout.EndHorizontal();

                //List Body
                scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    //EditorGUILayout.BeginVertical();
                    foreach (var rd in allResourceDetailDict.Values)
                    {
                        DrawFoldout(rd);
                    }
                    //EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        void DrawFoldout(ResourceDetail rd)
        {
            if (rd.isOpen)
            {
                DrawHeader(rd);
                //EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < rd.resources.Count; i++)
                {
                    EditorGUI.indentLevel+=2;
                    var res = rd.resources[i];
                    //EditorGUILayout.ObjectField("", res.resObj, typeof(Object), false);
                    res.OnResourceGUI();
                    res.isUsedOpen = EditorGUILayout.Foldout(res.isUsedOpen, "References:");
                    if (res.isUsedOpen)
                    {
                        string[] _TempArray = ResUtils.GetUseAssetPaths(res.path, allAssetsPaths);
                        EditorGUI.indentLevel++;
                        for (int j = 0; j < _TempArray.Length; j++)
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath(_TempArray[j], typeof(Object));
                            if (!AssetDatabase.IsSubAsset(obj))
                            {//排除FBX 内部引用
                                EditorGUILayout.BeginHorizontal();
                                //EditorGUILayout.LabelField("", td.md5);
                                EditorGUILayout.ObjectField("", obj, typeof(Object), false);
                                //EditorGUILayout.LabelField("",tr.hashCode.ToString());
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel-=2;
                }
                //EditorGUI.EndDisabledGroup();
            }
            else
            {
                DrawHeader(rd);
            }
            
        }

        void DrawHeader(ResourceDetail rd)
        {
            
            Rect rect = EditorGUILayout.BeginHorizontal();
            GUILayout.Label(AssetDatabase.GetCachedIcon(rd.resources[0].path), new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(20) });
            rd.isOpen = EditorGUILayout.Foldout(rd.isOpen, rd.resources[0].name);
            ResUtils.ColorLabelField("Reference", rd.resources.Count.ToString(), rd.resources.Count <= 1, 100);
            ResUtils.ColorLabelField("Warning", rd.warnNum.ToString(), rd.warnNum < 1 ? 0 : 1, 100);
            ResUtils.ColorLabelField("Error", rd.errorNum.ToString(), rd.errorNum == 0 ? 0 : 2,100);
            EditorGUILayout.EndHorizontal();
        }
    }
}
    

