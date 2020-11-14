using UnityEngine;

public class LoadBrickMain : MonoBehaviour
{
	public Texture loadingImage;

	public Vector2 logoSize = new Vector2(343f, 120f);

	private void Start()
	{
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
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 32)), "Loading others..." + SceneLoadManager.Instance.GetProgressString(), "BigLabel", new Color(0.9f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (BrickManager.Instance.result4TeamMatch != null && Application.CanStreamedLevelBeLoaded("LoadOthers") && !SceneLoadManager.Instance.IsLoadStart("LoadOthers"))
		{
			SceneLoadManager.Instance.SceneLoadLevelAsync("LoadOthers");
		}
	}
}
