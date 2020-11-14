using System.Collections.Generic;
using UnityEngine;

public class WoundFx : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.SCREEN_FX;

	public Texture2D screenFx;

	public Texture2D screenFxChild;

	public Color ToScreenFxClr;

	public Color FromScreenFxClr;

	public Color ToScreenFxClrChild;

	public Color FromScreenFxClrChild;

	public Color ToBloodClr;

	public Color FromBloodClr;

	public Texture2D[] bloodMarks;

	public Texture2D[] piercedWounds;

	public AudioClip heartbeatSound;

	public AudioClip growling;

	private Color screenFxClr = Color.white;

	private Queue<BloodMark> bloodMarkQ;

	private LocalController localController;

	private float deltaTime;

	private Color zombieFxClr;

	public Color ToZombieFxClr;

	public Color FromZombieFxClr;

	private Rect rcScreen = new Rect(0f, 0f, 0f, 0f);

	private float zombieDelta;

	private float zombieDeltaGrowling;

	private float zombieDeltaGrowlingMax = 3f;

	private void Start()
	{
		deltaTime = 0f;
		localController = GetComponent<LocalController>();
		bloodMarkQ = new Queue<BloodMark>();
		if (!MyInfoManager.Instance.IsBelow12())
		{
			screenFxClr = ToScreenFxClr;
		}
		else
		{
			screenFxClr = ToScreenFxClrChild;
		}
		zombieFxClr = ToZombieFxClr;
		zombieDelta = 0f;
		ResetZombieGrawling();
	}

	public void ClearScreen()
	{
		if (!MyInfoManager.Instance.IsBelow12())
		{
			screenFxClr = ToScreenFxClr;
		}
		else
		{
			screenFxClr = ToScreenFxClrChild;
		}
		while (bloodMarkQ.Count > 0)
		{
			bloodMarkQ.Dequeue();
		}
	}

	private void OnRespawn(Quaternion rotation)
	{
		ClearScreen();
	}

	private void Growling()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component)
		{
			component.clip = growling;
			component.loop = false;
			component.Play();
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.depth = (int)guiDepth;
		GUI.enabled = !DialogManager.Instance.IsModal;
		Color color = GUI.color;
		GUI.color = screenFxClr;
		rcScreen.width = (float)Screen.width;
		rcScreen.height = (float)Screen.height;
		if (!MyInfoManager.Instance.IsBelow12())
		{
			TextureUtil.DrawTexture(rcScreen, screenFx, ScaleMode.StretchToFill, alphaBlend: true);
		}
		else
		{
			TextureUtil.DrawTexture(rcScreen, screenFxChild, ScaleMode.StretchToFill, alphaBlend: true);
		}
		GUI.color = zombieFxClr;
		TextureUtil.DrawTexture(rcScreen, screenFx, ScaleMode.StretchToFill, alphaBlend: true);
		GUI.color = color;
		foreach (BloodMark item in bloodMarkQ)
		{
			item.Draw();
		}
		GUI.enabled = true;
	}

	private void ApplyScreenFx()
	{
		if (null != localController && !localController.IsDead)
		{
			if (!MyInfoManager.Instance.IsBelow12())
			{
				screenFxClr = Color.Lerp(screenFxClr, ToScreenFxClr, Time.deltaTime);
			}
			else
			{
				screenFxClr = Color.Lerp(screenFxClr, ToScreenFxClrChild, Time.deltaTime);
			}
			foreach (BloodMark item in bloodMarkQ)
			{
				item.Update();
			}
			while (bloodMarkQ.Count > 0)
			{
				BloodMark bloodMark = bloodMarkQ.Peek();
				if (bloodMark.IsAlive)
				{
					break;
				}
				bloodMarkQ.Dequeue();
			}
		}
	}

	private void ResetZombieGrawling()
	{
		zombieDeltaGrowling = 0f;
		zombieDeltaGrowlingMax = Random.Range(6f, 9f);
	}

	private void ApplyScreenFxForZombie()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && null != localController && !localController.IsDead)
		{
			if (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
			{
				zombieDelta += Time.deltaTime;
				if (zombieDelta > 0.8f)
				{
					zombieDelta = 0f;
					zombieFxClr = FromZombieFxClr;
				}
				zombieDeltaGrowling += Time.deltaTime;
				if (zombieDeltaGrowling > zombieDeltaGrowlingMax)
				{
					ResetZombieGrawling();
					Growling();
				}
			}
			zombieFxClr = Color.Lerp(zombieFxClr, ToZombieFxClr, Time.deltaTime);
		}
	}

	private void OnHit(int weaponBy)
	{
		if (weaponBy == 6)
		{
			if (!MyInfoManager.Instance.IsBelow12() && piercedWounds.Length > 0)
			{
				int num = Random.Range(0, piercedWounds.Length);
				bloodMarkQ.Enqueue(new BloodMark(piercedWounds[num], FromBloodClr, ToBloodClr, Random.Range(1f, 1.5f)));
			}
		}
		else if (!MyInfoManager.Instance.IsBelow12() && bloodMarks.Length > 0)
		{
			int num2 = Random.Range(0, bloodMarks.Length);
			bloodMarkQ.Enqueue(new BloodMark(bloodMarks[num2], FromBloodClr, ToBloodClr, Random.Range(1f, 1.5f)));
		}
		if (!MyInfoManager.Instance.IsBelow12())
		{
			screenFxClr = FromScreenFxClr;
		}
		else
		{
			screenFxClr = FromScreenFxClrChild;
		}
	}

	private void ApplyHeartbeat()
	{
		if (null != localController && !localController.IsDead && NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, localController.HitPoint) < 20)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 0.8f)
			{
				deltaTime = 0f;
				GetComponent<AudioSource>().PlayOneShot(heartbeatSound);
				if (!MyInfoManager.Instance.IsBelow12())
				{
					screenFxClr = FromScreenFxClr;
				}
				else
				{
					screenFxClr = FromScreenFxClrChild;
				}
				BroadcastMessage("OnHeartbeat");
			}
		}
	}

	private void Update()
	{
		ApplyHeartbeat();
		ApplyScreenFx();
		ApplyScreenFxForZombie();
	}
}
