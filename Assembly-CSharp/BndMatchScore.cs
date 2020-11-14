using UnityEngine;

public class BndMatchScore : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public ImageFont redScoreFont;

	public ImageFont blueScoreFont;

	public ImageFont goalFont;

	public Texture2D scoreBg;

	private int redTeamScore;

	private int blueTeamScore;

	private float paletteOffset = 76f;

	private Rect rcBndMatchScoreBg = new Rect(0f, 0f, 242f, 50f);

	private Vector2 crdRed = new Vector2(10f, 48f);

	private Vector2 crdBlue = new Vector2(232f, 48f);

	private Vector2 crdGoal = new Vector2(121f, 25f);

	private BndMatch bndMatch;

	public UIFlickerColor flickerRed;

	public UIFlickerColor flickerBlue;

	private void Start()
	{
		bndMatch = null;
		redTeamScore = 0;
		blueTeamScore = 0;
		if (MyInfoManager.Instance.BreakingInto)
		{
			CSNetManager.Instance.Sock.SendCS_BND_SCORE_REQ();
		}
	}

	private void VerifyBndMatch()
	{
		if (null == bndMatch)
		{
			bndMatch = GetComponent<BndMatch>();
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
			VerifyBndMatch();
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			Rect position = new Rect((float)((Screen.width - scoreBg.width) / 2), (!bndMatch.AmIUsingBuildGun) ? 0f : paletteOffset, (float)scoreBg.width, (float)scoreBg.height);
			GUI.BeginGroup(position);
			TextureUtil.DrawTexture(rcBndMatchScoreBg, scoreBg, ScaleMode.StretchToFill);
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
