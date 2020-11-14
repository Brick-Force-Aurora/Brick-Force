using UnityEngine;

public class IconReq
{
	public string code;

	public string iconPath;

	public WWW CDN;

	public IconReq(string c, string p)
	{
		code = c;
		iconPath = p;
		CDN = null;
	}
}
