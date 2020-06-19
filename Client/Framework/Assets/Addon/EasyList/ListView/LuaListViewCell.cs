using System;
using UnityEngine;
using System.Collections.Generic;

namespace EasyList
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/1/17 0:24:39</para>
    /// </summary> 
    public class LuaListViewCell : ListViewCell
    {
        public override void FillData(params object[] data)
        {
            base.FillData(data);
            //Debug.Log("ListViewCell.FillData " + data.ToString());
        }
        public override void Recycle()
        {
            base.Recycle();
        }
    }
}


