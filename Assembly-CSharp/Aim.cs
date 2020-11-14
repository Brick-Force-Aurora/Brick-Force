using _Emulator;
using UnityEngine;

public class Aim : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.CROSS_HAIR;

	public Texture2D debugCrossHair;

	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public Accuracy accuracy;

	private bool aiming = true;

	protected float crossEffectTime;

	public GUIDepth.LAYER GuiDepth
	{
		get
		{
			return guiDepth;
		}
		set
		{
			guiDepth = value;
		}
	}

	public Texture2D DebugCrossHair
	{
		get
		{
			return debugCrossHair;
		}
		set
		{
			debugCrossHair = value;
		}
	}

	public Texture2D VCrossHair
	{
		get
		{
			return vCrossHair;
		}
		set
		{
			vCrossHair = value;
		}
	}

	public Texture2D HCrossHair
	{
		get
		{
			return hCrossHair;
		}
		set
		{
			hCrossHair = value;
		}
	}

	public Accuracy _Accuracy
	{
		get
		{
			return accuracy;
		}
		set
		{
			accuracy = value;
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawCrossHair();
			GUI.enabled = true;
		}
	}

	private void DrawCrossHair()
	{
		if (aiming)
		{
			float factor = (16f / 9f) / ((float)Screen.width / (float)Screen.height) * 60f / Camera.main.fieldOfView;
			float num = (float)Screen.width * accuracy.Inaccurate * factor;
			Vector2 vector = new Vector2(((float)Screen.width - num) / 2f, ((float)Screen.height - num) / 2f);
			if (null != debugCrossHair)
			{
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, num, num), debugCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
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
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2) - num / 2f - 8f);
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2) + num / 2f);
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			if (null != hCrossHair)
			{
				vector = new Vector2((float)(Screen.width / 2) - num / 2f - 8f, (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)(Screen.width / 2) + num / 2f, (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
			if (null != debugCrossHair)
			{
				num = (float)Screen.width * accuracy.Accurate;
				vector = new Vector2(((float)Screen.width - num) / 2f, ((float)Screen.height - num) / 2f);
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, num, num), debugCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
		}
	}

	private void Start()
	{
		Modify();
		aiming = true;
		accuracy.Init();
	}

	private void Modify()
	{
		WeaponFunction component = GetComponent<WeaponFunction>();
		if (null != component)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)component.weaponBy);
			if (wpnMod != null)
			{
				accuracy.accuracy = wpnMod.fAccuracy;
				accuracy.accurateMin = wpnMod.fAccurateMin;
				accuracy.accurateMax = wpnMod.fAccurateMax;
				accuracy.inaccurateMin = wpnMod.fInaccurateMin;
				accuracy.inaccurateMax = wpnMod.fInaccurateMax;
				accuracy.accurateSpread = wpnMod.fAccurateSpread;
				accuracy.accurateCenter = wpnMod.fAccurateCenter;
				accuracy.inaccurateSpread = wpnMod.fInaccurateSpread;
				accuracy.inaccurateCenter = wpnMod.fInaccurateCenter;
				accuracy.moveInaccuracyFactor = wpnMod.fMoveInaccuracyFactor;
			}
			TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
			int num = 1;
			Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
			if (item == null)
			{
				item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
			}
			if (item != null)
			{
				int grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					accuracy.accuracy += value;
				}
			}
		}
	}

	private void Update()
	{
		UpdateCrossEffect();
	}

	public Vector2 CalcDeflection()
	{
		return accuracy.CalcDeflection();
	}

	public void Inaccurate(bool aimAccurateMore)
	{
		accuracy.MakeInaccurate(aimAccurateMore);
	}

	public void Accurate(bool aimAccurate)
	{
		accuracy.MakeAccurate(aimAccurate);
	}

	public void SetAiming(bool _aiming)
	{
		aiming = _aiming;
	}

	public void SetShootEnermyEffect()
	{
		crossEffectTime = 0.3f;
	}

	public void UpdateCrossEffect()
	{
		crossEffectTime -= Time.deltaTime;
	}
}
