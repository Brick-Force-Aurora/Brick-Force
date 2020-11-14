using UnityEngine;

public class SpectatorSwitch : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth;

	public GameObject fpCam;

	public Color clrOutline = Color.black;

	public Color clrText = Color.grey;

	private MyInfoManager.CONTROL_MODE curMode = MyInfoManager.CONTROL_MODE.NONE;

	private GameObject target;

	private LocalController localController;

	private SpectatorController spectatorController;

	private WeaponFunction[] deactivatedWeapons;

	private float deltaTime;

	public GameObject Target => target;

	private void Start()
	{
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && BrickManager.Instance.IsLoaded)
		{
			GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
			if (null != gUISkin && MyInfoManager.Instance.IsSpectator)
			{
				GUI.skin = gUISkin;
				GUI.depth = (int)guiDepth;
				GUI.enabled = !DialogManager.Instance.IsModal;
				string text = string.Empty;
				if (null == target)
				{
					text = StringMgr.Instance.Get("NO_SPECTATABLE_PLAYER");
				}
				else
				{
					PlayerProperty component = target.GetComponent<PlayerProperty>();
					if (null != component)
					{
						text = component.Desc.Nickname;
					}
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 - 202)), text, "BigLabel", clrOutline, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 - 200)), text, "BigLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				GUI.enabled = true;
			}
		}
	}

	private void OnRemoveBrickMan(GameObject obj)
	{
		if (target == obj)
		{
			GhostSwitch component = obj.GetComponent<GhostSwitch>();
			if (component != null)
			{
				component.DisableGhost();
			}
			if (!MyInfoManager.Instance.IsSpectator)
			{
				target = null;
			}
			else
			{
				target = BrickManManager.Instance.GetNextPlayer(obj, excludingMe: true, LookFriendlyOnly());
				if (null == target)
				{
					base.transform.parent = null;
				}
				else
				{
					base.transform.parent = target.transform;
				}
			}
		}
	}

	private void VerifyLocalController()
	{
		if (localController == null)
		{
			localController = GetComponent<LocalController>();
		}
	}

	private void VerifySpectatorController()
	{
		if (spectatorController == null)
		{
			spectatorController = GetComponent<SpectatorController>();
		}
	}

	private void Update()
	{
		if (target != null)
		{
			PlayerProperty component = target.GetComponent<PlayerProperty>();
			if (custom_inputs.Instance.GetButtonDown("K_JUMP") || null == component || component.Desc.IsHidePlayer)
			{
				target = BrickManManager.Instance.GetNextPlayer(target, excludingMe: false, LookFriendlyOnly());
				if (null == target)
				{
					base.transform.parent = null;
				}
				else
				{
					base.transform.parent = target.transform;
				}
			}
		}
		if (!MyInfoManager.Instance.IsGM || !Screen.lockCursor || Input.GetKeyDown(KeyCode.F4))
		{
		}
		VerifyLocalController();
		VerifySpectatorController();
		if (curMode != MyInfoManager.Instance.ControlMode)
		{
			curMode = MyInfoManager.Instance.ControlMode;
			ModeChange();
		}
		VerifyTarget();
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.2f)
		{
			deltaTime = 0f;
			P2PManager.Instance.SendPEER_SPECTATOR();
			if (MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.SPECTATOR_MODE)
			{
				GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.CAMERA_SPECTATOR_USE);
			}
		}
	}

	private bool LookFriendlyOnly()
	{
		return MyInfoManager.Instance.ControlMode != MyInfoManager.CONTROL_MODE.SPECTATOR_MODE;
	}

	private void VerifyTarget()
	{
		if (MyInfoManager.Instance.IsSpectator)
		{
			if (null == target)
			{
				target = BrickManManager.Instance.GetRandomPlayer(LookFriendlyOnly());
				if (null == target)
				{
					base.transform.parent = null;
				}
				else
				{
					base.transform.parent = target.transform;
				}
			}
			if (null != target)
			{
				PlayerProperty component = target.GetComponent<PlayerProperty>();
				if (null != component && component.Desc != null && component.Desc.Status != 4)
				{
					target = null;
				}
			}
		}
	}

	public void ModeChangeBruteforcely(MyInfoManager.CONTROL_MODE controlMode)
	{
		curMode = controlMode;
		ModeChange();
	}

	private void ModeChange()
	{
		if (!(null == localController) && !(null == fpCam) && !(null == spectatorController))
		{
			if (curMode == MyInfoManager.CONTROL_MODE.SPECTATOR_MODE || curMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR)
			{
				localController.enabled = false;
				deactivatedWeapons = GetComponentsInChildren<WeaponFunction>();
				for (int i = 0; i < deactivatedWeapons.Length; i++)
				{
					if (null != deactivatedWeapons[i])
					{
						deactivatedWeapons[i].gameObject.SetActive(value: false);
					}
				}
				fpCam.SetActive(value: false);
				spectatorController.enabled = true;
			}
			else
			{
				localController.enabled = true;
				if (deactivatedWeapons != null && deactivatedWeapons.Length > 0)
				{
					for (int j = 0; j < deactivatedWeapons.Length; j++)
					{
						if (null != deactivatedWeapons[j])
						{
							deactivatedWeapons[j].gameObject.SetActive(value: true);
						}
					}
				}
				fpCam.SetActive(value: true);
				spectatorController.enabled = false;
				target = null;
				base.transform.parent = null;
			}
		}
	}
}
