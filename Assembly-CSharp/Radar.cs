using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public float hudRadius = 120f;

	public float radarRadius = 50f;

	public Texture2D bkgnd;

	public Texture2D pin;

	public Texture2D flag;

	public Texture2D enemy;

	public Texture2D concentric;

	public Texture2D[] blastTarget;

	public Texture2D arrivalLocation_R;

	public Texture2D arrivalLocation_B;

	public Texture2D mark_tutor;

	private float dtMarkTutor;

	public Vector2 pinSize;

	public float heartBeatMax = 3f;

	private float heartBeatTime;

	private float flickering;

	private bool showHeartBeat = true;

	private CameraController cameraController;

	public Color signalMax = Color.yellow;

	public Color signalMin = Color.white;

	public Vector2 signalSize;

	public Texture2D signal;

	private Dictionary<int, RadioSenderMark> dicRadioSenders;

	private bool isVisible = true;

	private float heartBeatSub;

	public bool IsVisible => isVisible;

	private void DrawPin(bool isDead, Vector2 center, Vector2 sp)
	{
		Vector2 vector = new Vector2(sp.x, sp.y);
		sp.x -= pinSize.x / 2f;
		sp.y -= pinSize.y / 2f;
		if (!isDead)
		{
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), pin);
		}
		else
		{
			Color color = GUI.color;
			GUI.color = Color.grey;
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), pin);
			GUI.color = color;
		}
		if (Application.loadedLevelName.Contains("Tutor"))
		{
			Vector2 vector2 = pinSize + pinSize * dtMarkTutor;
			vector.x -= vector2.x / 2f;
			vector.y -= vector2.y / 2f;
			TextureUtil.DrawTexture(new Rect(center.x + vector.x, center.y + vector.y, vector2.x, vector2.y), mark_tutor, ScaleMode.ScaleToFit);
		}
	}

	private void DrawEnemy(Vector2 center, Vector2 sp)
	{
		sp.x -= (float)(enemy.width / 2);
		sp.y -= (float)(enemy.height / 2);
		TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, (float)enemy.width, (float)enemy.height), enemy);
	}

	private void DrawFlag(bool isDead, Vector2 center, Vector2 sp)
	{
		sp.x -= pinSize.x / 2f;
		sp.y -= pinSize.y / 2f;
		if (!isDead)
		{
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), flag);
		}
		else
		{
			Color color = GUI.color;
			GUI.color = Color.grey;
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), flag);
			GUI.color = color;
		}
	}

	private void DrawFlagR(bool isDead, Vector2 center, Vector2 sp)
	{
		sp.x -= pinSize.x / 2f;
		sp.y -= pinSize.y / 2f;
		if (!isDead)
		{
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), arrivalLocation_R);
		}
		else
		{
			Color color = GUI.color;
			GUI.color = Color.grey;
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), arrivalLocation_R);
			GUI.color = color;
		}
	}

	private void DrawFlagB(bool isDead, Vector2 center, Vector2 sp)
	{
		sp.x -= pinSize.x / 2f;
		sp.y -= pinSize.y / 2f;
		if (!isDead)
		{
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), arrivalLocation_B);
		}
		else
		{
			Color color = GUI.color;
			GUI.color = Color.grey;
			TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), arrivalLocation_B);
			GUI.color = color;
		}
	}

	private void DrawBomb(Texture2D blastTarget, Vector2 center, Vector2 sp)
	{
		sp.x -= pinSize.x / 2f;
		sp.y -= pinSize.y / 2f;
		TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, pinSize.x, pinSize.y), blastTarget);
	}

	private void DrawSignal(Vector2 center, Vector2 sp, float signalStrength)
	{
		sp.x -= signalSize.x / 2f;
		sp.y -= signalSize.y / 2f;
		Color color = GUI.color;
		GUI.color = Color.Lerp(signalMin, signalMax, signalStrength);
		TextureUtil.DrawTexture(new Rect(center.x + sp.x, center.y + sp.y, signalSize.x, signalSize.y), signal);
		GUI.color = color;
	}

	public bool HeartbeatDetect()
	{
		if (heartBeatTime < heartBeatMax)
		{
			return false;
		}
		heartBeatTime = 0f;
		showHeartBeat = true;
		flickering = 0f;
		heartBeatSub = 0f;
		return true;
	}

	public void Show(bool visible)
	{
		isVisible = visible;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && isVisible)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup(new Rect(0f, 0f, (float)bkgnd.width, (float)bkgnd.height));
			TextureUtil.DrawTexture(new Rect(0f, 0f, (float)bkgnd.width, (float)bkgnd.height), bkgnd);
			if (null != cameraController)
			{
				Vector2 center = new Vector2((float)(bkgnd.width / 2), (float)(bkgnd.height / 2));
				Vector3 position = cameraController.transform.position;
				Vector3 toDirection = cameraController.transform.TransformDirection(Vector3.forward);
				toDirection.y = 0f;
				toDirection = toDirection.normalized;
				Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, toDirection);
				GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
				for (int i = 0; i < array.Length; i++)
				{
					TPController component = array[i].GetComponent<TPController>();
					PlayerProperty component2 = array[i].GetComponent<PlayerProperty>();
					if (null != component2 && null != component && !component2.IsHostile() && component2.Desc.Status == 4 && !component2.Desc.IsHidePlayer)
					{
						Vector3 vector = Quaternion.Inverse(rotation) * (component2.transform.position - position);
						Vector2 a = new Vector2(vector.x, vector.z);
						Vector2 vector2 = hudRadius / radarRadius * a;
						vector2.y = -1f * vector2.y;
						if (Vector2.Distance(vector2, Vector2.zero) > hudRadius)
						{
							vector2 = hudRadius * vector2.normalized;
						}
						if (dicRadioSenders.ContainsKey(component2.Desc.Seq))
						{
							float signalStrength = dicRadioSenders[component2.Desc.Seq].GetSignalStrength();
							if (signalStrength > 0f)
							{
								DrawSignal(center, vector2, signalStrength);
							}
						}
						DrawPin(component.IsDead, center, vector2);
					}
				}
				Trigger[] enabledScriptables = BrickManager.Instance.GetEnabledScriptables();
				for (int j = 0; j < enabledScriptables.Length; j++)
				{
					Vector3 vector3 = Quaternion.Inverse(rotation) * (enabledScriptables[j].transform.position - position);
					Vector2 a2 = new Vector2(vector3.x, vector3.z);
					Vector2 vector4 = hudRadius / radarRadius * a2;
					vector4.y = -1f * vector4.y;
					if (Vector2.Distance(vector4, Vector2.zero) > hudRadius)
					{
						vector4 = hudRadius * vector4.normalized;
					}
					DrawPin(isDead: false, center, vector4);
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.CAPTURE_THE_FLAG && BrickManager.Instance.userMap != null)
				{
					SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER, 0);
					if (spawner != null)
					{
						Vector3 vector5 = Quaternion.Inverse(rotation) * (spawner.position - position);
						Vector2 a3 = new Vector2(vector5.x, vector5.z);
						Vector2 vector6 = hudRadius / radarRadius * a3;
						vector6.y = -1f * vector6.y;
						if (Vector2.Distance(vector6, Vector2.zero) > hudRadius)
						{
							vector6 = hudRadius * vector6.normalized;
						}
						DrawFlagR(isDead: false, center, vector6);
					}
					spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER, 0);
					if (spawner != null)
					{
						Vector3 vector7 = Quaternion.Inverse(rotation) * (spawner.position - position);
						Vector2 a4 = new Vector2(vector7.x, vector7.z);
						Vector2 vector8 = hudRadius / radarRadius * a4;
						vector8.y = -1f * vector8.y;
						if (Vector2.Distance(vector8, Vector2.zero) > hudRadius)
						{
							vector8 = hudRadius * vector8.normalized;
						}
						DrawFlagB(isDead: false, center, vector8);
					}
					if (BrickManManager.Instance.haveFlagSeq < 0)
					{
						Vector3 vector9 = Quaternion.Inverse(rotation) * (BrickManManager.Instance.vFlag - position);
						Vector2 a5 = new Vector2(vector9.x, vector9.z);
						Vector2 vector10 = hudRadius / radarRadius * a5;
						vector10.y = -1f * vector10.y;
						if (Vector2.Distance(vector10, Vector2.zero) > hudRadius)
						{
							vector10 = hudRadius * vector10.normalized;
						}
						DrawFlag(isDead: false, center, vector10);
					}
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
				{
					for (int k = 0; k < 2; k++)
					{
						if (BrickManager.Instance.userMap != null)
						{
							SpawnerDesc spawner2 = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BOMB_SPAWNER, k);
							if (spawner2 != null)
							{
								Vector3 vector11 = Quaternion.Inverse(rotation) * (spawner2.position - position);
								Vector2 a6 = new Vector2(vector11.x, vector11.z);
								Vector2 vector12 = hudRadius / radarRadius * a6;
								vector12.y = -1f * vector12.y;
								if (Vector2.Distance(vector12, Vector2.zero) > hudRadius)
								{
									vector12 = hudRadius * vector12.normalized;
								}
								DrawBomb(blastTarget[k], center, vector12);
							}
						}
					}
				}
				if (heartBeatTime < heartBeatMax && showHeartBeat)
				{
					GameObject[] array2 = BrickManManager.Instance.ToGameObjectArray();
					for (int l = 0; l < array2.Length; l++)
					{
						TPController component3 = array2[l].GetComponent<TPController>();
						PlayerProperty component4 = array2[l].GetComponent<PlayerProperty>();
						if (null != component4 && null != component3 && component4.IsHostile() && !component3.IsDead && component4.Desc.Status == 4 && !component4.Desc.IsHidePlayer)
						{
							Vector3 vector13 = Quaternion.Inverse(rotation) * (component4.transform.position - position);
							Vector2 a7 = new Vector2(vector13.x, vector13.z);
							Vector2 vector14 = hudRadius / radarRadius * a7;
							vector14.y = -1f * vector14.y;
							if (Vector2.Distance(vector14, Vector2.zero) > hudRadius)
							{
								vector14 = hudRadius * vector14.normalized;
							}
							DrawEnemy(center, vector14);
						}
					}
				}
			}
			DrawConcentric();
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}

	private void DrawConcentric()
	{
		if (heartBeatTime > 0f && heartBeatTime < heartBeatMax)
		{
			float num = heartBeatTime - heartBeatSub;
			float num2 = num / 0.7f;
			if (num2 >= 1f)
			{
				heartBeatSub += 1f;
			}
			num2 = Mathf.Min(num2, 1f);
			float num3 = (float)(bkgnd.width / 2);
			float num4 = (float)(bkgnd.height / 2);
			float num5 = Mathf.Max(num3, num4);
			float num6 = Mathf.Lerp(0f, num5, num2);
			if (num6 < num5)
			{
				TextureUtil.DrawTexture(new Rect(num3 - num6, num4 - num6, num6 * 2f, num6 * 2f), concentric, ScaleMode.StretchToFill, alphaBlend: true);
			}
		}
	}

	private void Start()
	{
		heartBeatTime = heartBeatMax;
		flickering = 0f;
		showHeartBeat = true;
		dicRadioSenders = new Dictionary<int, RadioSenderMark>();
		GameObject gameObject = GameObject.Find("Main Camera");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find mainCamera for radar");
		}
		else
		{
			cameraController = gameObject.GetComponent<CameraController>();
			if (null == cameraController)
			{
				Debug.LogError("Fail to get CameraController for radar");
			}
		}
	}

	private void Update()
	{
		heartBeatTime += Time.deltaTime;
		flickering += Time.deltaTime;
		dtMarkTutor += Time.deltaTime;
		if (dtMarkTutor > 0.5f)
		{
			dtMarkTutor = 0f;
		}
		if (flickering > 0.5f)
		{
			showHeartBeat = !showHeartBeat;
			flickering = 0f;
		}
		foreach (KeyValuePair<int, RadioSenderMark> dicRadioSender in dicRadioSenders)
		{
			dicRadioSender.Value.Update();
		}
	}

	private void OnRadioMsg(RadioSignal signal)
	{
		if (dicRadioSenders.ContainsKey(signal.Sender))
		{
			dicRadioSenders[signal.Sender].Reset();
		}
		else
		{
			dicRadioSenders.Add(signal.Sender, new RadioSenderMark(signal.Sender));
		}
	}
}
