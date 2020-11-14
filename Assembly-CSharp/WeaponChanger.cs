using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
	private GUIDepth.LAYER guiDepth = GUIDepth.LAYER.MENU;

	public Texture2D bombtex;

	private Texture2D[] weapons;

	public float scale = 1.2f;

	public float offset = 10f;

	public float showTimeLimit = 2f;

	private int[] slot2Key;

	private Weapon.TYPE[] key2Slot;

	private float deltaTime = float.PositiveInfinity;

	public void Initialize(GameObject[] usables)
	{
		slot2Key = new int[5]
		{
			2,
			1,
			0,
			3,
			4
		};
		key2Slot = new Weapon.TYPE[5]
		{
			Weapon.TYPE.MAIN,
			Weapon.TYPE.AUX,
			Weapon.TYPE.MELEE,
			Weapon.TYPE.PROJECTILE,
			Weapon.TYPE.MODE_SPECIFIC
		};
		deltaTime = float.PositiveInfinity;
		weapons = new Texture2D[usables.Length];
		for (int i = 0; i < usables.Length; i++)
		{
			weapons[i] = null;
		}
		for (int j = 0; j < usables.Length; j++)
		{
			if (!(null == usables[j]))
			{
				Weapon component = usables[j].GetComponent<Weapon>();
				if (!(null == component) && component.slot != Weapon.TYPE.COUNT)
				{
					WeaponFunction component2 = usables[j].GetComponent<WeaponFunction>();
					if (!(null == component2))
					{
						if (component2.weaponBy != Weapon.BY.COMPOSER)
						{
							weapons[slot2Key[(int)component.slot]] = TItemManager.Instance.GetWeaponBy((int)component2.weaponBy);
						}
						else
						{
							BrickComposer component3 = usables[j].GetComponent<BrickComposer>();
							if (null != component3)
							{
								weapons[slot2Key[(int)component.slot]] = component3.icon;
							}
						}
					}
				}
			}
		}
	}

	private bool NeedSpecificSlot()
	{
		return RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND || (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION && MyInfoManager.Instance.AmIBlasting());
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && !(deltaTime > showTimeLimit))
		{
			EquipCoordinator component = GetComponent<EquipCoordinator>();
			if (!(null == component) && slot2Key != null && component.CurrentWeapon < 5 && component.CurrentWeapon >= 0)
			{
				int currentWeapon = component.CurrentWeapon;
				int num = slot2Key[currentWeapon];
				GUISkin skin = GUI.skin;
				GUI.skin = GUISkinFinder.Instance.GetGUISkin();
				GUI.depth = (int)guiDepth;
				GUI.enabled = !DialogManager.Instance.IsModal;
				float num2 = 0f;
				for (int i = 0; i < weapons.Length; i++)
				{
					if ((key2Slot[i] != Weapon.TYPE.MODE_SPECIFIC || NeedSpecificSlot()) && null != weapons[i])
					{
						if (i > 0)
						{
							num2 += offset;
						}
						num2 += (float)weapons[i].width / 1.5f;
					}
				}
				float num3 = (float)Screen.width - num2;
				Color color = GUI.color;
				for (int j = 0; j < weapons.Length; j++)
				{
					if ((key2Slot[j] != Weapon.TYPE.MODE_SPECIFIC || NeedSpecificSlot()) && null != weapons[j])
					{
						float num4 = (float)Screen.height - (float)weapons[j].height / 1.5f - 110f;
						Color color2 = new Color(0.5f, 0.5f, 0.5f, 0.5f);
						Vector2 a = new Vector2((float)weapons[j].width / 1.5f, (float)weapons[j].height / 1.5f);
						if (j == num)
						{
							color2 = Color.white;
							a *= scale;
						}
						GUI.color = color2;
						TextureUtil.DrawTexture(new Rect(num3 - (a.x - (float)weapons[j].width / 1.5f) / 2f, num4 - (a.y - (float)weapons[j].height / 1.5f), a.x, a.y), weapons[j]);
						num3 += (float)weapons[j].width / 1.5f + offset;
					}
				}
				GUI.color = color;
				GUI.enabled = true;
				GUI.skin = skin;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (deltaTime < float.PositiveInfinity)
		{
			deltaTime += Time.deltaTime;
		}
	}

	public void Swap()
	{
		deltaTime = 0f;
	}
}
