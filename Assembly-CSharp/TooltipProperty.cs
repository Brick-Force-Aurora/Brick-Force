using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TooltipProperty
{
	public TItem tItem;

	public Texture2D gauge;

	public Texture2D gaugeFrame;

	public Texture2D[] gaugeTiers;

	private bool isShop;

	private Item item;

	private float maxAtkPow = 100f;

	private float maxRpm = 1000f;

	private float maxRecoilPitch = 5f;

	private float maxMobility = 2f;

	private float maxSlashSpeed = 3f;

	private float maxRange = 25f;

	private float maxRangeBang = 30f;

	private float maxEffTime = 8f;

	private Vector2 crdExtra = new Vector2(265f, 140f);

	private Vector2 curUIPos = Vector2.zero;

	private bool bCalcHeight;

	private float txtLineOffset = 16f;

	public float categoryPosX;

	public float categoryPosY;

	public TextAnchor categoryAnchor = TextAnchor.UpperRight;

	public string categoryLabelType = "MiniLabel";

	private float titlex;

	private float titley;

	private float gaugex;

	private float gaugey;

	private float outgaugex;

	private float outgaugey;

	private float outputx;

	private float outputy;

	public float sizeX = 275f;

	public bool IsShop
	{
		get
		{
			return isShop;
		}
		set
		{
			isShop = value;
		}
	}

	public void SetItem(Item _item)
	{
		item = _item;
	}

	public void Start()
	{
		titlex = sizeX - 120f;
		outgaugex = sizeX - 115f;
		gaugex = sizeX - 112f;
		outputx = sizeX - 60f;
		categoryPosX = sizeX - 10f;
		crdExtra.x = sizeX - 10f;
		item = null;
		tItem = null;
	}

	public float DoPropertyGuage(float ypos)
	{
		curUIPos.y = ypos;
		if (tItem != null)
		{
			switch (tItem.type)
			{
			case TItem.TYPE.WEAPON:
				DoWeapon((TWeapon)tItem);
				break;
			case TItem.TYPE.CLOTH:
				DoCostume((TCostume)tItem);
				break;
			case TItem.TYPE.ACCESSORY:
				DoAccessory((TAccessory)tItem);
				break;
			case TItem.TYPE.CHARACTER:
				DoCharacter((TCharacter)tItem);
				break;
			case TItem.TYPE.BUNDLE:
				DoBundle((TBundle)tItem);
				break;
			case TItem.TYPE.SPECIAL:
				DoSpecial((TSpecial)tItem);
				break;
			}
		}
		return curUIPos.y;
	}

	public float GagueHeight(float ypos)
	{
		bCalcHeight = true;
		curUIPos.y = ypos;
		if (tItem != null)
		{
			switch (tItem.type)
			{
			case TItem.TYPE.WEAPON:
				DoWeapon((TWeapon)tItem);
				break;
			case TItem.TYPE.CLOTH:
				DoCostume((TCostume)tItem);
				break;
			case TItem.TYPE.ACCESSORY:
				DoAccessory((TAccessory)tItem);
				break;
			case TItem.TYPE.CHARACTER:
				DoCharacter((TCharacter)tItem);
				break;
			case TItem.TYPE.BUNDLE:
				DoBundle((TBundle)tItem);
				break;
			case TItem.TYPE.SPECIAL:
				DoSpecial((TSpecial)tItem);
				break;
			}
		}
		bCalcHeight = false;
		return curUIPos.y;
	}

	private void DoMeleeProperty(TWeapon tWeapon, MeleeWeapon melee)
	{
		if (null == melee)
		{
			Debug.LogError("Error, Fail to get MeleeWeapon component from prefab ");
		}
		else if (bCalcHeight)
		{
			curUIPos.y += 100f;
			if (BuildOption.Instance.Props.useDurability)
			{
				curUIPos.y += 20f;
			}
		}
		else
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)melee.weaponBy);
			float num = (wpnMod?.fAtkPow ?? melee.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fSlashSpeed ?? melee.slashSpeed) / maxSlashSpeed;
			float num3 = (wpnMod?.fSpeedFactor ?? melee.speedFactor) / maxMobility;
			float num4 = 0f;
			float num5 = 1f;
			num4 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			float num6 = num * 100f;
			float num7 = num2 * 100f;
			float num8 = num5 * 100f;
			float num9 = num3 * 100f;
			float num10 = num4 * 100f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			titley = curUIPos.y + 10f;
			LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("ACCURACY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("ATK_SPEED"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(new Vector2(titlex, titley + 100f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			outgaugey = titley - 8f;
			TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 100f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
			}
			gaugey = titley - 5f;
			float num11 = 0f;
			float num12 = 0f;
			string str = string.Empty;
			string str2 = string.Empty;
			if (!isShop)
			{
				string str3 = StringMgr.Instance.Get("UP");
				int num13 = 0;
				int num14 = 0;
				int grade = item.upgradeProps[num14].grade;
				if (grade > 0)
				{
					float num15 = PimpManager.Instance.getValue(5, num14, grade - 1) / maxAtkPow;
					if (num15 > 1f)
					{
						num15 = 1f;
					}
					num11 = num15 * 100f;
					float num16 = num + num15;
					if (num16 > 1f)
					{
						num16 = 1f;
					}
					if (item.upgradeProps[num14].grade > 0)
					{
						num13 = gaugeID(item.upgradeProps[num14].grade);
						TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num16, 12f), GlobalVars.Instance.gaugeTiers[num13], ScaleMode.StretchToFill);
						str = "(" + item.upgradeProps[num14].grade.ToString() + str3 + ")";
					}
				}
				num14 = 5;
				int grade2 = item.upgradeProps[num14].grade;
				if (grade2 > 0)
				{
					float value = PimpManager.Instance.getValue(5, num14, grade2 - 1);
					float num17 = value / maxSlashSpeed;
					num12 = num17 * 100f;
					float num18 = num2 + num17;
					if (num18 > 1f)
					{
						num18 = 1f;
					}
					if (item.upgradeProps[num14].grade > 0)
					{
						num13 = gaugeID(item.upgradeProps[num14].grade);
						TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num18, 12f), GlobalVars.Instance.gaugeTiers[num13], ScaleMode.StretchToFill);
						str2 = "(" + item.upgradeProps[num14].grade.ToString() + str3 + ")";
					}
				}
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num5, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 100f, 100f * num4, 12f), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			float num19 = titley;
			LabelUtil.TextOut(new Vector2(outputx, num19), (num6 + num11).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(outputx, num19 + 20f), num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(outputx, num19 + 40f), (num7 + num12).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(outputx, num19 + 60f), StringMgr.Instance.Get("INFINITE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(outputx, num19 + 80f), num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(new Vector2(outputx, num19 + 100f), num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			curUIPos.y += 100f;
			if (BuildOption.Instance.Props.useDurability)
			{
				curUIPos.y += 20f;
			}
		}
	}

	private void DoSmokeGrenade(TWeapon tWeapon, SmokeGrenade grenade)
	{
		if (null != grenade)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 60f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
				float num = grenade.persistTime / maxEffTime;
				float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
				float num3 = 0f;
				num3 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				float num4 = num * 100f;
				float num5 = num2 * 100f;
				float num6 = num3 * 100f;
				if (num > 1f)
				{
					num = 1f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("EFFECT_TIME"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				gaugey = titley - 5f;
				float num7 = 0f;
				string str = string.Empty;
				if (!isShop)
				{
					string str2 = StringMgr.Instance.Get("UP");
					int num8 = 0;
					int num9 = 8;
					int grade = item.upgradeProps[num9].grade;
					if (grade > 0)
					{
						float num10 = PimpManager.Instance.getValue(7, num9, grade - 1) / maxEffTime;
						if (num10 > 1f)
						{
							num10 = 1f;
						}
						num7 = num10 * 100f;
						float num11 = num + num10;
						if (num11 > 1f)
						{
							num11 = 1f;
						}
						if (item.upgradeProps[num9].grade > 0)
						{
							num8 = gaugeID(item.upgradeProps[num9].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num11, 12f), GlobalVars.Instance.gaugeTiers[num8], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num9].grade.ToString() + str2 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num4 + num7).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				curUIPos.y += 60f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private void DoXmasBomb(TWeapon tWeapon, XmasBomb xmas, GdgtXmasBomb gdgtXmas)
	{
		bool flag = false;
		if (null != gdgtXmas)
		{
			flag = (gdgtXmas.smoke != null);
		}
		if (null != xmas)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 80f;
				if (flag)
				{
					curUIPos.y += 20f;
				}
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)xmas.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = xmas.maxAmmo);
				float num = xmas.persistTime / maxEffTime;
				float num2 = (wpnMod?.fAtkPow ?? xmas.AtkPow) / maxAtkPow;
				float num3 = (wpnMod?.fSpeedFactor ?? xmas.speedFactor) / maxMobility;
				float num4 = xmas.Radius / maxRange;
				float num5 = 0f;
				num5 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num5 < 0f)
				{
					num5 = 0f;
				}
				float num6 = num * 100f;
				float num7 = num2 * 100f;
				float num8 = num4 * 100f;
				float num9 = num3 * 100f;
				float num10 = num5 * 100f;
				if (num > 1f)
				{
					num = 1f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num > 1f)
				{
					num = 1f;
				}
				titley = curUIPos.y + 10f;
				if (flag)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("EFFECT_TIME"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					if (BuildOption.Instance.Props.useDurability)
					{
						LabelUtil.TextOut(new Vector2(titlex, titley + 100f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					}
				}
				else
				{
					LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					if (BuildOption.Instance.Props.useDurability)
					{
						LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					}
				}
				outgaugey = titley - 8f;
				if (flag)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					if (BuildOption.Instance.Props.useDurability)
					{
						TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 100f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					}
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					if (BuildOption.Instance.Props.useDurability)
					{
						TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
					}
				}
				gaugey = titley - 5f;
				float num11 = 0f;
				float num12 = 0f;
				float num13 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				string empty = string.Empty;
				if (!isShop)
				{
					string str3 = StringMgr.Instance.Get("UP");
					int num14 = 0;
					int num15 = 0;
					int grade = item.upgradeProps[num15].grade;
					if (grade > 0)
					{
						float num16 = PimpManager.Instance.getValue(6, num15, grade - 1) / maxAtkPow;
						if (num16 > 1f)
						{
							num16 = 1f;
						}
						num12 = num16 * 100f;
						float num17 = num2 + num16;
						if (num17 > 1f)
						{
							num17 = 1f;
						}
						if (item.upgradeProps[num15].grade > 0)
						{
							num14 = gaugeID(item.upgradeProps[num15].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num17, 12f), GlobalVars.Instance.gaugeTiers[num14], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num15].grade.ToString() + str3 + ")";
						}
					}
					num15 = 7;
					int grade2 = item.upgradeProps[num15].grade;
					if (grade2 > 0)
					{
						float num18 = PimpManager.Instance.getValue(6, num15, grade2 - 1) / maxRange;
						if (num18 > 1f)
						{
							num18 = 1f;
						}
						num13 = num18 * 100f;
						float num19 = num4 + num18;
						if (num19 > 1f)
						{
							num19 = 1f;
						}
						if (item.upgradeProps[num15].grade > 0)
						{
							num14 = gaugeID(item.upgradeProps[num15].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num19, 12f), GlobalVars.Instance.gaugeTiers[num14], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num15].grade.ToString() + str3 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				if (flag)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num4, 12f), gauge, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
					if (BuildOption.Instance.Props.useDurability)
					{
						TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 100f, Mathf.Min(100f * num5, 100f), 12f), gauge, ScaleMode.StretchToFill);
					}
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num4, 12f), gauge, ScaleMode.StretchToFill);
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
					if (BuildOption.Instance.Props.useDurability)
					{
						TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, Mathf.Min(100f * num5, 100f), 12f), gauge, ScaleMode.StretchToFill);
					}
				}
				GUI.color = color;
				outputy = titley;
				if (flag)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy), (num6 + num11).ToString("0.##") + "%" + empty, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num7 + num12).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), (num8 + num13).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					if (BuildOption.Instance.Props.useDurability)
					{
						LabelUtil.TextOut(new Vector2(outputx, outputy + 100f), num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
				}
				else
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy), (num7 + num12).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num8 + num13).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					if (BuildOption.Instance.Props.useDurability)
					{
						LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
				}
				curUIPos.y += 80f;
				if (flag)
				{
					curUIPos.y += 20f;
				}
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private void DoGrenade(TWeapon tWeapon, Grenade grenade)
	{
		if (null != grenade)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
				float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
				float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
				float num3 = grenade.Radius / maxRange;
				float num4 = 0f;
				num4 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				float num5 = num * 100f;
				float num6 = num3 * 100f;
				float num7 = num2 * 100f;
				float num8 = num4 * 100f;
				if (num > 1f)
				{
					num = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				gaugey = titley - 5f;
				float num9 = 0f;
				float num10 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				if (!isShop)
				{
					string str3 = StringMgr.Instance.Get("UP");
					int num11 = 0;
					int num12 = 0;
					int grade = item.upgradeProps[num12].grade;
					if (grade > 0)
					{
						float num13 = PimpManager.Instance.getValue(6, num12, grade - 1) / maxAtkPow;
						if (num13 > 1f)
						{
							num13 = 1f;
						}
						num9 = num13 * 100f;
						float num14 = num + num13;
						if (num14 > 1f)
						{
							num14 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num14, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
					num12 = 7;
					int grade2 = item.upgradeProps[num12].grade;
					if (grade2 > 0)
					{
						float num15 = PimpManager.Instance.getValue(6, num12, grade2 - 1) / maxRange;
						if (num15 > 1f)
						{
							num15 = 1f;
						}
						num10 = num15 * 100f;
						float num16 = num3 + num15;
						if (num16 > 1f)
						{
							num16 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num16, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, Mathf.Min(100f * num4, 100f), 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num5 + num9).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num6 + num10).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private void DoFlashbang(TWeapon tWeapon, FlashBang grenade)
	{
		if (null != grenade)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
				float num = GlobalVars.Instance.maxDistanceFlashbang / maxRangeBang;
				float num2 = (wpnMod?.explosionTime ?? grenade.explosionTime) / maxEffTime;
				float num3 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
				float num4 = 0f;
				num4 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				float num5 = num2 * 100f;
				float num6 = num * 100f;
				float num7 = num3 * 100f;
				float num8 = num4 * 100f;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (num > 1f)
				{
					num = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("EFFECT_TIME"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				gaugey = titley - 5f;
				float num9 = 0f;
				float num10 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				if (!isShop)
				{
					string str3 = StringMgr.Instance.Get("UP");
					int num11 = 0;
					int num12 = 7;
					int grade = item.upgradeProps[num12].grade;
					if (grade > 0)
					{
						float num13 = PimpManager.Instance.getValue(7, num12, grade - 1) / maxRangeBang;
						if (num13 > 1f)
						{
							num13 = 1f;
						}
						num10 = num13 * 100f;
						float num14 = num + num13;
						if (num14 > 1f)
						{
							num14 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num14, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
					num12 = 8;
					int grade2 = item.upgradeProps[num12].grade;
					if (grade2 > 0)
					{
						float num15 = PimpManager.Instance.getValue(7, num12, grade2 - 1) / maxEffTime;
						if (num15 > 1f)
						{
							num15 = 1f;
						}
						num9 = num15 * 100f;
						float num16 = num2 + num15;
						if (num16 > 1f)
						{
							num16 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num16, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, 100f * num4, 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num6 + num10).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num5 + num9).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private void DoSenseBomb(TWeapon tWeapon, SenseBomb grenade)
	{
		if (null != grenade)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
				float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
				float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
				float num3 = grenade.Radius / maxRange;
				float num4 = 0f;
				num4 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				float num5 = num * 100f;
				float num6 = num3 * 100f;
				float num7 = num2 * 100f;
				float num8 = num4 * 100f;
				if (num > 1f)
				{
					num = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				gaugey = titley - 5f;
				float num9 = 0f;
				float num10 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				if (!isShop)
				{
					string str3 = StringMgr.Instance.Get("UP");
					int num11 = 0;
					int num12 = 0;
					int grade = item.upgradeProps[num12].grade;
					if (grade > 0)
					{
						float num13 = PimpManager.Instance.getValue(6, num12, grade - 1) / maxAtkPow;
						if (num13 > 1f)
						{
							num13 = 1f;
						}
						num9 = num13 * 100f;
						float num14 = num + num13;
						if (num14 > 1f)
						{
							num14 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num14, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
					num12 = 7;
					int grade2 = item.upgradeProps[num12].grade;
					if (grade2 > 0)
					{
						float num15 = PimpManager.Instance.getValue(6, num12, grade2 - 1) / maxRange;
						if (num15 > 1f)
						{
							num15 = 1f;
						}
						num10 = num15 * 100f;
						float num16 = num3 + num15;
						if (num16 > 1f)
						{
							num16 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num16, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, Mathf.Min(100f * num4, 100f), 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num5 + num9).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num6 + num10).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private void DoPoisonBomb(TWeapon tWeapon, PoisonBomb grenade)
	{
		if (null != grenade)
		{
			if (bCalcHeight)
			{
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
				int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
				float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
				float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
				float num3 = grenade.Radius / maxRange;
				float num4 = 0f;
				num4 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				float num5 = num * 100f;
				float num6 = num3 * 100f;
				float num7 = num2 * 100f;
				float num8 = num4 * 100f;
				if (num > 1f)
				{
					num = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("EFFECT_RADIUS"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				gaugey = titley - 5f;
				float num9 = 0f;
				float num10 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				if (!isShop)
				{
					string str3 = StringMgr.Instance.Get("UP");
					int num11 = 0;
					int num12 = 0;
					int grade = item.upgradeProps[num12].grade;
					if (grade > 0)
					{
						float num13 = PimpManager.Instance.getValue(6, num12, grade - 1) / maxAtkPow;
						if (num13 > 1f)
						{
							num13 = 1f;
						}
						num9 = num13 * 100f;
						float num14 = num + num13;
						if (num14 > 1f)
						{
							num14 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num14, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
					num12 = 7;
					int grade2 = item.upgradeProps[num12].grade;
					if (grade2 > 0)
					{
						float num15 = PimpManager.Instance.getValue(6, num12, grade2 - 1) / maxRange;
						if (num15 > 1f)
						{
							num15 = 1f;
						}
						num10 = num15 * 100f;
						float num16 = num3 + num15;
						if (num16 > 1f)
						{
							num16 = 1f;
						}
						if (item.upgradeProps[num12].grade > 0)
						{
							num11 = gaugeID(item.upgradeProps[num12].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num16, 12f), GlobalVars.Instance.gaugeTiers[num11], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num12].grade.ToString() + str3 + ")";
						}
					}
				}
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 80f, Mathf.Min(100f * num4, 100f), 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num5 + num9).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num6 + num10).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				curUIPos.y += 80f;
				if (BuildOption.Instance.Props.useDurability)
				{
					curUIPos.y += 20f;
				}
			}
		}
	}

	private int gaugeID(int grade)
	{
		if (grade >= 1 && grade < 4)
		{
			return 0;
		}
		if (grade >= 4 && grade < 7)
		{
			return 1;
		}
		return 2;
	}

	private void DoGunProperty(TWeapon tWeapon, Gun gun, Scope scope, Aim aim)
	{
		Weapon component = tWeapon.CurPrefab().GetComponent<Weapon>();
		if (null == gun || null == component)
		{
			Debug.LogError("Error, Fail to get gun component from prefab ");
		}
		else
		{
			float num = 0f;
			if (BuildOption.Instance.Props.useDurability)
			{
				num += 20f;
			}
			if (bCalcHeight)
			{
				curUIPos.y += 140f;
				curUIPos.y += num;
			}
			else
			{
				WpnMod wpnMod = WeaponModifier.Instance.Get((int)gun.weaponBy);
				float num2 = (wpnMod?.fAtkPow ?? gun.AtkPow) / maxAtkPow;
				float num3 = (wpnMod?.fRateOfFire ?? gun.rateOfFire) / maxRpm;
				float num4 = (wpnMod?.fRecoilPitch ?? gun.recoilPitch) / maxRecoilPitch;
				float num5 = (wpnMod?.fSpeedFactor ?? gun.speedFactor) / maxMobility;
				float num6 = 0f;
				float num7 = (!(null == scope)) ? (scope.accuracy.accuracy / 100f) : (aim.accuracy.accuracy / 100f);
				if (wpnMod != null)
				{
					num7 = ((!(null == scope)) ? (wpnMod.fZAccuracy / 100f) : (wpnMod.fAccuracy / 100f));
				}
				num6 = (isShop ? 1f : ((!((float)item.Durability < 0f) && !((float)tWeapon.durabilityMax < 0f)) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f));
				if (num6 < 0f)
				{
					num6 = 0f;
				}
				float num8 = num2 * 100f;
				float num9 = num3 * 100f;
				float num10 = num4 * 100f;
				float num11 = num5 * 100f;
				float num12 = num6 * 100f;
				float num13 = (float)(wpnMod?.maxAmmo ?? gun.maxAmmo);
				float num14 = (!(null == scope)) ? scope.accuracy.accuracy : aim.accuracy.accuracy;
				if (wpnMod != null)
				{
					num14 = ((!(null == scope)) ? wpnMod.fZAccuracy : wpnMod.fAccuracy);
				}
				float num15 = wpnMod?.fRange ?? component.range;
				float num16 = wpnMod?.effectiveRange ?? component.effectiveRange;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				if (num5 > 1f)
				{
					num5 = 1f;
				}
				if (num6 > 1f)
				{
					num6 = 1f;
				}
				if (num7 > 1f)
				{
					num7 = 1f;
				}
				titley = curUIPos.y + 10f;
				LabelUtil.TextOut(new Vector2(titlex, titley), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 20f), StringMgr.Instance.Get("ACCURACY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 40f), StringMgr.Instance.Get("RECOILPITCH"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 60f), StringMgr.Instance.Get("RPM"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 80f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				LabelUtil.TextOut(new Vector2(titlex, titley + 100f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(titlex, titley + 120f), StringMgr.Instance.Get("DURABILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				}
				LabelUtil.TextOut(new Vector2(titlex, titley + 120f + num), StringMgr.Instance.Get("EFFECTIVE_RANGE"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
				outgaugey = titley - 8f;
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 20f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 40f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 60f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 80f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 100f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 120f, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				}
				TextureUtil.DrawTexture(new Rect(outgaugex, outgaugey + 120f + num, 106f, 18f), gaugeFrame, ScaleMode.StretchToFill);
				gaugey = titley - 5f;
				Color color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 40f, 100f * num4, 12f), gauge, ScaleMode.StretchToFill);
				GUI.color = color;
				float num17 = 0f;
				float num18 = 0f;
				float num19 = 0f;
				float num20 = 0f;
				float num21 = 0f;
				string str = string.Empty;
				string str2 = string.Empty;
				string str3 = string.Empty;
				string str4 = string.Empty;
				string str5 = string.Empty;
				if (!isShop)
				{
					string str6 = StringMgr.Instance.Get("UP");
					int num22 = 0;
					int num23 = 0;
					int grade = item.upgradeProps[num23].grade;
					if (grade > 0)
					{
						float num24 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num23, grade - 1) / maxAtkPow;
						if (num24 > 1f)
						{
							num24 = 1f;
						}
						num17 = num24 * 100f;
						float num25 = num2 + num24;
						if (num25 > 1f)
						{
							num25 = 1f;
						}
						if (item.upgradeProps[num23].grade > 0)
						{
							num22 = gaugeID(item.upgradeProps[num23].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num25, 12f), GlobalVars.Instance.gaugeTiers[num22], ScaleMode.StretchToFill);
							str = "(" + item.upgradeProps[num23].grade.ToString() + str6 + ")";
						}
					}
					num23 = 1;
					int grade2 = item.upgradeProps[num23].grade;
					if (grade2 > 0)
					{
						float num26 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num23, grade2 - 1) / 100f;
						if (num26 > 1f)
						{
							num26 = 1f;
						}
						num18 = num26 * 100f;
						float num27 = num7 + num26;
						if (num27 > 1f)
						{
							num27 = 1f;
						}
						if (item.upgradeProps[num23].grade > 0)
						{
							num22 = gaugeID(item.upgradeProps[num23].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num27, 12f), GlobalVars.Instance.gaugeTiers[num22], ScaleMode.StretchToFill);
							str2 = "(" + item.upgradeProps[num23].grade.ToString() + str6 + ")";
						}
					}
					num23 = 2;
					int grade3 = item.upgradeProps[num23].grade;
					if (grade3 > 0)
					{
						num19 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num23, grade3 - 1);
						if (item.upgradeProps[num23].grade > 0)
						{
							str3 = "(" + item.upgradeProps[num23].grade.ToString() + str6 + ")";
						}
					}
					num23 = 3;
					int grade4 = item.upgradeProps[num23].grade;
					if (grade4 > 0)
					{
						float num28 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num23, grade4 - 1) / maxRpm;
						if (num28 > 1f)
						{
							num28 = 1f;
						}
						num20 = num28 * 100f;
						float num29 = num3 + num28;
						if (num29 > 1f)
						{
							num29 = 1f;
						}
						if (item.upgradeProps[num23].grade > 0)
						{
							num22 = gaugeID(item.upgradeProps[num23].grade);
							TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num29, 12f), GlobalVars.Instance.gaugeTiers[num22], ScaleMode.StretchToFill);
							str4 = "(" + item.upgradeProps[num23].grade.ToString() + str6 + ")";
						}
					}
					num23 = 4;
					int grade5 = item.upgradeProps[num23].grade;
					if (grade5 > 0)
					{
						num21 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num23, grade5 - 1);
						str5 = "(" + item.upgradeProps[num23].grade.ToString() + str6 + ")";
					}
					if (grade3 > 0)
					{
						float num30 = num19 / maxRecoilPitch;
						num19 = num30 * 100f;
						float num31 = Mathf.Min(num4, 1f) + num30;
						num31 *= 100f;
						num30 = 0f - num30;
						num23 = 2;
						num22 = gaugeID(item.upgradeProps[num23].grade);
						TextureUtil.DrawTexture(new Rect(gaugex + num31, gaugey + 40f, 100f * num30, 12f), GlobalVars.Instance.gaugeTiers[num22], ScaleMode.StretchToFill);
					}
				}
				color = GUI.color;
				GUI.color = Color.yellow;
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey, 100f * num2, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 20f, 100f * num7, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 60f, 100f * num3, 12f), gauge, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 100f, 100f * num5, 12f), gauge, ScaleMode.StretchToFill);
				if (BuildOption.Instance.Props.useDurability)
				{
					TextureUtil.DrawTexture(new Rect(gaugex, gaugey + 120f, 100f * num6, 12f), gauge, ScaleMode.StretchToFill);
				}
				GUI.color = color;
				outputy = titley;
				LabelUtil.TextOut(new Vector2(outputx, outputy), (num8 + num17).ToString("0.##") + "%" + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 20f), (num14 + num18).ToString("0.##") + "%" + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 40f), (num10 + num19).ToString("0.##") + "%" + str3, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 60f), (num9 + num20).ToString("0.##") + "%" + str4, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 80f), (num13 + num21).ToString() + str5, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(outputx, outputy + 100f), num11.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (BuildOption.Instance.Props.useDurability)
				{
					LabelUtil.TextOut(new Vector2(outputx, outputy + 120f), num12.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
				LabelUtil.TextOut(new Vector2(outputx, outputy + 120f + num), num16.ToString() + "/" + num15.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				curUIPos.y += 140f;
				curUIPos.y += num;
			}
		}
	}

	private void DoWeapon(TWeapon tWeapon)
	{
		if (tWeapon.CurPrefab() != null)
		{
			Weapon component = tWeapon.CurPrefab().GetComponent<Weapon>();
			if (null == component)
			{
				Debug.LogError("Error, Fail to get weapon component from prefab ");
			}
			else
			{
				string str = string.Empty;
				switch (tWeapon.cat)
				{
				case 0:
					str = StringMgr.Instance.Get("HEAVY_WPN");
					break;
				case 1:
					str = StringMgr.Instance.Get("ASSAULT_WPN");
					break;
				case 2:
					str = StringMgr.Instance.Get("SNIPER_WPN");
					break;
				case 3:
					str = StringMgr.Instance.Get("SUB_MACHINE_WPN");
					break;
				case 4:
					str = StringMgr.Instance.Get("HAND_GUN_WPN");
					break;
				case 5:
					str = StringMgr.Instance.Get("MELEE_WPN");
					break;
				case 6:
					str = StringMgr.Instance.Get("SPECIAL_WPN");
					break;
				}
				switch (component.slot)
				{
				case Weapon.TYPE.MAIN:
					LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("MAIN_WEAPON") + "/" + str, categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
					DoGunProperty(tWeapon, tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.AUX:
					LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("AUX_WEAPON") + "/" + str, categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
					DoGunProperty(tWeapon, tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.MELEE:
					LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("MELEE_WEAPON") + "/" + str, categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
					DoMeleeProperty(tWeapon, tWeapon.CurPrefab().GetComponent<MeleeWeapon>());
					break;
				case Weapon.TYPE.PROJECTILE:
				{
					LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("SPEC_WEAPON") + "/" + str, categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
					Grenade component2 = tWeapon.CurPrefab().GetComponent<Grenade>();
					if (component2 != null)
					{
						DoGrenade(tWeapon, component2);
					}
					FlashBang component3 = tWeapon.CurPrefab().GetComponent<FlashBang>();
					if (component3 != null)
					{
						DoFlashbang(tWeapon, component3);
					}
					SmokeGrenade component4 = tWeapon.CurPrefab().GetComponent<SmokeGrenade>();
					if (component4 != null)
					{
						DoSmokeGrenade(tWeapon, component4);
					}
					SenseBomb component5 = tWeapon.CurPrefab().GetComponent<SenseBomb>();
					if (component5 != null)
					{
						DoSenseBomb(tWeapon, component5);
					}
					XmasBomb component6 = tWeapon.CurPrefab().GetComponent<XmasBomb>();
					GdgtXmasBomb component7 = tWeapon.CurPrefab().GetComponent<GdgtXmasBomb>();
					if (component6 != null)
					{
						DoXmasBomb(tWeapon, component6, component7);
					}
					PoisonBomb component8 = tWeapon.CurPrefab().GetComponent<PoisonBomb>();
					if (component8 != null)
					{
						DoPoisonBomb(tWeapon, component8);
					}
					break;
				}
				}
			}
		}
	}

	private void DoCostume(TCostume tCostume)
	{
		switch (tCostume.slot)
		{
		case TItem.SLOT.UPPER:
			LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("UPPER_CLOTH"), categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
			break;
		case TItem.SLOT.LOWER:
			LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), StringMgr.Instance.Get("LOWER_CLOTH"), categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
			break;
		}
		float num = 0f;
		int num2 = 0;
		if (item != null && tCostume.upgradeCategory >= TItem.UPGRADE_CATEGORY.HEAVY)
		{
			int num3 = 11;
			num2 = item.upgradeProps[num3].grade;
			if (num2 > 0)
			{
				num = PimpManager.Instance.getValue((int)tCostume.upgradeCategory, num3, num2 - 1);
			}
		}
		int num4 = tCostume.armor + (int)num;
		string text = string.Format(StringMgr.Instance.Get("ARMOR_UP"), num4);
		if (num > 0f)
		{
			string text2 = text;
			text = text2 + " ( " + num2.ToString() + StringMgr.Instance.Get("UP") + " )";
		}
		crdExtra.y = curUIPos.y;
		if (num4 > 0 && BuildOption.Instance.Props.useArmor)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 20 && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 21)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 22)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 23)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 88)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("DASHTIME"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 89)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("RESPAWNTIME"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tCostume.functionMask == 90)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), tCostume.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		DoBuff(curUIPos.y, (int)tCostume.upgradeCategory);
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tCostume.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset * (float)consumableDesc.disableByRoomType.Length;
			}
			else
			{
				for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
				{
					LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					curUIPos.y += txtLineOffset;
				}
			}
		}
	}

	private void DoAccessory(TAccessory tAccessory)
	{
		Accessory component = tAccessory.prefab.GetComponent<Accessory>();
		if (null == component)
		{
			Debug.LogError("Error, Fail to get equip component from accessory prefab ");
		}
		else if (!bCalcHeight)
		{
			LabelUtil.TextOut(new Vector2(categoryPosX, categoryPosY), tAccessory.GetKindString(), categoryLabelType, Color.white, GlobalVars.txtEmptyColor, categoryAnchor);
		}
		int num = 0;
		int num2 = 0;
		if (item != null && tAccessory.upgradeCategory >= TItem.UPGRADE_CATEGORY.HEAVY)
		{
			int num3 = 11;
			num = item.upgradeProps[num3].grade;
			if (num > 0)
			{
				num2 = (int)PimpManager.Instance.getValue((int)tAccessory.upgradeCategory, num3, num - 1);
			}
		}
		string text = string.Format(StringMgr.Instance.Get("ARMOR_UP"), tAccessory.armor + num2);
		if (num2 > 0)
		{
			string text2 = text;
			text = text2 + " ( " + num.ToString() + StringMgr.Instance.Get("UP") + " )";
		}
		crdExtra.y = curUIPos.y;
		if ((tAccessory.armor > 0 || num2 > 0) && BuildOption.Instance.Props.useArmor)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 20 && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 21)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 22)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 23)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 113)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP02"), tAccessory.functionFactor), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 88)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("DASHTIME"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 89)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("RESPAWNTIME"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		if (tAccessory.functionMask == 90)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), tAccessory.functionFactor * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		DoBuff(curUIPos.y, (component.cat != 0) ? 11 : 10);
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tAccessory.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset * (float)consumableDesc.disableByRoomType.Length;
			}
			else
			{
				for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
				{
					LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					curUIPos.y += txtLineOffset;
				}
			}
		}
	}

	private void DoSpecial(TSpecial tSpecial)
	{
		crdExtra.y = curUIPos.y;
		if (tSpecial.IsConsumableBuff)
		{
			TBuff tBuff = BuffManager.Instance.Get(tSpecial.param);
			if (tBuff != null)
			{
				if (tBuff.IsPoint)
				{
					float num = (float)tBuff.PointRatio;
					string text = string.Format(StringMgr.Instance.Get("POINT_UP"), num);
					if (num > 0f)
					{
						if (bCalcHeight)
						{
							curUIPos.y += txtLineOffset;
						}
						else
						{
							LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
							curUIPos.y += txtLineOffset;
						}
					}
				}
				if (tBuff.IsXp)
				{
					float num2 = (float)tBuff.XpRatio;
					string text2 = string.Format(StringMgr.Instance.Get("XP_UP"), num2);
					if (num2 > 0f)
					{
						if (bCalcHeight)
						{
							curUIPos.y += txtLineOffset;
						}
						else
						{
							LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text2, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
							curUIPos.y += txtLineOffset;
						}
					}
				}
				if (tBuff.IsLuck)
				{
					float num3 = (float)tBuff.Luck;
					string text3 = string.Format(StringMgr.Instance.Get("LUCK_UP"), num3);
					if (num3 > 0f)
					{
						if (bCalcHeight)
						{
							curUIPos.y += txtLineOffset;
						}
						else
						{
							LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text3, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
							curUIPos.y += txtLineOffset;
						}
					}
				}
			}
		}
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tSpecial.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset * (float)consumableDesc.disableByRoomType.Length;
			}
			else
			{
				for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
				{
					LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					curUIPos.y += txtLineOffset;
				}
			}
		}
	}

	private void DoCharacter(TCharacter tCharacter)
	{
		crdExtra.y = curUIPos.y;
		DoBuff(crdExtra.y, -1);
	}

	private BundleUnit[] sortBundlePacks(BundleUnit[] packs)
	{
		List<BundleUnit> list = new List<BundleUnit>();
		for (int i = 0; i < packs.Length; i++)
		{
			if (packs[i].tItem.catType == 0)
			{
				list.Add(packs[i]);
			}
		}
		for (int j = 0; j < packs.Length; j++)
		{
			if (packs[j].tItem.catType == 1)
			{
				list.Add(packs[j]);
			}
		}
		for (int k = 0; k < packs.Length; k++)
		{
			if (packs[k].tItem.catType == 2)
			{
				list.Add(packs[k]);
			}
		}
		for (int l = 0; l < packs.Length; l++)
		{
			if (packs[l].tItem.catType == 3 && packs[l].tItem.catKind == 5)
			{
				list.Add(packs[l]);
			}
		}
		for (int m = 0; m < packs.Length; m++)
		{
			if (packs[m].tItem.catType == 3 && packs[m].tItem.catKind == 4)
			{
				list.Add(packs[m]);
			}
		}
		for (int n = 0; n < packs.Length; n++)
		{
			if (packs[n].tItem.catType == 3 && packs[n].tItem.catKind == 7)
			{
				list.Add(packs[n]);
			}
		}
		for (int num = 0; num < packs.Length; num++)
		{
			if (packs[num].tItem.catType == 3 && packs[num].tItem.catKind == 1)
			{
				list.Add(packs[num]);
			}
		}
		for (int num2 = 0; num2 < packs.Length; num2++)
		{
			if (packs[num2].tItem.catType == 3 && packs[num2].tItem.catKind == 2)
			{
				list.Add(packs[num2]);
			}
		}
		for (int num3 = 0; num3 < packs.Length; num3++)
		{
			if (packs[num3].tItem.catType == 3 && packs[num3].tItem.catKind == 3)
			{
				list.Add(packs[num3]);
			}
		}
		for (int num4 = 0; num4 < packs.Length; num4++)
		{
			if (packs[num4].tItem.catType == 3 && packs[num4].tItem.catKind == 6)
			{
				list.Add(packs[num4]);
			}
		}
		for (int num5 = 0; num5 < packs.Length; num5++)
		{
			if (packs[num5].tItem.catType == 3 && packs[num5].tItem.catKind == 8)
			{
				list.Add(packs[num5]);
			}
		}
		for (int num6 = 0; num6 < packs.Length; num6++)
		{
			if (packs[num6].tItem.catType == 4)
			{
				list.Add(packs[num6]);
			}
		}
		for (int num7 = 0; num7 < packs.Length; num7++)
		{
			if (packs[num7].tItem.catType == 5)
			{
				list.Add(packs[num7]);
			}
		}
		for (int num8 = 0; num8 < packs.Length; num8++)
		{
			if (packs[num8].tItem.catType == 6)
			{
				list.Add(packs[num8]);
			}
		}
		for (int num9 = 0; num9 < packs.Length; num9++)
		{
			if (packs[num9].tItem.catType == 7)
			{
				list.Add(packs[num9]);
			}
		}
		for (int num10 = 0; num10 < packs.Length; num10++)
		{
			if (packs[num10].tItem.catType == 8)
			{
				list.Add(packs[num10]);
			}
		}
		for (int num11 = 0; num11 < packs.Length; num11++)
		{
			if (packs[num11].tItem.catType == 9)
			{
				list.Add(packs[num11]);
			}
		}
		return list.ToArray();
	}

	private void DoBundle(TBundle tBundle)
	{
		if (tBundle != null)
		{
			BundleUnit[] array = sortBundlePacks(BundleManager.Instance.Unpack(tBundle.code));
			if (array != null)
			{
				crdExtra.y = curUIPos.y;
				Vector2 pos = crdExtra;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].tItem != null)
					{
						string empty = string.Empty;
						if (array[i].opt >= 1000000)
						{
							empty = StringMgr.Instance.Get("INFINITE");
						}
						else
						{
							empty = array[i].opt.ToString();
							empty = ((!array[i].tItem.IsAmount) ? (empty + StringMgr.Instance.Get("DAYS")) : (empty + StringMgr.Instance.Get("TIMES_UNIT")));
						}
						LabelUtil.TextOut(pos, array[i].tItem.Name + "/" + empty, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
						pos.y += txtLineOffset;
						curUIPos.y += txtLineOffset;
					}
				}
			}
		}
	}

	public float DoBuff(float y, int cat)
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		if (item != null && cat >= 0)
		{
			num = 10;
			num2 = item.upgradeProps[num].grade;
			if (num2 > 0)
			{
				num3 = PimpManager.Instance.getValue(cat, num, num2 - 1);
			}
		}
		float num4 = num3;
		if (tItem.tBuff != null)
		{
			num4 += (float)tItem.tBuff.PointRatio;
		}
		string text = string.Format(StringMgr.Instance.Get("POINT_UP"), num4);
		if (num3 > 0f)
		{
			string text2 = text;
			text = text2 + " ( " + num2.ToString() + StringMgr.Instance.Get("UP") + " )";
		}
		if (num4 > 0f)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		num3 = 0f;
		if (item != null && cat >= 0)
		{
			num = 9;
			num2 = item.upgradeProps[num].grade;
			if (num2 > 0)
			{
				num3 = PimpManager.Instance.getValue(cat, num, num2 - 1);
			}
		}
		float num5 = num3;
		if (tItem.tBuff != null)
		{
			num5 += (float)tItem.tBuff.XpRatio;
		}
		text = string.Format(StringMgr.Instance.Get("XP_UP"), num5);
		if (num3 > 0f)
		{
			string text2 = text;
			text = text2 + " ( " + num2.ToString() + StringMgr.Instance.Get("UP") + " )";
		}
		if (num5 > 0f)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		num3 = 0f;
		if (item != null && cat >= 0)
		{
			num = 12;
			num2 = item.upgradeProps[num].grade;
			if (num2 > 0)
			{
				num3 = PimpManager.Instance.getValue(cat, num, num2 - 1);
			}
		}
		float num6 = num3;
		if (tItem.tBuff != null)
		{
			num6 += (float)tItem.tBuff.Luck;
		}
		text = string.Format(StringMgr.Instance.Get("LUCK_UP"), num6);
		if (num3 > 0f)
		{
			string text2 = text;
			text = text2 + " ( " + num2.ToString() + StringMgr.Instance.Get("UP") + " )";
		}
		if (num6 > 0f)
		{
			if (bCalcHeight)
			{
				curUIPos.y += txtLineOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(sizeX - 10f, curUIPos.y), text, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				curUIPos.y += txtLineOffset;
			}
		}
		return curUIPos.y;
	}
}
