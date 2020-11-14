using System;
using UnityEngine;

[Serializable]
public class Dialog
{
	public string WindowStyle = "Window";

	public Vector2 size;

	protected Rect rc;

	protected DialogManager.DIALOG_INDEX id = DialogManager.DIALOG_INDEX.NUM;

	public Rect ClientRect
	{
		get
		{
			return rc;
		}
		set
		{
			rc = value;
		}
	}

	public DialogManager.DIALOG_INDEX ID => id;

	public virtual void Start()
	{
	}

	public virtual void OnPopup()
	{
	}

	public virtual void OnClose(DialogManager.DIALOG_INDEX popup)
	{
	}

	public virtual bool DoDialog()
	{
		return true;
	}

	public virtual void Update()
	{
	}
}
