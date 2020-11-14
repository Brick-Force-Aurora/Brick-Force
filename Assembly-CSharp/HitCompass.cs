using System.Collections.Generic;
using UnityEngine;

public class HitCompass : MonoBehaviour
{
	private float lifeTime = 3f;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Color compassColor;

	public Color compassColorChild = new Color(1f, 1f, 0f, 1f);

	public Color zeroColor;

	public Texture2D[] hitCompass;

	private CameraController cameraController;

	private Dictionary<int, DirAttacker> attackers;

	private void Start()
	{
		attackers = new Dictionary<int, DirAttacker>();
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

	private void OnDirectionalHit(int attacker)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ESCAPE)
		{
			GameObject y = BrickManManager.Instance.Get(attacker);
			if (null != y)
			{
				if (attackers.ContainsKey(attacker))
				{
					attackers[attacker].Reset();
				}
				else if (!MyInfoManager.Instance.IsBelow12())
				{
					attackers.Add(attacker, new DirAttacker(attacker, hitCompass[0], compassColor, zeroColor, lifeTime, cameraController));
				}
				else
				{
					attackers.Add(attacker, new DirAttacker(attacker, hitCompass[0], compassColorChild, zeroColor, lifeTime, cameraController));
				}
			}
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			Color color = GUI.color;
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, DirAttacker> attacker in attackers)
			{
				if (!attacker.Value.Draw())
				{
					list.Add(attacker.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				attackers.Remove(list[i]);
			}
			GUI.enabled = true;
			GUI.color = color;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, DirAttacker> attacker in attackers)
		{
			if (!attacker.Value.Update())
			{
				list.Add(attacker.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			attackers.Remove(list[i]);
		}
	}
}
