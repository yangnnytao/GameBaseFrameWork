using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class X_RectAutoSize : MonoBehaviour
{
    //原始尺寸
    private Vector2 olSize;
    //缩放后的尺寸
    private Vector2 size;
    //原始尺寸宽高比
    private float al;
    private RectTransform self;
    public bool lockHeight;
    public bool lockPos = true;
    [SerializeField]
    private bool _lockWide = false;
    internal float parentHeight;
    public float heightBoder;

    public bool mIsUpdate = false;
    private Image _image;
    private RectTransform _parentRect;
    private void Awake()
    {
        self = GetComponent<RectTransform>();
        _image = self.GetComponent<Image>();
        _parentRect = self.parent.GetComponent<RectTransform>();
        UpdateIcon();
    }
    private void OnEnable()
    {
        UpdateIcon();
    }

    private void Update()
    {
        if (mIsUpdate)
        { 
        
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {

        if (!lockHeight)
        {
            parentHeight = _parentRect.rect.size.y - heightBoder;
        }
        if (_lockWide)
            parentHeight = _parentRect.rect.size.x - heightBoder;

        _image.SetNativeSize();
        olSize = self.sizeDelta;
        if (!_lockWide)
        {
            al = olSize.x / olSize.y;
            size = new Vector2(parentHeight * al, parentHeight);

        }
        else
        {
            al = olSize.y / olSize.x;
            size = new Vector2(parentHeight, parentHeight * al);
        }
        self.sizeDelta = size;
        if (lockPos)
        {
            self.anchoredPosition = Vector2.zero;
        }
        
    }
}