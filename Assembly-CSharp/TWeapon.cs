using UnityEngine;

public class TWeapon : TItem
{
	public enum CATEGORY
	{
		HEAVY,
		ASSAULT,
		SNIPER,
		SUB_MACHINE,
		HAND_GUN,
		MELEE,
		SPECIAL
	}

    private GameObject prefab;      // Main prefab (game object) of the weapon
    private GameObject prefab11;    // Alternative prefab of the weapon
    public string bone;             // Bone associated with the weapon
    public int cat;                 // Weapon category index
    public int durabilityMax;       // Maximum durability of the weapon
    public bool IsTwoHands;         // Flag indicating if the weapon requires both hands

    private static string[] categories = new string[7]
	{
		"heavy",
		"assault",
		"sniper",
		"submachine",
		"handgun",
		"melee",
		"special"
	};

	public TWeapon(string itemCode, string itemName, string itemBone, GameObject itemPrefab, GameObject itemPrefab11, Texture2D itemIcon, Texture2D itemIcon11, int ct, int ck, int cc, bool itemTakeoffable, SLOT itemSlot, string itemComment, TBuff tb, bool itemDiscomposable, string itemBpBackCode, int itemDurabilityMax, int upCat, bool basic, int _season, string _grp1, string _grp2, string _grp3, bool twohands, int starRate)
		: base(itemCode, TYPE.WEAPON, itemName, itemIcon, ct, ck, itemTakeoffable, itemSlot, itemComment, tb, itemDiscomposable, itemBpBackCode, 0, (UPGRADE_CATEGORY)upCat, basic, starRate)
	{
		prefab = itemPrefab;
		prefab11 = itemPrefab11;
		icon11 = itemIcon11;
		bone = itemBone;
		cat = cc;
		season = _season;
		grp1 = _grp1;
		grp2 = _grp2;
		grp3 = _grp3;
		IsTwoHands = twohands;
		durabilityMax = itemDurabilityMax;
	}

    // Returns the current prefab of the weapon
    // Check developer options and age restrictions to determine which prefab to return
    public GameObject CurPrefab()
	{
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12 && prefab11 != null)
		{
			return prefab11;
		}
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Age < 12 && prefab11 != null)
		{
			return prefab11;
		}
		return prefab;
	}

    // Converts a string category to an integer index
    public static int String2WeaponCategory(string category)
	{
		for (int i = 0; i < categories.Length; i++)
		{
			if (categories[i] == category)
			{
				return i;
			}
		}
		Debug.LogError("Error, Trying to find category with invalid category " + category);
		return -1;
	}

    // Gets the weapon type based on the slot value
    public Weapon.TYPE GetWeaponType()
	{
		return (Weapon.TYPE)(slot - 2);
	}

    // Calculates and returns the discount ratio based on the provided level
    public static int GetDiscountRatio(int lv)
	{
		if (lv >= 5)
		{
			return 15;
		}
		if (lv >= 4)
		{
			return 8;
		}
		if (lv >= 3)
		{
			return 5;
		}
		if (lv >= 2)
		{
			return 3;
		}
		return 0;
	}

    // Calculates and returns the discount ratio based on the weapon category and player's XP values
    public int GetDiscountRatio()
	{
		int result = 0;
		switch (cat)
		{
		case 0:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.HEAVY, MyInfoManager.Instance.Heavy);
			break;
		case 1:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.ASSAULT, MyInfoManager.Instance.Assault);
			break;
		case 2:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.SNIPER, MyInfoManager.Instance.Sniper);
			break;
		case 3:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.SUB_MACHINE, MyInfoManager.Instance.SubMachine);
			break;
		case 4:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.HAND_GUN, MyInfoManager.Instance.HandGun);
			break;
		case 5:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.MELEE, MyInfoManager.Instance.Melee);
			break;
		case 6:
			result = XpManager.Instance.GetDiscountRatio(CATEGORY.SPECIAL, MyInfoManager.Instance.Special);
			break;
		}
		return result;
	}
}
