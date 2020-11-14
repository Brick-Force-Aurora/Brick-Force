using UnityEngine;

public class MatchEnder : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public float wait = 3f;

	public string loadLevel = string.Empty;

	public Texture2D[] endImage;

	private int ending = -1;

	private float deltaTime;

	public bool IsOverAll => ending >= 0;

	private void Start()
	{
	}

	private void OnMatchEnd(sbyte code)
	{
		GlobalVars.Instance.AllPlayerStatusToWaiting();
		ending = code + 1;
		int num = 8;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
		{
			num = 4;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.INDIVIDUAL || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ESCAPE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE)
		{
			VoiceManager.Instance.Play("EndGame");
		}
		else if (ending == 1)
		{
			VoiceManager.Instance.Play("Draw_1");
		}
		else if (ending == 2)
		{
			if (MyInfoManager.Instance.Slot < num)
			{
				VoiceManager.Instance.Play("RedWin_4");
			}
			else
			{
				VoiceManager.Instance.Play("BlueWin_2");
			}
		}
		else if (ending == 0)
		{
			if (MyInfoManager.Instance.Slot < num)
			{
				VoiceManager.Instance.Play("BlueWin_2");
			}
			else
			{
				VoiceManager.Instance.Play("RedWin_4");
			}
		}
		RoomManager.Instance.ClearVote();
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && ending >= 0)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (ending < endImage.Length)
			{
				float num = (float)endImage[ending].width;
				float num2 = (float)endImage[ending].height;
				TextureUtil.DrawTexture(new Rect(((float)Screen.width - num) / 2f, ((float)Screen.height - num2) / 2f, num, num2), endImage[ending]);
			}
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		if (ending >= 0 && !Application.isLoadingLevel)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > wait && !GlobalVars.Instance.shutdownNow)
			{
				Application.LoadLevel(loadLevel);
			}
		}
	}
}
