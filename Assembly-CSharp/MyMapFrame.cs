using System;
using UnityEngine;

[Serializable]
public class MyMapFrame
{
	public string[] tabKey;

	private string[] tabs;

	public Rect crdFrame = new Rect(190f, 62f, 570f, 538f);

	public Rect crdSubFrame = new Rect(250f, 42f, 510f, 425f);

	public Rect crdTab = new Rect(250f, 42f, 204f, 26f);

	public EditingMapFrame editMapFrm;

	public MyRegMapFrame myRegMapFrm;

	public DownloadMapFrame downloadMapFrm;

	public int currentTab;

	public int modeTab;

	public bool bGuiEnable;

	public void Start()
	{
		tabs = new string[tabKey.Length];
		for (int i = 0; i < 3; i++)
		{
			tabs[i] = StringMgr.Instance.Get(tabKey[i]);
		}
		editMapFrm.Start();
		myRegMapFrm.Start();
		downloadMapFrm.Start();
	}

	public void OnGUI()
	{
		if (!bGuiEnable)
		{
			GUI.enabled = false;
		}
		switch (currentTab)
		{
		case 0:
			editMapFrm.OnGUI();
			break;
		case 1:
			myRegMapFrm.SelectedTab(modeTab);
			myRegMapFrm.OnGUI();
			break;
		case 2:
			downloadMapFrm.SelectedTab(modeTab);
			downloadMapFrm.OnGUI();
			break;
		}
		if (!bGuiEnable)
		{
			GUI.enabled = true;
		}
	}
}
