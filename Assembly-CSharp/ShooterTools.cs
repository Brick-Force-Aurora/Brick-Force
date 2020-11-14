using System.Collections.Generic;
using UnityEngine;

public class ShooterTools : MonoBehaviour
{
	private GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public ShooterTool[] desc;

	private ShooterTool[] tools;

	private Vector2 crdToolSize = new Vector2(64f, 74f);

	private float offset = 7f;

	private Vector2 crdHotkey = new Vector2(55f, 18f);

	private Vector2 crdAmount = new Vector2(58f, 14f);

	private AudioSource audioSource;

	private LocalController localController;

	private BattleChat battleChat;

	private float pointBooster;

	private float xpBooster;

	private float luck;

	private int armor;

	private float mainAmmoMax;

	private float auxAmmoMax;

	private float grenadeMax1;

	private float grenadeMax2;

	private float hpCooltime;

	private float dashTimeInc;

	private float respawnTimeDec;

	private float fallenDamageDec;

	private int numTool;

	private float bufHeight;

	private int numBuf;

	private bool on;

	private void VerifyAudioSource()
	{
		if (null == audioSource)
		{
			audioSource = GetComponent<AudioSource>();
			if (tools != null)
			{
				for (int i = 0; i < tools.Length; i++)
				{
					if (tools[i] != null)
					{
						tools[i].Audio = audioSource;
					}
				}
			}
		}
	}

	private void VerifyLocalController()
	{
		if (null == localController)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				localController = gameObject.GetComponent<LocalController>();
				if (tools != null)
				{
					for (int i = 0; i < tools.Length; i++)
					{
						if (tools[i] != null)
						{
							tools[i].Controller = localController;
						}
					}
				}
			}
		}
	}

	private void Start()
	{
		string[] array = new string[5]
		{
			"K_SHOOTER1",
			"K_SHOOTER2",
			"K_SHOOTER3",
			"K_SHOOTER4",
			"K_SHOOTER5"
		};
		battleChat = GetComponent<BattleChat>();
		VerifyAudioSource();
		VerifyLocalController();
		tools = new ShooterTool[MyInfoManager.Instance.ShooterTools.Length];
		for (int i = 0; i < MyInfoManager.Instance.ShooterTools.Length; i++)
		{
			tools[i] = null;
			ConsumableDesc consumableDesc = null;
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(MyInfoManager.Instance.ShooterTools[i]);
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
			{
				TSpecial tSpecial = (TSpecial)itemBySequence.Template;
				string func = TItem.FunctionMaskToString(tSpecial.functionMask);
				consumableDesc = ConsumableManager.Instance.Get(func);
				if (consumableDesc != null && !consumableDesc.isShooterTool)
				{
					consumableDesc = null;
				}
			}
			if (itemBySequence != null && consumableDesc != null)
			{
				tools[i] = new ShooterTool(this, consumableDesc, itemBySequence, base.audio, array[i], custom_inputs.Instance.GetKeyCodeName(array[i]), battleChat, localController);
			}
		}
	}

	public void StartCoolTime(string func)
	{
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null)
			{
				tools[i].StartCoolTime(func);
			}
		}
	}

	public bool find(string name)
	{
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null && tools[i].Name != null && tools[i].Name.Length > 0 && tools[i].Name == name)
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
			bool flag = false;
			int num = tools.Length;
			float num2 = (float)num * crdToolSize.x + (float)(num + 1) * offset;
			float num3 = crdToolSize.y + crdHotkey.y;
			Rect position = new Rect(((float)Screen.width - num2) / 2f, (float)Screen.height - num3 - 32f, num2, num3);
			GUI.BeginGroup(position);
			numTool = 0;
			for (int i = 0; i < tools.Length; i++)
			{
				if (tools[i] != null && !(tools[i].Icon == null))
				{
					flag = true;
					numTool++;
					Rect position2 = new Rect(offset + (float)i * (crdToolSize.x + offset), crdHotkey.y, crdToolSize.x, crdToolSize.y);
					GUI.Box(new Rect(position2.x + 6f, position2.y - crdHotkey.y, crdHotkey.x, crdHotkey.y), string.Empty, "cns_hotkey");
					GUI.Box(position2, string.Empty, "cns_item");
					Rect position3 = new Rect(offset + (float)i * (crdToolSize.x + offset), crdHotkey.y, crdToolSize.x, 60f);
					TextureUtil.DrawTexture(position3, tools[i].Icon, ScaleMode.StretchToFill);
					Color clrText = Color.gray;
					if (tools[i].IsPassive)
					{
						string text = "off";
						if (tools[i].IsActive)
						{
							clrText = Color.yellow;
							text = "on";
						}
						LabelUtil.TextOut(new Vector2(position2.x + 56f, position2.y + 9f), text, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
					LabelUtil.TextOut(new Vector2(position2.x + 32f, position2.y + 10f - crdHotkey.y), tools[i].Hotkey, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					GUI.Box(new Rect(position2.x + 3f, position2.y + 57f, crdAmount.x, crdAmount.y), tools[i].Amount, "cns_onoff");
					string coolTime = tools[i].CoolTime;
					if (coolTime.Length > 0)
					{
						LabelUtil.TextOut(new Vector2(position2.x + position2.width / 2f + 1f, position2.y + position2.height / 2f + 1f), coolTime, "BigLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
						LabelUtil.TextOut(new Vector2(position2.x + position2.width / 2f - 1f, position2.y + position2.height / 2f - 1f), coolTime, "BigLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
				}
			}
			GUI.EndGroup();
			if (BuildOption.Instance.IsNetmarbleOrDev)
			{
				Rect position4 = new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height - 160), 28f, 28f);
				if (!flag)
				{
					position4.y = (float)(Screen.height - 60);
				}
				if (pointBooster > 0f)
				{
					TextureUtil.DrawTexture(position4, BuffManager.Instance.getPointUpTex(), ScaleMode.StretchToFill);
					position4.x += 30f;
				}
				if (xpBooster > 0f)
				{
					TextureUtil.DrawTexture(position4, BuffManager.Instance.getXpUpTex(), ScaleMode.StretchToFill);
					position4.x += 30f;
				}
				if (luck > 0f)
				{
					TextureUtil.DrawTexture(position4, BuffManager.Instance.getLuckUpTex(), ScaleMode.StretchToFill);
					position4.x += 30f;
				}
				if (mainAmmoMax > 0f)
				{
					Texture2D texture2D = MyInfoManager.Instance.HaveFunctionTex("main_ammo_inc");
					if (null != texture2D)
					{
						TextureUtil.DrawTexture(position4, texture2D, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (auxAmmoMax > 0f)
				{
					Texture2D texture2D2 = MyInfoManager.Instance.HaveFunctionTex("aux_ammo_inc");
					if (null != texture2D2)
					{
						TextureUtil.DrawTexture(position4, texture2D2, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (grenadeMax1 > 0f)
				{
					Texture2D texture2D3 = MyInfoManager.Instance.HaveFunctionTex("special_ammo_inc");
					if (null != texture2D3)
					{
						TextureUtil.DrawTexture(position4, texture2D3, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (grenadeMax2 > 0f)
				{
					Texture2D texture2D4 = MyInfoManager.Instance.HaveFunctionTex("special_ammo_add");
					if (null != texture2D4)
					{
						TextureUtil.DrawTexture(position4, texture2D4, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (hpCooltime > 0f)
				{
					Texture2D texture2D5 = MyInfoManager.Instance.HaveFunctionTex("hp_cooltime");
					if (null != texture2D5)
					{
						TextureUtil.DrawTexture(position4, texture2D5, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (dashTimeInc > 0f)
				{
					Texture2D texture2D6 = MyInfoManager.Instance.HaveFunctionTex("dash_time_inc");
					if (null != texture2D6)
					{
						TextureUtil.DrawTexture(position4, texture2D6, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (respawnTimeDec > 0f)
				{
					Texture2D texture2D7 = MyInfoManager.Instance.HaveFunctionTex("respwan_time_dec");
					if (null != texture2D7)
					{
						TextureUtil.DrawTexture(position4, texture2D7, ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (fallenDamageDec > 0f)
				{
					Texture2D y = MyInfoManager.Instance.HaveFunctionTex("fallen_damage_reduce");
					if (null != y)
					{
						TextureUtil.DrawTexture(position4, MyInfoManager.Instance.HaveFunctionTex("fallen_damage_reduce"), ScaleMode.StretchToFill);
						position4.x += 30f;
					}
				}
				if (on)
				{
					DrawBufInfo();
				}
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void DrawBufInfo()
	{
		float bufferTooltipHeight = getBufferTooltipHeight();
		float num = 130f;
		float num2 = (float)Screen.height - bufferTooltipHeight - 10f;
		GUI.Box(new Rect(num, num2, 180f, bufferTooltipHeight), string.Empty, "BoxInnerLine");
		Vector2 pos = new Vector2(num + 10f, num2 + 10f);
		DoBuffRows(ref pos, calcHeight: false);
	}

	private float getBufferTooltipHeight()
	{
		float num = 20f;
		num += 15f;
		return num + bufHeight;
	}

	public void DoBuff()
	{
		numBuf = 0;
		pointBooster = 0f;
		xpBooster = 0f;
		luck = 0f;
		if (MyInfoManager.Instance.HaveFunction("premium_account") >= 0)
		{
			xpBooster += 10f;
			pointBooster += 10f;
		}
		List<long> list = new List<long>();
		for (int i = 0; i < MyInfoManager.Instance.ShooterTools.Length; i++)
		{
			ConsumableDesc consumableDesc = null;
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(MyInfoManager.Instance.ShooterTools[i]);
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL && itemBySequence.Amount > 0 && !list.Contains(itemBySequence.Seq))
			{
				itemBySequence.AmountBuf = itemBySequence.Amount;
				list.Add(itemBySequence.Seq);
			}
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL && itemBySequence.AmountBuf > 0)
			{
				TSpecial tSpecial = (TSpecial)itemBySequence.Template;
				if (tSpecial.IsConsumableBuff)
				{
					string func = TItem.FunctionMaskToString(tSpecial.functionMask);
					consumableDesc = ConsumableManager.Instance.Get(func);
					if (consumableDesc != null && !consumableDesc.isShooterTool)
					{
						consumableDesc = null;
					}
					if (consumableDesc != null)
					{
						TBuff tBuff = BuffManager.Instance.Get(tSpecial.param);
						if (tBuff != null)
						{
							if (tBuff.IsPoint)
							{
								pointBooster += (float)tBuff.PointRatio;
							}
							if (tBuff.IsXp)
							{
								xpBooster += (float)tBuff.XpRatio;
							}
							if (tBuff.IsLuck)
							{
								luck += (float)tBuff.Luck;
							}
						}
					}
					itemBySequence.AmountBuf--;
				}
			}
		}
		list.Clear();
		list = null;
		string[] usings = MyInfoManager.Instance.GetUsings();
		for (int j = 0; j < usings.Length; j++)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(usings[j]);
			if (tItem != null && tItem.tBuff != null)
			{
				pointBooster += (float)tItem.tBuff.PointRatio;
				xpBooster += (float)tItem.tBuff.XpRatio;
				luck += (float)tItem.tBuff.Luck;
			}
		}
		if (pointBooster > 0f)
		{
			numBuf++;
		}
		if (xpBooster > 0f)
		{
			numBuf++;
		}
		if (luck > 0f)
		{
			numBuf++;
		}
		Item[] usingItems = MyInfoManager.Instance.GetUsingItems();
		for (int k = 0; k < usingItems.Length; k++)
		{
			int num = 0;
			int num2 = 0;
			num = 10;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				pointBooster += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
			num = 9;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				xpBooster += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
			num = 12;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				luck += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
		}
		armor = MyInfoManager.Instance.SumArmor();
		mainAmmoMax = MyInfoManager.Instance.SumFunctionFactor("main_ammo_inc");
		auxAmmoMax = MyInfoManager.Instance.SumFunctionFactor("aux_ammo_inc");
		grenadeMax1 = MyInfoManager.Instance.SumFunctionFactor("special_ammo_inc");
		grenadeMax2 = MyInfoManager.Instance.SumFunctionFactor("special_ammo_add");
		hpCooltime = MyInfoManager.Instance.SumFunctionFactor("hp_cooltime");
		dashTimeInc = MyInfoManager.Instance.SumFunctionFactor("dash_time_inc");
		respawnTimeDec = MyInfoManager.Instance.SumFunctionFactor("respwan_time_dec");
		fallenDamageDec = MyInfoManager.Instance.SumFunctionFactor("fallen_damage_reduce");
		if (armor > 0)
		{
			numBuf++;
		}
		if (mainAmmoMax > 0f)
		{
			numBuf++;
		}
		if (auxAmmoMax > 0f)
		{
			numBuf++;
		}
		if (grenadeMax1 > 0f)
		{
			numBuf++;
		}
		if (grenadeMax2 > 0f)
		{
			numBuf++;
		}
		if (hpCooltime > 0f)
		{
			numBuf++;
		}
		if (dashTimeInc > 0f)
		{
			numBuf++;
		}
		if (respawnTimeDec > 0f)
		{
			numBuf++;
		}
		if (fallenDamageDec > 0f)
		{
			numBuf++;
		}
		bufHeight = (float)(numBuf * 15);
	}

	private void DoBuffRows(ref Vector2 pos, bool calcHeight)
	{
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(pos.x, pos.y), StringMgr.Instance.Get("ITEM_BUFF"), "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 15f;
		if (pointBooster > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("POINT_UP"), pointBooster), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (xpBooster > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("XP_UP"), xpBooster), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (luck > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("LUCK_UP"), luck), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (armor > 0 && BuildOption.Instance.Props.useArmor)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("ARMOR_UP"), armor), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (hpCooltime > 0f && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), hpCooltime * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (mainAmmoMax > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), mainAmmoMax * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (auxAmmoMax > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), auxAmmoMax * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (grenadeMax1 > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), grenadeMax1 * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (grenadeMax2 > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP02"), grenadeMax2), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (dashTimeInc > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("DASHTIME"), dashTimeInc * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (respawnTimeDec > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("RESPAWNTIME"), respawnTimeDec * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
		if (fallenDamageDec > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(pos.x, pos.y), string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), fallenDamageDec * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 15f;
		}
	}

	private void Update()
	{
		VerifyAudioSource();
		VerifyLocalController();
		on = (!DialogManager.Instance.IsModal && custom_inputs.Instance.GetButton("K_SITUATION"));
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null)
			{
				tools[i].Update();
			}
		}
	}

	public int AutoHeal()
	{
		int result = 0;
		int[] array = new int[3]
		{
			100,
			50,
			30
		};
		ShooterTool[] array2 = new ShooterTool[3];
		ShooterTool shooterTool = null;
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null)
			{
				if (tools[i].Name == "heal" && tools[i].IsEnable())
				{
					array2[0] = tools[i];
				}
				if (tools[i].Name == "heal50" && tools[i].IsEnable())
				{
					array2[1] = tools[i];
				}
				if (tools[i].Name == "heal30" && tools[i].IsEnable())
				{
					array2[2] = tools[i];
				}
				if (tools[i].Name == "auto_heal" && tools[i].IsEnable())
				{
					shooterTool = tools[i];
				}
			}
		}
		ShooterTool shooterTool2 = null;
		int num = 0;
		while (shooterTool2 == null && num < 3)
		{
			if (array2[num] != null)
			{
				shooterTool2 = array2[num];
				result = array[num];
			}
			num++;
		}
		if (shooterTool2 == null || shooterTool == null)
		{
			return 0;
		}
		shooterTool2.Use();
		shooterTool.Use();
		return result;
	}

	public bool Use(string function)
	{
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null && tools[i].Name == function && tools[i].IsEnable())
			{
				tools[i].Use();
				return true;
			}
		}
		return false;
	}

	public float DrawLLevelHeight()
	{
		if (numTool == 0 && numBuf == 0)
		{
			return 80f;
		}
		if (numTool == 1 && numBuf == 0)
		{
			return 170f;
		}
		if (numTool == 0 && numBuf > 0)
		{
			return 120f;
		}
		return 210f;
	}
}
