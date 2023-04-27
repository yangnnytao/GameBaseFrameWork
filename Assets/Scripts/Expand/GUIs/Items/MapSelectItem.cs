using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BaseFrame;

public class MapSelectItem : UIBase
{
    [SerializeField]
    private string _countryName;

    [SerializeField]
    private Toggle _toggle;

    protected override void InitUI()
    {
        base.InitUI();
        _toggle = this.GetComponent<Toggle>();
        _countryName = this.transform.Find("Item Label").GetComponent<Text>().text;
    }

    protected override void OnAddListener()
    {
        base.OnAddListener();
        if (_toggle != null)
            _toggle.onValueChanged.AddListener(OntoggleTrigger);
    }

    protected override void OnRemoveListener()
    {
        base.OnRemoveListener();
        if (_toggle != null)
            _toggle.onValueChanged.RemoveListener(OntoggleTrigger);
    }

    public void OntoggleTrigger(bool bool_)
    {
        if (bool_ && !string.IsNullOrEmpty(_countryName))
            AppFacade.Instance.SendMessageCommand(NotiConst.RANDOMEVENTPANEL_SELECTMAP, _countryName);
    }
}
