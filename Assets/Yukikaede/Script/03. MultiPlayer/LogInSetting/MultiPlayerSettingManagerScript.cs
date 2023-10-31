using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Tim;//此API包含Photon.Pun與PhotonNetwork
using Photon.Realtime;

public class MultiPlayerSettingManagerScript : MonoBehaviour
{
    //單例宣告
    public static MultiPlayerSettingManagerScript inst;
    public Sprite sprCheckmark;

    [Header("Constant Page")]
    public GameObject objLogIn;
    public GameObject objFriendList;
    public GameObject objTipsSetting;
    public Button btnFriendList;
    public Button btnTipsEnter;
    public Text txtTips;
    private bool bFriendListClick;
    public bool bYukiPass;

    [Header("Initial Page")]
    public GameObject objInitial;
    public InputField iptUserID;
    public InputField iptNickName;
    public Button btnLogIn;
    public Button btnBackToGameMenu;
    public Button btnReconnect;
    public Button btnReconnectAndRejoin;
    private string sGameVersion;

    [Header("Master Server Page")]
    public GameObject objMasterServer;
    public GameObject objLobbySetting;
    public GameObject objSqlLobby;
    public InputField iptLobbyName;
    public InputField iptSqlLobbyCustomRoomPropertiesValue;
    public Dropdown dpLobbyTypeInMaster;
    public Dropdown dpCustomRoomPropertiesKey;
    public Button btnCreateLobby;
    public Button btnJoinLobby;
    public Button btnBackToLogIn;
    public Button btnCreateNewLobby;
    public Button btnBackToServerMenu;
    public Button btnLobby1;
    public Button btnLobby2;
    public Button btnLobby3;
    public Button btnLobby4;
    public Button btnLobby5;
    public Button[] btnLobbyArray;
    private TypedLobby _TypedLobby;
    private int iLobby;
    private bool bCreateLobby;
    private string sLobbyName;
    private string sSqlLobbyKey;
    private string sSqlLobbyValue;

    [Header("Lobby Page")]
    public GameObject objLobby;
    public GameObject objRoomSetting;
    public GameObject objBtnPreCreatedRoom;
    public InputField iptRoomName;
    public Image imgRoomVisible;
    public Image imgRoomListArea;
    public Button btnCreateRoom;
    public Button btnJoinRoom;
    public Button btnLeaveLobby;
    public Button btnCreateNewRoom;
    public Button btnBackToLabby;
    public Button btnRoomVisible;
    public Button btnRooomListRefresh;
    public RoomOptions _RoomOptions;
    public Dictionary<string, GameObject> dictRoomListItems;
    public bool bIsVisible;
    public bool bRoomSelect;
    public string sSelectedRoomNameForJoin;
    private LobbyType _LobbyType;

    [Header("Room Page")]
    public GameObject objRoom;
    public InputField iptChatInput;
    public Image imgGameReady;
    public Button btnGameStart;
    public Button btnGameReady;
    public Button btnLeaveRoom;
    public Button btnChatEnter;   
    public Text txtRoomInfo;
    public Text txtMessage;
    public bool bGameReady;
    public bool bLeaveRoom;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initial();
        BtnSetting();
    }

    public void Initial()
    {
        objInitial.SetActive(true);
        objMasterServer.SetActive(false);
        objLobby.SetActive(false);
        objRoom.SetActive(false);

        iLobby = 0;
        sLobbyName = "";

        bCreateLobby = false;
        bIsVisible = true;
        bGameReady = false;
        bLeaveRoom = false;
        bRoomSelect = false;

        btnLobbyArray = new Button[5] { btnLobby1, btnLobby2, btnLobby3, btnLobby4, btnLobby5 };

        dictRoomListItems = new Dictionary<string, GameObject>();

        LobbyListRefresh();

        /*YukiPass捷徑*/
        bYukiPass = false;
    }

    public void EnterMasterServer()
    {        
        //btnCreateLobby.gameObject.SetActive(true);//創建Lobby，因為已設定好預設的Lobby這邊就不開啟使用
        objLobbySetting.SetActive(false);
        btnCreateNewLobby.gameObject.SetActive(false);
        btnJoinLobby.interactable = true;
        btnJoinLobby.interactable = false;
        if (bLeaveRoom)
        {
            CreateNewLobbyEven();
            MyPhotonNetwork.JoinLobby(_TypedLobby);
            bLeaveRoom = false;
        }
        else if (bYukiPass)
        {
            /*進入Lobby設定*/
            LobbySelectEven(1);
            MyPhotonNetwork.JoinLobby(_TypedLobby);
        }
        else
        {
            objMasterServer.SetActive(true);
        }
        //離開房間後重新回到大廳列表，而非回到MasterServer中的Lobby建立選單
    }

    public void EnetrLobby()
    {
        Debug.Log("EnetrLobby");
        objFriendList.SetActive(false);
        objLobby.SetActive(true);
        btnCreateRoom.gameObject.SetActive(true);
        objMasterServer.SetActive(false);
        objRoomSetting.SetActive(false);
        btnCreateNewRoom.gameObject.SetActive(false);
        btnJoinRoom.interactable = false;
        bRoomSelect = false;
        RoomListUpdate();

        if (bYukiPass)
        {
            Debug.Log("YukiPass");
            /*進入Room設定*/            
            iptRoomName.text = "YukiTestRoom";
            _RoomOptions.IsVisible = bIsVisible;//False : 不會出現在房間列表、不能隨機加入、只能指定房間名稱加入。
            _RoomOptions.IsOpen = true;//False : 不開放玩家加入
            _RoomOptions.PlayerTtl = 1000;//玩家位置保留時間(斷線重連用 ms)
            _RoomOptions.EmptyRoomTtl = 1000;//空房保留時間 (ms)
            _RoomOptions.MaxPlayers = 2;//隨遊戲做修改
                                        //禁用下方功能後，如果房間無限期地保持使用狀態，則事件歷史記錄可能會變得太長而無法加載。默認值：true。 清理緩存和離開用戶的道具。
            _RoomOptions.CleanupCacheOnLeave = true;//當用戶離開時，將其事件和屬性從房間中刪除。這在玩家無法將物品放置在房間中而完全消失的房間中時才有意義。
            string[] expectedUsers;
            expectedUsers = null;

            objLobby.SetActive(false);
            MyPhotonNetwork.JoinOrCreateRoom(iptRoomName.text, _RoomOptions, _TypedLobby, expectedUsers);
        }
    }

    #region 按鍵功能設定 BtnSetting()
    void BtnSetting()
    {
        #region Constant Page
        btnFriendList.onClick.AddListener(delegate ()
        {
            bFriendListClick = !bFriendListClick;
            objFriendList.SetActive(bFriendListClick);
            //開啟時先刷新好友畫面後顯示
        });
        btnTipsEnter.onClick.AddListener(delegate ()
        {
            objTipsSetting.SetActive(false);
        });
        #endregion

        #region Initial Page
        btnLogIn.onClick.AddListener(delegate ()
        {
            if (iptNickName.text != "")
            {
                sGameVersion = Application.version;//取得當前遊戲版本號來避免不同版本的遊戲透過連線出現在一起
                Debug.Log(sGameVersion);

                /*開始連線的相關設定*/
                MyPhotonNetwork.AutomaticallySyncScene = true;
                MyPhotonNetwork.AuthValues = new AuthenticationValues();
                MyPhotonNetwork.AuthValues.UserId = iptUserID.text;
                MyPhotonNetwork.NickName = iptNickName.text;
                MyPhotonNetwork.GameVersion = sGameVersion;

                /*伺服器端連線設定*/
                Debug.Log("PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime : " + MyPhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);
                MyPhotonNetwork.NetworkingClient.AppId = MyPhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
                Debug.Log("PhotonNetwork.ConnectToRegion(\"jp\") : " + MyPhotonNetwork.ConnectToRegion("jp"));

                /*關閉objInitial*/
                objInitial.SetActive(false);

                /*YukiPass捷徑*/
                bYukiPass = iptNickName.text == "yuki" ? true : false;
            }
            else
            {
                string s = "Enter A NickName";
                TipsEven(s);
            }
        });
        btnBackToGameMenu.onClick.AddListener(delegate ()
        {
            Photon.Pun.PhotonNetwork.LoadLevel("MenuScene");
        });
        btnReconnect.onClick.AddListener(delegate ()
        {
            //wait
        });
        btnReconnectAndRejoin.onClick.AddListener(delegate ()
        {
            //wait
        });
        #endregion

        #region ManagerServer Page
        btnCreateLobby.onClick.AddListener(delegate ()
        {
            bCreateLobby = true;
            objLobbySetting.SetActive(true);
            objLobby.SetActive(false);
            btnJoinLobby.interactable = false;
            btnCreateLobby.gameObject.SetActive(false);
            btnCreateNewLobby.gameObject.SetActive(true);
        });
        btnJoinLobby.onClick.AddListener(delegate ()
        {
            if (iLobby != 0)
            {
                Debug.Log("PhotonNetwork.JoinLobby");
                MyPhotonNetwork.JoinLobby(_TypedLobby);
            }
            else
            {
                string s = "Please Select A Lobby";
                TipsEven(s);
            }
        });
        btnBackToLogIn.onClick.AddListener(delegate ()
        {
            MyPhotonNetwork.Disconnect();
            Initial();
        });
        btnCreateNewLobby.onClick.AddListener(delegate ()
        {
            CreateNewLobbyEven();
        });
        btnBackToServerMenu.onClick.AddListener(delegate ()
        {
            btnCreateNewLobby.gameObject.SetActive(false);
            btnCreateLobby.gameObject.SetActive(true);
        });
        btnLobby1.onClick.AddListener(delegate ()
        {
            LobbySelectEven(1);
        });
        btnLobby2.onClick.AddListener(delegate ()
        {
            LobbySelectEven(2);
        });
        btnLobby3.onClick.AddListener(delegate ()
        {
            LobbySelectEven(3);
        });
        btnLobby4.onClick.AddListener(delegate ()
        {
            LobbySelectEven(4);
        });
        btnLobby5.onClick.AddListener(delegate ()
        {
            LobbySelectEven(5);
        });

        /*LobbyType選擇到sql時要開啟sql選單*/
        dpLobbyTypeInMaster.onValueChanged.AddListener(delegate {
            bool b = dpLobbyTypeInMaster.options[dpLobbyTypeInMaster.value].text == "SqlLobby" ? true : false;
            objSqlLobby.SetActive(b);
        });
        #endregion

        #region Lobby Page
        btnCreateRoom.onClick.AddListener(delegate ()
        {
            objRoomSetting.SetActive(true);
            btnJoinRoom.interactable = false;
            btnCreateRoom.gameObject.SetActive(false);
            btnCreateNewRoom.gameObject.SetActive(true);
            Debug.Log("Creat Room");
        });
        btnJoinRoom.onClick.AddListener(delegate ()
        {
            if (sSelectedRoomNameForJoin != "")
            {
                objLobby.SetActive(false);
                MyPhotonNetwork.JoinRoom(sSelectedRoomNameForJoin);
            }
            else
            {
                string s = "Please Select A Room";
                TipsEven(s);
            }
        });
        btnLeaveLobby.onClick.AddListener(delegate ()
        {
            bCreateLobby = false;
            objLobby.SetActive(false);
            LobbyListRefresh();
            MyPhotonNetwork.LeaveLobby();
            Debug.Log("LeaveLobby" + btnLobbyArray.Length);
        });
        btnCreateNewRoom.onClick.AddListener(delegate ()
        {
            CreateNewRoomEven();
        });
        btnRoomVisible.onClick.AddListener(delegate ()
        {
            bIsVisible = !bIsVisible;//Initial()時設定為true，按下按鈕為false
            imgRoomVisible.sprite = bIsVisible ? null : sprCheckmark;
        });
        btnBackToLabby.onClick.AddListener(delegate ()
        {
            btnCreateNewRoom.gameObject.SetActive(false);
            btnCreateRoom.gameObject.SetActive(true);
        });
        btnRooomListRefresh.onClick.AddListener(delegate ()
        {
            RoomListUpdate();
        });
        #endregion

        #region Room Page
        btnGameStart.onClick.AddListener(delegate ()
        {
            MyPhotonNetwork.CurrentRoom.IsOpen = false;
            MyPhotonNetwork.CurrentRoom.IsVisible = false;
            Photon.Pun.PhotonNetwork.LoadLevel("MultiPlayerGameScene");
        });
        btnGameReady.onClick.AddListener(delegate ()
        {
            bGameReady = !bGameReady;//預設為False

            imgGameReady.sprite = bGameReady ? sprCheckmark : null;

            ExitGames.Client.Photon.Hashtable spcp = new ExitGames.Client.Photon.Hashtable();
            spcp.Add("Ready", bGameReady);
            MyPhotonNetwork.SetPlayerCustomProperties(spcp);
        });
        btnLeaveRoom.onClick.AddListener(delegate ()
        {
            MyPhotonNetwork.LeaveRoom();
            objRoom.SetActive(false);
            bLeaveRoom = true;
            Debug.Log("btnLeaveRoom");
        });
        btnChatEnter.onClick.AddListener(delegate ()
        {
            ExitGames.Client.Photon.Hashtable spcp = new ExitGames.Client.Photon.Hashtable();
            spcp.Add("Chat", iptChatInput.text);
            MyPhotonNetwork.SetPlayerCustomProperties(spcp);
            iptChatInput.text = "";
        });
        #endregion
    }
    #endregion
    /*Button Setting End*/


    #region 選擇Lobby後隱藏其他btnLobby的按鈕 LobbySelect(int i)
    /// <summary>
    /// 選擇Lobby後隱藏其他btnLobby的按鈕 LobbySelect(int i)
    /// </summary>
    void LobbySelect(int i)
    {
        for (int j = 0; j < btnLobbyArray.Length; j++)
        {
            if ((i - 1) != j)
            {
                btnLobbyArray[j].interactable = false;
            }
        }
        Text txtLobby = btnLobbyArray[i - 1].GetComponentInChildren<Text>();
        sLobbyName = txtLobby.text;
        CreateNewLobbyEven();
    }
    #endregion

    #region 選擇/取消Lobby的相關事件 LobbySelectEven(int i)
    void LobbySelectEven(int i)
    {
        if (iLobby == 0)
        {
            iLobby = i;
            LobbySelect(iLobby);
            btnJoinLobby.interactable = true;
        }
        else
        {
            LobbyListRefresh();
            btnJoinLobby.interactable = false;
        }
    }
    #endregion

    #region 創建Lobby設定完成並創建 CreateNewLobbyEven()
    /// <summary>
    /// 創建Lobby設定完成並創建 CreateNewLobbyEven()
    /// </summary>
    public void CreateNewLobbyEven()
    {
        _RoomOptions = new RoomOptions();
        if (bCreateLobby)
        {
            if (sLobbyName == "")
            {
                _LobbyType = LobbyType.Default;
            }
            else
            {
                _LobbyType = (LobbyType)System.Enum.Parse(typeof(LobbyType), dpLobbyTypeInMaster.options[dpLobbyTypeInMaster.value].text);

                if (dpLobbyTypeInMaster.options[dpLobbyTypeInMaster.value].text == "SqlLobby")
                {
                    sSqlLobbyKey = "";
                    sSqlLobbyValue = "";
                    sSqlLobbyKey = dpCustomRoomPropertiesKey.options[dpCustomRoomPropertiesKey.value].text;
                    sSqlLobbyValue = iptSqlLobbyCustomRoomPropertiesValue.text;
                    _RoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { sSqlLobbyKey, sSqlLobbyValue } };
                    _RoomOptions.CustomRoomPropertiesForLobby = new string[] { sSqlLobbyKey };
                }
            }
            _TypedLobby = new TypedLobby(iptLobbyName.text, _LobbyType);

            Debug.Log("PhotonNetwork.JoinLobby() : " + MyPhotonNetwork.JoinLobby(_TypedLobby));
            objMasterServer.SetActive(false);
        }
        else
        {
            //將預設的Lobby1~Lobby5的LobbyType設定為SqlLobby模式
            _LobbyType = (LobbyType)System.Enum.Parse(typeof(LobbyType), "SqlLobby");
            sSqlLobbyKey = "";
            sSqlLobbyValue = "";
            switch (iLobby)
            {
                case 1:
                    sSqlLobbyKey = "C0";
                    break;
                case 2:
                    sSqlLobbyKey = "C1";
                    break;
                case 3:
                    sSqlLobbyKey = "C2";
                    break;
                case 4:
                    sSqlLobbyKey = "C3";
                    break;
                case 5:
                    sSqlLobbyKey = "C4";
                    break;
            }
            sSqlLobbyValue = iLobby.ToString();
            _RoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { sSqlLobbyKey, sSqlLobbyValue } };
            _RoomOptions.CustomRoomPropertiesForLobby = new string[] { sSqlLobbyKey };

            if (sLobbyName != "")
            {
                _TypedLobby = new TypedLobby(sLobbyName, _LobbyType);
            }
            else
            {
                string s = "Please Enter A Lobby Name";
                TipsEven(s);
            }
        }
    }
    #endregion

    #region 創建Room設定完成並創建 CreateNewRoomEven()
    /// <summary>
    /// 創建Room設定完成並創建 CreateNewRoomEven()
    /// </summary>
    public void CreateNewRoomEven()
    {
        _RoomOptions.IsVisible = bIsVisible;//False : 不會出現在房間列表、不能隨機加入、只能指定房間名稱加入。
        _RoomOptions.IsOpen = true;//False : 不開放玩家加入
        _RoomOptions.PlayerTtl = 1000;//玩家位置保留時間(斷線重連用 ms)
        _RoomOptions.EmptyRoomTtl = 1000;//空房保留時間 (ms)
        _RoomOptions.MaxPlayers = 2;//隨遊戲做修改
                                    //禁用下方功能後，如果房間無限期地保持使用狀態，則事件歷史記錄可能會變得太長而無法加載。默認值：true。 清理緩存和離開用戶的道具。
        _RoomOptions.CleanupCacheOnLeave = true;//當用戶離開時，將其事件和屬性從房間中刪除。這在玩家無法將物品放置在房間中而完全消失的房間中時才有意義。

        string[] expectedUsers;
        expectedUsers = null;

        if (iptRoomName.text != "")
        {
            objLobby.SetActive(false);
            MyPhotonNetwork.JoinOrCreateRoom(iptRoomName.text, _RoomOptions, _TypedLobby, expectedUsers);
            string s = string.Format(
                "Create Room Finish :\n" +
                "IsVisible : {0}\n" +
                "IsOpen : {1}\n" +
                "MaxPlayers : {2}\n" +
                "_TypedLobby : {3}, {4}, {5}, {6}",
                _RoomOptions.IsVisible,
                _RoomOptions.IsOpen,
                _RoomOptions.MaxPlayers,
                sLobbyName,
                _LobbyType,
                sSqlLobbyKey,
                sSqlLobbyValue
                );
            Debug.Log(s);
        }
        else
        {
            string s = "Please Enter A Room Name";
            TipsEven(s);
        }
    }
    #endregion

    #region 重置LobbyList選單 LobbyListRefresh()
    void LobbyListRefresh()
    {
        iLobby = 0;
        for (int i = 0; i < btnLobbyArray.Length; i++)
        {
            btnLobbyArray[i].interactable = true;
        }
    }
    #endregion

    #region RoomListRefresh相關設定
    #region 清空原有的RoomList資料 RoomListClear()
    /// <summary>
    /// 清空原有的RoomList資料 RoomListClear()
    /// </summary>
    public void RoomListClear()
    {
        foreach (var item in dictRoomListItems.Values)
        {
            Destroy(item);
        }
        dictRoomListItems.Clear();
        Debug.Log("RoomListRefresh - RoomListClear");
    }
    #endregion

    #region 更新Room清單 RoomListUpdate()
    void RoomListUpdate()
    {
        if (_LobbyType == LobbyType.Default)
        {
            //Default模式會自動更新Lobby中的Room資訊
            Debug.Log("LobbyType.Default - 無須此功能");
        }
        else if (_LobbyType == LobbyType.SqlLobby)
        {
            //這功能只限定sqlLobby的模式使用：MyPhotonNetwork.GetCustomRoomList(_TypedLobby, sSqlLobbyFilter); 
            Debug.Log("LobbyType.SqlLobby");
            string sSqlLobbyFilter = string.Format("{0} = '{1}'", sSqlLobbyKey, sSqlLobbyValue);
            //MyPhotonNetwork.GetCustomRoomList(_TypedLobby, sSqlLobbyFilter);
            bool bGetCustomRoomList = MyPhotonNetwork.GetCustomRoomList(_TypedLobby, sSqlLobbyFilter);
            Debug.Log("_TypedLobby : " + _TypedLobby + " sSqlLobbyFilter : " + sSqlLobbyFilter + " , " + bGetCustomRoomList);
        }
        else if (_LobbyType == LobbyType.AsyncRandomLobby)
        {
            Debug.Log("LobbyType.AsyncRandomLobby - 無法使用此功能");
        }
    }
    #endregion

    #region 重新排序RoomList清單 RoomListSorting()
    /// <summary>
    /// 重新排序RoomList清單 RoomListSorting()
    /// </summary>
    public void RoomListSorting()
    {
        int index = 0;
        foreach (var item in dictRoomListItems.Values)
        {
            item.transform.SetSiblingIndex(index);
            index++;
        }
        Debug.Log("RoomListRefresh - RoomListSorting");
    }
    #endregion
    #endregion

    #region Tips事件 TipsEven()
    /// <summary>
    /// Tips事件，須提供一個string資料來做Tips的提示說明 CreateNewRoomEven(string s)
    /// </summary>
    public void TipsEven(string s)
    {
        txtTips.text = "";
        objTipsSetting.SetActive(true);
        txtTips.text = s;
    }
    #endregion

    void Update()
    {
        #region 在Room中聊天可按下Enter鍵輸入訊息
        if (Input.GetKeyDown(KeyCode.Return) && iptChatInput.text != "")
        {
            ExitGames.Client.Photon.Hashtable spcp = new ExitGames.Client.Photon.Hashtable();
            spcp.Add("Chat", iptChatInput.text);
            MyPhotonNetwork.SetPlayerCustomProperties(spcp);
            iptChatInput.text = "";
        }
        #endregion
    }

}
