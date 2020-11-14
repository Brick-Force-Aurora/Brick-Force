using UnityEngine;

public class BungeeMatchScore : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public ImageFont scoreFont;

	public ImageFont goalFont;

	public Texture2D scoreBg;

	public Vector2 size = new Vector2(242f, 50f);

	public Vector2 crdScore = new Vector2(0f, 0f);

	public Vector2 crdGoal = new Vector2(0f, 0f);

	private int score;

	private void Start()
	{
		score = 0;
		if (MyInfoManager.Instance.BreakingInto)
		{
			CSNetManager.Instance.Sock.SendCS_BUNGEE_SCORE_REQ();
		}
	}

	private void OnBungeeScore(int totalKill)
	{
		scoreFont.Scale = 2f;
		score = totalKill;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup(new Rect(((float)Screen.width - size.x) / 2f, 0f, size.x, size.y));
			TextureUtil.DrawTexture(new Rect(0f, 0f, size.x, size.y), scoreBg, ScaleMode.StretchToFill);
			scoreFont.Print(crdScore, score);
			goalFont.Print(crdGoal, RoomManager.Instance.KillCount);
			GUI.EndGroup();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
	}
}
