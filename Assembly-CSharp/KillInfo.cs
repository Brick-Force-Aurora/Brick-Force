using UnityEngine;

public class KillInfo
{
	private const float lifeTime = 5f;

	private const float alphaTime = 4f;

	private string killer;

	private string victim;

	private int victimSequence;

	private int killerSequence;

	private Texture2D weapon;

	private Texture2D headshot;

	private float deltaTime;

	private float alpha = 1f;

	private float dragY;

	private int weaponBy;

	public string Killer => killer;

	public string Victim => victim;

	public int VictimSequence => victimSequence;

	public int KillerSequence => killerSequence;

	public Texture2D WeaponTex => weapon;

	public Texture2D HeadShot => headshot;

	public bool IsAlive => deltaTime < 5f;

	public bool IsAlpha => deltaTime > 4f;

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = value;
		}
	}

	public float DragY
	{
		get
		{
			return dragY;
		}
		set
		{
			dragY = value;
		}
	}

	public int WeaponBy
	{
		get
		{
			return weaponBy;
		}
		set
		{
			weaponBy = value;
		}
	}

	public KillInfo(string killerNickname, string victimNickname, Texture2D weaponImage, Texture2D headshotImage, int killerSeq, int victimSeq, int _weaponBy)
	{
		killer = killerNickname;
		victim = victimNickname;
		weapon = weaponImage;
		headshot = headshotImage;
		killerSequence = killerSeq;
		victimSequence = victimSeq;
		deltaTime = 0f;
		weaponBy = _weaponBy;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
	}
}
