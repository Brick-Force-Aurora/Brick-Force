using System;
using UnityEngine;

[Serializable]
public class DroppedItem
{
	public int itemSeq;

	public string itemCode = string.Empty;

	public int bulletCount;

	public int bulletCount2;

	public GameObject obj;

	public GameObject eff;

	public DroppedItem(int _itemSeq, string _itemCode, int _bulletCount, int _bulletCount2, float x, float y, float z)
	{
		itemSeq = _itemSeq;
		itemCode = _itemCode;
		bulletCount = _bulletCount;
		bulletCount2 = _bulletCount2;
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(itemCode);
		eff = (UnityEngine.Object.Instantiate((UnityEngine.Object)GlobalVars.Instance.droppedEff, new Vector3(x, y, z), Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		obj = (UnityEngine.Object.Instantiate((UnityEngine.Object)tWeapon.CurPrefab()) as GameObject);
		obj.transform.position = new Vector3(x, y, z);
		obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		obj.GetComponent<WeaponGadget>().enabled = true;
		obj.GetComponent<WeaponFunction>().enabled = false;
		obj.GetComponent<WeaponFunction>().ItemSeq = itemSeq;
		Aim component = obj.GetComponent<Aim>();
		if (null != component)
		{
			component.enabled = false;
		}
		Scope component2 = obj.GetComponent<Scope>();
		if (null != component2)
		{
			component2.enabled = false;
		}
		Recursively.SetLayer(obj.GetComponent<Transform>(), LayerMask.NameToLayer("Default"));
	}
}
