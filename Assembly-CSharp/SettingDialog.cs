using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingDialog : Dialog
{
	public enum INVITE_SETTING
	{
		ALL_ALLOW,
		FRIEND_ONLY,
		ALL_NOT
	}

	public enum WHISPER_SETTING
	{
		ALL_ALLOW,
		FRIEND_ONLY,
		ALL_NOT
	}

	private bool fullScreen;

	private bool uiScale;

	private int minScreenWidth = 800;

	private int minScreenHeight = 600;

	private int curRes = -1;

	private Resolution currentResolution;

	private Resolution[] supportedResolutions;

	private string[] supportedResolutionDescs;

	private int currentQualityLevel;

	private Color clrSubTitle = new Color(0.95f, 0.59f, 0.09f);

	private Color clrCommon = Color.white;

	private float cameraSpeedFactor = 1f;

	private Vector2 spRes = Vector2.zero;

	public Texture2D icon;

	public string[] mainTabKey;

	private Rect crdTab = new Rect(17f, 67f, 739f, 28f);

	private Rect crdOutline = new Rect(11f, 93f, 751f, 406f);

	private Rect crdOk = new Rect(622f, 507f, 140f, 34f);

	private Vector2 crdResolutionLabel = new Vector2(26f, 116f);

	private Vector2 crdGraphicQualityLabel = new Vector2(406f, 116f);

	private Rect crdFullscreen = new Rect(26f, 460f, 21f, 22f);

	private Vector2 crdSupportedResolutionSize = new Vector2(295f, 24f);

	private Rect crdSupportedResolutionList = new Rect(43f, 172f, 315f, 265f);

	private Rect crdSupportedResolutionListArea = new Rect(27f, 152f, 346f, 302f);

	private Rect crdQualitySlider = new Rect(420f, 176f, 312f, 10f);

	private Rect crdQualityOutline = new Rect(407f, 160f, 335f, 68f);

	private Vector2 crdSimpleLabel = new Vector2(417f, 195f);

	private Vector2 crdBestLabel = new Vector2(730f, 195f);

	private Rect crdUIScale = new Rect(407f, 238f, 21f, 22f);

	private Vector2 crdSfxLabel = new Vector2(26f, 116f);

	private Rect crdSfxMute = new Rect(48f, 192f, 21f, 22f);

	private Rect crdSfxSlider = new Rect(48f, 165f, 300f, 10f);

	private Rect crdSfxRect = new Rect(26f, 146f, 346f, 84f);

	private Vector2 crdSfxValue = new Vector2(345f, 191f);

	private float sfxVolume;

	private bool sfxMute;

	private Vector2 crdBgmLabel = new Vector2(27f, 295f);

	private Rect crdBgmMute = new Rect(48f, 370f, 21f, 22f);

	private Rect crdBgmVolumeSlider = new Rect(48f, 344f, 300f, 10f);

	private Rect crdBgmVolumeRect = new Rect(26f, 326f, 346f, 84f);

	private Vector2 crdBgmValue = new Vector2(345f, 370f);

	private Vector2 crdRadioLabel = new Vector2(406f, 116f);

	private Rect crdRadioSndMute = new Rect(432f, 154f, 21f, 22f);

	private float bgmVolume;

	private bool bgmMute;

	private bool radioSndMute = true;

	private int selVoc;

	private GUIContent selVoiceContent;

	private ComboBox cboxVoice;

	private GUIContent[] listVoice;

	private int selCF;

	private GUIContent selCFContent;

	private ComboBox cboxCF;

	private GUIContent[] listCF;

	private int selLang;

	private GUIContent selLangContent;

	private ComboBox cboxLang;

	private GUIContent[] listLang;

	private Vector2 crdVoiceLabel = new Vector2(25f, 190f);

	private Rect crdCBoxVoice = new Rect(25f, 220f, 200f, 25f);

	private Rect crdApplyVoice = new Rect(236f, 216f, 140f, 34f);

	private Vector2 crdCFLabel = new Vector2(406f, 116f);

	private Rect crdCBoxCF = new Rect(406f, 145f, 200f, 25f);

	private Rect crdApplyCF = new Rect(613f, 141f, 140f, 34f);

	private Vector2 crdLangLabel = new Vector2(26f, 116f);

	private Rect crdCBoxLang = new Rect(25f, 145f, 200f, 25f);

	private Rect crdApplyLang = new Rect(236f, 141f, 140f, 34f);

	private Rect crdSensibilitySlider = new Rect(48f, 165f, 300f, 10f);

	private Vector2 crdSensibility = new Vector2(345f, 191f);

	private Rect crdSensibilityRect = new Rect(26f, 146f, 346f, 84f);

	private Vector2 crdMouseSensibilityLabel = new Vector2(26f, 116f);

	private Vector2 crdMouseAccessChange = new Vector2(26f, 258f);

	private Rect crdReverseMouse = new Rect(30f, 287f, 21f, 22f);

	private Rect crdLeftRightInBuild = new Rect(30f, 326f, 21f, 22f);

	private Rect crdReverseMouseWheel = new Rect(30f, 365f, 21f, 22f);

	private Rect crdKeyMapArea = new Rect(27f, 128f, 719f, 309f);

	private Rect crdKeyMapPosition = new Rect(36f, 140f, 700f, 285f);

	private Rect crdDefault = new Rect(254f, 452f, 265f, 34f);

	private Vector2 crdKeyName = new Vector2(150f, 24f);

	private Vector2 crdKeyValue = new Vector2(280f, 24f);

	private string focusedKey = string.Empty;

	private Vector2 spKeyMap = Vector2.zero;

	private Dictionary<string, KeyCode> inputKeyCommon;

	private Dictionary<string, KeyCode> inputKeyShooter;

	private Dictionary<string, KeyCode> inputKeyBuilder;

	private Dictionary<string, KeyCode> inputKeyWeaponChange;

	private Dictionary<string, KeyCode> inputKeyBungee;

	private float newInputDelta;

	private int tab;

	private string[] mainTab;

	private bool reverseMouse;

	private bool switchLRBuild;

	private bool reverseMouseWheel;

	private Rect crdHideOurForcesNickname = new Rect(40f, 122f, 21f, 22f);

	private Rect crdHideEnemyForcesNickname = new Rect(40f, 152f, 21f, 22f);

	private bool hideOurForcesNickname;

	private bool hideEnemyForcesNickname;

	private Vector2 crdInviteSetLabel = new Vector2(40f, 192f);

	private Rect crdInviteAllow = new Rect(40f, 222f, 21f, 22f);

	private Rect crdInviteFriend = new Rect(40f, 252f, 21f, 22f);

	private Rect crdInviteNo = new Rect(40f, 282f, 21f, 22f);

	private Vector2 crdWhisperSetLabel = new Vector2(40f, 322f);

	private Rect crdWhisperAllow = new Rect(40f, 352f, 21f, 22f);

	private Rect crdWhisperFriend = new Rect(40f, 382f, 21f, 22f);

	private Rect crdWhisperNo = new Rect(40f, 412f, 21f, 22f);

	private bool InviteAllow;

	private bool InviteFriend;

	private bool InviteNo;

	private INVITE_SETTING inviteSetting;

	private bool WhisperAllow;

	private bool WhisperFriend;

	private bool WhisperNo;

	private WHISPER_SETTING whisperSetting;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SETTING;
		mainTab = new string[mainTabKey.Length];
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	private void RefreshTabNames()
	{
		for (int i = 0; i < mainTabKey.Length; i++)
		{
			mainTab[i] = StringMgr.Instance.Get(mainTabKey[i]);
		}
	}

	private void DoEtc()
	{
		hideOurForcesNickname = GUI.Toggle(crdHideOurForcesNickname, hideOurForcesNickname, StringMgr.Instance.Get("HIDE_OUR_FORCES_NICKNAME"));
		hideEnemyForcesNickname = GUI.Toggle(crdHideEnemyForcesNickname, hideEnemyForcesNickname, StringMgr.Instance.Get("HIDE_ENEMY_FORCES_NICKNAME"));
		InviteAllow = false;
		InviteFriend = false;
		InviteNo = false;
		switch (inviteSetting)
		{
		case INVITE_SETTING.ALL_ALLOW:
			InviteAllow = true;
			break;
		case INVITE_SETTING.FRIEND_ONLY:
			InviteFriend = true;
			break;
		case INVITE_SETTING.ALL_NOT:
			InviteNo = true;
			break;
		}
		LabelUtil.TextOut(crdInviteSetLabel, StringMgr.Instance.Get("INVITE_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		InviteAllow = GUI.Toggle(crdInviteAllow, InviteAllow, StringMgr.Instance.Get("INVITE_ALLOW"));
		if (inviteSetting != 0 && InviteAllow)
		{
			inviteSetting = INVITE_SETTING.ALL_ALLOW;
			InviteFriend = false;
			InviteNo = false;
		}
		InviteFriend = GUI.Toggle(crdInviteFriend, InviteFriend, StringMgr.Instance.Get("INVITE_ALLOW_FRIEND"));
		if (inviteSetting != INVITE_SETTING.FRIEND_ONLY && InviteFriend)
		{
			inviteSetting = INVITE_SETTING.FRIEND_ONLY;
			InviteNo = false;
		}
		InviteNo = GUI.Toggle(crdInviteNo, InviteNo, StringMgr.Instance.Get("INVITE_ALLOW_NOT"));
		if (inviteSetting != INVITE_SETTING.ALL_NOT && InviteNo)
		{
			inviteSetting = INVITE_SETTING.ALL_NOT;
		}
		switch (inviteSetting)
		{
		case INVITE_SETTING.ALL_ALLOW:
			InviteAllow = true;
			break;
		case INVITE_SETTING.FRIEND_ONLY:
			InviteFriend = true;
			break;
		case INVITE_SETTING.ALL_NOT:
			InviteNo = true;
			break;
		}
		WhisperAllow = false;
		WhisperFriend = false;
		WhisperNo = false;
		switch (whisperSetting)
		{
		case WHISPER_SETTING.ALL_ALLOW:
			WhisperAllow = true;
			break;
		case WHISPER_SETTING.FRIEND_ONLY:
			WhisperFriend = true;
			break;
		case WHISPER_SETTING.ALL_NOT:
			WhisperNo = true;
			break;
		}
		LabelUtil.TextOut(crdWhisperSetLabel, StringMgr.Instance.Get("WHISPER_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		WhisperAllow = GUI.Toggle(crdWhisperAllow, WhisperAllow, StringMgr.Instance.Get("WHISPER_SET_ALL"));
		if (whisperSetting != 0 && WhisperAllow)
		{
			whisperSetting = WHISPER_SETTING.ALL_ALLOW;
			WhisperFriend = false;
			WhisperNo = false;
		}
		WhisperFriend = GUI.Toggle(crdWhisperFriend, WhisperFriend, StringMgr.Instance.Get("INVITE_ALLOW_FRIEND"));
		if (whisperSetting != WHISPER_SETTING.FRIEND_ONLY && WhisperFriend)
		{
			whisperSetting = WHISPER_SETTING.FRIEND_ONLY;
			WhisperNo = false;
		}
		WhisperNo = GUI.Toggle(crdWhisperNo, WhisperNo, StringMgr.Instance.Get("INVITE_ALLOW_NOT"));
		if (whisperSetting != WHISPER_SETTING.ALL_NOT && WhisperNo)
		{
			whisperSetting = WHISPER_SETTING.ALL_NOT;
		}
		switch (whisperSetting)
		{
		case WHISPER_SETTING.ALL_ALLOW:
			WhisperAllow = true;
			break;
		case WHISPER_SETTING.FRIEND_ONLY:
			WhisperFriend = true;
			break;
		case WHISPER_SETTING.ALL_NOT:
			WhisperNo = true;
			break;
		}
	}

	private void DoGraphic()
	{
		LabelUtil.TextOut(crdResolutionLabel, StringMgr.Instance.Get("RESOLUTION"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdSupportedResolutionListArea, string.Empty, "BoxInnerLine");
		GUI.enabled = !BuildOption.Instance.Props.isWebPlayer;
		Rect rect = new Rect(0f, 0f, crdSupportedResolutionSize.x, (float)supportedResolutionDescs.Length * crdSupportedResolutionSize.y);
		fullScreen = GUI.Toggle(crdFullscreen, fullScreen, StringMgr.Instance.Get("FULL_SCREEN"));
		spRes = GUI.BeginScrollView(crdSupportedResolutionList, spRes, rect);
		curRes = GUI.SelectionGrid(rect, curRes, supportedResolutionDescs, 1, "SelectBlue");
		GUI.EndScrollView();
		GUI.enabled = true;
		GUI.Box(crdQualityOutline, string.Empty, "BoxBlue");
		LabelUtil.TextOut(crdGraphicQualityLabel, StringMgr.Instance.Get("GRAPHIC_QUALITY"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		float value = (float)currentQualityLevel;
		value = GUI.HorizontalSlider(crdQualitySlider, value, 0f, 5f);
		currentQualityLevel = Mathf.RoundToInt(value);
		LabelUtil.TextOut(crdSimpleLabel, StringMgr.Instance.Get("SIMPLE_OPTION"), "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdBestLabel, StringMgr.Instance.Get("BEST_OPTION"), "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		uiScale = GUI.Toggle(crdUIScale, uiScale, StringMgr.Instance.Get("UI_SCALE"));
	}

	private void DoSound()
	{
		GUI.Box(crdSfxRect, string.Empty, "BoxBlue");
		GUI.Box(crdBgmVolumeRect, string.Empty, "BoxBlue");
		LabelUtil.TextOut(crdSfxLabel, StringMgr.Instance.Get("SFX"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		sfxMute = GUI.Toggle(crdSfxMute, sfxMute, StringMgr.Instance.Get("MUTE"));
		sfxVolume = GUI.HorizontalSlider(crdSfxSlider, sfxVolume, 0f, 1f);
		LabelUtil.TextOut(crdSfxValue, sfxVolume.ToString("0.#"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdBgmLabel, StringMgr.Instance.Get("BGM"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bgmMute = GUI.Toggle(crdBgmMute, bgmMute, StringMgr.Instance.Get("MUTE"));
		bgmVolume = GUI.HorizontalSlider(crdBgmVolumeSlider, bgmVolume, 0f, 1f);
		LabelUtil.TextOut(crdBgmValue, bgmVolume.ToString("0.#"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdRadioLabel, StringMgr.Instance.Get("RADIO_SOUND"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (sfxMute)
		{
			radioSndMute = true;
			GUI.enabled = false;
		}
		radioSndMute = GUI.Toggle(crdRadioSndMute, radioSndMute, StringMgr.Instance.Get("MUTE"));
		if (sfxMute)
		{
			GUI.enabled = true;
		}
	}

	private void DoLanguage()
	{
		selLang = cboxLang.List(crdCBoxLang, selLangContent, listLang);
		selLangContent = listLang[selLang];
		LabelUtil.TextOut(crdLangLabel, StringMgr.Instance.Get("LANGUAGE_SELECT"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (GlobalVars.Instance.MyButton(crdApplyLang, StringMgr.Instance.Get("APPLY2"), "BtnAction") && 0 <= selLang && selLang < BuildOption.Instance.Props.supportLanguages.Length)
		{
			int num = (int)BuildOption.Instance.Props.supportLanguages[selLang];
			PlayerPrefs.SetInt("BfLanguage", num);
			LangOptManager.Instance.LangOpt = num;
			GUISkinFinder.Instance.LanguageChanged();
			if (BuildOption.Instance.Props.ShowAgb)
			{
				GlobalVars.Instance.LoadAbg();
			}
			cboxVoice = null;
			listVoice = null;
			selVoiceContent = null;
			cboxLang = null;
			listLang = null;
			selLangContent = null;
			cboxCF = null;
			listCF = null;
			selCFContent = null;
			CreateVoice();
			CreateLang();
			CreateCountryFilter();
		}
	}

	private void DoVoice()
	{
		bool enabled = GUI.enabled;
		GUI.enabled = !cboxLang.IsClickedComboButton();
		if (selVoiceContent == null)
		{
			Debug.LogError("SelVoiceContent is NULL");
		}
		selVoc = cboxVoice.List(crdCBoxVoice, selVoiceContent, listVoice);
		GUI.enabled = enabled;
		selVoiceContent = listVoice[selVoc];
		LabelUtil.TextOut(crdVoiceLabel, StringMgr.Instance.Get("VOICE"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (GlobalVars.Instance.MyButton(crdApplyVoice, StringMgr.Instance.Get("APPLY2"), "BtnAction"))
		{
			PlayerPrefs.SetInt("BfVoice", selVoc);
			PlayerPrefs.SetInt("BfVoiceSet", 1);
			string langVoc = BuildOption.Instance.Props.GetLangVoc(selVoc);
			langVoc += ".unity3d";
			int langVoiceVer = BuildOption.Instance.Props.GetLangVoiceVer(selVoc);
			GlobalVars.Instance.ResetVoices(langVoc, langVoiceVer);
		}
	}

	private void DoCountryFilter()
	{
		selCF = cboxCF.List(crdCBoxCF, selCFContent, listCF);
		selCFContent = listCF[selCF];
		LabelUtil.TextOut(crdCFLabel, StringMgr.Instance.Get("LOCAL_FILTER"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (GlobalVars.Instance.MyButton(crdApplyCF, StringMgr.Instance.Get("APPLY2"), "BtnAction"))
		{
			Property props = BuildOption.Instance.Props;
			if (props != null && 0 <= selCF && selCF <= props.Filters.Length)
			{
				int num = selCF - 1;
				CSNetManager.Instance.Sock.SendCS_CHG_COUNTRY_FILTER_REQ((int)((0 > num || num >= props.Filters.Length) ? BuildOption.COUNTRY_FILTER.NONE : props.Filters[num]));
			}
		}
	}

	private bool IsWaitingInput()
	{
		return focusedKey.Length > 0 && custom_inputs.Instance.IsDefined(focusedKey);
	}

	private void SetNoneSameKeyInCommon(KeyCode key)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, KeyCode> item in inputKeyCommon)
		{
			if (item.Value == key)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputKeyCommon[list[i]] = KeyCode.None;
		}
	}

	private void SetNoneSameKeyInShooter(KeyCode key)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, KeyCode> item in inputKeyShooter)
		{
			if (item.Value == key)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputKeyShooter[list[i]] = KeyCode.None;
		}
	}

	private void SetNoneSameKeyInBuilder(KeyCode key)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, KeyCode> item in inputKeyBuilder)
		{
			if (item.Value == key)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputKeyBuilder[list[i]] = KeyCode.None;
		}
	}

	private void SetNoneSameKeyInWeaponChange(KeyCode key)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, KeyCode> item in inputKeyWeaponChange)
		{
			if (item.Value == key)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputKeyWeaponChange[list[i]] = KeyCode.None;
		}
	}

	private void SetNoneSameKeyInBungee(KeyCode key)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, KeyCode> item in inputKeyBungee)
		{
			if (item.Value == key)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputKeyBungee[list[i]] = KeyCode.None;
		}
	}

	private void SetNoneSameKey(KeyDef.CATEGORY category, KeyCode key)
	{
		switch (category)
		{
		case KeyDef.CATEGORY.COMMON:
			SetNoneSameKeyInCommon(key);
			SetNoneSameKeyInShooter(key);
			SetNoneSameKeyInBuilder(key);
			SetNoneSameKeyInBungee(key);
			break;
		case KeyDef.CATEGORY.SHOOTER_MODE:
			SetNoneSameKeyInCommon(key);
			SetNoneSameKeyInShooter(key);
			break;
		case KeyDef.CATEGORY.BUILD_MODE:
			SetNoneSameKeyInCommon(key);
			SetNoneSameKeyInBuilder(key);
			break;
		case KeyDef.CATEGORY.WEAPON_CHANGE:
			SetNoneSameKeyInCommon(key);
			SetNoneSameKeyInWeaponChange(key);
			break;
		case KeyDef.CATEGORY.BUNGEE_MODE:
			SetNoneSameKeyInCommon(key);
			SetNoneSameKeyInBungee(key);
			break;
		}
	}

	private bool CheckNoneKey()
	{
		foreach (KeyValuePair<string, KeyCode> item in inputKeyCommon)
		{
			if (item.Value == KeyCode.None)
			{
				string msg = string.Format(StringMgr.Instance.Get("KEY_NONE"), StringMgr.Instance.Get(item.Key));
				SystemMsgManager.Instance.ShowMessage(msg);
				return false;
			}
		}
		foreach (KeyValuePair<string, KeyCode> item2 in inputKeyShooter)
		{
			if (item2.Value == KeyCode.None)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("KEY_NONE"), StringMgr.Instance.Get(item2.Key));
				SystemMsgManager.Instance.ShowMessage(msg2);
				return false;
			}
		}
		foreach (KeyValuePair<string, KeyCode> item3 in inputKeyBuilder)
		{
			if (item3.Value == KeyCode.None)
			{
				string msg3 = string.Format(StringMgr.Instance.Get("KEY_NONE"), StringMgr.Instance.Get(item3.Key));
				SystemMsgManager.Instance.ShowMessage(msg3);
				return false;
			}
		}
		foreach (KeyValuePair<string, KeyCode> item4 in inputKeyWeaponChange)
		{
			if (item4.Value == KeyCode.None)
			{
				string msg4 = string.Format(StringMgr.Instance.Get("KEY_NONE"), StringMgr.Instance.Get(item4.Key));
				SystemMsgManager.Instance.ShowMessage(msg4);
				return false;
			}
		}
		foreach (KeyValuePair<string, KeyCode> item5 in inputKeyBungee)
		{
			if (item5.Value == KeyCode.None)
			{
				string msg5 = string.Format(StringMgr.Instance.Get("KEY_NONE"), StringMgr.Instance.Get(item5.Key));
				SystemMsgManager.Instance.ShowMessage(msg5);
				return false;
			}
		}
		return true;
	}

	private void CheckNewKey()
	{
		if (IsWaitingInput())
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (inputKeyCommon.ContainsKey(focusedKey))
				{
					SetNoneSameKey(KeyDef.CATEGORY.COMMON, Event.current.keyCode);
					inputKeyCommon[focusedKey] = Event.current.keyCode;
				}
				else if (inputKeyShooter.ContainsKey(focusedKey))
				{
					SetNoneSameKey(KeyDef.CATEGORY.SHOOTER_MODE, Event.current.keyCode);
					inputKeyShooter[focusedKey] = Event.current.keyCode;
				}
				else if (inputKeyBuilder.ContainsKey(focusedKey))
				{
					SetNoneSameKey(KeyDef.CATEGORY.BUILD_MODE, Event.current.keyCode);
					inputKeyBuilder[focusedKey] = Event.current.keyCode;
				}
				else if (inputKeyWeaponChange.ContainsKey(focusedKey))
				{
					SetNoneSameKey(KeyDef.CATEGORY.WEAPON_CHANGE, Event.current.keyCode);
					inputKeyWeaponChange[focusedKey] = Event.current.keyCode;
				}
				else if (inputKeyBungee.ContainsKey(focusedKey))
				{
					SetNoneSameKey(KeyDef.CATEGORY.BUNGEE_MODE, Event.current.keyCode);
					inputKeyBungee[focusedKey] = Event.current.keyCode;
				}
				focusedKey = string.Empty;
				newInputDelta = 0f;
			}
			int num = 323;
			int num2 = 0;
			while (IsWaitingInput() && num2 < 6)
			{
				if (Input.GetMouseButtonDown(num2))
				{
					num += num2;
					if (inputKeyCommon.ContainsKey(focusedKey))
					{
						SetNoneSameKey(KeyDef.CATEGORY.COMMON, (KeyCode)num);
						inputKeyCommon[focusedKey] = (KeyCode)num;
					}
					else if (inputKeyShooter.ContainsKey(focusedKey))
					{
						SetNoneSameKey(KeyDef.CATEGORY.SHOOTER_MODE, (KeyCode)num);
						inputKeyShooter[focusedKey] = (KeyCode)num;
					}
					else if (inputKeyBuilder.ContainsKey(focusedKey))
					{
						SetNoneSameKey(KeyDef.CATEGORY.BUILD_MODE, (KeyCode)num);
						inputKeyBuilder[focusedKey] = (KeyCode)num;
					}
					else if (inputKeyWeaponChange.ContainsKey(focusedKey))
					{
						SetNoneSameKey(KeyDef.CATEGORY.WEAPON_CHANGE, (KeyCode)num);
						inputKeyWeaponChange[focusedKey] = (KeyCode)num;
					}
					else if (inputKeyBungee.ContainsKey(focusedKey))
					{
						SetNoneSameKey(KeyDef.CATEGORY.BUNGEE_MODE, (KeyCode)num);
						inputKeyBungee[focusedKey] = (KeyCode)num;
					}
					focusedKey = string.Empty;
					newInputDelta = 0f;
				}
				num2++;
			}
		}
	}

	private void DoInput()
	{
		GUI.Box(crdKeyMapArea, string.Empty, "BoxBlue");
		float num = 2f;
		int num2 = inputKeyCommon.Count + inputKeyShooter.Count + inputKeyBuilder.Count + inputKeyWeaponChange.Count + inputKeyBungee.Count + 5;
		spKeyMap = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdKeyMapPosition.width - 25f, crdKeyName.y * (float)num2 + num * (float)(num2 - 1)), position: crdKeyMapPosition, scrollPosition: spKeyMap);
		Vector2 pos = new Vector2(crdKeyName.x, 0f);
		Vector2 vector = new Vector2(crdKeyName.x * 2.5f, 0f);
		LabelUtil.TextOut(new Vector2(0f, pos.y), StringMgr.Instance.Get("COMMON_KEY_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		vector.y += crdKeyValue.y + num;
		pos.y += crdKeyName.y + num;
		foreach (KeyValuePair<string, KeyCode> item in inputKeyCommon)
		{
			string text = StringMgr.Instance.Get(item.Key);
			LabelUtil.TextOut(pos, text, "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			string text2 = (!(focusedKey == item.Key)) ? custom_inputs.Instance.ChangeKeyString(item.Value.ToString()) : "...";
			if (GUI.Button(new Rect(vector.x, vector.y, crdKeyValue.x, crdKeyValue.y), text2, "BtnBlue") && newInputDelta > 0.3f)
			{
				focusedKey = item.Key;
			}
			vector.y += crdKeyValue.y + num;
			pos.y += crdKeyName.y + num;
		}
		LabelUtil.TextOut(new Vector2(0f, pos.y), StringMgr.Instance.Get("SHOOTER_KEY_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		vector.y += crdKeyValue.y + num;
		pos.y += crdKeyName.y + num;
		foreach (KeyValuePair<string, KeyCode> item2 in inputKeyShooter)
		{
			string text3 = StringMgr.Instance.Get(item2.Key);
			LabelUtil.TextOut(pos, text3, "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			string text4 = (!(focusedKey == item2.Key)) ? custom_inputs.Instance.ChangeKeyString(item2.Value.ToString()) : "...";
			if (GUI.Button(new Rect(vector.x, vector.y, crdKeyValue.x, crdKeyValue.y), text4, "BtnBlue") && newInputDelta > 0.3f)
			{
				focusedKey = item2.Key;
			}
			vector.y += crdKeyValue.y + num;
			pos.y += crdKeyName.y + num;
		}
		LabelUtil.TextOut(new Vector2(0f, pos.y), StringMgr.Instance.Get("WEAPON_CHANGE_KEY_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		vector.y += crdKeyValue.y + num;
		pos.y += crdKeyName.y + num;
		foreach (KeyValuePair<string, KeyCode> item3 in inputKeyWeaponChange)
		{
			string text5 = StringMgr.Instance.Get(item3.Key);
			LabelUtil.TextOut(pos, text5, "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			string text6 = (!(focusedKey == item3.Key)) ? custom_inputs.Instance.ChangeKeyString(item3.Value.ToString()) : "...";
			if (GUI.Button(new Rect(vector.x, vector.y, crdKeyValue.x, crdKeyValue.y), text6, "BtnBlue") && newInputDelta > 0.3f)
			{
				focusedKey = item3.Key;
			}
			vector.y += crdKeyValue.y + num;
			pos.y += crdKeyName.y + num;
		}
		LabelUtil.TextOut(new Vector2(0f, pos.y), StringMgr.Instance.Get("BUILDER_KEY_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		vector.y += crdKeyValue.y + num;
		pos.y += crdKeyName.y + num;
		foreach (KeyValuePair<string, KeyCode> item4 in inputKeyBuilder)
		{
			string text7 = StringMgr.Instance.Get(item4.Key);
			LabelUtil.TextOut(pos, text7, "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			string text8 = (!(focusedKey == item4.Key)) ? custom_inputs.Instance.ChangeKeyString(item4.Value.ToString()) : "...";
			if (GUI.Button(new Rect(vector.x, vector.y, crdKeyValue.x, crdKeyValue.y), text8, "BtnBlue") && newInputDelta > 0.3f)
			{
				focusedKey = item4.Key;
			}
			vector.y += crdKeyValue.y + num;
			pos.y += crdKeyName.y + num;
		}
		LabelUtil.TextOut(new Vector2(0f, pos.y), StringMgr.Instance.Get("BUNGEE_KEY_SET"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		vector.y += crdKeyValue.y + num;
		pos.y += crdKeyName.y + num;
		foreach (KeyValuePair<string, KeyCode> item5 in inputKeyBungee)
		{
			string text9 = StringMgr.Instance.Get(item5.Key);
			LabelUtil.TextOut(pos, text9, "Label", clrCommon, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			string text10 = (!(focusedKey == item5.Key)) ? custom_inputs.Instance.ChangeKeyString(item5.Value.ToString()) : "...";
			if (GUI.Button(new Rect(vector.x, vector.y, crdKeyValue.x, crdKeyValue.y), text10, "BtnBlue") && newInputDelta > 0.3f)
			{
				focusedKey = item5.Key;
			}
			vector.y += crdKeyValue.y + num;
			pos.y += crdKeyName.y + num;
		}
		GUI.EndScrollView();
		if (GlobalVars.Instance.MyButton(crdDefault, StringMgr.Instance.Get("DEFAULT_KEY"), "BtnAction"))
		{
			SetDefaultKeyMap();
		}
	}

	private void DoMouse()
	{
		GUI.Box(crdSensibilityRect, string.Empty, "BoxBlue");
		LabelUtil.TextOut(crdMouseSensibilityLabel, StringMgr.Instance.Get("MOUSE_SENSIBILITY"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		cameraSpeedFactor = GUI.HorizontalSlider(crdSensibilitySlider, cameraSpeedFactor, CameraController.minCamSpeed, CameraController.maxCamSpeed);
		int num = Mathf.RoundToInt(CameraController.CameraSpeedFactorToPercent(cameraSpeedFactor));
		LabelUtil.TextOut(crdSensibility, num.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdMouseAccessChange, StringMgr.Instance.Get("MOUSE_INVERSE_CATEGORY"), "Label", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		reverseMouse = GUI.Toggle(crdReverseMouse, reverseMouse, StringMgr.Instance.Get("REVERSE_MOUSE"));
		switchLRBuild = GUI.Toggle(crdLeftRightInBuild, switchLRBuild, StringMgr.Instance.Get("MOUSE_SWITCH_LEFT_RIGHT_IN_BUILD_MODE"));
		reverseMouseWheel = GUI.Toggle(crdReverseMouseWheel, reverseMouseWheel, StringMgr.Instance.Get("REVERSE_MOUSE_WHEEL"));
	}

	private void DoLocal()
	{
		DoVoice();
		DoLanguage();
		if (BuildOption.Instance.Props.useCountryFilterInSettingDlg)
		{
			DoCountryFilter();
		}
	}

	public override bool DoDialog()
	{
		RefreshTabNames();
		newInputDelta += Time.deltaTime;
		bool result = false;
		CheckNewKey();
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CHANGE_SETTING"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "BoxPopLine");
		GUI.enabled = !IsWaitingInput();
		tab = GUI.SelectionGrid(crdTab, tab, mainTab, mainTab.Length, "popTab");
		switch (tab)
		{
		case 0:
			DoGraphic();
			break;
		case 1:
			DoSound();
			break;
		case 2:
			DoInput();
			break;
		case 3:
			DoMouse();
			break;
		case 4:
			DoLocal();
			break;
		case 5:
			DoEtc();
			break;
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			SaveInput();
			GameObject gameObject = GameObject.Find("Main");
			PlayerPrefs.SetInt("HideOurForcesNickname", hideOurForcesNickname ? 1 : 0);
			PlayerPrefs.SetInt("HideEnemyForcesNickname", hideEnemyForcesNickname ? 1 : 0);
			PlayingCameraChange();
			PlayerPrefs.SetFloat("CameraSpeedFactor", cameraSpeedFactor);
			PlayerPrefs.SetInt("ReverseMouse", reverseMouse ? 1 : 0);
			PlayerPrefs.SetInt("SwitchLRBuild", switchLRBuild ? 1 : 0);
			PlayerPrefs.SetInt("ReverseMouseWheel", reverseMouseWheel ? 1 : 0);
			PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
			PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
			PlayerPrefs.SetInt("SfxMute", sfxMute ? 1 : 0);
			PlayerPrefs.SetInt("BgmMute", bgmMute ? 1 : 0);
			PlayerPrefs.SetInt("RadioSndMute", radioSndMute ? 1 : 0);
			PlayerPrefs.SetInt("UIScale", uiScale ? 1 : 0);
			GlobalVars.Instance.IsScaleUI = uiScale;
			GlobalVars.Instance.ReloadPlayerPrefs();
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChangeAudioSource");
			}
			GlobalVars.Instance.ChangeAudioSource();
			SaveInviteMessage();
			SaveWhisperMessage();
			if (0 <= curRes && curRes < supportedResolutions.Length && (Screen.fullScreen != fullScreen || currentResolution.width != supportedResolutions[curRes].width || currentResolution.height != supportedResolutions[curRes].height || QualitySettings.GetQualityLevel() != currentQualityLevel) && null != gameObject)
			{
				Resolution resolution = supportedResolutions[curRes];
				SettingParam settingParam = new SettingParam();
				settingParam.width = resolution.width;
				settingParam.height = resolution.height;
				settingParam.fullScreen = fullScreen;
				settingParam.qualityLevel = currentQualityLevel;
				gameObject.SendMessage("OnSettingChange", settingParam);
			}
			result = true;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.enabled = true;
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	private void InitGraphic()
	{
		fullScreen = Screen.fullScreen;
		currentResolution = default(Resolution);
		uiScale = ((PlayerPrefs.GetInt("UIScale", 0) != 0) ? true : false);
		int @int = PlayerPrefs.GetInt("BfScreenWidth", minScreenWidth);
		int int2 = PlayerPrefs.GetInt("BfScreenHeight", minScreenHeight);
		if (@int < minScreenWidth || int2 < minScreenHeight)
		{
			@int = minScreenWidth;
			int2 = minScreenHeight;
		}
		currentResolution.width = @int;
		currentResolution.height = int2;
		List<Resolution> list = new List<Resolution>();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			Resolution item = Screen.resolutions[i];
			if (item.width >= minScreenWidth && item.height >= minScreenHeight)
			{
				list.Add(item);
			}
		}
		supportedResolutions = list.ToArray();
		supportedResolutionDescs = new string[supportedResolutions.Length];
		for (int j = 0; j < supportedResolutionDescs.Length; j++)
		{
			supportedResolutionDescs[j] = supportedResolutions[j].width.ToString() + "x" + supportedResolutions[j].height.ToString();
			if (currentResolution.width == supportedResolutions[j].width && currentResolution.height == supportedResolutions[j].height)
			{
				curRes = j;
			}
		}
		currentQualityLevel = QualitySettings.GetQualityLevel();
	}

	private void InitSound()
	{
		int @int = PlayerPrefs.GetInt("SfxMute", 0);
		int int2 = PlayerPrefs.GetInt("BgmMute", 0);
		int int3 = PlayerPrefs.GetInt("RadioSndMute", 0);
		sfxMute = ((@int != 0) ? true : false);
		bgmMute = ((int2 != 0) ? true : false);
		radioSndMute = ((int3 != 0) ? true : false);
		if (sfxMute)
		{
			radioSndMute = true;
		}
		sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
		bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
	}

	private void CreateVoice()
	{
		if (cboxVoice == null)
		{
			cboxVoice = new ComboBox();
			cboxVoice.Initialize(bImage: false, new Vector2(crdCBoxVoice.width, crdCBoxVoice.height));
			cboxVoice.setBackground(Color.white, GlobalVars.Instance.txtMainColor);
			cboxVoice.SetParentWindowSize(size);
		}
		if (listVoice == null)
		{
			listVoice = new GUIContent[BuildOption.Instance.Props.LangVoices.Length];
			for (int i = 0; i < BuildOption.Instance.Props.LangVoices.Length; i++)
			{
				listVoice[i] = new GUIContent(StringMgr.Instance.Get(BuildOption.Instance.Props.GetLangVoiceName(i)));
			}
			selVoiceContent = new GUIContent();
			selVoiceContent.text = StringMgr.Instance.Get(BuildOption.Instance.Props.GetLangVoiceName(0));
			cboxVoice.SetSelectedItemIndex(0);
			int @int = PlayerPrefs.GetInt("BfVoice", -1);
			if (@int >= 0)
			{
				selVoiceContent = new GUIContent();
				selVoiceContent.text = StringMgr.Instance.Get(BuildOption.Instance.Props.GetLangVoiceName(@int));
				cboxVoice.SetSelectedItemIndex(@int);
			}
		}
	}

	private void CreateLang()
	{
		if (cboxLang == null)
		{
			cboxLang = new ComboBox();
			cboxLang.Initialize(bImage: false, new Vector2(crdCBoxLang.width, crdCBoxLang.height));
			cboxLang.setBackground(Color.white, GlobalVars.Instance.txtMainColor);
			cboxLang.SetParentWindowSize(size);
		}
		if (listLang == null)
		{
			int langOpt = LangOptManager.Instance.LangOpt;
			listLang = new GUIContent[BuildOption.Instance.Props.supportLanguages.Length];
			for (int i = 0; i < BuildOption.Instance.Props.supportLanguages.Length; i++)
			{
				LangOptManager.LANG_OPT lANG_OPT = BuildOption.Instance.Props.supportLanguages[i];
				listLang[i] = new GUIContent(StringMgr.Instance.Get(LangOptManager.Instance.GetLangName((int)lANG_OPT)));
				if (langOpt == (int)lANG_OPT)
				{
					selLangContent = new GUIContent(StringMgr.Instance.Get(LangOptManager.Instance.GetLangName((int)lANG_OPT)));
					cboxLang.SetSelectedItemIndex(i);
				}
			}
			if (selLangContent == null)
			{
				int defaultLanguage = (int)BuildOption.Instance.Props.DefaultLanguage;
				for (int j = 0; j < BuildOption.Instance.Props.supportLanguages.Length; j++)
				{
					LangOptManager.LANG_OPT lANG_OPT2 = BuildOption.Instance.Props.supportLanguages[j];
					if (defaultLanguage == (int)lANG_OPT2)
					{
						selLangContent = new GUIContent(StringMgr.Instance.Get(LangOptManager.Instance.GetLangName((int)lANG_OPT2)));
						cboxLang.SetSelectedItemIndex(j);
					}
				}
			}
		}
	}

	private void CreateCountryFilter()
	{
		if (cboxCF == null)
		{
			cboxCF = new ComboBox();
			cboxCF.Initialize(bImage: true, new Vector2(crdCBoxCF.width, crdCBoxCF.height));
			cboxCF.setBackground(Color.white, GlobalVars.Instance.txtMainColor);
			cboxCF.SetParentWindowSize(size);
		}
		if (listCF == null)
		{
			int countryFilter = MyInfoManager.Instance.CountryFilter;
			Property props = BuildOption.Instance.Props;
			listCF = new GUIContent[props.Filters.Length + 1];
			listCF[0] = new GUIContent(StringMgr.Instance.Get("ALL_SERVERS"), BuildOption.Instance.defaultCountryFilter);
			for (int i = 1; i < listCF.Length; i++)
			{
				listCF[i] = new GUIContent(StringMgr.Instance.Get(BuildOption.Instance.CountryNames[(int)props.Filters[i - 1]]), BuildOption.Instance.CountryIcons[(int)props.Filters[i - 1]]);
				if (props.Filters[i - 1] == (BuildOption.COUNTRY_FILTER)countryFilter)
				{
					selCFContent = new GUIContent(StringMgr.Instance.Get(BuildOption.Instance.CountryNames[(int)props.Filters[i - 1]]), BuildOption.Instance.CountryIcons[(int)props.Filters[i - 1]]);
					cboxCF.SetSelectedItemIndex(i);
				}
			}
			if (selCFContent == null)
			{
				selCFContent = new GUIContent(StringMgr.Instance.Get("ALL_SERVERS"), BuildOption.Instance.defaultCountryFilter);
				cboxCF.SetSelectedItemIndex(0);
			}
		}
	}

	private void SetDefaultKeyMap()
	{
		for (int i = 0; i < custom_inputs.Instance.DefaultDefinition.Length; i++)
		{
			switch (custom_inputs.Instance.DefaultDefinition[i].category)
			{
			case KeyDef.CATEGORY.COMMON:
				inputKeyCommon[custom_inputs.Instance.DefaultDefinition[i].name] = custom_inputs.Instance.DefaultDefinition[i].defaultInputKey;
				break;
			case KeyDef.CATEGORY.SHOOTER_MODE:
				inputKeyShooter[custom_inputs.Instance.DefaultDefinition[i].name] = custom_inputs.Instance.DefaultDefinition[i].defaultInputKey;
				break;
			case KeyDef.CATEGORY.BUILD_MODE:
				inputKeyBuilder[custom_inputs.Instance.DefaultDefinition[i].name] = custom_inputs.Instance.DefaultDefinition[i].defaultInputKey;
				break;
			case KeyDef.CATEGORY.WEAPON_CHANGE:
				inputKeyWeaponChange[custom_inputs.Instance.DefaultDefinition[i].name] = custom_inputs.Instance.DefaultDefinition[i].defaultInputKey;
				break;
			case KeyDef.CATEGORY.BUNGEE_MODE:
				inputKeyBungee[custom_inputs.Instance.DefaultDefinition[i].name] = custom_inputs.Instance.DefaultDefinition[i].defaultInputKey;
				break;
			}
		}
	}

	private void InitInput()
	{
		focusedKey = string.Empty;
		inputKeyCommon = new Dictionary<string, KeyCode>();
		inputKeyShooter = new Dictionary<string, KeyCode>();
		inputKeyBuilder = new Dictionary<string, KeyCode>();
		inputKeyWeaponChange = new Dictionary<string, KeyCode>();
		inputKeyBungee = new Dictionary<string, KeyCode>();
		for (int i = 0; i < custom_inputs.Instance.DefaultDefinition.Length; i++)
		{
			switch (custom_inputs.Instance.DefaultDefinition[i].category)
			{
			case KeyDef.CATEGORY.COMMON:
				inputKeyCommon.Add(custom_inputs.Instance.DefaultDefinition[i].name, custom_inputs.Instance.InputKey[i]);
				break;
			case KeyDef.CATEGORY.SHOOTER_MODE:
				inputKeyShooter.Add(custom_inputs.Instance.DefaultDefinition[i].name, custom_inputs.Instance.InputKey[i]);
				break;
			case KeyDef.CATEGORY.BUILD_MODE:
				inputKeyBuilder.Add(custom_inputs.Instance.DefaultDefinition[i].name, custom_inputs.Instance.InputKey[i]);
				break;
			case KeyDef.CATEGORY.WEAPON_CHANGE:
				inputKeyWeaponChange.Add(custom_inputs.Instance.DefaultDefinition[i].name, custom_inputs.Instance.InputKey[i]);
				break;
			case KeyDef.CATEGORY.BUNGEE_MODE:
				inputKeyBungee.Add(custom_inputs.Instance.DefaultDefinition[i].name, custom_inputs.Instance.InputKey[i]);
				break;
			}
		}
		int @int = PlayerPrefs.GetInt("ReverseMouse", 0);
		int int2 = PlayerPrefs.GetInt("SwitchLRBuild", 0);
		int int3 = PlayerPrefs.GetInt("ReverseMouseWheel", 0);
		cameraSpeedFactor = PlayerPrefs.GetFloat("CameraSpeedFactor", BuildOption.Instance.Props.defaultCameraSpeedFactor);
		reverseMouse = ((@int != 0) ? true : false);
		switchLRBuild = ((int2 != 0) ? true : false);
		reverseMouseWheel = ((int3 != 0) ? true : false);
		GlobalVars.Instance.ReloadPlayerPrefs();
	}

	private void SaveInput()
	{
		foreach (KeyValuePair<string, KeyCode> item in inputKeyCommon)
		{
			custom_inputs.Instance.InputKey[custom_inputs.Instance.KeyIndex(item.Key)] = item.Value;
		}
		foreach (KeyValuePair<string, KeyCode> item2 in inputKeyShooter)
		{
			custom_inputs.Instance.InputKey[custom_inputs.Instance.KeyIndex(item2.Key)] = item2.Value;
		}
		foreach (KeyValuePair<string, KeyCode> item3 in inputKeyBuilder)
		{
			custom_inputs.Instance.InputKey[custom_inputs.Instance.KeyIndex(item3.Key)] = item3.Value;
		}
		foreach (KeyValuePair<string, KeyCode> item4 in inputKeyWeaponChange)
		{
			custom_inputs.Instance.InputKey[custom_inputs.Instance.KeyIndex(item4.Key)] = item4.Value;
		}
		foreach (KeyValuePair<string, KeyCode> item5 in inputKeyBungee)
		{
			custom_inputs.Instance.InputKey[custom_inputs.Instance.KeyIndex(item5.Key)] = item5.Value;
		}
		custom_inputs.Instance.saveInputs();
	}

	private void InitEtc()
	{
		int @int = PlayerPrefs.GetInt("HideOurForcesNickname", 0);
		int int2 = PlayerPrefs.GetInt("HideEnemyForcesNickname", 0);
		hideOurForcesNickname = ((@int != 0) ? true : false);
		hideEnemyForcesNickname = ((int2 != 0) ? true : false);
		inviteSetting = INVITE_SETTING.ALL_ALLOW;
		if (MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK1_FRIEND))
		{
			inviteSetting = INVITE_SETTING.FRIEND_ONLY;
		}
		else if (MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK2_ALL_NO))
		{
			inviteSetting = INVITE_SETTING.ALL_NOT;
		}
		whisperSetting = WHISPER_SETTING.ALL_ALLOW;
		if (MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK1_FRIEND))
		{
			whisperSetting = WHISPER_SETTING.FRIEND_ONLY;
		}
		else if (MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK2_ALL_NO))
		{
			whisperSetting = WHISPER_SETTING.ALL_NOT;
		}
		CreateVoice();
		CreateLang();
		CreateCountryFilter();
	}

	private void SaveInviteMessage()
	{
		MyInfoManager.Instance.RemoveCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK1_FRIEND);
		MyInfoManager.Instance.RemoveCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK2_ALL_NO);
		if (inviteSetting == INVITE_SETTING.FRIEND_ONLY)
		{
			MyInfoManager.Instance.SetCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK1_FRIEND);
		}
		else if (inviteSetting == INVITE_SETTING.ALL_NOT)
		{
			MyInfoManager.Instance.SetCommonMask(MyInfoManager.COMMON_OPT.INVITE_MASK2_ALL_NO);
		}
	}

	private void SaveWhisperMessage()
	{
		MyInfoManager.Instance.RemoveCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK1_FRIEND);
		MyInfoManager.Instance.RemoveCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK2_ALL_NO);
		if (whisperSetting == WHISPER_SETTING.FRIEND_ONLY)
		{
			MyInfoManager.Instance.SetCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK1_FRIEND);
		}
		else if (whisperSetting == WHISPER_SETTING.ALL_NOT)
		{
			MyInfoManager.Instance.SetCommonMask(MyInfoManager.COMMON_OPT.WHISPER_MASK2_ALL_NO);
		}
		MyInfoManager.Instance.SaveCommonMaskServer();
	}

	public void InitDialog()
	{
		InitGraphic();
		InitSound();
		InitInput();
		InitEtc();
	}

	public bool SetTab(int _tab)
	{
		if (_tab >= mainTab.Length)
		{
			return false;
		}
		tab = _tab;
		return true;
	}

	private void PlayingCameraChange()
	{
		float @float = PlayerPrefs.GetFloat("CameraSpeedFactor", BuildOption.Instance.Props.defaultCameraSpeedFactor);
		if (@float != cameraSpeedFactor)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				CameraController component = gameObject.GetComponent<CameraController>();
				if (null != component)
				{
					component.ChangeCameraSpeedFactor(cameraSpeedFactor);
				}
			}
		}
	}
}
