using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/17 0:25:30</para>
/// </summary> 
public class LuaListViewAdapter : ListViewAdapter
{
    private int dataCount = 0;
    private LuaFunction luaFunc;

    public void Init(int dataCount, LuaFunction luaFunc)
    {
        this.dataCount = dataCount;
        this.luaFunc = luaFunc;
    }
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void FillItemData(ListViewCell item, int cellindex)
    {
        base.FillItemData(item, cellindex);
        luaFunc.BeginPCall();
        luaFunc.Call<ListViewCell, int>(item, cellindex);
        luaFunc.EndPCall();
        //Debug.Log("LuaListViewAdapter.FillItemData " + cellindex);
    }

    public override int GetCellPrefabIndex(int index)
    {
        return base.GetCellPrefabIndex(index);
    }

    public override int GetCellPrefabIndex(object data)
    {
        return base.GetCellPrefabIndex(data);
    }

    public override int GetCount()
    {
        return dataCount;
    }

    public override bool IsTitleCell(int index)
    {
        return base.IsTitleCell(index);
    }

    public override bool IsTitleExpandInDefault(int index)
    {
        return base.IsTitleExpandInDefault(index);
    }
}

