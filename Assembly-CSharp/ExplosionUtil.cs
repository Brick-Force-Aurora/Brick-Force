using System.Collections.Generic;
using UnityEngine;

public class ExplosionUtil
{
	public static float CalcPowFrom(Vector3 BombPos, Vector3 position, int damage, float rad)
	{
		float num = Vector3.Distance(BombPos, position);
		if (num > rad)
		{
			return 0f;
		}
		float num2 = (rad - num) / rad;
		return (float)damage * num2;
	}

	public static void CheckMyself(Vector3 boomPos, int boomDamage, float boomRadius, int weaponBy)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null == component)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
			else
			{
				Vector3 position = gameObject.transform.position;
				if (Vector3.Distance(position, boomPos) < boomRadius)
				{
					int num = 0;
					for (int i = 0; i < 3; i++)
					{
						position.y += 0.3f;
						if (!Physics.Linecast(boomPos, position, out RaycastHit _, layerMask))
						{
							num += Mathf.FloorToInt(0.3f * (float)(i + 1) * CalcPowFrom(boomPos, position, boomDamage, boomRadius));
						}
					}
					if (num > 0)
					{
						component.GetHit(MyInfoManager.Instance.Seq, num, 1f, weaponBy, -1, autoHealPossible: true, checkZombie: false);
					}
				}
			}
		}
	}

	public static void CheckBoxmen(Vector3 boomPos, int boomDamage, float boomRadius, int weaponBy, float rigidity)
	{
		HitPart[] array = CheckBoxmen(boomPos, boomRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				allComponents[0].Desc.accumDamaged += Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position, boomDamage, boomRadius));
			}
		}
		GameObject[] array2 = BrickManManager.Instance.ToGameObjectArray();
		for (int j = 0; j < array2.Length; j++)
		{
			PlayerProperty component = array2[j].GetComponent<PlayerProperty>();
			if (null != component && component.Desc.accumDamaged > 0)
			{
				P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, component.Desc.Seq, component.Desc.accumDamaged, rigidity, weaponBy);
				component.Desc.accumDamaged = 0;
			}
		}
	}

	public static void CheckMonster(Vector3 boomPos, int boomDamage, float boomRadius, bool teamConsider = true)
	{
		HitPart[] array = CheckMon(boomPos, boomRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (!teamConsider || ((MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))))
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position, boomDamage, boomRadius));
				if (num > 0)
				{
					MonManager.Instance.Hit(allComponents[0].Desc.Seq, num, 1f, -3, Vector3.zero, Vector3.zero, -1);
				}
			}
		}
	}

	public static void CheckDestructibles(Vector3 boomPos, int boomDamage, float boomRadius)
	{
		BrickProperty[] array = CheckDestructibles(boomPos, boomRadius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Hit((int)CalcPowFrom(boomPos, array[i].transform.position, boomDamage, boomRadius));
			if (array[i].HitPoint <= 0)
			{
				CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(array[i].Seq);
			}
			else
			{
				P2PManager.Instance.SendPEER_BRICK_HITPOINT(array[i].Seq, array[i].HitPoint);
			}
		}
	}

	public static HitPart[] CheckBoxmen(Vector3 position, float radius, bool includeFriendly)
	{
		List<HitPart> list = new List<HitPart>();
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Me")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty component = array[i].GetComponent<PlayerProperty>();
			if (null != component && (includeFriendly || component.Desc.IsHostile()) && Vector3.Distance(position, array[i].transform.position) < radius)
			{
				HitPart[] componentsInChildren = array[i].GetComponentsInChildren<HitPart>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (Physics.Linecast(position, componentsInChildren[j].transform.position, out RaycastHit hitInfo, layerMask) && hitInfo.transform.gameObject == componentsInChildren[j].transform.gameObject)
					{
						list.Add(componentsInChildren[j]);
					}
				}
			}
		}
		return list.ToArray();
	}

	public static HitPart[] CheckMon(Vector3 position, float radius, bool includeFriendly)
	{
		List<HitPart> list = new List<HitPart>();
		GameObject[] array = MonManager.Instance.ToGameObjectArray();
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty component = array[i].GetComponent<MonProperty>();
			if (null != component && (includeFriendly || component.Desc.IsHostile()) && Vector3.Distance(position, array[i].transform.position) < radius)
			{
				HitPart[] componentsInChildren = array[i].GetComponentsInChildren<HitPart>();
				list.Add(componentsInChildren[0]);
			}
		}
		return list.ToArray();
	}

	public static BrickProperty[] CheckDestructibles(Vector3 position, float radius)
	{
		List<BrickProperty> list = new List<BrickProperty>();
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Me")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("InvincibleArmor"));
		int layerMask2 = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Collider[] array = Physics.OverlapSphere(position, radius, layerMask2);
		for (int i = 0; i < array.Length; i++)
		{
			BrickProperty[] array2 = null;
			array2 = ((array[i].gameObject.layer != LayerMask.NameToLayer("Brick")) ? array[i].GetComponentsInChildren<BrickProperty>(includeInactive: true) : Recursively.GetAllComponents<BrickProperty>(array[i].gameObject.transform, includeInactive: false));
			if (array2.Length > 0)
			{
				Brick brick = BrickManager.Instance.GetBrick(array2[0].Index);
				if (brick != null && brick.destructible)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						RaycastHit hitInfo;
						if (!(Vector3.Distance(position, array2[j].transform.position) > radius) && Physics.Linecast(position, array2[j].transform.position, out hitInfo, layerMask) && Vector3.Distance(hitInfo.point, array2[j].transform.position) < 1f)
						{
							list.Add(array2[j]);
						}
					}
				}
			}
		}
		return list.ToArray();
	}
}
