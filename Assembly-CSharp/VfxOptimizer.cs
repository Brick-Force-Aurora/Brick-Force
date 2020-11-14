using System.Collections.Generic;
using UnityEngine;

public class VfxOptimizer : MonoBehaviour
{
	public enum VFX_TYPE
	{
		SHELL,
		MUZZLE_FIRE,
		BULLET_TRAIL,
		BULLET_MARK,
		BULLET_IMPACT,
		SHELL2,
		COUNT
	}

	public GameObject[] impacts;

	public string[] layers;

	private Dictionary<int, GameObject> dicImpact;

	private float[] deltaTimes;

	private float deltaMax = 0.2f;

	private Camera cam;

	private static VfxOptimizer _instance;

	public static VfxOptimizer Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(VfxOptimizer)) as VfxOptimizer);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the VfxOptimizer Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		dicImpact = new Dictionary<int, GameObject>();
	}

	private void Start()
	{
		deltaTimes = new float[6];
		switch (QualitySettings.GetQualityLevel())
		{
		case 0:
		case 1:
			deltaMax = 0.5f;
			break;
		case 2:
		case 3:
			deltaMax = 0.3f;
			break;
		case 4:
		case 5:
			deltaMax = 0.1f;
			break;
		}
		for (int i = 0; i < impacts.Length; i++)
		{
			int key = LayerMask.NameToLayer(layers[i]);
			if (impacts[i] != null)
			{
				dicImpact.Add(key, impacts[i]);
			}
		}
	}

	private void VerifyCamera()
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

	private void Update()
	{
		VerifyCamera();
		for (int i = 0; i < 6; i++)
		{
			deltaTimes[i] += Time.deltaTime;
		}
	}

	public void SetupCamera()
	{
		GameObject gameObject = GameObject.Find("Main Camera");
		if (null != gameObject)
		{
			cam = gameObject.GetComponent<Camera>();
		}
	}

	public GameObject CreateFx(GameObject pfb, Vector3 pos, Quaternion rot, VFX_TYPE vfxType)
	{
		if (deltaTimes == null || deltaTimes.Length == 0)
		{
			return null;
		}
		if (null != pfb && cam != null && Direction.IsForward(pos, cam.transform) && deltaTimes[(int)vfxType] > deltaMax)
		{
			deltaTimes[(int)vfxType] = 0f;
			return Object.Instantiate((Object)pfb, pos, rot) as GameObject;
		}
		return null;
	}

	public GameObject CreateFxImmediate(GameObject pfb, Vector3 pos, Quaternion rot, VFX_TYPE vfxType)
	{
		if (null != pfb && Direction.IsForward(pos, cam.transform))
		{
			return Object.Instantiate((Object)pfb, pos, rot) as GameObject;
		}
		return null;
	}

	public GameObject GetImpact(int layer)
	{
		if (dicImpact.ContainsKey(layer))
		{
			return dicImpact[layer];
		}
		return null;
	}
}
