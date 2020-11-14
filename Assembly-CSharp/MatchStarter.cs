using UnityEngine;

public class MatchStarter : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public Texture2D[] countDigit;

	private bool countDown;

	private int count;

	private float deltaTime;

	public float waitBoxWidth = 700f;

	public float waitBoxHeight = 100f;

	public int MaxCount => countDigit.Length - 1;

	public bool CountDown => countDown;

	private void Start()
	{
		count = 0;
		if (MyInfoManager.Instance.BreakingInto)
		{
			count = countDigit.Length;
		}
	}

	private void OnMatchCountDown(int cnt)
	{
		count = cnt;
		countDown = (count < countDigit.Length);
		deltaTime = 0f;
		if (count == 0)
		{
			VoiceManager.Instance.Play("Ingame_Ready_combo_1");
		}
		else if (count == countDigit.Length - 1)
		{
			VoiceManager.Instance.Play("Ingame_Start_combo_1");
			BroadcastMessage("OnResume", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && 0 <= count && count < countDigit.Length)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			bool flag = true;
			BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Status == 2 || array[i].Status == 3)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				Rect position = new Rect(((float)Screen.width - waitBoxWidth) / 2f, ((float)Screen.height - waitBoxHeight) / 2f, waitBoxWidth, waitBoxHeight);
				GUI.Box(position, string.Empty, "BoxBase");
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)), StringMgr.Instance.Get("OTHER_PLAYER_ENTER_WAIT"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter, waitBoxWidth - 20f);
			}
			else
			{
				Texture2D texture2D = countDigit[count];
				if (null != texture2D)
				{
					TextureUtil.DrawTexture(new Rect((float)((Screen.width - texture2D.width) / 2), (float)((Screen.height - texture2D.height) / 2), (float)texture2D.width, (float)texture2D.height), texture2D);
				}
			}
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
		{
			if (countDown)
			{
				deltaTime += Time.deltaTime;
				if (deltaTime > 1f)
				{
					deltaTime = 0f;
					count++;
					if (count >= countDigit.Length)
					{
						VoiceManager.Instance.Play("Start_Ment_1");
						countDown = false;
						MyInfoManager.Instance.ClanMatchExplosionChangeMessage();
					}
					CSNetManager.Instance.Sock.SendCS_MATCH_COUNTDOWN_REQ(count);
					if (count == countDigit.Length - 1)
					{
						VoiceManager.Instance.Play("Ingame_Start_combo_1");
						BroadcastMessage("OnResume", null, SendMessageOptions.DontRequireReceiver);
					}
					if (count >= countDigit.Length - 1)
					{
						CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(2);
					}
				}
			}
			else if (count <= 0 && BrickManager.Instance.IsLoaded)
			{
				bool flag = true;
				BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Status == 2 || array[i].Status == 3)
					{
						flag = false;
					}
				}
				if (flag)
				{
					VoiceManager.Instance.Play("Ingame_Ready_combo_1");
					CSNetManager.Instance.Sock.SendCS_MATCH_COUNTDOWN_REQ(count);
					countDown = true;
					deltaTime = 0f;
				}
			}
		}
		else if (count == countDigit.Length - 1)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 1f)
			{
				VoiceManager.Instance.Play("Start_Ment_1");
				countDown = false;
				count = 0;
			}
			MyInfoManager.Instance.ClanMatchExplosionChangeMessage();
		}
	}
}
