using System.Collections.Generic;
using UnityEngine;

public class ProjectileAlert : MonoBehaviour
{
	public Texture2D dirImage;

	private CameraController cameraController;

	private Dictionary<int, PTT> mine;

	private void Start()
	{
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
	}

	public void RemoveMine(int index)
	{
		if (mine != null && mine.ContainsKey(index))
		{
			mine.Remove(index);
		}
	}

	public void TrackMine(int index, Weapon.BY weapon, Vector3 pos, float range)
	{
		if (mine == null)
		{
			mine = new Dictionary<int, PTT>();
		}
		if (!mine.ContainsKey(index))
		{
			mine.Add(index, new PTT(weapon, pos, range));
		}
		else
		{
			mine[index].pos = pos;
			mine[index].range = range;
		}
	}

	private void Draw(Weapon.BY weapon, Vector3 pos, float range)
	{
		Texture2D weaponBy = TItemManager.Instance.GetWeaponBy((int)weapon);
		if (!(null == weaponBy))
		{
			Vector3 position = cameraController.transform.position;
			float num = Mathf.Abs(Vector3.Distance(position, pos));
			if (!(num > range))
			{
				Vector3 from = cameraController.transform.TransformDirection(Vector3.forward);
				from.y = 0f;
				from = from.normalized;
				Vector3 from2 = cameraController.transform.TransformDirection(Vector3.right);
				from2.y = 0f;
				from2 = from2.normalized;
				Vector3 from3 = cameraController.transform.TransformDirection(Vector3.left);
				from3.y = 0f;
				from3 = from3.normalized;
				Vector3 to = pos - position;
				to.y = 0f;
				to = to.normalized;
				float num2 = Vector3.Angle(from, to);
				if (Vector3.Angle(from2, to) >= Vector3.Angle(from3, to))
				{
					num2 = 180f + (180f - num2);
				}
				float f = num2 * 0.0174532924f;
				Vector2 vector = new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2));
				float num3 = (float)Screen.height / 2f * 0.4f;
				float num4 = num3 * Mathf.Sin(f);
				float num5 = (0f - num3) * Mathf.Cos(f);
				float num6 = (float)weaponBy.width * 0.8f;
				float num7 = (float)weaponBy.height * 0.8f;
				Rect position2 = new Rect(vector.x + num4 - num6 / 2f, vector.y + num5 - num7 / 2f, num6, num7);
				TextureUtil.DrawTexture(position2, weaponBy, ScaleMode.StretchToFill);
				Vector2 vector2 = new Vector2(position2.x + position2.width / 2f, position2.y + position2.height / 2f);
				Rect position3 = new Rect(vector2.x - (float)(dirImage.width / 2), vector2.y - (float)(dirImage.height / 2), (float)dirImage.width, (float)dirImage.height);
				Matrix4x4 matrix = GUI.matrix;
				GUIUtility.RotateAroundPivot(num2, vector2);
				TextureUtil.DrawTexture(position3, dirImage, ScaleMode.StretchToFill);
				GUI.matrix = matrix;
				string text = num.ToString("0.#") + "m";
				LabelUtil.TextOut(vector2, text, "Label", Color.yellow, Color.black, TextAnchor.MiddleCenter);
			}
		}
	}

	private void OnGUI()
	{
		if (mine != null)
		{
			foreach (KeyValuePair<int, PTT> item in mine)
			{
				Draw(item.Value.weapon, item.Value.pos, item.Value.range);
			}
		}
		GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty component = array[i].GetComponent<PlayerProperty>();
			if (null != component && component.IsHostile())
			{
				GdgtGrenade[] componentsInChildren = array[i].GetComponentsInChildren<GdgtGrenade>(includeInactive: true);
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					Weapon.BY weaponBY = componentsInChildren[0].GetWeaponBY();
					ProjectileWrap[] array2 = componentsInChildren[0].ToProjectileWrap();
					if (weaponBY != 0 && weaponBY != Weapon.BY.BLACKHOLE && array2 != null)
					{
						for (int j = 0; j < array2.Length; j++)
						{
							Draw(weaponBY, array2[j].targetPos, array2[j].range);
						}
					}
				}
				GdgtFlashBang[] componentsInChildren2 = array[i].GetComponentsInChildren<GdgtFlashBang>(includeInactive: true);
				if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
				{
					Weapon.BY weaponBY2 = componentsInChildren2[0].GetWeaponBY();
					ProjectileWrap[] array3 = componentsInChildren2[0].ToProjectileWrap();
					if (weaponBY2 != 0 && weaponBY2 != Weapon.BY.BLACKHOLE && array3 != null)
					{
						for (int k = 0; k < array3.Length; k++)
						{
							Draw(weaponBY2, array3[k].targetPos, array3[k].range);
						}
					}
				}
				GdgtXmasBomb[] componentsInChildren3 = array[i].GetComponentsInChildren<GdgtXmasBomb>(includeInactive: true);
				if (componentsInChildren3 != null && componentsInChildren3.Length > 0)
				{
					Weapon.BY weaponBY3 = componentsInChildren3[0].GetWeaponBY();
					ProjectileWrap[] array4 = componentsInChildren3[0].ToProjectileWrap();
					if (weaponBY3 != 0 && weaponBY3 != Weapon.BY.BLACKHOLE && array4 != null)
					{
						for (int l = 0; l < array4.Length; l++)
						{
							Draw(weaponBY3, array4[l].targetPos, array4[l].range);
						}
					}
				}
			}
		}
	}
}
