using UnityEngine;

public class DurabilityEvent
{
	public string code;

	public int durability;

	public int diff;

	public DurabilityEvent(string _code, int _durability, int _diff)
	{
		code = _code;
		durability = _durability;
		diff = _diff;
	}

	public override string ToString()
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(code);
		if (tWeapon == null)
		{
			return string.Empty;
		}
		string key = (diff <= 0) ? "REPAIR_EVENT" : "DECAY_EVENT";
		float num = (float)durability / (float)tWeapon.durabilityMax * 100f;
		float num2 = (float)Mathf.Abs(diff) / (float)tWeapon.durabilityMax * 100f;
		return string.Format(StringMgr.Instance.Get(key), tWeapon.Name, num.ToString("0.##"), num2.ToString("0.##"));
	}
}
