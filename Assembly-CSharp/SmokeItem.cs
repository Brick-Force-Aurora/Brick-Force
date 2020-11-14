using UnityEngine;

public class SmokeItem : ActiveItemBase
{
	private void Awake()
	{
	}

	private void Update()
	{
	}

	public override void StartItem()
	{
		if (IsMyItem())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (component != null)
				{
					component.EquipSmokeBomb();
				}
			}
		}
		else
		{
			GameObject gameObject2 = BrickManManager.Instance.Get(useUserSeq);
			if (gameObject2 != null)
			{
				LookCoordinator component2 = gameObject2.GetComponent<LookCoordinator>();
				if (component2 != null)
				{
					component2.EquipSmokeBomb();
				}
			}
		}
	}
}
