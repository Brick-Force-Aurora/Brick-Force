using UnityEngine;

public class TPController : MonoBehaviour
{
	private const string head = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head";

	private const string spine = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1";

	private const string upperArm = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm";

	private const string upperArmL = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm";

	private Transform headBone;

	private Transform spineBone;

	private Transform rightUpperArm;

	private Transform leftUpperArm;

	private Animation bipAnimation;

	private bool initViaPeer;

	private bool initIdle;

	private GameObject hitEff;

	public GameObject[] hitEffs;

	private Transform transformHead;

	public GameObject mainBody;

	public float rightArmCorrect = 10f;

	public bool isLocallyControlled = true;

	public AudioClip disarming;

	public AudioClip growling;

	private float changeIdle;

	private float deltaTime;

	private float tooFarDistance = 3f;

	private float moveDamping = 10f;

	private bool IsFar;

	private float ElapsedFar;

	private float ElapsedFarMax = 0.8f;

	private float FarDistance = 0.8f;

	private bool dead;

	private bool wasDead;

	private float invincibleWait;

	private CharacterController characterController;

	private float vertSpeed;

	private float moveSpeed;

	private Vector3 moveDir = Vector3.forward;

	private Vector3 targetPos = Vector3.zero;

	private float targetHorzAngle;

	private float targetVertAngle;

	private float brokenAngle = float.PositiveInfinity;

	private LocalController.CONTROL_CONTEXT controlContext = LocalController.CONTROL_CONTEXT.NONE;

	private Weapon.TYPE nextWeaponType = Weapon.TYPE.MAIN;

	public GameObject beginWantedFx;

	public GameObject wantedFx;

	private Transform wantedFxInst;

	public Color orgColor = Color.white;

	public Color zombieColor = Color.green;

	public GameObject beginZombieFx;

	public GameObject zombieFx;

	private Transform zombieFxInst;

	public GameObject explosion;

	public GameObject congratulation;

	public GameObject heal;

	public GameObject speedup;

	private GameObject congratulationCopy;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private Camera maincam;

	public Texture2D texXpBar;

	public Texture2D texArmorBar;

	public Texture2D texScrollLight;

	private bool bHitEvent;

	private float dtHitEvent;

	private bool isBoost;

	private float dtBoost;

	private GameObject boostEff;

	private bool isTrampoline;

	private float previousY;

	private bool isSpeedUp;

	private bool bungeeRespawn;

	public AudioClip sndLanding;

	private bool isChild;

	private CannonController cannon;

	private float zombieAngleMax = 15f;

	private float zombieAngleMin = -15f;

	private float deltaTimeZombieAngle;

	private float limitTimeZombieAngle = 6f;

	private bool zombieAngleDir;

	private bool isZombie;

	private float ElapsedFever;

	private bool isFever;

	private GameObject objFever;

	public bool IsLocallyControlled
	{
		get
		{
			return isLocallyControlled;
		}
		set
		{
			isLocallyControlled = value;
		}
	}

	public bool IsDead => dead;

	public bool IsSpeedUp => isSpeedUp;

	public bool IsChild
	{
		get
		{
			return isChild;
		}
		set
		{
			isChild = value;
		}
	}

	public Vector3 lookAt()
	{
		return base.transform.forward;
	}

	public void destroyCongratulation()
	{
		if (congratulationCopy != null)
		{
			Object.DestroyImmediate(congratulationCopy);
		}
	}

	public void OnHitEvent()
	{
		bHitEvent = true;
		dtHitEvent = 0f;
	}

	public void Heal()
	{
		if (null != heal && null != spineBone)
		{
			GameObject gameObject = Object.Instantiate((Object)heal) as GameObject;
			gameObject.transform.position = spineBone.position;
			gameObject.transform.parent = spineBone;
		}
	}

	public void Speedup()
	{
		if (null != speedup && null != headBone)
		{
			isSpeedUp = true;
			GameObject gameObject = Object.Instantiate((Object)speedup) as GameObject;
			gameObject.transform.position = headBone.position;
			gameObject.transform.parent = headBone;
			gameObject.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
			gameObject.GetComponent<SpeedUpTPEffect>().owner = this;
		}
	}

	public void SpeedupEnd()
	{
		isSpeedUp = false;
	}

	public void BeginWanted()
	{
		if (null != beginWantedFx && null != spineBone)
		{
			GameObject gameObject = Object.Instantiate((Object)beginWantedFx) as GameObject;
			gameObject.transform.position = spineBone.position;
			gameObject.transform.parent = spineBone;
			gameObject.transform.rotation = Quaternion.identity;
		}
	}

	private void OnStep()
	{
		int stepOnBrick = BrickManager.Instance.GetStepOnBrick(base.transform.position);
		if (stepOnBrick >= 0)
		{
			AudioClip stepSound = BrickManager.Instance.GetStepSound(stepOnBrick);
			if (null != stepSound)
			{
				GetComponent<AudioSource>().PlayOneShot(stepSound);
			}
		}
	}

	private void OnSwitchWeaponStart()
	{
	}

	private void OnSwitchWeapon()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			component.ChangeWeapon(nextWeaponType);
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (null != currentWeapon)
			{
				bipAnimation["reload_h"].normalizedSpeed = currentWeapon.drawSpeed;
				base.animation["SwitchWeapon"].normalizedSpeed = currentWeapon.drawSpeed;
			}
		}
	}

	private void OnSwitchWeaponEnd()
	{
		ToIdle();
		SetMoveAnimation(controlContext);
	}

	private void OnReloadStart()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
			if (null != currentWeaponGadget)
			{
				currentWeaponGadget.ClipOut();
			}
			WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
			if (null != currentLeftWeaponGadget)
			{
				currentLeftWeaponGadget.ClipOut();
			}
		}
	}

	private void OnReload()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
			if (null != currentWeaponGadget)
			{
				currentWeaponGadget.ClipIn();
			}
			WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
			if (null != currentLeftWeaponGadget)
			{
				currentLeftWeaponGadget.ClipIn();
			}
		}
	}

	private void OnReloadEnd()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
			if (null != currentWeaponGadget)
			{
				currentWeaponGadget.BoltUp();
			}
			WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
			if (null != currentLeftWeaponGadget)
			{
				currentLeftWeaponGadget.BoltUp();
			}
		}
	}

	private void OnAnimationEnd()
	{
	}

	private void OnLeaveCannon()
	{
		cannon = null;
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			component.SetActiveCurrentWeapon(state: true);
		}
	}

	private void OnGetCannon(CannonController cannonController)
	{
		cannon = cannonController;
		targetVertAngle = 0f;
		bipAnimation.Stop();
		bipAnimation.Play("a_idle");
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			component.SetActiveCurrentWeapon(state: false);
		}
	}

	public void SetWeapon(Weapon.TYPE weaponType)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null != component)
		{
			component.ChangeWeapon(weaponType);
		}
	}

	public void Rotate(Vector3 dir, float xAngle, float yAngle)
	{
		if (!dead)
		{
			targetHorzAngle = xAngle;
			targetVertAngle = yAngle;
			moveDir = dir;
		}
	}

	public void Move(LocalController.CONTROL_CONTEXT cc, float speed, float vSpeed, Vector3 pos, float Elapsed)
	{
		if (!isLocallyControlled)
		{
			targetPos = pos;
			vertSpeed = vSpeed;
			if (!dead)
			{
				moveSpeed = speed;
			}
			if (cc != controlContext)
			{
				SetMoveAnimation(cc);
				controlContext = cc;
			}
			if (!initViaPeer)
			{
				base.transform.position = targetPos;
				initViaPeer = true;
			}
		}
	}

	private void InitializeBendingBone()
	{
		headBone = base.transform.Find("man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
		if (null == headBone)
		{
			Debug.LogError("Fail to find bending bone: man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
		}
		spineBone = base.transform.Find("man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
		if (null == spineBone)
		{
			Debug.LogError("Fail to find bending bone: " + spineBone);
		}
		rightUpperArm = base.transform.Find("man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm");
		if (null == rightUpperArm)
		{
			Debug.LogError("Fail to find bending bone: man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm");
		}
		leftUpperArm = base.transform.Find("man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm");
		if (null == leftUpperArm)
		{
			Debug.LogError("Fail to find bending bone: man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm");
		}
	}

	public void Throw()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && (currentWeapon.slot == Weapon.TYPE.PROJECTILE || currentWeapon.slot == Weapon.TYPE.MODE_SPECIFIC))
			{
				bipAnimation.CrossFade("throw_h");
			}
		}
	}

	public void SwapWeapon(Weapon.TYPE weaponType)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot != weaponType)
			{
				nextWeaponType = weaponType;
				if (!(bipAnimation == null))
				{
					bipAnimation.Stop("reload_h");
					if (component.IsTwoHands())
					{
						bipAnimation.Stop("2_reload_h_skirt");
					}
					base.animation.Stop("SwitchWeapon");
					bipAnimation["reload_h"].normalizedSpeed = currentWeapon.drawSpeed;
					if (component.IsTwoHands())
					{
						bipAnimation["2_reload_h_skirt"].normalizedSpeed = currentWeapon.drawSpeed;
					}
					base.animation["SwitchWeapon"].normalizedSpeed = currentWeapon.drawSpeed;
					bipAnimation.CrossFade("reload_h");
					if (component.IsTwoHands())
					{
						bipAnimation.CrossFade("2_reload_h_skirt");
					}
					base.animation.Blend("SwitchWeapon");
				}
			}
		}
	}

	public void Reload(Weapon.TYPE weaponType)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == weaponType && !(bipAnimation == null))
			{
				bipAnimation.Stop("reload_h");
				if (component.IsTwoHands())
				{
					bipAnimation.Stop("2_reload_h_skirt");
				}
				base.animation.Stop("Reload");
				bipAnimation["reload_h"].normalizedSpeed = currentWeapon.reloadSpeed;
				if (component.IsTwoHands())
				{
					bipAnimation["2_reload_h_skirt"].normalizedSpeed = currentWeapon.drawSpeed;
				}
				base.animation["Reload"].normalizedSpeed = currentWeapon.reloadSpeed;
				bipAnimation.CrossFade("reload_h");
				if (component.IsTwoHands())
				{
					bipAnimation.CrossFade("2_reload_h_skirt");
				}
				base.animation.Blend("Reload");
			}
		}
	}

	public void Uninstall(bool uninstall)
	{
		if (uninstall)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (null != component)
			{
				component.clip = disarming;
				component.loop = true;
				component.Play();
			}
		}
		else
		{
			AudioSource component2 = GetComponent<AudioSource>();
			if (null != component2)
			{
				component2.Stop();
			}
		}
	}

	public void Install(bool install)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == Weapon.TYPE.MODE_SPECIFIC)
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					currentWeaponGadget.Install(install);
				}
			}
		}
	}

	public void GunAnim(Weapon.TYPE weaponType, int anim)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == weaponType)
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					currentWeaponGadget.GunAnim(anim);
					TWeapon tWeapon = currentWeapon.tItem as TWeapon;
					if (tWeapon != null && tWeapon.IsTwoHands)
					{
						WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
						if (!(null == currentLeftWeaponGadget))
						{
							currentLeftWeaponGadget.GunAnim(anim);
						}
					}
				}
			}
		}
	}

	public Weapon.TYPE GetCurrentWeaponType()
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (null == component)
		{
			return Weapon.TYPE.COUNT;
		}
		Weapon currentWeapon = component.GetCurrentWeapon();
		if (currentWeapon != null)
		{
			return currentWeapon.slot;
		}
		return Weapon.TYPE.COUNT;
	}

	public void FireAction(Weapon.TYPE weaponType)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == weaponType)
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					currentWeaponGadget.FireAction();
					TWeapon tWeapon = currentWeapon.tItem as TWeapon;
					if (tWeapon != null && tWeapon.IsTwoHands)
					{
						WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
						if (!(null == currentLeftWeaponGadget))
						{
							currentLeftWeaponGadget.FireAction();
						}
					}
				}
			}
		}
	}

	public void Fire(Weapon.TYPE weaponType, int tile, Vector3 origin, Vector3 direction)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon weapon = component.GetCurrentWeapon();
			if (null == weapon)
			{
				weapon = component.GetWeaponBySlot(weaponType);
				if (null == weapon)
				{
					return;
				}
			}
			if (weapon.slot == weaponType)
			{
				WeaponGadget component2 = weapon.transform.gameObject.GetComponent<WeaponGadget>();
				if (!(null == component2))
				{
					component2.Fire(tile, origin, direction);
					TWeapon tWeapon = weapon.tItem as TWeapon;
					if (tWeapon != null && tWeapon.IsTwoHands)
					{
						WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
						if (!(null == currentLeftWeaponGadget))
						{
							currentLeftWeaponGadget.Fire(tile, origin, direction);
						}
					}
				}
			}
		}
	}

	public void Compose(bool isDel)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && (currentWeapon.slot == Weapon.TYPE.MODE_SPECIFIC || currentWeapon.slot == Weapon.TYPE.MAIN))
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					currentWeaponGadget.Compose(isDel);
					TWeapon tWeapon = currentWeapon.tItem as TWeapon;
					if (tWeapon != null && tWeapon.IsTwoHands)
					{
						WeaponGadget currentLeftWeaponGadget = component.GetCurrentLeftWeaponGadget();
						if (!(null == currentLeftWeaponGadget))
						{
							currentLeftWeaponGadget.Compose(isDel);
						}
					}
				}
			}
		}
	}

	public void Slash(float normalizedSpeed)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == Weapon.TYPE.MELEE)
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					currentWeaponGadget.Fire(0, Vector3.zero, Vector3.zero);
					if (!(bipAnimation == null))
					{
						bipAnimation.Stop("slash_h");
						bipAnimation["slash_h"].normalizedSpeed = normalizedSpeed;
						bipAnimation.Play("slash_h");
					}
				}
			}
		}
	}

	public void ThrowProjectile(int projectile, Vector3 pos, Vector3 rot)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			Weapon currentWeapon = component.GetCurrentWeapon();
			if (!(null == currentWeapon) && currentWeapon.slot == Weapon.TYPE.PROJECTILE)
			{
				WeaponGadget currentWeaponGadget = component.GetCurrentWeaponGadget();
				if (!(null == currentWeaponGadget))
				{
					bool bSoundvoc = false;
					PlayerProperty component2 = GetComponent<PlayerProperty>();
					if (component2 != null && !component2.IsHostile())
					{
						bSoundvoc = true;
					}
					currentWeaponGadget.Throw(projectile, component.RightHandPos, pos, rot, bSoundvoc, component.IsYang);
				}
			}
		}
	}

	public void SetSenseBeam(int playerSlot, int projectile, Vector3 pos, Vector3 normal)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			bool bSoundvoc = false;
			PlayerProperty component2 = GetComponent<PlayerProperty>();
			if (component2 != null && !component2.IsHostile())
			{
				bSoundvoc = true;
			}
			GdgtSenseBomb componentInChildren = mainBody.GetComponentInChildren<GdgtSenseBomb>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSenseBeam(playerSlot, projectile, component.RightHandPos, pos, normal, bSoundvoc);
			}
		}
	}

	public void FireRW(int launcher, int ammoId, Vector3 pos, Vector3 rot)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			GdgtGun[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGun>();
			if (componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Fire2(ammoId, launcher, pos, rot);
				}
			}
		}
	}

	public void FlyRW(int fireID, Vector3 pos, Vector3 rot)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			GdgtGun[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGun>();
			if (componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Fly(fireID, pos, rot);
				}
			}
		}
	}

	public void KaBoomRW(int fireID, Vector3 pos, Vector3 rot, bool viewColeff)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			GdgtGun[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGun>();
			if (componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].KaBoom(fireID, pos, rot, viewColeff);
				}
			}
		}
	}

	public void ProjectileFly(int projectile, Vector3 pos, Vector3 rot, float range)
	{
		GdgtGrenade[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
		if (componentsInChildren.Length > 0)
		{
			componentsInChildren[0].Fly(projectile, pos, rot, range);
		}
		else
		{
			GdgtFlashBang[] componentsInChildren2 = mainBody.GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
			if (componentsInChildren2.Length > 0)
			{
				componentsInChildren2[0].Fly(projectile, pos, rot, range);
			}
			GdgtXmasBomb[] componentsInChildren3 = GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
			if (componentsInChildren3.Length > 0)
			{
				componentsInChildren3[0].Fly(projectile, pos, rot, range);
			}
		}
	}

	public void KaboomSenseBoom(int projectile)
	{
		GdgtSenseBomb[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtSenseBomb>(includeInactive: true);
		if (componentsInChildren.Length > 0)
		{
			componentsInChildren[0].Kaboom(projectile);
		}
	}

	public void ProjectileKaboom(int projectile)
	{
		switch (projectile)
		{
		case -1:
		{
			GdgtGrenade[] componentsInChildren5 = mainBody.GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
			if (componentsInChildren5.Length > 0)
			{
				componentsInChildren5[0].SelfKaboom(base.transform.position);
			}
			else
			{
				GdgtFlashBang[] componentsInChildren6 = mainBody.GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
				if (componentsInChildren6.Length > 0)
				{
					componentsInChildren6[0].SelfKaboom(base.transform.position);
				}
				GdgtXmasBomb[] componentsInChildren7 = GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
				if (componentsInChildren7.Length > 0)
				{
					componentsInChildren7[0].SelfKaboom(base.transform.position);
				}
			}
			break;
		}
		case -2:
		{
			GlobalVars instance = GlobalVars.Instance;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y = position2.y + 1f;
			Vector3 position3 = base.transform.position;
			instance.SwitchFlashbang(bVis: true, new Vector3(x, y, position3.z));
			break;
		}
		default:
		{
			GdgtGrenade[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].Kaboom(projectile);
			}
			else
			{
				GdgtFlashBang[] componentsInChildren2 = mainBody.GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
				if (componentsInChildren2.Length > 0)
				{
					componentsInChildren2[0].Kaboom(projectile);
				}
				GdgtSenseBomb[] componentsInChildren3 = mainBody.GetComponentsInChildren<GdgtSenseBomb>(includeInactive: true);
				if (componentsInChildren3.Length > 0)
				{
					componentsInChildren3[0].Kaboom(projectile);
				}
				GdgtXmasBomb[] componentsInChildren4 = GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
				if (componentsInChildren4.Length > 0)
				{
					componentsInChildren4[0].Kaboom(projectile);
				}
			}
			break;
		}
		}
	}

	public void Congratulation()
	{
		if (null != congratulation)
		{
			congratulationCopy = (Object.Instantiate((Object)congratulation, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
	}

	private void SetMoveAnimation(LocalController.CONTROL_CONTEXT cc)
	{
		if (!dead)
		{
			bool flag = false;
			LookCoordinator component = GetComponent<LookCoordinator>();
			if (component != null)
			{
				flag = component.IsTwoHands();
			}
			if (bipAnimation != null)
			{
				switch (cc)
				{
				case LocalController.CONTROL_CONTEXT.SQUATTED:
					if (!flag)
					{
						bipAnimation.CrossFade("1a_down_idle");
					}
					else
					{
						bipAnimation.CrossFade("2a_down_idle");
					}
					break;
				case LocalController.CONTROL_CONTEXT.STOP:
				case LocalController.CONTROL_CONTEXT.HANG:
					if (!flag)
					{
						bipAnimation.CrossFade("a_idle");
					}
					else
					{
						bipAnimation.CrossFade("2a_idle");
					}
					break;
				case LocalController.CONTROL_CONTEXT.RUN:
				case LocalController.CONTROL_CONTEXT.JUMP:
				case LocalController.CONTROL_CONTEXT.CLIMB:
					if (!flag)
					{
						bipAnimation.CrossFade("a_run");
					}
					else
					{
						bipAnimation.CrossFade("2a_run_skirt");
					}
					break;
				case LocalController.CONTROL_CONTEXT.SQUATTED_WALK:
					if (!flag)
					{
						bipAnimation.CrossFade("1a_down_walk");
					}
					else
					{
						bipAnimation.CrossFade("2a_down_walk");
					}
					break;
				case LocalController.CONTROL_CONTEXT.WALK:
					if (!flag)
					{
						bipAnimation.CrossFade("a_walk");
					}
					else
					{
						bipAnimation.CrossFade("2a_walk_skirt");
					}
					break;
				}
			}
			switch (cc)
			{
			case LocalController.CONTROL_CONTEXT.RUN:
			case LocalController.CONTROL_CONTEXT.CLIMB:
				base.animation.Play("StepRun");
				break;
			default:
				base.animation.Stop("StepRun");
				break;
			}
			if (controlContext == LocalController.CONTROL_CONTEXT.JUMP && cc != LocalController.CONTROL_CONTEXT.JUMP)
			{
				AudioSource component2 = GetComponent<AudioSource>();
				if (null != component2 && null != sndLanding)
				{
					component2.PlayOneShot(sndLanding);
				}
			}
		}
	}

	private void InitializeAnimation()
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		bipAnimation = null;
		int num = 0;
		while (bipAnimation == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name == "man")
			{
				bipAnimation = componentsInChildren[num];
			}
			num++;
		}
		if (null == bipAnimation)
		{
			Debug.LogError("Fail to get Animation component");
		}
		else
		{
			bipAnimation.wrapMode = WrapMode.Loop;
			bipAnimation["walk"].layer = 1;
			bipAnimation["walk"].wrapMode = WrapMode.Loop;
			bipAnimation["1a_down_walk"].layer = 1;
			bipAnimation["1a_down_walk"].wrapMode = WrapMode.Loop;
			bipAnimation["2a_down_walk"].layer = 1;
			bipAnimation["2a_down_walk"].wrapMode = WrapMode.Loop;
			bipAnimation["run"].layer = 1;
			bipAnimation["run"].wrapMode = WrapMode.Loop;
			bipAnimation["2_run_skirt"].layer = 1;
			bipAnimation["2_run_skirt"].wrapMode = WrapMode.Loop;
			bipAnimation["a_walk"].layer = 1;
			bipAnimation["a_walk"].wrapMode = WrapMode.Loop;
			bipAnimation["2a_walk_skirt"].layer = 1;
			bipAnimation["2a_walk_skirt"].wrapMode = WrapMode.Loop;
			bipAnimation["a_run"].layer = 1;
			bipAnimation["a_run"].wrapMode = WrapMode.Loop;
			bipAnimation["2a_run_skirt"].layer = 1;
			bipAnimation["2a_run_skirt"].wrapMode = WrapMode.Loop;
			bipAnimation["throw_h"].layer = 10;
			bipAnimation["throw_h"].wrapMode = WrapMode.Once;
			bipAnimation["throw_h"].AddMixingTransform(spineBone);
			bipAnimation["slash_big_h"].layer = 10;
			bipAnimation["slash_big_h"].wrapMode = WrapMode.Once;
			bipAnimation["slash_big_h"].AddMixingTransform(spineBone);
			bipAnimation["slash_h"].layer = 10;
			bipAnimation["slash_h"].wrapMode = WrapMode.Once;
			bipAnimation["slash_h"].AddMixingTransform(spineBone);
			bipAnimation["change_bottom_1"].layer = 1;
			bipAnimation["change_bottom_1"].wrapMode = WrapMode.Once;
			bipAnimation["change_bottom_2"].layer = 1;
			bipAnimation["change_bottom_2"].wrapMode = WrapMode.Once;
			bipAnimation["change_top_1"].layer = 1;
			bipAnimation["change_top_1"].wrapMode = WrapMode.Once;
			bipAnimation["change_top_2"].layer = 1;
			bipAnimation["change_top_2"].wrapMode = WrapMode.Once;
			bipAnimation["change_weapon_1"].layer = 1;
			bipAnimation["change_weapon_1"].wrapMode = WrapMode.Once;
			bipAnimation["change_weapon_2"].layer = 1;
			bipAnimation["change_weapon_2"].wrapMode = WrapMode.Once;
			bipAnimation["reload_h"].layer = 10;
			bipAnimation["reload_h"].wrapMode = WrapMode.Once;
			bipAnimation["reload_h"].AddMixingTransform(spineBone);
			bipAnimation["2_reload_h_skirt"].layer = 10;
			bipAnimation["2_reload_h_skirt"].wrapMode = WrapMode.Once;
			bipAnimation["2_reload_h_skirt"].AddMixingTransform(spineBone);
			bipAnimation["a_idle"].layer = 1;
			bipAnimation["a_idle"].wrapMode = WrapMode.Loop;
			bipAnimation["2a_idle"].layer = 1;
			bipAnimation["2a_idle"].wrapMode = WrapMode.Loop;
			bipAnimation["1a_down_idle"].layer = 1;
			bipAnimation["1a_down_idle"].wrapMode = WrapMode.Loop;
			bipAnimation["2a_down_idle"].layer = 1;
			bipAnimation["2a_down_idle"].wrapMode = WrapMode.Loop;
			bipAnimation["idle01"].layer = 1;
			bipAnimation["idle01"].wrapMode = WrapMode.Loop;
			bipAnimation["idle02"].layer = 1;
			bipAnimation["idle02"].wrapMode = WrapMode.Loop;
			bipAnimation["idle03"].layer = 1;
			bipAnimation["idle03"].wrapMode = WrapMode.Loop;
			bipAnimation["die"].layer = 1;
			bipAnimation["die"].wrapMode = WrapMode.ClampForever;
			bipAnimation["die_1"].layer = 1;
			bipAnimation["die_1"].wrapMode = WrapMode.ClampForever;
			bipAnimation["die_2"].layer = 1;
			bipAnimation["die_2"].wrapMode = WrapMode.ClampForever;
			bipAnimation["e_angry_1"].layer = 1;
			bipAnimation["e_angry_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_cry_1"].layer = 1;
			bipAnimation["e_cry_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_cry_2"].layer = 1;
			bipAnimation["e_cry_2"].wrapMode = WrapMode.Loop;
			bipAnimation["e_death_1"].layer = 1;
			bipAnimation["e_death_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_death_2"].layer = 1;
			bipAnimation["e_death_2"].wrapMode = WrapMode.Loop;
			bipAnimation["e_dizzy_1"].layer = 1;
			bipAnimation["e_dizzy_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_fear_1"].layer = 1;
			bipAnimation["e_fear_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_fear_2"].layer = 1;
			bipAnimation["e_fear_2"].wrapMode = WrapMode.Loop;
			bipAnimation["e_laugh_1"].layer = 1;
			bipAnimation["e_laugh_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_loss_1"].layer = 1;
			bipAnimation["e_loss_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_love_1"].layer = 1;
			bipAnimation["e_love_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_nonesense_1"].layer = 1;
			bipAnimation["e_nonesense_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_rage_1"].layer = 1;
			bipAnimation["e_rage_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_shout_1"].layer = 1;
			bipAnimation["e_shout_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_shout_2"].layer = 1;
			bipAnimation["e_shout_2"].wrapMode = WrapMode.Loop;
			bipAnimation["e_shy_1"].layer = 1;
			bipAnimation["e_shy_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_sigh_1"].layer = 1;
			bipAnimation["e_sigh_1"].wrapMode = WrapMode.Loop;
			bipAnimation["e_suprise_1"].layer = 1;
			bipAnimation["e_suprise_1"].wrapMode = WrapMode.Loop;
			bipAnimation["upstand"].layer = 1;
			bipAnimation["upstand"].wrapMode = WrapMode.Loop;
			bipAnimation["setbomb"].layer = 1;
			bipAnimation["setbomb"].wrapMode = WrapMode.Once;
			bipAnimation["1a_up"].layer = 1;
			bipAnimation["1a_up"].wrapMode = WrapMode.ClampForever;
			bipAnimation["1a_down"].layer = 1;
			bipAnimation["1a_down"].wrapMode = WrapMode.ClampForever;
			bipAnimation["2a_up"].layer = 1;
			bipAnimation["2a_up"].wrapMode = WrapMode.ClampForever;
			bipAnimation["2a_down"].layer = 1;
			bipAnimation["2a_down"].wrapMode = WrapMode.ClampForever;
			SetIdle(queued: false);
		}
	}

	private void SetIdle(bool queued)
	{
		string[] array = new string[3]
		{
			"idle01",
			"idle02",
			"idle03"
		};
		if (queued)
		{
			bipAnimation.CrossFadeQueued(array[Random.Range(0, array.Length)]);
		}
		else
		{
			bipAnimation.CrossFade(array[Random.Range(0, array.Length)]);
		}
		changeIdle = Random.Range(3f, 10f);
		deltaTime = 0f;
	}

	private void OnChangeCostume(TItem.SLOT slot)
	{
		if (isLocallyControlled && !(null == bipAnimation))
		{
			string[] array = new string[2]
			{
				"change_bottom_1",
				"change_bottom_2"
			};
			string[] array2 = new string[2]
			{
				"change_top_1",
				"change_top_2"
			};
			string[] array3 = new string[2]
			{
				"change_weapon_1",
				"change_weapon_2"
			};
			string text = string.Empty;
			Weapon.TYPE tYPE = Weapon.TYPE.COUNT;
			switch (slot)
			{
			case TItem.SLOT.UPPER:
				text = array2[Random.Range(0, array2.Length)];
				break;
			case TItem.SLOT.LOWER:
				text = array[Random.Range(0, array.Length)];
				break;
			case TItem.SLOT.MELEE:
				text = array3[Random.Range(0, array3.Length)];
				tYPE = Weapon.TYPE.MELEE;
				break;
			case TItem.SLOT.AUX:
				text = array3[Random.Range(0, array3.Length)];
				tYPE = Weapon.TYPE.AUX;
				break;
			case TItem.SLOT.MAIN:
				text = array3[Random.Range(0, array3.Length)];
				tYPE = Weapon.TYPE.MAIN;
				break;
			case TItem.SLOT.BOMB:
				text = array3[Random.Range(0, array3.Length)];
				tYPE = Weapon.TYPE.PROJECTILE;
				break;
			}
			if (tYPE != Weapon.TYPE.COUNT)
			{
				LookCoordinator component = GetComponent<LookCoordinator>();
				if (null != component)
				{
					component.ChangeWeapon(tYPE);
				}
			}
			if (text.Length > 0)
			{
				bipAnimation.CrossFade(text);
				SetIdle(queued: true);
			}
		}
	}

	public void EmotionalAct(string ani)
	{
		bool flag = true;
		switch (ani)
		{
		case "e_angry_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.ANGRY);
			break;
		case "e_cry_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.CRY);
			break;
		case "e_cry_2":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.CRY);
			break;
		case "e_death_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.DEATH);
			break;
		case "e_death_2":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.DEATH);
			break;
		case "e_dizzy_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.DIZZY);
			break;
		case "e_fear_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.FEAR);
			break;
		case "e_fear_2":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.FEAR);
			break;
		case "e_laugh_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.LAUGH);
			break;
		case "e_loss_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.LOSS);
			break;
		case "e_love_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.LOVE);
			break;
		case "e_nonesense_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.NONSENSE);
			break;
		case "e_rage_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.RAGE);
			break;
		case "e_shout_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.SHOUT);
			break;
		case "e_shout_2":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.SHOUT);
			break;
		case "e_shy_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.SHY);
			break;
		case "e_sigh_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.SIGH);
			break;
		case "e_suprise_1":
			SendMessage("OnFeel", FacialExpressor.EXPRESSION.SUPRISED);
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			bipAnimation.Play(ani);
		}
	}

	private GameObject GetHitEff()
	{
		if (MyInfoManager.Instance.IsBelow12())
		{
			return hitEffs[1];
		}
		return hitEffs[0];
	}

	private void Start()
	{
		hitEff = GetHitEff();
		InitializeBendingBone();
		InitializeAnimation();
		isLocallyControlled = (Application.loadedLevelName.Contains("Briefing") || Application.loadedLevelName.Contains("Result") || Application.loadedLevelName.Contains("Lobby"));
		initViaPeer = false;
		controlContext = LocalController.CONTROL_CONTEXT.NONE;
		characterController = GetComponent<CharacterController>();
		if (null == characterController)
		{
			Debug.LogError("Fail to get CharacterController component");
		}
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name.Contains("Bip01 Neck"))
			{
				transformHead = componentsInChildren[i];
			}
		}
		Transform transform = base.transform.FindChild("FX_Falling");
		if (null != transform)
		{
			transform.gameObject.SetActive(value: false);
		}
		deltaTimeZombieAngle = 0f;
		limitTimeZombieAngle = Random.Range(6f, 9f);
	}

	private void InterpolatePlayerPosition()
	{
		float num = Vector3.Distance(base.transform.position, targetPos);
		if (num > tooFarDistance)
		{
			base.transform.position = targetPos;
		}
		else if (num > 0.2f && num <= tooFarDistance)
		{
			float num2 = num / tooFarDistance;
			base.transform.position = Vector3.Lerp(base.transform.position, targetPos, moveDamping * num2 * Time.deltaTime);
			float num3 = Vector3.Distance(base.transform.position, targetPos);
			float y = targetPos.y;
			Vector3 position = base.transform.position;
			float num4 = Mathf.Abs(y - position.y);
			if (num3 > FarDistance && num4 < 0.1f)
			{
				IsFar = true;
			}
			else
			{
				IsFar = false;
			}
		}
		if (IsFar)
		{
			ElapsedFar += Time.deltaTime;
			if (ElapsedFar > ElapsedFarMax)
			{
				IsFar = false;
				ElapsedFar = 0f;
				base.transform.position = targetPos;
			}
		}
	}

	private bool ApplyNotPlayingUserPosition()
	{
		PlayerProperty component = GetComponent<PlayerProperty>();
		if (null == component)
		{
			return true;
		}
		if (component.Desc.Status == 4)
		{
			return false;
		}
		base.transform.position = new Vector3(0f, -1000f, 0f);
		return true;
	}

	public void OnTrampoline(int brickSeq)
	{
		boostEff = (Object.Instantiate((Object)GlobalVars.Instance.tramp_hor_eff, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		isTrampoline = true;
		Vector3 position = base.transform.position;
		previousY = position.y - 1f;
		GameObject brickObject = BrickManager.Instance.GetBrickObject(brickSeq);
		if (!(brickObject == null))
		{
			Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.Play("fire");
			}
		}
	}

	public void OnBoost(int brickSeq)
	{
		Vector3 position = base.transform.position;
		position.y += 1f;
		boostEff = (Object.Instantiate((Object)GlobalVars.Instance.tramp_vert_eff, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		isBoost = true;
		dtBoost = 0f;
		GameObject brickObject = BrickManager.Instance.GetBrickObject(brickSeq);
		if (!(brickObject == null))
		{
			Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.Play("fire");
			}
		}
	}

	private bool ApplyTrampoline()
	{
		if (!isTrampoline)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		if (position.y - previousY > -0.1f)
		{
			if (boostEff != null)
			{
				boostEff.transform.position = base.transform.position;
			}
		}
		else
		{
			Object.DestroyImmediate(boostEff);
			boostEff = null;
			isTrampoline = false;
		}
		Vector3 position2 = base.transform.position;
		previousY = position2.y;
		return true;
	}

	private bool ApplyBoost()
	{
		if (isBoost)
		{
			dtBoost += Time.deltaTime;
			if (boostEff != null)
			{
				Vector3 position = base.transform.position;
				position.y += 1f;
				boostEff.transform.position = position;
			}
			if (dtBoost > 2f)
			{
				isBoost = false;
				Object.DestroyImmediate(boostEff);
				boostEff = null;
			}
		}
		return isBoost;
	}

	private void Update()
	{
		if (isLocallyControlled)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > changeIdle)
			{
				SetIdle(queued: false);
			}
		}
		else
		{
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
			ApplyTrampoline();
			ApplyBoost();
			ApplyBungeeRespawnFallingEffect();
			ApplyWantedEffect();
			ApplyZombieEffect();
			checkFever();
			updateFever();
			if (!ApplyNotPlayingUserPosition())
			{
				if (cannon == null)
				{
					if (dead)
					{
						moveSpeed = 0f;
					}
					InterpolatePlayerPosition();
					if (null != characterController && characterController.enabled)
					{
						Vector3 a = moveSpeed * moveDir + new Vector3(0f, vertSpeed, 0f);
						a *= Time.deltaTime;
						characterController.Move(a);
					}
					if (!dead)
					{
						base.transform.rotation = Quaternion.Euler(0f, targetHorzAngle, 0f);
					}
				}
				GdgtGrenade[] componentsInChildren = mainBody.GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
				if (componentsInChildren.Length > 0)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].LetProjectileFly();
					}
				}
				else
				{
					GdgtFlashBang[] componentsInChildren2 = mainBody.GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
					if (componentsInChildren2.Length > 0)
					{
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							componentsInChildren2[j].LetProjectileFly();
						}
					}
					GdgtXmasBomb[] componentsInChildren3 = GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
					if (componentsInChildren3.Length > 0)
					{
						for (int k = 0; k < componentsInChildren3.Length; k++)
						{
							componentsInChildren3[k].LetProjectileFly();
						}
					}
				}
			}
		}
	}

	private float GetHeadAngle()
	{
		if (brokenAngle > 90f)
		{
			brokenAngle = targetVertAngle;
		}
		brokenAngle = Mathf.Lerp(brokenAngle, targetVertAngle, 5f * Time.deltaTime);
		return brokenAngle * 0.8f;
	}

	private float GetSpineAngle()
	{
		return targetVertAngle * 0.2f;
	}

	private float GetRightArmAngle()
	{
		return targetVertAngle;
	}

	public void Die(int dieMan)
	{
		setFever(isOn: false);
		base.transform.gameObject.layer = LayerMask.NameToLayer("DeadMan");
		dead = true;
		SendMessage("OnDeath", dieMan);
		SendMessage("OnFeel", FacialExpressor.EXPRESSION.DEATH);
		string[] array = new string[3]
		{
			"die",
			"die_1",
			"die_2"
		};
		if (null != bipAnimation)
		{
			bipAnimation.CrossFade(array[Random.Range(0, 3)]);
		}
		if (null != base.animation && base.animation.IsPlaying("StepRun"))
		{
			base.animation.Stop("StepRun");
		}
	}

	private void ReinstallWeapon()
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND)
		{
			PlayerProperty component = GetComponent<PlayerProperty>();
			if (!(component == null))
			{
				for (int i = 0; i < component.Desc.Equipment.Length; i++)
				{
					TItem tItem = TItemManager.Instance.Get<TItem>(component.Desc.Equipment[i]);
					if (tItem != null && tItem.type == TItem.TYPE.WEAPON)
					{
						string itemCode = component.Desc.Equipment[i];
						TWeapon tWeapon = (TWeapon)tItem;
						int num = 0;
						while (component.Desc.WpnChg != null && num < component.Desc.WpnChg.Length)
						{
							TItem tItem2 = TItemManager.Instance.Get<TItem>(component.Desc.WpnChg[num]);
							if (tItem2 != null && tItem2.type == TItem.TYPE.WEAPON)
							{
								TWeapon tWeapon2 = (TWeapon)tItem2;
								if (tWeapon.slot == tWeapon2.slot)
								{
									itemCode = component.Desc.WpnChg[num];
									break;
								}
							}
							num++;
						}
						if (tWeapon.GetWeaponType() != Weapon.TYPE.PROJECTILE)
						{
							GetComponent<LookCoordinator>().UnequipWeaponBySlot((int)tWeapon.GetWeaponType());
							GetComponent<LookCoordinator>().Equip(itemCode);
						}
					}
				}
				GlobalVars.Instance.SetOullineColor(component.Desc.Seq);
			}
		}
	}

	public void Respawn()
	{
		base.transform.gameObject.layer = LayerMask.NameToLayer("Player");
		dead = false;
		wasDead = true;
		invincibleWait = 0f;
		SendMessage("OnFeel", FacialExpressor.EXPRESSION.DEFAULT);
		if (null != bipAnimation)
		{
			bipAnimation.CrossFade("a_idle");
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			bungeeRespawn = true;
		}
		if (RoomManager.Instance.DropItem)
		{
			ReinstallWeapon();
		}
		initIdle = false;
	}

	private bool IsVisible(Vector3 pos, int seq)
	{
		int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("BndWall"));
		Vector3 normalized = (pos - maincam.transform.position).normalized;
		Ray ray = new Ray(maincam.transform.position, normalized);
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

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (bHitEvent)
			{
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null != gameObject)
				{
					maincam = gameObject.GetComponent<Camera>();
				}
				if (maincam != null)
				{
					dtHitEvent += Time.deltaTime;
					Vector3 position = base.transform.position;
					position.y += 2f;
					Vector3 position2 = base.transform.position;
					position2.y += 1f;
					PlayerProperty component = GetComponent<PlayerProperty>();
					if (null != component && component.IsHostile() && !component.Desc.IsHidePlayer)
					{
						Vector3 vector = maincam.WorldToScreenPoint(position);
						TextureUtil.DrawTexture(new Rect(vector.x - 40f, (float)Screen.height - vector.y - 60f, 80f, 38f), VersionTextureManager.Instance.buildTexture.hitGaugeBg, ScaleMode.StretchToFill, alphaBlend: true);
						if (BuildOption.Instance.Props.useArmor)
						{
							float armorRatio = component.Desc.ArmorRatio;
							armorRatio *= 50f;
							GUI.BeginGroup(new Rect(vector.x - 19f, (float)Screen.height - vector.y - 52f, armorRatio, 8f));
							TextureUtil.DrawTexture(new Rect(0f, 0f, 50f, 8f), texArmorBar, ScaleMode.StretchToFill, alphaBlend: true);
							GUI.EndGroup();
						}
						float hpRatio = component.Desc.HpRatio;
						hpRatio *= 50f;
						GUI.BeginGroup(new Rect(vector.x - 19f, (float)Screen.height - vector.y - 40f, hpRatio, 8f));
						TextureUtil.DrawTexture(new Rect(0f, 0f, 50f, 8f), texXpBar, ScaleMode.StretchToFill, alphaBlend: true);
						GUI.EndGroup();
					}
					if (dtHitEvent >= 2f)
					{
						bHitEvent = false;
					}
				}
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	public void GetHit(int dmg, int hitman)
	{
		if (!(transformHead == null))
		{
			if (!dead)
			{
				SendMessage("OnHitByUnknown", hitman);
				SendMessage("OnFeel", FacialExpressor.EXPRESSION.CRY);
			}
			if (targetVertAngle < 0f)
			{
				brokenAngle = 30f;
			}
			else
			{
				brokenAngle = -30f;
			}
			if (hitEff != null)
			{
				Object.Instantiate((Object)hitEff, transformHead.position, Quaternion.Euler(0f, 0f, 0f));
			}
			if (dmg > 0)
			{
				PlayerProperty component = GetComponent<PlayerProperty>();
				if (component != null)
				{
					component.Desc.Hp -= dmg;
					if (component.Desc.Hp < 0)
					{
						component.Desc.Hp = 0;
					}
				}
			}
		}
	}

	public void UnInvincible()
	{
		InvincibleArmor component = GetComponent<InvincibleArmor>();
		if (null != component)
		{
			component.Destroy();
		}
	}

	private void LateUpdate()
	{
		if (!isLocallyControlled)
		{
			if (!dead && headBone != null && spineBone != null && rightUpperArm != null)
			{
				float headAngle = GetHeadAngle();
				float spineAngle = GetSpineAngle();
				float rightArmAngle = GetRightArmAngle();
				float y = 0f;
				if (isZombie)
				{
					y = ((!zombieAngleDir) ? Mathf.Lerp(zombieAngleMax, zombieAngleMin, deltaTimeZombieAngle / limitTimeZombieAngle) : Mathf.Lerp(zombieAngleMin, zombieAngleMax, deltaTimeZombieAngle / limitTimeZombieAngle));
				}
				headBone.localRotation = Quaternion.Euler(0f, y, headAngle) * headBone.localRotation;
				spineBone.localRotation = Quaternion.Euler(0f, 0f, spineAngle) * spineBone.localRotation;
				rightUpperArm.localRotation = Quaternion.Euler(rightArmAngle, 0f, 0f) * rightUpperArm.localRotation;
				leftUpperArm.localRotation = Quaternion.Euler(0f - rightArmAngle, 0f, 0f) * leftUpperArm.localRotation;
			}
			if (wasDead)
			{
				invincibleWait += Time.deltaTime;
				if (invincibleWait > 0.2f)
				{
					wasDead = false;
					InvincibleArmor component = GetComponent<InvincibleArmor>();
					if (null != component)
					{
						component.Enable();
					}
				}
			}
		}
	}

	private void ApplyBungeeRespawnFallingEffect()
	{
		if (vertSpeed < 0f && IsGrounded())
		{
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

	private void VerifyZombieFxInst()
	{
		if (null == zombieFxInst && null != zombieFx && null != spineBone)
		{
			GameObject gameObject = Object.Instantiate((Object)zombieFx) as GameObject;
			gameObject.transform.position = spineBone.position;
			gameObject.transform.parent = spineBone;
			gameObject.transform.rotation = Quaternion.identity;
			zombieFxInst = gameObject.transform;
		}
	}

	private void BeginZombie()
	{
		if (null != beginZombieFx && null != spineBone)
		{
			GameObject gameObject = Object.Instantiate((Object)beginZombieFx) as GameObject;
			gameObject.transform.position = spineBone.position;
			gameObject.transform.parent = spineBone;
			gameObject.transform.rotation = Quaternion.identity;
		}
		SendMessage("OnFeel", FacialExpressor.EXPRESSION.ANGRY);
	}

	private void Growling()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component)
		{
			component.clip = growling;
			component.loop = false;
			component.Play();
		}
	}

	private void ApplyZombieEffect()
	{
		PlayerProperty component = GetComponent<PlayerProperty>();
		if (!(component == null) && RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE)
		{
			bool flag = isZombie;
			isZombie = ZombieVsHumanManager.Instance.IsZombie(component.Desc.Seq);
			if (isZombie)
			{
				deltaTimeZombieAngle += Time.deltaTime;
				if (deltaTimeZombieAngle >= limitTimeZombieAngle)
				{
					Growling();
					zombieAngleDir = !zombieAngleDir;
					deltaTimeZombieAngle = 0f;
					limitTimeZombieAngle = Random.Range(6f, 9f);
				}
			}
			VerifyZombieFxInst();
			if (null != zombieFxInst)
			{
				if (isZombie)
				{
					if (!zombieFxInst.gameObject.activeInHierarchy)
					{
						zombieFxInst.gameObject.SetActive(value: true);
					}
				}
				else if (zombieFxInst.gameObject.activeInHierarchy)
				{
					zombieFxInst.gameObject.SetActive(value: false);
				}
			}
			if (!flag && isZombie)
			{
				BeginZombie();
				ChangeBodyColor(zombieColor);
			}
			else if (flag && !isZombie)
			{
				ChangeBodyColor(orgColor);
			}
		}
	}

	private void ChangeBodyColor(Color color)
	{
		if (null != mainBody)
		{
			SkinnedMeshRenderer[] componentsInChildren = mainBody.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.material.color = color;
			}
			MeshRenderer[] componentsInChildren2 = mainBody.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				meshRenderer.material.color = color;
			}
		}
	}

	private void VerifyWantedFxInst()
	{
		if (null == wantedFxInst && null != wantedFx && null != spineBone)
		{
			GameObject gameObject = Object.Instantiate((Object)wantedFx) as GameObject;
			gameObject.transform.position = spineBone.position;
			gameObject.transform.parent = spineBone;
			gameObject.transform.rotation = Quaternion.identity;
			wantedFxInst = gameObject.transform;
		}
	}

	private void ApplyWantedEffect()
	{
		PlayerProperty component = GetComponent<PlayerProperty>();
		if (!(component == null) && (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.TEAM_MATCH || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.INDIVIDUAL))
		{
			VerifyWantedFxInst();
			bool flag = WantedManager.Instance.IsWanted(component.Desc.Seq);
			if (null != wantedFxInst)
			{
				if (flag)
				{
					if (!wantedFxInst.gameObject.activeInHierarchy)
					{
						wantedFxInst.gameObject.SetActive(value: true);
					}
				}
				else if (wantedFxInst.gameObject.activeInHierarchy)
				{
					wantedFxInst.gameObject.SetActive(value: false);
				}
			}
		}
	}

	private bool IsGrounded()
	{
		if (null != characterController)
		{
			return characterController.isGrounded;
		}
		return false;
	}

	private void GunFeverEff(bool isOn)
	{
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (!(null == component))
		{
			component.EnableBrickComposerFever(isOn);
		}
	}

	public void setFever(bool isOn)
	{
		if (isOn)
		{
			isFever = true;
			Object.Instantiate((Object)GlobalVars.Instance.fxFeverOn, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		else if (objFever != null)
		{
			GunFeverEff(isOn: false);
			Object.Destroy(objFever);
			objFever = null;
		}
	}

	private void checkFever()
	{
		if (isFever)
		{
			ElapsedFever += Time.deltaTime;
			if (ElapsedFever > 0.3f)
			{
				isFever = false;
				ElapsedFever = 0f;
				if (objFever == null)
				{
					objFever = (Object.Instantiate((Object)GlobalVars.Instance.fxFeverChar, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
					GunFeverEff(isOn: true);
				}
			}
		}
	}

	private void updateFever()
	{
		if (objFever != null)
		{
			objFever.transform.position = base.transform.position;
			objFever.transform.rotation = base.transform.rotation;
		}
	}

	public void ToIdleOrRun()
	{
		if (controlContext == LocalController.CONTROL_CONTEXT.RUN || controlContext == LocalController.CONTROL_CONTEXT.SQUATTED_WALK)
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
		bool flag = false;
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (component != null)
		{
			flag = component.IsTwoHands();
		}
		if (controlContext == LocalController.CONTROL_CONTEXT.SQUATTED)
		{
			if (!flag)
			{
				bipAnimation.CrossFade("1a_down_idle");
			}
			else
			{
				bipAnimation.CrossFade("2a_down_idle");
			}
		}
		else if (!flag)
		{
			bipAnimation.CrossFade("a_idle");
		}
		else
		{
			bipAnimation.CrossFade("2a_idle");
		}
	}

	public void ToRun()
	{
		bool flag = false;
		LookCoordinator component = GetComponent<LookCoordinator>();
		if (component != null)
		{
			flag = component.IsTwoHands();
		}
		if (controlContext == LocalController.CONTROL_CONTEXT.SQUATTED_WALK)
		{
			if (!flag)
			{
				bipAnimation.CrossFade("1a_down_walk");
			}
			else
			{
				bipAnimation.CrossFade("2a_down_walk");
			}
		}
		else if (!flag)
		{
			bipAnimation.CrossFade("a_run");
		}
		else
		{
			bipAnimation.CrossFade("2a_run_skirt");
		}
	}
}
