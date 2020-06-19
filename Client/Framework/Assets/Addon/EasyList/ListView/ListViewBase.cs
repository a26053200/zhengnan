using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyList
{
    /// <summary>
    /// <para></para>
    /// <para>Author: zhengnan </para>
    /// <para>Create: DATE TIME</para>
    /// </summary> 
    [RequireComponent(typeof(ScrollRect))]
    public class ListViewBase : MonoBehaviour
    {
        [SerializeField]
        protected GameObject[] prefabs;
        
        protected ScrollRect scrollRect;

        protected RectTransform scrollRectTransform;

        protected RectTransform rectTransform;
        
        protected Vector2[] cellSizes;
    
        protected List<ListViewCell>[] buffersArray;
    
        protected Dictionary<int, CellInfo> cellInfoMapper = new Dictionary<int, CellInfo>();
    
        protected Vector2 viewPortSize;
    
        [SerializeField]
        protected ListViewAdapter adapter;
        public virtual ListViewAdapter Adapter
        {
            get
            {
                return adapter;
            }
            set
            {
                adapter = value;
            }
        }
        
        protected virtual int CellsCount
        {
            get
            {
                return this.Adapter.GetCount();
            }
        }
        protected virtual void Initialize(Action onComplete = null)
        {
            this.rectTransform = transform.GetComponent<RectTransform>();
            this.scrollRectTransform = transform.parent.GetComponent<RectTransform>();
            this.scrollRect = this.scrollRectTransform.GetComponent<ScrollRect>();
            this.viewPortSize = this.scrollRectTransform.sizeDelta;

            this.cellSizes = new Vector2[this.prefabs.Length];
            for (int i = 0; i < this.prefabs.Length; i++)
            {
                this.prefabs[i].SetActive(false);
                RectTransform cellRect = this.prefabs[i].GetComponent<RectTransform>();
                this.cellSizes[i] = cellRect.sizeDelta;
            }
        }
        
        protected virtual void GenerateCellBuffers()
        {
            this.buffersArray = new List<ListViewCell>[this.prefabs.Length];
            for (int i = 0; i < this.prefabs.Length; i++)
            {
                this.buffersArray[i] = new List<ListViewCell>();
            }
        }
        
        public virtual void RefreshData()
        {
            
        }

        protected virtual float CalculateSizeAndCellPos(float beyondDistance)
        {
            return 0;
        }

        protected virtual void FillData(Vector2 vec2)
        {

        }
        
        public void ClearAllCell()
        {
            int childCount = this.transform.childCount;
    
            for (int i = 0; i < childCount; i++)
            {
                if (i < this.transform.childCount)
                    GameObject.Destroy(this.transform.GetChild(i).gameObject);
            }
        }
    }
}