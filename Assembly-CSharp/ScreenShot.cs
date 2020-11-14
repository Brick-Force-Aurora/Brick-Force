using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
	private bool bSaving;

	private void CreateFolderIfNotExists()
	{
		string path = ScreenShotFolderName();
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	private string ScreenShotFolderName()
	{
		return Application.dataPath + "/../Screenshots";
	}

	private string ScreenShotName(int width, int height)
	{
		return string.Format("{0}/Screenshot_{1}.png", ScreenShotFolderName(), DateTime.Now.ToString("yyyyMMdd_HHmmss"));
	}

	private void Update()
	{
		if (!bSaving && custom_inputs.Instance.GetButtonDown("K_SCREEN_SHOT"))
		{
			bSaving = true;
			MakeScreenshot();
		}
	}

	private void MakeScreenshot()
	{
		CreateFolderIfNotExists();
		StartCoroutine(ScreenshotEncode());
	}

	private IEnumerator ScreenshotEncode()
	{
		yield return (object)new WaitForEndOfFrame();
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipmap: false);
		texture.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
		texture.Apply();
		yield return (object)0;
		byte[] bytes = texture.EncodeToPNG();
		string screenShotName = ScreenShotName(Screen.width, Screen.height);
		File.WriteAllBytes(screenShotName, bytes);
		UnityEngine.Object.DestroyObject(texture);
		SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("SCREENSHOT_SAVE"), Path.GetFileName(screenShotName)));
		bSaving = false;
	}
}
