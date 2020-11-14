using _Emulator;
using UnityEngine;

public class MeleeWeapon : WeaponFunction
{
	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public float AtkPow = 30f;

	public float Rigidity = 0.6f;

	public string slashAnimation;

	public float slashSpeed = 1f;

	private bool slashing;

	private bool drawn;

	private bool isValidRange;

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				if (skinnedMeshRenderer.material.mainTexture != null && UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name) != null)
				{
					skinnedMeshRenderer.material.mainTexture = UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name);
				}
			}
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
		Modify();
		Reset();
	}

	private void Modify()
	{
		WeaponFunction component = GetComponent<WeaponFunction>();
		if (null != component)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)component.weaponBy);
			if (wpnMod != null)
			{
				speedFactor = wpnMod.fSpeedFactor;
				AtkPow = wpnMod.fAtkPow;
				Rigidity = wpnMod.fRigidity;
				slashSpeed = wpnMod.fSlashSpeed;
			}
			TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
			Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
			if (item == null)
			{
				item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
			}
			if (item != null)
			{
				int num = 0;
				int grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					AtkPow += value;
				}
				num = 5;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value2 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					slashSpeed += value2;
				}
			}
		}
	}

	private void DrawCrossHair()
	{
		if (Screen.lockCursor)
		{
			Color color = GUI.color;
			if (crossEffectTime > 0f)
			{
				GUI.color = Color.red;
			}
			else
			{
				GUI.color = Config.instance.crosshairColor;
			}
			if (null != vCrossHair)
			{
				Vector2 vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2 - 8));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			if (null != hCrossHair)
			{
				Vector2 vector = new Vector2((float)(Screen.width / 2 - 8), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)(Screen.width / 2), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
		}
	}

	public override void Reset(bool bDefenseRespwan = false)
	{
		if (drawn)
		{
			Restart();
		}
	}

	public override void SetDrawn(bool draw)
	{
		drawn = draw;
		if (drawn)
		{
			Restart();
		}
	}

	public void SlashStart()
	{
		slashing = true;
	}

	public void SlashEnd()
	{
		slashing = false;
	}

	public void EnterValidRange(bool bigSlash)
	{
		isValidRange = true;
	}

	public void LeaveValidRange()
	{
		isValidRange = false;
	}

	private void Restart()
	{
		slashing = false;
		isValidRange = false;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawCrossHair();
			DrawAmmo();
			GUI.enabled = true;
		}
	}

	private void DrawAmmo()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			Texture2D weaponBy = TItemManager.Instance.GetWeaponBy((int)base.weaponBy);
			if (null != ammoBg && null != weaponBy)
			{
				Rect position = new Rect((float)(Screen.width - ammoBg.width - 1), (float)(Screen.height - ammoBg.height - 1), (float)ammoBg.width, (float)ammoBg.height);
				TextureUtil.DrawTexture(position, ammoBg);
				float num = 40f;
				float num2 = (float)weaponBy.width * num / (float)weaponBy.height;
				TextureUtil.DrawTexture(new Rect(position.x + (position.width - num2) / 2f, position.y + 10f, num2, num), weaponBy, ScaleMode.StretchToFill);
			}
		}
	}

	private float CalcPowFrom(Vector3 BombPos, Vector3 position)
	{
		float num = Vector3.Distance(BombPos, position);
		if (num > GlobalVars.Instance.BoomRadius)
		{
			return 0f;
		}
		float num2 = (GlobalVars.Instance.BoomRadius - num) / GlobalVars.Instance.BoomRadius;
		return (float)GlobalVars.Instance.BoomDamage * num2;
	}

	private void CheckMyself(Vector3 boomPos, float DamageRadius)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null == component)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
			else
			{
				Vector3 position = gameObject.transform.position;
				if (Vector3.Distance(position, boomPos) < DamageRadius)
				{
					int num = 0;
					for (int i = 0; i < 3; i++)
					{
						position.y += 0.3f;
						if (!Physics.Linecast(base.transform.position, position, out RaycastHit _, layerMask))
						{
							int num2 = Mathf.FloorToInt(0.3f * (float)(i + 1) * CalcPowFrom(boomPos, position));
							if (num2 > 0)
							{
								num += num2;
							}
						}
					}
					if (num > 0)
					{
						component.GetHit(MyInfoManager.Instance.Seq, num, 1f, (int)weaponBy, -1, autoHealPossible: true, checkZombie: false);
					}
				}
			}
		}
	}

	private void CheckBoxmen(Vector3 boomPos, float DamageRadius)
	{
		HitPart[] array = ExplosionUtil.CheckBoxmen(boomPos, DamageRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position));
				if (num > 0)
				{
					P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, allComponents[0].Desc.Seq, num, Rigidity, -3);
				}
			}
		}
	}

	private void CheckMonster(Vector3 boomPos, float DamageRadius)
	{
		HitPart[] array = ExplosionUtil.CheckMon(boomPos, DamageRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position));
				num += AddUpgradedDamagei(category);
				if (num > 0)
				{
					MonManager.Instance.Hit(allComponents[0].Desc.Seq, num, 1f, -3, Vector3.zero, Vector3.zero, -1);
				}
			}
		}
	}

	private void CheckDestructibles(Vector3 boomPos, float DamageRadius)
	{
		BrickProperty[] array = ExplosionUtil.CheckDestructibles(boomPos, DamageRadius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Hit((int)CalcPowFrom(boomPos, array[i].transform.position));
			if (array[i].HitPoint <= 0)
			{
				CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(array[i].Seq);
			}
			else
			{
				P2PManager.Instance.SendPEER_BRICK_HITPOINT(array[i].Seq, array[i].HitPoint);
			}
		}
	}

	private float CalcAtkPow()
	{
		float atkPow = AtkPow;
		return atkPow + WantedManager.Instance.GetWantedAtkPowBoost(MyInfoManager.Instance.Seq, atkPow);
	}

	private void CheckSlash()
	{
		if (isValidRange)
		{
			int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
			Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			if (Physics.Raycast(ray, out RaycastHit hitInfo, GetComponent<Weapon>().range, layerMask))
			{
				GameObject gameObject = null;
				GameObject gameObject2 = hitInfo.transform.gameObject;
				if (gameObject2.layer == LayerMask.NameToLayer("Chunk") || gameObject2.layer == LayerMask.NameToLayer("Brick"))
				{
					BrickProperty brickProperty = null;
					GameObject gameObject3 = null;
					if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
					{
						BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
						if (allComponents.Length > 0)
						{
							brickProperty = allComponents[0];
						}
					}
					else
					{
						gameObject = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
						if (null != gameObject)
						{
							brickProperty = gameObject.GetComponent<BrickProperty>();
						}
					}
					if (null != brickProperty)
					{
						P2PManager.Instance.SendPEER_HIT_BRICK(brickProperty.Seq, brickProperty.Index, hitInfo.point, hitInfo.normal, isBullet: false);
						gameObject3 = BrickManager.Instance.GetBulletImpact(brickProperty.Index);
						Brick brick = BrickManager.Instance.GetBrick(brickProperty.Index);
						if (brick != null && brick.destructible)
						{
							brickProperty.Hit((int)CalcAtkPow());
							if (brickProperty.HitPoint <= 0)
							{
								if (!Application.loadedLevelName.Contains("Tutor"))
								{
									CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(brickProperty.Seq);
								}
								gameObject3 = null;
								if (brickProperty.Index == 115 || brickProperty.Index == 193)
								{
									CheckMyself(gameObject.transform.position, GlobalVars.Instance.BoomRadius);
									CheckBoxmen(gameObject.transform.position, GlobalVars.Instance.BoomRadius);
									CheckMonster(gameObject.transform.position, GlobalVars.Instance.BoomRadius);
									CheckDestructibles(gameObject.transform.position, GlobalVars.Instance.BoomRadius);
								}
							}
							else
							{
								P2PManager.Instance.SendPEER_BRICK_HITPOINT(brickProperty.Seq, brickProperty.HitPoint);
							}
						}
					}
					if (null != gameObject3)
					{
						Object.Instantiate((Object)gameObject3, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("BoxMan"))
				{
					PlayerProperty[] allComponents2 = Recursively.GetAllComponents<PlayerProperty>(gameObject2.transform, includeInactive: false);
					TPController[] allComponents3 = Recursively.GetAllComponents<TPController>(gameObject2.transform, includeInactive: false);
					if (allComponents2.Length != 1)
					{
						Debug.LogError("PlayerProperty should be unique for a box man, but it has multiple PlayerProperty components or non ");
					}
					if (allComponents3.Length != 1)
					{
						Debug.LogError("TPController should be unique for a box man, but it has multiple TPController components or non ");
					}
					PlayerProperty playerProperty = null;
					TPController tPController = null;
					if (allComponents2.Length > 0)
					{
						playerProperty = allComponents2[0];
					}
					if (allComponents3.Length > 0)
					{
						tPController = allComponents3[0];
					}
					if (playerProperty != null && tPController != null)
					{
						int num = 0;
						HitPart component = gameObject2.GetComponent<HitPart>();
						if (component != null)
						{
							if (component.GetHitImpact() != null)
							{
								Object.Instantiate((Object)component.GetHitImpact(), hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
							}
							num = (int)(CalcAtkPow() * component.damageFactor);
							if (!playerProperty.IsHostile())
							{
								num = 0;
							}
							WeaponFunction component2 = GetComponent<WeaponFunction>();
							if (null == component2)
							{
								Debug.LogError("wpnFunc == nulll");
							}
							TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
							if (tWeapon == null)
							{
								Debug.LogError("wpn == null");
							}
							Item item = MyInfoManager.Instance.GetItemBySequence(component2.ItemSeq);
							if (item == null)
							{
								item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
							}
							num = GlobalVars.Instance.applyDurabilityDamage(item?.Durability ?? (-1), tWeapon.durabilityMax, num);
							P2PManager.Instance.SendPEER_HIT_BRICKMAN(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, (int)component.part, hitInfo.point, hitInfo.normal, lucky: false, 0, ray.direction);
							P2PManager.Instance.SendPEER_PIERCE(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, num, Rigidity, (int)weaponBy);
						}
						tPController.GetHit(num, playerProperty.Desc.Seq);
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("Mon"))
				{
					MonProperty[] allComponents4 = Recursively.GetAllComponents<MonProperty>(gameObject2.transform, includeInactive: false);
					MonProperty monProperty = null;
					if (allComponents4.Length > 0)
					{
						monProperty = allComponents4[0];
					}
					if (monProperty != null)
					{
						HitPart component3 = gameObject2.GetComponent<HitPart>();
						if (component3 != null)
						{
							if ((MyInfoManager.Instance.Slot < 4 && monProperty.Desc.bRedTeam) || (MyInfoManager.Instance.Slot >= 4 && !monProperty.Desc.bRedTeam))
							{
								return;
							}
							if (component3.GetHitImpact() != null)
							{
								Object.Instantiate((Object)component3.GetHitImpact(), hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
							}
							if (monProperty.Desc.Xp <= 0)
							{
								return;
							}
							int num2 = (int)(CalcAtkPow() * component3.damageFactor);
							if (monProperty.Desc.bHalfDamage)
							{
								num2 /= 2;
							}
							MonManager.Instance.Hit(monProperty.Desc.Seq, num2, 0f, (int)weaponBy, Vector3.zero, Vector3.zero, -1);
						}
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("InvincibleArmor") || gameObject2.layer == LayerMask.NameToLayer("Bomb") || gameObject2.layer == LayerMask.NameToLayer("InstalledBomb"))
				{
					GameObject impact = VfxOptimizer.Instance.GetImpact(gameObject2.layer);
					if (null != impact)
					{
						Object.Instantiate((Object)impact, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
						P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject2.layer, hitInfo.point, hitInfo.normal);
					}
				}
				isValidRange = false;
			}
		}
	}

	private bool CanSlash()
	{
		return localCtrl.CanControl() && !slashing && drawn;
	}

	private void Update()
	{
		if (Screen.lockCursor && BrickManager.Instance.IsLoaded)
		{
			VerifyCamera();
			VerifyLocalController();
			UpdateCrossEffect();
			if (CanSlash() && custom_inputs.Instance.GetButtonDown("K_FIRE1"))
			{
				GetComponent<Weapon>().FireSound();
				localCtrl.DoSlashAnimation(slashSpeed);
				P2PManager.Instance.SendPEER_SLASH(MyInfoManager.Instance.Seq, slashSpeed);
			}
			CheckSlash();
		}
	}
}
