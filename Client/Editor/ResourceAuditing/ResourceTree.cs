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
        private int sortBy = 0;

        public ResourceTree(ResourceAuditing wnd, Dictionary<string, T> allResourceDetailDict, List<string> allAssetsPaths)
        {
            this.wnd = wnd;
            this.allResourceDetailDict = allResourceDetailDict;
            orgList = new List<T>(allResourceDetailDict.Values);
            totalPage = (int)Mathf.Ceil((float)orgList.Count / (float)pageSize);
            SortListBy(sortBy);
            this.allAssetsPaths = allAssetsPaths;
        }

        public void OnGUI()
        {
            if (allResourceDetailDict.Count > 0)
            {
                //List Header
                DrawHeader();

                //List Body
                scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    //EditorGUILayout.BeginVertical();
                    for (int i = currPage * pageSize; i < (currPage + 1) * pageSize && i < orgList.Count; i++)
                    {
                        DrawFoldoutContent(orgList[i]);
                    }
                    //EditorGUILayout.EndVertical();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(wnd.position.width * (0.618f)));
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        if (GUILayout.Button(Button_Pre, GUILayout.ExpandWidth(true)))
                        {
                            currPage = Mathf.Max(0, currPage - 1);
                        }
                        EditorGUILayout.LabelField((currPage + 1) + "/" + totalPage, EditorStyles.centeredGreyMiniLabel);
                        if (GUILayout.Button(Button_Next, GUILayout.ExpandWidth(true)))
                        {
                            currPage = Mathf.Min(totalPage - 1, currPage + 1);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
            }
        }

        const string Title_Repeat = "Repeat";
        const string Title_Warning = "Warning";
        const string Title_Error = "Error";

        const string Button_Pre = "Up";
        const string Button_Next = "Down";

        void DrawFoldoutContent(ResourceDetail rd)
        {
            
            if (rd.isOpen)
            {
                DrawFoldout(rd);
                //EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < rd.resources.Count; i++)
                {
                    EditorGUI.indentLevel+=2;
                    var res = rd.resources[i];
                    //EditorGUILayout.ObjectField("", res.resObj, typeof(Object), false);
                    res.OnResourceGUI();
                    res.isUsedOpen = EditorGUILayout.Foldout(res.isUsedOpen, Title_Repeat);
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
        bool[] sorts = new bool[3] { true, false, false };
        string[] sortNames = new string[3] { "Error", "Warning", "Repeat" };
        bool posGroupEnabled;
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", GUILayout.Width(wnd.position.width * (1 - 0.618f)));
                EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * 0.618f));
                {
                    for (int i = 0; i < sorts.Length; i++)
                    {
                        bool oldBool = sorts[i];
                        sorts[i] = EditorGUILayout.ToggleLeft("Sort by " + sortNames[i], sorts[i]);
                        if (oldBool != sorts[i])
                        {
                            SortListBy(i);
                            for (int j = 0; j < sorts.Length; j++)
                                sorts[j] = j == i;
                            break;
                        }
                    }
                    
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        void SortListBy(int sortBy)
        {
            orgList.Sort((T t1, T t2) =>
            {
                switch(sortBy)
                {
                    case 0:
                        return t2.errorNum.CompareTo(t1.errorNum);
                    case 1:
                        return t2.warnNum.CompareTo(t1.warnNum);
                    case 2:
                        return t2.resources.Count.CompareTo(t1.resources.Count);
                }
                return 0;
            });
        }
        void DrawFoldout(ResourceDetail rd)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(wnd.position.width * (1 - 0.618f)));
            EditorGUILayout.LabelField(new GUIContent(AssetDatabase.GetCachedIcon(rd.resources[0].path)), new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(20) });
            rd.isOpen = EditorGUILayout.Foldout(rd.isOpen, rd.resources[0].name);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            ResUtils.ColorLabel(Title_Error, rd.errorNum.ToString(), rd.errorNum == 0 ? 0 : 2);
            ResUtils.ColorLabel(Title_Warning, rd.warnNum.ToString(), rd.warnNum < 1 ? 0 : 1);
            ResUtils.ColorLabel(Title_Repeat, rd.resources.Count.ToString(), rd.resources.Count <= 1 ? 0 : 2);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }
    }
}
    

