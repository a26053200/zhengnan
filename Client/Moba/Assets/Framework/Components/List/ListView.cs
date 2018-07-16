using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//----------------------
//this scprit only can use on UGUI4.6+
//----------------------
namespace Framework
{
    [RequireComponent(typeof(ScrollRect))]
    [DisallowMultipleComponent]
    public class ListView : MonoBehaviour
    {
        public enum Direction
        {
            automatic = -1,
            left_to_right,
            right_to_left,
            top_to_down,
            down_to_top
        }

        [System.Serializable]
        public class Item
        {
            public GameObject current;
            public Item prev;
            public Item next;
        }

        class Pivot
        {
            static public Vector3 LeftBottom = new Vector3(0, 0);
            static public Vector3 LeftMiddle = new Vector3(0, 0.5f);
            static public Vector3 LeftTop = new Vector3(0, 1);
            static public Vector3 MiddleTop = new Vector3(0.5f, 1);
            static public Vector3 RightTop = new Vector3(1, 1);
            static public Vector3 RightMiddle = new Vector3(1, 0.5f);
            static public Vector3 RightBottom = new Vector3(1, 0);
            static public Vector3 MiddleBottom = new Vector3(0.5f, 0);

        }

        public delegate int SortCompareFunc(Item left, Item right);
        class _SortCompareFunc : IComparer<Item>
        {
            private SortCompareFunc compareFunc = null;
            public _SortCompareFunc(SortCompareFunc comp)
            {
                compareFunc = comp;
            }
            public int Compare(Item left, Item right)
            {
                return compareFunc == null ? 0 : compareFunc(left, right);
            }
        }

        public bool OpenDebugMode = false;
        public bool AutoItemRename = true;
        public bool AutoContentSize = true;
        public Direction ScrollDirection = Direction.left_to_right;
        public Direction GroupItemDirection = Direction.automatic;
        public Vector3 FirstItemOffset = new Vector3(0, 0, 0);
        public Vector3 ItemMargin = new Vector3(0, 0, 0);
        public Vector3 GroupItemMargin = new Vector3(0, 0, 0);
        [Range(1, 100)]
        public int EachOfGroup = 1;

        private List<Item> _listItems = new List<Item>();
        private List<GameObject> _listItemCache = new List<GameObject>();
        private bool _isUpdateListviewDirty = true;
        [SerializeField]
        private GameObject _itemModel;
        private Direction _prevDirection = Direction.left_to_right;
        private Vector3 _prevFirstItemOffset = Vector3.zero;
        private Vector3 _prevItemMargin = Vector3.zero;
        private Vector3 _prevGroupItemMargin = Vector3.zero;
        private bool _prevOpenDebugMode = false;
        private int _prevEachOfGroup = 0;
        private Direction _prevGroupItemDirection = Direction.automatic;
        private Vector3 _oldContentSize = Vector3.zero;
        private Image _imageDebugContentDraw;
        private Vector3 _currentGroupItemPivotPrev = Vector3.zero;
        private Vector3 _currentGroupItemPivotNext = Vector3.zero;

        void Start()
        {
            checkCompoment();

            //check item model set by editor mode
            if (_itemModel != null)
            {
                var modelTmp = _itemModel;
                _itemModel = null;
                setItemModel(modelTmp);
            }

            _prevDirection = ScrollDirection;
            _prevFirstItemOffset = FirstItemOffset;
            _prevItemMargin = ItemMargin;
            _prevGroupItemMargin = GroupItemMargin;
            _prevOpenDebugMode = OpenDebugMode;
            _prevEachOfGroup = EachOfGroup;
            _prevGroupItemDirection = GroupItemDirection;
            _oldContentSize = new Vector3(GetComponent<ScrollRect>().content.rect.width, GetComponent<ScrollRect>().content.rect.height);
        }

        void OnDestroy()
        {
            if (_itemModel != null)
            {
                Destroy(_itemModel);
                _itemModel = null;
            }
        }

        void OnValidate()
        {
            if (_prevDirection != ScrollDirection)
            {
                changeDirection(ScrollDirection);
            }

            if (_prevFirstItemOffset != FirstItemOffset)
            {
                _isUpdateListviewDirty = true;
                _prevFirstItemOffset = FirstItemOffset;
            }

            if (_prevItemMargin != ItemMargin)
            {
                _isUpdateListviewDirty = true;
                _prevItemMargin = ItemMargin;
                _prevGroupItemMargin = GroupItemMargin;
            }

            if (_prevOpenDebugMode != OpenDebugMode)
            {
                checkCompoment();
                _prevOpenDebugMode = OpenDebugMode;
            }

            if (_prevGroupItemMargin != GroupItemMargin)
            {
                if (EachOfGroup <= 1)
                {
                    _prevGroupItemMargin = Vector3.zero;
                    GroupItemMargin = Vector3.zero;
                    Debug.LogError("set GroupItemMargin error, EachOfGroup must > 1");
                }
                else
                {
                    _isUpdateListviewDirty = true;
                    _prevGroupItemMargin = GroupItemMargin;
                }
            }

            if (_prevEachOfGroup != EachOfGroup)
            {
                if (EachOfGroup < 1)
                {
                    Debug.LogError("set EachOfGroup error: value must > 0");
                    _prevEachOfGroup = 1;
                    EachOfGroup = 1;
                }
                else
                {
                    _isUpdateListviewDirty = true;
                    _prevEachOfGroup = EachOfGroup;
                }
            }

            if (_prevGroupItemDirection != GroupItemDirection)
            {
                _isUpdateListviewDirty = true;
                _prevGroupItemDirection = GroupItemDirection;
            }
        }

        public void addItem(GameObject newItem)
        {
            insertItem(newItem, _listItems.Count);
        }

        public void insertItem(GameObject newItem, int index)
        {
            checkCompoment();

            if (index < 0)
            {
                Debug.LogWarning("insertItem warning: out of range, but auto fixed index=" + index);
                index = 0;
            }
            else if (index > _listItems.Count)
            {
                Debug.LogWarning("insertItem warning: out of range, but auto fixed index=" + index);
                index = _listItems.Count;
            }

            Item itemTmp = new Item();
            _listItems.Insert(index, itemTmp);
            int prevIndex = index - 1;
            int nextIndex = index + 1;
            itemTmp.current = newItem;

            if (prevIndex < 0)
            {
                prevIndex = _listItems.Count - 1;
            }
            itemTmp.prev = _listItems[prevIndex];
            _listItems[prevIndex].next = itemTmp;

            if (nextIndex > _listItems.Count - 1)
            {
                nextIndex = 0;
            }
            itemTmp.next = _listItems[nextIndex];
            _listItems[nextIndex].prev = itemTmp;

            ListView.changeParentLocal(itemTmp.current, GetComponent<ScrollRect>().content.gameObject);

            if (AutoItemRename)
            {
                itemTmp.current.name = "item_" + index;
            }

            _isUpdateListviewDirty = true;
        }

        public void addItemByModel()
        {
            if (_itemModel != null)
                addItem(getItemModel());
        }

        public void insertItemByModel(int index)
        {
            if (_itemModel != null)
                insertItem(getItemModel(), index);
        }

        public void removeItem(int index, bool isDestroy = true)
        {
            if (index < 0 || index > _listItems.Count - 1)
            {
                Debug.LogError("ListView removeItem error: out of range");
                return;
            }

            var item = _listItems[index];

            //cut connect
            item.prev.next = item.next;
            item.next.prev = item.prev;
            item.next = null;
            item.prev = null;

            //destroy object
            if (_listItems[index].current)
            {
                if (isDestroy)
                    Destroy(_listItems[index].current);
                else
                {
                    _listItems[index].current.gameObject.SetActive(false);
                    _listItemCache.Add(_listItems[index].current.gameObject);
                }
            }

            _listItems.RemoveAt(index);
            _isUpdateListviewDirty = true;
        }

        public void removeItem(GameObject item, bool isDestroy = true)
        {
            int index = getItemIndex(item);
            if (index >= 0 && index < _listItems.Count)
            {
                removeItem(index, isDestroy);
            }
            else
            {
                Debug.LogError("removeItem error: not find item =" + item);
            }
        }

        public void clearItem(bool isDestroy = true)
        {
            checkCompoment();

            if (isDestroy)
            {
                for (int i = 0; i < _listItems.Count; ++i)
                {
                    Destroy(_listItems[i].current);
                }
            }
            else
            {
                for (int i = 0; i < _listItems.Count; ++i)
                {
                    _listItems[i].current.SetActive(false);
                    _listItemCache.Add(_listItems[i].current);
                }
            }
            _listItems.Clear();

            if (_oldContentSize.x > 0 && _oldContentSize.y > 0)
            {
                GetComponent<ScrollRect>().content.sizeDelta = new Vector2(_oldContentSize.x, _oldContentSize.y);
                GetComponent<ScrollRect>().content.transform.localPosition = Vector3.zero;
            }
        }

        public void changeDirection(Direction dir)
        {
            ScrollDirection = dir;
            _prevDirection = dir;
            _isUpdateListviewDirty = true;
        }

        public void setItemModel(GameObject item)
        {
            if (item == null)
            {
                Debug.LogError("setItemModel error: item is null");
                return;
            }

            if (item.GetComponent<RectTransform>() == null)
            {
                Debug.LogError("setItemModel error: item must contain RectTransform Compoment");
                return;
            }

            if (_itemModel != null)
            {
                Destroy(_itemModel);
                _itemModel = null;
            }

            _itemModel = Instantiate(item) as GameObject;
            _itemModel.SetActive(false);

            DontDestroyOnLoad(_itemModel);

            if (AutoItemRename)
            {
                _itemModel.name = this.name + "_ItemModel";
            }
        }

        public GameObject getItemModel()
        {
            if (_itemModel == null)
            {
                Debug.LogError("getItemModel error: you don't set model, item model is null");
                return null;
            }
            else
            {
                var ret = Instantiate(_itemModel) as GameObject;
                ret.SetActive(true);
                return ret;
            }
        }

        public void Sort(SortCompareFunc compareFunc)
        {
            if (_listItems.Count == 0)
                return;

            List<GameObject> listSwap = new List<GameObject>();
            _listItems.Sort(new _SortCompareFunc(compareFunc));

            while (_listItems.Count > 0)
            {
                listSwap.Add(_listItems[0].current);
                removeItem(0, false);
            }

            for (int i = 0; i < listSwap.Count; ++i)
            {
                addItem(listSwap[i]);
                listSwap[i].gameObject.SetActive(true);
            }
            _isUpdateListviewDirty = true;
        }

        public GameObject getItem(int index)
        {
            GameObject ret = null;
            if (index < 0 || index > _listItems.Count - 1)
            {
                return ret;
            }
            else
            {
                ret = _listItems[index].current;
            }
            return ret;
        }

        public GameObject popItemFromCache()
        {
            GameObject ret = null;
            if (_listItemCache.Count > 0)
            {
                ret = _listItemCache[_listItemCache.Count - 1];
                ret.SetActive(true);
                _listItemCache.RemoveAt(_listItemCache.Count - 1);
            }

            return ret;
        }

        public int getItemIndex(GameObject item)
        {
            int ret = -1;
            for (int i = 0; i < _listItems.Count; ++i)
            {
                if (_listItems[i].current == item)
                {
                    ret = i;
                    break;
                }
            }

            if (ret == -1)
            {
                Debug.LogError("getItemIndex error: not find item =" + item);
                return ret;
            }
            else
                return ret;
        }

        public int Size()
        {
            return _listItems.Count;
        }

        public void pauseScrolling()
        {
            GetComponent<ScrollRect>().horizontal = false;
            GetComponent<ScrollRect>().vertical = false;
        }

        public void resumeScrolling()
        {
            GetComponent<ScrollRect>().horizontal = true;
            GetComponent<ScrollRect>().vertical = true;
        }

        public void updateListView()
        {
            if (_isUpdateListviewDirty)
            {
                _isUpdateListviewDirty = false;

                checkCompoment();

                updateGroupItemDirection();
                updateContentSize();
                for (int i = 0; i < _listItems.Count; ++i)
                {
                    updateItem(i);
                }
            }
        }

        public void checkCompoment()
        {
            if (GetComponent<ScrollRect>() == null)
            {
                Debug.LogError("ListView error: Require ScrollRect Compoment !!!");
            }
            else if (GetComponent<ScrollRect>().content == null)
            {
                //create scroll content automatic
                var contentNew = new GameObject();
                ListView.changeParentLocal(contentNew, this.gameObject);
                var transNew = contentNew.AddComponent<RectTransform>();
                transNew.sizeDelta = GetComponent<RectTransform>().sizeDelta;
                GetComponent<ScrollRect>().content = transNew;

                if (AutoItemRename)
                {
                    contentNew.name = "Content";
                }
            }

            if (OpenDebugMode)
            {
                if (_imageDebugContentDraw == null)
                {
                    var content = GetComponent<ScrollRect>().content;
                    _imageDebugContentDraw = content.GetComponent<Image>();
                    if (_imageDebugContentDraw == null)
                    {
                        _imageDebugContentDraw = content.gameObject.AddComponent<Image>();
                        _imageDebugContentDraw.sprite = null;
                        _imageDebugContentDraw.color = new Color(1, 1, 1, 0.5f);
                    }
                    var rectTrans = _imageDebugContentDraw.GetComponent<RectTransform>();
                    rectTrans.sizeDelta = content.sizeDelta;
                }
                _imageDebugContentDraw.enabled = true;
            }
            else
            {
                if (_imageDebugContentDraw)
                {
                    _imageDebugContentDraw.enabled = false;
                }
            }
        }

        //static public void SortQuickly(List<int> listValue, int begin, int end)
        //{
        //    if (begin >= end)
        //        return;

        //    int left = begin;
        //    int right = end;
        //    int key = listValue[begin];

        //    while (left < right)
        //    {
        //        while (left < right && key <= listValue[right]) --right;

        //        listValue[left] = listValue[right];

        //        while (left < right && key >= listValue[left]) ++left;

        //        listValue[right] = listValue[left];
        //    }

        //    listValue[left] = key;
        //    SortQuickly(listValue, begin, left - 1);
        //    SortQuickly(listValue, left + 1, end);
        //}

        void Update()
        {
            updateListView();
        }

        private void updateItem(int index)
        {
            if (index < 0 || index > _listItems.Count - 1)
            {
                Debug.LogError("ListView updateItem error: out of range");
                return;
            }
            var item = _listItems[index];
            RectTransform content = GetComponent<ScrollRect>().content;

            //calculate position
            Vector3 pivotDecide = Vector3.zero;
            Vector3 pivotAdd = Vector3.zero;

            switch (ScrollDirection)
            {
                case Direction.left_to_right: pivotDecide = Pivot.LeftBottom; pivotAdd = Pivot.RightBottom; break;
                case Direction.right_to_left: pivotDecide = Pivot.RightBottom; pivotAdd = Pivot.LeftBottom; break;
                case Direction.down_to_top: pivotDecide = Pivot.LeftBottom; pivotAdd = Pivot.LeftTop; break;
                case Direction.top_to_down: pivotDecide = Pivot.LeftTop; pivotAdd = Pivot.LeftBottom; break;
                default: Debug.LogError("unsupport direction"); break;
            }

            if (index == 0)
            {
                if (ScrollDirection == Direction.right_to_left)
                    UnityHepler.setLocalPositionByPivot(item.current, new Vector3(content.rect.width, 0), pivotDecide);
                else if (ScrollDirection == Direction.top_to_down)
                    UnityHepler.setLocalPositionByPivot(item.current, new Vector3(0, content.rect.height), pivotDecide);
                else
                {
                    UnityHepler.setLocalPositionByPivot(item.current, Vector3.zero, pivotDecide);
                }
                fixedLocalPositionByContent(item.current);
            }
            else
            {
                //first item of group
                if (index % EachOfGroup == 0)
                {
                    var prevItem = getItem((index / EachOfGroup - 1) * EachOfGroup);
                    Vector3 prevPosition = UnityHepler.getLocalPositioByPivot(prevItem, pivotAdd);
                    UnityHepler.setLocalPositionByPivot(item.current, prevPosition, pivotDecide);

                    fixedItemMargin(item.current, ItemMargin);
                }
                else
                {
                    //next item of group
                    Vector3 prevPosition = UnityHepler.getLocalPositioByPivot(item.prev.current, _currentGroupItemPivotPrev);
                    UnityHepler.setLocalPositionByPivot(item.current, prevPosition, _currentGroupItemPivotNext);

                    fixedItemMargin(item.current, GroupItemMargin);
                }
            }
        }

        private void updateGroupItemDirection()
        {
            if (GroupItemDirection == Direction.automatic)
            {
                switch (ScrollDirection)
                {
                    case Direction.left_to_right: _currentGroupItemPivotPrev = Pivot.LeftBottom; _currentGroupItemPivotNext = Pivot.LeftTop; break;
                    case Direction.right_to_left: _currentGroupItemPivotPrev = Pivot.RightTop; _currentGroupItemPivotNext = Pivot.RightBottom; break;
                    case Direction.down_to_top: _currentGroupItemPivotPrev = Pivot.LeftTop; _currentGroupItemPivotNext = Pivot.LeftBottom; break;
                    case Direction.top_to_down: _currentGroupItemPivotPrev = Pivot.RightBottom; _currentGroupItemPivotNext = Pivot.LeftBottom; break;
                    default: Debug.LogError("unsupport direction"); break;
                }
            }
            else
            {
                switch (GroupItemDirection)
                {
                    case Direction.left_to_right: _currentGroupItemPivotPrev = Pivot.RightBottom; _currentGroupItemPivotNext = Pivot.LeftBottom; break;
                    case Direction.right_to_left: _currentGroupItemPivotPrev = Pivot.LeftBottom; _currentGroupItemPivotNext = Pivot.RightBottom; break;
                    case Direction.down_to_top: _currentGroupItemPivotPrev = Pivot.LeftTop; _currentGroupItemPivotNext = Pivot.LeftBottom; break;
                    case Direction.top_to_down: _currentGroupItemPivotPrev = Pivot.LeftBottom; _currentGroupItemPivotNext = Pivot.LeftTop; break;
                    default: Debug.LogError("unsupport direction"); break;
                }
            }
        }

        private void updateContentSize()
        {
            if (!AutoContentSize)
                return;

            if (_listItems.Count == 0)
                return;

            var content = GetComponent<ScrollRect>().content;
            var allItemSize = getAllSizeItem();

            float widthNew = content.rect.width;
            float heightNew = content.rect.height;
            Vector3 pivotNew = Vector3.zero;

            switch (ScrollDirection)
            {
                case Direction.left_to_right:
                    {
                        widthNew = allItemSize.x + FirstItemOffset.x;
                        heightNew = getMaxHeightItem();
                        pivotNew = Pivot.LeftBottom;
                        break;
                    }
                case Direction.right_to_left:
                    {
                        widthNew = allItemSize.x + FirstItemOffset.x;
                        heightNew = getMaxHeightItem();
                        pivotNew = Pivot.RightBottom;
                        break;
                    }
                case Direction.down_to_top:
                    {
                        widthNew = getMaxWidthItem();
                        heightNew = allItemSize.y + FirstItemOffset.y;
                        pivotNew = Pivot.LeftBottom;
                        break;
                    }
                case Direction.top_to_down:
                    {
                        widthNew = getMaxWidthItem();
                        heightNew = allItemSize.y + FirstItemOffset.y;
                        pivotNew = Pivot.LeftTop;
                        break;
                    }
                default: Debug.LogError("updateContentSize error: unsupport direction !"); break;
            }

            content.sizeDelta = new Vector3(widthNew, heightNew);

            var pos1 = UnityHepler.getWorldPositioByPivot(this.gameObject, pivotNew);
            UnityHepler.setWorldPositionByPivot(content.gameObject, pos1, pivotNew);

            checkCompoment();
        }

        private float getMaxWidthItem()
        {
            float ret = 0;
            for (int i = 0; i < _listItems.Count; ++i)
            {
                var rectTrans = _listItems[i].current.transform.GetComponent<RectTransform>();
                if (ret < rectTrans.rect.width)
                    ret = rectTrans.rect.width;
            }
            return ret;
        }

        private float getMaxHeightItem()
        {
            float ret = 0;
            for (int i = 0; i < _listItems.Count; ++i)
            {
                var rectTrans = _listItems[i].current.transform.GetComponent<RectTransform>();
                if (ret < rectTrans.rect.height)
                    ret = rectTrans.rect.height;
            }
            return ret;
        }

        private Vector3 getAllSizeItem()
        {
            Vector3 ret = Vector3.zero;
            for (int i = 0; i < _listItems.Count; i += EachOfGroup)
            {
                var rectTrans = _listItems[i].current.transform.GetComponent<RectTransform>();
                ret += new Vector3(rectTrans.rect.width, rectTrans.rect.height);
            }

            var absItemMargin = new Vector3(Mathf.Abs(ItemMargin.x), Mathf.Abs(ItemMargin.y), Mathf.Abs(ItemMargin.z));
            ret += ((_listItems.Count - 1) / EachOfGroup) * absItemMargin;

            return ret;
        }

        private void fixedLocalPositionByContent(GameObject target)
        {
            var contentTmp = GetComponent<ScrollRect>().content;

            var rectTrans = GetComponent<ScrollRect>().content.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("fixedLocalPositionByContent error: dose not contain RectTransform !");
                return;
            }

            var rectTransTarget = target.GetComponent<RectTransform>();
            if (rectTransTarget == null)
            {
                Debug.LogError("setLocalPositionByArchor error: target dose not contain RectTransform !");
                return;
            }

            target.transform.localPosition -= new Vector3(contentTmp.pivot.x * contentTmp.rect.width, contentTmp.pivot.y * contentTmp.rect.height);

            if (FirstItemOffset.x != 0 || FirstItemOffset.y != 0)
            {
                target.transform.localPosition += new Vector3(FirstItemOffset.x, FirstItemOffset.y);
            }
        }

        private void fixedItemMargin(GameObject target, Vector3 margin)
        {
            target.transform.localPosition += margin;
        }

        static public void changeParentLocal(GameObject target, GameObject parent)
        {
            var oldPos = target.transform.localPosition;
            var oldScale = target.transform.localScale;

            target.transform.SetParent(parent.transform);

            target.transform.localPosition = oldPos;
            target.transform.localScale = oldScale;
        }

        private string strItemIndex = "0";
        void OnGUI()
        {
            if (!OpenDebugMode)
                return;

            if (GUILayout.Button("InterItem"))
            {
                int indexTmp = 0;
                int.TryParse(strItemIndex, out indexTmp);
                insertItem(getItemModel(), indexTmp);
            }
            strItemIndex = GUILayout.TextField(strItemIndex);
            if (GUILayout.Button("removeItem"))
            {
                int indexTmp = 0;
                int.TryParse(strItemIndex, out indexTmp);
                removeItem(indexTmp);
            }
            if (GUILayout.Button("clearItem"))
            {
                clearItem();
            }
        }

        void OnDrawGizmos()
        {
            if (!OpenDebugMode)
                return;

            Gizmos.color = new Color(1, 1, 1, 0.5f);

            var content = GetComponent<ScrollRect>().content;
            if (content)
            {
                var trans = content.GetComponent<RectTransform>();
                if (trans)
                {
                    Vector3 worldSize = trans.TransformVector(trans.sizeDelta);
                    worldSize.z = 0;
                    Gizmos.DrawCube(trans.position, worldSize);
                }
            }
        }
    }
}
