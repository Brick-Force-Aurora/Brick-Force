using UnityEngine;

public class TimeLimitedParticleDestroyer : MonoBehaviour
{
	public float particleTime = 2.5f;

	public float lifeTime = 10f;

	private bool particlePhase = true;

	private float deltaTime;

	private void Start()
	{
		particlePhase = true;
		deltaTime = 0f;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (particlePhase && deltaTime > particleTime)
		{
			particlePhase = false;
			ParticleEmitter[] componentsInChildren = GetComponentsInChildren<ParticleEmitter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].minEmission = 0f;
				componentsInChildren[i].maxEmission = 0f;
			}
		}
		if (deltaTime > lifeTime)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
