using System.Collections;

internal class clipNameManager
{
	private ArrayList clipNames;

	public void Alloc()
	{
		clipNames = new ArrayList();
	}

	public void Add(string clipName)
	{
		clipNames.Add(clipName);
	}

	public bool Find(string findName)
	{
		if (clipNames.Contains(findName))
		{
			return true;
		}
		return false;
	}
}
