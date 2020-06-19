using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyList
{
    /// <summary>
    /// <para>指定位置的列表</para>
    /// <para>Author: zhengnan </para>
    /// <para>Create: DATE TIME</para>
    /// </summary> 
    public class ListPositionView : ListViewBase
    {
        protected override void Initialize(Action onComplete = null)
        {
            base.Initialize(onComplete);
            GenerateCellBuffers();
            cellInfoMapper.Clear();
            //TODO
            //ForceRecycleAllBuffers();
            CalculateSizeAndCellPos(0f);
            this.scrollRect.onValueChanged.AddListener(FillData);
            FillData(Vector2.zero);
        }
        
        public override void RefreshData()
        {
            ClearAllCell();
            CalculateSizeAndCellPos(0f);
        }

        protected override float CalculateSizeAndCellPos(float beyondDistance)
        {
            for (int i = 0; i < this.CellsCount; i++)
            {
                CellInfo cellInfo = this.Adapter.GetCellInfo(i);
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                ListViewCell cell = GameObject.Instantiate<GameObject>(this.prefabs[prefabIndex]).GetComponent<ListViewCell>();
                cell.gameObject.SetActive(true);
                cell.transform.SetParent(transform);
                cell.transform.localScale = Vector3.one;
                this.Adapter.FillItemData(cell, i);
            }

            return this.CellsCount;
        }
    }
}