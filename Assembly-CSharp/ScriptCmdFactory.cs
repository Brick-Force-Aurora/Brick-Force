using System;

public class ScriptCmdFactory
{
	public static ScriptCmd CreateDefault(int index)
	{
		string text = string.Empty;
		switch (index)
		{
		case 0:
			text = EnableScript.GetDefaultDescription();
			break;
		case 1:
			text = ShowDialog.GetDefaultDescription();
			break;
		case 2:
			text = PlaySound.GetDefaultDescription();
			break;
		case 3:
			text = Sleep.GetDefaultDescription();
			break;
		case 4:
			text = Exit.GetDefaultDescription();
			break;
		case 5:
			text = ShowScript.GetDefaultDescription();
			break;
		case 6:
			text = GiveWeapon.GetDefaultDescription();
			break;
		case 7:
			text = TakeAwayAll.GetDefaultDescription();
			break;
		case 8:
			text = SetMission.GetDefaultDescription();
			break;
		}
		if (text.Length <= 0)
		{
			return null;
		}
		return Create(text);
	}

	public static ScriptCmd Create(string description)
	{
		ScriptCmd result = null;
		string[] array = description.Split(ScriptCmd.ArgDelimeters, StringSplitOptions.RemoveEmptyEntries);
		if (array != null && array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			string text = array[0].ToLower();
			switch (text)
			{
			case "enablescript":
				if (array.Length >= 3)
				{
					EnableScript enableScript = new EnableScript();
					enableScript.Id = int.Parse(array[1]);
					enableScript.Enable = bool.Parse(array[2]);
					result = enableScript;
				}
				break;
			case "showdialog":
				if (array.Length >= 2)
				{
					ShowDialog showDialog = new ShowDialog();
					showDialog.Speaker = int.Parse(array[1]);
					showDialog.Dialog = ((array.Length < 3) ? string.Empty : array[2]);
					result = showDialog;
				}
				break;
			case "playsound":
				if (array.Length >= 2)
				{
					PlaySound playSound = new PlaySound();
					playSound.Index = int.Parse(array[1]);
					result = playSound;
				}
				break;
			case "sleep":
				if (array.Length >= 2)
				{
					Sleep sleep = new Sleep();
					sleep.Howlong = float.Parse(array[1]);
					result = sleep;
				}
				break;
			case "exit":
			{
				Exit exit = new Exit();
				result = exit;
				break;
			}
			case "showscript":
				if (array.Length >= 3)
				{
					ShowScript showScript = new ShowScript();
					showScript.Id = int.Parse(array[1]);
					showScript.Visible = bool.Parse(array[2]);
					result = showScript;
				}
				break;
			case "giveweapon":
			{
				GiveWeapon giveWeapon = new GiveWeapon();
				giveWeapon.WeaponCode = ((array.Length < 2) ? string.Empty : array[1]);
				result = giveWeapon;
				break;
			}
			case "takeawayall":
			{
				TakeAwayAll takeAwayAll = new TakeAwayAll();
				result = takeAwayAll;
				break;
			}
			case "setmission":
			{
				SetMission setMission = new SetMission();
				if (array.Length >= 2)
				{
					setMission.Progress = array[1];
					setMission.Title = array[2];
					setMission.SubTitle = array[3];
					if (array.Length > 4 && array[4].Length > 0)
					{
						setMission.Tag = array[4];
					}
				}
				else
				{
					setMission.Progress = string.Empty;
					setMission.Title = string.Empty;
					setMission.SubTitle = string.Empty;
					setMission.Tag = string.Empty;
				}
				result = setMission;
				break;
			}
			}
		}
		return result;
	}
}
