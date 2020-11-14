using UnityEngine;

public class Connecting : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.LOADING;

	public float offset = 24f;

	private bool show;

	public bool Show => show;

	private void Start()
	{
		show = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && BrickManager.Instance.IsLoaded && show)
		{
			bool flag = true;
			BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Status == 2 || array[i].Status == 3)
				{
					flag = false;
				}
			}
			if (flag)
			{
				show = false;
			}
			else
			{
				bool flag2 = false;
				if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
				{
					flag2 = true;
				}
				GUISkin skin = GUI.skin;
				GUI.skin = GUISkinFinder.Instance.GetGUISkin();
				GUI.depth = (int)guiDepth;
				GUI.enabled = !DialogManager.Instance.IsModal;
				Peer[] array2 = P2PManager.Instance.ToArray();
				if (array2 == null || array2.Length <= 0)
				{
					show = false;
				}
				else
				{
					Vector2 vector = LabelUtil.CalcLength("MiniLabel", MyInfoManager.Instance.Nickname);
					float x = vector.x;
					for (int j = 0; j < array2.Length; j++)
					{
						BrickManDesc desc = BrickManManager.Instance.GetDesc(array2[j].Seq);
						if (desc != null)
						{
							Vector2 vector2 = LabelUtil.CalcLength("MiniLabel", desc.Nickname);
							if (x < vector2.x)
							{
								x = vector2.x;
							}
						}
					}
					Vector2 vector3 = new Vector2(x + offset, x + 2f * offset);
					Vector2 vector4 = new Vector2(vector3.x + (float)(array2.Length + 2) * offset, vector3.y + (float)(array2.Length + 2) * offset);
					GUI.BeginGroup(new Rect(((float)Screen.width - vector4.x) / 2f, ((float)Screen.height - vector4.y) / 2f, vector4.x, vector4.y));
					GUI.Box(new Rect(0f, 0f, vector4.x, vector4.y), string.Empty);
					float num = vector3.x;
					float y = vector3.y;
					LabelUtil.TextOut(new Vector2(num, y), MyInfoManager.Instance.Nickname, "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(240, 153, 32), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					for (int num2 = array2.Length - 1; num2 >= 0; num2--)
					{
						GUI.Toggle(new Rect(num, y, offset - 4f, offset - 4f), P2PManager.Instance.IsConnected(MyInfoManager.Instance.Seq, array2[num2].Seq), string.Empty);
						num += offset;
					}
					num = vector3.x;
					y += offset;
					for (int k = 0; k < array2.Length - 1; k++)
					{
						BrickManDesc desc2 = BrickManManager.Instance.GetDesc(array2[k].Seq);
						if (desc2 != null)
						{
							LabelUtil.TextOut(new Vector2(num, y), desc2.Nickname, "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(240, 153, 32), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
							for (int num3 = array2.Length - 1; num3 > k; num3--)
							{
								GUI.Toggle(new Rect(num, y, offset - 4f, offset - 4f), P2PManager.Instance.IsConnected(array2[k].Seq, array2[num3].Seq), string.Empty);
								num += offset;
							}
							num = vector3.x;
							y += offset;
						}
					}
					num = vector3.x;
					y = vector3.y;
					GUIUtility.RotateAroundPivot(270f, new Vector2(num, y));
					for (int num4 = array2.Length - 1; num4 >= 0; num4--)
					{
						BrickManDesc desc3 = BrickManManager.Instance.GetDesc(array2[num4].Seq);
						if (desc3 != null)
						{
							if (flag2 && GlobalVars.Instance.MyButton(new Rect(num + 5f, y + 5f, 14f, 14f), string.Empty, "X"))
							{
								CSNetManager.Instance.Sock.SendCS_KICK_REQ(array2[num4].Seq);
							}
							Vector2 vector5 = LabelUtil.CalcLength("MiniLabel", desc3.Nickname);
							GUI.Label(new Rect(num + offset, y, vector5.x, vector5.y), desc3.Nickname, "MiniLabel");
							y += offset;
						}
					}
					GUI.EndGroup();
				}
				GUI.enabled = true;
				GUI.skin = skin;
			}
		}
	}
}
