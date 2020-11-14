using UnityEngine;

public class TimeLimitedPoisonDestroyer : MonoBehaviour
{
	public int poisonDamage = 10;

	public float limit = 1f;

	private float deltaTime;

	private float dtPoision;

	private void Start()
	{
		deltaTime = 0f;
		dtPoision = 10f;
	}

	private void Update()
	{
		dtPoision += Time.deltaTime;
		if (dtPoision > 1f)
		{
			CheckMyself(base.gameObject.transform.position, GlobalVars.Instance.RadiusPoisonBrick);
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				CheckMonster(base.gameObject.transform.position, GlobalVars.Instance.RadiusPoisonBrick);
			}
			dtPoision = 0f;
		}
		deltaTime += Time.deltaTime;
		if (deltaTime > limit)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	private void CheckMyself(Vector3 boomPos, float DamageRadius)
	{
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
				if (Vector3.Distance(position, boomPos) < DamageRadius)
				{
					component.GetHit(MyInfoManager.Instance.Seq, poisonDamage, 1f, -4, -1, autoHealPossible: true, checkZombie: false);
				}
			}
		}
	}

	private void CheckBoxmen(Vector3 boomPos, float DamageRadius)
	{
		HitPart[] array = ExplosionUtil.CheckBoxmen(boomPos, DamageRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, allComponents[0].Desc.Seq, poisonDamage, 1f, -4);
			}
		}
	}

	private void CheckMonster(Vector3 boomPos, float DamageRadius)
	{
		HitPart[] array = ExplosionUtil.CheckMon(boomPos, DamageRadius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))
			{
				MonManager.Instance.Hit(allComponents[0].Desc.Seq, poisonDamage, 1f, -4, Vector3.zero, Vector3.zero, -1);
			}
		}
	}
}
