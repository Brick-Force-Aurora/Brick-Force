using UnityEngine;

public class ProjectileWrap
{
	public Vector3 targetPos;

	public Vector3 targetRot;

	public GameObject projectile;

	public float range;

	private float Elapsed;

	public bool overTime;

	public ProjectileWrap(GameObject obj)
	{
		targetPos = obj.transform.position;
		targetRot = obj.transform.rotation.eulerAngles;
		projectile = obj;
		range = 0f;
	}

	public void Fly()
	{
		if (!(projectile == null))
		{
			projectile.transform.position = Vector3.Lerp(projectile.transform.position, targetPos, 10f * Time.deltaTime);
			projectile.transform.rotation = Quaternion.Lerp(projectile.transform.rotation, Quaternion.Euler(targetRot), 10f * Time.deltaTime);
		}
	}

	public void Fly2()
	{
		if (!(projectile == null))
		{
			Elapsed += Time.deltaTime;
			if (Elapsed > 5f)
			{
				overTime = true;
			}
			projectile.transform.position = Vector3.Lerp(projectile.transform.position, targetPos, 10f * Time.deltaTime);
		}
	}

	public Vector3 GetPos()
	{
		return projectile.transform.position;
	}
}
