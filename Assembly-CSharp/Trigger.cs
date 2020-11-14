using UnityEngine;

public class Trigger : MonoBehaviour
{
	private BfScript script;

	private void Start()
	{
		script = null;
		BrickProperty component = base.transform.parent.gameObject.GetComponent<BrickProperty>();
		if (null == component)
		{
			Debug.LogError("Fail to find BrickProperty");
		}
		else
		{
			BrickInst brickInst = BrickManager.Instance.GetBrickInst(component.Seq);
			if (brickInst == null)
			{
				Debug.LogError("Fail to find BrickInst ");
			}
			else
			{
				script = brickInst.BrickForceScript;
			}
			base.enabled = script.EnableOnAwake;
			Show(script.VisibleOnAwake);
		}
	}

	public void Show(bool visible)
	{
		if (!Application.loadedLevelName.Contains("MapEditor"))
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = visible;
			}
			SkinnedMeshRenderer[] componentsInChildren2 = GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array2 = componentsInChildren2;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
			{
				skinnedMeshRenderer.enabled = visible;
			}
			ParticleRenderer[] componentsInChildren3 = GetComponentsInChildren<ParticleRenderer>();
			ParticleRenderer[] array3 = componentsInChildren3;
			foreach (ParticleRenderer particleRenderer in array3)
			{
				particleRenderer.enabled = visible;
			}
			BrickProperty component = base.transform.parent.gameObject.GetComponent<BrickProperty>();
			if (component.Index == 162)
			{
				if (visible)
				{
					component.Visible_t = true;
				}
				else if (component.Visible_t)
				{
					BrickManager.Instance.DestroyBrick(component.Seq);
				}
			}
			else if (component.Index == 180 && !visible && GlobalVars.Instance.immediateKillBrickTutor)
			{
				BrickManager.Instance.DestroyBrick(component.Seq);
				GlobalVars.Instance.immediateKillBrickTutor = false;
			}
		}
	}

	public void RunScript()
	{
		if (script != null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (!(null == gameObject))
			{
				GameObject gameObject2 = Object.Instantiate((Object)ScriptResManager.Instance.Executor, base.transform.position, base.transform.rotation) as GameObject;
				if (null != gameObject2)
				{
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.GetComponent<ScriptExecutor>().Run(script);
				}
			}
		}
	}

	private void Update()
	{
	}
}
