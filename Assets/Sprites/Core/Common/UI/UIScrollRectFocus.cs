using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// 当前仅支持HorizontalLayoutGroup
/// </summary>
public class UIScrollRectFocus : MonoBehaviour, IBeginDragHandler, IDragHandler,/* IEndDragHandler,*/IPointerDownHandler, IPointerUpHandler
{
    public float scrollSpeed = 8f;
    public Transform UIGrid;

    [SerializeField]
    private bool _autoScroll = false;
    [SerializeField]
    private float _autoScrollTime = 1;
    [SerializeField]
    private ToggleGroup _tipsRoot;
    [SerializeField]
    private Toggle mTipPrefab;
    [SerializeField]
    private bool _isOffsetType = false;

    [Header("Debug")]
    [SerializeField]
    private float _needTime = 0.15f;
    [SerializeField]
    private float _needOffset = 300;
    [SerializeField]
    private bool _moveMulti = false;

    private ScrollRect scrollRect;
    private float[] pageArray;
    private float targetPagePosition = 0f;

    public bool isDrag = false;

    private bool isTriggerIng = false;
    private int pageCount;
    private int currentPage = 0;
    private List<Transform> items = new List<Transform>();
    private List<Toggle> _tips = new List<Toggle>();



    private Vector2 _startPos;
    private Vector2 _endPos;
    private float _movingTime = 0;
    private bool _isInit = false;
    // Use this for initialization
    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    float tempOffset;
    void Update()
    {
        if (!isDrag || _isOffsetType)
        {
            if (scrollRect.horizontal)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPagePosition, scrollSpeed * Time.deltaTime);
            }
            else if (scrollRect.vertical)
            {
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPagePosition, scrollSpeed * Time.deltaTime);
            }
        }
    }
    void OnEnable()
    {
        InitPageArray();
    }


    private void OnDisable()
    {
        StopCoroutine(AutoScrollNext());
    }

    /// <summary>
    /// 初始化获取元素总个数
    /// </summary>
    public void InitPageArray()
    {
        items.Clear();
        //Common.ClearChildren(UIGrid);
        foreach (Transform item in UIGrid)
        {
            if (item.gameObject.activeSelf && !items.Contains(item))
            {
                items.Add(item);
            }
        }
        pageCount = items.Count;

        pageArray = new float[pageCount];
        for (int i = 0; i < pageCount; i++)
        {
            pageArray[i] = (1f / (pageCount - 1)) * i;
        }
        SetTips(pageCount);

        if (this.isActiveAndEnabled && _autoScroll && pageCount > 1)
        {
            StopCoroutine(AutoScrollNext());
            StartCoroutine(AutoScrollNext());
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.transform as RectTransform);
        SetCurrentPageIndex(0);
        if (scrollRect.horizontal)
        {
            scrollRect.horizontalNormalizedPosition = targetPagePosition;
        }
        else if (scrollRect.vertical)
        {
            scrollRect.verticalNormalizedPosition = targetPagePosition;
        }
        _isInit = true;
    }

    public void UpdateData(params object[] objs)
    {
        InitPageArray();
    }


    // Update is called once per frame

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (pageCount == 0) return;
        StartDrop(true);
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (pageCount == 0) return;


    //    float pos = scrollRect.horizontal ? scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition;//总位置
    //    int index = 0;
    //    float offset = Math.Abs(pageArray[index] - pos); //第一位置对于总位置的的偏移
    //    for (int i = 1; i < pageArray.Length; i++)
    //    {
    //        float _offset = Math.Abs(pageArray[i] - pos);//该单位对于总位置的偏移
    //        if (_offset < offset)
    //        {
    //            index = i;
    //            offset = _offset;
    //        }
    //    }
    //    targetPagePosition = pageArray[index];
    //    currentPage = index;
    //    if (_tips.Count > currentPage)
    //        _tips[currentPage].isOn = true;
    //    StopDrop();
    //}

    /// <summary>
    /// 向左移动一个元素
    /// </summary>
    public void ToLeft()
    {
        if (pageArray.Length == 0) return;
        if (currentPage > 0)
        {
            currentPage = currentPage - 1;
            if (_tips.Count > currentPage)
                _tips[currentPage].isOn = true;
            targetPagePosition = pageArray[currentPage];
        }
    }
    /// <summary>
    /// 向右移动一个元素
    /// </summary>
    public void ToRight()
    {
        if (pageArray.Length == 0) return;
        if (currentPage < pageCount - 1)
        {
            currentPage = currentPage + 1;
            if (_tips.Count > currentPage)
                _tips[currentPage].isOn = true;
            targetPagePosition = pageArray[currentPage];
        }
    }
    /// <summary>
    /// 获取当前页码
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPageIndex()
    {
        return currentPage;
    }
    /// <summary>
    /// 设置当前页码
    /// </summary>
    /// <param name="index"></param>
    public void SetCurrentPageIndex(int index)
    {
        if (pageArray.Length == 0) return;
        currentPage = index;
        if (currentPage >= pageArray.Length)
            currentPage = pageArray.Length - 1;
        if (pageArray.Length > 0)
            targetPagePosition = pageArray[currentPage];
        //_tips[currentPage].isOn = true;//设置为显示
    }
    /// <summary>
    /// 获取总页数
    /// </summary>
    /// <returns></returns>
    public int GetTotalPages()
    {
        return pageCount;
    }

    float _time;
    public IEnumerator AutoScrollNext()
    {
        Debug.Log("AutoScrollNext");
        while (true)
        {
            if (!isDrag)
            {
                _time += Time.deltaTime;
                if (_time > _autoScrollTime)
                {
                    _time = 0;
                    if (currentPage >= pageCount - 1)
                    {
                        SetCurrentPageIndex(0);
                        if (_tips.Count > currentPage)
                            _tips[0].isOn = true;
                    }
                    else
                        ToRight();
                }
            }
            else
                _time = 0;
            yield return null;
        }
    }

    Toggle tempToggle;
    private void SetTips(int cout_)
    {
        Clear();
        if (cout_ <= 1) return;
        if (mTipPrefab == null || cout_ == 0) return;
        for (int i = 0; i < cout_; i++)
        {
            tempToggle = CreateTips();
            if (i == 0)
                tempToggle.isOn = true;
            _tips.Add(tempToggle); ;
        }
    }

    private Toggle CreateTips()
    {
        Toggle tempToggle = GameObject.Instantiate(mTipPrefab, _tipsRoot.transform);
        tempToggle.group = _tipsRoot;
        tempToggle.onValueChanged.AddListener(value_ =>
            {
                if (value_)
                    SetCurrentPageIndex(tempToggle.transform.GetSiblingIndex());
            });

        return tempToggle;
    }

    private void Clear()
    {
        if (_tips != null)
        {
            Toggle[] tempToggle = _tips.ToArray();
            for (int i = 0; i < tempToggle.Length; i++)
            {
                tempToggle[i].onValueChanged.RemoveAllListeners();
                Destroy(tempToggle[i].gameObject);
            }
            _tips.Clear();
        }
    }

    private void StartDrop(params object[] objs)
    {
        isDrag = (bool)objs[0];
    }

    private void StopDrop()
    {
        isDrag = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (pageCount == 0) return;
        _startPos = eventData.position;
        _movingTime = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pageCount == 0) return;
        _endPos = eventData.position;
        float offset = Vector2.Distance(_startPos, _endPos);
        bool isleft = _startPos.x > _endPos.x;
        //如果时间很短并且超过一定得距离则跳到最后一页
        if (_moveMulti && _movingTime < _needTime && offset > _needOffset)
        {
            if (isleft)
            {
                SetCurrentPageIndex(pageArray.Length - 1);
                _tips[pageArray.Length - 1].isOn = true;
            }
            else
            {
                SetCurrentPageIndex(0);
                _tips[0].isOn = true;
            }
        }
        else
        {
            if (offset > _needOffset && isleft)
            {
                ToRight();
            }
            else if (offset > _needOffset && !isleft)
            {
                ToLeft();
            }
        }
        //如果时间很长并且超过一定距离则到下一页

        //rest
        _movingTime = 0;
        _startPos = Vector2.zero;
        _endPos = Vector2.zero;
        StopDrop();
    }

    public void OnDrag(PointerEventData eventData)
    {
        StartDrop(true);
        _movingTime += Time.deltaTime;
    }

}
