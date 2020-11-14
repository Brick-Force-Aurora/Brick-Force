using UnityEngine;

public class aiHealer : MonAI
{
	private GameObject effCopy;

	public float healRange = 10f;

	public int incHp = 10;

	public float repeatTime = 10f;

	private float dtHeal;

	public override void ActiveHealerEff()
	{
		Transform transform = null;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		int num = 0;
		while (transform == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name.Contains("Dummy_mon_effect"))
			{
				transform = componentsInChildren[num];
				break;
			}
			num++;
		}
		effCopy = (Object.Instantiate((Object)MonManager.Instance.healerEff, transform.position, transform.rotation) as GameObject);
	}

	public override void updateAreaHeal()
	{
		if (effCopy != null)
		{
			Transform transform = null;
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			int num = 0;
			while (transform == null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].name.Contains("Dummy_mon_effect"))
				{
					transform = componentsInChildren[num];
					break;
				}
				num++;
			}
			effCopy.transform.position = transform.position;
			effCopy.transform.rotation = transform.rotation;
		}
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			dtHeal += Time.deltaTime;
			if (dtHeal > repeatTime)
			{
				P2PManager.Instance.SendPEER_DF_HEALER(monProp.Desc.Seq);
				Transform transform2 = null;
				Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
				int num2 = 0;
				while (transform2 == null && num2 < componentsInChildren2.Length)
				{
					if (componentsInChildren2[num2].name.Contains("Dummy_mon_effect"))
					{
						transform2 = componentsInChildren2[num2];
						break;
					}
					num2++;
				}
				effCopy = (Object.Instantiate((Object)MonManager.Instance.healerEff, transform2.position, transform2.rotation) as GameObject);
				MonDesc[] array = MonManager.Instance.ToDescriptorArray();
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = MonManager.Instance.Get(array[i].Seq);
					float num3 = Vector3.Distance(base.transform.position, gameObject.transform.position);
					if (!(num3 > healRange))
					{
						MonAI aIClass = MonManager.Instance.GetAIClass(gameObject, array[i].tblID);
						if (aIClass != null)
						{
							aIClass.ActiveHealEff();
						}
						array[i].Xp += incHp;
						if (array[i].Xp > array[i].max_xp)
						{
							array[i].Xp = array[i].max_xp;
						}
						P2PManager.Instance.SendPEER_DF_SELFHEAL(array[i].Seq, array[i].Xp);
					}
				}
				dtHeal = 0f;
			}
		}
	}
}
