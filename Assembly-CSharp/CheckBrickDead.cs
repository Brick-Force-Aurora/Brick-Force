using UnityEngine;

public class CheckBrickDead : MonoBehaviour
{
	private int idBrick = -1;

	public int brickID
	{
		get
		{
			return idBrick;
		}
		set
		{
			idBrick = value;
		}
	}

	private void Update()
	{
		if (idBrick >= 0)
		{
			GameObject brickObject = BrickManager.Instance.GetBrickObject(idBrick);
			if (brickObject == null)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
