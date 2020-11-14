using UnityEngine;

public class SenseTrigger : Trigger
{
	public void OnTriggerEnter(Collider other)
	{
		if (!Application.loadedLevelName.Contains("MapEditor") && base.enabled)
		{
			LocalController component = other.GetComponent<LocalController>();
			if (component == null)
			{
				GlobalVars.Instance.immediateKillBrickTutor = true;
				RunScript();
			}
		}
	}
}
