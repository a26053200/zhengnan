using UnityEditor;
using UnityEngine;

namespace DataEditor
{
    public class DataTree
    {
        private static TreeNode root = null; 
        private TreeNode currentNode;
        public int treeIndex = 0;
        public void ShowFileTree(TreeNode node, int level)
        {
            string prefix = "";
            for (int i = 0; i < level; i++)
            {
                prefix += "~";
            }
            Debug.Log (prefix + node.name);
            if (node == null || node.children == null) 
            {
                return;
            }
            for (int i = 0; i < node.children.Count; i++) 
            {
                ShowFileTree (node.children[i], level+1);
            }
        }
        public void DrawFileTree(TreeNode node, int level)
        {
            if (node == null) 
            {
                return;
            }
            GUIStyle style = new GUIStyle();
            style.normal.background = null;
            style.normal.textColor = Color.white;
            if (node == currentNode) 
            {
                style.normal.textColor = Color.red;
            }
 
            Rect rect = new Rect(5+20*level, 5+20*treeIndex, node.name.Length*25, 20);
            treeIndex++;
 
            if (node.nodeType == TreeNode.TreeNodeType.Switch) {
                node.isOpen = EditorGUI.Foldout (rect, node.isOpen, node.name, true);
            }
            else
            {
                if (GUI.Button (rect, node.name, style)) 
                {
                    Debug.Log (node.name);
                    currentNode = node;
                }
            }
	
            if (node==null || !node.isOpen || node.children == null) 
            {
                return;
            }
            for (int i = 0; i < node.children.Count; i++) 
            {
                DrawFileTree (node.children[i], level+1);
            }
        }
    }
}