using UnityEngine;

namespace EasyList
{
    /// <summary>
    /// <para></para>
    /// <para>Author: zhengnan </para>
    /// <para>Create: DATE TIME</para>
    /// </summary> 
    public class CellInfo
    {
        public int index;
        
            public Vector3 localPos;
        
            private bool isTitle;
        
            public bool IsTitle
            {
                get
                {
                    return isTitle;
                }
                set
                {
                    isTitle = value;
                }
            }
        
            private bool isExpand;
        
            public bool IsExpand
            {
                get
                {
                    return isExpand;
                }
                set
                {
                    isExpand = value;
                }
            }
        
            public ListViewCell linkedListviewCell;
        
            public CellInfo(int index, Vector3 localPos)
            {
                this.index = index;
        
                this.localPos = localPos;
            }
        
            public CellInfo(bool isTitle, bool isExpand)
            {
                this.isTitle = isTitle;
        
                this.isExpand = isExpand;
            }
        
            public void SetLocalPos(Vector3 localPos)
            {
                this.localPos = localPos;
            }
        
            public bool IsShowing()
            {
                return linkedListviewCell != null && linkedListviewCell.IsUsing;
            }
        
            public bool CanShow()
            {
                return IsTitle || !IsTitle && IsExpand;
            }
    }
}