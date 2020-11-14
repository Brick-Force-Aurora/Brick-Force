using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
	private static TrainManager _instance;

	public GameObject objTrain;

	private List<TrainController> trainObj;

	private LocalController localController;

	public static TrainManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(TrainManager)) as TrainManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the TrainManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		trainObj = new List<TrainController>();
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void collideTest()
	{
		int layerMask = 1 << LayerMask.NameToLayer("BoxMan");
		for (int i = 0; i < trainObj.Count; i++)
		{
			float radius = 1f;
			Vector3 position = trainObj[i].train.transform.position;
			Collider[] array = Physics.OverlapSphere(new Vector3(position.x, position.y, position.z), radius, layerMask);
			if (array != null && array.Length > 0 && trainObj[i].shooter == MyInfoManager.Instance.Seq)
			{
				localController.TrainStop();
			}
		}
	}

	public void Update()
	{
		if (!IsEmpty())
		{
			VerifyLocalController();
			collideTest();
			for (int i = 0; i < trainObj.Count; i++)
			{
				if (trainObj[i].shooter >= 0 && localController.trainID != i)
				{
					GameObject gameObject = BrickManManager.Instance.Get(trainObj[i].shooter);
					if (gameObject != null)
					{
						Vector3 position = gameObject.transform.position;
						position.y += 0.35f;
						trainObj[i].train.transform.position = position;
					}
				}
			}
		}
	}

	public void SetRotation(int Id, Vector3 fwd)
	{
		TrainController controller = GetController(Id);
		if (controller != null)
		{
			controller.train.transform.forward = fwd;
		}
	}

	private void VerifyLocalController()
	{
		if (null == localController)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				localController = gameObject.GetComponent<LocalController>();
			}
		}
	}

	public void UnLoad()
	{
		if (trainObj != null && trainObj.Count != 0)
		{
			trainObj.Clear();
		}
	}

	public void Load()
	{
		if (BrickManager.Instance.userMap != null)
		{
			VerifyLocalController();
			localController.initTrain();
			SpawnerDesc[] railSpawners = BrickManager.Instance.userMap.GetRailSpawners();
			if (railSpawners != null)
			{
				for (int i = 0; i < railSpawners.Length; i++)
				{
					TrainController trainController = new TrainController();
					trainController.shooter = -1;
					trainController.seq = railSpawners[i].sequence;
					trainController.setInit(railSpawners[i].position, Rot.ToQuaternion(railSpawners[i].rotation));
					trainController.train = (Object.Instantiate((Object)Instance.objTrain, railSpawners[i].position, Rot.ToQuaternion(railSpawners[i].rotation)) as GameObject);
					trainObj.Add(trainController);
				}
			}
		}
	}

	public int GetTrainCount()
	{
		return trainObj.Count;
	}

	public TrainController GetController(int i)
	{
		return trainObj[i];
	}

	public Vector3 GetPosition(int i)
	{
		if (trainObj == null || trainObj.Count == 0)
		{
			return new Vector3(-99999f, -99999f, -99999f);
		}
		if (i < 0 || i >= trainObj.Count)
		{
			return new Vector3(-99999f, -99999f, -99999f);
		}
		return trainObj[i].train.transform.position;
	}

	public int GetSequance(int i)
	{
		if (trainObj == null || trainObj.Count == 0)
		{
			return -1;
		}
		if (i < 0 || i >= trainObj.Count)
		{
			return -1;
		}
		return trainObj[i].seq;
	}

	public bool IsEmpty()
	{
		if (trainObj == null || trainObj.Count == 0)
		{
			return true;
		}
		return false;
	}

	public void SetShooter(int ctrl, int player)
	{
		if (trainObj != null && trainObj.Count != 0 && ctrl >= 0 && ctrl < trainObj.Count)
		{
			trainObj[ctrl].shooter = player;
			if (player == -1)
			{
				if (localController.trainID == ctrl)
				{
					localController.LeaveTrain();
				}
				trainObj[ctrl].regen();
			}
			else if (MyInfoManager.Instance.Seq == player)
			{
				localController.OnGetTrain();
			}
		}
	}
}
