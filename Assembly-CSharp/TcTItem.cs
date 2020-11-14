public struct TcTItem
{
	public string code;

	public bool isKey;

	public int opt;

	public bool IsNull()
	{
		return code.Length <= 0;
	}
}
