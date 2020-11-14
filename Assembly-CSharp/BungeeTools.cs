using System;
using UnityEngine;

[Serializable]
public class BungeeTools : MonoBehaviour
{
	public const int RESERVE_SLOT = 0;

	public const int ITEM_SLOT = 1;

	public const int ITEM_USE = 0;

	public const int ITEM_CHANGE = 1;

	private string[] input = new string[2]
	{
		"K_BUNGEE1",
		"K_BUNGEE2"
	};

	private GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private static BungeeTools _instance;

	public BungeeTool[] tools;

	private Vector2 crdHotkey1 = new Vector2(189f, 87f);

	private Vector2 crdHotkey2 = new Vector2(95f, 67f);

	private LocalController localController;

	private BattleChat battleChat;

	public UIImage itemBackground;

	public UIImage keyTextBackground;

	public AudioClip sndItemSwap;

	public static BungeeTools Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(BungeeTools)) as BungeeTools);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BungeeTools Instance");
				}
			}
			return _instance;
		}
	}

	private void VerifyLocalController()
	{
		if (null == localController)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				localController = gameObject.GetComponent<LocalController>();
			}
		}
	}

	private void Start()
	{
		battleChat = GetComponent<BattleChat>();
		VerifyLocalController();
	}

	public void StartCoolTime()
	{
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null)
			{
				tools[i].StartCoolTime();
			}
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
			itemBackground.Draw();
			for (int i = 0; i < tools.Length; i++)
			{
				tools[i].itemIcon.Draw();
			}
			keyTextBackground.Draw();
			for (int j = 0; j < tools.Length; j++)
			{
				tools[j].uiEffect.Draw();
			}
			LabelUtil.TextOut(new Vector2(crdHotkey1.x, crdHotkey1.y), custom_inputs.Instance.GetKeyCodeName(input[0]), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdHotkey2.x, crdHotkey2.y), custom_inputs.Instance.GetKeyCodeName(input[1]), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		VerifyLocalController();
		for (int i = 0; i < tools.Length; i++)
		{
			if (tools[i] != null)
			{
				tools[i].Update();
			}
		}
		if (tools[1].UseAble() && !battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(input[0]) && localController != null && localController.CanControl())
		{
			tools[1].Use();
			if (tools[0].UseAble())
			{
				tools[1].AddActiveItem(tools[0].GetActiveItem());
				tools[0].ResetSlot();
			}
		}
		if (tools[1].UseAble() && tools[0].UseAble() && !battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(input[1]) && localController != null && localController.CanControl())
		{
			ActiveItemData activeItem = tools[1].GetActiveItem();
			tools[1].ResetSlot();
			tools[1].AddActiveItem(tools[0].GetActiveItem());
			tools[0].ResetSlot();
			tools[0].AddActiveItem(activeItem);
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.PlayOneShot(sndItemSwap);
				}
			}
		}
	}

	public bool AddActiveItem(ActiveItemData item)
	{
		if (tools[1].AddActiveItem(item))
		{
			return true;
		}
		if (tools[0].AddActiveItem(item))
		{
			return true;
		}
		return false;
	}

	public void ResetAllSlot()
	{
		for (int i = 0; i < tools.Length; i++)
		{
			tools[i].ResetSlot();
		}
	}
}
