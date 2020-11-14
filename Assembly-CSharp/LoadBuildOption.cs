using UnityEngine;

public class LoadBuildOption : MonoBehaviour
{
	private bool isChangeScene;

	public GameObject buildOption;

	private void Start()
	{
		Object.Instantiate((Object)buildOption);
	}

	private void Update()
	{
		if (!isChangeScene && Application.CanStreamedLevelBeLoaded("Bootstrap") && !SceneLoadManager.Instance.IsLoadStart("Bootstrap"))
		{
			SceneLoadManager.Instance.SceneLoadLevelAsync("Bootstrap");
			isChangeScene = true;
		}
	}
}
