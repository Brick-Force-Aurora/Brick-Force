public class Flying
{
	private long seq;

	private TItem tItem;

	private int amount;

	private bool isRareItem;

	public float deltaTime;

	public long Seq => seq;

	public TItem Template => tItem;

	public int Amount => amount;

	public bool IsRareItem => isRareItem;

	public Flying(long _seq, TItem _tItem, int _amount, bool _isRareItem)
	{
		seq = _seq;
		tItem = _tItem;
		amount = _amount;
		isRareItem = _isRareItem;
		deltaTime = 0f;
	}
}
