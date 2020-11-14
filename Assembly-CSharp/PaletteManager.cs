using UnityEngine;

public class PaletteManager : MonoBehaviour
{
	private const int BUNGEE_MODE_BRICK = 8;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.MENU;

	public string[] brickTabKey;

	public Vector2 crdPaletteSize = new Vector2(656f, 80f);

	public Vector2 crdPalette = new Vector2(64f, 64f);

	public Vector2 crdBrickLT = new Vector2(3f, 3f);

	public float offset = 1f;

	public Vector2 crdShortcut = new Vector2(12f, 12f);

	public Vector2 crdSiloArea = new Vector2(666f, 361f);

	public Rect crdSiloFrame = new Rect(0f, 25f, 666f, 336f);

	public Rect crdSiloTab = new Rect(28f, 0f, 600f, 25f);

	public Vector2 crdSilo = new Vector2(64f, 64f);

	public Vector2 crdSiloOffset = new Vector2(1f, 2f);

	public Rect crdSiloPosition = new Rect(4f, 29f, 660f, 328f);

	public TooltipBrick tooltip;

	private string[] brickTab;

	private Vector2 spGeneral = Vector2.zero;

	private Vector2 spColor = Vector2.zero;

	private Vector2 spDeco = Vector2.zero;

	private Vector2 spFunc = Vector2.zero;

	private Vector2 pospal1 = Vector2.zero;

	private Brick[] palette;

	private int currentPalette;

	private Texture[] paletteIcons;

	private int currentBrick = -1;

	private Texture dragAndDrop;

	private Brick dragAndDropBrick;

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private Brick[] general;

	private Texture[] generalIcon;

	private Brick[] colorbox;

	private Texture[] colorboxIcon;

	private Brick[] accessory;

	private Texture[] accessoryIcon;

	private Brick[] functional;

	private Texture[] functionalIcon;

	private int currentSilo;

	private bool menuOn;

	private static PaletteManager instance;

	public bool MenuOn => menuOn;

	public static PaletteManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (Object.FindObjectOfType(typeof(PaletteManager)) as PaletteManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the PaletteManager Instance");
				}
			}
			return instance;
		}
	}

	public void Switch(bool on)
	{
		menuOn = on;
		if (!menuOn)
		{
			SavePalette();
		}
		else
		{
			RefreshSilo();
		}
	}

	public void ToggleSwitch()
	{
		menuOn = !menuOn;
		if (!menuOn)
		{
			SavePalette();
		}
		else
		{
			RefreshSilo();
		}
	}

	private void SavePalette()
	{
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			int[] pal = new int[10];
			GetPaletteIndex(ref pal);
			CSNetManager.Instance.Sock.SendCS_SAVE_PALETTE_REQ(pal[0], pal[1], pal[2], pal[3], pal[4], pal[5], pal[6], pal[7], pal[8], pal[9]);
		}
	}

	public void PaletteSet(int pal, int id, int silo)
	{
		for (int i = 0; i < paletteIcons.Length; i++)
		{
			palette[i] = null;
			paletteIcons[i] = null;
		}
		if (id >= 0)
		{
			palette[pal] = BrickManager.Instance.GetBrick(id);
			if (palette[pal] != null)
			{
				paletteIcons[pal] = palette[pal].Icon;
			}
		}
		currentSilo = silo;
	}

	private void GetPaletteIndex(ref int[] pal)
	{
		for (int i = 0; i < 10; i++)
		{
			if (palette[i] == null)
			{
				pal[i] = -1;
			}
			else
			{
				pal[i] = palette[i].GetIndex();
			}
		}
	}

	protected int _CheckShortCut()
	{
		string[] array = new string[10]
		{
			"K_BRICK1",
			"K_BRICK2",
			"K_BRICK3",
			"K_BRICK4",
			"K_BRICK5",
			"K_BRICK6",
			"K_BRICK7",
			"K_BRICK8",
			"K_BRICK9",
			"K_BRICK0"
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (custom_inputs.Instance.GetButtonDown(array[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public void CheckShortCut()
	{
		if (!DialogManager.Instance.IsModal)
		{
			int num = _CheckShortCut();
			if (num < 0 && RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
			{
				num = GlobalVars.Instance.GetWheelKey(Input.GetAxis("Mouse ScrollWheel"));
			}
			if (num >= 0)
			{
				GlobalVars.Instance.SetWheelKey(num);
				currentPalette = num;
			}
		}
	}

	public void CheckDragAndDrop()
	{
		if (null != dragAndDrop && custom_inputs.Instance.GetButtonDown("K_FIRE2"))
		{
			dragAndDrop = null;
			dragAndDropBrick = null;
			TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
			if (component != null)
			{
				component.setActive(TUTO_INPUT.M_L);
				component.setClick(TUTO_INPUT.M_L);
				float x = ((float)Screen.width - crdSiloArea.x) / 2f + 4f;
				float y = ((float)Screen.height - crdSiloArea.y) / 2f + 29f;
				component.setChangePos(set: true, new Vector2(x, y));
				component.setBlick(set: true);
				component.setBlickPos(new Rect(x, y, 64f, 64f));
			}
		}
	}

	public void SetCurrentPalette(Brick brick)
	{
		paletteIcons[currentPalette] = brick.Icon;
		palette[currentPalette] = brick;
	}

	private void DoPalette()
	{
		string[] array = new string[10]
		{
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"i",
			"j"
		};
		string[] array2 = new string[10]
		{
			"K_BRICK1",
			"K_BRICK2",
			"K_BRICK3",
			"K_BRICK4",
			"K_BRICK5",
			"K_BRICK6",
			"K_BRICK7",
			"K_BRICK8",
			"K_BRICK9",
			"K_BRICK0"
		};
		Rect position = new Rect(((float)Screen.width - crdPaletteSize.x) / 2f, 0f, crdPaletteSize.x, crdPaletteSize.y);
		GUI.BeginGroup(position);
		GUI.Box(new Rect(0f, 0f, crdPaletteSize.x, crdPaletteSize.y), string.Empty, "BoxBrickBar");
		for (int i = 0; i < 10; i++)
		{
			Color color = GUI.color;
			if (palette[i] != null && !palette[i].IsEnable(RoomManager.Instance.CurrentRoomType))
			{
				GUI.color = Color.red;
			}
			Rect rect = new Rect(crdBrickLT.x + (float)i * (crdPalette.x + offset), crdBrickLT.y, crdPalette.x, crdPalette.y);
			GUIContent gUIContent = null;
			gUIContent = ((palette[i] != null) ? new GUIContent(paletteIcons[i], array[i] + palette[i].GetIndex().ToString()) : new GUIContent(string.Empty, string.Empty));
			if (i == 0)
			{
				pospal1.x = position.x + rect.x;
				pospal1.y = position.y + rect.y;
			}
			if (GlobalVars.Instance.MyButton2(rect, gUIContent, "ButtonBrick"))
			{
				currentPalette = i;
				GlobalVars.Instance.SetWheelKey(i);
				if (dragAndDrop != null && dragAndDropBrick != null && currentBrick >= 0)
				{
					paletteIcons[i] = dragAndDrop;
					palette[i] = dragAndDropBrick;
					dragAndDrop = null;
					dragAndDropBrick = null;
					currentBrick = -1;
					TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
					if (component != null)
					{
						component.setActive(TUTO_INPUT.M_L);
						component.setClick(TUTO_INPUT.M_L);
						component.setChangePos(set: false, Vector2.zero);
						GlobalVars.Instance.eventGravity = true;
						Switch(on: false);
					}
				}
			}
			GUI.color = color;
			if (tooltip.TargetBrick != null && palette[i] != null && tooltip.TargetBrick.GetIndex() == palette[i].GetIndex() && lastTooltip.StartsWith(array[i]))
			{
				ltTooltip = new Vector2(position.x + rect.x + rect.width, position.y + rect.y);
			}
			if (currentPalette == i)
			{
				GUI.Box(rect, string.Empty, "BoxBrickSel");
			}
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
			{
				LabelUtil.TextOut(new Vector2(rect.x + crdShortcut.x, rect.y + crdShortcut.y), custom_inputs.Instance.GetKeyCodeName(array2[i]), "TinyLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				if (palette[i] != null && palette[i].maxInstancePerMap > 0)
				{
					int num = BrickManager.Instance.CountLimitedBrick(palette[i].GetIndex());
					int maxInstancePerMap = palette[i].maxInstancePerMap;
					string text = num.ToString() + "/" + maxInstancePerMap.ToString();
					Color clrText = (num < maxInstancePerMap) ? Color.white : Color.red;
					LabelUtil.TextOut(new Vector2(rect.x, rect.y + rect.height), text, "TinyLabel", clrText, TextAnchor.LowerLeft);
				}
			}
		}
		GUI.EndGroup();
	}

	private void DoSilo()
	{
		for (int i = 0; i < brickTabKey.Length; i++)
		{
			brickTab[i] = StringMgr.Instance.Get(brickTabKey[i]);
		}
		Rect position = new Rect(((float)Screen.width - crdSiloArea.x) / 2f, ((float)Screen.height - crdSiloArea.y) / 2f, crdSiloArea.x, crdSiloArea.y);
		GUI.BeginGroup(position);
		GUI.Box(crdSiloFrame, string.Empty, "BoxBrickBar");
		currentSilo = GUI.SelectionGrid(crdSiloTab, currentSilo, brickTab, brickTab.Length, "BoxBrickTab");
		switch (currentSilo)
		{
		case 0:
		{
			int num8 = Mathf.FloorToInt((float)(generalIcon.Length / 10));
			if (generalIcon.Length % 10 > 0)
			{
				num8++;
			}
			spGeneral = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num8 * crdSilo.y + (float)(num8 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spGeneral);
			int num9 = 0;
			for (int num10 = 0; num10 < num8; num10++)
			{
				for (int num11 = 0; num11 < 10; num11++)
				{
					if (num9 >= generalIcon.Length)
					{
						break;
					}
					Rect rc4 = new Rect((float)num11 * (crdSilo.x + crdSiloOffset.x), (float)num10 * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					bool enabled4 = GUI.enabled;
					GUI.enabled = (general[num9].ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(general[num9].ticket) >= 0);
					if (GlobalVars.Instance.MyButton(rc4, generalIcon[num9], new GUIContent(string.Empty, "s" + general[num9].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = num9;
						dragAndDrop = generalIcon[num9];
						dragAndDropBrick = general[num9];
					}
					GUI.enabled = enabled4;
					if (tooltip.TargetBrick != null && tooltip.TargetBrick.GetIndex() == general[num9].GetIndex() && lastTooltip.StartsWith("s"))
					{
						ltTooltip = new Vector2(position.x + crdSiloPosition.x + rc4.x + rc4.width, position.y + crdSiloPosition.y + rc4.y - spGeneral.y);
					}
					num9++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 1:
		{
			int num5 = Mathf.FloorToInt((float)(colorbox.Length / 10));
			if (colorbox.Length % 10 > 0)
			{
				num5++;
			}
			spColor = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num5 * crdSilo.y + (float)(num5 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spColor);
			int num6 = 0;
			for (int n = 0; n < num5; n++)
			{
				for (int num7 = 0; num7 < 10; num7++)
				{
					if (num6 >= colorbox.Length)
					{
						break;
					}
					Rect rc3 = new Rect((float)num7 * (crdSilo.x + crdSiloOffset.x), (float)n * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					bool enabled3 = GUI.enabled;
					GUI.enabled = (colorbox[num6].ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(colorbox[num6].ticket) >= 0);
					if (GlobalVars.Instance.MyButton(rc3, colorboxIcon[num6], new GUIContent(string.Empty, "s" + colorbox[num6].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = num6;
						dragAndDrop = colorboxIcon[num6];
						dragAndDropBrick = colorbox[num6];
					}
					GUI.enabled = enabled3;
					if (tooltip.TargetBrick != null && tooltip.TargetBrick.GetIndex() == colorbox[num6].GetIndex() && lastTooltip.StartsWith("s"))
					{
						ltTooltip = new Vector2(position.x + crdSiloPosition.x + rc3.x + rc3.width, position.y + crdSiloPosition.y + rc3.y - spColor.y);
					}
					num6++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 2:
		{
			int num3 = Mathf.FloorToInt((float)(accessory.Length / 10));
			if (accessory.Length % 10 > 0)
			{
				num3++;
			}
			spDeco = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num3 * crdSilo.y + (float)(num3 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spDeco);
			int num4 = 0;
			for (int l = 0; l < num3; l++)
			{
				for (int m = 0; m < 10; m++)
				{
					if (num4 >= accessory.Length)
					{
						break;
					}
					Rect rc2 = new Rect((float)m * (crdSilo.x + crdSiloOffset.x), (float)l * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					bool enabled2 = GUI.enabled;
					GUI.enabled = (accessory[num4].ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(accessory[num4].ticket) >= 0);
					if (GlobalVars.Instance.MyButton(rc2, accessoryIcon[num4], new GUIContent(string.Empty, "s" + accessory[num4].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = num4;
						dragAndDrop = accessoryIcon[num4];
						dragAndDropBrick = accessory[num4];
					}
					GUI.enabled = enabled2;
					if (tooltip.TargetBrick != null && tooltip.TargetBrick.GetIndex() == accessory[num4].GetIndex() && lastTooltip.StartsWith("s"))
					{
						ltTooltip = new Vector2(position.x + crdSiloPosition.x + rc2.x + rc2.width, position.y + crdSiloPosition.y + rc2.y - spDeco.y);
					}
					num4++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 3:
		{
			int num = Mathf.FloorToInt((float)(functionalIcon.Length / 10));
			if (functionalIcon.Length % 10 > 0)
			{
				num++;
			}
			spFunc = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num * crdSilo.y + (float)(num - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spFunc);
			int num2 = 0;
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < 10; k++)
				{
					if (num2 >= functionalIcon.Length)
					{
						break;
					}
					Rect rc = new Rect((float)k * (crdSilo.x + crdSiloOffset.x), (float)j * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					bool enabled = GUI.enabled;
					if (!Application.loadedLevelName.Contains("Tutor"))
					{
						GUI.enabled = (functional[num2].ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(functional[num2].ticket) >= 0);
					}
					if (GlobalVars.Instance.MyButton(rc, functionalIcon[num2], new GUIContent(string.Empty, "s" + functional[num2].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = num2;
						dragAndDrop = functionalIcon[num2];
						dragAndDropBrick = functional[num2];
						TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
						if (component != null)
						{
							float x = pospal1.x;
							float y = pospal1.y;
							component.setChangePos(set: true, new Vector2(x, y));
							component.setBlick(set: true);
							component.setBlickPos(new Rect(x, y, 64f, 64f));
						}
					}
					if (!Application.loadedLevelName.Contains("Tutor"))
					{
						GUI.enabled = enabled;
					}
					if (tooltip.TargetBrick != null && tooltip.TargetBrick.GetIndex() == functional[num2].GetIndex() && lastTooltip.StartsWith("s"))
					{
						ltTooltip = GUIUtility.GUIToScreenPoint(new Vector2(rc.x + rc.width, rc.y));
					}
					num2++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		}
		GUI.EndGroup();
		if (dragAndDrop != null)
		{
			Vector3 mousePosition = Input.mousePosition;
			float x2 = mousePosition.x;
			float num12 = (float)Screen.height;
			Vector3 mousePosition2 = Input.mousePosition;
			Vector2 vector = new Vector2(x2, num12 - mousePosition2.y);
			GUI.Box(new Rect(vector.x, vector.y, 64f, 64f), dragAndDrop, "BoxBrickSel");
		}
	}

	private void ResetDragNdrop()
	{
		dragAndDrop = null;
		currentBrick = -1;
	}

	public void Use()
	{
		if (BrickManager.Instance.IsLoaded && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			DoPalette();
			if (!menuOn)
			{
				ResetDragNdrop();
			}
			else
			{
				DoSilo();
			}
			if (Event.current.type == EventType.Repaint)
			{
				if (lastTooltip != GUI.tooltip)
				{
					char[] trimChars = new char[11]
					{
						's',
						'a',
						'b',
						'c',
						'd',
						'e',
						'f',
						'g',
						'h',
						'i',
						'j'
					};
					string text = GUI.tooltip;
					text = text.TrimStart(trimChars);
					int num = -1;
					try
					{
						num = int.Parse(text);
					}
					catch
					{
					}
					if (num < 0)
					{
						tooltip.TargetBrick = null;
					}
					else
					{
						tooltip.TargetBrick = BrickManager.Instance.GetBrick(num);
					}
				}
				if (dragAndDrop == null && dragAndDropBrick == null && lastTooltip.Length > 0 && ltTooltip != Vector2.zero && !DialogManager.Instance.IsModal)
				{
					tooltip.SetCoord(ltTooltip);
					GUI.Window(1107, tooltip.ClientRect, ShowTooltip, string.Empty, "TooltipWindow");
				}
				lastTooltip = GUI.tooltip;
			}
			GUI.skin = skin;
		}
	}

	private void ShowTooltip(int id)
	{
		tooltip.DoDialog();
	}

	public Brick GetCurrentBrick()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			return BrickManager.Instance.GetBrick(8);
		}
		return palette[currentPalette];
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	public void Setup(int pal0, int pal1, int pal2, int pal3, int pal4, int pal5, int pal6, int pal7, int pal8, int pal9)
	{
		palette[0] = BrickManager.Instance.GetBrick(pal0);
		if (palette[0] != null)
		{
			paletteIcons[0] = palette[0].Icon;
		}
		palette[1] = BrickManager.Instance.GetBrick(pal1);
		if (palette[1] != null)
		{
			paletteIcons[1] = palette[1].Icon;
		}
		palette[2] = BrickManager.Instance.GetBrick(pal2);
		if (palette[2] != null)
		{
			paletteIcons[2] = palette[2].Icon;
		}
		palette[3] = BrickManager.Instance.GetBrick(pal3);
		if (palette[3] != null)
		{
			paletteIcons[3] = palette[3].Icon;
		}
		palette[4] = BrickManager.Instance.GetBrick(pal4);
		if (palette[4] != null)
		{
			paletteIcons[4] = palette[4].Icon;
		}
		palette[5] = BrickManager.Instance.GetBrick(pal5);
		if (palette[5] != null)
		{
			paletteIcons[5] = palette[5].Icon;
		}
		palette[6] = BrickManager.Instance.GetBrick(pal6);
		if (palette[6] != null)
		{
			paletteIcons[6] = palette[6].Icon;
		}
		palette[7] = BrickManager.Instance.GetBrick(pal7);
		if (palette[7] != null)
		{
			paletteIcons[7] = palette[7].Icon;
		}
		palette[8] = BrickManager.Instance.GetBrick(pal8);
		if (palette[8] != null)
		{
			paletteIcons[8] = palette[8].Icon;
		}
		palette[9] = BrickManager.Instance.GetBrick(pal9);
		if (palette[9] != null)
		{
			paletteIcons[9] = palette[9].Icon;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		brickTab = new string[brickTabKey.Length];
		palette = new Brick[10];
		paletteIcons = new Texture[palette.Length];
		for (int i = 0; i < paletteIcons.Length; i++)
		{
			palette[i] = null;
			paletteIcons[i] = null;
		}
		RefreshSilo();
	}

	private void RefreshSilo()
	{
		general = BrickManager.Instance.ToBrickArray(Brick.CATEGORY.GENERAL);
		generalIcon = BrickManager.Instance.ToBrickIconArray(Brick.CATEGORY.GENERAL);
		colorbox = BrickManager.Instance.ToBrickArray(Brick.CATEGORY.COLORBOX);
		colorboxIcon = BrickManager.Instance.ToBrickIconArray(Brick.CATEGORY.COLORBOX);
		accessory = BrickManager.Instance.ToBrickArray(Brick.CATEGORY.ACCESSORY);
		accessoryIcon = BrickManager.Instance.ToBrickIconArray(Brick.CATEGORY.ACCESSORY);
		functional = BrickManager.Instance.ToBrickArray(Brick.CATEGORY.FUNCTIONAL);
		functionalIcon = BrickManager.Instance.ToBrickIconArray(Brick.CATEGORY.FUNCTIONAL);
	}
}
