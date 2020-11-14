using Procurios.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
	public class GUIButton
	{
		private static int highestDepthID;

		private static int targetID;

		public static bool Button(Rect bounds, string caption, GUIStyle btnStyle)
		{
			int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);
			int num = (1000 - GUI.depth) * 1000 + controlID;
			bool flag = false;
			if (Event.current.type == EventType.Layout)
			{
				if (bounds.Contains(Event.current.mousePosition) && num > highestDepthID)
				{
					highestDepthID = num;
					targetID = controlID;
				}
			}
			else if (Event.current.type == EventType.Repaint)
			{
				flag = (GUIUtility.hotControl == controlID);
				btnStyle.Draw(bounds, new GUIContent(caption), targetID == controlID, flag, on: false, hasKeyboardFocus: false);
				if (targetID == controlID)
				{
					highestDepthID = 0;
					targetID = 0;
				}
			}
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (targetID == controlID)
				{
					GUIUtility.hotControl = controlID;
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
				}
				return targetID == controlID;
			}
			return false;
		}
	}

	public enum E_FEVER
	{
		NONE = -1,
		FULL,
		ING
	}

	public class senseBombObj
	{
		public GameObject beamObj;

		public GameObject bombObj;

		public Vector3 expPos = Vector3.zero;

		public senseBombObj(Vector3 p, Vector3 n)
		{
			bombObj = (UnityEngine.Object.Instantiate((UnityEngine.Object)Instance.SenseBomb, p, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			bombObj.transform.up = n;
			expPos = p;
		}

		public void createBeam(bool isRed, Vector3 p, Vector3 n)
		{
			beamObj = (UnityEngine.Object.Instantiate((UnityEngine.Object)((!isRed) ? Instance.SenseBeam2 : Instance.SenseBeam), p, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			beamObj.transform.up = n;
			beamObj.transform.localScale = new Vector3(1f, 2f, 1f);
		}
	}

	private const float epsilon = 0.0001f;

	private const float orgScreenWidth = 1024f;

	private const float orgScreenHeight = 768f;

	private const float orgRatio = 1.33333337f;

	public const string YDCODE = "c17";

	public AudioClip playerHit;

	public GameObject drum_bomp_explosion;

	public GameObject drum_poison_explosion;

	public GameObject drum_poison_eff;

	public GameObject tramp_vert_eff;

	public GameObject tramp_hor_eff;

	public GameObject SenseBeam;

	public GameObject SenseBeam2;

	public GameObject SenseBomb;

	public GameObject installingEff;

	public GameObject CatridgeOrClipL;

	public Texture2D cboxDn;

	public Texture2D cboxUp;

	public Texture2D cboxDn2;

	public Texture2D cboxUp2;

	public Texture2D iconMapMode;

	public Texture2D iconTeamMode;

	public Texture2D iconBlastMode;

	public Texture2D iconsurvivalMode;

	public Texture2D iconCTFMode;

	public Texture2D iconDefenseMode;

	public Texture2D iconBndMode;

	public Texture2D iconBungeeMode;

	public Texture2D iconEscapeMode;

	public Texture2D iconZombieMode;

	public Texture2D texPingGreen;

	public Texture2D texPingGray;

	public Texture2D texPingRed;

	public Texture2D texPingYellow;

	public Texture2D iconQuickjoin;

	public Texture2D iconBlock;

	public Texture2D iconCancel;

	public Texture2D iconCart;

	public Texture2D iconDisk;

	public Texture2D iconEquip;

	public Texture2D iconGarbage;

	public Texture2D iconGift;

	public Texture2D iconJoin;

	public Texture2D iconReformat;

	public Texture2D iconRewrite;

	public Texture2D iconStart;

	public Texture2D iconMyItem;

	public Texture2D iconDetail;

	public AudioClip sndButtonClick;

	public AudioClip sndMouseOver;

	public AudioClip sndItemInstall;

	public AudioClip sndPortal;

	public Texture2D iconDownloaded;

	public Texture2D iconWarnYellow;

	public Texture2D iconWarnRed;

	public Texture2D iconblueRibbon;

	public Texture2D iconglory;

	public Texture2D icongoldRibbon;

	public Texture2D iconhotMap;

	public Texture2D iconMedal;

	public Texture2D iconNewmap;

	public Texture2D iconDeclare;

	public Texture2D iconredRibbon;

	public Texture2D iconSkull;

	public Texture2D iconNoAddiction;

	public Texture2D[] gaugeTiers;

	public Texture headLine;

	public Texture iconUpgrade;

	public Texture iconUpgradeMax;

	public Texture2D iconPCBang;

	public Texture2D iconThumbUp;

	public Texture2D iconThumbDn;

	public Texture2D iconSave;

	public Texture2D[] iconWeaponOpt;

	public Texture2D iconLockSlot;

	public Texture2D iconBoxGray;

	public Texture2D iconEnemyKill;

	public Texture2D iconEnemyKillBg;

	public GameObject muzzlefireUsk1;

	public GameObject muzzlefireUsk12;

	public GameObject explosionUsk;

	public GameObject fxPortalPPong;

	public GameObject selfExpolsion11;

	public string[] equipParentDirNames;

	public string[] ShopParentDirNames;

	public string[] ShopMainWpnCatNames;

	public string[] accessoryTabs;

	private AudioSource audioSource;

	public byte chkSum;

	public bool tutorFirstScriptOn = true;

	public bool bAreaCheck;

	public bool bNeighborDefense;

	public Vector3 neighborPoint = Vector3.zero;

	public LOBBY_TYPE LobbyType;

	public Color txtMainColor;

	public static Color txtEmptyColor = Color.clear;

	public int successUpgradePropID = -1;

	public bool OutoutPortalMessage;

	public bool applyNewP2P = true;

	public int packetAmount;

	public Texture2D weaponByZombie;

	public Texture2D weaponByCactus;

	public Texture2D weaponByFire;

	public Texture2D weaponByFlamDrum;

	public Texture2D weaponByToxicDrum;

	public Texture2D weaponByTrap;

	public Texture2D weaponByBlackhole;

	public Texture2D weaponByGoal;

	public Texture2D weaponByRankingUp;

	public Texture2D weaponBySelf;

	public int showBrickId = -1;

	public bool bCTFStateReceived;

	public bool bEraseItemOk;

	public bool bReceivedAck;

	public bool battleStarting;

	public float RemainTime;

	public float BoomRadius = 4f;

	public int BoomDamage = 100;

	public float RadiusPoisonBrick = 3f;

	public GameObject misslieMuzzleFireEff;

	public GameObject missileSmokeEff;

	public GameObject missileExplosionEff;

	public GameObject missileExplosionEff11;

	public GameObject missileObj;

	public GameObject rocketObj;

	public GameObject missileObj11;

	public GameObject rocketObj11;

	public float NativeGravity;

	public float NativeJumpHeightMin = 1f;

	public float NativeJumpHeightMax = 5f;

	public float NativeJumpHeight;

	public bool mute;

	public Transform hitParent;

	public int hitBirckman = -1;

	private static GlobalVars _instance = null;

	private int curWheelKey;

	private bool bWheelBlock;

	private float deltaTimeWheelBlock;

	public bool reverseMouseWheel;

	public bool switchLRBuild;

	public bool reverseMouse;

	public bool hideOurForcesNickname;

	public bool hideEnemyForcesNickname;

	public Vector3 vDefenseEnd = Vector3.zero;

	public WWW wwwUsk;

	public string DnVoiceFile = string.Empty;

	public bool bFlashBang;

	private int FlashbBangGrade;

	public AudioClip sndNoise;

	public float maxDistanceFlashbang = 14f;

	public float maintainFB;

	private float maxRadiusFB;

	public float pimpFBRadius;

	public float pimpFBContinueTime;

	public int GameMode;

	public bool GotoLobbyRoomList;

	public ArrayList arrTextAgb;

	public bool bOnceCalcHeight;

	private WWW wwwTOS;

	private string filenameTOS = string.Empty;

	public bool bRemember;

	public string strMyID = string.Empty;

	private bool escapeKey;

	private bool returnKey;

	private bool forceClosed;

	private bool IsMenuExOpenable;

	private float dtMenuEx;

	private BrickManDesc emdesc;

	private Camera cam;

	private int cm;

	private bool cmstart;

	private float ElapsedGod;

	private float ElapsedGodMax = 60f;

	private float ratioDiff;

	private Rect realUIScreenRect = new Rect(0f, 0f, 0f, 0f);

	private Rect uiScreenRect = new Rect(0f, 0f, 1024f, 768f);

	private Rect screenRect = new Rect(0f, 0f, 1024f, 768f);

	private bool isScaleUI;

	private Vector3 scale = Vector2.zero;

	private Matrix4x4 svMat = Matrix4x4.identity;

	public int totalComments;

	public bool IsIntroChange;

	public bool IsPriceChange;

	public bool IsIntroChangeTemp;

	public bool IsPriceChangeTemp;

	public string intro = string.Empty;

	public string introTemp = string.Empty;

	public int downloadPrice;

	public int downloadPriceTemp;

	public List<Snipets> snipets;

	private ushort[] battleModeList;

	public static string DELIMITER = "@#$%";

	public Texture2D uiBoom;

	public Texture2D uiBoomGaugeBg;

	public Texture2D uiBoomGaugeBar;

	public bool sys10First = true;

	public bool isLoadBattleTutor = true;

	public bool eventBridge = true;

	public bool eventGravity;

	public bool immediateKillBrickTutor;

	public string preWeaponCode = "aaa";

	public int opened;

	public bool blockDelBrick;

	public List<int> blockBricks;

	public GameObject fxFeverChar;

	public GameObject fxFeverGun;

	public GameObject fxFeverOn;

	public AudioClip sndFeverOn;

	public AudioClip sndFeverIng;

	public AudioClip sndFeverEnd;

	public int maxFeverCnt = 50;

	private int curFever;

	private float maxFeverTime = 10f;

	private float ElapsedFever;

	private int stateFever = -1;

	public string whisperNickTo = string.Empty;

	public string whisperNickFrom = string.Empty;

	public int netsendcnt;

	public int netsendsize;

	public int ex1Depth = 1;

	public int ex2Depth = 1;

	private string exceptionUserSeq = string.Empty;

	private StringBuilder consoleMsg;

	private Dictionary<int, string> exceptions = new Dictionary<int, string>();

	public bool shutdownNow;

	public bool outputCanPutWeaponMsg;

	public int droppedAmmo;

	public int droppedAmmo2;

	public string droppedItemCode = string.Empty;

	public int droppedItemSeq;

	private Dictionary<int, DroppedItem> dicDropedWeapon;

	public GameObject droppedEff;

	public bool cheatBlock;

	public bool beamTestState;

	public GameObject sbeamObj;

	public GameObject sbombObj;

	public Ray BeamRay;

	public Vector3 vBomb;

	public Vector3 vBombNormal;

	public Dictionary<int, senseBombObj> dicSenseBombObj;

	public bool IsRecognition;

	public int recogniVal;

	public int recogniteType = -1;

	public bool callRoomList;

	public int clanTeamMatchSuccess = -1;

	public int clanMatchMaxPlayer = -1;

	public int clanSendSqudREQ = -1;

	public int clanSendJoinREQ = -1;

	public int roomNo = -1;

	public int clanCreatePoint = -1;

	public int wannaPlayMap = -1;

	public int wannaPlayMode = -1;

	public static GlobalVars Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(GlobalVars)) as GlobalVars);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the GlobalVars Instance");
				}
			}
			return _instance;
		}
	}

	public bool EscapeKey
	{
		get
		{
			return escapeKey;
		}
		set
		{
			escapeKey = value;
		}
	}

	public Rect UIScreenRect => uiScreenRect;

	public Rect ScreenRect
	{
		get
		{
			if (IsScaleUI)
			{
				screenRect.width = (float)Screen.width / scale.x;
				screenRect.height = (float)Screen.height / scale.y;
			}
			else
			{
				screenRect.width = (float)Screen.width;
				screenRect.height = (float)Screen.height;
			}
			return screenRect;
		}
	}

	private bool IsInGame
	{
		get
		{
			for (int i = 0; i < Room.sceneByMode.Length; i++)
			{
				if (Room.sceneByMode[i] == Application.loadedLevelName)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsScaleUI
	{
		get
		{
			if (IsInGame)
			{
				return false;
			}
			if ((float)Screen.width < 1024f || (float)Screen.height < 768f)
			{
				return true;
			}
			return isScaleUI;
		}
		set
		{
			isScaleUI = value;
		}
	}

	public int StateFever
	{
		get
		{
			return stateFever;
		}
		set
		{
			stateFever = value;
		}
	}

	public Texture2D GetWeaponByExcetion(int wpnBy)
	{
		switch (wpnBy)
		{
		case -2:
			return weaponByFire;
		case -3:
			return weaponByFlamDrum;
		case -4:
			return weaponByToxicDrum;
		case -5:
			return weaponByTrap;
		case -6:
			return weaponByBlackhole;
		case -7:
			return weaponByCactus;
		case -8:
			return weaponByGoal;
		case -9:
			return weaponByRankingUp;
		case -10:
			return weaponBySelf;
		case -11:
			return weaponByZombie;
		default:
			Debug.LogError("weaponBy exception error: " + wpnBy);
			return null;
		}
	}

	public GameObject CurMissileObj()
	{
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12 && missileObj11 != null)
		{
			return missileObj11;
		}
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Age < 12 && missileObj11 != null)
		{
			return missileObj11;
		}
		return missileObj;
	}

	public GameObject CurRocketObj()
	{
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12 && missileObj11 != null)
		{
			return rocketObj11;
		}
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Age < 12 && missileObj11 != null)
		{
			return rocketObj11;
		}
		return rocketObj;
	}

	public GameObject CurMissileExplosionEff()
	{
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12 && missileObj11 != null)
		{
			return missileExplosionEff11;
		}
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Age < 12 && missileObj11 != null)
		{
			return missileExplosionEff11;
		}
		return missileExplosionEff;
	}

	public AudioSource GetAudioSource()
	{
		return audioSource;
	}

	public void ApplyAudioSource()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		ChangeAudioSource();
	}

	public void PlayOneShot(AudioClip _clip)
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.PlayOneShot(_clip);
		}
	}

	public void PlaySound(AudioClip _clip)
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.clip = _clip;
			audioSource.Play();
		}
	}

	public void StopSound()
	{
		audioSource.Stop();
	}

	public void ChangeAudioSource()
	{
		mute = ((PlayerPrefs.GetInt("SfxMute", 0) != 0) ? true : false);
		if (null != audioSource)
		{
			audioSource.volume = PlayerPrefs.GetFloat("SfxVolume", 1f);
		}
		GameObject gameObject = GameObject.Find("Me");
		if (gameObject != null)
		{
			gameObject.BroadcastMessage("OnChangeAudioSource");
		}
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		foreach (BrickManDesc brickManDesc in array)
		{
			if (brickManDesc.Seq != MyInfoManager.Instance.Seq)
			{
				GameObject gameObject2 = BrickManManager.Instance.Get(brickManDesc.Seq);
				if (gameObject2 != null)
				{
					gameObject2.BroadcastMessage("OnChangeAudioSource");
				}
			}
		}
		BrickInst[] array2 = BrickManager.Instance.ToBrickInstArray();
		if (array2 != null)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				GameObject brickObject = BrickManager.Instance.GetBrickObject(array2[j].Seq);
				if (null != brickObject)
				{
					AudioSource componentInChildren = brickObject.GetComponentInChildren<AudioSource>();
					if (componentInChildren != null)
					{
						brickObject.BroadcastMessage("OnChangeAudioSource");
					}
				}
			}
		}
	}

	public void PlaySoundButtonClick()
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.PlayOneShot(sndButtonClick);
		}
	}

	public void PlaySoundMouseOver()
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.PlayOneShot(sndMouseOver);
		}
	}

	public void PlaySoundItemInstall()
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.PlayOneShot(sndItemInstall);
		}
	}

	public void PlaySound(ref AudioClip clip)
	{
		if (!(audioSource == null) && !mute)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	private bool goodButton(Rect bounds, string caption, GUIStyle btnStyle)
	{
		int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);
		bool flag = bounds.Contains(Event.current.mousePosition);
		bool flag2 = GUIUtility.hotControl == controlID;
		if (GUIUtility.hotControl != 0 && !flag2)
		{
			flag = false;
		}
		if (Event.current.type == EventType.Repaint)
		{
			btnStyle.Draw(bounds, new GUIContent(caption), flag, flag2, on: false, hasKeyboardFocus: false);
		}
		switch (Event.current.GetTypeForControl(controlID))
		{
		case EventType.MouseDown:
			if (flag)
			{
				GUIUtility.hotControl = controlID;
			}
			break;
		case EventType.MouseUp:
			if (GUIUtility.hotControl == controlID)
			{
				GUIUtility.hotControl = 0;
			}
			if (flag && bounds.Contains(Event.current.mousePosition))
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool MyButton(Rect rc, Texture tex, GUIContent content, GUIStyle style)
	{
		Color color = GUI.color;
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Box(rc, content);
		GUI.color = color;
		if (GUI.Button(rc, tex, style))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public bool MyButton(Rect rc, string text)
	{
		if (GUI.Button(rc, text))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public bool MyButton(Rect rc, GUIContent content, GUIStyle style)
	{
		Color color = GUI.color;
		GUI.color = txtEmptyColor;
		GUI.Box(rc, content);
		GUI.color = color;
		if (GUI.Button(rc, string.Empty, style))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public bool MyButton2(Rect rc, GUIContent content, GUIStyle style)
	{
		GUI.Box(rc, content);
		if (GUI.Button(rc, string.Empty, style))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public bool MyButton3(Rect rc, GUIContent content, GUIStyle style)
	{
		GUIStyle style2 = GUI.skin.GetStyle("BtnAction");
		style2.fontStyle = FontStyle.Bold;
		if (GUI.Button(rc, content, style))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			style2.fontStyle = FontStyle.Normal;
			return true;
		}
		style2.fontStyle = FontStyle.Normal;
		return false;
	}

	public bool MyButtonBold(Rect rc, string text, GUIStyle style)
	{
		GUIStyle style2 = GUI.skin.GetStyle("BtnAction");
		style2.fontStyle = FontStyle.Bold;
		if (GUI.Button(rc, text, style))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			style2.fontStyle = FontStyle.Normal;
			return true;
		}
		style2.fontStyle = FontStyle.Normal;
		return false;
	}

	public bool MyButton(Rect rc, string text, GUIStyle guiStyle)
	{
		if (GUI.Button(rc, text, guiStyle))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public bool MyButton(Rect rc, Texture tex, GUIStyle guiStyle)
	{
		if (GUI.Button(rc, tex, guiStyle))
		{
			if (audioSource != null && !mute)
			{
				audioSource.PlayOneShot(sndButtonClick);
			}
			return true;
		}
		return false;
	}

	public byte Float32toFloat8(float fValue)
	{
		return (byte)((fValue + 32f) / 0.5019608f);
	}

	public float Float8toFloat32(byte fFloat8)
	{
		return (float)(int)fFloat8 * 0.5019608f - 32f;
	}

	public ushort Float32toFloat16(float fValue)
	{
		return (ushort)((fValue + 1024f) / 0.06250095f);
	}

	public float Float16toFloat32(ushort fFloat16)
	{
		return (float)(int)fFloat16 * 0.06250095f - 1024f;
	}

	public uint NormalToUByte4(float x, float y, float z)
	{
		uint num = 1u;
		uint num2 = 1u;
		uint num3 = 1u;
		if (x < 0f)
		{
			num = 0u;
			x = 0f - x;
		}
		if (y < 0f)
		{
			num2 = 0u;
			y = 0f - y;
		}
		if (z < 0f)
		{
			num3 = 0u;
			z = 0f - z;
		}
		return ((uint)(x * 255f) << 24) | ((uint)(y * 255f) << 16) | ((uint)(z * 255f) << 8) | (num << 2) | (num2 << 1) | num3;
	}

	public Vector3 UByte4ToNormal(uint ubyte4)
	{
		float num = (float)(double)((ubyte4 >> 24) & 0xFF) / 255f;
		float num2 = (float)(double)((ubyte4 >> 16) & 0xFF) / 255f;
		float num3 = (float)(double)((ubyte4 >> 8) & 0xFF) / 255f;
		return new Vector3(((ubyte4 & 4) == 0) ? (0f - num) : num, ((ubyte4 & 2) == 0) ? (0f - num2) : num2, ((ubyte4 & 1) == 0) ? (0f - num3) : num3);
	}

	public Color GetByteColor2FloatColor(byte bR, byte bG, byte bB)
	{
		return new Color((float)(int)bR / 255f, (float)(int)bG / 255f, (float)(int)bB / 255f);
	}

	public void ResetWheelKey()
	{
		curWheelKey = 0;
	}

	public void SetWheelKey(int key)
	{
		curWheelKey = key;
	}

	public int GetWheelKey(float wheelChanged)
	{
		if (bWheelBlock)
		{
			deltaTimeWheelBlock += Time.deltaTime;
			if (deltaTimeWheelBlock < 0.2f)
			{
				return -1;
			}
			bWheelBlock = false;
		}
		int num = 4;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
		{
			num = 5;
		}
		else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			num = 10;
		}
		if (reverseMouseWheel)
		{
			wheelChanged *= -1f;
		}
		if (wheelChanged > 0f)
		{
			curWheelKey--;
			bWheelBlock = true;
			deltaTimeWheelBlock = 0f;
			if (curWheelKey < 0)
			{
				curWheelKey = num - 1;
			}
			return curWheelKey;
		}
		if (wheelChanged < 0f)
		{
			curWheelKey++;
			bWheelBlock = true;
			deltaTimeWheelBlock = 0f;
			if (curWheelKey > num - 1)
			{
				curWheelKey = 0;
			}
			return curWheelKey;
		}
		return -1;
	}

	public void ChangeVoiceByLang(int nLang)
	{
		int @int = PlayerPrefs.GetInt("BfVoiceSet", -1);
		if (@int < 0)
		{
			string langName = LangOptManager.Instance.GetLangName(nLang);
			int vocID = BuildOption.Instance.Props.GetVocID(langName);
			if (vocID >= 0)
			{
				string langVoc = BuildOption.Instance.Props.GetLangVoc(vocID);
				langVoc += ".unity3d";
				int langVoiceVer = BuildOption.Instance.Props.GetLangVoiceVer(vocID);
				Instance.ResetVoices(langVoc, langVoiceVer);
				PlayerPrefs.SetInt("BfVoice", vocID);
			}
		}
	}

	public void ResetVoices(string strVoice, int ver)
	{
		AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.VOICE, strVoice, ver);
		DnVoiceFile = strVoice;
	}

	private int SelectFlashbangGrade(Vector3 forward, Vector3 eyePos, Vector3 expPos)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Vector3 vector = expPos - eyePos;
		vector.Normalize();
		float num = Vector3.Distance(expPos, eyePos);
		if (num > maxRadiusFB)
		{
			return 0;
		}
		Ray ray = new Ray(eyePos, vector);
		if (Physics.Raycast(ray, out RaycastHit _, num, layerMask))
		{
			FlashbBangGrade = 0;
			return 0;
		}
		int num2 = 1;
		float num3 = Vector3.Dot(forward, vector);
		if (num3 < 0f)
		{
			num2 += 2;
		}
		if (num > maxRadiusFB * 0.5f)
		{
			num2++;
		}
		if (num2 > 3)
		{
			num2 = 3;
		}
		FlashbBangGrade = num2;
		if (num2 == 1)
		{
			PlaySound(sndNoise);
		}
		return num2;
	}

	public void SwitchFlashbang(bool bVis, Vector3 expPos, bool ignoreDistance = false)
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (gameObject != null)
		{
			float num = 0f;
			if (!ignoreDistance)
			{
				maxRadiusFB = (maxDistanceFlashbang += pimpFBRadius);
				num = maintainFB + pimpFBContinueTime;
				if (bVis && SelectFlashbangGrade(gameObject.transform.forward, gameObject.transform.position, expPos) == 0)
				{
					return;
				}
			}
			else
			{
				num = maintainFB + pimpFBContinueTime;
				FlashbBangGrade = 1;
			}
			((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled = bVis;
			if (bVis)
			{
				gameObject.GetComponent<ScreenBrightness>().Init(FlashbBangGrade, num);
			}
			((Behaviour)gameObject.GetComponent<BlurEffect>()).enabled = false;
		}
		else
		{
			Debug.LogError("not found. main camera");
		}
	}

	public void UpdateFlashbang()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (gameObject != null && ((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled && ((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled)
		{
			if (!gameObject.GetComponent<ScreenBrightness>().bBright && !((Behaviour)gameObject.GetComponent<BlurEffect>()).enabled)
			{
				((Behaviour)gameObject.GetComponent<BlurEffect>()).enabled = true;
				gameObject.GetComponent<BlurEffect>().Init(FlashbBangGrade);
			}
			if (gameObject.GetComponent<ScreenBrightness>().bEnd)
			{
				((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled = false;
				((Behaviour)gameObject.GetComponent<BlurEffect>()).enabled = false;
			}
		}
	}

	public void EnableScreenBright(bool enable)
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (!(gameObject == null))
		{
			if (enable)
			{
				if (!((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled)
				{
					((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled = true;
					gameObject.GetComponent<ScreenBrightness>().Init(1, 0f);
				}
			}
			else
			{
				((Behaviour)gameObject.GetComponent<ScreenBrightness>()).enabled = false;
			}
		}
	}

	public void EnableScreenBrightSmart(bool bEnable, float maintain = 1f, float startBright = 0f, float maxBrightness = 1f)
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (!(gameObject == null))
		{
			if (bEnable)
			{
				if (!((Behaviour)gameObject.GetComponent<ScreenBrightnessSmart>()).enabled)
				{
					((Behaviour)gameObject.GetComponent<ScreenBrightnessSmart>()).enabled = true;
					gameObject.GetComponent<ScreenBrightnessSmart>().Init(maintain, startBright, maxBrightness);
				}
			}
			else
			{
				((Behaviour)gameObject.GetComponent<ScreenBrightnessSmart>()).enabled = false;
			}
		}
	}

	public void AutoDisableScreenBrightSmart()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (!(gameObject == null) && !((UnityEngine.Object)gameObject.GetComponent<ScreenBrightnessSmart>() == null) && ((Behaviour)gameObject.GetComponent<ScreenBrightnessSmart>()).enabled && gameObject.GetComponent<ScreenBrightnessSmart>().bEnd)
		{
			((Behaviour)gameObject.GetComponent<ScreenBrightnessSmart>()).enabled = false;
		}
	}

	private void SaveAssetBundleTOS(byte[] bytes)
	{
		string path = Path.Combine(Application.dataPath, "Resources/TOS/");
		string path2 = Path.Combine(path, filenameTOS);
		FileStream fileStream = File.Open(path2, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(bytes, 0, bytes.Length);
		binaryWriter.Close();
		fileStream.Close();
	}

	private IEnumerator DownLoadTOS(string fileName)
	{
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/TOS/" + fileName;
		wwwTOS = new WWW(url);
		yield return (object)wwwTOS;
		SaveAssetBundleTOS(wwwTOS.bytes);
		using (MemoryStream stream = new MemoryStream(wwwTOS.bytes))
		{
			using (StreamReader reader = new StreamReader(stream))
			{
				arrTextAgb = null;
				arrTextAgb = new ArrayList();
				string sLine = string.Empty;
				while (sLine != null)
				{
					sLine = reader.ReadLine();
					if (sLine != null)
					{
						arrTextAgb.Add(sLine);
					}
				}
				reader.Close();
			}
		}
	}

	public void LoadAbg()
	{
		string path = Path.Combine(Application.dataPath, "Resources/TOS/");
		string text = LangOptManager.Instance.GetAgbCurrent();
		if (text.Contains("no"))
		{
			text = LangOptManager.Instance.GetAgbById(1);
		}
		filenameTOS = text;
		string str = Path.Combine(path, text);
		if (wwwTOS != null)
		{
			wwwTOS.Dispose();
			wwwTOS = null;
		}
		wwwTOS = new WWW("file://" + str);
		if (wwwTOS.bytes.Length > 0)
		{
			using (MemoryStream stream = new MemoryStream(wwwTOS.bytes))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					arrTextAgb = null;
					arrTextAgb = new ArrayList();
					string text2 = string.Empty;
					while (text2 != null)
					{
						text2 = streamReader.ReadLine();
						if (text2 != null)
						{
							arrTextAgb.Add(text2);
						}
					}
					streamReader.Close();
				}
			}
		}
		else
		{
			StartCoroutine(DownLoadTOS(text));
		}
		bOnceCalcHeight = false;
	}

	public int applyDurabilityDamage(int durability, int durabilityMax, int damage)
	{
		if (durability < 0 || durabilityMax < 0)
		{
			return damage;
		}
		float num = (float)durability / (float)durabilityMax;
		num *= 100f;
		if (num > 30f)
		{
			return damage;
		}
		if (num > 11f && num <= 30f)
		{
			return (int)((float)damage * 0.7f);
		}
		return (int)((float)damage * 0.3f);
	}

	public void BattleStarting()
	{
		battleStarting = true;
		RemainTime = 5f;
	}

	private void Awake()
	{
		string exceptionURL = BuildOption.Instance.Props.ExceptionURL;
		if (exceptionURL.Length > 0)
		{
			consoleMsg = new StringBuilder();
			Application.RegisterLogCallback(HandleLog);
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		ReloadPlayerPrefs();
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
			}
		}
		txtMainColor = Instance.GetByteColor2FloatColor(244, 151, 25);
		isScaleUI = ((PlayerPrefs.GetInt("UIScale", 0) != 0) ? true : false);
		dicDropedWeapon = new Dictionary<int, DroppedItem>();
		List<string> list = new List<string>();
		dicSenseBombObj = new Dictionary<int, senseBombObj>();
		for (int i = 0; i < equipParentDirNames.Length; i++)
		{
			list.Add(equipParentDirNames[i]);
		}
		if (BuildOption.Instance.Props.usePremiumItem)
		{
			list.Add("PREMIUM_INVEN_TAB");
		}
		if (BuildOption.Instance.Props.usePCBangItem)
		{
			list.Add("PCBANG_INVEN_TAB");
		}
		if (list.Count != equipParentDirNames.Length)
		{
			equipParentDirNames = list.ToArray();
		}
	}

	private void OnGUI()
	{
		if (BuildOption.Instance.IsDeveloper && consoleMsg != null && consoleMsg.Length > 0)
		{
			Rect position = new Rect(0f, 0f, 600f, 200f);
			GUI.TextArea(position, consoleMsg.ToString());
		}
	}

	private void OnDestroy()
	{
		Application.RegisterLogCallback(null);
		consoleMsg = null;
	}

	public void ReloadPlayerPrefs()
	{
		hideEnemyForcesNickname = ((PlayerPrefs.GetInt("HideEnemyForcesNickname", 0) != 0) ? true : false);
		hideOurForcesNickname = ((PlayerPrefs.GetInt("HideOurForcesNickname", 0) != 0) ? true : false);
		int @int = PlayerPrefs.GetInt("ReverseMouse", 0);
		int int2 = PlayerPrefs.GetInt("SwitchLRBuild", 0);
		int int3 = PlayerPrefs.GetInt("ReverseMouseWheel", 0);
		reverseMouse = ((@int != 0) ? true : false);
		switchLRBuild = ((int2 != 0) ? true : false);
		reverseMouseWheel = ((int3 != 0) ? true : false);
	}

	private void checkEscapePressed()
	{
		escapeKey = Input.GetKeyDown(KeyCode.Escape);
	}

	public bool IsEscapePressed()
	{
		if (escapeKey)
		{
			escapeKey = false;
			return true;
		}
		return false;
	}

	private void checkReturnPressed()
	{
		returnKey = (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
	}

	public bool IsReturnPressed()
	{
		if (returnKey)
		{
			returnKey = false;
			return true;
		}
		return false;
	}

	public void SetForceClosed(bool set)
	{
		forceClosed = set;
	}

	public void resetMenuEx()
	{
		dtMenuEx = 0f;
		IsMenuExOpenable = false;
	}

	public bool IsMenuExOpenOk()
	{
		if (DialogManager.Instance.IsModal)
		{
			return false;
		}
		if (forceClosed)
		{
			forceClosed = false;
			return false;
		}
		return IsMenuExOpenable;
	}

	private void checkMenuEx()
	{
		dtMenuEx += Time.deltaTime;
		if (dtMenuEx > 0.33f)
		{
			IsMenuExOpenable = true;
		}
	}

	public bool IsModalAll()
	{
		return DialogManager.Instance.IsModal || MessageBoxMgr.Instance.HasMsg();
	}

	public void initcm()
	{
		cm = 0;
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.CH, cm);
	}

	private bool IsVisible(Vector3 pos, int seq)
	{
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
			}
		}
		float num = Vector3.Distance(cam.transform.position, pos);
		if (num > 15f)
		{
			return false;
		}
		int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("BndWall"));
		Vector3 normalized = (pos - cam.transform.position).normalized;
		Ray ray = new Ray(cam.transform.position, normalized);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BoxMan"))
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(hitInfo.transform, includeInactive: false);
			if (allComponents.Length == 1 && allComponents[0].Desc.Seq == seq)
			{
				return true;
			}
		}
		return false;
	}

	public void streamming()
	{
		if (!hideEnemyForcesNickname && MyInfoManager.Instance.CheckControllable())
		{
			cm++;
			NoCheat.Instance.Sync(NoCheat.WATCH_DOG.CH, cm);
			if (!cmstart)
			{
				cmstart = true;
			}
			GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 position = array[i].transform.position;
				position.y += 2f;
				Vector3 position2 = array[i].transform.position;
				position2.y += 1f;
				PlayerProperty component = array[i].GetComponent<PlayerProperty>();
				if (null != component && component.IsHostile() && !component.Desc.IsHidePlayer)
				{
					bool flag = false;
					if (emdesc != null && emdesc.Seq == component.Desc.Seq)
					{
						flag = true;
					}
					if (!flag && IsVisible(position2, component.Desc.Seq))
					{
						flag = true;
					}
					if (flag)
					{
						Vector3 vector = cam.WorldToViewportPoint(position);
						if (vector.z > 0f && 0f < vector.x && vector.x < 1f && 0f < vector.y && vector.y < 1f)
						{
							Vector3 sp = cam.WorldToScreenPoint(position);
							LabelUtil.TextOut(sp, component.Desc.Nickname, "Label", Color.red, Color.black, TextAnchor.LowerCenter);
						}
					}
				}
			}
		}
	}

	private bool isE(int slot)
	{
		switch (RoomManager.Instance.CurrentRoomType)
		{
		case Room.ROOM_TYPE.MAP_EDITOR:
			return false;
		case Room.ROOM_TYPE.INDIVIDUAL:
		case Room.ROOM_TYPE.BUNGEE:
		case Room.ROOM_TYPE.ESCAPE:
			return true;
		case Room.ROOM_TYPE.TEAM_MATCH:
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
		case Room.ROOM_TYPE.EXPLOSION:
		case Room.ROOM_TYPE.BND:
			return (MyInfoManager.Instance.Slot < 8 && slot >= 8) || (MyInfoManager.Instance.Slot >= 8 && slot < 8);
		case Room.ROOM_TYPE.MISSION:
			return (MyInfoManager.Instance.Slot < 4 && slot >= 4) || (MyInfoManager.Instance.Slot >= 4 && slot < 4);
		default:
			return false;
		}
	}

	private void checkE()
	{
		if (!hideEnemyForcesNickname && MyInfoManager.Instance.CheckControllable())
		{
			emdesc = null;
			if (!(null == cam))
			{
				int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("BndWall"));
				Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BoxMan"))
				{
					PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(hitInfo.transform, includeInactive: false);
					if (allComponents.Length == 1 && isE(allComponents[0].Desc.Slot))
					{
						emdesc = allComponents[0].Desc;
					}
				}
			}
		}
	}

	private void UpdateGod()
	{
		if (MyInfoManager.Instance.GodMode)
		{
			ElapsedGod += Time.deltaTime;
			if (ElapsedGod > ElapsedGodMax)
			{
				ElapsedGod = 0f;
				HandleLog("GodMode", "GODMODE!!!!!{ " + MyInfoManager.Instance.Nickname + " }", LogType.Exception);
			}
		}
	}

	private void Update()
	{
		if (!MyInfoManager.Instance.IsSpectator && MyInfoManager.Instance.Seq == RoomManager.Instance.Master && battleStarting)
		{
			int num = (int)RemainTime;
			RemainTime -= Time.deltaTime;
			int num2 = (int)RemainTime;
			if (num != num2 && num2 >= 0)
			{
				CSNetManager.Instance.Sock.SendCS_START_REQ((int)Instance.RemainTime);
			}
			if (RemainTime < 0f)
			{
				RemainTime = 0f;
				battleStarting = false;
			}
		}
		if (MyInfoManager.Instance.isGuiOn && !hideEnemyForcesNickname && MyInfoManager.Instance.CheckControllable() && cmstart)
		{
			NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.CH, cm);
		}
		checkEscapePressed();
		checkReturnPressed();
		checkMenuEx();
		checkE();
		updateFeverTime();
		UpdateGod();
		UpdateDroppedWeapons();
		AutoDisableScreenBrightSmart();
	}

	public void FitRightNBottomRectInScreen(ref Rect rc)
	{
		Vector2 point = new Vector2(rc.x + rc.width, rc.y + rc.height);
		Vector2 vector = PixelToGUIScalePoint(point);
		if (vector.x > (float)Screen.width)
		{
			rc.x -= rc.width;
		}
		if (vector.y > (float)Screen.height)
		{
			rc.y -= rc.height;
		}
	}

	public Vector2 ToGUIPoint(Vector2 point)
	{
		if (!IsScaleUI)
		{
			return new Vector2(((float)Screen.width - uiScreenRect.width) / 2f + point.x, ((float)Screen.height - uiScreenRect.height) / 2f + point.y);
		}
		return new Vector2(point.x + realUIScreenRect.x, point.y + realUIScreenRect.y);
	}

	public Rect ToGUIRect(Rect rc)
	{
		if (!IsScaleUI)
		{
			return new Rect(((float)Screen.width - uiScreenRect.width) / 2f + rc.x, ((float)Screen.height - uiScreenRect.height) / 2f + rc.y, rc.width, rc.height);
		}
		return new Rect(rc.x + realUIScreenRect.x, rc.y + realUIScreenRect.y, rc.width, rc.height);
	}

	public Vector2 PixelToGUIPoint(Vector2 point)
	{
		if (IsScaleUI)
		{
			return new Vector2(point.x / scale.x, point.y / scale.y);
		}
		return point;
	}

	public Vector2 PixelToGUIScalePoint(Vector2 point)
	{
		if (IsScaleUI)
		{
			return new Vector2(point.x * scale.x, point.y * scale.y);
		}
		return point;
	}

	private void _BeginScale()
	{
		ratioDiff = (float)Screen.width / (float)Screen.height - 1.33333337f;
		if (Mathf.Abs(ratioDiff) <= 0.0001f)
		{
			scale.x = (float)Screen.width / 1024f;
			scale.y = (float)Screen.height / 768f;
			scale.z = 1f;
			realUIScreenRect.width = (float)Screen.width;
			realUIScreenRect.height = (float)Screen.height;
			realUIScreenRect.x = 0f;
			realUIScreenRect.y = 0f;
			svMat = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		}
		else
		{
			if (ratioDiff < 0f)
			{
				scale.x = (scale.y = (float)Screen.width / 1024f);
				scale.z = 1f;
				realUIScreenRect.width = (float)Screen.width;
				realUIScreenRect.height = 768f * scale.y;
				realUIScreenRect.x = 0f;
				realUIScreenRect.y = ((float)Screen.height - realUIScreenRect.height) / 2f;
			}
			else
			{
				scale.x = (scale.y = (float)Screen.height / 768f);
				scale.z = 1f;
				realUIScreenRect.width = 1024f * scale.x;
				realUIScreenRect.height = (float)Screen.height;
				realUIScreenRect.x = ((float)Screen.width - realUIScreenRect.width) / 2f;
				realUIScreenRect.y = 0f;
			}
			realUIScreenRect.x /= scale.x;
			realUIScreenRect.y /= scale.x;
			realUIScreenRect.width /= scale.x;
			realUIScreenRect.height /= scale.x;
			GUI.BeginGroup(realUIScreenRect);
			svMat = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		}
	}

	public void BeginGUIWithBox(string boxStyle)
	{
		if (IsScaleUI)
		{
			if (boxStyle.Length > 0)
			{
				GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty, boxStyle);
			}
			else
			{
				GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty);
			}
			_BeginScale();
		}
		else
		{
			if (boxStyle.Length > 0)
			{
				GUI.Box(ScreenRect, string.Empty, boxStyle);
			}
			else
			{
				GUI.Box(ScreenRect, string.Empty);
			}
			GUI.BeginGroup(new Rect(((float)Screen.width - UIScreenRect.width) / 2f, ((float)Screen.height - UIScreenRect.height) / 2f, UIScreenRect.width, UIScreenRect.height));
		}
	}

	public void BeginGUI(Texture2D bg)
	{
		if (IsScaleUI)
		{
			if (null != bg)
			{
				TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), bg, ScaleMode.StretchToFill);
			}
			_BeginScale();
		}
		else
		{
			if (null != bg)
			{
				TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), bg, ScaleMode.StretchToFill);
			}
			GUI.BeginGroup(new Rect(((float)Screen.width - UIScreenRect.width) / 2f, ((float)Screen.height - UIScreenRect.height) / 2f, UIScreenRect.width, UIScreenRect.height));
		}
	}

	public void EndGUI()
	{
		if (IsScaleUI)
		{
			GUI.matrix = svMat;
			if (Mathf.Abs(ratioDiff) > 0.0001f)
			{
				GUI.EndGroup();
			}
		}
		else
		{
			GUI.EndGroup();
		}
	}

	public void ClearComments()
	{
		snipets.Clear();
	}

	public void AddComment(int cmtseq, string nick, string cmt, byte likeOrDislike)
	{
		Snipets snipets = new Snipets();
		snipets.cmtSeq = cmtseq;
		snipets.nickNameCmt = nick;
		snipets.cmt = WordFilter.Instance.IgnoreFilter(cmt);
		snipets.likeOrDislike = likeOrDislike;
		this.snipets.Add(snipets);
	}

	public Snipets[] ToSnipetArray()
	{
		return snipets.ToArray();
	}

	public void allocBattleMode(int size)
	{
		battleModeList = new ushort[size];
	}

	public void setBattleMode(int id, ushort val)
	{
		battleModeList[id] = val;
	}

	public ushort getBattleMode(int mode)
	{
		return battleModeList[mode];
	}

	public static void cmsLog(object message)
	{
	}

	public string DelimiterProcess(string orgText)
	{
		string result = string.Empty;
		string[] separator = new string[1]
		{
			DELIMITER
		};
		string[] array = orgText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 1; i < array.Length; i++)
		{
			switch (array[i][0])
			{
			case 'n':
				array[i] = array[i].Remove(0, 1);
				list.Add(array[i]);
				break;
			case 'l':
			{
				array[i] = array[i].Remove(0, 1);
				string rank = XpManager.Instance.GetRank(Convert.ToInt32(array[i]));
				list.Add(rank);
				break;
			}
			case 'i':
			{
				array[i] = array[i].Remove(0, 1);
				TItem tItem = TItemManager.Instance.Get<TItem>(array[i]);
				if (tItem != null)
				{
					string text2 = StringMgr.Instance.Get(tItem.name);
					if (text2 != null)
					{
						list.Add(text2);
					}
					else
					{
						Debug.LogError("Item name not found: " + tItem.name);
					}
				}
				else
				{
					Debug.LogError("Item code error: " + array[i]);
				}
				break;
			}
			case 'k':
			{
				array[i] = array[i].Remove(0, 1);
				string text = StringMgr.Instance.Get(array[i]);
				if (text != null)
				{
					list.Add(text);
				}
				else
				{
					Debug.LogError("String error: " + array[i]);
				}
				break;
			}
			}
		}
		if (list.Count > 0)
		{
			string format = StringMgr.Instance.Get(array[0]);
			result = string.Format(format, list.ToArray());
		}
		else if (array.Length == 1)
		{
			result = string.Format(StringMgr.Instance.Get(array[0]));
		}
		else if (array.Length == 2)
		{
			result = string.Format(StringMgr.Instance.Get(array[0]), array[1]);
		}
		else if (array.Length == 3)
		{
			result = string.Format(StringMgr.Instance.Get(array[0]), array[1], array[2]);
		}
		return result;
	}

	public void resetFever(bool timeover)
	{
		stateFever = -1;
		ElapsedFever = 0f;
		if (timeover)
		{
			curFever = 0;
		}
	}

	public float getFeverPercent()
	{
		if (curFever <= 0)
		{
			return 0f;
		}
		float num = (float)curFever / (float)maxFeverCnt;
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	public void setCurFever(float percent)
	{
		curFever = (int)(percent * (float)maxFeverCnt);
	}

	public bool IsFeverMode()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			return true;
		}
		return false;
	}

	private void IsMaxFeverCount()
	{
		if (curFever == maxFeverCnt)
		{
			PlayOneShot(sndFeverOn);
			StateFever = 0;
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component)
				{
					component.SetUIFever(isSet: true);
				}
			}
		}
	}

	public void addFeverCounter()
	{
		if (stateFever < 0)
		{
			curFever++;
			IsMaxFeverCount();
		}
	}

	public void activeFeverMode()
	{
		if (stateFever == 1)
		{
			reactiveFeverMode();
		}
		else
		{
			curFever = maxFeverCnt;
			IsMaxFeverCount();
			ElapsedFever = 0f;
		}
	}

	public void reactiveFeverMode()
	{
		PlayOneShot(sndFeverOn);
		ElapsedFever = 0f;
		curFever = maxFeverCnt;
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null != component)
			{
				component.initElapsedFeverActing();
			}
		}
	}

	private void updateFeverTime()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE && stateFever > 0)
		{
			ElapsedFever += Time.deltaTime;
			if (ElapsedFever >= maxFeverTime)
			{
				PlayOneShot(sndFeverEnd);
				resetFever(timeover: true);
				P2PManager.Instance.SendPEER_STATE_FEVER(isOn: false);
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					LocalController component = gameObject.GetComponent<LocalController>();
					if (null != component)
					{
						component.SoundStop();
						component.SetUIFever(isSet: false);
						component.SetWpnFever(isSet: false);
					}
				}
			}
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType logType)
	{
		if (logType == LogType.Exception)
		{
			string exceptionURL = BuildOption.Instance.Props.ExceptionURL;
			if (exceptionURL.Length > 0)
			{
				string text = logString + stackTrace;
				if (BuildOption.Instance.IsDeveloper)
				{
					consoleMsg.Append("[Exception] ");
					consoleMsg.Append(text);
					consoleMsg.Append("\n");
				}
				int hashCode = text.GetHashCode();
				if (!exceptions.ContainsKey(hashCode))
				{
					exceptions.Add(hashCode, text);
					Hashtable hashtable = new Hashtable();
					hashtable["Exception"] = logString;
					hashtable["Stack"] = stackTrace;
					exceptionUserSeq = "1560";
					Hashtable hashtable2 = new Hashtable();
					hashtable2["I_GameCode"] = "bf";
					hashtable2["I_LogId"] = 0;
					hashtable2["I_LogDetailId"] = 1;
					hashtable2["I_ChannelUserId"] = exceptionUserSeq;
					hashtable2["I_PCSeq"] = exceptionUserSeq;
					hashtable2["I_LogDes"] = hashtable;
					hashtable2["I_ConnectIP"] = "0";
					string value = JSON.JsonEncode(hashtable2);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("destination", "netmarble.mobile.gameLog");
					dictionary.Add("gamecode", "bf");
					dictionary.Add("body", value);
					Hashtable hashtable3 = new Hashtable();
					hashtable3["Content-Type"] = "application/x-www-form-urlencoded";
					POST(exceptionURL, dictionary, hashtable3);
				}
			}
		}
	}

	public WWW POST(string url, Dictionary<string, string> post, Hashtable headers)
	{
		WWWForm wWWForm = new WWWForm();
		foreach (KeyValuePair<string, string> item in post)
		{
			wWWForm.AddField(item.Key, item.Value);
		}
		WWW wWW = new WWW(url, wWWForm.data, headers);
		StartCoroutine(WaitForRequest(wWW));
		return wWW;
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		yield return (object)www;
		if (www.error != null)
		{
		}
	}

	public GameObject VerifyMe()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			return gameObject;
		}
		return null;
	}

	public GameObject VerifyMainCamera()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (null != gameObject)
		{
			return gameObject;
		}
		return null;
	}

	public void SetYangDingVoice(bool bSet)
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			if (bSet)
			{
				MyInfoManager.Instance.IsYang = true;
			}
			else
			{
				MyInfoManager.Instance.IsYang = false;
			}
		}
		else
		{
			MyInfoManager.Instance.IsYang = false;
		}
	}

	public void AllPlayerStatusToWaiting()
	{
		MyInfoManager.Instance.Status = 0;
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].Status = 0;
			}
		}
	}

	public void DropedWeaponAllClear()
	{
		Instance.senseBombInit();
		outputCanPutWeaponMsg = false;
		foreach (KeyValuePair<int, DroppedItem> item in dicDropedWeapon)
		{
			UnityEngine.Object.Destroy(item.Value.eff);
			UnityEngine.Object.Destroy(item.Value.obj);
		}
		dicDropedWeapon.Clear();
	}

	public void DropWeapon(int dropWeaponType)
	{
		GameObject gameObject = GameObject.Find("Me");
		if (gameObject != null)
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (component != null)
			{
				component.currentHaveWeapon(dropWeaponType);
			}
		}
	}

	public void DropWeapon3P(int itemSeq, string itemCode, int bulletCount, int bulletCount2, float x, float y, float z)
	{
		if (!dicDropedWeapon.ContainsKey(itemSeq))
		{
			TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(itemCode);
			if (tWeapon != null)
			{
				dicDropedWeapon.Add(itemSeq, new DroppedItem(itemSeq, itemCode, bulletCount, bulletCount2, x, y, z));
			}
		}
	}

	public void DropWeaponDestroy(int itemSeq, bool me = false)
	{
		if (dicDropedWeapon.ContainsKey(itemSeq))
		{
			DroppedItem droppedItem = dicDropedWeapon[itemSeq];
			if (droppedItem != null)
			{
				UnityEngine.Object.DestroyImmediate(droppedItem.eff);
				UnityEngine.Object.DestroyImmediate(droppedItem.obj);
				dicDropedWeapon.Remove(itemSeq);
			}
		}
	}

	public void SwapWeapon1P(int itemSeq)
	{
		if (dicDropedWeapon.ContainsKey(itemSeq))
		{
			DroppedItem droppedItem = dicDropedWeapon[itemSeq];
			if (droppedItem != null)
			{
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
					if (null != component)
					{
						cheatBlock = true;
						component.SwapWeapon(itemSeq, droppedItem.itemCode, droppedItem.bulletCount, droppedItem.bulletCount2);
						DropWeaponDestroy(itemSeq, me: true);
					}
				}
			}
		}
	}

	public void SwapWeapon3P(int playerSeq, int itemSeq)
	{
		if (dicDropedWeapon.ContainsKey(itemSeq))
		{
			DroppedItem droppedItem = dicDropedWeapon[itemSeq];
			if (droppedItem != null)
			{
				GameObject gameObject = BrickManManager.Instance.Get(playerSeq);
				if (null != gameObject)
				{
					TPController component = gameObject.GetComponent<TPController>();
					LookCoordinator component2 = gameObject.GetComponent<LookCoordinator>();
					if (null != component2 && component != null)
					{
						TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(droppedItem.itemCode);
						if (tWeapon != null)
						{
							int weaponType = (int)tWeapon.GetWeaponType();
							component2.UnequipWeaponBySlot(weaponType);
							component2.Equip(droppedItem.itemCode);
							component.ToIdleOrRun();
							SetOullineColor(playerSeq);
							UnityEngine.Object.DestroyImmediate(droppedItem.eff);
							UnityEngine.Object.DestroyImmediate(droppedItem.obj);
							dicDropedWeapon.Remove(itemSeq);
						}
					}
				}
			}
		}
	}

	public void CheckDropedWpns()
	{
		if (dicDropedWeapon != null && dicDropedWeapon.Count != 0)
		{
			Vector3 zero = Vector3.zero;
			GameObject gameObject = GameObject.Find("Me");
			if (gameObject != null)
			{
				zero = gameObject.transform.position;
				foreach (KeyValuePair<int, DroppedItem> item in dicDropedWeapon)
				{
					float num = Vector3.Distance(item.Value.obj.transform.position, zero);
					if (num < 2f)
					{
						CSNetManager.Instance.Sock.SendCS_PICKUP_DROPPED_ITEM_REQ(item.Value.itemSeq);
						break;
					}
				}
			}
		}
	}

	private void UpdateDroppedWeapons()
	{
		outputCanPutWeaponMsg = false;
		if (RoomManager.Instance.DropItem && dicDropedWeapon.Count != 0)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (!(gameObject == null))
			{
				Vector3 position = gameObject.transform.position;
				foreach (KeyValuePair<int, DroppedItem> item in dicDropedWeapon)
				{
					item.Value.obj.transform.eulerAngles += Vector3.up * Time.deltaTime * 10f;
					float num = Vector3.Distance(position, item.Value.obj.transform.position);
					if (num <= 2f)
					{
						outputCanPutWeaponMsg = true;
					}
				}
			}
		}
	}

	public void SetOullineColor(int playerID)
	{
		GameObject gameObject = BrickManManager.Instance.Get(playerID);
		if (!(gameObject == null))
		{
			LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
			BrickManDesc desc = BrickManManager.Instance.GetDesc(playerID);
			if (null != component && desc != null)
			{
				if (BuildOption.Instance.IsNetmarble && desc.IsGM)
				{
					Color value = new Color(1f, 0.82f, 0f, 1f);
					SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
					{
						skinnedMeshRenderer.material.SetColor("_OutlineColor", value);
						if (skinnedMeshRenderer.materials != null && skinnedMeshRenderer.materials.Length > 0)
						{
							for (int j = 0; j < skinnedMeshRenderer.materials.Length; j++)
							{
								skinnedMeshRenderer.materials[j].SetColor("_OutlineColor", value);
							}
						}
					}
					MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in componentsInChildren2)
					{
						meshRenderer.material.SetColor("_OutlineColor", value);
						if (meshRenderer.materials != null && meshRenderer.materials.Length > 0)
						{
							for (int l = 0; l < meshRenderer.materials.Length; l++)
							{
								meshRenderer.materials[l].SetColor("_OutlineColor", value);
							}
						}
					}
				}
				else if (desc.IsHostile())
				{
					Color value2 = new Color(1f, 0f, 0f, 0.5f);
					SkinnedMeshRenderer[] componentsInChildren3 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren3)
					{
						skinnedMeshRenderer2.material.SetColor("_OutlineColor", value2);
						if (skinnedMeshRenderer2.materials != null && skinnedMeshRenderer2.materials.Length > 0)
						{
							for (int n = 0; n < skinnedMeshRenderer2.materials.Length; n++)
							{
								skinnedMeshRenderer2.materials[n].SetColor("_OutlineColor", value2);
							}
						}
					}
					MeshRenderer[] componentsInChildren4 = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer2 in componentsInChildren4)
					{
						meshRenderer2.material.SetColor("_OutlineColor", value2);
						if (meshRenderer2.materials != null && meshRenderer2.materials.Length > 0)
						{
							for (int num2 = 0; num2 < meshRenderer2.materials.Length; num2++)
							{
								meshRenderer2.materials[num2].SetColor("_OutlineColor", value2);
							}
						}
					}
				}
			}
		}
	}

	public void SetOullineColor(int playerID, GameObject outlineObj)
	{
		GameObject x = BrickManManager.Instance.Get(playerID);
		if (!(x == null))
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(playerID);
			if (desc != null)
			{
				if (BuildOption.Instance.IsNetmarble && desc.IsGM)
				{
					Color value = new Color(1f, 0.82f, 0f, 1f);
					SkinnedMeshRenderer[] componentsInChildren = outlineObj.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
					{
						skinnedMeshRenderer.material.SetColor("_OutlineColor", value);
						if (skinnedMeshRenderer.materials != null && skinnedMeshRenderer.materials.Length > 0)
						{
							for (int j = 0; j < skinnedMeshRenderer.materials.Length; j++)
							{
								skinnedMeshRenderer.materials[j].SetColor("_OutlineColor", value);
							}
						}
					}
					MeshRenderer[] componentsInChildren2 = outlineObj.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in componentsInChildren2)
					{
						meshRenderer.material.SetColor("_OutlineColor", value);
						if (meshRenderer.materials != null && meshRenderer.materials.Length > 0)
						{
							for (int l = 0; l < meshRenderer.materials.Length; l++)
							{
								meshRenderer.materials[l].SetColor("_OutlineColor", value);
							}
						}
					}
				}
				else if (desc.IsHostile())
				{
					Color value2 = new Color(1f, 0f, 0f, 0.5f);
					SkinnedMeshRenderer[] componentsInChildren3 = outlineObj.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren3)
					{
						skinnedMeshRenderer2.material.SetColor("_OutlineColor", value2);
						if (skinnedMeshRenderer2.materials != null && skinnedMeshRenderer2.materials.Length > 0)
						{
							for (int n = 0; n < skinnedMeshRenderer2.materials.Length; n++)
							{
								skinnedMeshRenderer2.materials[n].SetColor("_OutlineColor", value2);
							}
						}
					}
					MeshRenderer[] componentsInChildren4 = outlineObj.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer2 in componentsInChildren4)
					{
						meshRenderer2.material.SetColor("_OutlineColor", value2);
						if (meshRenderer2.materials != null && meshRenderer2.materials.Length > 0)
						{
							for (int num2 = 0; num2 < meshRenderer2.materials.Length; num2++)
							{
								meshRenderer2.materials[num2].SetColor("_OutlineColor", value2);
							}
						}
					}
				}
			}
		}
	}

	public float GetSpecialAmmoInc()
	{
		float num = MyInfoManager.Instance.SumFunctionFactor("special_ammo_inc");
		if (num < 1f)
		{
			return MyInfoManager.Instance.SumFunctionFactor("special_ammo_add");
		}
		return num;
	}

	public bool GetSpecialAmmoType()
	{
		float num = MyInfoManager.Instance.SumFunctionFactor("special_ammo_inc");
		if (num > 0f)
		{
			return false;
		}
		num = MyInfoManager.Instance.SumFunctionFactor("special_ammo_add");
		if (num > 0f)
		{
			return true;
		}
		return false;
	}

	public void senseBombInit()
	{
		beamTestState = false;
		sbeamObj = null;
		sbombObj = null;
		vBomb = Vector3.zero;
		vBombNormal = Vector3.zero;
	}

	public void AddSenseBombObj(int seq, Vector3 p, Vector3 n)
	{
		if (!dicSenseBombObj.ContainsKey(seq))
		{
			dicSenseBombObj.Add(seq, new senseBombObj(p, n));
		}
	}

	public void AddSenseBeamObj(int seq, bool isRed, Vector3 p, Vector3 n)
	{
		if (dicSenseBombObj.ContainsKey(seq))
		{
			senseBombObj senseBombObj = dicSenseBombObj[seq];
			senseBombObj.createBeam(isRed, p, n);
		}
	}

	public void DeleteSenseBombObj(int seq)
	{
		dicSenseBombObj.Remove(seq);
	}

	public void ClearAllSenseBomObj()
	{
		dicSenseBombObj.Clear();
	}

	public void ENTER_SQUADING_ACK()
	{
		if (ChannelManager.Instance.CurChannel.Mode == 4)
		{
			if (Instance.clanSendSqudREQ == 0)
			{
				CSNetManager.Instance.Sock.SendCS_CREATE_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, Instance.wannaPlayMap, Instance.wannaPlayMode, Instance.clanMatchMaxPlayer);
			}
			else if (Instance.clanSendSqudREQ == 1 || Instance.clanSendJoinREQ == 0)
			{
				Room room = RoomManager.Instance.GetRoom(Instance.roomNo);
				if (room != null)
				{
					CSNetManager.Instance.Sock.SendCS_JOIN_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, room.Squad, room.SquadCounter);
				}
			}
			else if (Instance.clanSendJoinREQ == 1)
			{
				CSNetManager.Instance.Sock.SendCS_JOIN_SQUAD_REQ(InviteManager.Instance.GetData().clanSeq, InviteManager.Instance.GetData().squadIndex, InviteManager.Instance.GetData().squadCounterIndex);
			}
		}
	}
}
