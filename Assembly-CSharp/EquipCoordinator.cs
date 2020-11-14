using UnityEngine;

public class EquipCoordinator : MonoBehaviour
{
	private const float WEAPON_RADIUS = 0.25f;

	private const float INVALID = -1000000f;

	public string rightHandBone = "Main Camera/fps_man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/dummy_weapon_r";

	private Transform rightHand;

	private string leftHandBone = "Main Camera/fps_man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand/dummy_weapon_l";

	private Transform leftHand;

	public string firstPersonBody = "Main Camera/fps_man";

	private Transform firstPerson;

	public Vector3 firstPersonOffsetOnEmptyHand = new Vector3(0f, -1.36f, 0.55f);

	private GameObject[] equipments;

	private int prevWeaponType = -1;

	private int currentWeaponType;

	private float deltaTime;

	private GameObject wingEffect;

	private int wingEffectMode;

	private GameObject WeaponLeft;

	private float dropItemUpOffset = 0.2f;

	public int PrevWeapon => prevWeaponType;

	public int CurrentWeapon => currentWeaponType;

	public void Reinit(int[] usables)
	{
		deltaTime = 0f;
		ClearAllWeapons();
		for (int i = 0; i < usables.Length; i++)
		{
			string empty = string.Empty;
			if (usables[i] != 4)
			{
				TItem.SLOT slot = (TItem.SLOT)(2 + usables[i]);
				empty = MyInfoManager.Instance.GetUsingBySlot(slot);
			}
			else
			{
				empty = RoomManager.Instance.ModeSpecificWeaponCode;
			}
			equipments[usables[i]] = null;
			if (empty.Length > 0)
			{
				equipments[usables[i]] = Equip(empty);
			}
		}
		prevWeaponType = -1;
		currentWeaponType = (int)RoomManager.Instance.DefaultWeaponType;
		VerifyNextWeapon();
		WeaponChanger component = GetComponent<WeaponChanger>();
		if (null != component)
		{
			component.Initialize(equipments);
		}
		SwapWeapon(initiallyDrawn: true);
		disableWingEffect();
	}

	public void ResetWeaponOnly(int[] usables)
	{
		deltaTime = 0f;
		ClearAllWeapons();
		for (int i = 0; i < usables.Length; i++)
		{
			string empty = string.Empty;
			if (usables[i] != 4)
			{
				TItem.SLOT slot = (TItem.SLOT)(2 + usables[i]);
				Item currentWeaponBySlot = MyInfoManager.Instance.GetCurrentWeaponBySlot(slot);
				empty = currentWeaponBySlot.Code;
			}
			else
			{
				empty = RoomManager.Instance.ModeSpecificWeaponCode;
			}
			equipments[usables[i]] = null;
			if (empty.Length > 0)
			{
				equipments[usables[i]] = Equip(empty);
			}
		}
		prevWeaponType = -1;
		currentWeaponType = (int)RoomManager.Instance.DefaultWeaponType;
		VerifyNextWeapon();
		WeaponChanger component = GetComponent<WeaponChanger>();
		if (null != component)
		{
			component.Initialize(equipments);
		}
		SwapWeapon(initiallyDrawn: true);
	}

	public void WeaponBack(int[] usables)
	{
		deltaTime = 0f;
		ClearAllWeaponsSlow();
		for (int i = 0; i < usables.Length; i++)
		{
			string empty = string.Empty;
			if (usables[i] != 4)
			{
				TItem.SLOT slot = (TItem.SLOT)(2 + usables[i]);
				Item currentWeaponBySlot = MyInfoManager.Instance.GetCurrentWeaponBySlot(slot);
				empty = currentWeaponBySlot.Code;
			}
			else
			{
				empty = RoomManager.Instance.ModeSpecificWeaponCode;
			}
			equipments[usables[i]] = null;
			if (empty.Length > 0)
			{
				equipments[usables[i]] = Equip(empty);
			}
		}
		prevWeaponType = -1;
		currentWeaponType = (int)RoomManager.Instance.DefaultWeaponType;
		VerifyNextWeapon();
		WeaponChanger component = GetComponent<WeaponChanger>();
		if (null != component)
		{
			component.Initialize(equipments);
		}
		SwapWeapon(initiallyDrawn: true);
	}

	public void catchBuildGun(int[] usables)
	{
		deltaTime = 0f;
		ClearAllWeapons();
		for (int i = 0; i < usables.Length; i++)
		{
			string empty = string.Empty;
			if (usables[i] != 4)
			{
				TItem.SLOT slot = (TItem.SLOT)(2 + usables[i]);
				empty = MyInfoManager.Instance.GetUsingBySlot(slot);
			}
			else
			{
				empty = "Composer3";
			}
			equipments[usables[i]] = null;
			if (empty.Length > 0)
			{
				equipments[usables[i]] = Equip(empty);
			}
		}
		prevWeaponType = -1;
		currentWeaponType = 4;
		VerifyNextWeapon();
		WeaponChanger component = GetComponent<WeaponChanger>();
		if (null != component)
		{
			component.Initialize(equipments);
		}
		SwapWeapon(initiallyDrawn: true);
	}

	public void Initialize(int[] usables)
	{
		deltaTime = 0f;
		HideDummyMesh();
		firstPerson = base.transform.Find(firstPersonBody);
		if (null == firstPerson)
		{
			Debug.LogError("Fail to find first person body ");
		}
		else
		{
			firstPerson.localPosition = firstPersonOffsetOnEmptyHand;
		}
		rightHand = base.transform.Find(rightHandBone);
		if (null == rightHand)
		{
			Debug.LogError("Fail to find right hand bone");
		}
		leftHand = base.transform.Find(leftHandBone);
		if (null == leftHand)
		{
			Debug.LogError("Fail to find left hand bone");
		}
		equipments = new GameObject[5];
		for (int i = 0; i < 5; i++)
		{
			equipments[i] = null;
		}
		for (int j = 0; j < usables.Length; j++)
		{
			string empty = string.Empty;
			if (usables[j] != 4)
			{
				TItem.SLOT slot = (TItem.SLOT)(2 + usables[j]);
				empty = MyInfoManager.Instance.GetUsingBySlot(slot);
			}
			else
			{
				empty = RoomManager.Instance.ModeSpecificWeaponCode;
			}
			equipments[usables[j]] = null;
			if (empty.Length > 0)
			{
				equipments[usables[j]] = Equip(empty);
			}
		}
		prevWeaponType = -1;
		currentWeaponType = (int)RoomManager.Instance.DefaultWeaponType;
		VerifyNextWeapon();
		WeaponChanger component = GetComponent<WeaponChanger>();
		if (null != component)
		{
			component.Initialize(equipments);
		}
		SwapWeapon(initiallyDrawn: true);
		disableWingEffect();
	}

	private void VerifyNextWeapon()
	{
		if (equipments[currentWeaponType] == null)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < equipments.Length)
			{
				if (equipments[num] != null)
				{
					currentWeaponType = num;
					flag = true;
				}
				num++;
			}
		}
	}

	private bool FillAmmo(Weapon.TYPE weaponType, TWeapon tWeapon)
	{
		if (tWeapon == null || equipments[(int)weaponType] == null)
		{
			return false;
		}
		if (null == tWeapon.CurPrefab())
		{
			return false;
		}
		WeaponFunction component = tWeapon.CurPrefab().GetComponent<WeaponFunction>();
		WeaponFunction component2 = equipments[(int)weaponType].GetComponent<WeaponFunction>();
		if (component.weaponBy != component2.weaponBy || component2.IsFullAmmo())
		{
			return false;
		}
		component2.Reset();
		return true;
	}

	public void ExpandAmmo()
	{
		for (int i = 0; i < equipments.Length; i++)
		{
			if (null != equipments[i])
			{
				Gun component = equipments[i].GetComponent<Gun>();
				if (component != null)
				{
					component.ExpandAmmo();
				}
			}
		}
	}

	public void AddAmmo(Weapon.TYPE weaponType, float percent)
	{
		if (!(equipments[(int)weaponType] == null))
		{
			Gun component = equipments[(int)weaponType].GetComponent<Gun>();
			if (component != null)
			{
				component.AddAmmo(percent);
			}
		}
	}

	public bool PickupFromTemplate(string weaponCode)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(weaponCode);
		if (tWeapon == null)
		{
			return false;
		}
		Weapon.TYPE weaponType = tWeapon.GetWeaponType();
		if (!IsEmptyHand(weaponType))
		{
			return FillAmmo(weaponType, tWeapon);
		}
		equipments[(int)weaponType] = Equip(weaponCode);
		if (null == equipments[(int)weaponType])
		{
			return false;
		}
		prevWeaponType = -1;
		currentWeaponType = (int)weaponType;
		return true;
	}

	public bool PickupFromFromInstance(long itemSeq)
	{
		Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(itemSeq);
		if (itemBySequence == null)
		{
			return false;
		}
		TWeapon tWeapon = (TWeapon)itemBySequence.Template;
		if (tWeapon == null)
		{
			return false;
		}
		Weapon.TYPE weaponType = tWeapon.GetWeaponType();
		string code = tWeapon.code;
		HideWeapon(equipments[(int)weaponType]);
		Object.DestroyImmediate(equipments[(int)weaponType]);
		if (weaponType == Weapon.TYPE.MAIN && WeaponLeft != null)
		{
			Object.DestroyImmediate(WeaponLeft);
			WeaponLeft = null;
		}
		GameObject gameObject = Equip(code);
		if (null == gameObject)
		{
			return false;
		}
		WeaponFunction component = gameObject.GetComponent<WeaponFunction>();
		if (null == component)
		{
			return false;
		}
		component.ItemSeq = itemSeq;
		equipments[(int)weaponType] = gameObject;
		prevWeaponType = -1;
		currentWeaponType = (int)weaponType;
		WeaponChanger component2 = GetComponent<WeaponChanger>();
		if (null != component2)
		{
			component2.Initialize(equipments);
		}
		return true;
	}

	public void Throw()
	{
		DestroyCurrentWeapon();
		firstPerson.transform.localPosition = firstPersonOffsetOnEmptyHand;
	}

	public void ThrowAll()
	{
		for (int i = 0; i < equipments.Length; i++)
		{
			if (null != equipments[i])
			{
				HideWeapon(equipments[i]);
				Object.DestroyObject(equipments[i], 1f);
				equipments[i] = null;
			}
		}
		firstPerson.transform.localPosition = firstPersonOffsetOnEmptyHand;
	}

	private void HideWeapon(GameObject weapon)
	{
		if (!(weapon == null))
		{
			SkinnedMeshRenderer[] componentsInChildren = weapon.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.enabled = false;
			}
			MeshRenderer[] componentsInChildren2 = weapon.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				meshRenderer.enabled = false;
			}
		}
	}

	private void ClearAllWeapons()
	{
		Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			equipments[(int)componentsInChildren[i].slot] = null;
			Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
	}

	private void ClearAllWeaponsSlow()
	{
		Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].slot != Weapon.TYPE.PROJECTILE)
			{
				equipments[(int)componentsInChildren[i].slot] = null;
				Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
	}

	private void DestroyCurrentWeapon()
	{
		Weapon componentInChildren = rightHand.GetComponentInChildren<Weapon>();
		if (null != componentInChildren)
		{
			equipments[(int)componentInChildren.slot] = null;
			Object.DestroyImmediate(componentInChildren.gameObject);
			if (currentWeaponType == 2 && WeaponLeft != null)
			{
				Object.DestroyImmediate(WeaponLeft);
			}
		}
	}

	public bool IsEmptyHand(Weapon.TYPE weaponType)
	{
		return equipments[(int)weaponType] == null;
	}

	public void SwapWeapon(bool initiallyDrawn)
	{
		if (prevWeaponType != currentWeaponType)
		{
			GameObject gameObject = null;
			if (0 <= currentWeaponType && currentWeaponType < equipments.Length)
			{
				gameObject = equipments[currentWeaponType];
			}
			if (gameObject == null)
			{
				currentWeaponType = prevWeaponType;
			}
			else
			{
				for (int i = 0; i < equipments.Length; i++)
				{
					if (equipments[i] != null && equipments[i] != gameObject)
					{
						equipments[i].SetActive(value: false);
						if (WeaponLeft != null)
						{
							WeaponLeft.SetActive(value: false);
						}
					}
				}
				gameObject.SetActive(value: true);
				if (currentWeaponType == 2 && WeaponLeft != null)
				{
					WeaponLeft.SetActive(value: true);
				}
				if (currentWeaponType != 2 || WeaponLeft == null)
				{
					firstPerson.localPosition = gameObject.GetComponent<WeaponFunction>().firstPersonOffset;
				}
				else
				{
					firstPerson.localPosition = new Vector3(0f, -1.36f, 0.55f);
				}
				if (!initiallyDrawn)
				{
					gameObject.GetComponent<Weapon>().GadgetSound(Weapon.GADGET.BOLTUP);
				}
				gameObject.GetComponent<WeaponFunction>().SetDrawn(initiallyDrawn);
				if (currentWeaponType == 2 && WeaponLeft != null)
				{
					WeaponLeft.GetComponent<WeaponFunction>().SetDrawn(initiallyDrawn);
				}
				AudioSource componentInChildren = gameObject.GetComponentInChildren<AudioSource>();
				if (componentInChildren != null)
				{
					gameObject.BroadcastMessage("OnChangeAudioSource");
					if (currentWeaponType == 2 && WeaponLeft != null)
					{
						WeaponLeft.BroadcastMessage("OnChangeAudioSource");
					}
				}
			}
		}
	}

	public void SetActiveCurrentWeapon(bool state)
	{
		GameObject gameObject = null;
		if (0 <= currentWeaponType && currentWeaponType < equipments.Length)
		{
			gameObject = equipments[currentWeaponType];
		}
		if (gameObject != null)
		{
			gameObject.SetActive(state);
		}
		if (WeaponLeft != null)
		{
			WeaponLeft.SetActive(state);
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Attach(Transform child, Transform parent, Quaternion childRotation)
	{
		child.position = parent.position;
		child.rotation = parent.rotation;
		child.parent = parent;
		child.localRotation = childRotation;
		child.localScale = new Vector3(1f, 1f, 1f);
	}

	private GameObject AttachWeapon(GameObject weaponPrefab, TWeapon tItem)
	{
		if (null == weaponPrefab || null == rightHand)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate((Object)weaponPrefab) as GameObject;
		gameObject.GetComponent<Weapon>().tItem = tItem;
		gameObject.GetComponent<WeaponFunction>().enabled = true;
		gameObject.GetComponent<WeaponFunction>().SetDrawn(draw: false);
		if (tItem != null && tItem.IsTwoHands)
		{
			WeaponLeft = (Object.Instantiate((Object)weaponPrefab) as GameObject);
			WeaponLeft.GetComponent<Weapon>().tItem = tItem;
			WeaponLeft.GetComponent<WeaponFunction>().enabled = true;
			WeaponLeft.GetComponent<WeaponFunction>().SetDrawn(draw: false);
			WeaponLeft.GetComponent<Gun>().LeftHand = true;
			WeaponLeft.GetComponent<Gun>().TwoHands = true;
		}
		if (RoomManager.Instance.CurrentRoomType != 0 && tItem != null)
		{
			gameObject.GetComponent<WeaponFunction>().category = tItem.cat;
			if (WeaponLeft != null)
			{
				WeaponLeft.GetComponent<WeaponFunction>().category = tItem.cat;
			}
		}
		Recursively.SetLayer(gameObject.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
		if (WeaponLeft != null)
		{
			Recursively.SetLayer(WeaponLeft.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
		}
		Attach(gameObject.transform, rightHand, Quaternion.Euler(0f, 90f, 90f));
		if (WeaponLeft != null)
		{
			Attach(WeaponLeft.transform, leftHand, Quaternion.Euler(0f, 90f, 90f));
		}
		gameObject.SetActive(value: false);
		if (WeaponLeft != null)
		{
			WeaponLeft.SetActive(value: false);
		}
		return gameObject;
	}

	private GameObject Equip(string itemCode)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(itemCode);
		if (tWeapon != null)
		{
			return AttachWeapon(tWeapon.CurPrefab(), tWeapon);
		}
		GameObject gameObject = TItemManager.Instance.FindPrefab(itemCode);
		if (null != gameObject)
		{
			return AttachWeapon(gameObject, null);
		}
		return null;
	}

	private void HideDummyMesh()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = false;
		}
	}

	public void DeleteClcokBomb()
	{
		if (Application.loadedLevelName.Contains("Explosion") && currentWeaponType == 4)
		{
			SetActiveCurrentWeapon(state: false);
		}
	}

	public bool SetCurrent(int slot)
	{
		if (slot < 0 || slot >= equipments.Length)
		{
			return false;
		}
		if (equipments[slot] == null)
		{
			return false;
		}
		WeaponFunction component = equipments[slot].GetComponent<WeaponFunction>();
		if (null == component)
		{
			return false;
		}
		if (slot == currentWeaponType)
		{
			return false;
		}
		if (component.weaponBy == Weapon.BY.CLOCKBOMB && !MyInfoManager.Instance.AmIBlasting())
		{
			return false;
		}
		prevWeaponType = currentWeaponType;
		currentWeaponType = slot;
		WeaponChanger component2 = GetComponent<WeaponChanger>();
		if (null != component2)
		{
			component2.Swap();
		}
		return true;
	}

	public bool SetPrev()
	{
		return SetCurrent(prevWeaponType);
	}

	public void OnRespawn(Quaternion rotation)
	{
		WeaponFunction[] componentsInChildren = GetComponentsInChildren<WeaponFunction>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			bool bDefenseRespwan = false;
			componentsInChildren[i].Reset(bDefenseRespwan);
		}
	}

	private void collideSenseBomb()
	{
		for (int i = 0; i < equipments.Length; i++)
		{
			if (null != equipments[i])
			{
				SenseBomb component = equipments[i].GetComponent<SenseBomb>();
				if (component != null)
				{
					component.collideTest();
				}
				Gun component2 = equipments[i].GetComponent<Gun>();
				if (component2 != null)
				{
					component2.collisionTest1st();
					component2.collsionTest2nd();
				}
				XmasBomb component3 = equipments[i].GetComponent<XmasBomb>();
				if (!(component3 != null))
				{
				}
			}
		}
	}

	public void SelfKaboomSenseBomb(int bombId)
	{
		if (MyInfoManager.Instance.SenseBombSeq == bombId)
		{
			for (int i = 0; i < equipments.Length; i++)
			{
				if (null != equipments[i])
				{
					SenseBomb component = equipments[i].GetComponent<SenseBomb>();
					if (component != null)
					{
						component.SelfKaboom();
					}
				}
			}
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f)
		{
			deltaTime = 0f;
			if (currentWeaponType != 4)
			{
				P2PManager.Instance.SendPEER_WEAPON_STATUS(MyInfoManager.Instance.Seq, currentWeaponType);
			}
		}
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			collideSenseBomb();
		}
	}

	public void SetDetonating(bool set)
	{
		if (!(rightHand == null))
		{
			Grenade componentInChildren = rightHand.GetComponentInChildren<Grenade>();
			if (componentInChildren != null)
			{
				componentInChildren.SetDetonating(set);
			}
		}
	}

	public void ScopeOff()
	{
		if (!(rightHand == null))
		{
			Gun componentInChildren = rightHand.GetComponentInChildren<Gun>();
			if (componentInChildren != null)
			{
				componentInChildren.scopeOff();
			}
		}
	}

	public void SetShootEnermyEffect()
	{
		Aim componentInChildren = GetComponentInChildren<Aim>();
		if (null != componentInChildren)
		{
			componentInChildren.SetShootEnermyEffect();
		}
		else
		{
			MeleeWeapon componentInChildren2 = rightHand.GetComponentInChildren<MeleeWeapon>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SetShootEnermyEffect();
			}
			else
			{
				Grenade componentInChildren3 = rightHand.GetComponentInChildren<Grenade>();
				if (componentInChildren3 != null)
				{
					componentInChildren3.SetShootEnermyEffect();
				}
				else
				{
					FlashBang componentInChildren4 = rightHand.GetComponentInChildren<FlashBang>();
					if (componentInChildren4 != null)
					{
						componentInChildren4.SetShootEnermyEffect();
					}
					else
					{
						SmokeGrenade componentInChildren5 = rightHand.GetComponentInChildren<SmokeGrenade>();
						if (componentInChildren5 != null)
						{
							componentInChildren5.SetShootEnermyEffect();
						}
						else
						{
							SenseBomb componentInChildren6 = rightHand.GetComponentInChildren<SenseBomb>();
							if (componentInChildren6 != null)
							{
								componentInChildren6.SetShootEnermyEffect();
							}
							else
							{
								XmasBomb componentInChildren7 = rightHand.GetComponentInChildren<XmasBomb>();
								if (componentInChildren7 != null)
								{
									componentInChildren7.SetShootEnermyEffect();
								}
								else
								{
									PoisonBomb componentInChildren8 = rightHand.GetComponentInChildren<PoisonBomb>();
									if (componentInChildren8 != null)
									{
										componentInChildren8.SetShootEnermyEffect();
									}
									else
									{
										GetComponent<LocalController>().SetShootEnermyEffect();
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void EquipSmokeBomb()
	{
		deltaTime = 0f;
		if (equipments[3] == null)
		{
			equipments[3] = Equip("kgs440");
		}
		else
		{
			WeaponFunction component = equipments[3].GetComponent<WeaponFunction>();
			component.Reset();
		}
		SetCurrent(3);
	}

	public void ReturnBuildGun()
	{
		deltaTime = 0f;
		SetCurrent(4);
		if (GlobalVars.Instance.StateFever <= 0)
		{
			WeaponFunction component = equipments[4].GetComponent<WeaponFunction>();
			if (null != component)
			{
				P2PManager.Instance.SendPEER_STATE_FEVER(isOn: false);
				component.setFever(isOn: false);
			}
		}
	}

	public void enableWingEffect(bool enable, AngelWingItem item)
	{
		if (enable)
		{
			wingEffectMode++;
		}
		else
		{
			wingEffectMode--;
		}
		if (wingEffectMode > 0)
		{
			if (wingEffect == null)
			{
				Transform transform = base.transform.Find(item.firstPersonAttachBone);
				if (null == transform)
				{
					Debug.LogError("Fail to find the bone " + item.firstPersonAttachBone);
				}
				else
				{
					wingEffect = (Object.Instantiate((Object)item.wingEffect) as GameObject);
					Attach(wingEffect.transform, transform, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			MyInfoManager.Instance.BungeeFlyModeByItem(flyMode: true);
		}
		else
		{
			disableWingEffect();
		}
	}

	private void disableWingEffect()
	{
		wingEffectMode = 0;
		if (wingEffect != null)
		{
			Object.DestroyImmediate(wingEffect);
			wingEffect = null;
		}
		MyInfoManager.Instance.BungeeFlyModeByItem(flyMode: false);
	}

	public void EquipBrickBoom()
	{
		deltaTime = 0f;
		if (equipments[2] != null)
		{
			Object.DestroyImmediate(equipments[2]);
		}
		equipments[2] = Equip("BrickGun03");
		WeaponFunction component = equipments[2].GetComponent<WeaponFunction>();
		component.Reset();
		SetCurrent(2);
	}

	public Weapon.BY GetCurrentWeaponBy()
	{
		WeaponFunction component = equipments[CurrentWeapon].GetComponent<WeaponFunction>();
		return component.weaponBy;
	}

	public bool IsTwoHands()
	{
		if (CurrentWeapon < 0)
		{
			return false;
		}
		if (equipments[CurrentWeapon] == null)
		{
			return false;
		}
		Weapon component = equipments[CurrentWeapon].GetComponent<Weapon>();
		if (component != null)
		{
			TWeapon tWeapon = (TWeapon)component.tItem;
			if (tWeapon != null)
			{
				return tWeapon.IsTwoHands;
			}
		}
		return false;
	}

	public GameObject currentHaveWeapon(int dropWeaponType)
	{
		if (dropWeaponType >= 0)
		{
			if (dropWeaponType == 4 || dropWeaponType == 3)
			{
				return null;
			}
			Weapon component = equipments[dropWeaponType].GetComponent<Weapon>();
			if (null != component)
			{
				if (dropWeaponType != 0)
				{
					equipments[dropWeaponType].SetActive(value: true);
					Gun componentInChildren = equipments[dropWeaponType].GetComponentInChildren<Gun>();
					if (componentInChildren != null)
					{
						if (componentInChildren.magazine.Cur == 0 && componentInChildren.curAmmo == 0)
						{
							return null;
						}
						int droppedAmmo = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, componentInChildren.magazine.Cur) + componentInChildren.curAmmo;
						GlobalVars.Instance.droppedAmmo = droppedAmmo;
						int droppedAmmo2 = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, componentInChildren.maxLauncherAmmoGame);
						GlobalVars.Instance.droppedAmmo2 = droppedAmmo2;
					}
				}
				GlobalVars.Instance.droppedItemSeq = 0;
				GlobalVars.Instance.droppedItemCode = component.tItem.code;
				float weaponDropPosition = GetWeaponDropPosition(base.transform.position);
				if (weaponDropPosition != -1000000f)
				{
					Vector3 position = base.transform.position;
					float x = position.x;
					float y = weaponDropPosition;
					Vector3 position2 = base.transform.position;
					Vector3 pos = new Vector3(x, y, position2.z);
					CSNetManager.Instance.Sock.SendCS_DROP_ITEM_REQ(GlobalVars.Instance.droppedItemCode, GlobalVars.Instance.droppedAmmo, GlobalVars.Instance.droppedAmmo2, pos);
				}
				return equipments[dropWeaponType];
			}
		}
		else
		{
			Weapon componentInChildren2 = rightHand.GetComponentInChildren<Weapon>();
			if (null != componentInChildren2)
			{
				TWeapon tWeapon = componentInChildren2.tItem as TWeapon;
				if (tWeapon == null)
				{
					return null;
				}
				if (tWeapon.GetWeaponType() == Weapon.TYPE.MODE_SPECIFIC || tWeapon.GetWeaponType() == Weapon.TYPE.PROJECTILE)
				{
					return null;
				}
				GlobalVars.Instance.droppedAmmo = 0;
				if (componentInChildren2.slot != 0)
				{
					Gun componentInChildren3 = rightHand.GetComponentInChildren<Gun>();
					if (componentInChildren3 != null)
					{
						if (componentInChildren3.magazine.Cur == 0 && componentInChildren3.curAmmo == 0)
						{
							return null;
						}
						int droppedAmmo3 = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, componentInChildren3.magazine.Cur) + componentInChildren3.curAmmo;
						GlobalVars.Instance.droppedAmmo = droppedAmmo3;
						int droppedAmmo4 = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, componentInChildren3.maxLauncherAmmoGame);
						GlobalVars.Instance.droppedAmmo2 = droppedAmmo4;
					}
				}
				GlobalVars.Instance.droppedItemSeq = 0;
				GlobalVars.Instance.droppedItemCode = componentInChildren2.tItem.code;
				float weaponDropPosition2 = GetWeaponDropPosition(base.transform.position);
				if (weaponDropPosition2 != -1000000f)
				{
					Vector3 position3 = base.transform.position;
					float x2 = position3.x;
					float y2 = weaponDropPosition2;
					Vector3 position4 = base.transform.position;
					Vector3 pos2 = new Vector3(x2, y2, position4.z);
					CSNetManager.Instance.Sock.SendCS_DROP_ITEM_REQ(GlobalVars.Instance.droppedItemCode, GlobalVars.Instance.droppedAmmo, GlobalVars.Instance.droppedAmmo2, pos2);
				}
				return componentInChildren2.gameObject;
			}
		}
		return null;
	}

	private float GetWeaponDropPosition(Vector3 curPos)
	{
		curPos.y += 1f;
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Ray ray = new Ray(curPos, Vector3.down);
		if (Physics.SphereCast(ray, 0.25f, out RaycastHit hitInfo, 5000f, layerMask))
		{
			Vector3 point = hitInfo.point;
			return point.y + dropItemUpOffset;
		}
		return -1000000f;
	}

	public void SwapWeapon(long itemSeq, string itemCode, int ammo, int ammo2)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(itemCode);
		if (tWeapon != null)
		{
			int weaponType = (int)tWeapon.GetWeaponType();
			GlobalVars.Instance.DropWeapon(weaponType);
			HideWeapon(equipments[weaponType]);
			Object.DestroyImmediate(equipments[weaponType]);
			equipments[weaponType] = null;
			if (weaponType == 2 && WeaponLeft != null)
			{
				Object.DestroyImmediate(WeaponLeft);
				WeaponLeft = null;
			}
			GameObject gameObject = Equip(itemCode);
			WeaponFunction component = gameObject.GetComponent<WeaponFunction>();
			if (!(null == component))
			{
				component.ItemSeq = itemSeq;
				equipments[weaponType] = gameObject;
				prevWeaponType = -1;
				currentWeaponType = weaponType;
				WeaponChanger component2 = GetComponent<WeaponChanger>();
				if (null != component2)
				{
					component2.Initialize(equipments);
					component2.Swap();
				}
				GetComponent<LocalController>().SwitchWeapon();
				GetComponent<LocalController>().ToIdle();
				Gun component3 = equipments[weaponType].GetComponent<Gun>();
				if (component3 != null)
				{
					component3.PickedAmmo = ammo;
					component3.PickedAmmo2 = ammo2;
				}
			}
		}
	}
}
