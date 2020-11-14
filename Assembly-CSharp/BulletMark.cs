using UnityEngine;

public class BulletMark : MonoBehaviour
{
	private float lifeTime;

	public float lengthOfLife = 10f;

	public void GenerateDecal(Texture2D mark, GameObject mesh, GameObject parent)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.transform.Rotate(new Vector3(0f, 0f, Random.Range(-180f, 180f)));
		lifeTime = 0f;
		Decal.dCount++;
		Decal component = GetComponent<Decal>();
		component.affectedObjects = new GameObject[1];
		component.affectedObjects[0] = mesh;
		component.decalMode = 0;
		component.pushDistance = 0.009f;
		Material material = new Material(component.decalMaterial);
		material.mainTexture = mark;
		component.decalMaterial = material;
		component.CalculateDecal();
		((Component)component).transform.parent = parent.transform;
	}

	private void Update()
	{
		lifeTime += Time.deltaTime;
		if (lifeTime > lengthOfLife)
		{
			Object.Destroy(base.gameObject);
			Decal.dCount--;
		}
	}
}
