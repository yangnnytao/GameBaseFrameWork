using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFrame;
using UnityEngine.UI;

public class HallPanel : UIBase
{
    [SerializeField]
    private Button _eventPanelBtn;

    protected override void OnAddListener()
    {
        base.OnAddListener();
        _eventPanelBtn.onClick.AddListener(OnEventPanelBtnTrigger);
    }

    protected override void OnRemoveListener()
    {
        base.OnRemoveListener();
        _eventPanelBtn.onClick.RemoveListener(OnEventPanelBtnTrigger);
    }

    private void OnEventPanelBtnTrigger()
    {
        GUIManager.Instance.ShowUI<RandomEventPanel>("RandomEventPanel", false, false);
    }
}
