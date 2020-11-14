using UnityEngine;

public class ParentFollow : MonoBehaviour
{
	private Transform hitParent;

	private float Elapsedtime;

	private bool forceDead;

	private int parentSeq = -1;

	private bool isHuman = true;

	public Transform HitParent
	{
		get
		{
			return hitParent;
		}
		set
		{
			hitParent = value;
		}
	}

	public int ParentSeq
	{
		get
		{
			return parentSeq;
		}
		set
		{
			parentSeq = value;
		}
	}

	public bool IsHuman
	{
		set
		{
			isHuman = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (forceDead)
		{
			Elapsedtime += Time.deltaTime;
			if (Elapsedtime > 1f)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
		}
		if (hitParent != null)
		{
			if (isHuman)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(parentSeq);
				if (desc != null && desc.Hp <= 0)
				{
					forceDead = true;
				}
			}
			else
			{
				MonDesc desc2 = MonManager.Instance.GetDesc(parentSeq);
				if (desc2 != null && desc2.Xp <= 0)
				{
					forceDead = true;
				}
			}
			base.gameObject.transform.parent = hitParent;
		}
	}
}
