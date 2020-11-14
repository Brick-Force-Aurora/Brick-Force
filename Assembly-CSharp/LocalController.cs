using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

public class LocalController : MonoBehaviour
{
	public enum TRAIN_MOVE_DIR
	{
		NONE,
		NORTH,
		SOUTH,
		WEST,
		EAST
	}

	private enum EBEARTRAP
	{
		NONE = -1,
		OVER,
		WAIT
	}

	public enum CONTROL_CONTEXT
	{
		STOP,
		WALK,
		RUN,
		JUMP,
		DIE,
		CLIMB,
		CANNON_TRY,
		CANNON_CTL,
		HANG,
		SQUATTED,
		SQUATTED_WALK,
		TRAIN_TRY,
		TRAIN_CTL,
		TRAIN_RUN,
		NONE
	}

	private enum ERAIL
	{
		NONE = -1,
		RAIL,
		LINK,
		UP,
		DN
	}

	public class CDoor
	{
		public bool die;

		public int seq;

		public float elapsed;
	}

	private const float TIME_ESCAPE_ACTIVE_INTERVAL = 10f;

	private ObscuredFloat gravity = 9.8f;

	private ObscuredFloat jumpHeight = 1.3f;

	public float runSpeed = 3f;

	public float walkSpeed = 1f;

	public float climbSpeed = 2f;

	public float veryHigh = 4f;

	public float deadlyHigh = 10f;

	public float minJumpSpeed = 1f;

	public float maxJumpSpeed = 2.5f;

	public float speedMax = 3f;

	public float notifyCycle = 0.02f;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public float maxRigidity = 1f;

	public float invincibleTime = 5f;

	public Texture2D actionBg;

	public Texture2D actionGauge;

	public Texture2D hpBg;

	public ImageFont hpFont;

	public ImageFont armorFont;

	public ImageFont imgFont;

	public Texture2D ladder;

	public Texture2D hitIcon;

	public Texture2D gravityIcon;

	public Texture2D texDirection;

	public Texture2D texDirection2;

	public Texture2D texDirectionBack;

	public Texture2D texFeverGaugeBg;

	public Texture2D texFeverGauge;

	public Texture2D texFeverGaugeDeco;

	public Texture2D texFeverGaugeGlow;

	public Texture2D texFeverTextNml;

	public Texture2D[] texFeverText;

	public Texture2D[] texFeverQ;

	public UILabel voteText;

	public UILabel voteAgree;

	public UILabel voteTime;

	private float ElapsedFevertxt;

	private bool FeverTxtBack;

	private float ElapsedBlinkFever;

	private float trainTryingTimeMax = 3f;

	public float emptyHandSpeedFactor = 1.5f;

	public float emptyHandDrawSpeed = 1f;

	public bool canThrowWeapon = true;

	public bool viewUpgradeBox;

	public Texture2D luckyImage;

	public AudioClip disarming;

	public AudioClip zombieBang;

	public AudioClip doorOpen;

	public AudioClip doorClose;

	public AudioClip warning;

	private float ElapsedWarning;

	private AudioSource audioSource;

	private bool ladderPhase = true;

	private float ladderTime;

	private string statusMessage = string.Empty;

	private float statusDelta;

	private float statusMessageLimit = 5f;

	private float speedTime;

	private bool speedUpEffect = true;

	private ObscuredFloat stamina;

	public float staminaMax = 100f;

	private bool dashState;

	private float doublePressTime;

	public float doublePressMax = 0.5f;

	public Texture2D dashBg;

	public Texture2D dashGauge;

	private int autoHealCount;

	public int armorChanceMax = 10000;

	public int armorChance = 10000;

	public float damageSorbRatio = 0.7f;

	public bool checkResetFlag;

	public float dtResetFlag;

	private RadioMenu radioMenu;

	private bool onceHideFps;

	public SystemMessage systemMsg;

	private bool outputGuideOnce;

	public Texture2D uizTob;

	public Texture2D uibToz;

	private float dtRecognition;

	private float dtRecognitionMax = 3f;

	private bool overHighGrass;

	public Texture2D uiHighGrass;

	public BlickTexture blickHighGrass;

	private EBEARTRAP eBearTrap = EBEARTRAP.NONE;

	private float deltaBeartrap;

	private float maxOverBearTrap = 2f;

	private float maxWaitBearTrap = 1f;

	private int bearSeq = -1;

	public Texture2D uiBearTrap;

	public BlickTexture blickBearTrap;

	private bool bSpeeddown;

	private float vertSpeed;

	private Vector3 vertDir = Vector3.up;

	public bool fallenDamage = true;

	private float moveSpeed;

	private Vector3 moveDir = Vector3.forward;

	private CONTROL_CONTEXT prevControlContext = CONTROL_CONTEXT.NONE;

	private CONTROL_CONTEXT controlContext;

	private Dictionary<int, int> dicDamageLog;

	private Dictionary<int, int> dicInflictedDamage;

	private CollisionFlags collisionFlags;

	private float inAirTime;

	private Vector3 lastGroundedPosition = Vector3.zero;

	private bool isJumping;

	private Camera cam;

	private float resultSpeed;

	private float lastClimbTime;

	private bool weaponChange;

	private float weaponChangeTimer;

	public float weaponChangeTime = 14f;

	private Vector3 respawnPosition = Vector3.zero;

	public float weaponChangeRadius = 3f;

	public ZombieVirus zombieVirus = new ZombieVirus();

	public int trainID = -1;

	public int trainSeq = -1;

	private int trainIDclosest = -1;

	private int trainSeqclosest = -1;

	private Vector3 railPosCenter = Vector3.zero;

	private Quaternion railRotCenter = Quaternion.Euler(0f, 0f, 0f);

	private TRAIN_MOVE_DIR trnMDir;

	private float trainSpeed = 6f;

	private bool canTrainStart;

	private Vector3 trnFwd = Vector3.zero;

	private Vector3 curTrnPos = Vector3.zero;

	private int trnSeq = -1;

	private int trnNextSeq = -1;

	private bool Rotated;

	private bool RailOvered;

	private bool IsSlopeUp;

	private ERAIL eRail = ERAIL.NONE;

	private float invincibleTimer;

	private bool invincible;

	private float cannonTryingTime;

	private CannonController cannon;

	private Animation bipAnimation;

	private SecureInt hitPointSecure;

	private SecureInt armorSecure;

	private int maxArmor;

	private bool controllable = true;

	private SecureFloat deltaFromDeathSecure;

	private float rigidity;

	private float lastNotified = float.PositiveInfinity;

	private float lastNotifiedDir = float.PositiveInfinity;

	private Vector3 ntfdDir = Vector3.forward;

	private CONTROL_CONTEXT ntfdControlContext = CONTROL_CONTEXT.NONE;

	private float ntfdSpeed;

	private float luckyTime;

	private bool iAmLucky;

	private ExplosionMatch explosionMatch;

	private bool uninstalling;

	private float uninstallDelta;

	private bool openning;

	private float openningDelta;

	public float openningTime = 4f;

	public float uninstallTime = 7f;

	public float uninstallRange = 2f;

	private BndMatch bndMatch;

	private ZombieMatch zombieMatch;

	private Vector3 orgEyePosition = Vector3.zero;

	private float sitUpingDelta = 1f;

	private float sitUpTime = 0.1f;

	private float GravityValue;

	private float deltaGravity;

	private float deltaJumpHeight;

	private float changedHeight;

	public float portalRestartTime = 5f;

	public int trapDamage = 10;

	public AudioClip sndPong;

	public AudioClip sndLanding;

	private bool isBoost;

	private float dtBoost;

	private GameObject boostEff;

	public float distanceBoost = 1f;

	public float maxBoostTime = 2f;

	private bool protectBoost;

	private float dtProtectBoost;

	private GameObject trampolineEff;

	private bool isTrampoline;

	private float dtTrampoline;

	private float previousY;

	public float maxTrampolineheight = 5f;

	private bool isPosion;

	public float poisonRadius = 2.5f;

	public int poisonDamage = 10;

	private Vector3 posionSpot = Vector3.zero;

	private float respwanTimeDec;

	private float dashTimeInc;

	private float staminaMaxAdd;

	private float fallenDamageReduce;

	private SecureFloat positionX;

	private SecureFloat positionY;

	private SecureFloat positionZ;

	private CharacterController cc;

	public GameObject blackholeFX;

	public bool bungeeRespawn;

	private float ElapsedFever;

	private float ElapsedFeverActing;

	private bool isFever;

	private bool blinkFever;

	private bool actingFever;

	private float deltaTimeBlackholeHit;

	private bool activateBlackhole;

	private int activateBlackholeUser = -1;

	private bool isEscapeActive;

	private float timeEscapeActive;

	private BattleChat battleChat;

	private Dictionary<int, CDoor> dicDoor;

	private float dtPre;

	private TrainController trainCtrl;

	private int FORCE_PREV = 5;

	private float deltaKeyJump = float.PositiveInfinity;

	private bool isBackDir;

	private bool drawBackDir = true;

	private float ElapsedBlinkBack;

	private bool initIdle;

	private float deltaTime;

	private float deltaTimeInflictedDamage;

	private bool isDamaged;

	private float dtDamageBrick;

	private float DamageBrickTimeMax = 1f;

	private float savedJumpHeight;

	private float elapsedPortalWait;

	private bool outputFeverMsg;

	private float ElapsedFeverMsg;

	private float selfRespawnReuseTime;

	private bool dropWeaponSkip;

	public float NormalizedSpeedTime
	{
		get
		{
			if (speedTime > speedMax)
			{
				return 1f;
			}
			if (speedTime <= 0f || speedMax <= 0f)
			{
				return 0f;
			}
			return speedTime / speedMax;
		}
	}

	public bool SpeedUpEffect => speedUpEffect;

	public bool AutoHealPossible => autoHealCount % 2 == 0;

	public float VertSpeed => vertSpeed;

	public float MoveSpeed => moveSpeed;

	public Vector3 MoveDir => moveDir;

	public CONTROL_CONTEXT ControlContext => controlContext;

	public bool CanTrainStart
	{
		set
		{
			canTrainStart = value;
		}
	}

	public bool Invincible => invincible;

	public bool IsInfected => zombieVirus.Active;

	public float NormalizedInvincibleTimer
	{
		get
		{
			if (invincibleTimer > invincibleTime)
			{
				return 1f;
			}
			if (invincibleTimer <= 0f || invincibleTime <= 0f)
			{
				return 0f;
			}
			return invincibleTimer / invincibleTime;
		}
	}

	public Animation BipAnimation => bipAnimation;

	private int hitPoint
	{
		get
		{
			int num = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPointSecure.Get());
			if (num > GetMaxHp())
			{
				BuildOption.Instance.HardExit();
			}
			return hitPointSecure.Get();
		}
		set
		{
			hitPointSecure.Set(value);
		}
	}

	public int HitPoint => hitPoint;

	private int armor
	{
		get
		{
			if (armorSecure.Get() > maxArmor)
			{
				BuildOption.Instance.HardExit();
			}
			return armorSecure.Get();
		}
		set
		{
			armorSecure.Set(value);
		}
	}

	public int Armor => armor;

	public int MaxArmor => maxArmor;

	public bool IsDead => hitPoint <= 0;

	public bool Controllable => controllable;

	public float DeltaFromDeath
	{
		get
		{
			return deltaFromDeathSecure.Get();
		}
		set
		{
			deltaFromDeathSecure.Set(value);
		}
	}

	public Vector3 PosionSpot
	{
		set
		{
			posionSpot = value;
			isPosion = true;
			dtDamageBrick = 0f;
		}
	}

	public float RespwanTimeDec => respwanTimeDec;

	public Vector3 TranceformPosition
	{
		get
		{
			return new Vector3(positionX.Get(), positionY.Get(), positionZ.Get());
		}
		set
		{
			positionX.Set(value.x);
			positionY.Set(value.y);
			positionZ.Set(value.z);
			if (base.transform != null)
			{
				base.transform.position = value;
			}
		}
	}

	public bool ActingFever => actingFever;

	public bool ActivateBlackhole
	{
		get
		{
			return activateBlackhole;
		}
		set
		{
			activateBlackhole = value;
			deltaTimeBlackholeHit = 0f;
		}
	}

	public int ActivateBlackholeUser
	{
		get
		{
			return activateBlackholeUser;
		}
		set
		{
			activateBlackholeUser = value;
		}
	}

	public float SelfRespawnReuseTime
	{
		get
		{
			return selfRespawnReuseTime;
		}
		set
		{
			selfRespawnReuseTime = value;
		}
	}

	public void WantedOn()
	{
		if (!IsDead)
		{
			SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp()), autoHealPossible: false);
		}
	}

	public void Infect()
	{
		VerifyAudioSource();
		if (null != audioSource)
		{
			audioSource.clip = zombieBang;
			audioSource.loop = false;
			audioSource.Play();
		}
		zombieVirus.Active = true;
		SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp()), autoHealPossible: false);
	}

	public void Cure()
	{
		zombieVirus.Active = false;
		SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp()), autoHealPossible: false);
	}

	public void IDidSomething(bool defaultWpnChg = true)
	{
		if (invincible)
		{
			invincible = false;
			P2PManager.Instance.SendPEER_UNINVINCIBLE();
		}
		if (defaultWpnChg && weaponChange)
		{
			MakeWeaponChangeDisable();
		}
	}

	private void changeGravity(bool bInc)
	{
		if (bInc)
		{
			gravity = (float)gravity + deltaGravity;
			jumpHeight = (float)jumpHeight - deltaJumpHeight;
		}
		else
		{
			gravity = (float)gravity - deltaGravity;
			jumpHeight = (float)jumpHeight + deltaJumpHeight;
		}
		if ((float)gravity < 0.3f)
		{
			gravity = 0.3f;
		}
		if ((float)jumpHeight < 0.5f)
		{
			jumpHeight = 0.5f;
		}
	}

	public void SetUIFever(bool isSet)
	{
		isFever = isSet;
		if (isSet)
		{
			ElapsedFever = 0f;
			outputFeverMsg = true;
			ElapsedFeverMsg = 0f;
		}
		else
		{
			blinkFever = false;
		}
	}

	public void SetWpnFever(bool isSet)
	{
		actingFever = isSet;
		BrickComposer componentInChildren = GetComponentInChildren<BrickComposer>();
		if (null != componentInChildren)
		{
			componentInChildren.setFever(isSet);
		}
		if (isSet)
		{
			FeverFx componentInChildren2 = GetComponentInChildren<FeverFx>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.reset();
			}
		}
	}

	private void VerifyAudioSource()
	{
		if (null == audioSource)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void VerifyBattleChat()
	{
		if (battleChat == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				battleChat = gameObject.GetComponent<BattleChat>();
			}
		}
	}

	private void Awake()
	{
		hitPointSecure.Init(100);
		armorSecure.Init(0);
		deltaFromDeathSecure.Init(0f);
		positionX.Init(0f);
		positionY.Init(0f);
		positionZ.Init(0f);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.RUNSPEED, runSpeed);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.WALKSPEED, walkSpeed);
	}

	private void VerifyBndMatch()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && bndMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				bndMatch = gameObject.GetComponent<BndMatch>();
			}
		}
	}

	private void VerifyZombieMatch()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && zombieMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				zombieMatch = gameObject.GetComponent<ZombieMatch>();
			}
		}
	}

	private void VerifyExplosionMatch()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION && explosionMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				explosionMatch = gameObject.GetComponent<ExplosionMatch>();
			}
		}
	}

	private void LeaveUninstall()
	{
		if (uninstalling)
		{
			P2PManager.Instance.SendPEER_UNINSTALLING_BOMB(uninstalling: false);
			uninstalling = false;
			uninstallDelta = 0f;
			VerifyAudioSource();
			if (null != audioSource)
			{
				audioSource.Stop();
			}
		}
	}

	private void EnterUninstall()
	{
		IDidSomething();
		P2PManager.Instance.SendPEER_UNINSTALLING_BOMB(uninstalling: true);
		CancelSpeedUp();
		uninstalling = true;
		uninstallDelta = 0f;
		VerifyAudioSource();
		if (null != audioSource)
		{
			audioSource.clip = disarming;
			audioSource.loop = true;
			audioSource.Play();
		}
	}

	private bool ApplyInstallClockBomb()
	{
		BombFuction componentInChildren = GetComponentInChildren<BombFuction>();
		if (null == componentInChildren)
		{
			return false;
		}
		IDidSomething(defaultWpnChg: false);
		if (componentInChildren.IsInstalling)
		{
			controlContext = CONTROL_CONTEXT.STOP;
			moveSpeed = 0f;
		}
		return componentInChildren.IsInstalling;
	}

	public void ResetClockBomb()
	{
		uninstalling = false;
		uninstallDelta = 0f;
	}

	private bool ApplyUninstallClockBomb()
	{
		RaycastHit hit2;
		if (uninstalling)
		{
			uninstallDelta += Time.deltaTime;
			if (!CanUninstall() || !custom_inputs.Instance.GetButton("K_ACTION") || !GetUninstallTarget(out RaycastHit _))
			{
				LeaveUninstall();
			}
			else if (uninstallDelta >= uninstallTime)
			{
				explosionMatch.Step = ExplosionMatch.STEP.BLASTED;
				LeaveUninstall();
				CSNetManager.Instance.Sock.SendCS_BM_UNINSTALL_BOMB_REQ(explosionMatch.BlastTarget);
			}
		}
		else if (CanUninstall() && custom_inputs.Instance.GetButton("K_ACTION") && GetUninstallTarget(out hit2))
		{
			EnterUninstall();
		}
		if (uninstalling)
		{
			controlContext = CONTROL_CONTEXT.STOP;
			moveSpeed = 0f;
		}
		return uninstalling;
	}

	public void CommandOpenDoor(int brickSeq, sbyte status, bool soundOn)
	{
		if (status == 1)
		{
			if (soundOn)
			{
				GameObject brickObject = BrickManager.Instance.GetBrickObject(brickSeq);
				if (brickObject != null)
				{
					AudioSource componentInChildren = brickObject.GetComponentInChildren<AudioSource>();
					if (componentInChildren != null)
					{
						componentInChildren.PlayOneShot(doorOpen);
					}
				}
			}
			BrickManager.Instance.EnableColliderBox(brickSeq, enable: false);
			BrickManager.Instance.AnimationPlay(-1, brickSeq, "fire");
			CDoor cDoor = new CDoor();
			cDoor.die = false;
			cDoor.seq = brickSeq;
			cDoor.elapsed = 0f;
			if (!dicDoor.ContainsKey(brickSeq))
			{
				dicDoor.Add(brickSeq, cDoor);
			}
		}
		else
		{
			if (soundOn)
			{
				GameObject brickObject2 = BrickManager.Instance.GetBrickObject(brickSeq);
				if (brickObject2 != null)
				{
					AudioSource componentInChildren2 = brickObject2.GetComponentInChildren<AudioSource>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.PlayOneShot(doorClose);
					}
				}
			}
			BrickManager.Instance.AnimationCrossFade(brickSeq, "idle");
			BrickManager.Instance.EnableColliderBox(brickSeq, enable: true);
			dicDoor.Remove(brickSeq);
		}
	}

	private void TryOpenDoor()
	{
		if (!Application.loadedLevelName.Contains("MapEdit") && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase))
		{
			int targetDoor = GetTargetDoor();
			if (custom_inputs.Instance.GetButtonDown("K_ACTION") && targetDoor >= 0 && !dicDoor.ContainsKey(targetDoor))
			{
				CSNetManager.Instance.Sock.SendCS_OPEN_DOOR_REQ(targetDoor);
			}
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				foreach (KeyValuePair<int, CDoor> item in dicDoor)
				{
					if (!item.Value.die)
					{
						item.Value.elapsed += Time.deltaTime;
						if (item.Value.elapsed >= 5f)
						{
							item.Value.die = true;
							if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
							{
								CSNetManager.Instance.Sock.SendCS_CLOSE_DOOR_REQ(item.Value.seq);
							}
						}
					}
				}
			}
		}
	}

	private void ApplySitup()
	{
		if (controlContext != CONTROL_CONTEXT.CANNON_CTL && controlContext != CONTROL_CONTEXT.CANNON_TRY)
		{
			sitUpingDelta += Time.deltaTime;
			if (null != cam)
			{
				float value = sitUpingDelta / sitUpTime;
				value = Mathf.Clamp(value, 0f, 1f);
				CameraController component = cam.GetComponent<CameraController>();
				if (controlContext == CONTROL_CONTEXT.SQUATTED || controlContext == CONTROL_CONTEXT.SQUATTED_WALK)
				{
					cam.transform.localPosition = Vector3.Lerp(orgEyePosition, component.EyePositionOnSquatted, value);
				}
				else
				{
					cam.transform.localPosition = Vector3.Lerp(orgEyePosition, component.EyePosition, value);
				}
			}
		}
	}

	private void OnCheatingDetecting()
	{
		Debug.Log("float type cheating Detected.");
		BuildOption.Instance.HardExit();
	}

	private void Start()
	{
		ObscuredFloat.onCheatingDetected = OnCheatingDetecting;
		dicDoor = new Dictionary<int, CDoor>();
		deltaKeyJump = float.PositiveInfinity;
		onceHideFps = false;
		armor = (maxArmor = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.ARMOR, MyInfoManager.Instance.SumArmor()));
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.ARMOR, armor);
		autoHealCount = 0;
		speedTime = speedMax;
		uninstalling = false;
		uninstallDelta = 0f;
		rigidity = 0f;
		sitUpingDelta = 1f;
		SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp()), autoHealPossible: false);
		TranceformPosition = new Vector3(Random.Range(-1000f, 1000f), 500f, Random.Range(-1000f, 1000f));
		InitializeAnimation();
		controlContext = CONTROL_CONTEXT.STOP;
		dicDamageLog = new Dictionary<int, int>();
		dicInflictedDamage = new Dictionary<int, int>();
		GlobalVars.Instance.ResetWheelKey();
		radioMenu = null;
		GameObject gameObject = GameObject.Find("Main");
		if (gameObject != null)
		{
			radioMenu = gameObject.GetComponent<RadioMenu>();
		}
		doublePressTime = doublePressMax;
		stamina = staminaMax;
		hpBg = VersionTextureManager.Instance.buildTexture.lifeGaugeBg;
		systemMsg.Start();
		Transform transform = base.transform.FindChild("FX_Falling");
		if (null != transform && !bungeeRespawn && transform.gameObject.activeInHierarchy)
		{
			transform.gameObject.SetActive(value: false);
		}
	}

	private bool CanUninstall()
	{
		return CanControl() && !MyInfoManager.Instance.AmIBlasting() && (bool)explosionMatch && explosionMatch.CanUninstall;
	}

	private bool GetUninstallTarget(out RaycastHit hit)
	{
		if (cam == null)
		{
			VerifyCamera();
		}
		int layerMask = (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Chunk"));
		Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (Physics.Raycast(ray, out hit, uninstallRange, layerMask))
		{
			GameObject gameObject = hit.transform.gameObject;
			if (gameObject.layer == LayerMask.NameToLayer("Brick") || gameObject.layer == LayerMask.NameToLayer("Chunk"))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private int GetTargetDoor()
	{
		if (null == cc)
		{
			return -1;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Edit Layer");
		float num = cc.height / 2f;
		Vector3 tranceformPosition = TranceformPosition;
		float x = tranceformPosition.x;
		Vector3 tranceformPosition2 = TranceformPosition;
		float y = tranceformPosition2.y + num;
		Vector3 tranceformPosition3 = TranceformPosition;
		Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
			if (null != componentInChildren && componentInChildren.Index == 165)
			{
				return componentInChildren.Seq;
			}
		}
		return -1;
	}

	private void OnHeartbeat()
	{
		hpFont.Scale = 2f;
	}

	public void LogAttacker(int shooter, int damage)
	{
		if (dicDamageLog != null && BrickManManager.Instance.GetDesc(shooter) != null && damage > 0)
		{
			if (dicDamageLog.ContainsKey(shooter))
			{
				Dictionary<int, int> dictionary;
				Dictionary<int, int> dictionary2 = dictionary = dicDamageLog;
				int key;
				int key2 = key = shooter;
				key = dictionary[key];
				dictionary2[key2] = key + damage;
			}
			else
			{
				dicDamageLog.Add(shooter, damage);
			}
			if (dicInflictedDamage != null)
			{
				if (dicInflictedDamage.ContainsKey(shooter))
				{
					Dictionary<int, int> dictionary3;
					Dictionary<int, int> dictionary4 = dictionary3 = dicInflictedDamage;
					int key;
					int key3 = key = shooter;
					key = dictionary3[key];
					dictionary4[key3] = key + damage;
				}
				else
				{
					dicInflictedDamage.Add(shooter, damage);
				}
			}
		}
	}

	public void SetLucky(bool lucky)
	{
		if (lucky)
		{
			iAmLucky = true;
			luckyTime = 0f;
		}
	}

	private void SetHitPoint(int hp, bool autoHealPossible)
	{
		hitPoint = hp;
		if (hitPoint == 0 && autoHealPossible)
		{
			GameObject gameObject = GameObject.Find("Main");
			ShooterTools component = gameObject.GetComponent<ShooterTools>();
			if (null != component)
			{
				int num = component.AutoHeal();
				if (num > 0)
				{
					if (autoHealCount % 2 == 0)
					{
						num = (hitPoint = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, num));
					}
					autoHealCount++;
				}
			}
		}
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.HIT_POINT, hitPoint);
	}

	private int Defense(int damage)
	{
		if (!BuildOption.Instance.Props.useArmor)
		{
			return damage;
		}
		int num = Random.Range(0, armorChanceMax);
		if (num < armorChance && armor > 0)
		{
			int num2 = Mathf.FloorToInt((float)damage * damageSorbRatio);
			int num3 = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.ARMOR, num2);
			int num4 = armor - num3;
			if (num4 < 0)
			{
				num3 = armor;
				num2 = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, armor);
			}
			damage -= num2;
			if (armor <= num3)
			{
				armor = 0;
			}
			else
			{
				armor -= num3;
			}
			armorFont.Scale = 2f;
			NoCheat.Instance.Sync(NoCheat.WATCH_DOG.ARMOR, armor);
		}
		return damage;
	}

	private void GetDamage(int damage)
	{
		hitPoint -= NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, damage);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.HIT_POINT, hitPoint);
	}

	public int ExamShootHack(int shooter, int damage, float rateOfFire)
	{
		float time = Time.time;
		float num = time - dtPre;
		dtPre = time;
		float num2 = rateOfFire / 60f;
		float num3 = 1f / num2;
		num3 *= 0.92f;
		if (num > num3)
		{
			return damage;
		}
		BrickManDesc desc = BrickManManager.Instance.GetDesc(shooter);
		if (desc == null)
		{
			return damage;
		}
		desc.hackShoot++;
		if (desc.hackShoot > 300)
		{
		}
		float num4 = (float)damage * (num / num3);
		return (int)num4;
	}

	public void GetHit(int shooter, int damage, float rigidFactor, int weaponBy, int hitpart, bool autoHealPossible, bool checkZombie)
	{
		if (hitPoint > 0 && damage > 0)
		{
			if (invincible || MyInfoManager.Instance.GodMode)
			{
				if (MyInfoManager.Instance.GodMode)
				{
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GOD_USE);
				}
			}
			else if (!RoomManager.Instance.IsEscapeNotAttack() || (TItemManager.Instance.WeaponBy2Category(weaponBy) != 5 && weaponBy != 26 && weaponBy != 25))
			{
				if (zombieMatch != null)
				{
					if (!zombieMatch.IsPlaying)
					{
						return;
					}
					if (checkZombie && ZombieVsHumanManager.Instance.IsZombie(shooter) && ZombieVsHumanManager.Instance.IsHuman(MyInfoManager.Instance.Seq))
					{
						CSNetManager.Instance.Sock.SendCS_ZOMBIE_INFECT_REQ(MyInfoManager.Instance.Seq, shooter);
						return;
					}
				}
				if (weaponBy != 0 && weaponBy != -6 && weaponBy != 32 && weaponBy != -1)
				{
					damage = Defense(damage);
				}
				if (NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint) <= damage)
				{
					LogAttacker(shooter, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint));
					SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, 0), autoHealPossible);
				}
				else
				{
					LogAttacker(shooter, damage);
					GetDamage(damage);
					rigidity += rigidFactor;
					if (rigidity > maxRigidity)
					{
						rigidity = maxRigidity;
					}
				}
				if (audioSource != null)
				{
					audioSource.Stop();
				}
				MyInfoManager.Instance.bBombInstallFail = true;
				MyInfoManager.Instance.bBombUnInstallFail = true;
				hpFont.Scale = 2f;
				if (NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint) <= 0)
				{
					if (MyInfoManager.Instance.Seq == BrickManManager.Instance.haveFlagSeq)
					{
						bool flag = false;
						if (BrickManager.Instance.userMap != null)
						{
							Vector3 tranceformPosition = TranceformPosition;
							flag = ((tranceformPosition.y >= BrickManager.Instance.userMap.min.y) ? (flag = false) : (flag = true));
						}
						bool flag2 = (weaponBy == 0) ? (flag2 = true) : (flag2 = false);
						bool flag3 = (weaponBy == -6) ? (flag3 = true) : (flag3 = false);
						bool flag4 = false;
						if (shooter == MyInfoManager.Instance.Seq)
						{
							flag4 = ((weaponBy == 24 || weaponBy == 7) ? true : false);
						}
						if (!flag && !flag2 && !flag4 && !flag3)
						{
							SockTcp sock = CSNetManager.Instance.Sock;
							Vector3 tranceformPosition2 = TranceformPosition;
							float x = tranceformPosition2.x;
							Vector3 tranceformPosition3 = TranceformPosition;
							float y = tranceformPosition3.y;
							Vector3 tranceformPosition4 = TranceformPosition;
							sock.SendCS_CTF_DROP_FLAG_REQ(x, y, tranceformPosition4.z);
						}
						else if (BrickManager.Instance.userMap != null)
						{
							SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
							CSNetManager.Instance.Sock.SendCS_CTF_DROP_FLAG_REQ(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
						}
						if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
						{
							checkResetFlag = true;
							dtResetFlag = 0f;
						}
					}
					if (GetComponent<EquipCoordinator>().CurrentWeapon == 3)
					{
						GetComponent<EquipCoordinator>().SetDetonating(set: false);
					}
					else if (GetComponent<EquipCoordinator>().CurrentWeapon == 2)
					{
						GetComponent<EquipCoordinator>().ScopeOff();
					}
					if (actingFever)
					{
						SetUIFever(isSet: false);
						SetWpnFever(isSet: false);
					}
					isJumping = false;
					isBoost = false;
					isTrampoline = false;
					if (boostEff != null)
					{
						Object.DestroyImmediate(boostEff);
					}
					if (trampolineEff != null)
					{
						Object.DestroyImmediate(trampolineEff);
					}
					if (activateBlackhole)
					{
						activateBlackhole = false;
					}
					GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
					CancelTrain();
					SetDie(shooter);
					if (IsUserDamaged() && (weaponBy == -2 || weaponBy == -5 || weaponBy == -7 || weaponBy == -4 || weaponBy == -3))
					{
						weaponBy = -10;
					}
					if (dicInflictedDamage != null)
					{
						CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(dicInflictedDamage);
					}
					if (dicDamageLog != null)
					{
						if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode)
						{
							ZombieVsHumanManager.Instance.SetupLocalDeath(hitpart == 4);
						}
						CSNetManager.Instance.Sock.SendCS_KILL_LOG_REQ(0, shooter, 0, MyInfoManager.Instance.Seq, weaponBy, TItemManager.Instance.WeaponBy2Slot(weaponBy), TItemManager.Instance.WeaponBy2Category(weaponBy), hitpart, dicDamageLog);
					}
					P2PManager.Instance.SendPEER_DIE(shooter);
					if (dicDamageLog != null)
					{
						dicDamageLog.Clear();
					}
					if (dicInflictedDamage != null)
					{
						dicInflictedDamage.Clear();
					}
				}
				BroadcastMessage("OnHit", weaponBy);
				BroadcastMessage("OnHitSnd", shooter);
			}
		}
	}

	public void GetHitFromMob(int monseq, int damage)
	{
		if (hitPoint > 0 && damage > 0 && !invincible && !MyInfoManager.Instance.GodMode)
		{
			damage = Defense(damage);
			if (hitPoint < damage)
			{
				SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, 0), autoHealPossible: true);
			}
			else
			{
				GetDamage(damage);
			}
			hpFont.Scale = 2f;
			if (hitPoint <= 0)
			{
				isBoost = false;
				isTrampoline = false;
				if (boostEff != null)
				{
					Object.DestroyImmediate(boostEff);
				}
				if (trampolineEff != null)
				{
					Object.DestroyImmediate(trampolineEff);
				}
				SetDie(-1);
				CSNetManager.Instance.Sock.SendCS_KILL_LOG_REQ(1, monseq, 0, MyInfoManager.Instance.Seq, 1, -1, -1, -1, dicDamageLog);
				P2PManager.Instance.SendPEER_DIE(-1);
				CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(dicInflictedDamage);
				dicDamageLog.Clear();
				dicInflictedDamage.Clear();
			}
			BroadcastMessage("OnHit", 1);
			BroadcastMessage("OnHitSnd", -1);
		}
	}

	private void CalcResultSpeed(Vector3 prev, Vector3 next)
	{
		prev.y = 0f;
		next.y = 0f;
		resultSpeed = Vector3.Distance(prev, next) / Time.deltaTime;
	}

	private void OnStep()
	{
		int stepOnBrick = BrickManager.Instance.GetStepOnBrick(TranceformPosition);
		if (stepOnBrick >= 0)
		{
			AudioClip stepSound = BrickManager.Instance.GetStepSound(stepOnBrick);
			if (null != stepSound)
			{
				GetComponent<AudioSource>().PlayOneShot(stepSound);
			}
		}
	}

	private void OnReloadStart()
	{
		Gun[] componentsInChildren = GetComponentsInChildren<Gun>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (null != componentsInChildren[i])
			{
				componentsInChildren[i].ClipOut();
			}
		}
	}

	public bool AddBonusAmmo(int add)
	{
		Gun componentInChildren = GetComponentInChildren<Gun>();
		if (null != componentInChildren)
		{
			return componentInChildren.AddBonusAmmo(add);
		}
		return false;
	}

	public bool AddBonusAmmoDF(int add)
	{
		Gun componentInChildren = GetComponentInChildren<Gun>();
		if (null != componentInChildren)
		{
			return componentInChildren.AddBonusAmmoDF(add);
		}
		return false;
	}

	private void OnReload()
	{
		Gun[] componentsInChildren = GetComponentsInChildren<Gun>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (null != componentsInChildren[i])
			{
				componentsInChildren[i].ClipIn();
			}
		}
	}

	private void OnReloadEnd()
	{
		base.animation.Stop("Reload");
		Gun[] componentsInChildren = GetComponentsInChildren<Gun>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (null != componentsInChildren[i])
			{
				componentsInChildren[i].BoltUp();
			}
		}
	}

	private void OnSwitchWeaponStart()
	{
		WeaponFunction[] componentsInChildren = GetComponentsInChildren<WeaponFunction>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (null != componentsInChildren[i])
			{
				componentsInChildren[i].SetDrawn(draw: false);
			}
		}
	}

	private void OnSwitchWeapon()
	{
		GetComponent<EquipCoordinator>().SwapWeapon(initiallyDrawn: false);
		float drawSpeed = GetComponentInChildren<Weapon>().drawSpeed;
		bipAnimation["reload_h"].normalizedSpeed = drawSpeed;
		base.animation["SwitchWeapon"].normalizedSpeed = drawSpeed;
	}

	private void OnSwitchWeaponEnd()
	{
		base.animation.Stop("SwitchWeapon");
		WeaponFunction[] componentsInChildren = GetComponentsInChildren<WeaponFunction>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (null != componentsInChildren[i])
			{
				componentsInChildren[i].SetDrawn(draw: true);
			}
		}
	}

	private void OnAnimationEnd()
	{
	}

	private void OnSlashStart()
	{
		MeleeWeapon componentInChildren = GetComponentInChildren<MeleeWeapon>();
		if (null != componentInChildren)
		{
			componentInChildren.SlashStart();
		}
	}

	private void OnSlashEnd()
	{
		MeleeWeapon componentInChildren = GetComponentInChildren<MeleeWeapon>();
		if (null != componentInChildren)
		{
			componentInChildren.SlashEnd();
		}
	}

	private void OnSlashValidBegin(string action)
	{
		MeleeWeapon componentInChildren = GetComponentInChildren<MeleeWeapon>();
		if (null != componentInChildren)
		{
			componentInChildren.EnterValidRange(action.Contains("big"));
		}
	}

	private void OnSlashValidEnd()
	{
		MeleeWeapon componentInChildren = GetComponentInChildren<MeleeWeapon>();
		if (null != componentInChildren)
		{
			componentInChildren.LeaveValidRange();
		}
	}

	public bool AddBonusBomb(int add)
	{
		HandBomb componentInChildren = GetComponentInChildren<HandBomb>();
		if (null != componentInChildren)
		{
			return componentInChildren.AddBonusBomb(add);
		}
		return false;
	}

	private void OnThrowStart()
	{
		HandBomb componentInChildren = GetComponentInChildren<HandBomb>();
		if (null != componentInChildren)
		{
			componentInChildren.ThrowStart();
		}
	}

	private void OnThrow()
	{
		HandBomb componentInChildren = GetComponentInChildren<HandBomb>();
		if (null != componentInChildren)
		{
			componentInChildren.Throw();
		}
	}

	private void OnThrowEnd()
	{
		HandBomb componentInChildren = GetComponentInChildren<HandBomb>();
		if (null != componentInChildren)
		{
			componentInChildren.ThrowEnd();
		}
	}

	public void DoFireAnimation(string fireAnimation)
	{
		if (!bipAnimation.IsPlaying(fireAnimation))
		{
			bipAnimation.Play(fireAnimation);
		}
		else
		{
			float length = bipAnimation[fireAnimation].length;
			bipAnimation[fireAnimation].time = length / 4f;
		}
	}

	public void DoSlashAnimation(float normalizedSpeed)
	{
		IDidSomething();
		bipAnimation.Stop("slash_big_h");
		base.animation.Stop("Slash");
		bipAnimation["slash_big_h"].normalizedSpeed = normalizedSpeed;
		base.animation["Slash"].normalizedSpeed = normalizedSpeed;
		bipAnimation.Play("slash_big_h");
		base.animation.Blend("Slash");
	}

	public void DoThrowAnimation(string throwAnimation)
	{
		bipAnimation.Stop(throwAnimation);
		base.animation.Stop("Throw");
		bipAnimation.Play(throwAnimation);
		base.animation.Blend("Throw");
	}

	private void InitializeAnimation()
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		bipAnimation = null;
		int num = 0;
		while (bipAnimation == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name == "fps_man")
			{
				bipAnimation = componentsInChildren[num];
			}
			num++;
		}
		if (null == bipAnimation)
		{
			Debug.LogError("Fail to find biped animation for Local Controller");
		}
		else
		{
			bipAnimation.wrapMode = WrapMode.Loop;
			bipAnimation["idle_h"].layer = 1;
			bipAnimation["idle_h"].wrapMode = WrapMode.Loop;
			bipAnimation["2_idle_h"].layer = 1;
			bipAnimation["2_idle_h"].wrapMode = WrapMode.Loop;
			bipAnimation["run_h"].layer = 1;
			bipAnimation["run_h"].wrapMode = WrapMode.Loop;
			bipAnimation["2_run"].layer = 1;
			bipAnimation["2_run"].wrapMode = WrapMode.Loop;
			bipAnimation["walk_h"].layer = 1;
			bipAnimation["walk_h"].wrapMode = WrapMode.Loop;
			bipAnimation["2_walk_h"].layer = 1;
			bipAnimation["2_walk_h"].wrapMode = WrapMode.Loop;
			bipAnimation["1_down_walk"].layer = 1;
			bipAnimation["1_down_walk"].wrapMode = WrapMode.Loop;
			bipAnimation["2_down_walk"].layer = 1;
			bipAnimation["2_down_walk"].wrapMode = WrapMode.Loop;
			bipAnimation["reload_h"].layer = 10;
			bipAnimation["reload_h"].wrapMode = WrapMode.Once;
			bipAnimation["2_reload_h"].layer = 10;
			bipAnimation["2_reload_h"].wrapMode = WrapMode.Once;
			bipAnimation["mashine_r_h"].layer = 10;
			bipAnimation["mashine_r_h"].wrapMode = WrapMode.Once;
			bipAnimation["mashine_s_h"].layer = 10;
			bipAnimation["mashine_s_h"].wrapMode = WrapMode.Once;
			bipAnimation["pistol_h"].layer = 10;
			bipAnimation["pistol_h"].wrapMode = WrapMode.Once;
			bipAnimation["2_pistol_h"].layer = 10;
			bipAnimation["2_pistol_h"].wrapMode = WrapMode.Once;
			bipAnimation["sniping_h"].layer = 10;
			bipAnimation["sniping_h"].wrapMode = WrapMode.Once;
			bipAnimation["shotgun_h"].layer = 10;
			bipAnimation["shotgun_h"].wrapMode = WrapMode.Once;
			bipAnimation["slash_big_h"].layer = 10;
			bipAnimation["slash_big_h"].wrapMode = WrapMode.Once;
			bipAnimation["slash_big_h"].layer = 10;
			bipAnimation["slash_big_h"].wrapMode = WrapMode.Once;
			bipAnimation["throw_h"].layer = 10;
			bipAnimation["throw_h"].wrapMode = WrapMode.Once;
			bipAnimation["stand_h"].layer = 10;
			bipAnimation["stand_h"].wrapMode = WrapMode.Loop;
			bipAnimation["setbomb_h"].layer = 10;
			bipAnimation["setbomb_h"].wrapMode = WrapMode.Once;
			bipAnimation.CrossFade("idle_h");
		}
	}

	private void OnGetCannon(CannonController cannonController)
	{
		if (cannonController.Shooter == MyInfoManager.Instance.Seq && controlContext != CONTROL_CONTEXT.CANNON_CTL)
		{
			moveSpeed = 0f;
			BrickProperty component = cannonController.transform.parent.GetComponent<BrickProperty>();
			if (null != component && !Application.loadedLevelName.Contains("Tutor"))
			{
				CSNetManager.Instance.Sock.SendCS_EMPTY_CANNON_REQ(component.Seq);
			}
		}
	}

	private bool CollideToMon()
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return false;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Mon");
		float num = cc.height / 2f;
		Vector3 tranceformPosition = TranceformPosition;
		float x = tranceformPosition.x;
		Vector3 tranceformPosition2 = TranceformPosition;
		float y = tranceformPosition2.y + num;
		Vector3 tranceformPosition3 = TranceformPosition;
		Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			MonProperty componentInChildren = collider.GetComponentInChildren<MonProperty>();
			if (null != componentInChildren && !componentInChildren.Desc.colHit && (MyInfoManager.Instance.Slot >= 4 || !componentInChildren.Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || componentInChildren.Desc.bRedTeam))
			{
				componentInChildren.Desc.colHit = true;
				GetHit(-1, 10, 0f, -1, -1, autoHealPossible: true, checkZombie: false);
			}
		}
		return false;
	}

	private TrainController CheckTrain()
	{
		if (TrainManager.Instance.IsEmpty())
		{
			return null;
		}
		float height = cc.height;
		for (int i = 0; i < TrainManager.Instance.GetTrainCount(); i++)
		{
			float num = Vector3.Distance(TrainManager.Instance.GetPosition(i), TranceformPosition);
			if (height > num)
			{
				trainIDclosest = i;
				trainSeqclosest = TrainManager.Instance.GetSequance(i);
				return TrainManager.Instance.GetController(i);
			}
		}
		return null;
	}

	private CannonController CheckCannon()
	{
		if (null == cc)
		{
			return null;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Edit Layer");
		float num = cc.height / 2f;
		Vector3 tranceformPosition = TranceformPosition;
		float x = tranceformPosition.x;
		Vector3 tranceformPosition2 = TranceformPosition;
		float y = tranceformPosition2.y + num;
		Vector3 tranceformPosition3 = TranceformPosition;
		Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
			if (null != componentInChildren)
			{
				Brick brick = BrickManager.Instance.GetBrick(componentInChildren.Index);
				if (brick != null && brick.shootable)
				{
					CannonController componentInChildren2 = collider.gameObject.GetComponentInChildren<CannonController>();
					if (null != componentInChildren2 && componentInChildren2.CheckControllable(cam.transform))
					{
						return componentInChildren2;
					}
				}
			}
		}
		return null;
	}

	private CannonController CheckCannonTutor()
	{
		if (null == cc)
		{
			return null;
		}
		if (controlContext == CONTROL_CONTEXT.CANNON_CTL)
		{
			return null;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Edit Layer");
		float num = cc.height / 2f;
		Vector3 tranceformPosition = TranceformPosition;
		float x = tranceformPosition.x;
		Vector3 tranceformPosition2 = TranceformPosition;
		float y = tranceformPosition2.y + num;
		Vector3 tranceformPosition3 = TranceformPosition;
		Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
			if (null != componentInChildren)
			{
				Brick brick = BrickManager.Instance.GetBrick(componentInChildren.Index);
				if (brick != null && brick.shootable)
				{
					CannonController componentInChildren2 = collider.gameObject.GetComponentInChildren<CannonController>();
					if (null != componentInChildren2)
					{
						return componentInChildren2;
					}
				}
			}
		}
		return null;
	}

	private bool ApplyTrain()
	{
		if (Application.loadedLevelName.Contains("MapEdit"))
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && null != bndMatch && bndMatch.IsBuildPhase)
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && null != zombieMatch && !zombieMatch.IsPlaying)
		{
			return false;
		}
		if (controlContext == CONTROL_CONTEXT.CANNON_TRY || controlContext == CONTROL_CONTEXT.CANNON_CTL)
		{
			return false;
		}
		if (controlContext != CONTROL_CONTEXT.TRAIN_TRY && controlContext != CONTROL_CONTEXT.TRAIN_CTL && controlContext != CONTROL_CONTEXT.TRAIN_RUN && custom_inputs.Instance.GetButton("K_ACTION") && CanControl())
		{
			TrainController trainController = CheckTrain();
			if (trainController != null && trainController.shooter < 0)
			{
				moveSpeed = 0f;
				EnterTrain(trainController);
			}
		}
		switch (controlContext)
		{
		case CONTROL_CONTEXT.TRAIN_TRY:
			cannonTryingTime += Time.deltaTime;
			if (trainCtrl == null || !custom_inputs.Instance.GetButton("K_ACTION") || !CanControl() || (trainCtrl.shooter >= 0 && trainCtrl.shooter != MyInfoManager.Instance.Seq))
			{
				LeaveTrain();
				return false;
			}
			if (trainCtrl != null && cannonTryingTime >= trainTryingTimeMax)
			{
				controlContext = CONTROL_CONTEXT.TRAIN_CTL;
				trainID = trainIDclosest;
				trainSeq = trainSeqclosest;
				CSNetManager.Instance.Sock.SendCS_GET_TRAIN_REQ(trainSeq, trainID);
			}
			break;
		case CONTROL_CONTEXT.TRAIN_CTL:
		case CONTROL_CONTEXT.TRAIN_RUN:
			if (trainCtrl != null && custom_inputs.Instance.GetButtonDown("K_JUMP") && CanControl())
			{
				CancelTrain();
			}
			break;
		}
		if (controlContext == CONTROL_CONTEXT.TRAIN_CTL || controlContext == CONTROL_CONTEXT.TRAIN_RUN)
		{
			ApplyTrainMove();
		}
		return controlContext == CONTROL_CONTEXT.TRAIN_TRY || controlContext == CONTROL_CONTEXT.TRAIN_CTL || controlContext == CONTROL_CONTEXT.TRAIN_RUN;
	}

	public void OnGetTrain()
	{
		Vector3 position = trainCtrl.train.transform.position;
		float x = position.x;
		Vector3 position2 = trainCtrl.train.transform.position;
		float y = position2.y + 0.2f;
		Vector3 position3 = trainCtrl.train.transform.position;
		TranceformPosition = new Vector3(x, y, position3.z);
		base.transform.rotation = trainCtrl.train.transform.rotation;
		cam.transform.rotation = trainCtrl.train.transform.rotation;
		CameraController component = cam.GetComponent<CameraController>();
		CameraController cameraController = component;
		Vector3 eulerAngles = cam.transform.rotation.eulerAngles;
		float y2 = eulerAngles.y;
		Vector3 eulerAngles2 = cam.transform.rotation.eulerAngles;
		cameraController.Reset(y2, eulerAngles2.x);
		trainCtrl.shooter = MyInfoManager.Instance.Seq;
		curTrnPos = trainCtrl.train.transform.position;
		if (!FindNeighborRailOrRailLink())
		{
			canTrainStart = false;
			moveSpeed = 0f;
		}
		else
		{
			canTrainStart = true;
		}
	}

	private bool ApplyCannon()
	{
		if (Application.loadedLevelName.Contains("MapEdit"))
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && null != bndMatch && bndMatch.IsBuildPhase)
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && null != zombieMatch && !zombieMatch.IsPlaying)
		{
			return false;
		}
		if (controlContext != CONTROL_CONTEXT.CANNON_TRY && controlContext != CONTROL_CONTEXT.CANNON_CTL && custom_inputs.Instance.GetButton("K_ACTION") && CanControl())
		{
			CannonController cannonController = CheckCannon();
			if (null != cannonController && cannonController.Shooter < 0)
			{
				moveSpeed = 0f;
				EnterCannon(cannonController);
			}
		}
		switch (controlContext)
		{
		case CONTROL_CONTEXT.CANNON_TRY:
			cannonTryingTime += Time.deltaTime;
			if (null == cannon || !custom_inputs.Instance.GetButton("K_ACTION") || !CanControl() || (cannon.Shooter >= 0 && cannon.Shooter != MyInfoManager.Instance.Seq))
			{
				LeaveCannon();
				return false;
			}
			if (null != cannon && cannonTryingTime >= cannon.tryingTime)
			{
				BrickProperty component = cannon.transform.parent.GetComponent<BrickProperty>();
				if (null == component)
				{
					LeaveCannon();
					return false;
				}
				controlContext = CONTROL_CONTEXT.CANNON_CTL;
				if (!Application.loadedLevelName.Contains("Tutor"))
				{
					CSNetManager.Instance.Sock.SendCS_GET_CANNON_REQ(component.Seq);
				}
				else
				{
					cannon.SetShooter(MyInfoManager.Instance.Seq);
				}
			}
			break;
		case CONTROL_CONTEXT.CANNON_CTL:
			if (cannon != null && custom_inputs.Instance.GetButtonDown("K_JUMP") && CanControl())
			{
				CancelCannon();
			}
			break;
		}
		return controlContext == CONTROL_CONTEXT.CANNON_TRY || controlContext == CONTROL_CONTEXT.CANNON_CTL;
	}

	private void EnterTrain(TrainController trainController)
	{
		trainCtrl = trainController;
		CancelSpeedUp();
		controlContext = CONTROL_CONTEXT.TRAIN_TRY;
		cannonTryingTime = 0f;
		onceHideFps = true;
	}

	private void EnterCannon(CannonController cannonController)
	{
		bipAnimation.CrossFade("stand_h");
		GetComponent<EquipCoordinator>().SetActiveCurrentWeapon(state: false);
		cannon = cannonController;
		cannon.CheckSoundOnOff();
		CancelSpeedUp();
		controlContext = CONTROL_CONTEXT.CANNON_TRY;
		cannonTryingTime = 0f;
		BroadcastMessage("OnEnterCannon", cannon);
		onceHideFps = true;
		TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
		if (component != null)
		{
			component.setActive(TUTO_INPUT.M_L);
			component.setClick(TUTO_INPUT.M_L);
		}
	}

	public void initTrain()
	{
		trainCtrl = null;
		trnSeq = -1;
		trainID = -1;
	}

	public void LeaveTrain()
	{
		onceHideFps = false;
		initTrain();
		cannonTryingTime = 0f;
		controlContext = CONTROL_CONTEXT.STOP;
	}

	private void LeaveCannon()
	{
		onceHideFps = false;
		bipAnimation.Stop("stand_h");
		GetComponent<EquipCoordinator>().SetActiveCurrentWeapon(state: true);
		cannon = null;
		cannonTryingTime = 0f;
		controlContext = CONTROL_CONTEXT.NONE;
		cam.gameObject.BroadcastMessage("OnLeaveCannon", this);
	}

	private bool CheckClimbable()
	{
		if (null == cc)
		{
			return false;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Edit Layer");
		float num = cc.radius + 0.2f;
		Vector3 tranceformPosition = TranceformPosition;
		float x = tranceformPosition.x;
		Vector3 tranceformPosition2 = TranceformPosition;
		float y = tranceformPosition2.y + num;
		Vector3 tranceformPosition3 = TranceformPosition;
		Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
			if (null != componentInChildren)
			{
				Brick brick = BrickManager.Instance.GetBrick(componentInChildren.Index);
				if (brick != null && brick.climbable)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsFalling()
	{
		return !IsLadder() && inAirTime > 0.1f;
	}

	private void CheckFalling()
	{
		inAirTime += Time.deltaTime;
		if (IsGrounded() || IsLadder())
		{
			inAirTime = 0f;
			float y = lastGroundedPosition.y;
			Vector3 tranceformPosition = TranceformPosition;
			float num = y - tranceformPosition.y;
			float num2 = veryHigh + changedHeight;
			float num3 = deadlyHigh + changedHeight;
			if (num > veryHigh)
			{
				float num4 = num - num2;
				float num5 = num3 - num2;
				if (num5 < 0.001f)
				{
					Debug.LogError("Deadly High Should be bigger than Very High ");
				}
				else if (fallenDamage && !BrickManager.Instance.userMap.IsPortalMove)
				{
					int num6 = Mathf.FloorToInt(num4 / num5 * 100f);
					if (fallenDamageReduce > 0f)
					{
						num6 -= (int)((float)num6 * fallenDamageReduce);
						if (num6 < 0)
						{
							num6 = 0;
						}
					}
					if (num6 > 0 && IsFallDamageMode())
					{
						P2PManager.Instance.SendPEER_FALL_DOWN();
						if (IsUserDamaged())
						{
							GetHit(MyInfoManager.Instance.Seq, num6, 0.5f, -10, -1, autoHealPossible: true, checkZombie: false);
						}
						else
						{
							GetHit(MyInfoManager.Instance.Seq, num6, 0.5f, 0, -1, autoHealPossible: true, checkZombie: false);
						}
					}
				}
			}
			lastGroundedPosition = TranceformPosition;
		}
		if (hitPoint > 0)
		{
			float bungee = BrickManager.Instance.Bungee;
			Vector3 tranceformPosition2 = TranceformPosition;
			if (bungee > tranceformPosition2.y)
			{
				if (activateBlackhole)
				{
					GetHit(activateBlackholeUser, 1000, 0f, -6, -1, autoHealPossible: false, checkZombie: false);
				}
				else if (IsUserDamaged())
				{
					GetHit(MyInfoManager.Instance.Seq, 1000, 1f, -10, -1, autoHealPossible: true, checkZombie: false);
				}
				else
				{
					GetHit(MyInfoManager.Instance.Seq, 1000, 1f, 0, -1, autoHealPossible: true, checkZombie: false);
				}
			}
		}
	}

	private float GetJumpHeightByControlContext()
	{
		float num = jumpHeight;
		if (controlContext == CONTROL_CONTEXT.SQUATTED || controlContext == CONTROL_CONTEXT.SQUATTED_WALK)
		{
			num *= 0.35f;
		}
		return num;
	}

	private bool ApplyJump()
	{
		if (!isJumping && !IsFalling() && CanControl() && custom_inputs.Instance.GetButtonDown("K_JUMP"))
		{
			if (controlContext != CONTROL_CONTEXT.CLIMB)
			{
				moveSpeed = resultSpeed;
			}
			else
			{
				JumpOnLadder();
			}
			moveSpeed = Mathf.Min(Mathf.Max(moveSpeed, minJumpSpeed), maxJumpSpeed);
			vertDir = Vector3.up;
			vertSpeed = CalculateJumpVerticalSpeed(GetJumpHeightByControlContext());
			isJumping = true;
			controlContext = CONTROL_CONTEXT.JUMP;
		}
		return isJumping;
	}

	private bool IsFallDamageMode()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			return false;
		}
		return true;
	}

	private bool IsGrounded()
	{
		if (null != cc)
		{
			return cc.isGrounded;
		}
		return (collisionFlags & CollisionFlags.Below) != CollisionFlags.None;
	}

	private bool IsCeiled()
	{
		return (collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
	}

	private float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * (float)gravity);
	}

	private bool ApplyClimb()
	{
		bool flag = CheckClimbable();
		if (IsLadder() && (IsGrounded() || !flag || IsDead))
		{
			lastClimbTime = Time.time;
			return false;
		}
		if (custom_inputs.Instance.GetButton("K_SIT"))
		{
			return false;
		}
		if (IsDead)
		{
			return false;
		}
		if (flag && Time.time - lastClimbTime > 0.1f)
		{
			Vector3 vector = cam.transform.TransformDirection(Vector3.forward);
			Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
			Vector3 vector3 = new Vector3(0f, vector.y, 0f);
			vector2 = vector2.normalized;
			vector3 = vector3.normalized;
			float forwardAxisRaw = GetForwardAxisRaw();
			float axisRaw = custom_inputs.Instance.GetAxisRaw("K_RIGHT", "K_LEFT");
			bool flag2 = Mathf.Abs(forwardAxisRaw) > 0.01f || Mathf.Abs(axisRaw) > 0.01f;
			bool button = custom_inputs.Instance.GetButton("K_WALK");
			moveDir = forwardAxisRaw * vector3 + axisRaw * vector2;
			moveDir = moveDir.normalized;
			if (!flag2 || !CanControl())
			{
				moveSpeed = 0f;
				controlContext = CONTROL_CONTEXT.HANG;
			}
			else
			{
				controlContext = CONTROL_CONTEXT.CLIMB;
				if (button)
				{
					moveSpeed = CalcRigidSpeed(climbSpeed / 2f);
				}
				else
				{
					moveSpeed = CalcRigidAndWeightSpeed(climbSpeed);
				}
			}
		}
		return IsLadder();
	}

	public bool CanControl()
	{
		return Screen.lockCursor && hitPoint > 0 && controllable;
	}

	private bool ApplyFly()
	{
		if (MyInfoManager.Instance.IsGM && MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.FLY_MODE)
		{
			if (!MyInfoManager.Instance.isGhostOn)
			{
				GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.CAMERA_FLY_USE);
			}
		}
		else if ((RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE) || MyInfoManager.Instance.ControlMode != MyInfoManager.CONTROL_MODE.PLAYING_FLY_MODE)
		{
			if (vertDir != Vector3.up)
			{
				vertDir = Vector3.up;
				vertSpeed = 0f;
			}
			return false;
		}
		Vector3 a = cam.transform.TransformDirection(Vector3.forward);
		Vector3 a2 = new Vector3(a.z, 0f, 0f - a.x);
		float forwardAxisRaw = GetForwardAxisRaw();
		float axisRaw = custom_inputs.Instance.GetAxisRaw("K_RIGHT", "K_LEFT");
		float axisRaw2 = custom_inputs.Instance.GetAxisRaw("K_FLY_UP", "K_FLY_DOWN");
		float axisRaw3 = custom_inputs.Instance.GetAxisRaw("K_JUMP", "K_FLY_DOWN");
		bool flag = (double)Mathf.Abs(axisRaw) > 0.01 || (double)Mathf.Abs(forwardAxisRaw) > 0.01;
		bool button = custom_inputs.Instance.GetButton("K_WALK");
		moveDir = forwardAxisRaw * a + axisRaw * a2;
		moveDir = moveDir.normalized;
		if ((double)Mathf.Abs(axisRaw2) > 0.01)
		{
			vertDir = axisRaw2 * Vector3.up;
			vertDir = vertDir.normalized;
			if (button)
			{
				vertSpeed = CalcRigidSpeed(walkSpeed);
			}
			else
			{
				vertSpeed = CalcRigidAndWeightSpeed(runSpeed);
			}
		}
		else if ((double)Mathf.Abs(axisRaw3) > 0.01)
		{
			vertDir = axisRaw3 * Vector3.up;
			vertDir = vertDir.normalized;
			if (button)
			{
				vertSpeed = CalcRigidSpeed(walkSpeed);
			}
			else
			{
				vertSpeed = CalcRigidAndWeightSpeed(runSpeed);
			}
		}
		else
		{
			vertDir = Vector3.up;
			vertSpeed = 0f;
		}
		if (!CanControl())
		{
			vertSpeed = 0f;
		}
		controlContext = CONTROL_CONTEXT.RUN;
		if (button)
		{
			controlContext = CONTROL_CONTEXT.WALK;
		}
		if (!flag || !CanControl())
		{
			controlContext = CONTROL_CONTEXT.STOP;
		}
		switch (controlContext)
		{
		case CONTROL_CONTEXT.WALK:
			moveSpeed = CalcRigidSpeed(walkSpeed);
			break;
		case CONTROL_CONTEXT.RUN:
			moveSpeed = CalcRigidAndWeightSpeed(runSpeed);
			break;
		case CONTROL_CONTEXT.STOP:
			moveSpeed = 0f;
			break;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR && MyInfoManager.Instance.FlyingFast)
		{
			moveSpeed += moveSpeed * 0.05f;
			vertSpeed += vertSpeed * 0.05f;
		}
		return true;
	}

	private void JumpOnLadder()
	{
		Vector3 a = cam.transform.TransformDirection(Vector3.forward);
		a.y = 0f;
		a = a.normalized;
		Vector3 a2 = new Vector3(a.z, 0f, 0f - a.x);
		float num = GetForwardAxisRaw();
		float num2 = custom_inputs.Instance.GetAxisRaw("K_RIGHT", "K_LEFT");
		if (Application.loadedLevelName.Contains("Tutor") && DialogManager.Instance.IsModal)
		{
			num = 0f;
			num2 = 0f;
		}
		bool flag = (double)Mathf.Abs(num2) > 0.01 || (double)Mathf.Abs(num) > 0.01;
		bool button = custom_inputs.Instance.GetButton("K_WALK");
		moveDir = num * a + num2 * a2;
		moveDir = moveDir.normalized;
		controlContext = CONTROL_CONTEXT.RUN;
		if (button)
		{
			controlContext = CONTROL_CONTEXT.WALK;
		}
		if (!flag || !CanControl())
		{
			controlContext = CONTROL_CONTEXT.STOP;
		}
		moveSpeed = CalcRigidSpeed(walkSpeed / 2f);
	}

	private bool FindNeighborRailOrRailLink()
	{
		Vector3[] array = new Vector3[8]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(1f, -1f, 0f),
			new Vector3(-1f, -1f, 0f),
			new Vector3(0f, -1f, 1f),
			new Vector3(0f, -1f, -1f)
		};
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 8; i++)
		{
			zero = trainCtrl.train.transform.position + array[i];
			GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(zero);
			if (brickObjectByPos != null)
			{
				BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
				if (component != null && (component.Index == 197 || component.Index == 198 || component.Index == 202 || component.Index == 203 || component.Index == 199))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsSameRailDirection(TRAIN_MOVE_DIR Tmr)
	{
		if (Tmr == trnMDir)
		{
			return true;
		}
		return false;
	}

	private bool CheckDirectionSameKind(TRAIN_MOVE_DIR Tmr)
	{
		if ((Tmr == TRAIN_MOVE_DIR.EAST || Tmr == TRAIN_MOVE_DIR.WEST) && (trnMDir == TRAIN_MOVE_DIR.EAST || trnMDir == TRAIN_MOVE_DIR.WEST))
		{
			return true;
		}
		if ((Tmr == TRAIN_MOVE_DIR.NORTH || Tmr == TRAIN_MOVE_DIR.SOUTH) && (trnMDir == TRAIN_MOVE_DIR.NORTH || trnMDir == TRAIN_MOVE_DIR.SOUTH))
		{
			return true;
		}
		return false;
	}

	private TRAIN_MOVE_DIR GetDirectionRail(Vector3 forward)
	{
		TRAIN_MOVE_DIR result = TRAIN_MOVE_DIR.NONE;
		int num = Mathf.RoundToInt(forward.x);
		int num2 = Mathf.RoundToInt(forward.z);
		if (num > 0)
		{
			return TRAIN_MOVE_DIR.EAST;
		}
		if (num < 0)
		{
			return TRAIN_MOVE_DIR.WEST;
		}
		if (num2 > 0)
		{
			return TRAIN_MOVE_DIR.NORTH;
		}
		if (num2 < 0)
		{
			return TRAIN_MOVE_DIR.SOUTH;
		}
		return result;
	}

	private TRAIN_MOVE_DIR GetDirectionRailLink(Vector3 forward)
	{
		TRAIN_MOVE_DIR result = TRAIN_MOVE_DIR.NONE;
		int num = Mathf.RoundToInt(forward.x);
		int num2 = Mathf.RoundToInt(forward.z);
		if (num > 0)
		{
			return TRAIN_MOVE_DIR.EAST;
		}
		if (num < 0)
		{
			return TRAIN_MOVE_DIR.WEST;
		}
		if (num2 > 0)
		{
			return TRAIN_MOVE_DIR.NORTH;
		}
		if (num2 < 0)
		{
			return TRAIN_MOVE_DIR.SOUTH;
		}
		return result;
	}

	private Vector3 GetRailDir(float v)
	{
		Vector3 tranceformPosition = TranceformPosition;
		GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(new Vector3(tranceformPosition.x, tranceformPosition.y, tranceformPosition.z));
		if (brickObjectByPos != null)
		{
			BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
			if (component != null)
			{
				if (trnSeq == component.Seq)
				{
					return trnFwd;
				}
				if (component.Index == 197)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.RAIL;
					IsSlopeUp = false;
					trnSeq = component.Seq;
					trnFwd = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component.Index == 198)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					Rotated = false;
					eRail = ERAIL.LINK;
					IsSlopeUp = false;
					trnSeq = component.Seq;
					trnFwd = Quaternion.Euler(0f, 90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRailLink(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component.Index == 202)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					Rotated = false;
					eRail = ERAIL.LINK;
					IsSlopeUp = false;
					trnSeq = component.Seq;
					trnFwd = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRailLink(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component.Index == 199)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.UP;
					IsSlopeUp = true;
					trnSeq = component.Seq;
					trnFwd = Quaternion.Euler(0f, -180f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component.Index == 203)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.DN;
					IsSlopeUp = false;
					trnSeq = component.Seq;
					trnFwd = brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
			}
		}
		if (IsSlopeUp)
		{
			tranceformPosition.y += 1f;
		}
		else
		{
			tranceformPosition.y -= 1f;
		}
		brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(new Vector3(tranceformPosition.x, tranceformPosition.y, tranceformPosition.z));
		if (brickObjectByPos != null)
		{
			BrickProperty component2 = brickObjectByPos.GetComponent<BrickProperty>();
			if (component2 != null)
			{
				if (trnSeq == component2.Seq)
				{
					return trnFwd;
				}
				if (component2.Index == 197)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.RAIL;
					IsSlopeUp = false;
					trnSeq = component2.Seq;
					trnFwd = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component2.Index == 198)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.LINK;
					IsSlopeUp = false;
					trnSeq = component2.Seq;
					trnFwd = Quaternion.Euler(0f, 90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRailLink(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component2.Index == 202)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.LINK;
					IsSlopeUp = false;
					trnSeq = component2.Seq;
					trnFwd = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRailLink(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component2.Index == 199)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.UP;
					IsSlopeUp = true;
					trnSeq = component2.Seq;
					trnFwd = Quaternion.Euler(0f, -180f, 0f) * brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
				if (component2.Index == 203)
				{
					if (!RailOvered)
					{
						RailOvered = true;
					}
					eRail = ERAIL.DN;
					IsSlopeUp = false;
					trnSeq = component2.Seq;
					trnFwd = brickObjectByPos.transform.forward;
					trnMDir = GetDirectionRail(trnFwd);
					curTrnPos = brickObjectByPos.transform.position;
					return trnFwd;
				}
			}
		}
		trnFwd = trainCtrl.train.transform.forward;
		if (!RailOvered)
		{
			trnMDir = GetDirectionRail(trnFwd);
		}
		return trnFwd;
	}

	private bool CurPosIsRail(Vector3 p)
	{
		GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(p);
		if (brickObjectByPos != null)
		{
			BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
			if (component != null && (component.Index == 197 || component.Index == 198 || component.Index == 202 || component.Index == 203 || component.Index == 199))
			{
				return true;
			}
		}
		p.y -= 1f;
		brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(p);
		if (brickObjectByPos != null)
		{
			BrickProperty component2 = brickObjectByPos.GetComponent<BrickProperty>();
			if (component2 != null && (component2.Index == 197 || component2.Index == 198 || component2.Index == 202 || component2.Index == 203 || component2.Index == 199))
			{
				return true;
			}
		}
		return false;
	}

	private bool FindNextRailPath(float v)
	{
		Vector3 vector = trainCtrl.train.transform.forward;
		Vector3 tranceformPosition = TranceformPosition;
		if (CurPosIsRail(tranceformPosition))
		{
			vector = GetRailDir(v);
		}
		else
		{
			trnMDir = GetDirectionRail(vector);
		}
		Vector3 pos = Vector3.zero;
		if (v > 0f || v < 0f)
		{
			pos = curTrnPos + v * vector;
		}
		GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(pos);
		if (brickObjectByPos != null)
		{
			BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
			if (component != null)
			{
				if (trnNextSeq == component.Seq)
				{
					return true;
				}
				if (component.Index == 197)
				{
					trnNextSeq = component.Seq;
					Vector3 forward = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail = GetDirectionRail(forward);
					if (IsSameRailDirection(directionRail))
					{
						return true;
					}
					return false;
				}
				if (component.Index == 198)
				{
					trnNextSeq = component.Seq;
					railPosCenter = brickObjectByPos.transform.position;
					railRotCenter = Quaternion.Euler(0f, 90f, 0f) * brickObjectByPos.transform.rotation;
					return true;
				}
				if (component.Index == 202)
				{
					trnNextSeq = component.Seq;
					railPosCenter = brickObjectByPos.transform.position;
					railRotCenter = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.rotation;
					return true;
				}
				if (component.Index == 199)
				{
					trnNextSeq = component.Seq;
					Vector3 forward2 = Quaternion.Euler(0f, -180f, 0f) * brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail2 = GetDirectionRail(forward2);
					if (IsSameRailDirection(directionRail2))
					{
						return true;
					}
					return false;
				}
				if (component.Index == 203)
				{
					trnNextSeq = component.Seq;
					Vector3 forward3 = brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail3 = GetDirectionRail(forward3);
					if (IsSameRailDirection(directionRail3))
					{
						return true;
					}
					return false;
				}
			}
		}
		if (IsSlopeUp)
		{
			pos.y += 1f;
		}
		else
		{
			pos.y -= 1f;
		}
		brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(pos);
		if (brickObjectByPos != null)
		{
			BrickProperty component2 = brickObjectByPos.GetComponent<BrickProperty>();
			if (component2 != null)
			{
				if (trnNextSeq == component2.Seq)
				{
					return true;
				}
				if (component2.Index == 197)
				{
					trnNextSeq = component2.Seq;
					Vector3 forward4 = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail4 = GetDirectionRail(forward4);
					if (IsSameRailDirection(directionRail4))
					{
						return true;
					}
					return false;
				}
				if (component2.Index == 198)
				{
					trnNextSeq = component2.Seq;
					railPosCenter = brickObjectByPos.transform.position;
					railRotCenter = Quaternion.Euler(0f, 90f, 0f) * brickObjectByPos.transform.rotation;
					return true;
				}
				if (component2.Index == 202)
				{
					trnNextSeq = component2.Seq;
					railPosCenter = brickObjectByPos.transform.position;
					railRotCenter = Quaternion.Euler(0f, -90f, 0f) * brickObjectByPos.transform.rotation;
					return true;
				}
				if (component2.Index == 199)
				{
					trnNextSeq = component2.Seq;
					Vector3 forward5 = Quaternion.Euler(0f, -180f, 0f) * brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail5 = GetDirectionRail(forward5);
					if (IsSameRailDirection(directionRail5))
					{
						return true;
					}
					return false;
				}
				if (component2.Index == 203)
				{
					trnNextSeq = component2.Seq;
					Vector3 forward6 = brickObjectByPos.transform.forward;
					TRAIN_MOVE_DIR directionRail6 = GetDirectionRail(forward6);
					if (IsSameRailDirection(directionRail6))
					{
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}

	public void TrainStop()
	{
		moveSpeed = 0f;
		controlContext = CONTROL_CONTEXT.TRAIN_CTL;
		canTrainStart = false;
	}

	private bool ApplyTrainMove()
	{
		if (!canTrainStart)
		{
			return false;
		}
		float num = 1f;
		if (!FindNextRailPath(num))
		{
			TrainStop();
			return false;
		}
		if (Application.loadedLevelName.Contains("Tutor") && DialogManager.Instance.IsModal)
		{
			num = 0f;
		}
		bool flag = (double)Mathf.Abs(num) > 0.01;
		moveDir = trainCtrl.train.transform.forward;
		controlContext = CONTROL_CONTEXT.TRAIN_RUN;
		moveSpeed = trainSpeed;
		if (!CanControl())
		{
			controlContext = CONTROL_CONTEXT.TRAIN_CTL;
		}
		else if (!flag)
		{
			controlContext = CONTROL_CONTEXT.TRAIN_CTL;
		}
		CONTROL_CONTEXT cONTROL_CONTEXT = controlContext;
		if (cONTROL_CONTEXT == CONTROL_CONTEXT.TRAIN_CTL)
		{
			moveSpeed = 0f;
		}
		return true;
	}

	private void ApplyMove()
	{
		if (trainCtrl == null)
		{
			Vector3 a = cam.transform.TransformDirection(Vector3.forward);
			a.y = 0f;
			a = a.normalized;
			Vector3 a2 = new Vector3(a.z, 0f, 0f - a.x);
			float num = GetForwardAxisRaw();
			float num2 = custom_inputs.Instance.GetAxisRaw("K_RIGHT", "K_LEFT");
			if (Application.loadedLevelName.Contains("Tutor") && DialogManager.Instance.IsModal)
			{
				num = 0f;
				num2 = 0f;
			}
			CONTROL_CONTEXT cONTROL_CONTEXT = controlContext;
			bool flag = (double)Mathf.Abs(num2) > 0.01 || (double)Mathf.Abs(num) > 0.01;
			bool button = custom_inputs.Instance.GetButton("K_WALK");
			bool button2 = custom_inputs.Instance.GetButton("K_SIT");
			moveDir = num * a + num2 * a2;
			moveDir = moveDir.normalized;
			controlContext = CONTROL_CONTEXT.RUN;
			if (button2)
			{
				controlContext = CONTROL_CONTEXT.SQUATTED_WALK;
			}
			else if (button)
			{
				controlContext = CONTROL_CONTEXT.WALK;
			}
			if (!CanControl())
			{
				controlContext = CONTROL_CONTEXT.STOP;
			}
			else if (!flag)
			{
				controlContext = (button2 ? CONTROL_CONTEXT.SQUATTED : CONTROL_CONTEXT.STOP);
			}
			switch (controlContext)
			{
			case CONTROL_CONTEXT.WALK:
			case CONTROL_CONTEXT.SQUATTED_WALK:
				moveSpeed = CalcRigidSpeed(walkSpeed);
				break;
			case CONTROL_CONTEXT.RUN:
				moveSpeed = CalcRigidAndWeightSpeed(runSpeed);
				break;
			case CONTROL_CONTEXT.STOP:
			case CONTROL_CONTEXT.SQUATTED:
				moveSpeed = 0f;
				break;
			}
			if (((cONTROL_CONTEXT == CONTROL_CONTEXT.SQUATTED || cONTROL_CONTEXT == CONTROL_CONTEXT.SQUATTED_WALK) && controlContext != CONTROL_CONTEXT.SQUATTED && controlContext != CONTROL_CONTEXT.SQUATTED_WALK) || (cONTROL_CONTEXT != CONTROL_CONTEXT.SQUATTED && cONTROL_CONTEXT != CONTROL_CONTEXT.SQUATTED_WALK && (controlContext == CONTROL_CONTEXT.SQUATTED || controlContext == CONTROL_CONTEXT.SQUATTED_WALK)))
			{
				sitUpingDelta = 0f;
				orgEyePosition = cam.transform.localPosition;
			}
		}
	}

	private bool IsDasashAble()
	{
		return true;
	}

	private void IncreaseStaminaIfNoDash()
	{
		if (!dashState)
		{
			stamina = (float)stamina + Time.deltaTime * 4f;
			if ((float)stamina > staminaMax + staminaMaxAdd)
			{
				stamina = staminaMax + staminaMaxAdd;
			}
		}
	}

	private void ApplyDash()
	{
		if (BuildOption.Instance.Props.useDefaultDash && RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase) && controllable && hitPoint > 0)
		{
			doublePressTime += Time.deltaTime;
			bool button = custom_inputs.Instance.GetButton("K_WALK");
			bool button2 = custom_inputs.Instance.GetButton("K_SIT");
			if (dashState)
			{
				stamina = (float)stamina - Time.deltaTime * 20f;
				if ((float)stamina < 0f)
				{
					stamina = 0f;
					dashState = false;
					SpeedUpMyEffect();
				}
				else if (button || button2)
				{
					dashState = false;
					SpeedUpMyEffect();
				}
			}
			if (!dashState && IsDasashAble())
			{
				if (custom_inputs.Instance.GetButtonDown("K_FORWARD"))
				{
					if (doublePressTime < doublePressMax && !button && !button2)
					{
						dashState = true;
						SpeedUpMyEffect();
					}
					else
					{
						doublePressTime = 0f;
					}
				}
			}
			else
			{
				float forwardAxisRaw = GetForwardAxisRaw();
				float axisRaw = custom_inputs.Instance.GetAxisRaw("K_RIGHT", "K_LEFT");
				if (forwardAxisRaw == 0f && axisRaw == 0f)
				{
					dashState = false;
					SpeedUpMyEffect();
				}
			}
		}
	}

	private float CalcRigidSpeed(float speed)
	{
		if (speed < 0f)
		{
			return 0f;
		}
		float num = 1f - rigidity;
		if (num < 0f)
		{
			num = 0f;
		}
		if (speedUpEffect)
		{
			return speed * 1.75f;
		}
		return speed * num;
	}

	private float CalcRigidAndWeightSpeed(float speed)
	{
		if (speed < 0f)
		{
			return 0f;
		}
		float num = 1f - rigidity;
		if (num < 0f)
		{
			num = 0f;
		}
		WeaponFunction componentInChildren = GetComponentInChildren<WeaponFunction>();
		if (speedUpEffect)
		{
			return speed * emptyHandSpeedFactor;
		}
		float num2 = (!(componentInChildren == null)) ? componentInChildren.speedFactor : emptyHandSpeedFactor;
		return speed * num * num2 * zombieVirus.SpeedFactor;
	}

	public bool CanAimAccurately()
	{
		return controlContext == CONTROL_CONTEXT.SQUATTED_WALK || controlContext == CONTROL_CONTEXT.SQUATTED || controlContext == CONTROL_CONTEXT.WALK || controlContext == CONTROL_CONTEXT.STOP;
	}

	public bool CanAimAccuratelyMore()
	{
		return controlContext == CONTROL_CONTEXT.SQUATTED_WALK || controlContext == CONTROL_CONTEXT.SQUATTED;
	}

	private bool IsLadder()
	{
		return controlContext == CONTROL_CONTEXT.CLIMB || controlContext == CONTROL_CONTEXT.HANG;
	}

	private void ApplyGravity()
	{
		if (RoomManager.Instance.CurrentRoomType != 0 || MyInfoManager.Instance.ControlMode != MyInfoManager.CONTROL_MODE.PLAYING_FLY_MODE)
		{
			if (IsLadder())
			{
				vertSpeed = 0f;
			}
			else
			{
				if (vertSpeed > 0f && IsCeiled())
				{
					vertSpeed = 0f;
				}
				float bungee = BrickManager.Instance.Bungee;
				Vector3 tranceformPosition = TranceformPosition;
				if (bungee <= tranceformPosition.y)
				{
					vertSpeed -= (float)gravity * Time.deltaTime;
					if (bungeeRespawn && vertSpeed < -20f)
					{
						Vector3 tranceformPosition2 = TranceformPosition;
						if (tranceformPosition2.y < 50f)
						{
							vertSpeed = -20f;
						}
					}
				}
				if (vertSpeed < 0f && IsGrounded())
				{
					if (isJumping)
					{
						VerifyAudioSource();
						if (null != audioSource && null != sndLanding)
						{
							audioSource.PlayOneShot(sndLanding);
						}
					}
					isJumping = false;
					vertSpeed = 0f;
					jumpHeight = savedJumpHeight;
					bungeeRespawn = false;
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
				{
					Transform transform = base.transform.FindChild("FX_Falling");
					if (null != transform)
					{
						if (bungeeRespawn && !transform.gameObject.activeInHierarchy)
						{
							transform.gameObject.SetActive(value: true);
						}
						if (!bungeeRespawn && transform.gameObject.activeInHierarchy)
						{
							transform.gameObject.SetActive(value: false);
						}
					}
				}
			}
		}
	}

	private void CheckDeath()
	{
		if (hitPoint <= 0)
		{
			deltaFromDeathSecure.Add(Time.deltaTime);
		}
	}

	public void CancelTrain()
	{
		if (trainCtrl != null && trainCtrl.shooter == MyInfoManager.Instance.Seq)
		{
			trainCtrl.shooter = -1;
			trainCtrl.regen();
			eRail = ERAIL.NONE;
			Rotated = false;
			RailOvered = false;
			IsSlopeUp = false;
			CSNetManager.Instance.Sock.SendCS_EMPTY_TRAIN_REQ(trainID);
		}
		LeaveTrain();
	}

	public void CancelCannon()
	{
		if (cannon != null && cannon.Shooter == MyInfoManager.Instance.Seq)
		{
			TranceformPosition = cannon.tpsLink.transform.position;
			BrickProperty component = cannon.transform.parent.GetComponent<BrickProperty>();
			if (null != component)
			{
				if (!Application.loadedLevelName.Contains("Tutor"))
				{
					CSNetManager.Instance.Sock.SendCS_EMPTY_CANNON_REQ(component.Seq);
				}
				else
				{
					cannon.SetShooter(-1);
				}
			}
		}
		LeaveCannon();
	}

	private void SetDie(int killer)
	{
		if (RoomManager.Instance.DropItem)
		{
			if (dropWeaponSkip)
			{
				dropWeaponSkip = false;
			}
			else
			{
				GlobalVars.Instance.DropWeapon(-1);
			}
		}
		CancelSpeedUp();
		CancelCannon();
		ResetWeaponChange();
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			ReturnBuildGun();
		}
		rigidity = 0f;
		deltaFromDeathSecure.Set(0f);
		BroadcastMessage("OnDeath", killer);
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.EXPLOSION && RoomManager.Instance.NeedRespawnTicket)
		{
			CSNetManager.Instance.Sock.SendCS_RESPAWN_TICKET_REQ();
		}
	}

	private bool VerifyCamera()
	{
		if (cam == null)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
			}
		}
		return cam != null;
	}

	public float GetVertAngle()
	{
		Transform transform = cam.transform;
		Vector3 eulerAngles = transform.eulerAngles;
		float num = eulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, -60f, 60f);
		return 0f - num;
	}

	public float GetHorzAngle()
	{
		Transform transform = cam.transform;
		Vector3 eulerAngles = transform.eulerAngles;
		return eulerAngles.y;
	}

	private void NotifyMove()
	{
		lastNotified += Time.deltaTime;
		lastNotifiedDir += Time.deltaTime;
		if (!BuildOption.Instance.Props.packetSaving)
		{
			if ((lastNotified > 0.3f || ntfdControlContext != controlContext || Mathf.Abs(vertSpeed) > 2f || Mathf.Abs(ntfdSpeed - moveSpeed) > 0.5f) && lastNotified > BuildOption.Instance.Props.SendRate)
			{
				lastNotified = 0f;
				if (controlContext != CONTROL_CONTEXT.CANNON_CTL)
				{
					ntfdControlContext = controlContext;
					ntfdSpeed = moveSpeed;
					if (eBearTrap == EBEARTRAP.OVER)
					{
						moveSpeed = 0f;
					}
					P2PManager.Instance.SendPEER_MOVE(MyInfoManager.Instance.Seq, (int)controlContext, moveSpeed, vertSpeed, TranceformPosition, IsDead, isRegularSend: true);
				}
			}
		}
		else
		{
			if ((ntfdControlContext != controlContext || Mathf.Abs(vertSpeed) > 2f || Mathf.Abs(ntfdSpeed - moveSpeed) > 0.5f) && lastNotified > BuildOption.Instance.Props.SendRate)
			{
				lastNotified = 0f;
				if (controlContext != CONTROL_CONTEXT.CANNON_CTL)
				{
					ntfdSpeed = moveSpeed;
					ntfdControlContext = controlContext;
					P2PManager.Instance.SendPEER_MOVE(MyInfoManager.Instance.Seq, (int)controlContext, moveSpeed, vertSpeed, TranceformPosition, IsDead, isRegularSend: true);
				}
			}
			if (lastNotified > 0.3f && lastNotified > BuildOption.Instance.Props.SendRate)
			{
				lastNotified = 0f;
				if (controlContext != CONTROL_CONTEXT.CANNON_CTL)
				{
					ntfdSpeed = moveSpeed;
					ntfdControlContext = controlContext;
					GameObject gameObject = GlobalVars.Instance.VerifyMe();
					GameObject gameObject2 = GlobalVars.Instance.VerifyMainCamera();
					if (gameObject2 == null)
					{
						return;
					}
					Camera component = gameObject2.GetComponent<Camera>();
					Vector3 position = base.transform.position;
					float x = position.x;
					Vector3 position2 = base.transform.position;
					float y = position2.y;
					Vector3 position3 = base.transform.position;
					Vector3 myPos = new Vector3(x, y, position3.z);
					BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
					foreach (BrickManDesc brickManDesc in array)
					{
						if (brickManDesc.Seq != MyInfoManager.Instance.Seq)
						{
							if (brickManDesc.nextmov)
							{
								P2PManager.Instance.SendPEER_MOVE_W(brickManDesc.Seq, (int)controlContext, moveSpeed, vertSpeed, TranceformPosition, IsDead, isRegularSend: true);
								brickManDesc.nextmov = false;
							}
							else
							{
								component.enabled = false;
								GameObject player = BrickManManager.Instance.Get(brickManDesc.Seq);
								if (GeniusPacketSend.checkSendMove(player, myPos, gameObject.collider.bounds) != GeniusPacketSend.SEND_PACKET_LEVEL.NONE)
								{
									P2PManager.Instance.SendPEER_MOVE_W(brickManDesc.Seq, (int)controlContext, moveSpeed, vertSpeed, TranceformPosition, IsDead, isRegularSend: true);
								}
								else
								{
									brickManDesc.nextmov = true;
								}
								component.enabled = true;
							}
						}
					}
				}
			}
		}
		Vector3 normalized = cam.transform.TransformDirection(Vector3.forward).normalized;
		if (Mathf.Abs(Vector3.Angle(ntfdDir, normalized)) > 1f && lastNotifiedDir > BuildOption.Instance.Props.SendRate)
		{
			if (controlContext != CONTROL_CONTEXT.CANNON_CTL)
			{
				ntfdDir = normalized;
				P2PManager.Instance.SendPEER_DIR(MyInfoManager.Instance.Seq, moveDir, GetHorzAngle(), GetVertAngle());
			}
			lastNotifiedDir = 0f;
		}
		if (lastNotifiedDir > 0.3f && lastNotifiedDir > BuildOption.Instance.Props.SendRate)
		{
			if (controlContext != CONTROL_CONTEXT.CANNON_CTL)
			{
				GameObject gameObject3 = GameObject.Find("Me");
				if (null != gameObject3)
				{
					Camera camera = null;
					GameObject gameObject4 = GameObject.Find("Main Camera");
					if (null != gameObject4)
					{
						camera = gameObject4.GetComponent<Camera>();
						camera.enabled = false;
					}
					BrickManDesc[] array2 = BrickManManager.Instance.ToDescriptorArray();
					for (int j = 0; j < array2.Length; j++)
					{
						GameObject player2 = BrickManManager.Instance.Get(array2[j].Seq);
						if (array2[j].Seq != MyInfoManager.Instance.Seq)
						{
							Vector3 position4 = base.transform.position;
							float x2 = position4.x;
							Vector3 position5 = base.transform.position;
							float y2 = position5.y;
							Vector3 position6 = base.transform.position;
							Vector3 myPos2 = new Vector3(x2, y2, position6.z);
							if (GeniusPacketSend.checkSendDir(player2, myPos2, gameObject3.collider.bounds) != GeniusPacketSend.SEND_PACKET_LEVEL.NONE)
							{
								ntfdDir = normalized;
								P2PManager.Instance.SendPEER_DIR_W(array2[j].Seq, MyInfoManager.Instance.Seq, moveDir, GetHorzAngle(), GetVertAngle());
							}
						}
					}
					camera.enabled = true;
				}
			}
			lastNotifiedDir = 0f;
		}
	}

	private void ApplyAnimation()
	{
		if (prevControlContext == CONTROL_CONTEXT.NONE || prevControlContext != controlContext)
		{
			switch (controlContext)
			{
			case CONTROL_CONTEXT.STOP:
			case CONTROL_CONTEXT.HANG:
			case CONTROL_CONTEXT.SQUATTED:
				if (!GetComponent<EquipCoordinator>().IsTwoHands())
				{
					bipAnimation.CrossFade("idle_h");
				}
				else
				{
					bipAnimation.CrossFade("2_idle_h");
				}
				break;
			case CONTROL_CONTEXT.RUN:
			case CONTROL_CONTEXT.JUMP:
			case CONTROL_CONTEXT.CLIMB:
				if (!GetComponent<EquipCoordinator>().IsTwoHands())
				{
					bipAnimation.CrossFade("run_h");
				}
				else
				{
					bipAnimation.CrossFade("2_run");
				}
				break;
			case CONTROL_CONTEXT.WALK:
			case CONTROL_CONTEXT.SQUATTED_WALK:
				if (!GetComponent<EquipCoordinator>().IsTwoHands())
				{
					bipAnimation.CrossFade("walk_h");
				}
				else
				{
					bipAnimation.CrossFade("2_walk_h");
				}
				break;
			}
			switch (controlContext)
			{
			case CONTROL_CONTEXT.RUN:
			case CONTROL_CONTEXT.CLIMB:
				base.animation.Blend("StepRun");
				break;
			default:
				base.animation.Stop("StepRun");
				break;
			}
			prevControlContext = controlContext;
		}
	}

	private int CheckShortcut()
	{
		if (viewUpgradeBox)
		{
			return -1;
		}
		if (radioMenu == null || !radioMenu.On)
		{
			string[] array = null;
			switch (RoomManager.Instance.CurrentRoomType)
			{
			case Room.ROOM_TYPE.BND:
				array = new string[4]
				{
					"K_MAIN",
					"K_AUX",
					"K_MELEE",
					"K_SPEC"
				};
				if (BuildOption.Instance.AllowBuildGunInDestroyPhase() && RoomManager.Instance.UseBuildGun && custom_inputs.Instance.GetButtonDown("K_MODE_TOGGLE_IN_BND"))
				{
					bndMatch.IsBuilderMode = !bndMatch.IsBuilderMode;
					return (!bndMatch.IsBuilderMode) ? FORCE_PREV : 4;
				}
				break;
			case Room.ROOM_TYPE.ESCAPE:
				if (!RoomManager.Instance.IsEscapeNotAttack())
				{
					array = new string[5]
					{
						"K_MAIN",
						"K_AUX",
						"K_MELEE",
						"K_SPEC",
						"K_MODE"
					};
				}
				break;
			default:
				array = new string[5]
				{
					"K_MAIN",
					"K_AUX",
					"K_MELEE",
					"K_SPEC",
					"K_MODE"
				};
				break;
			case Room.ROOM_TYPE.MAP_EDITOR:
			case Room.ROOM_TYPE.BUNGEE:
				break;
			}
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND)
			{
				if (GetComponent<EquipCoordinator>().CurrentWeapon != 4)
				{
					int num = 0;
					while (array != null && num < array.Length)
					{
						if (custom_inputs.Instance.GetButtonDown(array[num]))
						{
							return num;
						}
						num++;
					}
				}
			}
			else
			{
				int num2 = 0;
				while (array != null && num2 < array.Length)
				{
					if (custom_inputs.Instance.GetButtonDown(array[num2]))
					{
						return num2;
					}
					num2++;
				}
			}
		}
		return -1;
	}

	public int GetNextAtkValue()
	{
		Gun componentInChildren = GetComponentInChildren<Gun>();
		if (null != componentInChildren)
		{
			return componentInChildren.GetNextAtkValue();
		}
		return 0;
	}

	public float GetNextShockValue()
	{
		Gun componentInChildren = GetComponentInChildren<Gun>();
		if (null != componentInChildren)
		{
			return componentInChildren.GetNextShockValue();
		}
		return 0f;
	}

	public int GetNextChargeValue()
	{
		Gun componentInChildren = GetComponentInChildren<Gun>();
		if (null != componentInChildren)
		{
			return componentInChildren.GetNextChargeValue();
		}
		return 0;
	}

	public void ExpandAmmo()
	{
		GetComponent<EquipCoordinator>().ExpandAmmo();
	}

	public void AutoReload()
	{
		if ((!bipAnimation.IsPlaying("reload_h") || !bipAnimation.IsPlaying("2_reload_h")) && !base.animation.IsPlaying("Reload"))
		{
			Gun componentInChildren = GetComponentInChildren<Gun>();
			Weapon componentInChildren2 = GetComponentInChildren<Weapon>();
			if (null != componentInChildren && componentInChildren.CanReload())
			{
				_ReloadWeapon(componentInChildren2.reloadSpeed, (int)componentInChildren2.slot);
			}
		}
	}

	private bool GetFlyButton()
	{
		bool result = custom_inputs.Instance.GetButtonDown("K_ACTION");
		if (custom_inputs.Instance.GetButtonDown("K_JUMP"))
		{
			if (deltaKeyJump < 0.3f)
			{
				result = true;
			}
			else
			{
				deltaKeyJump = 0f;
			}
		}
		deltaKeyJump += Time.deltaTime;
		return result;
	}

	private void CheckAction()
	{
		if (CanControl())
		{
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR && custom_inputs.Instance.GetButtonDown("K_ACTION"))
			{
				if (isBoost)
				{
					isBoost = false;
					Object.DestroyImmediate(boostEff);
					boostEff = null;
				}
				if (isTrampoline)
				{
					isTrampoline = false;
					Object.DestroyImmediate(trampolineEff);
					trampolineEff = null;
				}
				MyInfoManager.Instance.ToggleFlyModeByJetPack();
			}
			if (custom_inputs.Instance.GetButtonDown("K_RELOAD") && (!bipAnimation.IsPlaying("reload_h") || !bipAnimation.IsPlaying("2_reload_h")) && !base.animation.IsPlaying("Reload"))
			{
				Gun componentInChildren = GetComponentInChildren<Gun>();
				Weapon componentInChildren2 = GetComponentInChildren<Weapon>();
				if (null != componentInChildren && componentInChildren.CanReload())
				{
					_ReloadWeapon(componentInChildren2.reloadSpeed, (int)componentInChildren2.slot);
				}
			}
			EquipCoordinator component = GetComponent<EquipCoordinator>();
			if (component != null)
			{
				int num = CheckShortcut();
				bool flag = true;
				bool flag2 = false;
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
				{
					flag = false;
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
				{
					flag = false;
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ESCAPE && RoomManager.Instance.IsEscapeNotAttack())
				{
					flag = false;
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && bndMatch.IsBuildPhase)
				{
					flag = false;
				}
				flag2 = flag;
				if (num >= 0)
				{
					flag = false;
				}
				if (flag)
				{
					num = GlobalVars.Instance.GetWheelKey(Input.GetAxis("Mouse ScrollWheel"));
				}
				if (num >= 0 && num < FORCE_PREV)
				{
					if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && num >= 0 && num < 4)
					{
						bndMatch.IsBuilderMode = false;
					}
					GlobalVars.Instance.SetWheelKey(num);
					Weapon componentInChildren3 = GetComponentInChildren<Weapon>();
					float drawSpeed = (!(componentInChildren3 == null)) ? componentInChildren3.drawSpeed : emptyHandDrawSpeed;
					if (component.SetCurrent(ShortCutToSlot(num)))
					{
						_DrawWeapon(drawSpeed, component.CurrentWeapon, notify: true);
					}
				}
				if ((flag2 && custom_inputs.Instance.GetButtonDown("K_PREV")) || num == FORCE_PREV)
				{
					Weapon componentInChildren4 = GetComponentInChildren<Weapon>();
					float drawSpeed2 = (!(componentInChildren4 == null)) ? componentInChildren4.drawSpeed : emptyHandDrawSpeed;
					if (component.SetPrev())
					{
						_DrawWeapon(drawSpeed2, component.CurrentWeapon, notify: true);
					}
				}
			}
		}
	}

	public void SwitchWeapon()
	{
		EquipCoordinator component = GetComponent<EquipCoordinator>();
		if (component != null)
		{
			Weapon componentInChildren = GetComponentInChildren<Weapon>();
			float drawSpeed = (!(componentInChildren == null)) ? componentInChildren.drawSpeed : emptyHandDrawSpeed;
			_DrawWeapon(drawSpeed, component.CurrentWeapon, notify: true);
		}
	}

	public void PickupFromTemplate(string weaponCode)
	{
		if (weaponCode == "s09")
		{
			int[] usables = new int[1]
			{
				4
			};
			GameObject gameObject = GameObject.Find("Me");
			if (null == gameObject)
			{
				Debug.LogError("Fail to find Me");
			}
			else
			{
				EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
				if (null == component)
				{
					Debug.LogError("Fail to get EquipCoordinator component for Me");
				}
				else
				{
					component.catchBuildGun(usables);
				}
			}
			MyInfoManager.Instance.IsEditor = true;
		}
		else
		{
			EquipCoordinator component2 = GetComponent<EquipCoordinator>();
			if (null != component2 && component2.PickupFromTemplate(weaponCode))
			{
				_DrawWeapon(emptyHandDrawSpeed, component2.CurrentWeapon, notify: true);
			}
		}
	}

	private void _ReloadWeapon(float reloadSpeed, int slot)
	{
		P2PManager.Instance.SendPEER_RELOAD(MyInfoManager.Instance.Seq, slot);
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.Stop("reload_h");
		}
		else
		{
			bipAnimation.Stop("2_reload_h");
		}
		base.animation.Stop("Reload");
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation["reload_h"].normalizedSpeed = reloadSpeed;
		}
		else
		{
			bipAnimation["2_reload_h"].normalizedSpeed = reloadSpeed;
		}
		base.animation["Reload"].normalizedSpeed = reloadSpeed;
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.CrossFade("reload_h");
		}
		else
		{
			bipAnimation.CrossFade("2_reload_h");
		}
		base.animation.Blend("Reload");
	}

	private void _CancelOngoingWeapongAction()
	{
		if (base.animation.IsPlaying("Reload"))
		{
			base.animation.Stop("Reload");
		}
	}

	private void _DrawWeapon(float drawSpeed, int currentWeapon, bool notify)
	{
		if (notify)
		{
			P2PManager.Instance.SendPEER_SWAP_WEAPON(MyInfoManager.Instance.Seq, currentWeapon);
		}
		_CancelOngoingWeapongAction();
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.Stop("reload_h");
		}
		else
		{
			bipAnimation.Stop("2_reload_h");
		}
		base.animation.Stop("SwitchWeapon");
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation["reload_h"].normalizedSpeed = drawSpeed;
		}
		else
		{
			bipAnimation["2_reload_h"].normalizedSpeed = drawSpeed;
		}
		base.animation["SwitchWeapon"].normalizedSpeed = drawSpeed;
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.CrossFade("reload_h");
		}
		else
		{
			bipAnimation.CrossFade("2_reload_h");
		}
		base.animation.Blend("SwitchWeapon");
		if (controlContext == CONTROL_CONTEXT.RUN)
		{
			ToRun();
		}
		else
		{
			ToIdle();
		}
	}

	private int ShortCutToSlot(int shortCut)
	{
		switch (shortCut)
		{
		case 0:
			return 2;
		case 1:
			return 1;
		case 2:
			return 0;
		case 3:
			return 3;
		case 4:
			return 4;
		default:
			return -1;
		}
	}

	private void DrawActionGauge(float cur, float max)
	{
		float num = cur / max;
		Rect position = new Rect((float)((Screen.width - 448) / 2), (float)(Screen.height / 2 + 63), 448f, 34f);
		Rect position2 = new Rect((float)((Screen.width - 412) / 2), (float)(Screen.height / 2 + 70), 412f * num, 20f);
		TextureUtil.DrawTexture(position, actionBg, ScaleMode.StretchToFill, alphaBlend: true);
		TextureUtil.DrawTexture(position2, actionGauge, ScaleMode.StretchToFill, alphaBlend: true);
	}

	private void DrawHp()
	{
		TextureUtil.DrawTexture(new Rect(1f, (float)(Screen.height - hpBg.height - 1), (float)hpBg.width, (float)hpBg.height), hpBg);
		if (BuildOption.Instance.Props.useArmor)
		{
			armorFont.alignment = TextAnchor.LowerRight;
			armorFont.Print(new Vector2(112f, (float)(Screen.height - 47)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, armor));
		}
		hpFont.alignment = TextAnchor.LowerRight;
		hpFont.Print(new Vector2(112f, (float)(Screen.height - 12)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint));
	}

	private void OnGUI_tutorTargetDirection()
	{
		if (GlobalVars.Instance.showBrickId >= 0)
		{
			GameObject brickObject = BrickManager.Instance.GetBrickObject(GlobalVars.Instance.showBrickId);
			if (!(brickObject == null))
			{
				Camera camera = null;
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null == gameObject)
				{
					Debug.LogError("Fail to find mainCamera for tutor arrow");
				}
				else
				{
					camera = gameObject.GetComponent<Camera>();
					if (null == camera)
					{
						Debug.LogError("Fail to get mainCam for tutor arrow");
					}
				}
				Vector3 position = camera.transform.position;
				float num = Vector3.Distance(position, brickObject.transform.position);
				if (!(num < 3f))
				{
					if (num < 6f)
					{
						Vector3 position2 = brickObject.transform.position;
						position2.y += 1.2f;
						Vector3 vector = camera.WorldToViewportPoint(position2);
						if (vector.z > 0f && 0f < vector.x && vector.x < 1f && 0f < vector.y && vector.y < 1f)
						{
							Vector3 vector2 = camera.WorldToScreenPoint(position2);
							Rect position3 = new Rect(vector2.x - (float)(texDirection2.width / 2), (float)Screen.height - vector2.y - (float)(texDirection2.height / 2), (float)texDirection2.width, (float)texDirection2.height);
							Matrix4x4 matrix = GUI.matrix;
							GUIUtility.RotateAroundPivot(0f, new Vector2(vector2.x, (float)Screen.height - vector2.y));
							TextureUtil.DrawTexture(position3, texDirection2);
							GUI.matrix = matrix;
						}
					}
					else
					{
						Vector3 from = camera.transform.TransformDirection(Vector3.forward);
						from.y = 0f;
						from = from.normalized;
						Vector3 from2 = camera.transform.TransformDirection(Vector3.right);
						from2.y = 0f;
						from2 = from2.normalized;
						Vector3 from3 = camera.transform.TransformDirection(Vector3.left);
						from3.y = 0f;
						from3 = from3.normalized;
						Vector3 to = brickObject.transform.position - position;
						to.y = 0f;
						to = to.normalized;
						float num2 = Vector3.Angle(from, to);
						Vector2 pivotPoint = new Vector2((float)(Screen.width / 2), 180f);
						if (num2 > 135f)
						{
							if (!isBackDir)
							{
								drawBackDir = true;
								isBackDir = true;
								ElapsedBlinkBack = 0f;
							}
							float num3 = (float)texDirectionBack.width;
							float num4 = (float)texDirectionBack.height;
							Rect position4 = new Rect(pivotPoint.x - num3 / 2f, pivotPoint.y - num4 / 2f, num3, num4);
							Matrix4x4 matrix2 = GUI.matrix;
							GUIUtility.RotateAroundPivot(0f, pivotPoint);
							if (drawBackDir)
							{
								TextureUtil.DrawTexture(position4, texDirectionBack);
							}
							GUI.matrix = matrix2;
							GUIStyle style = GUI.skin.GetStyle("MissionLabel");
							style.fontStyle = FontStyle.Bold;
							LabelUtil.TextOut(new Vector2(pivotPoint.x, position4.y + num4 + 15f), num.ToString("0.##"), "MissionLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
							style.fontStyle = FontStyle.Normal;
							ElapsedWarning += Time.deltaTime;
							if (ElapsedWarning > 2f)
							{
								AudioSource audioSource = GlobalVars.Instance.GetAudioSource();
								if (audioSource != null && !audioSource.isPlaying)
								{
									audioSource.PlayOneShot(warning);
									ElapsedWarning = 0f;
								}
							}
						}
						else
						{
							if (isBackDir)
							{
								isBackDir = false;
							}
							if (Vector3.Angle(from2, to) >= Vector3.Angle(from3, to))
							{
								num2 = 180f + (180f - num2);
							}
							float num5 = (float)texDirection.width;
							float num6 = (float)texDirection.height;
							Rect position5 = new Rect(pivotPoint.x - num5 / 2f, pivotPoint.y - num6 / 2f, num5, num6);
							Matrix4x4 matrix3 = GUI.matrix;
							GUIUtility.RotateAroundPivot(num2, pivotPoint);
							TextureUtil.DrawTexture(position5, texDirection);
							GUI.matrix = matrix3;
							GUIStyle style2 = GUI.skin.GetStyle("MissionLabel");
							style2.fontStyle = FontStyle.Bold;
							LabelUtil.TextOut(new Vector2(pivotPoint.x, position5.y + num6 + 15f), num.ToString("0.##"), "MissionLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
							style2.fontStyle = FontStyle.Normal;
						}
					}
				}
			}
		}
	}

	private void OutputCannonGuide()
	{
		if (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase) && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ZOMBIE || !(null != zombieMatch) || zombieMatch.IsPlaying) && CheckCannonTutor() != null)
		{
			outputGuideOnce = true;
			int num = custom_inputs.Instance.KeyIndex("K_ACTION");
			string text = string.Format(StringMgr.Instance.Get("STRING_VALCAN_BOARDING"), custom_inputs.Instance.InputKey[num].ToString());
			Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
			vector.x += 60f;
			GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
		}
	}

	private void OutputTrainGuide()
	{
		if (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase) && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ZOMBIE || !(null != zombieMatch) || zombieMatch.IsPlaying) && controlContext != CONTROL_CONTEXT.TRAIN_CTL && controlContext != CONTROL_CONTEXT.TRAIN_RUN && CheckTrain() != null && !outputGuideOnce)
		{
			int num = custom_inputs.Instance.KeyIndex("K_ACTION");
			string text = string.Format(StringMgr.Instance.Get("STRING_TRAIN_BOARDING"), custom_inputs.Instance.InputKey[num].ToString());
			Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
			vector.x += 60f;
			GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
		}
	}

	private string CheckExplBoom()
	{
		string result = string.Empty;
		BombFuction componentInChildren = GetComponentInChildren<BombFuction>();
		RaycastHit hit;
		if (null != componentInChildren)
		{
			if (componentInChildren.VerifyCameraAll() && componentInChildren.GetInstallTarget(out hit))
			{
				int num = custom_inputs.Instance.KeyIndex("K_ACTION");
				result = string.Format(StringMgr.Instance.Get("DEFUSION_MODE_MESSAGE_01"), custom_inputs.Instance.InputKey[num].ToString());
			}
		}
		else if (GetUninstallTarget(out hit))
		{
			int num2 = custom_inputs.Instance.KeyIndex("K_ACTION");
			result = string.Format(StringMgr.Instance.Get("DEFUSION_MODE_MESSAGE_02"), custom_inputs.Instance.InputKey[num2].ToString());
		}
		return result;
	}

	private void OutputExplBoomGuide()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
		{
			string text = CheckExplBoom();
			if (text != null && text.Length > 0)
			{
				Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
				GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
			}
		}
	}

	private void OutputDoorOpenGuide()
	{
		if (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase) && GetTargetDoor() >= 0)
		{
			int num = custom_inputs.Instance.KeyIndex("K_ACTION");
			string text = string.Format(StringMgr.Instance.Get("OPEN_DOOR"), custom_inputs.Instance.InputKey[num].ToString());
			Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
			vector.x += 60f;
			GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
		}
	}

	private void DrawFeverGauge()
	{
		if (GlobalVars.Instance.IsFeverMode())
		{
			float feverPercent = GlobalVars.Instance.getFeverPercent();
			Vector2 zero = Vector2.zero;
			zero.x = 20f;
			zero.y = (float)Screen.height - 490f;
			Rect position = new Rect(zero.x, zero.y, 68f, 248f);
			Rect position2 = new Rect(zero.x + 12f, zero.y + 28f, 44f, 198f);
			Rect position3 = new Rect(zero.x - 5f, zero.y - 41f, 112f, 36f);
			Rect position4 = new Rect(12f, 158f, 88f, 88f);
			Rect position5 = new Rect(position2.x, position2.y + 198f, position2.width, (0f - position2.height) * feverPercent);
			Rect srcRect = new Rect(0f, 0f, 1f, feverPercent);
			TextureUtil.DrawTexture(position2, texFeverGaugeBg, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(position5, texFeverGauge, srcRect);
			TextureUtil.DrawTexture(position, texFeverGaugeDeco, ScaleMode.StretchToFill);
			if (blinkFever)
			{
				TextureUtil.DrawTexture(position3, texFeverText[FeverTxtBack ? 1 : 0], ScaleMode.StretchToFill);
				if (GlobalVars.Instance.StateFever == 0)
				{
					TextureUtil.DrawTexture(position4, texFeverQ[FeverTxtBack ? 1 : 0], ScaleMode.StretchToFill);
					LabelUtil.ToBold("SysMsgLabel");
					int num = custom_inputs.Instance.KeyIndex("K_FEVER");
					LabelUtil.TextOut(new Vector2(position4.x + 36f, position4.y + 36f), custom_inputs.Instance.InputKey[num].ToString(), "SysMsgLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					LabelUtil.ToNormal("SysMsgLabel");
				}
			}
			else
			{
				TextureUtil.DrawTexture(position3, texFeverTextNml, ScaleMode.StretchToFill);
			}
			if (actingFever && !FeverTxtBack)
			{
				TextureUtil.DrawTexture(position, texFeverGaugeGlow, ScaleMode.StretchToFill);
			}
		}
	}

	private void DrawDashGauge()
	{
		if (BuildOption.Instance.Props.useDefaultDash && RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase))
		{
			float num = (float)stamina / (staminaMax + staminaMaxAdd);
			Vector2 zero = Vector2.zero;
			zero.x = (float)((Screen.width - 348) / 2);
			zero.y = (float)(Screen.height - 29);
			Rect position = new Rect(zero.x, zero.y, 348f, 24f);
			Rect position2 = new Rect(zero.x + 27f, zero.y + 5f, 316f * num, 14f);
			TextureUtil.DrawTexture(position, dashBg, ScaleMode.StretchToFill);
			Color color = GUI.color;
			if ((float)stamina == staminaMax + staminaMaxAdd)
			{
				GUI.color = GlobalVars.Instance.GetByteColor2FloatColor(250, 190, 27);
			}
			else if (dashState)
			{
				GUI.color = GlobalVars.Instance.GetByteColor2FloatColor(250, 81, 34);
			}
			else
			{
				GUI.color = Color.Lerp(GlobalVars.Instance.GetByteColor2FloatColor(byte.MaxValue, 0, 0), GlobalVars.Instance.GetByteColor2FloatColor(176, 221, 55), num);
			}
			TextureUtil.DrawTexture(position2, dashGauge, ScaleMode.StretchToFill);
			GUI.color = color;
		}
	}

	private void DrawRecognition()
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || !(null != bndMatch) || !bndMatch.IsBuildPhase) && GlobalVars.Instance.recogniteType >= 0)
		{
			float num = 0f;
			float a = 1f;
			if (dtRecognition >= 2f)
			{
				float num2 = dtRecognition + 1f - dtRecognitionMax;
				a = dtRecognitionMax - dtRecognition;
				num = num2 * 20f;
			}
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, a);
			Vector2 vector = new Vector2(578f, 363f);
			Rect position = new Rect(vector.x, vector.y - num, 62f, 44f);
			Vector2 pos = new Vector2(vector.x + 64f, vector.y + 30f - num);
			if (GlobalVars.Instance.recogniteType == 0)
			{
				TextureUtil.DrawTexture(position, uizTob, ScaleMode.StretchToFill);
			}
			else if (GlobalVars.Instance.recogniteType == 1)
			{
				TextureUtil.DrawTexture(position, uibToz, ScaleMode.StretchToFill);
			}
			imgFont.Print(pos, GlobalVars.Instance.recogniVal);
			GUI.color = color;
		}
	}

	private void OnGUI()
	{
		if (!MyInfoManager.Instance.isGuiOn)
		{
			if (custom_inputs.Instance.GetButtonDown("K_FIRE2"))
			{
				MyInfoManager.Instance.isGuiOn = true;
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null == gameObject)
				{
					Debug.LogError("Fail to find mainCamera for radar");
				}
				else
				{
					CameraController component = gameObject.GetComponent<CameraController>();
					if (null == component)
					{
						Debug.LogError("Fail to get CameraController for radar");
					}
					component.EnableFpCam(enable: true);
				}
			}
		}
		else
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawHp();
			DrawBanish();
			if (iAmLucky)
			{
				Color color = GUI.color;
				GUI.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), luckyTime / 2f);
				TextureUtil.DrawTexture(new Rect(10f, (float)(Screen.height - hpBg.height - luckyImage.height), (float)luckyImage.width, (float)luckyImage.height), luckyImage);
				GUI.color = color;
			}
			if (GravityValue != 0f)
			{
				TextureUtil.DrawTexture(new Rect(20f, 210f, 140f, 26f), gravityIcon);
				string text = StringMgr.Instance.Get("GRAVITY") + " : " + (GravityValue * 10f).ToString();
				LabelUtil.TextOut(new Vector2(50f, 225f), text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			}
			switch (controlContext)
			{
			case CONTROL_CONTEXT.CLIMB:
			case CONTROL_CONTEXT.HANG:
				ladderTime += Time.deltaTime;
				if (ladderPhase && ladderTime > 1f)
				{
					ladderPhase = false;
					ladderTime = 0f;
				}
				if (!ladderPhase && ladderTime > 0.5f)
				{
					ladderPhase = true;
					ladderTime = 0f;
				}
				if (ladder != null && ladderPhase)
				{
					TextureUtil.DrawTexture(new Rect((float)((Screen.width - ladder.width) / 2), (float)(Screen.height / 2 - ladder.height - 64), (float)ladder.width, (float)ladder.height), ladder);
				}
				break;
			case CONTROL_CONTEXT.CANNON_TRY:
				if (null != cannon)
				{
					DrawActionGauge(cannonTryingTime, cannon.tryingTime);
				}
				break;
			case CONTROL_CONTEXT.TRAIN_TRY:
				if (trainCtrl != null)
				{
					DrawActionGauge(cannonTryingTime, trainTryingTimeMax);
				}
				break;
			}
			if (uninstalling)
			{
				DrawActionGauge(uninstallDelta, uninstallTime);
			}
			if (openning)
			{
				DrawActionGauge(openningDelta, openningTime);
			}
			if (statusMessage.Length > 0)
			{
				float a = 1f;
				float num = (statusDelta - (statusMessageLimit - 2f)) / 2f;
				if (num > 0f)
				{
					a = Mathf.Lerp(1f, 0f, num);
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), 182f), statusMessage, "BigLabel", new Color(0f, 0f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), 180f), statusMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			outputGuideOnce = false;
			DrawWeaponChangeGuide();
			OutputCannonGuide();
			OutputTrainGuide();
			OutputExplBoomGuide();
			OutputDoorOpenGuide();
			OutputBungeeMoveGuide();
			OutputFeverActiveGuide();
			OutputPutWeaponGuide();
			DrawDashGauge();
			DrawFeverGauge();
			DrawRecognition();
			if (!overHighGrass)
			{
				blickBearTrap.Draw((float)(Screen.width / 2), (float)(Screen.height - 10) - shooterToolHeight(), uiBearTrap);
				blickBearTrap.DrawReaminText((float)(Screen.width / 2 + 15), (float)(Screen.height + 2) - shooterToolHeight(), uiBearTrap, 2f - deltaBeartrap);
			}
			else if (eBearTrap == EBEARTRAP.OVER)
			{
				blickHighGrass.Draw((float)(Screen.width / 2 - 28), (float)(Screen.height - 10) - shooterToolHeight(), uiHighGrass);
				blickBearTrap.Draw((float)(Screen.width / 2 + 28), (float)(Screen.height - 10) - shooterToolHeight(), uiBearTrap);
				blickBearTrap.DrawReaminText((float)(Screen.width / 2 + 43), (float)(Screen.height + 2) - shooterToolHeight(), uiBearTrap, 2f - deltaBeartrap);
			}
			else
			{
				blickHighGrass.Draw((float)(Screen.width / 2), (float)(Screen.height - 10) - shooterToolHeight(), uiHighGrass);
			}
			if (Application.loadedLevelName.Contains("Tutor"))
			{
				OnGUI_tutorTargetDirection();
			}
			GlobalVars.Instance.streamming();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private float shooterToolHeight()
	{
		GameObject gameObject = GameObject.Find("Main");
		ShooterTools component = gameObject.GetComponent<ShooterTools>();
		if (component != null)
		{
			return component.DrawLLevelHeight();
		}
		return 0f;
	}

	private void DrawWeaponChangeGuide()
	{
		bool flag = ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq);
		if (weaponChange && !flag)
		{
			string text = string.Format(StringMgr.Instance.Get("WEAPON_CHANGE_POSSIBLE"), custom_inputs.Instance.GetKeyCodeName("K_WPNCHG"));
			string text2 = string.Format(StringMgr.Instance.Get("REMAIN_TIME"), Mathf.CeilToInt(weaponChangeTime - weaponChangeTimer));
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 156)), text, "BigLabel", Color.white, Color.black, TextAnchor.UpperCenter);
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 190)), text2, "Label", Color.white, Color.black, TextAnchor.UpperCenter);
		}
	}

	public void ResetGravity()
	{
		gravity = GlobalVars.Instance.NativeGravity;
		jumpHeight = GlobalVars.Instance.NativeJumpHeight;
		GravityValue = (float)BrickManager.Instance.GravityValue;
		if (GravityValue < -10f)
		{
			GravityValue = -10f;
		}
		if (GravityValue > 10f)
		{
			GravityValue = 10f;
		}
		if (RoomManager.Instance.CurrentRoomType != 0)
		{
			if (GravityValue < 0f)
			{
				float num = GlobalVars.Instance.NativeGravity * 0.1f;
				gravity = GlobalVars.Instance.NativeGravity + num * GravityValue;
				if ((float)gravity < 0.5f)
				{
					gravity = 0.5f;
				}
				float num2 = GlobalVars.Instance.NativeJumpHeightMax / 10f;
				jumpHeight = GlobalVars.Instance.NativeJumpHeight + num2 * Mathf.Abs(GravityValue);
			}
			if (GravityValue > 0f)
			{
				float num3 = GlobalVars.Instance.NativeGravity * 0.1f;
				gravity = GlobalVars.Instance.NativeGravity + num3 * GravityValue;
				if ((float)gravity < 0.5f)
				{
					gravity = 0.5f;
				}
				float num4 = GlobalVars.Instance.NativeJumpHeightMin / 10f;
				jumpHeight = GlobalVars.Instance.NativeJumpHeight - num4 * Mathf.Abs(GravityValue);
			}
			deltaGravity = (float)gravity * 0.1f;
			deltaJumpHeight = (float)jumpHeight * 0.1f;
			savedJumpHeight = jumpHeight;
			changedHeight = (float)jumpHeight - GlobalVars.Instance.NativeJumpHeight;
		}
	}

	public void Spawn(Vector3 position, Quaternion rotation)
	{
		MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAY_MODE;
		dicDoor.Clear();
		gravity = GlobalVars.Instance.NativeGravity;
		jumpHeight = GlobalVars.Instance.NativeJumpHeight;
		GravityValue = (float)BrickManager.Instance.GravityValue;
		if (GravityValue < -10f)
		{
			GravityValue = -10f;
		}
		if (GravityValue > 10f)
		{
			GravityValue = 10f;
		}
		if (RoomManager.Instance.CurrentRoomType != 0)
		{
			if (GravityValue < 0f)
			{
				float num = GlobalVars.Instance.NativeGravity * 0.1f;
				gravity = GlobalVars.Instance.NativeGravity + num * GravityValue;
				if ((float)gravity < 0.5f)
				{
					gravity = 0.5f;
				}
				float num2 = GlobalVars.Instance.NativeJumpHeightMax / 10f;
				jumpHeight = GlobalVars.Instance.NativeJumpHeight + num2 * Mathf.Abs(GravityValue);
			}
			if (GravityValue > 0f)
			{
				float num3 = GlobalVars.Instance.NativeGravity * 0.1f;
				gravity = GlobalVars.Instance.NativeGravity + num3 * GravityValue;
				if ((float)gravity < 0.5f)
				{
					gravity = 0.5f;
				}
				float num4 = GlobalVars.Instance.NativeJumpHeightMin / 10f;
				jumpHeight = GlobalVars.Instance.NativeJumpHeight - num4 * Mathf.Abs(GravityValue);
			}
			deltaGravity = (float)gravity * 0.1f;
			deltaJumpHeight = (float)jumpHeight * 0.1f;
			savedJumpHeight = jumpHeight;
			changedHeight = (float)jumpHeight - GlobalVars.Instance.NativeJumpHeight;
		}
		else
		{
			savedJumpHeight = jumpHeight;
			changedHeight = 0f;
		}
		Respawn(position, rotation);
	}

	public void ResetPlayingSpectator()
	{
		if (MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR)
		{
			MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.NONE;
			GetComponent<SpectatorSwitch>().ModeChangeBruteforcely(MyInfoManager.Instance.ControlMode);
		}
	}

	public bool Heal(float ratio)
	{
		GlobalVars.Instance.IsRecognition = true;
		GlobalVars.Instance.recogniteType = 0;
		float num = ratio * (float)GetMaxHp();
		float num2 = num;
		if (num2 > 0f)
		{
			GlobalVars.Instance.recogniVal = (int)num2;
		}
		return Heal(GlobalVars.Instance.recogniVal);
	}

	public bool Heal(int inc)
	{
		if (inc <= 0)
		{
			return false;
		}
		if (hitPoint <= 0 || NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint) >= GetMaxHp())
		{
			return false;
		}
		SetHitPoint(Mathf.Min(hitPoint + NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, inc), NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp())), autoHealPossible: false);
		return true;
	}

	private void RespawnSecure()
	{
		hitPointSecure.Reset();
		armorSecure.Reset();
		deltaFromDeathSecure.Reset();
	}

	public void CheckRespawnItemHave()
	{
		respwanTimeDec = MyInfoManager.Instance.HaveFunctionTotalFactor("respwan_time_dec");
	}

	public bool CheckJustRespawnItemHave()
	{
		long num = MyInfoManager.Instance.HaveFunction("just_respawn");
		if (num >= 0)
		{
			return true;
		}
		return false;
	}

	private void CheckDashItemHave()
	{
		dashTimeInc = MyInfoManager.Instance.HaveFunctionTotalFactor("dash_time_inc");
		staminaMaxAdd = staminaMax * dashTimeInc;
	}

	private void CheckFallenDamageReduceItemHave()
	{
		fallenDamageReduce = MyInfoManager.Instance.HaveFunctionTotalFactor("fallen_damage_reduce");
	}

	private void CheckWeaponBack()
	{
		if (!Application.loadedLevelName.Contains("BattleTutor") && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND && RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ESCAPE || BuildOption.Instance.Props.UseEscapeAttack))
		{
			int[] array = null;
			if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.EXPLOSION)
			{
				switch (RoomManager.Instance.WeaponOption)
				{
				case 2:
					array = new int[1];
					break;
				case 1:
					array = new int[2]
					{
						0,
						1
					};
					break;
				default:
					array = new int[3]
					{
						0,
						2,
						1
					};
					break;
				}
			}
			else
			{
				switch (RoomManager.Instance.WeaponOption)
				{
				case 2:
					array = new int[2]
					{
						0,
						4
					};
					break;
				case 1:
					array = new int[3]
					{
						0,
						1,
						4
					};
					break;
				default:
					array = new int[4]
					{
						0,
						2,
						1,
						4
					};
					break;
				}
			}
			GetComponent<EquipCoordinator>().WeaponBack(array);
		}
	}

	public int GetMaxHp()
	{
		int num = 100;
		int num2 = num;
		num2 += (int)((float)num * zombieVirus.MaxHpFactor);
		return num2 + WantedManager.Instance.GetWantedHpMaxBoost(MyInfoManager.Instance.Seq, num);
	}

	public void Respawn(Vector3 position, Quaternion rotation)
	{
		armor = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.ARMOR, MyInfoManager.Instance.SumArmor());
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.ARMOR, armor);
		autoHealCount = 0;
		ResetPlayingSpectator();
		MyInfoManager.Instance.TurnoffFlyMode();
		CheckDashItemHave();
		CheckFallenDamageReduceItemHave();
		SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, GetMaxHp()), autoHealPossible: false);
		stamina = staminaMax + staminaMaxAdd;
		ResetWeaponChange();
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		TranceformPosition = position;
		respawnPosition = position;
		inAirTime = 0f;
		lastGroundedPosition = position;
		invincibleTimer = 0f;
		invincible = true;
		dtPre = 0f;
		isDamaged = false;
		activateBlackhole = false;
		activateBlackholeUser = -1;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			bungeeRespawn = true;
		}
		if (RoomManager.Instance.DropItem)
		{
			CheckWeaponBack();
		}
		P2PManager.Instance.SendPEER_RESPAWN();
		if (dicDamageLog != null)
		{
			dicDamageLog.Clear();
		}
		if (dicInflictedDamage != null)
		{
			dicInflictedDamage.Clear();
		}
		BroadcastMessage("OnRespawn", rotation);
		initIdle = false;
	}

	private void ResetWeaponChange()
	{
		weaponChangeTimer = 0f;
		weaponChange = (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ESCAPE);
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND && null != bndMatch && bndMatch.IsBuildPhase)
		{
			weaponChange = false;
		}
		if (Application.loadedLevelName.Contains("Tutor"))
		{
			weaponChange = false;
		}
	}

	private void CheckInvincible()
	{
		if (invincible)
		{
			invincibleTimer += Time.deltaTime;
			if (invincibleTimer > invincibleTime)
			{
				invincible = false;
			}
		}
	}

	public void MakeWeaponChangeDisable()
	{
		weaponChange = false;
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.WEAPON_CHANGE))
		{
			_CloseWeaponChangeDialog();
		}
	}

	private void _CloseWeaponChangeDialog()
	{
		WeaponChangeDialog weaponChangeDialog = (WeaponChangeDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.WEAPON_CHANGE);
		if (weaponChangeDialog != null)
		{
			weaponChangeDialog.Done = true;
		}
	}

	private bool _CheckWeaponChangeRadius()
	{
		if (IsDead || Vector3.Distance(respawnPosition, TranceformPosition) <= weaponChangeRadius)
		{
			return true;
		}
		_CloseWeaponChangeDialog();
		return false;
	}

	private void CheckWeaponChange()
	{
		if (weaponChange)
		{
			bool flag = _CheckWeaponChangeRadius();
			if (!ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq) && custom_inputs.Instance.GetButtonDown("K_WPNCHG") && Screen.lockCursor)
			{
				if (!flag)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("OUT_OF_WEAPON_CHANGE_RADIUS"));
				}
				else
				{
					((WeaponChangeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.WEAPON_CHANGE, exclusive: true))?.InitDialog();
				}
			}
			weaponChangeTimer += Time.deltaTime;
			if (weaponChangeTimer > weaponChangeTime)
			{
				MakeWeaponChangeDisable();
			}
		}
	}

	private void ApplyRigidity()
	{
		rigidity = Mathf.Lerp(rigidity, 0f, Time.deltaTime);
	}

	private void UpdateLucky()
	{
		if (iAmLucky)
		{
			luckyTime += Time.deltaTime;
			if (luckyTime >= 2f)
			{
				iAmLucky = false;
				luckyTime = 0f;
			}
		}
	}

	private bool VerifyCharacterController()
	{
		if (null == cc)
		{
			cc = GetComponent<CharacterController>();
		}
		return null != cc;
	}

	private void UpdateSpeed()
	{
		speedTime += Time.deltaTime;
		SpeedUpMyEffect();
	}

	public void CancelSpeedUp()
	{
		speedTime = speedMax;
		dashState = false;
		SpeedUpMyEffect();
	}

	public bool SpeedUp()
	{
		if (speedTime < speedMax)
		{
			return false;
		}
		speedTime = 0f;
		SpeedUpMyEffect();
		return true;
	}

	public bool IsSpeedUpState()
	{
		return speedTime < speedMax && moveSpeed != 0f;
	}

	private void SpeedUpMyEffect()
	{
		bool flag = IsSpeedUpState() || dashState || GlobalVars.Instance.StateFever > 0;
		if (flag != speedUpEffect)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (gameObject != null)
			{
				speedUpEffect = flag;
				if (flag)
				{
					gameObject.GetComponent<CameraController>().SetSpeedUpFov(isSpeedUp: true);
					gameObject.transform.FindChild("FX_Speed_spark").gameObject.SetActive(value: true);
					GetComponent<MoveScreenFx>().ShowMoveScreenFx(isVisible: true);
					P2PManager.Instance.SendPEER_CONSUME(MyInfoManager.Instance.Seq, "speedup");
				}
				else
				{
					gameObject.GetComponent<CameraController>().SetSpeedUpFov(isSpeedUp: false);
					gameObject.transform.FindChild("FX_Speed_spark").gameObject.SetActive(value: false);
					GetComponent<MoveScreenFx>().ShowMoveScreenFx(isVisible: true);
					P2PManager.Instance.SendPEER_CONSUME(MyInfoManager.Instance.Seq, "speedup_end");
				}
			}
		}
	}

	private void ReportInflictedDamage()
	{
		deltaTimeInflictedDamage += Time.deltaTime;
		if (deltaTimeInflictedDamage > 5f)
		{
			deltaTimeInflictedDamage = 0f;
			if (dicInflictedDamage != null && dicInflictedDamage.Count > 0)
			{
				CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(dicInflictedDamage);
				dicInflictedDamage.Clear();
			}
		}
	}

	private void collideBricks()
	{
		if (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND || (!bndMatch.IsBuildPhase && !bndMatch.IsBuilderMode)))
		{
			if (isDamaged)
			{
				dtDamageBrick += Time.deltaTime;
				if (!(dtDamageBrick > DamageBrickTimeMax))
				{
					return;
				}
				isDamaged = false;
			}
			int layerMask = (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Edit Layer"));
			float radius = cc.height / 3f;
			Vector3 tranceformPosition = TranceformPosition;
			float x = tranceformPosition.x;
			Vector3 tranceformPosition2 = TranceformPosition;
			float y = tranceformPosition2.y;
			Vector3 tranceformPosition3 = TranceformPosition;
			Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), radius, layerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
				if (null != componentInChildren)
				{
					if (componentInChildren.Index == 133)
					{
						isDamaged = true;
						dtDamageBrick = 0f;
						GetHit(MyInfoManager.Instance.Seq, 10, 0f, -2, -1, autoHealPossible: true, checkZombie: false);
					}
					else if (componentInChildren.Index == 190)
					{
						isDamaged = true;
						dtDamageBrick = 0f;
						GetHit(MyInfoManager.Instance.Seq, trapDamage, 0f, -7, -1, autoHealPossible: true, checkZombie: false);
						BrickManager.Instance.AnimationPlay(componentInChildren.Index, componentInChildren.Seq, "fire");
					}
				}
			}
			radius = cc.height / 4.5f;
			Vector3 tranceformPosition4 = TranceformPosition;
			float x2 = tranceformPosition4.x;
			Vector3 tranceformPosition5 = TranceformPosition;
			float y2 = tranceformPosition5.y;
			Vector3 tranceformPosition6 = TranceformPosition;
			array = Physics.OverlapSphere(new Vector3(x2, y2, tranceformPosition6.z), radius, layerMask);
			bool flag = false;
			Collider[] array3 = array;
			foreach (Collider collider2 in array3)
			{
				BrickProperty componentInChildren2 = collider2.GetComponentInChildren<BrickProperty>();
				if (null != componentInChildren2)
				{
					if (componentInChildren2.Index == 166)
					{
						isDamaged = true;
						dtDamageBrick = 0f;
						GetHit(MyInfoManager.Instance.Seq, trapDamage, 0f, -5, -1, autoHealPossible: true, checkZombie: false);
						BrickManager.Instance.AnimationPlay(componentInChildren2.Index, componentInChildren2.Seq, "fire");
					}
					else if (componentInChildren2.Index == 200)
					{
						flag = true;
					}
					else if (componentInChildren2.Index == 201 && eBearTrap == EBEARTRAP.NONE)
					{
						eBearTrap = EBEARTRAP.OVER;
						BrickManager.Instance.AnimationPlay(componentInChildren2.Index, componentInChildren2.Seq, "fire");
						bearSeq = componentInChildren2.Seq;
						blickBearTrap.IsActive = true;
						blickBearTrap.ViewText = true;
					}
				}
			}
			if (overHighGrass != flag)
			{
				overHighGrass = flag;
				blickHighGrass.IsActive = overHighGrass;
			}
			BrickManager.Instance.CheckDistanceDoorT(TranceformPosition);
		}
	}

	private void collidePoison()
	{
		if (isPosion)
		{
			dtDamageBrick += Time.deltaTime;
			if (!(dtDamageBrick > DamageBrickTimeMax))
			{
				return;
			}
			isPosion = false;
		}
		Vector3 tranceformPosition = TranceformPosition;
		if (tranceformPosition.x > posionSpot.x - poisonRadius)
		{
			Vector3 tranceformPosition2 = TranceformPosition;
			if (tranceformPosition2.x < posionSpot.x + poisonRadius)
			{
				Vector3 tranceformPosition3 = TranceformPosition;
				if (tranceformPosition3.y > posionSpot.y - poisonRadius)
				{
					Vector3 tranceformPosition4 = TranceformPosition;
					if (tranceformPosition4.y < posionSpot.x + poisonRadius)
					{
						Vector3 tranceformPosition5 = TranceformPosition;
						if (tranceformPosition5.z > posionSpot.z - poisonRadius)
						{
							Vector3 tranceformPosition6 = TranceformPosition;
							if (tranceformPosition6.z < posionSpot.x + poisonRadius)
							{
								GetHit(-1, poisonDamage, 0f, -1, -1, autoHealPossible: true, checkZombie: false);
							}
						}
					}
				}
			}
		}
	}

	public void OnTrampoline(int brickSeq)
	{
		AudioSource component = GetComponent<AudioSource>();
		if (!isTrampoline && !(trampolineEff != null))
		{
			P2PManager.Instance.SendPEER_BOOST(MyInfoManager.Instance.Seq, brickSeq, 159);
			jumpHeight = maxTrampolineheight;
			moveSpeed = resultSpeed;
			moveSpeed = Mathf.Min(Mathf.Max(moveSpeed, minJumpSpeed), maxJumpSpeed);
			vertDir = Vector3.up;
			vertSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			isJumping = true;
			controlContext = CONTROL_CONTEXT.JUMP;
			trampolineEff = (Object.Instantiate((Object)GlobalVars.Instance.tramp_hor_eff, TranceformPosition, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			isTrampoline = true;
			dtTrampoline = 0f;
			if (null != component && null != sndPong)
			{
				component.PlayOneShot(sndPong);
			}
			Vector3 tranceformPosition = TranceformPosition;
			previousY = tranceformPosition.y - 1f;
		}
	}

	public void OnBoost(int brickSeq)
	{
		AudioSource component = GetComponent<AudioSource>();
		if (isBoost || boostEff != null)
		{
			isBoost = false;
			Object.DestroyImmediate(boostEff);
			boostEff = null;
		}
		GameObject brickObject = BrickManager.Instance.GetBrickObject(brickSeq);
		float num = Vector3.Dot(moveDir, brickObject.transform.forward);
		if (num >= 0f)
		{
			protectBoost = true;
			dtProtectBoost = 0f;
		}
		else
		{
			moveDir = brickObject.transform.forward;
			P2PManager.Instance.SendPEER_BOOST(MyInfoManager.Instance.Seq, brickSeq, 160);
			Vector3 tranceformPosition = TranceformPosition;
			tranceformPosition.y += 1f;
			boostEff = (Object.Instantiate((Object)GlobalVars.Instance.tramp_vert_eff, tranceformPosition, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			isBoost = true;
			dtBoost = 0f;
			if (null != component && null != sndPong)
			{
				component.PlayOneShot(sndPong);
			}
		}
	}

	private void collideBoosters()
	{
		if (MyInfoManager.Instance.ControlMode != MyInfoManager.CONTROL_MODE.PLAYING_FLY_MODE && controlContext != CONTROL_CONTEXT.TRAIN_CTL && controlContext != CONTROL_CONTEXT.TRAIN_RUN)
		{
			if (protectBoost)
			{
				dtProtectBoost += Time.deltaTime;
				if (dtProtectBoost > 0.5f)
				{
					protectBoost = false;
				}
			}
			else
			{
				int layerMask = 1 << LayerMask.NameToLayer("Edit Layer");
				float radius = cc.height / 2f;
				Vector3 tranceformPosition = TranceformPosition;
				float x = tranceformPosition.x;
				Vector3 tranceformPosition2 = TranceformPosition;
				float y = tranceformPosition2.y;
				Vector3 tranceformPosition3 = TranceformPosition;
				Collider[] array = Physics.OverlapSphere(new Vector3(x, y, tranceformPosition3.z), radius, layerMask);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					BrickProperty componentInChildren = collider.GetComponentInChildren<BrickProperty>();
					if (null != componentInChildren)
					{
						if (componentInChildren.Index == 159)
						{
							OnTrampoline(componentInChildren.Seq);
							BrickManager.Instance.AnimationPlay(componentInChildren.Index, componentInChildren.Seq, "fire");
						}
						else if (componentInChildren.Index == 160)
						{
							OnBoost(componentInChildren.Seq);
							BrickManager.Instance.AnimationPlay(componentInChildren.Index, componentInChildren.Seq, "fire");
						}
					}
				}
			}
		}
	}

	private bool ApplyTrampoline()
	{
		if (!isTrampoline)
		{
			return false;
		}
		dtTrampoline += Time.deltaTime;
		Vector3 tranceformPosition = TranceformPosition;
		if (tranceformPosition.y - previousY > 0f)
		{
			if (trampolineEff != null)
			{
				trampolineEff.transform.position = TranceformPosition;
			}
		}
		else
		{
			Object.DestroyImmediate(trampolineEff);
			trampolineEff = null;
			isTrampoline = false;
		}
		if (dtTrampoline > 3f && isTrampoline)
		{
			Object.DestroyImmediate(trampolineEff);
			trampolineEff = null;
			isTrampoline = false;
			dtTrampoline = 0f;
		}
		Vector3 tranceformPosition2 = TranceformPosition;
		previousY = tranceformPosition2.y;
		return true;
	}

	private bool ApplyBoost()
	{
		if (isBoost)
		{
			bool flag = false;
			dtBoost += Time.deltaTime;
			if (boostEff != null)
			{
				Vector3 tranceformPosition = TranceformPosition;
				tranceformPosition.y += 1f;
				boostEff.transform.position = tranceformPosition;
			}
			if (dtBoost > maxBoostTime || flag)
			{
				isBoost = false;
				Object.DestroyImmediate(boostEff);
				boostEff = null;
			}
		}
		return isBoost;
	}

	private float GetMinValue()
	{
		float num = (float)(double)MyInfoManager.Instance.spdhackProtector.PvMov;
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayBySlot();
		foreach (BrickManDesc brickManDesc in array)
		{
			if (!brickManDesc.spdhackProtector.Breakinto)
			{
				float num2 = (float)(double)brickManDesc.spdhackProtector.PvMov;
				if (num > num2)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public void PickupFromInstance(long weaponSeq)
	{
		EquipCoordinator component = GetComponent<EquipCoordinator>();
		if (null != component && component.PickupFromFromInstance(weaponSeq))
		{
			_DrawWeapon(emptyHandDrawSpeed, component.CurrentWeapon, notify: false);
		}
	}

	private void checkPortals()
	{
		if (!BrickManager.Instance.userMap.IsPortalMove)
		{
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.INDIVIDUAL || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ESCAPE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
			{
				SpawnerDesc portalAllPos = BrickManager.Instance.userMap.GetPortalAllPos(TranceformPosition);
				if (TranceformPosition != portalAllPos.position)
				{
					GlobalVars.Instance.PlayOneShot(GlobalVars.Instance.sndPortal);
					BroadcastMessage("OnRespawn2", Rot.ToQuaternion(portalAllPos.rotation));
				}
				TranceformPosition = portalAllPos.position;
				base.transform.rotation = Rot.ToQuaternion(portalAllPos.rotation);
			}
			else
			{
				bool red = (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION) ? (MyInfoManager.Instance.Slot < 8) : (MyInfoManager.Instance.Slot < 4);
				SpawnerDesc portalPos = BrickManager.Instance.userMap.GetPortalPos(red, TranceformPosition);
				if (TranceformPosition != portalPos.position)
				{
					GlobalVars.Instance.PlayOneShot(GlobalVars.Instance.sndPortal);
					BroadcastMessage("OnRespawn2", Rot.ToQuaternion(portalPos.rotation));
				}
				TranceformPosition = portalPos.position;
				base.transform.rotation = Rot.ToQuaternion(portalPos.rotation);
			}
		}
		else
		{
			elapsedPortalWait += Time.deltaTime;
			if (elapsedPortalWait >= portalRestartTime)
			{
				BrickManager.Instance.userMap.IsPortalMove = false;
				elapsedPortalWait = 0f;
			}
		}
		if (GlobalVars.Instance.OutoutPortalMessage)
		{
			statusMessage = StringMgr.Instance.Get("CMT_POTAL_MESSAGE_01");
			GlobalVars.Instance.OutoutPortalMessage = false;
		}
	}

	public void addStatusMsg(string msg)
	{
		statusMessage = msg;
	}

	private void updateBlackholeHit()
	{
		if (activateBlackhole)
		{
			deltaTimeBlackholeHit += Time.deltaTime;
			if (deltaTimeBlackholeHit > 5f)
			{
				ActivateBlackhole = false;
			}
		}
	}

	private void checkGuiOn()
	{
		VerifyBattleChat();
		if (custom_inputs.Instance.GetButtonDown("K_UI_HIDDEN") && battleChat != null && !battleChat.IsChatting)
		{
			MyInfoManager.Instance.isGuiOn = !MyInfoManager.Instance.isGuiOn;
			if (MyInfoManager.Instance.isGuiOn)
			{
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null != gameObject)
				{
					CameraController component = gameObject.GetComponent<CameraController>();
					if (null != component)
					{
						component.EnableFpCam(enable: true);
					}
				}
			}
			else
			{
				GameObject gameObject2 = GameObject.Find("Main Camera");
				if (null != gameObject2)
				{
					CameraController component2 = gameObject2.GetComponent<CameraController>();
					if (null != component2)
					{
						component2.EnableFpCam(enable: false);
					}
				}
			}
		}
	}

	private void UpdateBearTrap()
	{
		if (eBearTrap == EBEARTRAP.OVER)
		{
			deltaBeartrap += Time.deltaTime;
			if (deltaBeartrap >= maxOverBearTrap)
			{
				deltaBeartrap = 0f;
				eBearTrap = EBEARTRAP.WAIT;
				BrickManager.Instance.AnimationCrossFade(bearSeq, "idle");
				blickBearTrap.IsActive = false;
			}
		}
		else if (eBearTrap == EBEARTRAP.WAIT)
		{
			deltaBeartrap += Time.deltaTime;
			if (deltaBeartrap >= maxWaitBearTrap)
			{
				deltaBeartrap = 0f;
				eBearTrap = EBEARTRAP.NONE;
			}
		}
	}

	private void UpdateRecognition()
	{
		if (GlobalVars.Instance.IsRecognition)
		{
			dtRecognition += Time.deltaTime;
			if (dtRecognition > dtRecognitionMax)
			{
				GlobalVars.Instance.IsRecognition = false;
				GlobalVars.Instance.recogniteType = -1;
				dtRecognition = 0f;
			}
		}
	}

	private void Update()
	{
		VerifyExplosionMatch();
		VerifyBndMatch();
		VerifyZombieMatch();
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.ARMOR, armor);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.HIT_POINT, hitPoint);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.RUNSPEED, runSpeed);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.WALKSPEED, walkSpeed);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.GODMODE, MyInfoManager.Instance.GodMode ? 1 : 0);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.GHOSTMODE, MyInfoManager.Instance.isGhostOn ? 1 : 0);
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.GM, MyInfoManager.Instance.GM);
		ReportInflictedDamage();
		if (VerifyCharacterController() && VerifyCamera() && BrickManager.Instance.IsLoaded)
		{
			if (statusMessage.Length > 0)
			{
				statusDelta += Time.deltaTime;
				if (statusDelta > statusMessageLimit)
				{
					statusDelta = 0f;
					statusMessage = string.Empty;
				}
			}
			if (isBackDir)
			{
				ElapsedBlinkBack += Time.deltaTime;
				if (ElapsedBlinkBack >= 0.2f)
				{
					drawBackDir = !drawBackDir;
					ElapsedBlinkBack = 0f;
				}
			}
			if (!initIdle)
			{
				deltaTime += Time.deltaTime;
				if (deltaTime > 1.5f)
				{
					deltaTime = 0f;
					ToIdleOrRun();
					initIdle = true;
				}
			}
			checkGuiOn();
			updateFever();
			UpdateSpeed();
			UpdateLucky();
			ApplyFever();
			updateBlackholeHit();
			updateEscapeActive();
			UpdateRecognition();
			collideBricks();
			collideBoosters();
			controllable = MyInfoManager.Instance.CheckControllable();
			if (!ApplyInstallClockBomb() && !ApplyUninstallClockBomb() && !ApplyCannon() && !ApplyTrain())
			{
				CheckAction();
				if (!ApplyFly() && !ApplyBoost())
				{
					ApplyDash();
					if (!ApplyJump() && !ApplyClimb())
					{
						ApplyMove();
						ApplyTrampoline();
					}
				}
			}
			if (!IsDead && RoomManager.Instance.DropItem && custom_inputs.Instance.GetButtonDown("K_ITEM_PICKUP"))
			{
				GlobalVars.Instance.CheckDropedWpns();
			}
			TryOpenDoor();
			ApplySitup();
			ApplyRigidity();
			ApplyGravity();
			ApplyAnimation();
			IncreaseStaminaIfNoDash();
			CheckDeath();
			CheckFalling();
			CheckInvincible();
			CheckWeaponChange();
			NotifyMove();
			CollideToMon();
			checkPortals();
			blickHighGrass.Update();
			blickBearTrap.Update();
			UpdateBearTrap();
			if (null != cc && base.transform.gameObject.activeInHierarchy)
			{
				if (eBearTrap == EBEARTRAP.OVER)
				{
					moveSpeed = 0f;
				}
				Vector3 tranceformPosition = TranceformPosition;
				Vector3 a = moveSpeed * moveDir + vertSpeed * vertDir;
				a *= Time.deltaTime;
				if (bSpeeddown || overHighGrass)
				{
					a *= 0.7f;
				}
				if (isBoost)
				{
					a *= distanceBoost;
				}
				if (!MyInfoManager.Instance.isGhostOn)
				{
					if (!cc.enabled)
					{
						cc.enabled = true;
					}
					collisionFlags = cc.Move(a);
				}
				else if (MyInfoManager.Instance.IsGM)
				{
					if (cc.enabled)
					{
						cc.enabled = false;
					}
					TranceformPosition += a;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GHOST_USE);
				}
				if (controlContext == CONTROL_CONTEXT.RUN || controlContext == CONTROL_CONTEXT.WALK || controlContext == CONTROL_CONTEXT.SQUATTED_WALK || controlContext == CONTROL_CONTEXT.TRAIN_RUN)
				{
					Vector3 tranceformPosition2 = TranceformPosition;
					if (Mathf.Abs(tranceformPosition2.y - tranceformPosition.y) > 0.01f)
					{
						bSpeeddown = true;
					}
					else if (bSpeeddown)
					{
						bSpeeddown = false;
					}
				}
				else if (bSpeeddown)
				{
					bSpeeddown = false;
				}
				CalcResultSpeed(tranceformPosition, TranceformPosition);
			}
			TranceformPosition = base.transform.position;
			if (trainCtrl != null && controlContext == CONTROL_CONTEXT.TRAIN_RUN)
			{
				Vector3 position = trainCtrl.train.transform.position;
				if (eRail == ERAIL.LINK && !Rotated)
				{
					float z = railPosCenter.z;
					Vector3 tranceformPosition3 = TranceformPosition;
					float num = z - tranceformPosition3.z;
					float x = railPosCenter.x;
					Vector3 tranceformPosition4 = TranceformPosition;
					float num2 = x - tranceformPosition4.x;
					if (num2 < 0.1f || num < 0.1f)
					{
						float x2 = railPosCenter.x;
						Vector3 tranceformPosition5 = TranceformPosition;
						Vector3 vector2 = TranceformPosition = new Vector3(x2, tranceformPosition5.y, railPosCenter.z);
						trainCtrl.train.transform.position = railPosCenter;
						trainCtrl.train.transform.rotation = railRotCenter;
						P2PManager.Instance.SendPEER_TRAIN_ROTATE(trainID, trainCtrl.train.transform.forward);
						Rotated = true;
						return;
					}
				}
				if (trnMDir == TRAIN_MOVE_DIR.EAST || trnMDir == TRAIN_MOVE_DIR.WEST)
				{
					Vector3 tranceformPosition6 = TranceformPosition;
					position.x = tranceformPosition6.x;
				}
				if (trnMDir == TRAIN_MOVE_DIR.NORTH || trnMDir == TRAIN_MOVE_DIR.SOUTH)
				{
					Vector3 tranceformPosition7 = TranceformPosition;
					position.z = tranceformPosition7.z;
				}
				Transform transform = trainCtrl.train.transform;
				float x3 = position.x;
				Vector3 tranceformPosition8 = TranceformPosition;
				transform.position = new Vector3(x3, tranceformPosition8.y + 0.35f, position.z);
			}
		}
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		hitPointSecure.Release();
		armorSecure.Release();
		deltaFromDeathSecure.Release();
		positionX.Release();
		positionY.Release();
		positionZ.Release();
	}

	private void OnNoticeCenter(string text)
	{
		if (MyInfoManager.Instance.Status == 4)
		{
			SystemInform.Instance.AddMessageCenter(text);
		}
	}

	public void SetShootEnermyEffect()
	{
		if (cannon != null)
		{
			cannon.SetShootEnermyEffect();
		}
	}

	private float GetForwardAxisRaw()
	{
		float result = custom_inputs.Instance.GetAxisRaw("K_FORWARD", "K_BACKWARD");
		if (MyInfoManager.Instance.isStraightMovement)
		{
			result = 1f;
			GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.STRAIGHT_MOVEMENT_USE);
		}
		return result;
	}

	public void GetHitBungeeBomb(int shooter, int weaponBy)
	{
		if (hitPoint > 0 && !invincible && !MyInfoManager.Instance.GodMode)
		{
			LogAttacker(shooter, 10);
			SetHitPoint(NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.HIT_POINT, 0), autoHealPossible: false);
			hpFont.Scale = 2f;
			if (NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint) <= 0)
			{
				if (MyInfoManager.Instance.Seq == BrickManManager.Instance.haveFlagSeq)
				{
					Vector3 tranceformPosition = TranceformPosition;
					bool flag = (tranceformPosition.y >= BrickManager.Instance.userMap.min.y) ? (flag = false) : (flag = true);
					bool flag2 = (weaponBy == 0) ? (flag2 = true) : (flag2 = false);
					bool flag3 = false;
					if (shooter == MyInfoManager.Instance.Seq)
					{
						flag3 = ((weaponBy == 24 || weaponBy == 7) ? true : false);
					}
					if (!flag && !flag2 && !flag3)
					{
						SockTcp sock = CSNetManager.Instance.Sock;
						Vector3 tranceformPosition2 = TranceformPosition;
						float x = tranceformPosition2.x;
						Vector3 tranceformPosition3 = TranceformPosition;
						float y = tranceformPosition3.y;
						Vector3 tranceformPosition4 = TranceformPosition;
						sock.SendCS_CTF_DROP_FLAG_REQ(x, y, tranceformPosition4.z);
					}
					else
					{
						SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
						CSNetManager.Instance.Sock.SendCS_CTF_DROP_FLAG_REQ(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
					}
					if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
					{
						checkResetFlag = true;
						dtResetFlag = 0f;
					}
				}
				isBoost = false;
				isTrampoline = false;
				if (boostEff != null)
				{
					Object.DestroyImmediate(boostEff);
				}
				if (trampolineEff != null)
				{
					Object.DestroyImmediate(trampolineEff);
				}
				GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
				SetDie(shooter);
				CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(dicInflictedDamage);
				CSNetManager.Instance.Sock.SendCS_KILL_LOG_REQ(0, shooter, 0, MyInfoManager.Instance.Seq, weaponBy, TItemManager.Instance.WeaponBy2Slot(weaponBy), TItemManager.Instance.WeaponBy2Category(weaponBy), -1, dicDamageLog);
				P2PManager.Instance.SendPEER_DIE(shooter);
				dicDamageLog.Clear();
				dicInflictedDamage.Clear();
			}
			BroadcastMessage("OnHit", weaponBy);
			BroadcastMessage("OnHitSnd", -1);
		}
	}

	private void OutputBungeeMoveGuide()
	{
		if (bungeeRespawn)
		{
			string text = StringMgr.Instance.Get("MOVE_DIRECTION_KEY");
			if (text != null && text.Length > 0)
			{
				Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
				GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
			}
		}
	}

	private void OutputFeverActiveGuide()
	{
		if (outputFeverMsg)
		{
			int num = custom_inputs.Instance.KeyIndex("K_FEVER");
			string text = string.Format(StringMgr.Instance.Get("BUNGEE_COMMENT"), custom_inputs.Instance.InputKey[num].ToString());
			if (text != null && text.Length > 0)
			{
				addStatusMsg(text);
			}
		}
	}

	private void OutputPutWeaponGuide()
	{
		if (GlobalVars.Instance.outputCanPutWeaponMsg && !IsDead)
		{
			int num = custom_inputs.Instance.KeyIndex("K_ITEM_PICKUP");
			string text = string.Format(StringMgr.Instance.Get("PICKUP_MESSAGE"), custom_inputs.Instance.InputKey[num].ToString());
			if (text != null && text.Length > 0)
			{
				Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
				GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
			}
		}
	}

	public void initElapsedFeverActing()
	{
		ElapsedFeverActing = 0f;
	}

	private void updateFever()
	{
		if (GlobalVars.Instance.IsFeverMode())
		{
			if (isFever)
			{
				ElapsedFever += Time.deltaTime;
				if (ElapsedFever >= 0.3f)
				{
					isFever = false;
					blinkFever = true;
					ElapsedBlinkFever = 0f;
					ElapsedFeverActing = 0f;
				}
			}
			if (blinkFever)
			{
				ElapsedFevertxt += Time.deltaTime;
				ElapsedBlinkFever += Time.deltaTime;
				if (ElapsedFevertxt > 0.5f)
				{
					ElapsedFevertxt = 0f;
					FeverTxtBack = !FeverTxtBack;
				}
			}
			if (actingFever)
			{
				ElapsedFeverActing += Time.deltaTime;
				float num = 1f - ElapsedFeverActing / 10f;
				if (num < 0f)
				{
					num = 0f;
				}
				GlobalVars.Instance.setCurFever(num);
			}
			if (outputFeverMsg)
			{
				ElapsedFeverMsg += Time.deltaTime;
				if (ElapsedFeverMsg > 3f)
				{
					outputFeverMsg = false;
				}
			}
		}
	}

	private void ApplyFever()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE && NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, hitPoint) > 0 && GlobalVars.Instance.StateFever >= 0 && custom_inputs.Instance.GetButtonDown("K_FEVER"))
		{
			GlobalVars.Instance.StateFever = 1;
			VerifyAudioSource();
			if (null != audioSource)
			{
				audioSource.clip = GlobalVars.Instance.sndFeverIng;
				audioSource.loop = true;
				audioSource.Play();
			}
			P2PManager.Instance.SendPEER_STATE_FEVER(isOn: true);
			SetWpnFever(isSet: true);
		}
	}

	public void EquipSmokeBomb()
	{
		GetComponent<EquipCoordinator>().EquipSmokeBomb();
		Weapon componentInChildren = GetComponentInChildren<Weapon>();
		float drawSpeed = (!(componentInChildren == null)) ? componentInChildren.drawSpeed : emptyHandDrawSpeed;
		_DrawWeapon(drawSpeed, GetComponent<EquipCoordinator>().CurrentWeapon, notify: true);
	}

	public void ReturnBuildGun()
	{
		GetComponent<EquipCoordinator>().ReturnBuildGun();
		Weapon componentInChildren = GetComponentInChildren<Weapon>();
		float drawSpeed = (!(componentInChildren == null)) ? componentInChildren.drawSpeed : emptyHandDrawSpeed;
		_DrawWeapon(drawSpeed, GetComponent<EquipCoordinator>().CurrentWeapon, notify: true);
	}

	public void sparcleFXOn()
	{
		GameObject gameObject = Object.Instantiate((Object)blackholeFX, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		GameObject gameObject2 = GameObject.Find("Main Camera");
		if (gameObject2 != null)
		{
			gameObject.transform.position = gameObject2.transform.position + gameObject2.transform.forward;
			gameObject.transform.parent = gameObject2.transform;
		}
	}

	public void EquipBrickBoom()
	{
		GetComponent<EquipCoordinator>().EquipBrickBoom();
		Weapon componentInChildren = GetComponentInChildren<Weapon>();
		float drawSpeed = (!(componentInChildren == null)) ? componentInChildren.drawSpeed : emptyHandDrawSpeed;
		_DrawWeapon(drawSpeed, GetComponent<EquipCoordinator>().CurrentWeapon, notify: true);
	}

	public void SoundStop()
	{
		audioSource.Stop();
	}

	private void DrawBanish()
	{
		if (RoomManager.Instance.IsVoteProgress())
		{
			VoteStatus vote = RoomManager.Instance.vote;
			voteText.ResetAddPosition();
			voteText.AddPositionX((float)Screen.width - voteText.position.x * 2f);
			voteText.AddPositionY((float)Screen.height - voteText.position.y * 2f);
			voteText.SetTextFormat(vote.targetNickname, vote.GetVoteReason());
			voteText.Draw();
			voteAgree.ResetAddPosition();
			voteAgree.AddPositionX((float)Screen.width - voteAgree.position.x * 2f);
			voteAgree.AddPositionY((float)Screen.height - voteAgree.position.y * 2f);
			voteAgree.SetTextFormat(vote.yes, vote.total);
			voteAgree.Draw();
			voteTime.ResetAddPosition();
			voteTime.AddPositionX((float)Screen.width - voteTime.position.x * 2f);
			voteTime.AddPositionY((float)Screen.height - voteTime.position.y * 2f);
			voteTime.SetTextFormat(vote.GetRemainTime());
			voteTime.Draw();
		}
	}

	private void ToIdleOrRun()
	{
		if (controlContext == CONTROL_CONTEXT.RUN)
		{
			ToRun();
		}
		else
		{
			ToIdle();
		}
	}

	public void ToIdle()
	{
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.CrossFade("idle_h");
		}
		else
		{
			bipAnimation.CrossFade("2_idle_h");
		}
	}

	public void ToRun()
	{
		if (!GetComponent<EquipCoordinator>().IsTwoHands())
		{
			bipAnimation.CrossFade("run_h");
		}
		else
		{
			bipAnimation.CrossFade("2_run");
		}
	}

	private void updateEscapeActive()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ESCAPE)
		{
			timeEscapeActive += Time.deltaTime;
			if (MoveSpeed != 0f)
			{
				isEscapeActive = true;
			}
			if (timeEscapeActive > 10f)
			{
				CSNetManager.Instance.Sock.SendCS_ESCAPE_ACTIVE_PLAYER_REQ(isEscapeActive && !IsDead);
				isEscapeActive = false;
				timeEscapeActive -= 10f;
			}
		}
	}

	public bool IsUserDamaged()
	{
		if (dicDamageLog.Count > 0)
		{
			return true;
		}
		return false;
	}

	public void DropWeaponSkipSetting()
	{
		dropWeaponSkip = true;
	}
}
