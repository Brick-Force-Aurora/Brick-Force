using System;
using UnityEngine;

[Serializable]
public class RandomBoxConfirmDialog : Dialog
{
	public Texture2D[] resultEventFrame;

	public Texture2D[] resultEventLight;

	public Texture2D gauge;

	public Texture2D gaugeFrame;

	private Rect crdBtnConfirm = new Rect(370f, 300f, 100f, 34f);

	private Rect crdIcon = new Rect(70f, 160f, 167f, 91f);

	public Rect crdOutline = new Rect(50f, 160f, 200f, 100f);

	public Vector2 crdName = new Vector2(50f, 160f);

	public Vector2 crdExpl = new Vector2(50f, 160f);

	public Vector2 crdDuration = new Vector2(50f, 160f);

	public Vector2 crdText1 = new Vector2(20f, 50f);

	public Vector2 crdText2 = new Vector2(20f, 80f);

	public Vector2 crdText3 = new Vector2(20f, 120f);

	public float maxAtkPow = 100f;

	public float maxRpm = 1000f;

	public float maxRecoilPitch = 5f;

	public float maxMobility = 2f;

	private string resultCode = string.Empty;

	private int resultRemain;

	private Vector2 crdResultEventLight = new Vector2(1024f, 1024f);

	private Vector2 crdResultEventFrame = new Vector2(610f, 448f);

	private int resultPhase;

	private float deltaTime;

	private Vector2 tmpSize = new Vector2(504f, 343f);

	private bool closeOnce;

	public bool CloseOnce
	{
		get
		{
			bool result = closeOnce;
			closeOnce = false;
			return result;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RANDOMBOX_CONFIRM;
	}

	public override void OnPopup()
	{
		rc = new Rect(0f, 0f, GlobalVars.Instance.ScreenRect.width, GlobalVars.Instance.ScreenRect.height);
	}

	public void InitDialog(string code, int remain)
	{
		resultCode = code;
		resultRemain = remain;
		closeOnce = false;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoResultEffect();
		GUI.BeginGroup(new Rect(((float)Screen.width - tmpSize.x) / 2f, ((float)Screen.height - tmpSize.y) / 2f, tmpSize.x, tmpSize.y));
		GUI.Box(new Rect(0f, 0f, tmpSize.x, tmpSize.y), string.Empty, "Window");
		GUI.Box(crdOutline, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdText1, StringMgr.Instance.Get("CONGRATULATION"), "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		string key = resultCode;
		TItem tItem = TItemManager.Instance.Get<TItem>(key);
		string optionStringByOption = tItem.GetOptionStringByOption(resultRemain / 86400);
		string text = string.Format(StringMgr.Instance.Get("TAKE_RANDOM_ITEM"), tItem.Name, optionStringByOption);
		LabelUtil.TextOut(crdText2, text, "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		LabelUtil.TextOut(crdText3, StringMgr.Instance.Get("RANDOMBOX_EXP1"), "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
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
		}
		if (tItem.CurIcon() == null)
		{
			Debug.LogError("Fail to get icon for item " + tItem.CurIcon());
		}
		else
		{
			TextureUtil.DrawTexture(crdIcon, tItem.CurIcon(), ScaleMode.StretchToFill);
		}
		LabelUtil.TextOut(crdName, tItem.Name, "Label", new Color(0.73f, 0.44f, 0.26f), GlobalVars.txtEmptyColor, TextAnchor.LowerLeft);
		LabelUtil.TextOut(crdDuration, optionStringByOption, "Label", new Color(0.93f, 0.55f, 0.33f), GlobalVars.txtEmptyColor, TextAnchor.LowerRight);
		if (GlobalVars.Instance.MyButton(crdBtnConfirm, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			result = true;
		}
		GUI.EndGroup();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		closeOnce = true;
	}

	private void DoGrenade(Grenade grenade)
	{
		if (null != grenade)
		{
			float num = grenade.AtkPow / maxAtkPow;
			float num2 = grenade.speedFactor / maxMobility;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			float num3 = num * 100f;
			float num4 = num2 * 100f;
			DoWeaponPropertyBase();
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(new Rect(333f, 155f, 104f * num, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 235f, 104f * num2, 12f), gauge, ScaleMode.StretchToFill);
			GUI.color = color;
			LabelUtil.TextOut(new Vector2(390f, 160f), num3.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 180f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 200f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 220f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 240f), num4.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 260f), grenade.maxAmmo.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private void DoHandBomb(HandBomb handBomb)
	{
		if (null != handBomb)
		{
			float num = handBomb.speedFactor / maxMobility;
			float num2 = num * 100f;
			DoWeaponPropertyBase();
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(new Rect(333f, 235f, 104f * num, 12f), gauge, ScaleMode.StretchToFill);
			GUI.color = color;
			LabelUtil.TextOut(new Vector2(390f, 160f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 180f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 200f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 220f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 240f), num2.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 260f), handBomb.maxAmmo.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private void DoGunProperty(Gun gun, Scope scope, Aim aim)
	{
		if (null == gun)
		{
			Debug.LogError("Error, Fail to get gun component from prefab ");
		}
		else
		{
			float num = gun.AtkPow / maxAtkPow;
			float num2 = gun.rateOfFire / maxRpm;
			float num3 = (!(null == scope)) ? (scope.accuracy.accuracy / 100f) : (aim.accuracy.accuracy / 100f);
			float num4 = gun.recoilPitch / maxRecoilPitch;
			float num5 = gun.speedFactor / maxMobility;
			float num6 = num * 100f;
			float num7 = num2 * 100f;
			float num8 = (!(null == scope)) ? scope.accuracy.accuracy : aim.accuracy.accuracy;
			float num9 = num4 * 100f;
			float num10 = num5 * 100f;
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
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			DoWeaponPropertyBase();
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(new Rect(333f, 155f, 104f * num, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 175f, 104f * num2, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 195f, 104f * num3, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 215f, 104f * num4, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 235f, 104f * num5, 12f), gauge, ScaleMode.StretchToFill);
			GUI.color = color;
			LabelUtil.TextOut(new Vector2(390f, 160f), num6.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 180f), num7.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 200f), num8.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 220f), num9.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 240f), num10.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 260f), gun.maxAmmo.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private void DoResultEffect()
	{
		Texture2D texture2D = resultEventLight[resultPhase % 2];
		if (null != texture2D)
		{
			float x = crdResultEventLight.x;
			float y = crdResultEventLight.y;
			GUI.DrawTexture(new Rect(((float)Screen.width - x) / 2f, ((float)Screen.height - y) / 2f, x, y), texture2D, ScaleMode.StretchToFill);
		}
		Texture2D texture2D2 = resultEventFrame[resultPhase % 2];
		if (null != texture2D2)
		{
			float x2 = crdResultEventFrame.x;
			float y2 = crdResultEventFrame.y;
			GUI.DrawTexture(new Rect(((float)Screen.width - x2) / 2f, ((float)Screen.height - y2) / 2f, x2, y2), texture2D2, ScaleMode.StretchToFill);
		}
	}

	public override void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.5f)
		{
			resultPhase++;
			deltaTime = 0f;
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
				switch (component.slot)
				{
				case Weapon.TYPE.MAIN:
					LabelUtil.TextOut(crdExpl, StringMgr.Instance.Get("MAIN_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoGunProperty(tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.AUX:
					LabelUtil.TextOut(crdExpl, StringMgr.Instance.Get("AUX_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoGunProperty(tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.MELEE:
					LabelUtil.TextOut(crdExpl, StringMgr.Instance.Get("MELEE_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoMeleeProperty(tWeapon.CurPrefab().GetComponent<MeleeWeapon>());
					break;
				case Weapon.TYPE.PROJECTILE:
				{
					LabelUtil.TextOut(crdExpl, StringMgr.Instance.Get("SPEC_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					Grenade component2 = tWeapon.CurPrefab().GetComponent<Grenade>();
					if (component2 != null)
					{
						DoGrenade(component2);
					}
					HandBomb component3 = tWeapon.CurPrefab().GetComponent<HandBomb>();
					if (component3 != null)
					{
						DoHandBomb(component3);
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
			LabelUtil.TextOut(new Vector2(0f, 0f), StringMgr.Instance.Get("UPPER_CLOTH"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			break;
		case TItem.SLOT.LOWER:
			LabelUtil.TextOut(new Vector2(0f, 0f), StringMgr.Instance.Get("LOWER_CLOTH"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			break;
		}
	}

	private void DoAccessory(TAccessory tAccessory)
	{
		Accessory component = tAccessory.prefab.GetComponent<Accessory>();
		if (null == component)
		{
			Debug.LogError("Error, Fail to get equip component from accessory prefab ");
		}
		else
		{
			LabelUtil.TextOut(new Vector2(0f, 0f), tAccessory.GetKindString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void DoCharacter(TCharacter tCharacter)
	{
	}

	private void DoMeleeProperty(MeleeWeapon melee)
	{
		if (null == melee)
		{
			Debug.LogError("Error, Fail to get MeleeWeapon component from prefab ");
		}
		else
		{
			float num = melee.AtkPow / maxAtkPow;
			float num2 = 1f;
			float num3 = melee.speedFactor / maxMobility;
			float num4 = num * 100f;
			float num5 = num2 * 100f;
			float num6 = num3 * 100f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			DoWeaponPropertyBase();
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(new Rect(333f, 155f, 104f * num, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 175f, 104f * num2, 12f), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(333f, 195f, 104f * num3, 12f), gauge, ScaleMode.StretchToFill);
			GUI.color = color;
			LabelUtil.TextOut(new Vector2(390f, 160f), num4.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 180f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 200f), num5.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 220f), "N/A", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 240f), num6.ToString("0.##") + "%", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(390f, 260f), StringMgr.Instance.Get("INFINITE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private void DoWeaponPropertyBase()
	{
		LabelUtil.TextOut(new Vector2(300f, 160f), StringMgr.Instance.Get("ATTACK_POWER"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(300f, 180f), StringMgr.Instance.Get("RPM"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(300f, 200f), StringMgr.Instance.Get("ACCURACY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(300f, 220f), StringMgr.Instance.Get("RECOILPITCH"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(300f, 240f), StringMgr.Instance.Get("MOBILITY"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(300f, 260f), StringMgr.Instance.Get("BULLET_COUNT"), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		TextureUtil.DrawTexture(new Rect(333f, 152f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect(333f, 172f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect(333f, 192f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect(333f, 212f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect(333f, 232f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect(333f, 252f, 110f, 18f), gaugeFrame, ScaleMode.StretchToFill);
	}
}
