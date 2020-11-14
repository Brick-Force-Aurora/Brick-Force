using UnityEngine;

public class BlackHoleItem : ActiveItemBase
{
	public override void StartItem()
	{
		BlackHole component = GameObject.Find("Main").GetComponent<BlackHole>();
		if (component != null)
		{
			component.On();
		}
		if (MyInfoManager.Instance.Seq != useUserSeq)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component2 = gameObject.GetComponent<LocalController>();
				if (null != component2)
				{
					if (!component2.IsDead && !component2.bungeeRespawn && !component2.ActivateBlackhole)
					{
						component2.sparcleFXOn();
					}
					GameObject gameObject2 = GameObject.Find("Me");
					if (gameObject2 != null)
					{
						BlackholeScreenFX component3 = gameObject2.GetComponent<BlackholeScreenFX>();
						if (component3 != null)
						{
							component3.Reset(useUserSeq);
						}
					}
				}
			}
			BrickManDesc desc = BrickManManager.Instance.GetDesc(useUserSeq);
			if (desc != null)
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("BUNGEE_ITEM_MESSAGE_06"), desc.Nickname));
			}
		}
	}
}
