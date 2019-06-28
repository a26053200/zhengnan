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

        List<T> orgList;
        List<T> loadedList;
        Vector2 scrollerPos = Vector2.zero;
        ResourceAuditing wnd;

        private int pageSize = 20;
        private int totalPage = 0;
        private int currPage = 0;


        public ResourceTree(ResourceAuditing wnd, Dictionary<string, T> allResourceDetailDict, List<string> allAssetsPaths)
        {
            this.wnd = wnd;
            this.allResourceDetailDict = allResourceDetailDict;
            orgList = new List<T>(allResourceDetailDict.Values);
            totalPage = (int)Mathf.Ceil((float)orgList.Count / (float)pageSize);
            
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
                    for (int i = currPage * pageSize; i < (currPage + 1) * pageSize && i < orgList.Count; i++)
                    {
                        DrawFoldout(orgList[i]);
                    }
                    //EditorGUILayout.EndVertical();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(Button_Pre, GUILayout.Width(wnd.position.x / 8)))
                    {
                        currPage = Mathf.Max(0, currPage - 1);
                    }
                    EditorGUILayout.HelpBox((currPage + 1) + "/" + totalPage, MessageType.None, true);
                    if (GUILayout.Button(Button_Next, GUILayout.Width(wnd.position.x / 8)))
                    {
                        currPage = Mathf.Min(totalPage, currPage + 1); 
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
        }

        const string Title_References = "References";
        const string Title_Warning = "Warning";
        const string Title_Error = "Error";

        const string Button_Pre = "Up";
        const string Button_Next = "Down";

        void DrawFoldoutContent(ResourceDetail rd)
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
                    res.isUsedOpen = EditorGUILayout.Foldout(res.isUsedOpen, Title_References);
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
                DrawFoldout(rd);
            }
            
        }
        bool[] sorts = new bool[3] { true, true, true };
        bool posGroupEnabled;
        void DrawHeader(ResourceDetail rd)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * (1 - 0.618f)));
                EditorGUILayout.LabelField("");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * 0.618f));
                {
                    posGroupEnabled = EditorGUILayout.BeginToggleGroup("Align position", posGroupEnabled);
                    {
                        sorts[0] = EditorGUILayout.Toggle("x", sorts[0]);
                        sorts[1] = EditorGUILayout.Toggle("y", sorts[1]);
                        sorts[2] = EditorGUILayout.Toggle("z", sorts[2]);
                    }
                    EditorGUILayout.EndToggleGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawFoldout(ResourceDetail rd)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * (1 - 0.618f)));
            EditorGUILayout.LabelField(new GUIContent(AssetDatabase.GetCachedIcon(rd.resources[0].path)), new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(20) });
            rd.isOpen = EditorGUILayout.Foldout(rd.isOpen, rd.resources[0].name);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * 0.618f));
            ResUtils.ColorLabel(Title_References, rd.resources.Count.ToString(), rd.resources.Count <= 1 ? 0 : 2);
            ResUtils.ColorLabel(Title_Warning, rd.warnNum.ToString(), rd.warnNum < 1 ? 0 : 1);
            ResUtils.ColorLabel(Title_Error, rd.errorNum.ToString(), rd.errorNum == 0 ? 0 : 2);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }
    }
}
    

