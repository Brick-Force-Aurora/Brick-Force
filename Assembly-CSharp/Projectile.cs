using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject explosion;

	protected float explosionTime = 3f;

	protected float deltaTime;

	private float detonatorTime;

	private float persistTime;

	private int index;

	private bool applyUsk;

	protected ProjectileAlert projectileAlert;

	public float ExplosionTime
	{
		get
		{
			return explosionTime;
		}
		set
		{
			explosionTime = value;
		}
	}

	public float DetonatorTime
	{
		get
		{
			return detonatorTime;
		}
		set
		{
			detonatorTime = value;
		}
	}

	public float PersistTime
	{
		get
		{
			return persistTime;
		}
		set
		{
			persistTime = value;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public bool ApplyUsk
	{
		get
		{
			return applyUsk;
		}
		set
		{
			applyUsk = value;
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			projectileAlert = gameObject.GetComponent<ProjectileAlert>();
		}
	}
}
