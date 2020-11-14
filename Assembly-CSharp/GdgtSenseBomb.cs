using UnityEngine;

public class GdgtSenseBomb : WeaponGadget
{
	private GameObject beamObj;

	private GameObject bombObj;

	public GameObject explosion;

	private Vector3 expPos = Vector3.zero;

	private int senseBombSeq = -1;

	private GameObject installingEff;

	private bool installing;

	private float dtWaitBeam;

	private float maxWaitBeam = 2f;

	private Vector3 vBomb;

	private Vector3 vBombNormal;

	private int playerslot = -1;

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
		base.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
	}

	public void EnableHandbomb(bool enable)
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = enable;
		}
		if (enable)
		{
			base.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
		}
	}

	private void OnDisable()
	{
	}

	private bool IsRed()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
		{
			if (playerslot < 4)
			{
				return true;
			}
			return false;
		}
		if (playerslot < 8)
		{
			return true;
		}
		return false;
	}

	private void activeBeam()
	{
		if (installing)
		{
			dtWaitBeam += Time.deltaTime;
			if (dtWaitBeam > maxWaitBeam)
			{
				Object.DestroyImmediate(installingEff);
				beamObj = (Object.Instantiate((Object)((!IsRed()) ? GlobalVars.Instance.SenseBeam2 : GlobalVars.Instance.SenseBeam), vBomb, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
				beamObj.transform.up = vBombNormal;
				beamObj.transform.localScale = new Vector3(1f, 2f, 1f);
				installing = false;
			}
		}
	}

	public override void SetSenseBeam(int playerSlot, int index, Vector3 initPos, Vector3 pos, Vector3 normal, bool bSoundvoc)
	{
		if (senseBombSeq != index)
		{
			bombObj = (Object.Instantiate((Object)GlobalVars.Instance.SenseBomb, pos, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			bombObj.transform.up = normal;
			vBomb = pos;
			vBombNormal = normal;
			installing = true;
			installingEff = (Object.Instantiate((Object)GlobalVars.Instance.installingEff, pos + vBombNormal * 0.14f, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			expPos = pos;
			senseBombSeq = index;
			playerslot = playerSlot;
		}
	}

	public void Kaboom(int index)
	{
		if (senseBombSeq == index)
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
			{
				if (explosion != null)
				{
					Object.Instantiate((Object)explosion, expPos, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, expPos, Quaternion.Euler(0f, 0f, 0f));
			}
			Object.DestroyImmediate(beamObj);
			Object.DestroyImmediate(bombObj);
			beamObj = null;
			bombObj = null;
		}
	}

	private void Update()
	{
		activeBeam();
	}
}
