using UnityEngine;

public class MapEditingInfo : MonoBehaviour
{
	public RenderTexture thumbnail;

	public float width;

	public float height;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D constructor;

	public Vector2 crdSize = new Vector2(152f, 189f);

	public Rect crdConstructor = new Rect(0f, 0f, 100f, 100f);

	public Rect crdThumbnail = new Rect(17f, 55f, 128f, 128f);

	public Rect crdThumbnailOutline = new Rect(17f, 55f, 128f, 128f);

	public byte[] ThumbnailToPNG()
	{
		int num = thumbnail.width;
		int num2 = thumbnail.height;
		RenderTexture.active = thumbnail;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGB24, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		return texture2D.EncodeToPNG();
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			UserMapInfo cur = UserMapInfoManager.Instance.GetCur();
			if (cur != null && BrickManager.Instance.IsLoaded)
			{
				GUI.BeginGroup(new Rect((float)Screen.width - crdSize.x, (float)Screen.height - crdSize.y, crdSize.x, crdSize.y));
				TextureUtil.DrawTexture(crdConstructor, constructor, ScaleMode.StretchToFill);
				GUI.Box(crdThumbnailOutline, string.Empty, "BoxBrickOutline");
				TextureUtil.DrawTexture(crdThumbnail, thumbnail);
				GUI.EndGroup();
				GUI.enabled = true;
			}
		}
	}
}
