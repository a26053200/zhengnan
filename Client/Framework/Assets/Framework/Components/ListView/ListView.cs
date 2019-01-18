using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using DG.Tweening;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/16 23:39:54</para>
/// </summary> 
public class ListView : MonoBehaviour
{
    public Action OnDataFillCompleted;

    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private float space;

    [SerializeField]
    private bool isManualFill = false;

    [SerializeField]
    private bool left2Right = true;

    [SerializeField]
    private float bufferFactor = 0.2f;

    [SerializeField]
    private int constrain = 1;

    private ScrollRect scrollRect;

    private RectTransform scrollRectTransform;

    private RectTransform rectTransform;

    private Vector2[] cellSizes;

    private List<ListViewCell>[] buffersArray;

    private Dictionary<int, CellInfo> cellInfoMapper = new Dictionary<int, CellInfo>();

    private Vector2 viewPortSize;

    private float visualDIA;

    private int CellsCount
    {
        get
        {
            return this.Adapter.GetCount();
        }
    }

    [SerializeField]
    private ListViewAdapter adapter;

    public ListViewAdapter Adapter
    {
        get
        {
            return adapter;
        }
        set
        {
            adapter = value;
            if (Application.isPlaying && this.gameObject.activeInHierarchy == true && isManualFill)
            {
                adapter.listview = this;
                Adapter.Initialize();
                Initialize();
            }
        }
    }

    // Use this for initialization
    protected void OnEnable()
    {
        if (Application.isPlaying && this.Adapter != null && !isManualFill)
        {
            ClearAllCell();
            Adapter.Initialize();
            Initialize();
        }
    }

    private void Initialize(Action onComplete = null)
    {
        if (!CheckConfigAndData())
        {
            return;
        }

        //ClearAllCell();

        this.rectTransform = transform.GetComponent<RectTransform>();
        this.scrollRectTransform = transform.parent.GetComponent<RectTransform>();
        this.scrollRect = this.scrollRectTransform.GetComponent<ScrollRect>();
        this.viewPortSize = this.scrollRectTransform.sizeDelta;

        this.cellSizes = new Vector2[this.prefabs.Length];
        for (int i = 0; i < this.prefabs.Length; i++)
        {
            RectTransform cellRect = this.prefabs[i].GetComponent<RectTransform>();
            this.cellSizes[i] = cellRect.sizeDelta;
        }

        if (this.scrollRect.horizontal)
        {
            this.visualDIA = (0.5f + this.bufferFactor) * this.viewPortSize.x;
        }
        else if (this.scrollRect.vertical)
        {
            this.visualDIA = (0.5f + this.bufferFactor) * this.viewPortSize.y;
        }

        GenerateCellBuffers();

        cellInfoMapper.Clear();

        //TODO
        //ForceRecycleAllBuffers();

        CalculateSizeAndCellPos(0f);

        this.scrollRect.onValueChanged.AddListener(FillData);

        FillData(Vector2.zero);

        if (OnDataFillCompleted != null)
        {
            OnDataFillCompleted();
        }
    }

    private bool CheckConfigAndData()
    {
        bool checkOk = true;
        if (this.constrain > 1 && this.AnyTitleCell())
        {
            Debug.LogError(string.Format("ListView:{0},this.constrain > 1 && have title cell", transform.name));
            checkOk = false;
        }
        return checkOk;
    }

    private bool AnyTitleCell()
    {
        bool isTitle = false;
        for (int i = 0; i < this.CellsCount;)
        {
            if (this.Adapter.IsTitleCell(i))
            {
                isTitle = true;
                break;
            }
            i += this.constrain;
        }

        return isTitle;
    }

    private void ClearAllCell()
    {
        int childCount = this.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (i < this.transform.childCount)
                GameObject.Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    public void ReFillData()
    {
        ForceRecycleAllBuffers();
        CalculateSizeAndCellPos(BeyondDistance());
        FillData(Vector2.zero);

        if (OnDataFillCompleted != null)
        {
            OnDataFillCompleted();
        }
    }

    public void RefreshData()
    {
        ForceRecycleAllBuffers();
        CalculateSizeAndCellPos(BeyondDistance());
        FillData(Vector2.zero);
    }

    private void GenerateCellBuffers()
    {
        this.buffersArray = new List<ListViewCell>[this.prefabs.Length];
        for (int i = 0; i < this.prefabs.Length; i++)
        {
            this.buffersArray[i] = new List<ListViewCell>();
        }
    }

    private float CalculateSizeAndCellPos(float beyondDistance)
    {
        GetCellInfoMapper();
        float length = 0;

        if (this.scrollRect.vertical)
        {
            length = CalculateSizeAndCellPosVertical(beyondDistance);
        }
        else if (this.scrollRect.horizontal)
        {
            length = CalculateSizeAndCellPosHorizontal(beyondDistance);
        }

        return length;
    }



    private float CalculateSizeAndCellPosVertical(float beyondDistance)
    {
        float length = 0;
        float offset = 0;

        for (int i = 0; i < this.CellsCount;)
        {
            CellInfo cellInfo = cellInfoMapper[i];
            if (cellInfo.CanShow())
            {
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                length += this.cellSizes[prefabIndex].y + this.space;
            }
            i += this.constrain;
        }

        if (length < viewPortSize.y)
        {
            length = viewPortSize.y;
        }

        this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, length);

        for (int i = 0; i < this.CellsCount;)
        {
            float cellHeight = CellSizeDeltaInIndex(i).y;
            float cellWidth = CellSizeDeltaInIndex(i).x;
            if (this.constrain > 1)
            {
                float offsetX = -this.viewPortSize.x / 2f + cellWidth / 2;
                for (int j = 0; j < this.constrain && i + j < this.CellsCount; j++)
                {
                    int cellIndex = i + j;
                    CellInfo cellInfo = cellInfoMapper[cellIndex];
                    if (cellInfo.CanShow())
                    {
                        Vector3 localPos = new Vector3(offsetX, (length / 2.0f - offset - cellHeight / 2.0f), 0.0f);
                        cellInfo.SetLocalPos(localPos);
                        offsetX += cellWidth + this.space;
                    }
                }
                offset += cellHeight + this.space;
            }
            else
            {
                int cellIndex = i;
                CellInfo cellInfo = cellInfoMapper[cellIndex];
                if (cellInfo.CanShow())
                {
                    Vector3 localPos = new Vector3(0f, (length / 2.0f - offset - cellHeight / 2.0f), 0.0f);
                    cellInfo.SetLocalPos(localPos);
                    offset += cellHeight + this.space;
                }
            }
            i += this.constrain;
        }

        this.transform.localPosition = new Vector3(0.0f, -(this.rectTransform.sizeDelta.y - this.scrollRectTransform.sizeDelta.y) / 2f + beyondDistance, 0.0f);

        return length;
    }

    private float CalculateSizeAndCellPosHorizontal(float beyondDistance)
    {
        float length = 0;
        float offset = 0;

        for (int i = 0; i < this.CellsCount; i++)
        {
            CellInfo cellInfo = cellInfoMapper[i];
            if (cellInfo.CanShow())
            {
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                length += this.cellSizes[prefabIndex].x + this.space;
            }
        }

        if (length < this.viewPortSize.x)
        {
            length = this.viewPortSize.x;
        }

        this.rectTransform.sizeDelta = new Vector2(length, this.rectTransform.sizeDelta.y);

        for (int i = 0; i < this.CellsCount; i++)
        {
            CellInfo cellInfo = cellInfoMapper[i];
            if (cellInfo.CanShow())
            {
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                float cellWidth = this.cellSizes[prefabIndex].x;

                Vector3 localPos = Vector3.zero;
                if (this.left2Right)
                {
                    localPos = new Vector3(-(length / 2.0f - offset - cellWidth / 2.0f), 0.0f, 0.0f);
                }
                else
                {
                    localPos = new Vector3((length / 2.0f - offset - cellWidth / 2.0f), 0.0f, 0.0f);
                }
                cellInfo.SetLocalPos(localPos);
                offset += cellWidth + this.space;
            }
        }

        if (this.left2Right)
        {
            this.transform.localPosition = new Vector3((this.rectTransform.sizeDelta.x - this.scrollRectTransform.sizeDelta.x) / 2f - beyondDistance, 0.0f, 0.0f);
        }
        else
        {
            this.transform.localPosition = new Vector3(-(this.rectTransform.sizeDelta.x - this.scrollRectTransform.sizeDelta.x) / 2f + beyondDistance, 0.0f, 0.0f);
        }

        return length;
    }

    private float BeyondDistance()
    {
        float beyondDistance = 0f;

        if (this.scrollRect.vertical)
        {
            if (this.rectTransform.sizeDelta.y > this.scrollRectTransform.sizeDelta.y)
            {
                beyondDistance = (this.rectTransform.sizeDelta.y - this.scrollRectTransform.sizeDelta.y) / 2f + this.rectTransform.localPosition.y;
            }
        }
        else if (this.scrollRect.horizontal)
        {
            if (this.rectTransform.sizeDelta.x > this.scrollRectTransform.sizeDelta.x)
            {
                if (this.left2Right)
                {
                    beyondDistance = (this.rectTransform.sizeDelta.x - this.scrollRectTransform.sizeDelta.x) / 2f - this.rectTransform.localPosition.x;
                }
                else
                {
                    beyondDistance = (this.rectTransform.sizeDelta.x - this.scrollRectTransform.sizeDelta.x) / 2f + this.rectTransform.localPosition.x;
                }
            }
        }

        return beyondDistance;
    }

    private Vector2 CellSizeDeltaInIndex(int index)
    {
        int prefabIndex = this.Adapter.GetCellPrefabIndex(index);
        return this.cellSizes[prefabIndex];
    }

    private void GetCellInfoMapper()
    {
        //cellInfoMapper.Clear();

        bool isTitleExpand = true;//标志从这个title到下个title之间，是否展开
        for (int i = 0; i < this.CellsCount;)//here
        {
            CellInfo cellInfo = null;
            bool isTitle = this.Adapter.IsTitleCell(i);
            if (isTitle)
            {
                isTitleExpand = this.Adapter.IsTitleExpandInDefault(i);
                if (cellInfoMapper.TryGetValue(i, out cellInfo))
                {
                    isTitleExpand = cellInfo.IsExpand;
                    cellInfo.IsTitle = isTitle;

                }
                else
                {
                    cellInfo = new CellInfo(isTitle, isTitleExpand);
                    this.cellInfoMapper.Add(i, cellInfo);
                }
            }
            else
            {
                if (cellInfoMapper.TryGetValue(i, out cellInfo))
                {
                    cellInfo.IsExpand = isTitleExpand;
                    cellInfo.IsTitle = isTitle;
                }
                else
                {
                    cellInfo = new CellInfo(isTitle, isTitleExpand);
                    this.cellInfoMapper.Add(i, cellInfo);
                }
            }

            cellInfo.index = i;

            i++;
        }
    }

    private void FillData(Vector2 vec2)
    {
        RecycleBuffers();
        if (this.scrollRect.horizontal)
        {
            float currentPos = -this.rectTransform.localPosition.x;

            var etor = this.cellInfoMapper.GetEnumerator();
            while (etor.MoveNext())
            {
                CellInfo cellInfo = etor.Current.Value;
                if (cellInfo.CanShow())
                {
                    float distance2Mid = Mathf.Abs(cellInfo.localPos.x - currentPos);
                    if (distance2Mid <= this.visualDIA && !cellInfo.IsShowing())
                    {
                        ListViewCell buffer = GetUsableBuffer(cellInfo.index);
                        if (buffer != null)
                        {
                            buffer.SetCellInfo(cellInfo, this);
                            this.Adapter.FillItemData(buffer, cellInfo.index);
                        }
                    }
                }
            }
            etor.Dispose();
        }
        else if (this.scrollRect.vertical)
        {
            float currentPos = -this.rectTransform.localPosition.y;

            var etor = this.cellInfoMapper.GetEnumerator();
            while (etor.MoveNext())
            {
                CellInfo cellInfo = etor.Current.Value;
                if (cellInfo.CanShow())
                {
                    float distance2Mid = Mathf.Abs(cellInfo.localPos.y - currentPos);
                    if (distance2Mid <= this.visualDIA && !cellInfo.IsShowing())
                    {
                        ListViewCell buffer = GetUsableBuffer(cellInfo.index);
                        if (buffer != null)
                        {
                            buffer.SetCellInfo(cellInfo, this);
                            this.Adapter.FillItemData(buffer, cellInfo.index);
                        }
                    }
                }
            }
            etor.Dispose();
        }

    }

    private ListViewCell GetUsableBuffer(int index)
    {
        ListViewCell buffer = null;
        int prefabIndex = this.Adapter.GetCellPrefabIndex(index);
        List<ListViewCell> buffers = this.buffersArray[prefabIndex];
        for (int i = 0; i < buffers.Count; i++)
        {
            if (!buffers[i].IsUsing)
            {
                buffer = buffers[i];
                break;
            }
        }

        if (buffer == null)
        {
            buffer = GameObject.Instantiate<GameObject>(this.prefabs[prefabIndex]).GetComponent<ListViewCell>();
            if (buffer != null)
            {
                buffer.transform.SetParent(transform);
                buffer.transform.localScale = Vector3.one;
                this.buffersArray[prefabIndex].Add(buffer);
            }
        }

        return buffer;
    }

    //obsolete
    //public virtual GameObject GameobjectInIndex(int index)
    //{
    //    GameObject obj = null;
    //    for (int i = 0; i < buffersArray.Length; i++)
    //    {
    //        List<IndexAndBuffer> buffers = buffersArray[i];
    //        for (int j = 0; j < buffers.Count; j++)
    //        {
    //            if (buffers[j].mainIndex == index && buffers[j].isUsing)
    //            {
    //                obj = buffers[j].bufferGo;
    //                break;
    //            }
    //        }
    //    }

    //    return obj;
    //}

    public virtual void GotoIndex(int index, float duration = 0.5f)
    {
        if (this.constrain > 1)
        {
            index = index / this.constrain;
        }

        if (index > this.CellsCount)
        {
            return;
        }

        if (this.scrollRect.vertical)
        {
            float height = 0;
            for (int i = 0; i < index; i++)
            {
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                height += this.cellSizes[prefabIndex].y;
            }

            RectTransform rt = this.transform.parent.GetComponent<RectTransform>();

            if (height >= rt.sizeDelta.y)
            {
                height = (this.rectTransform.sizeDelta.y - rt.sizeDelta.y) / 2 - height;
                this.transform.DOLocalMoveY(-height, duration);
            }
        }
        else if (this.scrollRect.horizontal)
        {
            float width = 0;
            for (int i = 0; i < index; i++)
            {
                int prefabIndex = this.Adapter.GetCellPrefabIndex(i);
                width += this.cellSizes[prefabIndex].x;
            }

            RectTransform rt = this.transform.parent.GetComponent<RectTransform>();

            if (this.left2Right)
            {
                width = width - (this.rectTransform.sizeDelta.x - rt.sizeDelta.x) / 2;
            }
            else
            {
                width = (this.rectTransform.sizeDelta.x - rt.sizeDelta.x) / 2 - width;// -base.spacing.y * (index - 1);
            }

            this.transform.DOLocalMoveX(-width, duration);

            //CalculateLayoutInputHorizontal();
        }
    }

    private void ForceRecycleAllBuffers()
    {
        for (int i = 0; i < this.buffersArray.Length; i++)
        {
            List<ListViewCell> buffers = this.buffersArray[i];
            for (int j = 0; j < buffers.Count; j++)
            {
                ListViewCell buffer = buffers[j];
                if (buffer.IsUsing)
                {
                    buffer.Recycle();
                }
            }
        }
    }

    private void RecycleBuffers()
    {
        if (this.scrollRect.horizontal)
        {
            float currentPos = -this.rectTransform.localPosition.x;
            for (int i = 0; i < this.buffersArray.Length; i++)
            {
                List<ListViewCell> buffers = this.buffersArray[i];
                for (int j = 0; j < buffers.Count; j++)
                {
                    ListViewCell buffer = buffers[j];
                    float distance2Mid = Mathf.Abs(buffer.CellInfo.localPos.x - currentPos);
                    if (buffer.IsUsing && distance2Mid > this.visualDIA)
                    {
                        buffer.Recycle();
                    }
                }
            }
        }
        else if (this.scrollRect.vertical)
        {
            float currentPos = -this.rectTransform.localPosition.y;
            for (int i = 0; i < this.buffersArray.Length; i++)
            {
                List<ListViewCell> buffers = this.buffersArray[i];
                for (int j = 0; j < buffers.Count; j++)
                {
                    ListViewCell buffer = buffers[j];
                    float distance2Mid = Mathf.Abs(buffer.CellInfo.localPos.y - currentPos);
                    if (buffer.IsUsing && distance2Mid > this.visualDIA)
                    {
                        buffer.Recycle();
                    }
                }
            }
        }
    }
}

public class CellInfo        //for each cell
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

