using UnityEngine;

public class SpeedUpTPEffect : MonoBehaviour
{
	public TPController owner;

	private void Start()
	{
	}

	private void Update()
	{
		if (owner != null && !owner.IsSpeedUp)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
