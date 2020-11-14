using UnityEngine;

public class MapEditorSituation : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public RenderTexture thumbnail;

	public Texture2D pointIcon;

	private Vector2 crdSize = new Vector2(612f, 347f);

	public Vector2 crdRoomTitle = new Vector2(10f, 7f);

	private Rect crdBasicBox = new Rect(18f, 50f, 575f, 148f);

	private Rect crdThumbnail = new Rect(30f, 60f, 128f, 128f);

	private Rect crdThumbnailBox = new Rect(29f, 59f, 130f, 130f);

	private Rect crdGeneralBox = new Rect(18f, 212f, 575f, 20f);

	private Rect crdSpecialBox = new Rect(18f, 239f, 575f, 20f);

	private Rect crdDeleteBox = new Rect(18f, 266f, 575f, 20f);

	private Rect crdFeeBox = new Rect(18f, 293f, 575f, 20f);

	private Vector2 crdDeveloper = new Vector2(340f, 95f);

	private Vector2 crdDeveloperIs = new Vector2(200f, 95f);

	private Vector2 crdAlias = new Vector2(340f, 60f);

	private Vector2 crdAliasIs = new Vector2(200f, 60f);

	private Vector2 crdModeString = new Vector2(200f, 154f);

	private Vector2 crdModeStringIs = new Vector2(200f, 130f);

	private Vector2 crdGeneralCount = new Vector2(226f, 210f);

	private Vector2 crdGeneralFee = new Vector2(515f, 210f);

	private Vector2 crdGeneralBrickLabel = new Vector2(44f, 210f);

	private Vector2 crdGeneralBrickUnit = new Vector2(235f, 210f);

	private Vector2 crdGeneralRegFee = new Vector2(334f, 210f);

	private Vector2 crdGeneralPoint = new Vector2(520f, 210f);

	private Vector2 crdSpecialCount = new Vector2(226f, 237f);

	private Vector2 crdSpecialFee = new Vector2(515f, 237f);

	private Vector2 crdSpecialBrickLabel = new Vector2(44f, 237f);

	private Vector2 crdSpecialBrickUnit = new Vector2(235f, 237f);

	private Vector2 crdSpecialRegFee = new Vector2(334f, 237f);

	private Vector2 crdSpecialPoint = new Vector2(520f, 237f);

	private Vector2 crdDeleteCount = new Vector2(226f, 264f);

	private Vector2 crdDeleteFee = new Vector2(515f, 264f);

	private Vector2 crdDeleteBrickLabel = new Vector2(44f, 264f);

	private Vector2 crdDeleteBrickUnit = new Vector2(235f, 264f);

	private Vector2 crdDeleteRegFee = new Vector2(334f, 264f);

	private Vector2 crdDeletePoint = new Vector2(520f, 264f);

	private Vector2 crdTotalCount = new Vector2(226f, 291f);

	private Vector2 crdTotalFee = new Vector2(517f, 291f);

	private Vector2 crdTotalBrickLabel = new Vector2(44f, 291f);

	private Vector2 crdTotalUnit = new Vector2(235f, 291f);

	private Vector2 crdTotalRegFee = new Vector2(334f, 291f);

	private Vector2 crdTotalPoint = new Vector2(520f, 291f);

	private Vector2 crdRegMsg = new Vector2(588f, 320f);

	private Rect crdPointAlias = new Rect(185f, 71f, 7f, 7f);

	private Rect crdPointDeveloper = new Rect(185f, 105f, 7f, 7f);

	private Rect crdPointMode = new Rect(185f, 140f, 7f, 7f);

	private Rect crdPointGeneral = new Rect(30f, 219f, 7f, 7f);

	private Rect crdPointSpecial = new Rect(30f, 246f, 7f, 7f);

	private Rect crdPointDelete = new Rect(30f, 273f, 7f, 7f);

	private Rect crdPointRegister = new Rect(30f, 300f, 7f, 7f);

	private bool on;

	private bool copyRight;

	private UserMapInfo umi;

	private ushort modeMask;

	private void Start()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			umi = UserMapInfoManager.Instance.GetCur();
			if (umi != null)
			{
				copyRight = true;
			}
		}
	}

	private void Update()
	{
		on = (!DialogManager.Instance.IsModal && custom_inputs.Instance.GetButton("K_SITUATION"));
	}

	private void CheckModeMask()
	{
		modeMask = BrickManager.Instance.GetPossibleModeMask();
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && on && copyRight)
		{
			CheckModeMask();
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup(new Rect(((float)Screen.width - crdSize.x) / 2f, ((float)Screen.height - crdSize.y) / 2f, crdSize.x, crdSize.y));
			GUI.Box(new Rect(0f, 0f, crdSize.x, crdSize.y), string.Empty, "Window");
			GUI.Box(crdBasicBox, string.Empty, "BoxFadeBlue");
			Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
			if (room != null)
			{
				LabelUtil.TextOut(crdRoomTitle, room.GetString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			GUI.Box(crdThumbnailBox, string.Empty, "BoxBrickOutline");
			TextureUtil.DrawTexture(crdThumbnail, thumbnail);
			TextureUtil.DrawTexture(crdPointDeveloper, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdDeveloperIs, StringMgr.Instance.Get("DEVELOPER_IS"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeveloper, MyInfoManager.Instance.Nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdPointAlias, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdAliasIs, StringMgr.Instance.Get("MAP_NAME_IS"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdAlias, umi.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdPointMode, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdModeStringIs, StringMgr.Instance.Get("POSSIBLE_MODE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (modeMask == 0)
			{
				LabelUtil.TextOut(crdModeString, StringMgr.Instance.Get("NO_MODE_POSSIBLE"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			else
			{
				string text = Room.ModeMask2String(modeMask);
				LabelUtil.TextOut(crdModeString, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			int specialCount = 0;
			int generalCount = 0;
			int deleteCount = 0;
			UserMapInfoManager.Instance.CalcCount(ref generalCount, ref specialCount, ref deleteCount);
			float generalFee = 0f;
			float specialFee = 0f;
			float deleteFee = 0f;
			int num = specialCount + generalCount + deleteCount;
			int diff = 0;
			int totalFee = 0;
			string minMaxMessage = string.Empty;
			string pointString = string.Empty;
			bool flag = UserMapInfoManager.Instance.CalcFee(generalCount, specialCount, deleteCount, Good.BUY_HOW.GENERAL_POINT, ref totalFee, ref generalFee, ref specialFee, ref deleteFee, ref pointString, ref minMaxMessage, ref diff);
			GUI.Box(crdGeneralBox, string.Empty, "BoxFadeBlue");
			GUI.Box(crdSpecialBox, string.Empty, "BoxFadeBlue");
			GUI.Box(crdDeleteBox, string.Empty, "BoxFadeBlue");
			GUI.Box(crdFeeBox, string.Empty, "BoxFadeBlue");
			TextureUtil.DrawTexture(crdPointGeneral, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdGeneralBrickLabel, StringMgr.Instance.Get("GENERAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdGeneralCount, generalCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdGeneralBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdGeneralRegFee, StringMgr.Instance.Get("REG_FEE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdGeneralFee, Mathf.FloorToInt(generalFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdGeneralPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdPointSpecial, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdSpecialBrickLabel, StringMgr.Instance.Get("SPECIAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdSpecialCount, specialCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdSpecialBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdSpecialRegFee, StringMgr.Instance.Get("REG_FEE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdSpecialFee, Mathf.FloorToInt(specialFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdSpecialPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdPointDelete, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdDeleteBrickLabel, StringMgr.Instance.Get("DELETE_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeleteCount, deleteCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdDeleteBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeleteRegFee, StringMgr.Instance.Get("REG_FEE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeleteFee, Mathf.FloorToInt(deleteFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdDeletePoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdPointRegister, pointIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(crdTotalBrickLabel, StringMgr.Instance.Get("ALL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdTotalCount, num.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			LabelUtil.TextOut(crdTotalUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdTotalRegFee, StringMgr.Instance.Get("REG_FEE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdTotalPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (!flag)
			{
				string text2 = string.Format(StringMgr.Instance.Get("MORE_POINT_NEED"), diff);
				LabelUtil.TextOut(crdTotalFee, totalFee.ToString(), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				LabelUtil.TextOut(crdRegMsg, text2, "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			}
			else
			{
				LabelUtil.TextOut(crdTotalFee, totalFee.ToString(), "MiniLabel", new Color(0.69f, 0.83f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				LabelUtil.TextOut(crdRegMsg, StringMgr.Instance.Get("REG_MAP_POSSIBLE"), "MiniLabel", new Color(0.69f, 0.83f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			}
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}
}
