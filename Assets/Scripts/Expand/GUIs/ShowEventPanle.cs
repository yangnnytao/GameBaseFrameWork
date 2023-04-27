using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BaseFrame;

public class ShowEventPanle : UIBase
{
    [SerializeField]
    private Button _exitBtn;
    [SerializeField]
    private Button _okBtn;
    [SerializeField]
    private Text _dexcText;

    protected override void OnAddListener()
    {
        base.OnAddListener();
        _exitBtn.onClick.AddListener(OnOkBtnTrigger);
        _okBtn.onClick.AddListener(OnOkBtnTrigger);
    }

    protected override void OnRemoveListener()
    {
        base.OnRemoveListener();
        _exitBtn.onClick.AddListener(OnOkBtnTrigger);
        _okBtn.onClick.AddListener(OnOkBtnTrigger);
    }

    public void InitDataM(string desc_)
    {
        _dexcText.text = desc_;
    }

    private void OnOkBtnTrigger()
    {
        GUIManager.Instance.PopUI();
    }
}
