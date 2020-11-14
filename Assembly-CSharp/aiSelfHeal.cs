using UnityEngine;

public class aiSelfHeal : MonAI
{
	public int incHp = 10;

	public float repeatTime = 10f;

	private float dtHeal;

	private bool bHeal;

	public bool IsHeal
	{
		get
		{
			return bHeal;
		}
		set
		{
			bHeal = value;
		}
	}

	public override void updateSelfHeal()
	{
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			dtHeal += Time.deltaTime;
			if (dtHeal > repeatTime)
			{
				eff = (Object.Instantiate((Object)MonManager.Instance.healEff, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
				dtHeal = 0f;
				monProp.Desc.Xp += incHp;
				if (monProp.Desc.Xp > monProp.Desc.max_xp)
				{
					monProp.Desc.Xp = monProp.Desc.max_xp;
				}
				P2PManager.Instance.SendPEER_DF_SELFHEAL(monProp.Desc.Seq, monProp.Desc.Xp);
			}
		}
	}
}
