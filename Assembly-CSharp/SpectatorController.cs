using UnityEngine;

public class SpectatorController : MonoBehaviour
{
	public float targetHeight = 1.7f;

	public float distance = 15f;

	private float maxDistance = 6f;

	public float minDistance = 0.6f;

	public float xSpeed = 270f;

	public float ySpeed = 120f;

	public int yMinLimit = -20;

	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float rotationDampening = 3f;

	public float zoomDampening = 5f;

	private float x;

	private float y;

	private float currentDistance;

	private float desiredDistance;

	private float correctedDistance;

	private LayerMask collisionLayers = -1;

	private int randomSpawnerTicketWhenTargetIsNull;

	private void Start()
	{
		randomSpawnerTicketWhenTargetIsNull = Random.Range(0, 16);
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		currentDistance = distance;
		desiredDistance = distance;
		correctedDistance = distance;
		collisionLayers.value = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Me")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Brain")) | (1 << LayerMask.NameToLayer("CoreBrain")));
		collisionLayers.value = ~collisionLayers.value;
	}

	private void LateUpdate()
	{
		if (MyInfoManager.Instance.IsSpectator)
		{
			Transform transform = null;
			SpectatorSwitch component = GetComponent<SpectatorSwitch>();
			if (null != component && component.Target != null)
			{
				transform = component.Target.transform;
			}
			if (null == transform || MyInfoManager.Instance.Status != 4)
			{
				transform = BrickManager.Instance.GetSpawnerTransform(MyInfoManager.Instance.GetRoundingSpawnerType(), randomSpawnerTicketWhenTargetIsNull);
				if (null == transform)
				{
					return;
				}
			}
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			}
			y = ClampAngle(y, (float)yMinLimit, (float)yMaxLimit);
			Quaternion rotation = Quaternion.Euler(y, x, 0f);
			desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * (float)zoomRate * Mathf.Abs(desiredDistance);
			desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
			correctedDistance = desiredDistance;
			Vector3 vector = transform.transform.position - (rotation * Vector3.forward * desiredDistance + new Vector3(0f, 0f - targetHeight, 0f));
			Vector3 position = transform.transform.position;
			float num = position.x;
			Vector3 position2 = transform.transform.position;
			float num2 = position2.y + targetHeight;
			Vector3 position3 = transform.transform.position;
			Vector3 vector2 = new Vector3(num, num2, position3.z);
			Vector3 normalized = (vector - vector2).normalized;
			float num3 = Vector3.Distance(vector2, vector);
			Ray ray = new Ray(vector2, normalized);
			bool flag = false;
			if (Physics.SphereCast(ray, 0.1f, out RaycastHit hitInfo, num3, collisionLayers.value))
			{
				vector = hitInfo.point;
				correctedDistance = Vector3.Distance(vector2, vector);
				flag = true;
			}
			currentDistance = ((flag && !(correctedDistance > currentDistance)) ? correctedDistance : Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening));
			vector = transform.transform.position - (rotation * Vector3.forward * currentDistance + new Vector3(0f, 0f - targetHeight, 0f));
			base.transform.rotation = rotation;
			base.transform.position = vector;
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
