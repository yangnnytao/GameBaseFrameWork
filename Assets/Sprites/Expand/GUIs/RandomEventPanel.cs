using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFrame;
using UnityEngine.UI;

public class RandomEventPanel : UIBase
{
    [SerializeField]
    private Dropdown _mapListDrop;
    [SerializeField]
    private InputField _posXInput;
    [SerializeField]
    private InputField _posYInput;
    [SerializeField]
    private InputField _posZInput;
    [SerializeField]
    private Button _exitBtn;
    [SerializeField]
    private Button _okBtn;


    private string _curMapName;
    private int _curPosX;
    private int _curPosY;
    private int _curPosZ;

    /// <summary> 需要注册的消息列表 </summary>
    private List<string> MessageList
    {
        get
        {
            return new List<string>()
                {
                    NotiConst.RANDOMEVENTPANEL_SELECTMAP,
                };
        }
    }

    protected override void InitUI()
    {
        base.InitUI();
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);

        List<string> dataList = new List<string>();
        foreach (var tempData in TableDataManager.mRandomEventTableData)
        {
            if (!dataList.Contains(tempData.mName))
                dataList.Add(tempData.mName);
        }

        _mapListDrop.AddOptions(dataList);
    }

    protected override void OnAddListener()
    {
        base.OnAddListener();
        _exitBtn.onClick.AddListener(OnExitBtnTrigger);
        _okBtn.onClick.AddListener(OnOkBtnTrigger);
        _posXInput.onValueChanged.AddListener(SetPosX);
        _posYInput.onValueChanged.AddListener(SetPosY);
        _posZInput.onValueChanged.AddListener(SetPosZ);
    }

    protected override void OnRemoveListener()
    {
        base.OnRemoveListener();
        _exitBtn.onClick.RemoveListener(OnExitBtnTrigger);
        _okBtn.onClick.RemoveListener(OnOkBtnTrigger);
        _posXInput.onValueChanged.RemoveListener(SetPosX);
        _posYInput.onValueChanged.RemoveListener(SetPosY);
        _posZInput.onValueChanged.RemoveListener(SetPosZ);
    }

    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        object body = message.Body;
        switch (name)
        {
            case NotiConst.RANDOMEVENTPANEL_SELECTMAP:
                string tempStr = (string)body;
                _curMapName = tempStr;
                break;
        }
    }

    private void OnOkBtnTrigger()
    {
        RandomEventData tempData = TableDataManager.mRandomEventTableData.Find(x => (x.mPosX == _curPosX && x.mPosY == _curPosY && x.mPosZ == _curPosZ && x.mName == _curMapName));
        int tempInt = tempData.GetRandomEvent;
        string tempStr = TableDataManager.mLanguageTableData.Find(x => x.mBaseID == tempInt).mName;
        GUIManager.Instance.ShowUI<ShowEventPanle>("ShowEventPanle", false, false,
            (data_) => { data_.InitDataM(tempStr); });

    }

    private void OnExitBtnTrigger()
    {
        GUIManager.Instance.PopUI();
    }

    private void SetPosX(string value_)
    {
        _curPosX = int.Parse(value_);
    }
    private void SetPosY(string value_)
    {
        _curPosY = int.Parse(value_);
    }
    private void SetPosZ(string value_)
    {
        _curPosZ = int.Parse(value_);
    }
}
