using UnityEngine;

public class ZombieMatchScore : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public ImageFont curFont;

	public ImageFont totalFont;

	public Texture2D scoreBg;

	public Texture2D gaugeBg;

	public Texture2D gauge;

	public Texture2D humanIcon;

	public Texture2D zombieIcon;

	public Texture2D arrow;

	private int cur;

	private int total;

	private Vector2 size = new Vector2(162f, 50f);

	private Vector2 crdCurrent = new Vector2(4f, 46f);

	private Vector2 crdTotal = new Vector2(158f, 4f);

	private Vector2 sizeZH = new Vector2(344f, 60f);

	private float offsetZH = 36f;

	private Rect crdZombieIcon = new Rect(307f, 0f, 37f, 37f);

	private Rect crdHumanIcon = new Rect(0f, 0f, 37f, 37f);

	private Rect crdZombieGaugeBg = new Rect(178f, 20f, 130f, 14f);

	private Rect crdHumanGaugeBg = new Rect(36f, 20f, 130f, 14f);

	private Rect crdZombieGauge = new Rect(0f, 0f, 0f, 0f);

	private Rect crdHumanGauge = new Rect(0f, 0f, 0f, 0f);

	private Rect crdZombieArrow = new Rect(0f, 0f, 0f, 0f);

	private Rect crdHumanArrow = new Rect(0f, 0f, 0f, 0f);

	private Vector2 crdZombieCur = new Vector2(0f, 0f);

	private Vector2 crdHumanCur = new Vector2(0f, 0f);

	private void Start()
	{
		cur = 1;
		total = RoomManager.Instance.KillCount;
		if (MyInfoManager.Instance.BreakingInto)
		{
			CSNetManager.Instance.Sock.SendCS_ZOMBIE_MODE_SCORE_REQ();
		}
	}

	private void Update()
	{
	}

	private void DrawRounding()
	{
		GUI.BeginGroup(new Rect(((float)Screen.width - size.x) / 2f, 0f, size.x, size.y));
		TextureUtil.DrawTexture(new Rect(0f, 0f, size.x, size.y), scoreBg, ScaleMode.StretchToFill);
		curFont.Print(crdCurrent, cur);
		totalFont.Print(crdTotal, total);
		GUI.EndGroup();
	}

	private void DrawZombieVsHuman()
	{
		GUI.BeginGroup(new Rect(((float)Screen.width - sizeZH.x) / 2f, offsetZH, sizeZH.x, sizeZH.y));
		float humanRatio = ZombieVsHumanManager.Instance.GetHumanRatio(RoomManager.Instance.GetCurrentRoomInfo().MaxPlayer);
		string text = ZombieVsHumanManager.Instance.GetHumanCount().ToString();
		float num = crdHumanGaugeBg.width * humanRatio;
		crdHumanGauge.x = crdHumanGaugeBg.x + crdHumanGaugeBg.width - num;
		crdHumanGauge.y = crdHumanGaugeBg.y;
		crdHumanGauge.width = num;
		crdHumanGauge.height = crdHumanGaugeBg.height;
		crdHumanArrow.x = crdHumanGauge.x - (float)(arrow.width / 2);
		crdHumanArrow.y = crdHumanGaugeBg.y;
		crdHumanArrow.width = (float)arrow.width;
		crdHumanArrow.height = (float)arrow.height;
		crdHumanCur.x = crdHumanGauge.x + 5f;
		crdHumanCur.y = crdHumanGaugeBg.y + (float)arrow.height;
		TextureUtil.DrawTexture(crdHumanIcon, humanIcon);
		TextureUtil.DrawTexture(crdHumanGaugeBg, gaugeBg);
		TextureUtil.DrawTexture(crdHumanGauge, gauge, new Rect(1f - humanRatio, 0f, humanRatio, 1f));
		TextureUtil.DrawTexture(crdHumanArrow, arrow);
		LabelUtil.TextOut(crdHumanCur, text, "MiniLabel", Color.yellow, TextAnchor.UpperRight);
		float zombieRatio = ZombieVsHumanManager.Instance.GetZombieRatio(RoomManager.Instance.GetCurrentRoomInfo().MaxPlayer);
		string text2 = ZombieVsHumanManager.Instance.GetZombieCount().ToString();
		num = crdZombieGaugeBg.width * zombieRatio;
		crdZombieGauge.x = crdZombieGaugeBg.x;
		crdZombieGauge.y = crdZombieGaugeBg.y;
		crdZombieGauge.width = num;
		crdZombieGauge.height = crdZombieGaugeBg.height;
		crdZombieArrow.x = crdZombieGauge.x + crdZombieGauge.width - (float)(arrow.width / 2);
		crdZombieArrow.y = crdZombieGaugeBg.y;
		crdZombieArrow.width = (float)arrow.width;
		crdZombieArrow.height = (float)arrow.height;
		crdZombieCur.x = crdZombieGauge.x + crdZombieGauge.width;
		crdZombieCur.y = crdZombieGaugeBg.y + (float)arrow.height;
		TextureUtil.DrawTexture(crdZombieIcon, zombieIcon);
		TextureUtil.DrawTexture(crdZombieGaugeBg, gaugeBg);
		TextureUtil.DrawTexture(crdZombieGauge, gauge, new Rect(0f, 0f, zombieRatio, 1f));
		TextureUtil.DrawTexture(crdZombieArrow, arrow);
		LabelUtil.TextOut(crdZombieCur, text2, "MiniLabel", Color.yellow, TextAnchor.UpperLeft);
		GUI.EndGroup();
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawRounding();
			DrawZombieVsHuman();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void OnZombieScore(RoundScore zombieScore)
	{
		if (cur != zombieScore._cur)
		{
			cur = zombieScore._cur;
			curFont.Scale = 2f;
		}
		if (total != zombieScore._total)
		{
			total = zombieScore._total;
			totalFont.Scale = 2f;
		}
		if (cur > total)
		{
			cur = total;
		}
	}
}
