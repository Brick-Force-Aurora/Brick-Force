using CodeStage.AntiCheat.Detectors;
using System.Collections;
using UnityEngine;

public class BootstrapMain : MonoBehaviour
{
	private bool setup;

	private bool once;

	public Texture loadingImage;

	private int minScreenWidth = 800;

	private int minScreenHeight = 600;

	public Vector2 logoSize = new Vector2(343f, 120f);

	private void Awake()
	{
		InjectionDetector.StartDetection(OnInjectionDetected);
		Input.eatKeyPressOnTextFieldFocus = false;
	}

	private void OnInjectionDetected()
	{
		Debug.Log("Injection Detected.");
		BuildOption.Instance.HardExit();
	}

	private void Start()
	{
		once = false;
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 vector = new Vector2((float)((Screen.width - loadingImage.width) / 2), (float)((Screen.height - loadingImage.height) / 2));
		TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)loadingImage.width, (float)loadingImage.height), loadingImage);
		Texture2D logo = BuildOption.Instance.Props.logo;
		if (null != logo)
		{
			Vector2 vector2 = new Vector2((float)((Screen.width - logo.width) / 2), vector.y + (float)loadingImage.height - logoSize.y);
			TextureUtil.DrawTexture(new Rect(vector2.x, vector2.y, (float)logo.width, (float)logo.height), logo, ScaleMode.StretchToFill);
		}
		if (LangOptManager.Instance.IsFontReady)
		{
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 32)), "Loading Bricks...", "BigLabel", new Color(0.9f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		}
	}

	private IEnumerator Init()
	{
		int width = PlayerPrefs.GetInt("BfScreenWidth", Screen.currentResolution.width);
		int height = PlayerPrefs.GetInt("BfScreenHeight", Screen.currentResolution.height);
		int fullScreen = PlayerPrefs.GetInt("BfFullScreen", BuildOption.Instance.Props.fullScreenMode ? 1 : 0);
		if (width < minScreenWidth || height < minScreenHeight)
		{
			width = minScreenWidth;
			height = minScreenHeight;
		}
		int maxResIdx = Screen.resolutions.Length - 1;
		int maxWidth = Screen.resolutions[maxResIdx].width;
		int maxHeight = Screen.resolutions[maxResIdx].height;
		if (width >= maxWidth && height >= maxHeight)
		{
			fullScreen = 1;
		}
		Screen.SetResolution(width, height, (fullScreen == 1) ? true : false);
		yield return (object)null;
		int currentLevel = PlayerPrefs.GetInt("BfQualityLevel", 3);
		QualitySettings.SetQualityLevel(currentLevel);
		yield return (object)null;
		setup = true;
	}

	private void Update()
	{
		if (LangOptManager.Instance.IsFontReady && !once && SceneLoadManager.Instance.IsLoadedDone())
		{
			once = true;
			StringMgr.Instance.Load();
			WordFilter.Instance.Load();
			StartCoroutine(Init());
		}
	}

	private void LateUpdate()
	{
		if (setup && Application.CanStreamedLevelBeLoaded("LoadBrick"))
		{
			Application.LoadLevel("LoadBrick");
		}
	}
}
