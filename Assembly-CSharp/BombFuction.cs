using _Emulator;
using UnityEngine;

public class BombFuction : WeaponFunction
{
	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public Texture2D gaugeBg;

	public Texture2D gaugeBar;

	public float installTime = 10f;

	private bool drawn;

	private bool installing;

	private float deltaTime;

	private ExplosionMatch explosionMatch;

	public bool IsInstalling => installing;

	private void EnsureVisibility()
	{
		if (explosionMatch.BombInstaller == MyInfoManager.Instance.Seq)
		{
			Hide();
		}
		else if (!MyInfoManager.Instance.IsSpectator)
		{
			Show();
		}
	}

	private void VerifyExplosionMatch()
	{
		if (explosionMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				explosionMatch = gameObject.GetComponent<ExplosionMatch>();
			}
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

	private void Restart()
	{
		deltaTime = 0f;
		installing = false;
		P2PManager.Instance.SendPEER_INSTALLING_BOMB(installing: false);
		GetComponent<Weapon>().EndFireSound();
	}

	private void DrawCrossHair()
	{
		if (Screen.lockCursor)
		{
			GUI.depth = 35;
			Color color = GUI.color;
			GUI.color = Config.instance.crosshairColor;
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

	private void DrawInstallingGauge()
	{
		if (installing)
		{
			float num = deltaTime / installTime;
			Rect position = new Rect((float)((Screen.width - 448) / 2), (float)(Screen.height / 2 + 63), 448f, 34f);
			Rect position2 = new Rect((float)((Screen.width - 412) / 2), (float)(Screen.height / 2 + 70), 412f * num, 20f);
			TextureUtil.DrawTexture(position, gaugeBg, ScaleMode.StretchToFill, alphaBlend: true);
			TextureUtil.DrawTexture(position2, gaugeBar, ScaleMode.StretchToFill, alphaBlend: true);
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
			DrawInstallingGauge();
			GUI.enabled = true;
		}
	}

	private bool CanInstall()
	{
		return localCtrl.CanControl() && drawn && explosionMatch.CanInstall;
	}

	private void Start()
	{
		Reset();
	}

	public bool VerifyCameraAll()
	{
		return cam != null && fpCam != null && camCtrl != null;
	}

	public bool GetInstallTarget(out RaycastHit hit)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Chunk"));
		Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (Physics.Raycast(ray, out hit, GetComponent<Weapon>().range, layerMask))
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

	private void Show()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = true;
		}
	}

	private void Hide()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = false;
		}
	}

	public void Clear()
	{
		Reset();
	}

	private void Update()
	{
		if (Screen.lockCursor && BrickManager.Instance.IsLoaded)
		{
			VerifyCamera();
			VerifyLocalController();
			VerifyExplosionMatch();
			RaycastHit hit2;
			if (installing)
			{
				deltaTime += Time.deltaTime;
				if (!CanInstall() || !custom_inputs.Instance.GetButton("K_ACTION") || !GetInstallTarget(out RaycastHit hit))
				{
					installing = false;
					deltaTime = 0f;
					P2PManager.Instance.SendPEER_INSTALLING_BOMB(installing: false);
					GetComponent<Weapon>().EndFireSound();
				}
				else if (deltaTime >= installTime)
				{
					BlastTarget component = hit.transform.gameObject.GetComponent<BlastTarget>();
					if (null == component)
					{
						Debug.LogError("Fail to get BlastTarget Component from core objs");
					}
					else
					{
						explosionMatch.Step = ExplosionMatch.STEP.INSTALL_TRY;
						installing = false;
						deltaTime = 0f;
						P2PManager.Instance.SendPEER_INSTALLING_BOMB(installing: false);
						GetComponent<Weapon>().EndFireSound();
						CSNetManager.Instance.Sock.SendCS_BM_INSTALL_BOMB_REQ(component.Spot, hit.point, hit.normal);
					}
				}
			}
			else if (CanInstall() && custom_inputs.Instance.GetButton("K_ACTION") && GetInstallTarget(out hit2))
			{
				installing = true;
				deltaTime = 0f;
				localCtrl.CancelSpeedUp();
				GetComponent<Weapon>().StartFireSound();
				P2PManager.Instance.SendPEER_INSTALLING_BOMB(installing: true);
			}
			EnsureVisibility();
		}
	}
}
