using UnityEngine;

public class StreamedLevelLoadibilityChecker : MonoBehaviour
{
	public string[] shouldBeStreamedLevel;

	public bool outputDebugMessage = true;

	public bool CanStreamedLevelBeLoaded()
	{
		for (int i = 0; i < shouldBeStreamedLevel.Length; i++)
		{
			if (!Application.CanStreamedLevelBeLoaded(shouldBeStreamedLevel[i]))
			{
				if (outputDebugMessage)
				{
					Debug.LogError("Level is not streamed yet : " + shouldBeStreamedLevel[i]);
				}
				return false;
			}
		}
		return true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
