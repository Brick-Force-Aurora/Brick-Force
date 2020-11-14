using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector3 eyePosition;

	private Vector3 eyePositionOnSquatted;

	private float xSpeed = 270f;

	private float ySpeed = 120f;

	private float scopeCameraFactor = 1f;

	private float cameraSpeedFactor = 1f;

	private float cameraSpeedFactorAccel = 0.02f;

	public static float maxCamSpeed = 3.5f;

	public static float minCamSpeed = 0.1f;

	private float cfShowTime = 1f;

	private float yMinLimit = -85f;

	private float yMaxLimit = 85f;

	private float x;

	private float y;

	private float recoilYaw;

	private float sumOfRecoilYaw;

	private float recoilPitch;

	private float sumOfRecoilPitch;

	private bool alive = true;

	private Quaternion ghostFrom;

	private Quaternion ghostTo;

	private float ghostTime;

	public float ghostCameraSpeed = 2f;

	private Camera fpCam;

	private CannonController cannon;

	private float cannonTryingTime;

	private Vector3 orgPosition;

	private Quaternion orgRotation;

	private Transform me;

	private float scopeFov;

	private bool speedUpEffect;

	private float speedUpFov;

	public float speedUpFovMax = -10f;

	public float speedUpFovZoomIn = -20f;

	public float speedUpFovZoomOut = -20f;

	public Vector3 EyePosition => eyePosition;

	public Vector3 EyePositionOnSquatted => eyePositionOnSquatted;

	public static float CameraSpeedFactorToPercent(float camFactor)
	{
		return (camFactor - minCamSpeed) / (maxCamSpeed - minCamSpeed) * 100f;
	}

	private void Start()
	{
		ghostFrom = (ghostTo = base.transform.rotation);
		scopeCameraFactor = 1f;
		cameraSpeedFactor = PlayerPrefs.GetFloat("CameraSpeedFactor", 1f);
		me = base.transform.parent;
		eyePosition = new Vector3(0f, 1.55f, 0f);
		eyePositionOnSquatted = new Vector3(0f, 1.1f, 0f);
		cannon = null;
		cannonTryingTime = 0f;
		Camera[] componentsInChildren = GetComponentsInChildren<Camera>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == "First Person Camera")
			{
				fpCam = componentsInChildren[i];
			}
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		alive = true;
		recoilYaw = 0f;
		recoilPitch = 0f;
		sumOfRecoilYaw = 0f;
		sumOfRecoilPitch = 0f;
		if (QualitySettings.GetQualityLevel() < 3)
		{
			BloomAndFlares component = GetComponent<BloomAndFlares>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void OnHit()
	{
		recoilPitch -= 1f;
	}

	private void OnGetCannon(CannonController cannonController)
	{
		if (cannonController.Shooter == MyInfoManager.Instance.Seq)
		{
			base.transform.parent = cannonController.fpsLink.transform;
		}
	}

	private void OnEnterCannon(CannonController cannonController)
	{
		cannon = cannonController;
		cannonTryingTime = 0f;
		orgPosition = base.transform.position;
		orgRotation = base.transform.rotation;
	}

	private void OnLeaveCannon(LocalController localController)
	{
		Vector3 localPosition = eyePosition;
		base.transform.parent = me;
		base.transform.localPosition = localPosition;
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if (y > yMaxLimit)
		{
			y -= 360f;
		}
		cannon = null;
	}

	private void OnRespawn2(Quaternion rotation)
	{
		base.transform.rotation = rotation;
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
	}

	private void OnRespawn(Quaternion rotation)
	{
		alive = true;
		if (MyInfoManager.Instance.isGuiOn)
		{
			fpCam.enabled = true;
		}
		base.transform.localPosition = eyePosition;
		base.transform.rotation = rotation;
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
	}

	public void EnableFpCam(bool enable)
	{
		fpCam.enabled = enable;
	}

	private void OnDeath(int killer)
	{
		ghostFrom = (ghostTo = base.transform.rotation);
		ghostTime = 0f;
		GameObject gameObject = BrickManManager.Instance.Get(killer);
		if (null != gameObject)
		{
			base.transform.LookAt(gameObject.transform.position);
			ghostTo = base.transform.rotation;
			base.transform.rotation = ghostFrom;
		}
		alive = false;
		fpCam.enabled = false;
	}

	public void SetCameraSpeedFactor(float factor)
	{
		if (factor < 0f)
		{
			factor = 0f;
		}
		scopeCameraFactor = factor;
	}

	public void Pitchup(float pitch, float yaw)
	{
		recoilPitch = pitch;
		recoilYaw = yaw;
	}

	private void OnGUI()
	{
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.depth = 50;
		GUI.enabled = !DialogManager.Instance.IsModal;
		bool flag = (custom_inputs.Instance.GetButton("K_MOUSE_DOWN") || custom_inputs.Instance.GetButton("K_MOUSE_UP")) && Screen.lockCursor;
		if (flag || cfShowTime < 1f)
		{
			string text = StringMgr.Instance.Get("MOUSE_SENSIBILITY_MODIFIED") + Mathf.RoundToInt(CameraSpeedFactorToPercent(cameraSpeedFactor)).ToString();
			Color clrText;
			Color clrOutline;
			if (flag)
			{
				clrText = new Color(0.91f, 0.6f, 0f, 1f);
				clrOutline = new Color(0f, 0f, 0f, 1f);
			}
			else
			{
				clrText = Color.Lerp(new Color(0.91f, 0.6f, 0f, 1f), new Color(0.91f, 0.6f, 0f, 0f), cfShowTime);
				clrOutline = Color.Lerp(new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f), cfShowTime);
			}
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)), text, "BigLabel", clrText, clrOutline, TextAnchor.MiddleCenter);
		}
		GUI.enabled = true;
		GUI.skin = skin;
	}

	private void Update()
	{
		if ((custom_inputs.Instance.GetButton("K_MOUSE_DOWN") || custom_inputs.Instance.GetButton("K_MOUSE_UP")) && Screen.lockCursor)
		{
			cameraSpeedFactorAccel += 0.01f;
		}
		cfShowTime += Time.deltaTime;
		float num = cameraSpeedFactor;
		float axisRaw = custom_inputs.Instance.GetAxisRaw("K_MOUSE_UP", "K_MOUSE_DOWN");
		if ((custom_inputs.Instance.GetButtonDown("K_MOUSE_DOWN") || custom_inputs.Instance.GetButtonDown("K_MOUSE_UP")) && Screen.lockCursor)
		{
			if (axisRaw < 0f)
			{
				cameraSpeedFactor -= (maxCamSpeed - minCamSpeed) / 100f;
			}
			else
			{
				cameraSpeedFactor += (maxCamSpeed - minCamSpeed) / 100f;
			}
			cfShowTime = 0f;
			cameraSpeedFactorAccel = 0.02f;
		}
		cameraSpeedFactor += axisRaw * Time.deltaTime * cameraSpeedFactorAccel;
		if (cameraSpeedFactor < minCamSpeed)
		{
			cameraSpeedFactor = minCamSpeed;
		}
		if (cameraSpeedFactor > maxCamSpeed)
		{
			cameraSpeedFactor = maxCamSpeed;
		}
		if (Mathf.Abs(num - cameraSpeedFactor) > 0.01f)
		{
			PlayerPrefs.SetFloat("CameraSpeedFactor", cameraSpeedFactor);
		}
		UpdateSpeedUpFov();
	}

	private void _LateUpdate4SpectatorMode()
	{
		SpectatorController[] allComponents = Recursively.GetAllComponents<SpectatorController>(base.transform, includeInactive: false);
		SpectatorSwitch[] allComponents2 = Recursively.GetAllComponents<SpectatorSwitch>(base.transform, includeInactive: false);
		if (allComponents2.Length > 0 && allComponents2[0].Target != null && allComponents.Length > 0)
		{
			Vector3 position = allComponents2[0].Target.transform.position;
			Vector3 worldPosition = new Vector3(position.x, position.y + allComponents[0].targetHeight, position.z);
			base.transform.LookAt(worldPosition);
		}
	}

	private void _LateUpdate4Respawn()
	{
		ghostTime += ghostCameraSpeed * Time.deltaTime;
		base.transform.rotation = Quaternion.Lerp(ghostFrom, ghostTo, ghostTime);
	}

	private void _LateUpdate4Cannon()
	{
		if (cannon.Shooter < 0)
		{
			cannonTryingTime += Time.deltaTime;
			float value = cannonTryingTime / cannon.tryingTime;
			value = Mathf.Clamp(value, 0f, 1f);
			base.transform.position = Vector3.Lerp(orgPosition, cannon.fpsLink.transform.position, value);
			base.transform.rotation = Quaternion.Lerp(orgRotation, cannon.fpsLink.transform.rotation, value);
		}
	}

	public void Reset(float _x, float _y)
	{
		x = _x;
		y = _y;
		recoilYaw = 0f;
		recoilPitch = 0f;
		sumOfRecoilYaw = 0f;
		sumOfRecoilPitch = 0f;
	}

	private void LateUpdate()
	{
		if (MyInfoManager.Instance.IsSpectator)
		{
			_LateUpdate4SpectatorMode();
		}
		else if (!alive)
		{
			_LateUpdate4Respawn();
		}
		else if (null != cannon)
		{
			_LateUpdate4Cannon();
		}
		else
		{
			if (Screen.lockCursor)
			{
				float num = 0f;
				if (sumOfRecoilPitch > 0f)
				{
					num = y - Mathf.Lerp(y, y - sumOfRecoilPitch, 2f * Time.deltaTime);
					sumOfRecoilPitch -= num;
					if (sumOfRecoilPitch < 0f)
					{
						sumOfRecoilPitch = 0f;
					}
				}
				float num2 = 0f;
				if (sumOfRecoilYaw < 0f)
				{
					num2 = x - Mathf.Lerp(x, x - sumOfRecoilYaw, 2f * Time.deltaTime);
					sumOfRecoilYaw -= num2;
					if (sumOfRecoilYaw > 0f)
					{
						sumOfRecoilYaw = 0f;
					}
				}
				else if (sumOfRecoilYaw > 0f)
				{
					num2 = x - Mathf.Lerp(x, x - sumOfRecoilYaw, 2f * Time.deltaTime);
					sumOfRecoilYaw -= num2;
					if (sumOfRecoilYaw < 0f)
					{
						sumOfRecoilYaw = 0f;
					}
				}
				sumOfRecoilYaw += recoilYaw;
				sumOfRecoilPitch += recoilPitch;
				x += Input.GetAxis("Mouse X") * xSpeed * cameraSpeedFactor * scopeCameraFactor * 0.02f + recoilYaw - num2;
				if (GlobalVars.Instance.reverseMouse)
				{
					y += Input.GetAxis("Mouse Y") * ySpeed * cameraSpeedFactor * scopeCameraFactor * 0.02f - recoilPitch + num;
				}
				else
				{
					y -= Input.GetAxis("Mouse Y") * ySpeed * cameraSpeedFactor * scopeCameraFactor * 0.02f + recoilPitch - num;
				}
				recoilPitch = 0f;
				recoilYaw = 0f;
				y = Angles.ClampAngle(y, yMinLimit, yMaxLimit);
			}
			base.transform.rotation = Quaternion.Euler(y, x, 0f);
		}
	}

	private void ResetFov()
	{
		GetComponent<Camera>().fieldOfView = 60f + scopeFov + speedUpFov;
	}

	public void SetScopeFov(float fov)
	{
		scopeFov = fov - 60f;
		ResetFov();
	}

	public void SetSpeedUpFov(bool isSpeedUp)
	{
		speedUpEffect = isSpeedUp;
	}

	private void UpdateSpeedUpFov()
	{
		if (speedUpEffect && speedUpFov != speedUpFovMax)
		{
			speedUpFov += speedUpFovZoomIn * Time.deltaTime;
			if (Mathf.Abs(speedUpFov) > Mathf.Abs(speedUpFovMax))
			{
				speedUpFov = speedUpFovMax;
			}
			ResetFov();
		}
		else if (!speedUpEffect && speedUpFov != 0f)
		{
			speedUpFov -= speedUpFovZoomOut * Time.deltaTime;
			if (speedUpFovMax < 0f && speedUpFov > 0f)
			{
				speedUpFov = 0f;
			}
			else if (speedUpFovMax > 0f && speedUpFov < 0f)
			{
				speedUpFov = 0f;
			}
			ResetFov();
		}
	}

	public void ChangeCameraSpeedFactor(float speed)
	{
		cameraSpeedFactor = speed;
		cfShowTime = 0f;
	}
}
