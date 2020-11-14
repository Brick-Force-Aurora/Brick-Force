using UnityEngine;

public class TouchTrigger : Trigger
{
	private void OnTriggerEnter(Collider other)
	{
		if (!Application.loadedLevelName.Contains("MapEditor") && base.enabled)
		{
			LocalController component = other.GetComponent<LocalController>();
			if (null != component)
			{
				RunScript();
			}
		}
	}
}
