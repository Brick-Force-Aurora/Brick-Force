using UnityEngine;

public class AssetBundleRef
{
	public AssetBundle assetBundle;

	public int version;

	public string url;

	public AssetBundleRef()
	{
		url = string.Empty;
		version = 0;
	}

	public AssetBundleRef(string strUrlIn, int intVersionIn)
	{
		url = strUrlIn;
		version = intVersionIn;
	}
}
