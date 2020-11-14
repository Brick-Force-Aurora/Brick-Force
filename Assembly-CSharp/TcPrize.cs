using UnityEngine;

public class TcPrize
{
	private TItem tItem;

	private int amount;

	private float deltaTime;

	private float flickering;

	private bool outline;

	private bool isRareItem;

	public Texture2D Icon => tItem.CurIcon();

	public string Name => tItem.Name;

	public string Code => tItem.code;

	public string AmountString => tItem.GetOptionStringByOption(amount);

	public bool IsRareItem => isRareItem;

	public bool NeedOutline => deltaTime > 2.1f || outline;

	public TcPrize(Flying flying)
	{
		tItem = flying.Template;
		amount = flying.Amount;
		isRareItem = flying.IsRareItem;
		deltaTime = 0f;
		flickering = 0f;
		outline = true;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		flickering += Time.deltaTime;
		if (flickering > 0.3f)
		{
			flickering = 0f;
			outline = !outline;
		}
	}
}
