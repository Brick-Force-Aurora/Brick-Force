using System.Collections.Generic;
using UnityEngine;

public class WeaponWatch : MonoBehaviour
{
	private Dictionary<long, float> weaponHeldTime;

	private float deltaTime;

	private float deltaMax = 1f;

	private void Start()
	{
		deltaMax = 1f;
		deltaTime = 0f;
		weaponHeldTime = new Dictionary<long, float>();
	}

	private void OnSwapWeapon()
	{
		deltaMax = 1f;
		UpdateWeaponHeldTime();
	}

	private void UpdateWeaponHeldTime()
	{
		Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>(includeInactive: true);
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				WeaponFunction component = componentsInChildren[i].GetComponent<WeaponFunction>();
				if (null != component && Weapon.TYPE.MELEE <= componentsInChildren[i].slot && componentsInChildren[i].slot <= Weapon.TYPE.PROJECTILE)
				{
					Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
					if (item == null)
					{
						item = MyInfoManager.Instance.GetUsingItemBySlot((TItem.SLOT)(componentsInChildren[i].slot + 2));
					}
					if (item != null)
					{
						if (weaponHeldTime.ContainsKey(item.Seq))
						{
							Dictionary<long, float> dictionary;
							Dictionary<long, float> dictionary2 = dictionary = weaponHeldTime;
							long seq;
							long key = seq = item.Seq;
							float num = dictionary[seq];
							dictionary2[key] = num + componentsInChildren[i].FlushHeldTime();
						}
						else
						{
							weaponHeldTime.Add(item.Seq, componentsInChildren[i].FlushHeldTime());
						}
					}
				}
			}
			float num2 = 0f;
			foreach (KeyValuePair<long, float> item2 in weaponHeldTime)
			{
				num2 += item2.Value;
			}
			Dictionary<long, float> dictionary3 = new Dictionary<long, float>();
			foreach (KeyValuePair<long, float> item3 in weaponHeldTime)
			{
				dictionary3.Add(item3.Key, (!(num2 <= 0f)) ? (item3.Value / num2) : 0f);
			}
			CSNetManager.Instance.Sock.SendCS_WEAPON_HELD_RATIO_REQ(dictionary3);
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > deltaMax)
		{
			deltaTime = 0f;
			deltaMax = Mathf.Min(deltaMax * 2f, 60f);
			UpdateWeaponHeldTime();
		}
	}
}
