public class GiveWeapon : ScriptCmd
{
	private string weaponCode;

	public string WeaponCode
	{
		get
		{
			return weaponCode;
		}
		set
		{
			weaponCode = value;
		}
	}

	public override int GetIconIndex()
	{
		return 6;
	}

	public override string GetDescription()
	{
		string str = "giveweapon" + ScriptCmd.ArgDelimeters[0];
		return str + weaponCode.ToString();
	}

	public static string GetDefaultDescription()
	{
		return "giveweapon" + ScriptCmd.ArgDelimeters[0] + string.Empty;
	}

	public override string GetName()
	{
		return "GiveWeapon";
	}
}
