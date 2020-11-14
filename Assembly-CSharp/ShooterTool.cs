using System;
using UnityEngine;

[Serializable]
public class ShooterTool
{
	private float coolTimeInst;

	private ShooterTools tools;

	private ConsumableDesc desc;

	private Item item;

	private float deltaTime = 10000f;

	private string input;

	private string hotkey;

	private bool active = true;

	private AudioSource audio;

	private BattleChat battleChat;

	private LocalController controller;

	private bool keyPressed;

	public Texture2D Icon
	{
		get
		{
			if (desc == null)
			{
				return null;
			}
			return (!IsEnable()) ? desc.disable : desc.enable;
		}
	}

	public string Name
	{
		get
		{
			if (desc == null)
			{
				return string.Empty;
			}
			return desc.name;
		}
	}

	public string Hotkey => hotkey;

	public bool IsActive
	{
		get
		{
			if (desc == null || !desc.passive)
			{
				return false;
			}
			return active;
		}
	}

	public string CoolTime
	{
		get
		{
			if (desc == null || deltaTime >= coolTimeInst)
			{
				return string.Empty;
			}
			return Mathf.CeilToInt(coolTimeInst - deltaTime).ToString();
		}
	}

	public string Amount
	{
		get
		{
			if (item == null)
			{
				return string.Empty;
			}
			return item.GetAmountString();
		}
	}

	public LocalController Controller
	{
		set
		{
			controller = value;
		}
	}

	public AudioSource Audio
	{
		set
		{
			audio = value;
		}
	}

	public bool IsPassive
	{
		get
		{
			if (desc == null)
			{
				return false;
			}
			return desc.passive;
		}
	}

	public ShooterTool(ShooterTools _tools, ConsumableDesc _desc, Item _item, AudioSource _audio, string _input, string _hotkey, BattleChat _battleChat, LocalController _localController)
	{
		tools = _tools;
		desc = _desc;
		item = _item;
		audio = _audio;
		input = _input;
		hotkey = _hotkey;
		active = true;
		battleChat = _battleChat;
		controller = _localController;
		coolTimeInst = desc.cooltime;
		switch (desc.name)
		{
		case "heal":
		case "heal50":
		case "heal30":
		{
			float num = MyInfoManager.Instance.SumFunctionFactor("hp_cooltime");
			coolTimeInst = desc.cooltime - (float)Mathf.FloorToInt(num * desc.cooltime);
			break;
		}
		}
	}

	private void ErrorSound()
	{
		if (null != audio && desc != null && null != desc.errorClip)
		{
			audio.PlayOneShot(desc.errorClip);
		}
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (desc != null && !battleChat.IsChatting && controller != null && controller.IsDead)
		{
			switch (desc.name)
			{
			case "respawn":
				Respawn();
				break;
			case "just_respawn":
				JustRespawn();
				break;
			}
		}
		keyPressed = custom_inputs.Instance.GetButtonDown(input);
		if (desc != null && !battleChat.IsChatting && keyPressed && controller != null && controller.CanControl())
		{
			switch (desc.name)
			{
			case "auto_reload":
				active = !active;
				break;
			case "heal":
				if (!Heal(100))
				{
					ErrorSound();
				}
				break;
			case "heal50":
				if (!Heal(50))
				{
					ErrorSound();
				}
				break;
			case "heal30":
				if (!Heal(30))
				{
					ErrorSound();
				}
				break;
			case "assault_ammo":
				if (!ChargeAssaultAmmo())
				{
					ErrorSound();
				}
				break;
			case "speedup":
				if (!SpeedUp())
				{
					ErrorSound();
				}
				break;
			case "heartbeat_radar":
				if (!HeartbeatDetect())
				{
					ErrorSound();
				}
				break;
			case "grenade_ammo":
				if (!ChargeGrenadeAmmo())
				{
					ErrorSound();
				}
				break;
			case "pistol_ammo":
				if (!ChargePistolAmmo())
				{
					ErrorSound();
				}
				break;
			case "heavy_ammo":
				if (!ChargeHeavyAmmo())
				{
					ErrorSound();
				}
				break;
			case "sniper_ammo":
				if (!ChargeSniperAmmo())
				{
					ErrorSound();
				}
				break;
			case "submachine_ammo":
				if (!ChargeSubmachineAmmo())
				{
					ErrorSound();
				}
				break;
			case "auto_heal":
				active = !active;
				break;
			case "just_respawn":
				active = !active;
				break;
			}
		}
	}

	private bool GetExternalCondition()
	{
		switch (desc.name)
		{
		case "auto_heal":
			return controller != null && controller.AutoHealPossible;
		case "respawn":
			return controller != null && controller.IsDead && controller.DeltaFromDeath > 2f;
		default:
			return true;
		}
	}

	public bool IsEnable()
	{
		if (item == null || !item.EnoughToConsume)
		{
			return false;
		}
		if (desc.IsDisableRoom)
		{
			return false;
		}
		if (desc.passive)
		{
			if (coolTimeInst < 0f)
			{
				return GetExternalCondition() && active;
			}
			return GetExternalCondition() && active && deltaTime > coolTimeInst;
		}
		return GetExternalCondition() && deltaTime > coolTimeInst;
	}

	public void StartCoolTime(string func)
	{
		if (func == desc.name)
		{
			deltaTime = 0f;
		}
	}

	public void Use()
	{
		tools.StartCoolTime(desc.name);
		CSNetManager.Instance.Sock.SendCS_USE_SHOOTER_CONSUMABLE_REQ(item.Seq, item.Code);
		if (desc.name == "heal" || desc.name == "heal50" || desc.name == "heal30")
		{
			P2PManager.Instance.SendPEER_CONSUME(MyInfoManager.Instance.Seq, desc.name);
		}
		if (null != audio && desc != null && null != desc.actionClip)
		{
			audio.PlayOneShot(desc.actionClip);
		}
	}

	public bool Heal(int inc)
	{
		bool result = false;
		if (IsEnable())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component && component.Heal(inc))
				{
					result = true;
					Use();
				}
			}
		}
		return result;
	}

	public bool Respawn()
	{
		bool result = false;
		if (IsEnable())
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				Respawner component = gameObject.GetComponent<Respawner>();
				if (null != component && component.Resurrect())
				{
					result = true;
					Use();
				}
			}
		}
		return result;
	}

	public bool JustRespawn()
	{
		bool result = false;
		if (IsEnable())
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				Respawner component = gameObject.GetComponent<Respawner>();
				if (null != component && component.Resurrect2())
				{
					result = true;
					Use();
				}
			}
		}
		return result;
	}

	public bool ChargeAssaultAmmo()
	{
		bool flag = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				Gun[] componentsInChildren = gameObject.GetComponentsInChildren<Gun>(includeInactive: true);
				int num = 0;
				while (!flag && num < componentsInChildren.Length)
				{
					int num2 = TItemManager.Instance.WeaponBy2Category((int)componentsInChildren[num].weaponBy);
					if (num2 == 1 && componentsInChildren[num].Charge())
					{
						flag = true;
						Use();
					}
					num++;
				}
			}
		}
		return flag;
	}

	private bool ChargePistolAmmo()
	{
		bool result = false;
		if (IsEnable())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				Gun[] componentsInChildren = gameObject.GetComponentsInChildren<Gun>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					int num = TItemManager.Instance.WeaponBy2Category((int)componentsInChildren[i].weaponBy);
					if (num == 4 && componentsInChildren[i].Charge())
					{
						result = true;
						Use();
					}
				}
			}
		}
		return result;
	}

	private bool ChargeHeavyAmmo()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				Gun[] componentsInChildren = gameObject.GetComponentsInChildren<Gun>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (TItemManager.Instance.WeaponBy2Category((int)componentsInChildren[i].weaponBy) == 0 && componentsInChildren[i].Charge())
					{
						result = true;
						Use();
					}
				}
			}
		}
		return result;
	}

	private bool ChargeSniperAmmo()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				Gun[] componentsInChildren = gameObject.GetComponentsInChildren<Gun>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					int num = TItemManager.Instance.WeaponBy2Category((int)componentsInChildren[i].weaponBy);
					if (num == 2 && componentsInChildren[i].Charge())
					{
						result = true;
						Use();
					}
				}
			}
		}
		return result;
	}

	private bool ChargeSubmachineAmmo()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				Gun[] componentsInChildren = gameObject.GetComponentsInChildren<Gun>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					int num = TItemManager.Instance.WeaponBy2Category((int)componentsInChildren[i].weaponBy);
					if (num == 3 && componentsInChildren[i].Charge())
					{
						result = true;
						Use();
					}
				}
			}
		}
		return result;
	}

	public bool ChargeGrenadeAmmo()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				HandBomb[] componentsInChildren = gameObject.GetComponentsInChildren<HandBomb>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].Charge())
					{
						result = true;
						Use();
					}
				}
			}
		}
		return result;
	}

	public bool SpeedUp()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component && component.SpeedUp())
				{
					result = true;
					Use();
				}
			}
		}
		return result;
	}

	public bool HeartbeatDetect()
	{
		bool result = false;
		if (IsEnable() && !controller.IsDead)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (gameObject != null)
			{
				Radar component = gameObject.GetComponent<Radar>();
				if (null != component && component.HeartbeatDetect())
				{
					result = true;
					Use();
				}
			}
		}
		return result;
	}
}
