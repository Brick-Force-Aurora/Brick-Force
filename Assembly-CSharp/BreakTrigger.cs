using UnityEngine;

public class BreakTrigger : Trigger
{
	private void OnBreak()
	{
		if (!Application.loadedLevelName.Contains("MapEditor") && base.enabled)
		{
			RunScript();
		}
	}
}
