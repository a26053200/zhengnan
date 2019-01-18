using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/16 23:41:14</para>
/// </summary> 

public class ListViewCell : MonoBehaviour
{
    private CellInfo cellInfo;

    public CellInfo CellInfo
    {
        get
        {
            return cellInfo;
        }
        set
        {
            cellInfo = value;
        }
    }

    private bool isUsing;
    [HideInInspector]
    public bool IsUsing
    {
        get
        {
            return isUsing;
        }
        //set 
        //{ 
        //    isUsing = value; 
        //}
    }

    private Button titleBtn;

    private bool isTitleCell;
    [HideInInspector]
    public bool IsTitleCell
    {
        get
        {
            if (isTitleCell)
            {
                this.titleBtn = transform.GetComponent<Button>();
                if (this.titleBtn == null)
                {
                    Debug.LogError(string.Format("gameObject [{1}],ListViewCell.isExpanding = true, but do not have a expand button!"));
                }
            }

            return isTitleCell;
        }
        set
        {
            isTitleCell = value;
        }
    }

    private bool isExpand;
    [HideInInspector]
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

    private ListView listView;

    private Vector2 cellSize = Vector3.zero;

    public void SetCellInfo(CellInfo info, ListView listView = null)
    {
        info.linkedListviewCell = this;
        this.cellInfo = info;
        this.listView = listView;

        gameObject.SetActive(true);
        isUsing = true;
        transform.localPosition = this.cellInfo.localPos;
        gameObject.name = this.cellInfo.index.ToString();

        if (this.cellInfo.IsTitle)
        {
            this.titleBtn = transform.GetComponent<Button>();
            if (this.titleBtn != null)
            {
                this.titleBtn.onClick.RemoveAllListeners();
                this.titleBtn.onClick.AddListener(OnTitleBtnClick);
            }
            else
            {
                Debug.LogError(string.Format("gameObject [{0}],ListViewCell.isExpanding = true, but do not have a expand button!", transform.name));
            }
        }
    }

    private void OnTitleBtnClick()
    {
        this.cellInfo.IsExpand = !this.cellInfo.IsExpand;
        if (listView)
        {
            listView.RefreshData();
        }
    }

    public virtual void Recycle()
    {
        cellInfo.linkedListviewCell = null;
        gameObject.SetActive(false);
        this.isUsing = false;
    }

    //public virtual Vector2 GetCellSize()
    //{
    //    if (Vector3.zero.Equals(cellSize))
    //    {
    //        cellSize = transform.GetComponent<RectTransform>().sizeDelta;
    //    }
    //    return cellSize;
    //}

    public virtual void FillData(params object[] data)
    {
        
    }
}

