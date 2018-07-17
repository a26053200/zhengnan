using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Introduction: 无限列表
///             Content上禁止挂载ContentSizeFilter和LayOutGroup之类组件
/// Author: 	Cheng
/// Time: 
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect))]
public class ScrollList : MonoBehaviour
{
    public delegate void OnItemRender(int index, Transform child);

    public delegate void OnScrollOver(int index);//滚动结束,返回第一行索引

    public OnItemRender onItemRender;

    public OnScrollOver onScrollOver;
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum Arrangement
    {
        /// <summary>
        /// 横排
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// 竖排
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// 水平对齐
    /// </summary>
    public enum HorizontalAlign
    {
        /// <summary>
        /// 居左
        /// </summary>
        Left,

        /// <summary>
        /// 居中
        /// </summary>
        Middle,

        /// <summary>
        /// 局右
        /// </summary>
        Right,
    }

    /// <summary>
    /// 垂直对齐
    /// </summary>
    public enum VerticalAlign
    {
        /// <summary>
        /// 居上
        /// </summary>
        Top,

        /// <summary>
        /// 居中
        /// </summary>
        Middle,

        /// <summary>
        /// 局下
        /// </summary>
        Bottom,
    }


    public Arrangement arrangement = Arrangement.Vertical;

    /// <summary>
    /// 当选择水平或垂直流动是有用，指每行/列最大个数
    /// </summary>
    public int MaxPerLine
    {
        get { return maxPerLine; }
        set { SetMaxPerLine(value); }
    }

    /// <summary>
    /// 行距
    /// </summary>
    public float rowSpace = 0;

    /// <summary>
    /// 列距
    /// </summary>
    public float columuSpace = 0;

    public HorizontalAlign horizontalAlign = HorizontalAlign.Left;
    public VerticalAlign verticalAlign = VerticalAlign.Top;

    /// <summary>
    /// 边缘留空 上
    /// </summary>
    public float marginTop = 0;

    /// <summary>
    /// 边缘留空 下
    /// </summary>
    public float marginBottom = 0;

    /// <summary>
    /// 边缘留空 左
    /// </summary>
    public float marginLeft = 0;

    /// <summary>
    /// 边缘留空 右
    /// </summary>
    public float marginRight = 0;

    /// <summary>
    /// 总个数
    /// </summary>
    public int ChildCount
    {
        get { return childCount; }
        set { SetChildCount(value, true); }
    }

    /// <summary>
    /// 设置显示窗口大小
    /// </summary>
    public Vector2 ViewPort
    {
        get { return viewPort; }
        set { SetViewPort(value); }
    }

    /// <summary>
    /// 渲染子节点
    /// </summary>
    public GameObject item;

    ScrollRect scrollRect;
    Vector2 viewPort;
    RectTransform content;
    Vector2 itemSize;
    List<Transform> items;
    Dictionary<int, int> contains;
    List<int> outOfContains;
    int childCount; //需要渲染的总数据个数
    int scrollLineIndex; //当前第一个元素索引
    int totalCount; //在UI中显示的个数(不乘以maxPerLine)
    Vector2 startPos; //第一个元素所在位置
    int startIndex; //当前渲染起始坐标
    int endIndex; //当前渲染结束坐标
    int maxPerLine;
    bool scrolling = false;//正在滚动

    void Start()
    {
        maxPerLine = maxPerLine == 0 ? 1 : maxPerLine;
        items = new List<Transform>();
        contains = new Dictionary<int, int>();
        outOfContains = new List<int>();
        scrollRect = transform.GetComponent<ScrollRect>();
        content = scrollRect.content;
        if (content == null)
        {
            Debug.Log("ScrollRect " + scrollRect.gameObject.name + " Has No Content, Please Check And Retry.");
            return;
        }
        viewPort = scrollRect.viewport.rect.size;

        content.anchorMax = new Vector2(0, 1);
        content.anchorMin = new Vector2(0, 1);
        content.pivot = new Vector2(0, 1);

        RectTransform itemTrans = item.transform as RectTransform;
        itemTrans.pivot = new Vector2(0, 1);
        itemSize = itemTrans.sizeDelta;
        item.SetActive(false);

        ReBuild();
    }


    /// <summary>
    /// 当子节点、Mask、maxPerLine
    /// </summary>
    public void ReBuild()
    {
        if (scrollRect == null || content == null || item == null) return;
        ResetChildren();

        Vector2 maskSize = viewPort;
        int count = 0;

        if (arrangement == Arrangement.Horizontal)
        {
            count = Mathf.CeilToInt(maskSize.x / itemSize.x) + 1; //横向列数
            startPos = Vector2.zero;
            startPos.x = marginLeft;
            if (verticalAlign == VerticalAlign.Top)
            {
                startPos.y = -marginTop;
            }
            else if (verticalAlign == VerticalAlign.Middle)
            {
                startPos.y = -(maskSize.y * 0.5f - (itemSize.y * maxPerLine + (maxPerLine - 1) * rowSpace) * 0.5f);
            }
            else if (verticalAlign == VerticalAlign.Bottom)
            {
                startPos.y = -(maskSize.y - marginBottom - itemSize.y * maxPerLine - rowSpace * (maxPerLine - 1));
            }
            //优化：不在一开始生产所有的可见格子
            //for (int i = 0; i < count; i++)
            //{
            //    for (int j = 0; j < maxPerLine; j++)
            //    {
            //        RectTransform child = CreateItem(i*maxPerLine + j);
            //        child.localPosition = startPos +
            //                              new Vector2(i*itemSize.x + i*columuSpace, -j*itemSize.y - j*rowSpace);
            //    }
            //}
        }
        else if (arrangement == Arrangement.Vertical)
        {
            count = Mathf.CeilToInt(maskSize.y / itemSize.y) + 1; //竖向行数
            startPos = Vector2.zero;
            startPos.y = -marginTop; //重置开始节点位置
            if (horizontalAlign == HorizontalAlign.Left)
            {
                startPos.x = marginLeft;
            }
            else if (horizontalAlign == HorizontalAlign.Middle)
            {
                startPos.x = (maskSize.x * 0.5f - (itemSize.x * maxPerLine + (maxPerLine - 1) * columuSpace) * 0.5f);
            }
            else if (horizontalAlign == HorizontalAlign.Right)
            {
                startPos.x = maskSize.x - marginRight - itemSize.x * maxPerLine - columuSpace * (maxPerLine - 1);
            }

            //for (int i = 0; i < count; i++)
            //{
            //    for (int j = 0; j < maxPerLine; j++)
            //    {
            //        RectTransform child = CreateItem(i*maxPerLine + j);
            //        child.localPosition = startPos +
            //                              new Vector2(j*itemSize.x + j*columuSpace, -i*itemSize.y - i*rowSpace);
            //    }
            //}
        }
        totalCount = count;

        SetChildCount(childCount, true);
        BackTop();

        scrollRect.onValueChanged.RemoveAllListeners();
        scrollRect.onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// 列表滚动
    /// </summary>
    /// <param name="vec"></param>
    private void OnValueChanged(Vector2 vec)
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                //   if (vec.x < 0.0f || vec.x >= 1.0f)
                //       return;
                vec.x = Mathf.Clamp(vec.x, 0, 1);
                break;
            case Arrangement.Vertical:
                //   if (vec.y <= 0.0f || vec.y >= 1.0f)
                //       return;
                vec.y = Mathf.Clamp(vec.y, 0, 1);
                break;
        }

        int curLineIndex = GetCurLineIndex();
        if (curLineIndex != scrollLineIndex)
        {
            scrolling = true;
            UpdateRectItem(curLineIndex, false);
        }
        else
        {
            if(scrolling)
            {
                scrolling = false;
                onScrollOver(curLineIndex);
            }
        }
    }

    /// <summary>
    /// 获取页面第一行索引
    /// </summary>
    /// <returns></returns>
    private int GetCurLineIndex()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return
                    Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.x < 0.1f ? content.anchoredPosition.x : 0.1f - marginLeft) /
                                        (columuSpace + itemSize.x));
            case Arrangement.Vertical:
                return
                    Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.y > -0.1f ? content.anchoredPosition.y : -0.1f - marginTop) /
                                        (rowSpace + itemSize.y));
        }
        return 0;
    }

    /// <summary>
    /// 更新数据（待修改问出现的才刷新）
    /// </summary>
    /// <param name="curLineIndex"></param>
    /// <param name="forceRender"></param>
    private void UpdateRectItem(int curLineIndex, bool forceRender)
    {
        if (curLineIndex < 0)
            return;
        //Debug.Log(SystemUtils.GetDebugStackTrace());
        startIndex = curLineIndex * maxPerLine;
        endIndex = (curLineIndex + totalCount) * maxPerLine;
        if (endIndex >= childCount)
            endIndex = childCount;

        contains.Clear(); //渲染序号
        outOfContains.Clear(); //items的索引
        for (int i = 0; i < items.Count; i++)//如果当前已渲染的item中包含
        {
            int index = int.Parse(items[i].gameObject.name);
            if (index < startIndex || index >= endIndex)
            {
                outOfContains.Add(i);
                items[i].gameObject.SetActive(false);
            }
            else
            {
                items[i].gameObject.SetActive(true);
                contains.Add(index, i);
            }
        }

        // *************更改渲染****************
        for (int i = startIndex; i < endIndex; i++)
        {
            if (!contains.ContainsKey(i))
            {
                Transform child = items[outOfContains[0]];
                outOfContains.RemoveAt(0);
                child.gameObject.SetActive(true);
                int row = i / maxPerLine;
                int col = i % maxPerLine;
                if (arrangement == Arrangement.Vertical)
                    child.localPosition = startPos +
                                            new Vector2(col * itemSize.x + (col) * columuSpace,
                                                -row * itemSize.y - (row) * rowSpace);
                else
                    child.localPosition = startPos +
                                            new Vector2(row * itemSize.x + (row) * columuSpace,
                                                -col * itemSize.y - (col) * rowSpace);
                child.gameObject.name = i.ToString();
                if (onItemRender != null)
                    onItemRender(i, child);
            }
            else if (forceRender)
            {
                if (onItemRender != null)
                    onItemRender(i, items[contains[i]]);
            }
        }

        scrollLineIndex = curLineIndex;
    }

    /// <summary>
    /// 移除当前所有
    /// </summary>
    private void ResetChildren()
    {
        items.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 创建新节点
    /// </summary>
    /// <param name="index"></param>
    private RectTransform CreateItem(int index)
    {
        Transform child;
        if (content.childCount > index)
        {
            child = content.GetChild(index);
        }
        else
        {
            GameObject obj = GameObject.Instantiate(item) as GameObject;
            obj.transform.SetParent(content);
            obj.transform.localScale = Vector3.one;
            child = obj.transform;
            obj.SetActive(true);
        }
        child.gameObject.name = index.ToString();
        items.Add(child);

        return child as RectTransform;
    }

    /// <summary>
    /// 更新需要渲染的个数
    /// </summary>
    /// <param name="value"></param>
    public void SetChildCount(int value, bool forceRender)
    {
        if (value < 0) childCount = 0;
        else childCount = value;

        if (totalCount <= 0)//还未初始化
            return;
        if (value > items.Count && items.Count < maxPerLine * totalCount)
        {
            //当前格子数量少于应生成的数量
            int count = items.Count;
            int max = value < maxPerLine * totalCount ? value : maxPerLine * totalCount;
            for (int i = count; i < max; i++)
            {
                int row = i / maxPerLine;
                int col = i % maxPerLine;
                RectTransform child = CreateItem(i);
                if (arrangement == Arrangement.Vertical)
                    child.localPosition = startPos +
                                            new Vector2(col * itemSize.x + (col) * columuSpace,
                                                -row * itemSize.y - (row) * rowSpace);
                else
                    child.localPosition = startPos +
                                            new Vector2(row * itemSize.x + (row) * columuSpace,
                                                -col * itemSize.y - (col) * rowSpace);
            }
        }

        if (content == null) return;

        int rc = Mathf.CeilToInt((float)childCount / (float)maxPerLine); //设置content的大小
        if (arrangement == Arrangement.Horizontal)
        {
            content.sizeDelta = new Vector2(marginLeft + marginRight + itemSize.x * rc + columuSpace * (rc - 1),
                viewPort.y);
            if (content.sizeDelta.x > viewPort.x && content.anchoredPosition.x < viewPort.x - content.sizeDelta.x)
                content.anchoredPosition = new Vector2(viewPort.x - content.sizeDelta.x, content.anchoredPosition.y);
        }
        else
        {
            content.sizeDelta = new Vector2(viewPort.x, marginTop + marginBottom + itemSize.y * rc + rowSpace * (rc - 1));
            if (content.sizeDelta.y > viewPort.y && content.anchoredPosition.y > content.sizeDelta.y - viewPort.y)
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.sizeDelta.y - viewPort.y);
        }
        UpdateRectItem(GetCurLineIndex(), true);
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="index"></param>
    public void AddChild(int index)
    {
        if (index < 0) return;
        startIndex = scrollLineIndex * maxPerLine;
        endIndex = (scrollLineIndex + totalCount) * maxPerLine;
        SetChildCount(childCount + 1, index >= startIndex && index < endIndex);
    }

    /// <summary>
    /// 删除子节点
    /// </summary>
    /// <param name="index"></param>
    public void RemoveChild(int index)
    {
        if (index < 0 || index >= childCount) return;
        startIndex = scrollLineIndex * maxPerLine;
        endIndex = (scrollLineIndex + totalCount) * maxPerLine;
        SetChildCount(childCount - 1, index >= startIndex && index < endIndex);
    }


    /// <summary>
    /// 设置显示窗口大小(现在貌似可以废弃了)
    /// </summary>
    /// <param name="port"></param>
    public void SetViewPort(Vector2 port)
    {
        if (port == viewPort) return;
        viewPort = port;
        ReBuild();
    }

    /// <summary>
    /// 设置行列最大
    /// </summary>
    /// <param name="max"></param>
    public void SetMaxPerLine(int max)
    {
        maxPerLine = max;
        ReBuild();
    }

    /// <summary>
    /// 返回顶部
    /// </summary>
    public void BackTop()
    {
        content.localPosition = Vector3.zero;
        UpdateRectItem(0, true);
    }

    /// <summary>
    /// 返回底部
    /// </summary>
    public void BackBottom()
    {
        if (arrangement == Arrangement.Vertical)
        {
            content.localPosition = new Vector3(0, -viewPort.y + content.sizeDelta.y, 0);
        }
        else
        {
            content.localPosition = new Vector3(viewPort.x - content.sizeDelta.x, 0);
        }
        UpdateRectItem(Mathf.CeilToInt((float)childCount / (float)maxPerLine) - totalCount + 1, true);
    }

    public void RefreshViewItem()
    {
        UpdateRectItem(scrollLineIndex, true);
    }


    public void SetArrangement(int arr)
    {
        arrangement = (Arrangement)arr;
    }

    public void SetHorizontal(int h)
    {
        horizontalAlign = (HorizontalAlign)h;
    }

    public void SetVerticle(int v)
    {
        verticalAlign = (VerticalAlign)v;
    }
}
