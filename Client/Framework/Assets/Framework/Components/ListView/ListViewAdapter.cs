using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/16 23:40:41</para>
/// </summary> 
public abstract class ListViewAdapter : MonoBehaviour
{
    [HideInInspector]
    public ListView listview;

    public virtual void Initialize()
    {

    }

    public abstract int GetCount();

    public virtual void FillItemData(ListViewCell item, int cellindex)
    {
        item.FillData(cellindex);
    }

    public virtual int GetCellPrefabIndex(int index)
    {
        return 0;
    }

    public virtual int GetCellPrefabIndex(object data)
    {
        return 0;
    }

    public virtual bool IsTitleCell(int index)
    {
        return false;
    }

    public virtual bool IsTitleExpandInDefault(int index)
    {
        return true;
    }
}

