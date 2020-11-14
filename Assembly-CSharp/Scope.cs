using UnityEngine;

public class Scope : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.CROSS_HAIR;

	public Texture2D crossHair;

	public Texture2D blackOut;

	public Accuracy accuracy;

	public float fov = 15f;

	public float camSpeed = 0.4f;

	public int midstep;

	public float[] midfovs;

	public bool zoomKeep;

	private int curstep;

	private bool aiming = true;

	private bool scoping;

	private Camera cam;

	private Camera fpCam;

	private CameraController camCtrl;

	private bool cooldown;

	public Texture2D CrossHair
	{
		get
		{
			return crossHair;
		}
		set
		{
			crossHair = value;
		}
	}

	public Texture2D BlackOut
	{
		get
		{
			return blackOut;
		}
		set
		{
			blackOut = value;
		}
	}

	public Accuracy _accuracy
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

	public float Fov
	{
		get
		{
			return fov;
		}
		set
		{
			fov = value;
		}
	}

	public float CamSpeed
	{
		get
		{
			return camSpeed;
		}
		set
		{
			camSpeed = value;
		}
	}

	public int Midstep
	{
		get
		{
			return midstep;
		}
		set
		{
			midstep = value;
		}
	}

	public float[] Midfovs
	{
		get
		{
			return midfovs;
		}
		set
		{
			midfovs = value;
		}
	}

	public bool Scoping => scoping;

	private bool VerifyCamera()
	{
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
				camCtrl = gameObject.GetComponent<CameraController>();
			}
			GameObject gameObject2 = GameObject.Find("First Person Camera");
			if (null != gameObject2)
			{
				fpCam = gameObject2.GetComponent<Camera>();
			}
		}
		return cam != null && fpCam != null && camCtrl != null;
	}

	private void DrawCrossHair()
	{
		if (IsZooming() && !cooldown && null != crossHair)
		{
			Vector2 vector = new Vector2((float)((Screen.width - crossHair.width) / 2), (float)((Screen.height - crossHair.height) / 2));
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)crossHair.width, (float)crossHair.height), crossHair, ScaleMode.StretchToFill, alphaBlend: true);
			if (null != blackOut)
			{
				if (vector.x > 0f)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, vector.x, (float)Screen.height), blackOut, ScaleMode.StretchToFill, alphaBlend: false);
					TextureUtil.DrawTexture(new Rect((float)Screen.width - vector.x - 10f, 0f, vector.x + 10f, (float)Screen.height), blackOut, ScaleMode.StretchToFill, alphaBlend: false);
				}
				if (vector.y > 0f)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, vector.y), blackOut, ScaleMode.StretchToFill, alphaBlend: false);
					TextureUtil.DrawTexture(new Rect(0f, (float)Screen.height - vector.y - 10f, (float)Screen.width, vector.y + 10f), blackOut, ScaleMode.StretchToFill, alphaBlend: false);
				}
			}
		}
	}

	private void Start()
	{
		Modify();
		cooldown = false;
		aiming = true;
		scoping = false;
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
				accuracy.accuracy = wpnMod.fZAccuracy;
				accuracy.accurateMin = wpnMod.fZAccurateMin;
				accuracy.accurateMax = wpnMod.fZAccurateMax;
				accuracy.inaccurateMin = wpnMod.fZInaccurateMin;
				accuracy.inaccurateMax = wpnMod.fZInaccurateMax;
				accuracy.accurateSpread = wpnMod.fZAccurateSpread;
				accuracy.accurateCenter = wpnMod.fZAccurateCenter;
				accuracy.inaccurateSpread = wpnMod.fZInaccurateSpread;
				accuracy.inaccurateCenter = wpnMod.fZInaccurateCenter;
				accuracy.moveInaccuracyFactor = wpnMod.fZMoveInaccuracyFactor;
				fov = wpnMod.fZFov;
				camSpeed = wpnMod.fZCamSpeed;
			}
			TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
			Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
			if (item == null)
			{
				item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
			}
			if (item != null)
			{
				int num = 1;
				int grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					accuracy.accuracy += value;
				}
			}
		}
	}

	public void HandleFireEvent(bool aimAccurateMore)
	{
		Inaccurate(aimAccurateMore);
		if (!zoomKeep)
		{
			cooldown = true;
			if (IsZooming())
			{
				scoping = false;
				ZoomOut();
				Aim component = GetComponent<Aim>();
				if (component != null || component.enabled)
				{
					GetComponent<Aim>().SetAiming(_aiming: true);
				}
			}
		}
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

	private void OnDisable()
	{
		if (IsZooming())
		{
			scoping = false;
			ZoomOut();
		}
	}

	public bool ToggleScoping(bool forceApply = false)
	{
		bool flag = false;
		if (midstep > 0)
		{
			if (!scoping)
			{
				curstep = midstep;
				flag = true;
			}
			else
			{
				curstep--;
				if (curstep < 0)
				{
					flag = true;
				}
			}
		}
		if (midstep == 0 || flag || forceApply)
		{
			scoping = !scoping;
		}
		SetupCamera();
		return !scoping;
	}

	public bool IsZooming()
	{
		return aiming && scoping;
	}

	private void ZoomIn()
	{
		if (!(cam == null) && !(fpCam == null) && !(camCtrl == null))
		{
			if (curstep > 0)
			{
				camCtrl.SetScopeFov(midfovs[curstep - 1]);
			}
			else
			{
				camCtrl.SetScopeFov(fov);
			}
			fpCam.enabled = false;
			camCtrl.SetCameraSpeedFactor(camSpeed);
		}
	}

	private void ZoomOut()
	{
		if (!(cam == null) && !(fpCam == null) && !(camCtrl == null))
		{
			fpCam.enabled = true;
			camCtrl.SetCameraSpeedFactor(1f);
			camCtrl.SetScopeFov(60f);
		}
	}

	private void SetupCamera()
	{
		if (!cooldown)
		{
			VerifyCamera();
			if (IsZooming())
			{
				ZoomIn();
			}
			else
			{
				ZoomOut();
			}
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

	private void Update()
	{
		VerifyCamera();
		if (cooldown)
		{
			Gun component = GetComponent<Gun>();
			if (null != component)
			{
				cooldown = component.IsCoolDown();
			}
		}
	}
}
