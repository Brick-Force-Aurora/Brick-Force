using UnityEngine;

public class LookCoordinator : MonoBehaviour
{
	public enum PARTS
	{
		BODY,
		ARMS,
		FOOT,
		NUM
	}

	private bool initOnce;

	private bool isMirror;

	public Material[] defMat;

	public Material defHeadMat;

	public GameObject[] manBody;

	public GameObject[] womanBody;

	public GameObject[] manRealBody;

	public GameObject[] womanRealBody;

	public Material[] armBands;

	public GameObject defFace;

	public GameObject defHead;

	private string gender = string.Empty;

	private SkinnedMeshRenderer[] looks;

	private Material[] skins;

	private SkinnedMeshRenderer smrFace;

	private SkinnedMeshRenderer smrHead;

	public string rightHandBone = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/dummy_weapon_r";

	private string leftHandBone = "man/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand/dummy_weapon_l";

	private Transform rightHand;

	private Transform leftHand;

	private GameObject modeSpecific;

	private Weapon currentWeapon;

	private Weapon currentWeaponLeft;

	private GameObject wingEffect;

	private int wingEffectMode;

	private GameObject bungeeItemMain;

	private GameObject WeaponLeft;

	private bool testGender;

	public bool IsYang;

	public bool InitOnce => initOnce;

	public Vector3 RightHandPos => rightHand.position;

	public Vector3 LeftHandPos => leftHand.position;

	public bool TestGender
	{
		set
		{
			testGender = value;
		}
	}

	private void HideDummyMesh()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = false;
		}
	}

	public void UnequipAll()
	{
		Equip[] componentsInChildren = GetComponentsInChildren<Equip>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		modeSpecific = null;
		currentWeapon = null;
		disableWingEffect();
	}

	public void Reset()
	{
		isMirror = true;
		rightHand = base.transform.Find(rightHandBone);
		if (null == rightHand)
		{
			Debug.LogError("Fail to fine right hand ");
		}
		leftHand = base.transform.Find(leftHandBone);
		if (null == leftHand)
		{
			Debug.LogError("Fail to fine left hand ");
		}
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			skinnedMeshRenderer.enabled = false;
		}
		HideDummyMesh();
		gender = "m_";
		looks = new SkinnedMeshRenderer[manBody.Length];
		skins = new Material[manBody.Length];
		for (int j = 0; j < manBody.Length; j++)
		{
			looks[j] = manBody[j].GetComponent<SkinnedMeshRenderer>();
			skins[j] = defMat[j];
			looks[j].enabled = true;
			looks[j].material = skins[j];
		}
		InitDefaultHead();
		UnequipAll();
	}

	public void Init(bool mirror)
	{
		if (!initOnce)
		{
			isMirror = mirror;
			rightHand = base.transform.Find(rightHandBone);
			if (null == rightHand)
			{
				Debug.LogError("Fail to fine right hand ");
			}
			leftHand = base.transform.Find(leftHandBone);
			if (null == leftHand)
			{
				Debug.LogError("Fail to fine left hand ");
			}
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.enabled = false;
			}
			HideDummyMesh();
			gender = "m_";
			looks = new SkinnedMeshRenderer[manBody.Length];
			skins = new Material[manBody.Length];
			for (int j = 0; j < manBody.Length; j++)
			{
				looks[j] = manBody[j].GetComponent<SkinnedMeshRenderer>();
				skins[j] = defMat[j];
				looks[j].enabled = true;
				looks[j].material = skins[j];
			}
			InitDefaultHead();
			initOnce = true;
		}
	}

	public void InitDefaultHead()
	{
		smrFace = defFace.GetComponent<SkinnedMeshRenderer>();
		smrFace.enabled = true;
		FacialExpressor component = GetComponent<FacialExpressor>();
		if (null != component)
		{
			component.ChangeFace(smrFace, "default");
		}
		smrHead = defHead.GetComponent<SkinnedMeshRenderer>();
		smrHead.enabled = true;
		smrHead.material = defHeadMat;
	}

	private void Start()
	{
		Init(mirror: true);
	}

	public WeaponGadget GetCurrentWeaponGadget()
	{
		return rightHand.GetComponentInChildren<WeaponGadget>();
	}

	public WeaponGadget GetCurrentLeftWeaponGadget()
	{
		return leftHand.GetComponentInChildren<WeaponGadget>();
	}

	public Weapon GetCurrentWeapon()
	{
		return rightHand.GetComponentInChildren<Weapon>();
	}

	public WeaponFunction GetCurrentWeaponFunction()
	{
		return rightHand.GetComponentInChildren<WeaponFunction>();
	}

	public Weapon GetWeaponBySlot(Weapon.TYPE weaponType)
	{
		Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>();
		foreach (Weapon weapon in componentsInChildren)
		{
			if (weapon.slot == weaponType)
			{
				return weapon;
			}
		}
		return null;
	}

	private void EquipBuildGun(string itemCode)
	{
		string text = string.Empty;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND)
		{
			text = "Composer";
			switch (itemCode)
			{
			case "s08":
				text = "Composer2";
				break;
			case "s09":
				text = "Composer3";
				break;
			case "s92":
				text = "Composer4";
				break;
			}
		}
		else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			text = "BrickGun01";
		}
		if (text.Length != 0)
		{
			GameObject gameObject = TItemManager.Instance.FindPrefab(text);
			if (null != gameObject)
			{
				modeSpecific = (Object.Instantiate((Object)gameObject) as GameObject);
				modeSpecific.GetComponent<Weapon>().tItem = null;
				modeSpecific.GetComponent<WeaponGadget>().enabled = true;
				modeSpecific.GetComponent<WeaponFunction>().enabled = false;
			}
			if (null != modeSpecific)
			{
				modeSpecific.SetActive(value: false);
			}
		}
	}

	private void VerifyClockBomb()
	{
		if (null == modeSpecific && RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
		{
			GameObject gameObject = TItemManager.Instance.FindPrefab("ClockBomb");
			if (null != gameObject)
			{
				modeSpecific = (Object.Instantiate((Object)gameObject) as GameObject);
				modeSpecific.GetComponent<Weapon>().tItem = null;
				modeSpecific.GetComponent<WeaponGadget>().enabled = true;
				modeSpecific.GetComponent<WeaponFunction>().enabled = false;
			}
			if (null != modeSpecific)
			{
				modeSpecific.SetActive(value: false);
			}
		}
	}

	public void EnableHandbomb(bool enable)
	{
		GdgtGrenade[] componentsInChildren = GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			componentsInChildren[0].EnableHandbomb(enable);
		}
		GdgtFlashBang[] componentsInChildren2 = GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
		if (componentsInChildren2.Length > 0)
		{
			componentsInChildren2[0].EnableHandbomb(enable);
		}
		GdgtSenseBomb[] componentsInChildren3 = GetComponentsInChildren<GdgtSenseBomb>(includeInactive: true);
		if (componentsInChildren3.Length > 0)
		{
			componentsInChildren3[0].EnableHandbomb(enable);
		}
		GdgtXmasBomb[] componentsInChildren4 = GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
		if (componentsInChildren4.Length > 0)
		{
			componentsInChildren4[0].EnableHandbomb(enable);
		}
	}

	public void ChangeWeapon(Weapon.TYPE weaponType)
	{
		if (!(null == rightHand) && (!(currentWeapon != null) || currentWeapon.slot != weaponType))
		{
			Weapon weapon = null;
			if (weaponType == Weapon.TYPE.MODE_SPECIFIC)
			{
				VerifyClockBomb();
				if (modeSpecific != null)
				{
					weapon = modeSpecific.GetComponent<Weapon>();
					modeSpecific.SetActive(value: true);
				}
			}
			else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE && weaponType == Weapon.TYPE.MAIN && bungeeItemMain != null)
			{
				weapon = bungeeItemMain.GetComponent<Weapon>();
				bungeeItemMain.SetActive(value: true);
			}
			else
			{
				Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>(includeInactive: true);
				int num = 0;
				while (weapon == null && num < componentsInChildren.Length)
				{
					if (componentsInChildren[num].slot == weaponType && componentsInChildren[num].gameObject.layer == 0)
					{
						weapon = componentsInChildren[num];
					}
					num++;
				}
			}
			SetActiveCurrentWeapon(state: true);
			Weapon componentInChildren = rightHand.GetComponentInChildren<Weapon>();
			if (null != componentInChildren)
			{
				if (componentInChildren.slot == weaponType)
				{
					return;
				}
				if (componentInChildren.slot == Weapon.TYPE.MODE_SPECIFIC)
				{
					modeSpecific.SetActive(value: false);
					modeSpecific.transform.parent = null;
				}
				else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE && componentInChildren.slot == Weapon.TYPE.MAIN && bungeeItemMain != null)
				{
					bungeeItemMain.SetActive(value: false);
					bungeeItemMain.transform.parent = null;
				}
				else
				{
					Transform parent = base.transform.Find(((TWeapon)componentInChildren.tItem).bone);
					Attach(componentInChildren.transform, parent, Quaternion.Euler(componentInChildren.carryRotation));
					TWeapon tWeapon = (TWeapon)componentInChildren.GetComponent<Weapon>().tItem;
					if (tWeapon != null && tWeapon.GetWeaponType() == Weapon.TYPE.MAIN && tWeapon.IsTwoHands && WeaponLeft != null)
					{
						Attach(WeaponLeft.transform, parent, Quaternion.Euler(currentWeaponLeft.carryRotation));
					}
				}
				if (componentInChildren.slot == Weapon.TYPE.MAIN)
				{
					GdgtGun componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<GdgtGun>();
					if (componentInChildren2 != null)
					{
						if (componentInChildren2.carryAnim)
						{
							componentInChildren.gameObject.animation.Play("carry");
						}
						else
						{
							componentInChildren.gameObject.animation.Play("idle");
						}
					}
				}
			}
			if (weapon == null)
			{
				Debug.LogError("Next Weapon is null " + weaponType);
			}
			else
			{
				currentWeapon = weapon;
				weapon.GetComponent<Weapon>().GadgetSound(Weapon.GADGET.BOLTUP);
				Attach(weapon.transform, rightHand, Quaternion.Euler(0f, 90f, 90f));
				TWeapon tWeapon2 = (TWeapon)weapon.GetComponent<Weapon>().tItem;
				if (WeaponLeft != null && tWeapon2 != null && tWeapon2.IsTwoHands)
				{
					WeaponLeft.GetComponent<Weapon>().GadgetSound(Weapon.GADGET.BOLTUP);
					Attach(WeaponLeft.transform, leftHand, Quaternion.Euler(0f, 90f, 90f));
				}
				if (weapon.slot == Weapon.TYPE.MAIN)
				{
					weapon.gameObject.animation.Play("idle");
					if (tWeapon2 != null && tWeapon2.IsTwoHands)
					{
						WeaponLeft.gameObject.animation.Play("idle");
					}
				}
			}
		}
	}

	public void SetActiveCurrentWeapon(bool state)
	{
		if (currentWeapon != null)
		{
			currentWeapon.gameObject.SetActive(state);
			TWeapon tWeapon = currentWeapon.tItem as TWeapon;
			if (tWeapon != null && tWeapon.IsTwoHands && currentWeaponLeft != null)
			{
				currentWeaponLeft.gameObject.SetActive(state);
			}
		}
	}

	private void Wear(string itemCode)
	{
		TCostume tCostume = TItemManager.Instance.Get<TCostume>(itemCode);
		if (tCostume != null)
		{
			string text = gender + tCostume.main;
			string b = gender + tCostume.aux;
			string b2 = gender + tCostume.mark;
			bool flag = false;
			for (int i = 0; i < TItemManager.Instance.coatBodyCodes.Length; i++)
			{
				if (itemCode == TItemManager.Instance.coatBodyCodes[i])
				{
					flag = true;
					text += "_coat";
					if (gender == "m_")
					{
						looks[0].enabled = false;
						looks[0] = manRealBody[1].GetComponent<SkinnedMeshRenderer>();
						looks[0].enabled = true;
						looks[0].material = skins[0];
						manBody[0] = manRealBody[1];
						womanBody[0] = womanRealBody[1];
					}
					else
					{
						looks[0].enabled = false;
						looks[0] = womanRealBody[1].GetComponent<SkinnedMeshRenderer>();
						looks[0].enabled = true;
						looks[0].material = skins[0];
						manBody[0] = manRealBody[1];
						womanBody[0] = womanRealBody[1];
					}
					break;
				}
			}
			if (!flag && tCostume.slot == TItem.SLOT.UPPER)
			{
				if (gender == "m_")
				{
					SkinnedMeshRenderer component = manRealBody[1].GetComponent<SkinnedMeshRenderer>();
					if (component.enabled)
					{
						looks[0].enabled = false;
						looks[0] = manRealBody[0].GetComponent<SkinnedMeshRenderer>();
						looks[0].enabled = true;
						looks[0].material = skins[0];
						manBody[0] = manRealBody[0];
						womanBody[0] = womanRealBody[0];
					}
				}
				else
				{
					SkinnedMeshRenderer component2 = womanRealBody[1].GetComponent<SkinnedMeshRenderer>();
					if (component2.enabled)
					{
						looks[0].enabled = false;
						looks[0] = womanRealBody[0].GetComponent<SkinnedMeshRenderer>();
						looks[0].enabled = true;
						looks[0].material = skins[0];
						manBody[0] = manRealBody[0];
						womanBody[0] = womanRealBody[0];
					}
				}
			}
			for (int j = 0; j < looks.Length; j++)
			{
				if (looks[j].name == text)
				{
					skins[j] = tCostume.mainMat;
					looks[j].material = tCostume.mainMat;
				}
				if (looks[j].name == b)
				{
					skins[j] = tCostume.auxMat;
					looks[j].material = tCostume.auxMat;
				}
				if (looks[j].name == b2)
				{
					skins[j] = tCostume.markMat;
					looks[j].material = tCostume.markMat;
				}
			}
			if (isMirror)
			{
				BroadcastMessage("OnChangeCostume", tCostume.slot);
			}
		}
	}

	private void ChangeHead(string itemCode)
	{
		TCharacter tCharacter = TItemManager.Instance.Get<TCharacter>(itemCode);
		if (tCharacter != null)
		{
			if (testGender)
			{
				MyInfoManager.Instance.charCode = itemCode;
			}
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				if (itemCode == "c17")
				{
					IsYang = true;
				}
				else
				{
					IsYang = false;
				}
			}
			else
			{
				IsYang = false;
			}
			SkinnedMeshRenderer x = null;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			string b = tCharacter.prefix + "face";
			string b2 = tCharacter.prefix + "head";
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren)
			{
				if (skinnedMeshRenderer2.name == b)
				{
					x = skinnedMeshRenderer2;
				}
				if (skinnedMeshRenderer2.name == b2)
				{
					skinnedMeshRenderer = skinnedMeshRenderer2;
					if (tCharacter.mainMat != null)
					{
						skinnedMeshRenderer.material = tCharacter.mainMat;
					}
				}
			}
			if (x != null && skinnedMeshRenderer != null)
			{
				string b3 = gender;
				gender = tCharacter.gender;
				smrFace.enabled = false;
				smrFace = x;
				smrFace.enabled = true;
				FacialExpressor component = GetComponent<FacialExpressor>();
				if (null != component)
				{
					component.ChangeFace(smrFace, tCharacter.prefix);
				}
				smrHead.enabled = false;
				smrHead = skinnedMeshRenderer;
				smrHead.enabled = true;
				if (gender != b3)
				{
					if (gender == "m_")
					{
						for (int j = 0; j < manBody.Length; j++)
						{
							looks[j].enabled = false;
							looks[j] = manBody[j].GetComponent<SkinnedMeshRenderer>();
							looks[j].enabled = true;
							looks[j].material = skins[j];
						}
					}
					else
					{
						for (int k = 0; k < womanBody.Length; k++)
						{
							looks[k].enabled = false;
							looks[k] = womanBody[k].GetComponent<SkinnedMeshRenderer>();
							looks[k].enabled = true;
							looks[k].material = skins[k];
						}
					}
				}
			}
		}
	}

	private void EquipAccessory(string itemCode)
	{
		TAccessory tAccessory = TItemManager.Instance.Get<TAccessory>(itemCode);
		if (tAccessory != null)
		{
			Transform transform = base.transform.Find(tAccessory.bone);
			if (null == transform)
			{
				Debug.LogError("Fail to find the bone " + tAccessory.bone);
			}
			else
			{
				GameObject gameObject = Object.Instantiate((Object)tAccessory.prefab) as GameObject;
				Attach(gameObject.transform, transform, Quaternion.Euler(0f, 0f, 0f));
				gameObject.GetComponent<Equip>().tItem = tAccessory;
			}
		}
	}

	private void EquipWeapon(string itemCode, int player = -1)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(itemCode);
		if (tWeapon != null)
		{
			Transform transform = base.transform.Find(tWeapon.bone);
			if (null == transform)
			{
				Debug.LogError("Fail to find the bone " + tWeapon.bone);
			}
			else
			{
				GameObject gameObject = Object.Instantiate((Object)tWeapon.CurPrefab()) as GameObject;
				Attach(gameObject.transform, transform, Quaternion.Euler(gameObject.GetComponent<Weapon>().carryRotation));
				gameObject.GetComponent<Equip>().tItem = tWeapon;
				gameObject.GetComponent<WeaponGadget>().enabled = true;
				gameObject.GetComponent<WeaponFunction>().enabled = false;
				Aim component = gameObject.GetComponent<Aim>();
				if (null != component)
				{
					component.enabled = false;
				}
				Scope component2 = gameObject.GetComponent<Scope>();
				if (null != component2)
				{
					component2.enabled = false;
				}
				if (tWeapon.IsTwoHands)
				{
					WeaponLeft = (Object.Instantiate((Object)tWeapon.CurPrefab()) as GameObject);
					Attach(WeaponLeft.transform, transform, Quaternion.Euler(WeaponLeft.GetComponent<Weapon>().carryRotation));
					currentWeaponLeft = WeaponLeft.GetComponent<Weapon>();
					WeaponLeft.GetComponent<Equip>().tItem = tWeapon;
					WeaponLeft.GetComponent<WeaponGadget>().enabled = true;
					WeaponLeft.GetComponent<WeaponFunction>().enabled = false;
					WeaponLeft.GetComponent<GdgtGun>().LeftHand = true;
					component = WeaponLeft.GetComponent<Aim>();
					if (null != component)
					{
						component.enabled = false;
					}
					component2 = WeaponLeft.GetComponent<Scope>();
					if (null != component2)
					{
						component2.enabled = false;
					}
					GlobalVars.Instance.SetOullineColor(player, WeaponLeft);
				}
				if (isMirror)
				{
					BroadcastMessage("OnChangeCostume", tWeapon.slot);
				}
				GlobalVars.Instance.SetOullineColor(player, gameObject);
			}
		}
	}

	private void Attach(Transform child, Transform parent, Quaternion childRotation)
	{
		child.position = parent.position;
		child.rotation = parent.rotation;
		child.parent = parent;
		child.localRotation = childRotation;
		child.localScale = new Vector3(1f, 1f, 1f);
	}

	public void WeaponChange(string prev, string next, int player)
	{
		Unequip(prev);
		Equip(next, player);
	}

	public void Equip(string itemCode, int player = -1)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(itemCode);
		if (tItem != null)
		{
			switch (tItem.type)
			{
			case TItem.TYPE.WEAPON:
				EquipWeapon(itemCode, player);
				break;
			case TItem.TYPE.CLOTH:
				Wear(itemCode);
				break;
			case TItem.TYPE.ACCESSORY:
				EquipAccessory(itemCode);
				break;
			case TItem.TYPE.CHARACTER:
				ChangeHead(itemCode);
				break;
			case TItem.TYPE.SPECIAL:
				EquipBuildGun(itemCode);
				break;
			}
		}
	}

	public void UnequipWeaponBySlot(int slot)
	{
		string itemCode = string.Empty;
		Equip[] componentsInChildren = GetComponentsInChildren<Equip>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				TWeapon tWeapon = componentsInChildren[i].tItem as TWeapon;
				if (tWeapon != null && tWeapon.GetWeaponType() == (Weapon.TYPE)slot)
				{
					itemCode = tWeapon.code;
					if (tWeapon != null && tWeapon.IsTwoHands && WeaponLeft != null)
					{
						Object.DestroyImmediate(WeaponLeft.gameObject);
						WeaponLeft = null;
						currentWeaponLeft = null;
					}
					break;
				}
			}
		}
		Unequip(itemCode);
	}

	public void Unequip(string itemCode)
	{
		if (initOnce)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(itemCode);
			if (tItem != null)
			{
				if (tItem.type == TItem.TYPE.WEAPON)
				{
					TWeapon tWeapon = tItem as TWeapon;
					if (tWeapon != null && tWeapon.IsTwoHands && WeaponLeft != null)
					{
						Object.DestroyImmediate(WeaponLeft.gameObject);
						WeaponLeft = null;
						currentWeaponLeft = null;
					}
					Equip[] componentsInChildren = GetComponentsInChildren<Equip>();
					if (componentsInChildren != null)
					{
						int num = 0;
						while (true)
						{
							if (num >= componentsInChildren.Length)
							{
								return;
							}
							if (componentsInChildren[num] != null && componentsInChildren[num].tItem != null && componentsInChildren[num].tItem.code == itemCode)
							{
								break;
							}
							num++;
						}
						Object.DestroyImmediate(componentsInChildren[num].gameObject);
					}
				}
				else if (tItem.type == TItem.TYPE.ACCESSORY)
				{
					Equip[] componentsInChildren2 = GetComponentsInChildren<Equip>();
					if (componentsInChildren2 != null)
					{
						int num2 = 0;
						while (true)
						{
							if (num2 >= componentsInChildren2.Length)
							{
								return;
							}
							if (componentsInChildren2[num2] != null && componentsInChildren2[num2].tItem.code == itemCode)
							{
								break;
							}
							num2++;
						}
						Object.DestroyImmediate(componentsInChildren2[num2].gameObject);
					}
				}
				else if (tItem.type == TItem.TYPE.CHARACTER)
				{
					if (smrFace != null)
					{
						smrFace.enabled = false;
					}
					if (smrHead != null)
					{
						smrHead.enabled = false;
					}
					InitDefaultHead();
					if (skins != null && gender != "m_")
					{
						gender = "m_";
						for (int i = 0; i < manBody.Length; i++)
						{
							looks[i].enabled = false;
							looks[i] = manBody[i].GetComponent<SkinnedMeshRenderer>();
							looks[i].enabled = true;
							looks[i].material = skins[i];
						}
					}
				}
			}
		}
	}

	public void enableARMBAND(bool enable, bool red)
	{
		SkinnedMeshRenderer component = manBody[5].GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			if (!enable)
			{
				component.enabled = false;
			}
			else
			{
				component.material = armBands[(!red) ? 1 : 0];
				component.enabled = true;
			}
		}
	}

	public void EquipSmokeBomb()
	{
		ChangeWeapon(Weapon.TYPE.PROJECTILE);
		WeaponFunction currentWeaponFunction = GetCurrentWeaponFunction();
		if (currentWeaponFunction.weaponBy != Weapon.BY.KG440)
		{
			Equip component = currentWeapon.GetComponent<Equip>();
			WeaponChange(component.tItem.code, "wal", -1);
		}
	}

	public void ReturnBuildGun()
	{
		ChangeWeapon(Weapon.TYPE.MODE_SPECIFIC);
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
				Transform transform = base.transform.Find(item.thirdPersonAttachBone);
				if (null == transform)
				{
					Debug.LogError("Fail to find the bone " + item.thirdPersonAttachBone);
				}
				else
				{
					wingEffect = (Object.Instantiate((Object)item.wingEffect) as GameObject);
					Attach(wingEffect.transform, transform, Quaternion.Euler(0f, 0f, 0f));
				}
			}
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
	}

	public void EquipBrickBoom()
	{
		ChangeWeapon(Weapon.TYPE.MAIN);
		WeaponFunction currentWeaponFunction = GetCurrentWeaponFunction();
		if (currentWeaponFunction.weaponBy != Weapon.BY.BRICK_BOOMER)
		{
			Equip component = currentWeapon.GetComponent<Equip>();
			Unequip(component.tItem.code);
			GameObject gameObject = TItemManager.Instance.FindPrefab("BrickGun03");
			if (null != gameObject && bungeeItemMain == null)
			{
				bungeeItemMain = (Object.Instantiate((Object)gameObject) as GameObject);
				bungeeItemMain.GetComponent<Weapon>().tItem = null;
				bungeeItemMain.GetComponent<WeaponGadget>().enabled = true;
				bungeeItemMain.GetComponent<WeaponFunction>().enabled = false;
			}
		}
	}

	public void EnableBrickComposerFever(bool enable)
	{
		Weapon y = GetCurrentWeapon();
		if (!(null == y))
		{
			WeaponGadget componentInChildren = rightHand.GetComponentInChildren<GdgtBrickComposer>();
			if (!(null == componentInChildren))
			{
				componentInChildren.setFever(enable);
			}
		}
	}

	public void DefaultSex()
	{
		if (MyInfoManager.Instance.charCode.Length > 0)
		{
			ChangeHead(MyInfoManager.Instance.charCode);
		}
		else
		{
			smrFace.enabled = false;
			smrHead.enabled = false;
			InitDefaultHead();
			gender = "m_";
			for (int i = 0; i < manBody.Length; i++)
			{
				looks[i].enabled = false;
				looks[i] = manBody[i].GetComponent<SkinnedMeshRenderer>();
				skins[i] = defMat[i];
				looks[i].enabled = true;
				looks[i].material = skins[i];
			}
		}
	}

	public bool IsTwoHands()
	{
		if (currentWeapon == null)
		{
			return false;
		}
		return (currentWeapon.tItem as TWeapon)?.IsTwoHands ?? false;
	}
}
