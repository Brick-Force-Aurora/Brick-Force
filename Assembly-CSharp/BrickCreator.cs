using UnityEngine;

public class BrickCreator : MonoBehaviour
{
	public float delayTime = 10f;

	private int seq;

	private byte brick;

	private byte rot;

	private Brick newBrick;

	private float deltaTime;

	private Transform cube;

	private BoxCollider boxCollider;

	public int Seq
	{
		get
		{
			return seq;
		}
		set
		{
			seq = value;
		}
	}

	public byte Brick
	{
		get
		{
			return brick;
		}
		set
		{
			brick = value;
		}
	}

	public byte Rotation
	{
		get
		{
			return rot;
		}
		set
		{
			rot = value;
		}
	}

	private void Start()
	{
		newBrick = BrickManager.Instance.GetBrick(brick);
		if (newBrick == null)
		{
			Debug.LogError("ERROR, Fail to get Brick Template. Trying to start BrickCreator with invalid brick ");
		}
		deltaTime = 0f;
		base.transform.rotation = Rot.ToQuaternion(rot);
		cube = base.transform.Find("Cube");
		if (cube != null)
		{
			Vector3 size = new Vector3(1f, 1f, 1f);
			Vector3 center = Vector3.zero;
			BrickManager.Instance.FetchCenterAndSize(brick, ref center, ref size);
			cube.transform.localPosition = center;
			cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			UniformedScaler component = cube.GetComponent<UniformedScaler>();
			if (null != component)
			{
				component.targetScale = size;
			}
			boxCollider = GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				Debug.LogError("Brick Creator doesnot have box collider");
			}
			else
			{
				boxCollider.size = size;
				boxCollider.center = new Vector3(0f, size.y / 2f - 0.5f, 0f);
			}
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > delayTime)
		{
			if (null != boxCollider)
			{
				Object.DestroyImmediate(boxCollider);
				boxCollider = null;
			}
			if (BrickManager.Instance.IsEmpty(newBrick, base.transform.position, brickOnly: false))
			{
				BrickManager.Instance.DelBrickCreator(seq);
				BrickManager.Instance.AddBrick(seq, brick, base.transform.position, rot);
				Object.DestroyImmediate(base.transform.gameObject);
			}
			else if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				if (!BrickManager.Instance.IsEmpty(newBrick, base.transform.position, brickOnly: true))
				{
					Debug.LogError("Master deleted invalid brick " + seq);
					CSNetManager.Instance.Sock.SendCS_DEL_BRICK_REQ(seq);
					Object.DestroyImmediate(base.transform.gameObject);
				}
			}
			else if (!BrickManager.Instance.IsEmpty(newBrick, base.transform.position, brickOnly: true))
			{
				Debug.LogError("Slave deleted invalid brick Instance" + seq);
				Object.DestroyImmediate(base.transform.gameObject);
			}
		}
	}
}
