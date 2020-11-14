using UnityEngine;

public static class GeniusPacketSend
{
	public enum SEND_PACKET_LEVEL
	{
		NONE = -1,
		ONLY_SOUND,
		EXCEPT_EFFECT,
		ONLY_BRICK_SOUND,
		ALL
	}

	private static float inputMinDistance = 12f;

	private static float inputMaxDistance = 9999f;

	public static int preudpcnt;

	public static int preudpsize;

	public static int nextudpcnt;

	public static int nextudpsize;

	private static Camera camera;

	public static Vector3 SetVectorLength(Vector3 vector, float size)
	{
		Vector3 a = Vector3.Normalize(vector);
		return a *= size;
	}

	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{
		intersection = Vector3.zero;
		float num = Vector3.Dot(planePoint - linePoint, planeNormal);
		float num2 = Vector3.Dot(lineVec, planeNormal);
		if (num2 != 0f)
		{
			float size = num / num2;
			Vector3 b = SetVectorLength(lineVec, size);
			intersection = linePoint + b;
			return true;
		}
		return false;
	}

	public static SEND_PACKET_LEVEL check3rdPersonSendOrNoSend(GameObject player, Vector3 shootpos, Vector3 hitpoint, Vector3 shootdir, Vector3 hitnormal, int brickSeq, float range, bool possibleCan)
	{
		if (null == player)
		{
			return SEND_PACKET_LEVEL.NONE;
		}
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		GameObject gameObject = null;
		BrickProperty brickProperty = null;
		Ray ray = default(Ray);
		bool flag = true;
		bool flag2 = true;
		float num = 0f;
		Plane plane = default(Plane);
		shootpos.y += 1.2f;
		TPController component = player.GetComponent<TPController>();
		if ((bool)component)
		{
			float num2 = Vector3.Distance(shootpos, component.transform.position);
			bool flag3 = false;
			if (num2 < inputMinDistance)
			{
				flag3 = true;
			}
			bool flag4 = false;
			if (num2 > inputMaxDistance)
			{
				flag4 = true;
			}
			Vector3 position = component.transform.position;
			Vector3 position2 = component.transform.position;
			position2.y += 10f;
			Vector3 c = component.transform.position + component.lookAt() * 30f;
			plane.Set3Points(position, position2, c);
			flag = true;
			Vector3 rhs = shootpos - component.transform.position;
			rhs.Normalize();
			num = Vector3.Dot(component.lookAt(), rhs);
			if (num < 0.2f)
			{
				flag = false;
			}
			flag2 = true;
			rhs = hitpoint - component.transform.position;
			rhs.Normalize();
			num = Vector3.Dot(component.lookAt(), rhs);
			if (num <= 0.2f)
			{
				flag2 = false;
			}
			if (flag3)
			{
				return SEND_PACKET_LEVEL.ALL;
			}
			if (flag4)
			{
				return SEND_PACKET_LEVEL.NONE;
			}
			Vector3 position3 = component.transform.position;
			position3.y += 1.2f;
			ray.origin = position3;
			ray.direction = Vector3.Normalize(hitpoint - ray.origin);
			bool flag5 = false;
			if (Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask))
			{
				GameObject gameObject2 = hitInfo.transform.gameObject;
				if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
				{
					BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
					if (allComponents.Length > 0)
					{
						brickProperty = allComponents[0];
					}
				}
				else
				{
					gameObject = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
					if (null != gameObject)
					{
						brickProperty = gameObject.GetComponent<BrickProperty>();
					}
				}
				if (null != brickProperty && brickSeq != brickProperty.Seq)
				{
					flag5 = true;
				}
			}
			ray.direction = Vector3.Normalize(shootpos - ray.origin);
			bool flag6 = false;
			if (Physics.Raycast(ray, out hitInfo, num2 + 1f, layerMask))
			{
				float num3 = Vector3.Distance(hitInfo.point, component.transform.position);
				if (num2 > num3)
				{
					flag6 = true;
				}
			}
			if (!flag && !flag2)
			{
				return SEND_PACKET_LEVEL.NONE;
			}
			if (!flag5 && !flag6)
			{
				return SEND_PACKET_LEVEL.ALL;
			}
			if (flag5 || !flag2)
			{
				if (!flag6)
				{
					return SEND_PACKET_LEVEL.EXCEPT_EFFECT;
				}
				if (num2 < 20f)
				{
					return SEND_PACKET_LEVEL.ONLY_SOUND;
				}
				return SEND_PACKET_LEVEL.NONE;
			}
			if (!possibleCan && flag5 && flag6)
			{
				float distanceToPoint = plane.GetDistanceToPoint(shootpos);
				float distanceToPoint2 = plane.GetDistanceToPoint(hitpoint);
				if (distanceToPoint < distanceToPoint2)
				{
					return SEND_PACKET_LEVEL.NONE;
				}
				if (distanceToPoint * distanceToPoint2 < 0f)
				{
					Vector3 intersection = Vector3.zero;
					if (LinePlaneIntersection(out intersection, shootpos, shootdir, plane.normal, position3))
					{
						ray.direction = component.lookAt();
						if (Physics.Raycast(ray, out hitInfo, 30f, layerMask))
						{
							return SEND_PACKET_LEVEL.EXCEPT_EFFECT;
						}
					}
				}
			}
			return SEND_PACKET_LEVEL.ALL;
		}
		return SEND_PACKET_LEVEL.ALL;
	}

	public static SEND_PACKET_LEVEL checkHITMAN(GameObject hitman, GameObject thirdPlayer, Vector3 myPos)
	{
		if (hitman == null || thirdPlayer == null)
		{
			return SEND_PACKET_LEVEL.NONE;
		}
		TPController component = thirdPlayer.GetComponent<TPController>();
		if ((bool)component)
		{
			myPos.y += 1.2f;
			float num = Vector3.Distance(myPos, component.transform.position);
			bool flag = false;
			if (num < inputMinDistance)
			{
				flag = true;
			}
			bool flag2 = false;
			if (num > inputMaxDistance)
			{
				flag2 = true;
			}
			if (flag)
			{
				return SEND_PACKET_LEVEL.ALL;
			}
			if (flag2)
			{
				return SEND_PACKET_LEVEL.NONE;
			}
			Camera[] componentsInChildren = thirdPlayer.GetComponentsInChildren<Camera>();
			if (componentsInChildren.Length > 0)
			{
				camera = componentsInChildren[0];
				camera.fieldOfView = 75f;
				camera.nearClipPlane = 0.3f;
				camera.farClipPlane = 1000f;
			}
			camera.enabled = true;
			Vector3 position = thirdPlayer.transform.position;
			float x = position.x;
			Vector3 position2 = thirdPlayer.transform.position;
			float y = position2.y + 1.2f;
			Vector3 position3 = thirdPlayer.transform.position;
			Vector3 position4 = new Vector3(x, y, position3.z);
			camera.transform.position = position4;
			camera.transform.rotation = thirdPlayer.transform.rotation;
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			if (GeometryUtility.TestPlanesAABB(planes, hitman.collider.bounds))
			{
				camera.enabled = false;
				return SEND_PACKET_LEVEL.ALL;
			}
			camera.enabled = false;
			return SEND_PACKET_LEVEL.EXCEPT_EFFECT;
		}
		return SEND_PACKET_LEVEL.ALL;
	}

	public static SEND_PACKET_LEVEL checkSendDir(GameObject player, Vector3 myPos, Bounds bounds)
	{
		if (null == player)
		{
			return SEND_PACKET_LEVEL.NONE;
		}
		GameObject gameObject = null;
		BrickProperty brickProperty = null;
		TPController component = player.GetComponent<TPController>();
		if ((bool)component)
		{
			Vector3 vector = new Vector3(myPos.x, myPos.y + 1.2f, myPos.z);
			Vector3 position = component.transform.position;
			float x = position.x;
			Vector3 position2 = component.transform.position;
			float y = position2.y + 1.2f;
			Vector3 position3 = component.transform.position;
			Vector3 vector2 = new Vector3(x, y, position3.z);
			float num = Vector3.Distance(myPos, component.transform.position);
			Camera[] componentsInChildren = player.GetComponentsInChildren<Camera>();
			if (componentsInChildren.Length > 0)
			{
				camera = componentsInChildren[0];
				camera.fieldOfView = 75f;
				camera.nearClipPlane = 0.3f;
				camera.farClipPlane = 1000f;
			}
			camera.transform.position = vector2;
			camera.transform.rotation = component.transform.rotation;
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			if (GeometryUtility.TestPlanesAABB(planes, bounds))
			{
				int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
				Ray ray = default(Ray);
				ray.origin = vector2;
				ray.direction = Vector3.Normalize(vector - ray.origin);
				if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, layerMask))
				{
					float num2 = Vector3.Distance(hitInfo.point, vector2);
					if (num > num2)
					{
						GameObject gameObject2 = hitInfo.transform.gameObject;
						if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
						{
							BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
							if (allComponents.Length > 0)
							{
								brickProperty = allComponents[0];
							}
						}
						else
						{
							gameObject = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
							if (null != gameObject)
							{
								brickProperty = gameObject.GetComponent<BrickProperty>();
							}
						}
						if (null != brickProperty && IsTransparent(brickProperty.Index) && IsFaceToMeSameBrick(vector, vector2, num, brickProperty.Seq))
						{
							camera.enabled = false;
							return SEND_PACKET_LEVEL.ALL;
						}
						camera.enabled = false;
						return SEND_PACKET_LEVEL.NONE;
					}
				}
				camera.enabled = false;
				return SEND_PACKET_LEVEL.ALL;
			}
			camera.enabled = false;
			return SEND_PACKET_LEVEL.NONE;
		}
		return SEND_PACKET_LEVEL.ALL;
	}

	public static SEND_PACKET_LEVEL checkSendMove(GameObject player, Vector3 myPos, Bounds bounds)
	{
		if (null == player)
		{
			return SEND_PACKET_LEVEL.NONE;
		}
		TPController component = player.GetComponent<TPController>();
		if ((bool)component)
		{
			if (IsMyCamIn(player.collider.bounds))
			{
				return SEND_PACKET_LEVEL.ALL;
			}
			Vector3 position = component.transform.position;
			float x = position.x;
			Vector3 position2 = component.transform.position;
			float y = position2.y + 1.2f;
			Vector3 position3 = component.transform.position;
			Vector3 position4 = new Vector3(x, y, position3.z);
			Camera[] componentsInChildren = player.GetComponentsInChildren<Camera>();
			if (componentsInChildren.Length > 0)
			{
				camera = componentsInChildren[0];
				camera.fieldOfView = 60f;
				camera.nearClipPlane = 0.3f;
				camera.farClipPlane = 1000f;
			}
			camera.transform.position = position4;
			camera.transform.rotation = component.transform.rotation;
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			if (GeometryUtility.TestPlanesAABB(planes, bounds))
			{
				camera.enabled = false;
				return SEND_PACKET_LEVEL.ALL;
			}
			camera.enabled = false;
			return SEND_PACKET_LEVEL.NONE;
		}
		return SEND_PACKET_LEVEL.ALL;
	}

	private static bool IsTransparent(int id)
	{
		int[] array = new int[2]
		{
			14,
			169
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (id == array[i])
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsFaceToMeSameBrick(Vector3 myEye, Vector3 thirdEye, float d, int seq)
	{
		GameObject gameObject = null;
		BrickProperty brickProperty = null;
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Ray ray = default(Ray);
		ray.origin = thirdEye;
		ray.direction = Vector3.Normalize(myEye - ray.origin);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, layerMask))
		{
			float num = Vector3.Distance(hitInfo.point, thirdEye);
			if (d > num)
			{
				GameObject gameObject2 = hitInfo.transform.gameObject;
				if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
				{
					BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
					if (allComponents.Length > 0)
					{
						brickProperty = allComponents[0];
					}
				}
				else
				{
					gameObject = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
					if (null != gameObject)
					{
						brickProperty = gameObject.GetComponent<BrickProperty>();
					}
				}
				if (null != brickProperty && brickProperty.Seq == seq)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsMyCamIn(Bounds bounds)
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (gameObject != null)
		{
			Camera component = gameObject.GetComponent<Camera>();
			if (component != null)
			{
				Plane[] planes = GeometryUtility.CalculateFrustumPlanes(component);
				if (GeometryUtility.TestPlanesAABB(planes, bounds))
				{
					return true;
				}
			}
		}
		return false;
	}
}
