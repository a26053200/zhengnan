using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataEditor
{
    public class DataEditorWnd : EditorWindow
    {
        [MenuItem("DataEditor/Open")]
        static void ShowWnd()
        {
            Rect rect = new Rect(100,100,1024,768);
            DataEditorWnd wnd = EditorWindow.GetWindow<DataEditorWnd>(true, "DataEditor");
            wnd.position = rect;
            wnd.Init();
        }

        
        private List<string> _list = new List<string> ();
        private DataTree _dataTree;
        private TreeNode _root;
        
        private void Init()
        {
            GetAssets();
            _dataTree = new DataTree();
            _root = TreeNode.Get ().GenerateFileTree (_list);
        }
        
        private void GetAssets()
        {
            _list.Clear ();
            _list.Add ("生物/动物");
            _list.Add ("生物/动物/宠物/猫");
            _list.Add ("生物/动物/宠物/狗");
//		list.Add ("生物/动物/野生/老虎");
//		list.Add ("生物/动物/野生/狮子");
 
            _list.Add ("生物/植物");
            _list.Add ("生物/植物/蔬菜/白菜");
            _list.Add ("生物/植物/蔬菜/萝卜");
//		list.Add ("生物/植物/水果/苹果");
//		list.Add ("生物/植物/水果/橘子");
	
            Debug.Log ("获取数据完成");
        }
        
        void OnGUI()
        {
            _dataTree.treeIndex = 0;
            _dataTree.DrawFileTree (_root, 0);
        }

    }
}