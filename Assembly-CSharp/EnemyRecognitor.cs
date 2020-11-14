using UnityEngine;

public class EnemyRecognitor : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private BrickManDesc enemy;

	private Camera cam;

	private void Start()
	{
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
			}
		}
	}

	private bool IsVisible(Vector3 pos, int seq)
	{
		float num = Vector3.Distance(cam.transform.position, pos);
		if (num > 15f)
		{
			return false;
		}
		int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("BndWall"));
		Vector3 normalized = (pos - cam.transform.position).normalized;
		Ray ray = new Ray(cam.transform.position, normalized);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BoxMan"))
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(hitInfo.transform, includeInactive: false);
			if (allComponents.Length == 1 && allComponents[0].Desc.Seq == seq)
			{
				return true;
			}
		}
		return false;
	}

	private void OnGUI()
	{
		if (!GlobalVars.Instance.hideEnemyForcesNickname && MyInfoManager.Instance.isGuiOn && !MyInfoManager.Instance.IsSpectator)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 position = array[i].transform.position;
				position.y += 2f;
				Vector3 position2 = array[i].transform.position;
				position2.y += 1f;
				PlayerProperty component = array[i].GetComponent<PlayerProperty>();
				if (null != component && component.IsHostile() && !component.Desc.IsHidePlayer)
				{
					bool flag = false;
					if (enemy != null && enemy.Seq == component.Desc.Seq)
					{
						flag = true;
					}
					if (!flag && IsVisible(position2, component.Desc.Seq))
					{
						flag = true;
					}
					if (flag)
					{
						Vector3 vector = cam.WorldToViewportPoint(position);
						if (vector.z > 0f && 0f < vector.x && vector.x < 1f && 0f < vector.y && vector.y < 1f)
						{
							Vector3 sp = cam.WorldToScreenPoint(position);
							LabelUtil.TextOut(sp, component.Desc.Nickname, "Label", Color.red, Color.black, TextAnchor.LowerCenter);
						}
					}
				}
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		enemy = null;
		if (!(null == cam))
		{
			int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
			Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BoxMan"))
			{
				PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(hitInfo.transform, includeInactive: false);
				if (allComponents.Length == 1 && allComponents[0].Desc.IsHostile())
				{
					enemy = allComponents[0].Desc;
				}
			}
		}
	}
}
