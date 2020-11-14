using System.Collections;
using UnityEngine;

public class SettingChanger : MonoBehaviour
{
	private SettingParam sp;

	private void OnSettingChange(SettingParam param)
	{
		sp = param;
		StartCoroutine(ApplyChange());
	}

	private void Start()
	{
	}

	private IEnumerator ApplyChange()
	{
		bool fullscreen = sp.fullScreen;
		int maxResIdx = Screen.resolutions.Length - 1;
		int maxWidth = Screen.resolutions[maxResIdx].width;
		int maxHeight = Screen.resolutions[maxResIdx].height;
		if (sp.width >= maxWidth && sp.height >= maxHeight)
		{
			fullscreen = true;
		}
		Screen.SetResolution(sp.width, sp.height, fullscreen);
		PlayerPrefs.SetInt("BfScreenWidth", sp.width);
		PlayerPrefs.SetInt("BfScreenHeight", sp.height);
		PlayerPrefs.SetInt("BfFullScreen", sp.fullScreen ? 1 : 0);
		yield return (object)null;
		QualitySettings.SetQualityLevel(sp.qualityLevel);
		PlayerPrefs.SetInt("BfQualityLevel", QualitySettings.GetQualityLevel());
		sp = null;
	}

	private void Update()
	{
	}
}
