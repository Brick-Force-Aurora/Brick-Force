using UnityEngine;

public class FriendlyPlayerNames : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.WORLD_TAG;

	private Camera cam;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (null != gameObject)
		{
			cam = gameObject.GetComponent<Camera>();
		}
	}

	private void OnGUI()
	{
		if (!GlobalVars.Instance.hideOurForcesNickname && MyInfoManager.Instance.isGuiOn && !MyInfoManager.Instance.IsSpectator)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 position = array[i].transform.position;
				position.y += 2f;
				PlayerProperty component = array[i].GetComponent<PlayerProperty>();
				if (null != component && !component.IsHostile() && !component.Desc.IsHidePlayer)
				{
					Vector3 vector = cam.WorldToViewportPoint(position);
					if (vector.z > 0f && 0f < vector.x && vector.x < 1f && 0f < vector.y && vector.y < 1f)
					{
						Vector3 vector2 = cam.WorldToScreenPoint(position);
						LabelUtil.TextOut(new Vector2(vector2.x, (float)Screen.height - vector2.y - 20f), component.Desc.Nickname, "Label", Color.green, Color.black, TextAnchor.LowerCenter);
					}
				}
			}
			GUI.enabled = true;
		}
	}

	private void Update()
	{
	}
}
