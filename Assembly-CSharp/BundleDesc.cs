using System.Collections.Generic;

public class BundleDesc
{
	private List<BundleUnit> items;

	public BundleDesc()
	{
		items = new List<BundleUnit>();
	}

	public void Pack(TItem tItem, int opt)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].tItem.code == tItem.code)
			{
				return;
			}
		}
		items.Add(new BundleUnit(tItem, opt));
	}

	public BundleUnit[] Unpack()
	{
		return items.ToArray();
	}
}
