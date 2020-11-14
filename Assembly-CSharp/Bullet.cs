using UnityEngine;

public class Bullet : MonoBehaviour
{
	private float speed = 6f;

	private Vector3 orgPosition = Vector3.zero;

	private Vector3 orgDirection = Vector3.zero;

	private Vector3 prevPosition = Vector3.zero;

	private bool hit;

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public Vector3 OrgPosition
	{
		get
		{
			return orgPosition;
		}
		set
		{
			orgPosition = value;
		}
	}

	public Vector3 OrgDirection
	{
		get
		{
			return orgDirection;
		}
		set
		{
			orgDirection = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		orgDirection = base.transform.TransformDirection(Vector3.forward);
		prevPosition = (orgPosition = base.transform.position);
		base.transform.position += 10f * orgDirection;
		hit = false;
	}

	private bool LineTest(out RaycastHit hitInfo)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("BoxMan"));
		return Physics.Linecast(prevPosition, base.transform.position, out hitInfo, layerMask);
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (hit || Vector3.Distance(orgPosition, base.transform.position) > 1000f)
		{
			Object.DestroyImmediate(base.transform.gameObject);
		}
		else if (LineTest(out hitInfo))
		{
			base.transform.position = hitInfo.point;
			hit = true;
		}
		else
		{
			Vector3 a = speed * orgDirection;
			a *= Time.deltaTime;
			base.transform.position += a;
		}
	}
}
