using System.Collections.Generic;
using UnityEngine;

public class ScriptExecutor : MonoBehaviour
{
	private Queue<ScriptCmd> scriptCmdQueue;

	public void Run(BfScript script)
	{
		scriptCmdQueue = new Queue<ScriptCmd>();
		for (int i = 0; i < script.CmdList.Count; i++)
		{
			scriptCmdQueue.Enqueue(script.CmdList[i]);
		}
	}

	private void ExecuteCommand(ScriptCmd cmd)
	{
		if (cmd != null)
		{
			if (GlobalVars.Instance.tutorFirstScriptOn)
			{
				TutorHelpDialog tutorHelpDialog = (TutorHelpDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTORHELP, exclusive: true);
				if (tutorHelpDialog != null)
				{
					tutorHelpDialog.InitDialog();
					GlobalVars.Instance.tutorFirstScriptOn = false;
				}
			}
			switch (cmd.GetName())
			{
			case "EnableScript":
				EnableScript(((EnableScript)cmd).Id, ((EnableScript)cmd).Enable);
				break;
			case "ShowDialog":
				ShowDialog(((ShowDialog)cmd).Speaker, ((ShowDialog)cmd).Dialog);
				break;
			case "PlaySound":
				PlaySound(((PlaySound)cmd).Index);
				break;
			case "Sleep":
				Sleep(((Sleep)cmd).Howlong);
				break;
			case "Exit":
				Exit();
				break;
			case "ShowScript":
				ShowScript(((ShowScript)cmd).Id, ((ShowScript)cmd).Visible);
				break;
			case "GiveWeapon":
				GiveWeapon(((GiveWeapon)cmd).WeaponCode);
				break;
			case "TakeAwayAll":
				TakeAwayAll();
				break;
			case "SetMission":
				SetMission(((SetMission)cmd).Progress, ((SetMission)cmd).Title, ((SetMission)cmd).SubTitle, ((SetMission)cmd).Tag);
				break;
			}
		}
	}

	private void GiveWeapon(string weaponCode)
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null != component)
			{
				component.PickupFromTemplate(weaponCode);
				if (weaponCode != "waf")
				{
					TutorHelpDialog tutorHelpDialog = (TutorHelpDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTORHELP, exclusive: true);
					if (tutorHelpDialog != null)
					{
						tutorHelpDialog.OpenNext((!(GlobalVars.Instance.preWeaponCode == weaponCode)) ? true : false);
						GlobalVars.Instance.preWeaponCode = weaponCode;
					}
				}
			}
		}
	}

	private void TakeAwayAll()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (null != component)
			{
				component.ThrowAll();
			}
		}
	}

	private void EnableScript(int id, bool enable)
	{
		GameObject brickObject = BrickManager.Instance.GetBrickObject(id);
		if (null != brickObject)
		{
			Trigger componentInChildren = brickObject.GetComponentInChildren<Trigger>();
			if (null != componentInChildren)
			{
				componentInChildren.enabled = enable;
			}
		}
	}

	private void ShowScript(int id, bool visible)
	{
		if (id == 10854 && !visible)
		{
			GlobalVars.Instance.eventBridge = false;
		}
		GameObject brickObject = BrickManager.Instance.GetBrickObject(id);
		if (null != brickObject)
		{
			Trigger componentInChildren = brickObject.GetComponentInChildren<Trigger>();
			if (null != componentInChildren)
			{
				componentInChildren.Show(visible);
				if (visible)
				{
					GlobalVars.Instance.showBrickId = id;
				}
				if (id == 6161 && !visible)
				{
					GlobalVars.Instance.blockDelBrick = true;
					PaletteManager.Instance.PaletteSet(0, -1, 3);
					((TutorHelpDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTORHELP, exclusive: true))?.OpenNext(inc: true);
				}
				if (id == 6161 && visible)
				{
					GlobalVars.Instance.blockDelBrick = false;
				}
			}
		}
	}

	private void SetMission(string _p, string _t, string _s, string tag)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			string ex = StringMgr.Instance.GetEx(_p);
			string ex2 = StringMgr.Instance.GetEx(_t);
			string ex3 = StringMgr.Instance.GetEx(_s);
			string ex4 = StringMgr.Instance.GetEx(tag);
			gameObject.GetComponent<MissionLog>().SetMission((ex.Length > 0) ? ex : _p, (ex2.Length > 0) ? ex2 : _t, (ex3.Length > 0) ? ex3 : _s, (ex4.Length > 0) ? ex4 : tag, tag);
		}
	}

	private void ShowDialog(int speaker, string text)
	{
		base.transform.parent.gameObject.BroadcastMessage("OnPreShowDialog");
		ScriptDialog scriptDialog = base.transform.gameObject.AddComponent<ScriptDialog>();
		if (null != scriptDialog)
		{
			string ex = StringMgr.Instance.GetEx(text);
			scriptDialog.Speaker = speaker;
			scriptDialog.Text = ((ex.Length > 0) ? ex : text);
			TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
			if (text.Contains("SYS01"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.WASD);
					component.setClick(TUTO_INPUT.WASD);
				}
			}
			else if (text.Contains("SYS03") || text.Contains("SYS36"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.KEYALL);
					component.setClick((TUTO_INPUT)17);
				}
			}
			else if (text.Contains("SYS05") || text.Contains("SYS07") || text.Contains("SYS08") || text.Contains("SYS09") || text.Contains("SYS12") || text.Contains("SYS14") || text.Contains("SYS23") || text.Contains("SYS30"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.WASD);
					component.setClick(TUTO_INPUT.W);
				}
			}
			else if (text.Contains("SYS06") || text.Contains("SYS15") || text.Contains("SYS24"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.M_L);
					component.setClick(TUTO_INPUT.M_L);
				}
			}
			else if (text.Contains("SYS11") || text.Contains("SYS25"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.M_R);
					component.setClick(TUTO_INPUT.M_R);
				}
			}
			else if (text.Contains("SYS17"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.E);
					component.setClick(TUTO_INPUT.E);
				}
			}
			else if (text.Contains("SYS20"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.SPACE);
					component.setClick(TUTO_INPUT.SPACE);
					component.setKeyCheck();
				}
			}
			else if (text.Contains("SYS26"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.M);
					component.setClick(TUTO_INPUT.M);
				}
			}
			else if (text.Contains("SYS37"))
			{
				if (component != null)
				{
					component.setActive(TUTO_INPUT.WASD);
					component.setClick(TUTO_INPUT.D);
				}
			}
			else if (text.Contains("SYS10") && component != null)
			{
				if (GlobalVars.Instance.sys10First)
				{
					component.setActive(TUTO_INPUT.M_L);
					component.setClick(TUTO_INPUT.M_L);
					GlobalVars.Instance.sys10First = false;
				}
				else
				{
					component.setActive(TUTO_INPUT.M_R);
					component.setClick(TUTO_INPUT.M_R);
				}
			}
			if (text.Contains("SYS05") || text.Contains("SYS09") || text.Contains("SYS12") || text.Contains("SYS14") || text.Contains("SYS17") || text.Contains("SYS22"))
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					gameObject.GetComponent<MissionLog>().needPicture();
				}
			}
		}
	}

	private void Sleep(float deltaTime)
	{
		ScriptAlarm scriptAlarm = base.transform.gameObject.AddComponent<ScriptAlarm>();
		if (null != scriptAlarm)
		{
			scriptAlarm.DeltaTime = deltaTime;
			base.enabled = false;
		}
	}

	private void PlaySound(int index)
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component)
		{
			AudioClip audioClip = ScriptResManager.Instance.GetAudioClip(index);
			if (null != audioClip)
			{
				if (null != component)
				{
					if (GlobalVars.Instance.mute)
					{
						component.volume = 0f;
					}
					else
					{
						component.volume = PlayerPrefs.GetFloat("SfxVolume", 1f);
					}
				}
				component.PlayOneShot(audioClip);
			}
		}
	}

	private void Exit()
	{
		if ((!GlobalVars.Instance.isLoadBattleTutor || MyInfoManager.Instance.Tutorialed != 1) && (GlobalVars.Instance.isLoadBattleTutor || MyInfoManager.Instance.Tutorialed != 2))
		{
			if (GlobalVars.Instance.isLoadBattleTutor)
			{
				CSNetManager.Instance.Sock.SendCS_TUTORIAL_COMPLETE_REQ(1);
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_TUTORIAL_COMPLETE_REQ(2);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (scriptCmdQueue != null && scriptCmdQueue.Count > 0)
		{
			ExecuteCommand(scriptCmdQueue.Dequeue());
		}
		if (NoMoreCmdToDo())
		{
			Object.DestroyImmediate(base.transform.gameObject);
		}
	}

	private bool NoMoreCmdToDo()
	{
		ScriptDialog component = GetComponent<ScriptDialog>();
		ScriptAlarm component2 = GetComponent<ScriptAlarm>();
		return scriptCmdQueue.Count <= 0 && component == null && component2 == null;
	}

	private void CloseScriptDialogIfExists()
	{
		ScriptDialog component = GetComponent<ScriptDialog>();
		if (null != component)
		{
			Object.DestroyImmediate(component);
		}
	}

	private void OnPreShowDialog()
	{
		CloseScriptDialogIfExists();
	}
}
