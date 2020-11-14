using UnityEngine;

public class DailyMissionMonitor : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D title;

	private float labelWidth = 120f;

	private float labelX = 40f;

	private float labelY = 48f;

	private float offset = 2f;

	private float toggleX = 34f;

	private Vector2 crdToggle = new Vector2(22f, 22f);

	private Vector2 crdTitle = new Vector2(93f, 34f);

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && MissionManager.Instance.HaveMission)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DoTitle();
			DoDailyMission();
			GUI.enabled = true;
		}
	}

	private void DoTitle()
	{
		Rect position = new Rect((float)(Screen.width - title.width), 0f, (float)title.width, (float)title.height);
		TextureUtil.DrawTexture(position, title);
		LabelUtil.TextOut(new Vector2((float)Screen.width - crdTitle.x, crdTitle.y), StringMgr.Instance.Get("BRICK_KINGS_ORDER"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
	}

	private void DoDailyMission()
	{
		Vector2 vector = new Vector2((float)(Screen.width - title.width) + labelX, labelY);
		Mission[] array = MissionManager.Instance.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			Color color = (!array[i].Completed && !array[i].CanComplete) ? new Color(0.11f, 0.05f, 0.05f, 1f) : new Color(1f, 0.82f, 0f, 1f);
			Vector2 vector2 = LabelUtil.CalcSize("MiniLabel", array[i].MiniDescription, labelWidth);
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.Label(new Rect(vector.x, vector.y, vector2.x, vector2.y), array[i].MiniDescription, "MiniLabel");
			GUI.color = color2;
			float num = vector.y + vector2.y / 2f;
			LabelUtil.TextOut(new Vector2((float)Screen.width, num), array[i].ProgressString, "MiniLabel", color, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			Rect position = new Rect(vector.x - toggleX, num - crdToggle.y / 2f, crdToggle.x, crdToggle.y);
			GUI.Toggle(position, array[i].Completed || array[i].CanComplete, string.Empty);
			vector.y += vector2.y + offset;
		}
	}
}
