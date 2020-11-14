using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	public enum DIALOG_INDEX
	{
		CREATE_ROOM = 0,
		ROOM_PSWD = 1,
		MAP_INFO = 2,
		RENAME_MAP = 3,
		ROOM_CONFIG = 4,
		FRIENDS = 5,
		ADD_FRIEND = 6,
		ADD_BAN = 7,
		BUY_TERM = 8,
		SEND_MEMO = 9,
		MEMO = 10,
		RECV_MEMO = 11,
		BUY_CONFIRM = 12,
		PRESENT_CONFIRM = 13,
		MSG_BOX = 14,
		SCRIPT_EDITOR = 0xF,
		MENU_EX = 0x10,
		BACK_CONFIRM = 17,
		EXIT_CONFIRM = 18,
		REGISTER = 19,
		SCRIPT_CMD_SELECTOR = 20,
		TUTOR_COMPLETE = 21,
		SETTING = 22,
		CHANGE_LANG = 23,
		KICK = 24,
		PLAYER_DETAIL = 25,
		CLAN = 26,
		CREATE_CLAN = 27,
		CHANGE_INTRO = 28,
		CHANGE_NOTICE = 29,
		CLAN_CONFIRM = 30,
		RANDOMBOX = 0x1F,
		MARK = 0x20,
		RANDOMBOX_CONFIRM = 33,
		CREATE_MATCH_TEAM = 34,
		MAPEDIT_AUTHORITY = 35,
		ARE_YOU_SURE = 36,
		WAIT_QUEUE = 37,
		RANDOMBOX_SURE = 38,
		REPLACE_TOOL = 39,
		LINE_TOOL = 40,
		REPLACE_DST = 41,
		DOWNLOAD_FEE = 42,
		ITEM_UPGRADE = 43,
		SURE2UNPACK = 44,
		TCGATE = 45,
		TCBOARD = 46,
		MISSION = 47,
		SELECT_USER_MAP = 48,
		OFFICIAL_MAP_ONLY = 49,
		QUICKJOIN = 50,
		ITEM_SETTING = 51,
		WEAPON_CHANGE = 52,
		TUTORHELP = 53,
		MAP_PRICE_CHANGE = 54,
		MAPINTRO = 55,
		MAP_DOWNLOAD = 56,
		MAP_SETTING_CHANGE = 57,
		MAP_EVAL = 58,
		MAP_DETAIL = 59,
		ITEM_COMBINE = 60,
		HELPWINDOW = 61,
		LEVELUP = 62,
		TUTOR_Q_POPUP = 0x3F,
		BUNGEE_GUIDE = 0x40,
		BND_GUIDE = 65,
		BUILD_GUIDE = 66,
		TUTOR_Q_POPUP2 = 67,
		EXPLOSION_ATTACK_GUIDE = 68,
		EXPLOSION_DEFENCE_GUIDE = 69,
		MBUY_TERM = 70,
		INVITE_NOTICE = 71,
		PC_BANG_NOTICE = 72,
		BATTLE_GUIDE = 73,
		TCNETMARBLE = 74,
		TCRESULT = 75,
		LONG_TIME_PLAY = 76,
		RANDOMBOX_SURENETMARBLE = 77,
		SUGGEST_BANISH = 78,
		VOTE_BANISH = 79,
		CHANGE_NICK = 80,
		ACCUSATION = 81,
		ACCUSATION_MAP = 82,
		ZOMBIE_GUIDE = 83,
		FLAG_GUIDE = 84,
		DEFENSE_GUIDE = 85,
		ESCAPE_GUIDE = 86,
		SELF_RESPAWN_CONFIRM = 87,
		NUM = 88,
		USER_MENU = 1025,
		DRAG = 1026
	}

	public CreateRoomDialog createRoom;

	public RoomPswdDialog roomPswd;

	public MapInfoDlg mapInfo;

	public RenameMapDlg renameMap;

	public RoomConfigDialog roomConfig;

	public FriendsDialog friends;

	public AddFriendDialog addFriend;

	public AddBanDialog addBan;

	public BuyTermDialog buyTerm;

	public SendMemoDialog sendMemo;

	public MemoDialog memo;

	public RecvMemoDialog recvMemo;

	public BuyConfirmDialog buyConfirm;

	public PresentConfirmDialog presentConfirm;

	public MsgBoxDialog msgBox;

	public ScriptEditor scriptEditor;

	public MenuEx menuEx;

	public BackConfirmDialog backConfirm;

	public ExitConfirmDialog exitConfirm;

	public RegisterDialog register;

	public ScriptCmdSelector scriptCmdSelector;

	public TutorCompleteDialog tutorComplete;

	public SettingDialog setting;

	public ChangeLangDialog changeLang;

	public KickDialog kick;

	public PlayerDetailDialog playerDetail;

	public ClanDialog clan;

	public CreateClanDialog createClan;

	public ChangeIntroDialog changeIntro;

	public ChangeNoticeDialog changeNotice;

	public ClanConfirmDialog clanConfirm;

	public RandomBoxDialog randomBoxDlg;

	public MarkDialog mark;

	public RandomBoxConfirmDialog randomBoxConfirmDlg;

	public CreateMatchTeamDialog createMatchTeam;

	public MapeditAuthorityDialog mapeditAthorityDlg;

	public AreYouSure areYouSure;

	public WaitQueueDialog waitQueue;

	public RandomBoxSureDialog randomBoxSure;

	public ReplaceToolDialog replaceTool;

	public LineToolDialog lineTool;

	public ReplaceDstDialog replaceDst;

	public DownloadFeeDialog downloadFee;

	public ItemUpgradeDlg itemUpgradeDlg;

	public Sure2UnpackDialog sure2UnpackDlg;

	public TCGateDialog tcGateDlg;

	public TCBoardDialog tcBoardDlg;

	public MissionDialog missionDlg;

	public SelectUserMapDlg selectUserMapDlg;

	public OfficialMapOnly officialMapOnly;

	public QuickJoinDialog quickJoinDlg;

	public ItemSettingDialog itemSettingDlg;

	public WeaponChangeDialog weaponChangeDlg;

	public TutorHelpDialog tutorHelpDlg;

	public MapPriceChangeDlg mapPriceChangeDlg;

	public MapIntroDlg mapIntroDlg;

	public MapDownloadDlg mapDownloadDlg;

	public MapSettingChangeDlg mapSettingChangeDlg;

	public MapEvalDlg mapEvalDlg;

	public MapDetailDlg mapDetailDlg;

	public ItemCombineDialog itemCombineDialog;

	public HelpWindow helpWindow;

	public LevelUpDlg levelUpDlg;

	public TutorPopupDlg tutorPopupDlg;

	public BungeeGuideDialog bungeeGuideDialog;

	public BNDGuideDialog bndGuideDialog;

	public MapEditGuideDialog mapEditGuideDialog;

	public TutorPopupDlg2 tutorPopupDlg2;

	public ExplosionAttackGuideDialog explosionAttackGuideDialog;

	public ExplosionDefenceGuideDialog explosionDefenceGuideDialog;

	public MBuyTermDialog MBuyTermDialog;

	public InviteNoticeDialog inviteNoticeDialog;

	public PCBangDialog pcBangDialog;

	public BattleGuideDialog battleGuideDialog;

	public TCNetmarbleDialog tcNetmarbelDialog;

	public TCResultItemDialog tcResultItemDialog;

	public LongTimePlayDialog longTimePlayDialog;

	public RandomBoxSureNetmarble randomBoxSureNetmarbleDlg;

	public SuggestBanishDialog suggestBanishDialog;

	public VoteBanishDialog voteBanishDialog;

	public ChangeNickDialog changeNickDialog;

	public AccusationDialog accusationDialog;

	public AccusationMapDialog accusationMapDialog;

	public ZombieGuideDialog zombieGuideDialog;

	public FLAGGuideDialog flagGuideDialog;

	public DefenseGuideDialog defenseGuideDialog;

	public EscapeGuideDialog escapeGuideDialog;

	public SelfRespawnDialog selfRespawnDialog;

	private Stack<Dialog> popup;

	private Dialog[] silo;

	private GUI.WindowFunction[] functions;

	private static DialogManager _instance;

	private DIALOG_INDEX next = DIALOG_INDEX.NUM;

	private string param1 = string.Empty;

	public bool IsModal => popup.Count > 0;

	public static DialogManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(DialogManager)) as DialogManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the DialogManager Instance" + StackTraceUtility.ExtractStackTrace());
				}
			}
			return _instance;
		}
	}

	public bool IsSelfModal(DIALOG_INDEX id)
	{
		if (popup.Count == 1 && IsPopup(id))
		{
			return true;
		}
		return false;
	}

	public bool IsPopup(DIALOG_INDEX id)
	{
		foreach (Dialog item in popup)
		{
			if (item.ID == id)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		popup = new Stack<Dialog>();
		silo = new Dialog[88]
		{
			createRoom,
			roomPswd,
			mapInfo,
			renameMap,
			roomConfig,
			friends,
			addFriend,
			addBan,
			buyTerm,
			sendMemo,
			memo,
			recvMemo,
			buyConfirm,
			presentConfirm,
			msgBox,
			scriptEditor,
			menuEx,
			backConfirm,
			exitConfirm,
			register,
			scriptCmdSelector,
			tutorComplete,
			setting,
			changeLang,
			kick,
			playerDetail,
			clan,
			createClan,
			changeIntro,
			changeNotice,
			clanConfirm,
			randomBoxDlg,
			mark,
			randomBoxConfirmDlg,
			createMatchTeam,
			mapeditAthorityDlg,
			areYouSure,
			waitQueue,
			randomBoxSure,
			replaceTool,
			lineTool,
			replaceDst,
			downloadFee,
			itemUpgradeDlg,
			sure2UnpackDlg,
			tcGateDlg,
			tcBoardDlg,
			missionDlg,
			selectUserMapDlg,
			officialMapOnly,
			quickJoinDlg,
			itemSettingDlg,
			weaponChangeDlg,
			tutorHelpDlg,
			mapPriceChangeDlg,
			mapIntroDlg,
			mapDownloadDlg,
			mapSettingChangeDlg,
			mapEvalDlg,
			mapDetailDlg,
			itemCombineDialog,
			helpWindow,
			levelUpDlg,
			tutorPopupDlg,
			bungeeGuideDialog,
			bndGuideDialog,
			mapEditGuideDialog,
			tutorPopupDlg2,
			explosionAttackGuideDialog,
			explosionDefenceGuideDialog,
			MBuyTermDialog,
			inviteNoticeDialog,
			pcBangDialog,
			battleGuideDialog,
			tcNetmarbelDialog,
			tcResultItemDialog,
			longTimePlayDialog,
			randomBoxSureNetmarbleDlg,
			suggestBanishDialog,
			voteBanishDialog,
			changeNickDialog,
			accusationDialog,
			accusationMapDialog,
			zombieGuideDialog,
			flagGuideDialog,
			defenseGuideDialog,
			escapeGuideDialog,
			selfRespawnDialog
		};
		for (int i = 0; i < silo.Length; i++)
		{
			if (silo[i] != null)
			{
				silo[i].Start();
			}
		}
		Object.DontDestroyOnLoad(this);
	}

	private void ErasePopup(Dialog dlg)
	{
		Stack<Dialog> stack = new Stack<Dialog>();
		while (popup.Count > 0)
		{
			Dialog dialog = popup.Pop();
			if (dialog.ID != dlg.ID)
			{
				stack.Push(dialog);
			}
		}
		while (stack.Count > 0)
		{
			popup.Push(stack.Pop());
		}
	}

	public void CloseAll()
	{
		CloseAll(DIALOG_INDEX.NUM);
	}

	public void CloseAll(DIALOG_INDEX id)
	{
		while (popup.Count > 0)
		{
			popup.Pop()?.OnClose(id);
		}
	}

	public Dialog Popup(DIALOG_INDEX id, bool exclusive)
	{
		Dialog dialog = silo[(int)id];
		if (dialog != null)
		{
			if (exclusive)
			{
				CloseAll(id);
			}
			ErasePopup(dialog);
			dialog.OnPopup();
			popup.Push(dialog);
		}
		return dialog;
	}

	public void Clear()
	{
		Dialog dialog = null;
		while (popup.Count > 0)
		{
			if (popup.Peek().ID == DIALOG_INDEX.MSG_BOX)
			{
				dialog = popup.Peek();
			}
			popup.Pop();
		}
		if (dialog != null)
		{
			popup.Push(dialog);
		}
	}

	private void OnDisable()
	{
	}

	private void windowFunction(int id)
	{
		if (silo[id].DoDialog())
		{
			bool flag = false;
			while (popup.Count > 0 && !flag)
			{
				Dialog dialog = popup.Pop();
				if (dialog.ID == (DIALOG_INDEX)id)
				{
					flag = true;
					dialog.OnClose(DIALOG_INDEX.NUM);
				}
			}
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(null);
		GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
		if (null != gUISkin)
		{
			GUI.skin = gUISkin;
			Dialog[] array = popup.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				GUI.enabled = (i == 0);
				if (array[i].ID == DIALOG_INDEX.MEMO && i > 0 && array[0].ID == DIALOG_INDEX.RECV_MEMO)
				{
					GUI.enabled = true;
				}
				Rect clientRect = array[i].ClientRect;
				clientRect = GUI.Window((int)array[i].ID, clientRect, windowFunction, string.Empty, array[i].WindowStyle);
			}
			GUI.enabled = true;
			if (!ContextMenuManager.Instance.IsPopup)
			{
				if (array.Length > 0)
				{
					GUI.BringWindowToFront((int)array[0].ID);
				}
				for (int j = 1; j < array.Length; j++)
				{
					GUI.BringWindowToBack((int)array[j].ID);
				}
			}
		}
		GlobalVars.Instance.EndGUI();
	}

	public Dialog GetDialogAlways(DIALOG_INDEX id)
	{
		if (DIALOG_INDEX.CREATE_ROOM <= id && (int)id < silo.Length)
		{
			return silo[(int)id];
		}
		return null;
	}

	public Dialog GetDialog(DIALOG_INDEX id)
	{
		Dialog[] array = popup.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].ID == id)
			{
				return array[i];
			}
		}
		return null;
	}

	public Dialog GetTop()
	{
		if (popup.Count > 0)
		{
			return popup.Peek();
		}
		return null;
	}

	private void Start()
	{
	}

	public void Push(DIALOG_INDEX id, string _param1 = "")
	{
		next = id;
		param1 = _param1;
	}

	private void Update()
	{
		if (popup != null)
		{
			Dialog[] array = popup.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Update();
			}
		}
		if (next != DIALOG_INDEX.NUM && !IsModal)
		{
			Dialog dialog = Popup(next, exclusive: true);
			if (dialog != null && next == DIALOG_INDEX.MEMO)
			{
				MemoDialog memoDialog = (MemoDialog)dialog;
				if (memoDialog != null)
				{
					memoDialog.InitDialog();
					memoDialog.ReplyTitle(param1);
				}
			}
			next = DIALOG_INDEX.NUM;
		}
	}
}
