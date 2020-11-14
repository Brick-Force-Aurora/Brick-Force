using UnityEngine;

public class SelfCollsion : MonoBehaviour
{
	public bool bBoom;

	public Vector3 colPoint = Vector3.zero;

	public GameObject expolsion;

	private bool bUse = true;

	private bool collideEnter;

	private int idBrick = -1;

	private int idMon = -1;

	public int brickID
	{
		get
		{
			return idBrick;
		}
		set
		{
			idBrick = value;
		}
	}

	public int monID
	{
		get
		{
			return idMon;
		}
		set
		{
			idMon = value;
		}
	}

	public bool IsCollideEnter()
	{
		return collideEnter;
	}

	private void OnCollisionEnter(Collision col)
	{
		collideEnter = true;
		ContactPoint contactPoint = col.contacts[0];
		colPoint = contactPoint.point;
	}

	public void NoUse()
	{
		bUse = false;
	}

	public void Explosion(Vector3 point, Quaternion rot, bool myself)
	{
		if (bUse)
		{
			colPoint = point;
			if (!bBoom && expolsion != null)
			{
				GameObject gameObject = Object.Instantiate((Object)expolsion, point, rot) as GameObject;
				CheckBrickDead component = gameObject.GetComponent<CheckBrickDead>();
				if (component != null)
				{
					component.brickID = idBrick;
				}
				CheckMonDead component2 = gameObject.GetComponent<CheckMonDead>();
				if (component2 != null)
				{
					component2.MonID = idMon;
				}
				if (myself)
				{
					ParentFollow component3 = gameObject.GetComponent<ParentFollow>();
					if (component3 != null)
					{
						component3.HitParent = GlobalVars.Instance.hitParent;
						component3.ParentSeq = GlobalVars.Instance.hitBirckman;
					}
				}
			}
		}
	}
}
