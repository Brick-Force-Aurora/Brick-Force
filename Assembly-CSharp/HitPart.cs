using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitPart : MonoBehaviour
{
	public enum TYPE
	{
		HEAD,
		BODY,
		ARM,
		FOOT,
		BRAIN
	}

	public float damageFactor = 1f;

	public TYPE part;

	public GameObject hitImpact;

	public GameObject hitImpactChild;

	public GameObject luckyImpact;

	public GameObject GetHitImpact()
	{
		if (MyInfoManager.Instance.IsBelow12())
		{
			return hitImpactChild;
		}
		return hitImpact;
	}
}
