using UnityEngine;

public class BrickNumber : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private Vector2 crdSize = new Vector2(560f, 24f);

	public Vector2 crdCountBox = new Vector2(80f, 18f);

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && BrickManager.Instance.IsLoaded)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup(new Rect(((float)Screen.width - crdSize.x) / 2f, 77f, crdSize.x, crdSize.y));
			GUI.Box(new Rect(0f, 0f, crdSize.x, crdSize.y), string.Empty);
			int num = BrickManager.Instance.Count - BrickManager.Instance.SpecialCount;
			int specialCount = BrickManager.Instance.SpecialCount;
			LabelUtil.TextOut(new Vector2(crdSize.x / 4f - 5f, 0f), StringMgr.Instance.Get("GENERAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			GUI.Box(new Rect(crdSize.x / 4f, 3f, crdCountBox.x, crdCountBox.y), string.Empty, "BoxBrickText");
			LabelUtil.TextOut(new Vector2(crdSize.x / 4f + crdCountBox.x - 5f, 0f), num.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(new Vector2(crdSize.x / 4f + crdCountBox.x + 5f, 0f), StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(new Vector2(3f * crdSize.x / 4f - 5f, 0f), StringMgr.Instance.Get("SPECIAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			GUI.Box(new Rect(3f * crdSize.x / 4f, 3f, crdCountBox.x, crdCountBox.y), string.Empty, "BoxBrickText");
			LabelUtil.TextOut(new Vector2(3f * crdSize.x / 4f + crdCountBox.x - 5f, 0f), specialCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(new Vector2(3f * crdSize.x / 4f + crdCountBox.x + 5f, 0f), StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}
}
