using UnityEngine;

public class SerialKill : MonoBehaviour
{
	private const int ACTION_INDEX_HEADSHOT = -1;

	private const int ACTION_INDEX_GOALIN = -2;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public float serialKillTimeout;

	public Texture2D[] serialImage;

	public Texture2D serialBg;

	private string[] killVoc = new string[8]
	{
		"1_DoubleKill",
		"2_TrepleKill",
		"3_GreatKill",
		"4_UltraKill",
		"5_FantasticKill",
		"6_CrazyKill",
		"7_UnbeleivableKill",
		"7_UnbeleivableKill"
	};

	private float lastKill;

	private int serialKill;

	public Texture2D headshotImage;

	public Texture2D goalInImage;

	public AudioClip sndGoalIn;

	private Vector2 bgCenter = Vector2.zero;

	private Vector2 imgCenter = Vector2.zero;

	private Vector2 bgEnd = Vector2.zero;

	private Vector2 imgEnd = Vector2.zero;

	private Vector2 bgPoint = Vector2.zero;

	private Vector2 imgPoint = Vector2.zero;

	private int actionIndex = -1;

	private int actionStep = -1;

	private float actionTimer;

	private void HeadshotAction()
	{
		actionIndex = -1;
		actionStep = 0;
		actionTimer = 0f;
		VoiceManager.Instance.Play("HeadShot07");
		bgPoint = new Vector2((float)(-serialBg.width), 64f);
		bgCenter = new Vector2((float)((Screen.width - serialBg.width) / 2), 64f);
		bgEnd = new Vector2((float)Screen.width, 64f);
		imgPoint = new Vector2((float)Screen.width, 64f);
		imgCenter = new Vector2((float)((Screen.width - headshotImage.width) / 2), 64f);
		imgEnd = new Vector2((float)(-headshotImage.width), 64f);
	}

	public void GoalInAction()
	{
		if (!(goalInImage == null))
		{
			actionIndex = -2;
			actionStep = 0;
			actionTimer = 0f;
			if (sndGoalIn != null)
			{
				GlobalVars.Instance.PlayOneShot(sndGoalIn);
			}
			bgPoint = new Vector2((float)(-serialBg.width), 64f);
			bgCenter = new Vector2((float)((Screen.width - serialBg.width) / 2), 64f);
			bgEnd = new Vector2((float)Screen.width, 64f);
			imgPoint = new Vector2((float)Screen.width, 64f);
			imgCenter = new Vector2((float)((Screen.width - goalInImage.width) / 2), 64f);
			imgEnd = new Vector2((float)(-goalInImage.width), 64f);
		}
	}

	private bool SerialAction()
	{
		actionIndex = serialKill - 2;
		if (actionIndex < 0)
		{
			return false;
		}
		actionStep = 0;
		actionTimer = 0f;
		CSNetManager.Instance.Sock.SendCS_SERIAL_BONUS_REQ(serialKill);
		if (actionIndex >= serialImage.Length)
		{
			actionIndex = serialImage.Length - 1;
		}
		VoiceManager.Instance.Play(killVoc[actionIndex]);
		bgPoint = new Vector2((float)(-serialBg.width), 64f);
		bgCenter = new Vector2((float)((Screen.width - serialBg.width) / 2), 64f);
		bgEnd = new Vector2((float)Screen.width, 64f);
		imgPoint = new Vector2((float)Screen.width, 64f);
		imgCenter = new Vector2((float)((Screen.width - serialImage[actionIndex].width) / 2), 64f);
		imgEnd = new Vector2((float)(-serialImage[actionIndex].width), 64f);
		return true;
	}

	private void Update()
	{
		if (serialKill > 0)
		{
			lastKill += Time.deltaTime;
			if (lastKill > serialKillTimeout)
			{
				serialKill = 0;
				lastKill = 0f;
			}
		}
		bool flag = true;
		bool flag2 = true;
		switch (actionStep)
		{
		case 0:
			if (Vector2.Distance(imgPoint, imgCenter) > 0.1f)
			{
				flag = false;
				imgPoint = Vector2.Lerp(imgPoint, imgCenter, 8f * Time.deltaTime);
			}
			if (Vector2.Distance(bgPoint, bgCenter) > 0.1f)
			{
				flag2 = false;
				bgPoint = Vector2.Lerp(bgPoint, bgCenter, 8f * Time.deltaTime);
			}
			if (flag && flag2)
			{
				actionStep++;
			}
			break;
		case 1:
			actionTimer += Time.deltaTime;
			if (actionTimer > 0.5f)
			{
				actionStep++;
			}
			break;
		case 2:
			if (Vector2.Distance(imgPoint, imgEnd) > 0.1f)
			{
				flag = false;
				imgPoint = Vector2.Lerp(imgPoint, imgEnd, 5f * Time.deltaTime);
			}
			if (Vector2.Distance(bgPoint, bgEnd) > 0.1f)
			{
				flag2 = false;
				bgPoint = Vector2.Lerp(bgPoint, bgEnd, 5f * Time.deltaTime);
			}
			if (flag && flag2)
			{
				actionStep++;
			}
			break;
		}
	}

	private void Start()
	{
	}

	private void OnKillLog(KillInfo log)
	{
		if (log.KillerSequence == MyInfoManager.Instance.Seq && log.VictimSequence != MyInfoManager.Instance.Seq)
		{
			serialKill++;
			lastKill = 0f;
			if (!SerialAction() && log.HeadShot != null)
			{
				HeadshotAction();
			}
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (0 <= actionStep && actionStep <= 2)
			{
				Texture2D texture2D = null;
				if (0 <= actionIndex && actionIndex < serialImage.Length)
				{
					texture2D = serialImage[actionIndex];
				}
				else if (actionIndex == -1)
				{
					texture2D = headshotImage;
				}
				else if (actionIndex == -2)
				{
					texture2D = goalInImage;
				}
				if (texture2D != null)
				{
					TextureUtil.DrawTexture(new Rect(bgPoint.x, bgPoint.y, (float)serialBg.width, (float)serialBg.height), serialBg);
					TextureUtil.DrawTexture(new Rect(imgPoint.x, imgPoint.y, (float)texture2D.width, (float)texture2D.height), texture2D);
				}
			}
			GUI.enabled = true;
		}
	}
}
