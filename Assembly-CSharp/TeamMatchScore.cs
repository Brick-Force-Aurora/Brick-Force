using UnityEngine;

public class TeamMatchScore : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public ImageFont redScoreFont;

	public ImageFont blueScoreFont;

	public ImageFont goalFont;

	public Texture2D scoreBg;

	private int redTeamScore;

	private int blueTeamScore;

	public Vector2 size = new Vector2(242f, 50f);

	public Vector2 crdRed = new Vector2(10f, 48f);

	public Vector2 crdBlue = new Vector2(232f, 48f);

	public Vector2 crdGoal = new Vector2(121f, 25f);

	public UIFlickerColor flickerRed;

	public UIFlickerColor flickerBlue;

	private void Start()
	{
		redTeamScore = 0;
		blueTeamScore = 0;
		if (MyInfoManager.Instance.BreakingInto)
		{
			CSNetManager.Instance.Sock.SendCS_TEAM_SCORE_REQ();
		}
	}

	private void OnTeamScore(TeamScore teamScore)
	{
		if (redTeamScore != teamScore.redTeam)
		{
			redTeamScore = teamScore.redTeam;
			redScoreFont.Scale = 2f;
		}
		if (blueTeamScore != teamScore.blueTeam)
		{
			blueTeamScore = teamScore.blueTeam;
			blueScoreFont.Scale = 2f;
		}
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
			if (MyInfoManager.Instance.IsRedTeam())
			{
				flickerRed.Draw();
			}
			else
			{
				flickerBlue.Draw();
			}
			redScoreFont.Print(crdRed, redTeamScore);
			blueScoreFont.Print(crdBlue, blueTeamScore);
			goalFont.Print(crdGoal, RoomManager.Instance.KillCount);
			GUI.EndGroup();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		flickerRed.Update();
		flickerBlue.Update();
	}
}
