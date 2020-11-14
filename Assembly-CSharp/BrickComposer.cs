using _Emulator;
using UnityEngine;

public class BrickComposer : WeaponFunction
{
	public float rateOfFire = 5f;

	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public GameObject muzzleFire;

	public Texture2D icon;

	public GameObject targetBox;

	private float deltaTime = float.NegativeInfinity;

	private GameObject target;

	private RaycastHit hitBrick;

	private float distance = float.PositiveInfinity;

	private Transform muzzle;

	private GameObject muzzleFxInstance;

	private bool copyRight;

	private BattleChat battleChat;

	private EditorTools tools;

	private string statusMessage = string.Empty;

	private float statusDelta;

	private float statusMessageLimit = 5f;

	private bool fire1click;

	private bool fire2click;

	private float ElapsedFeverAutoClick;

	private int tutoBrickIndex;

	public bool AutoComposer;

	private float ElapsedAutoClick;

	private void InitializeAnimation()
	{
		base.animation.wrapMode = WrapMode.Loop;
		base.animation["fire"].layer = 1;
		base.animation["fire"].wrapMode = WrapMode.Once;
		base.animation["idle"].layer = 1;
		base.animation["idle"].wrapMode = WrapMode.Loop;
		base.animation.CrossFade("idle");
	}

	private void VerifyEditorTool()
	{
		if (null == tools)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				tools = gameObject.GetComponent<EditorTools>();
			}
		}
	}

	private void VerifyBattleChat()
	{
		if (null == battleChat)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				battleChat = gameObject.GetComponent<BattleChat>();
			}
		}
	}

	public bool IsCoolDown()
	{
		if (deltaTime < 0f)
		{
			return false;
		}
		float num = rateOfFire / 60f;
		if (num <= 0f)
		{
			return true;
		}
		float num2 = 1f / num;
		if (GlobalVars.Instance.StateFever > 0)
		{
			num2 *= 0.5f;
		}
		return deltaTime < num2;
	}

	private void Start()
	{
		muzzle = null;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		int num = 0;
		while (muzzle == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name.Contains("Dummy_fire_effect"))
			{
				muzzle = componentsInChildren[num];
			}
			num++;
		}
		if (null == muzzle)
		{
			Debug.LogError("Muzzle is null ");
		}
		InitializeAnimation();
		CreateComposerTarget();
		copyRight = false;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR && UserMapInfoManager.Instance.GetCur() != null)
		{
			copyRight = true;
		}
	}

	private void CreateComposerTarget()
	{
		if (null == target)
		{
			target = (Object.Instantiate((Object)targetBox) as GameObject);
			if (null != target)
			{
				target.GetComponent<ComposerTarget>().ShowTarget(show: false);
			}
		}
	}

	private void DestroyComposerTarget()
	{
		if (null != target)
		{
			Object.Destroy(target);
			target = null;
		}
	}

	private void OnDisable()
	{
		fire1click = false;
		fire2click = false;
		feverAutoFire1Click = false;
		feverAutoFire2Click = false;
		DestroyComposerTarget();
	}

	private void OnEnable()
	{
		CreateComposerTarget();
	}

	private void CreateMuzzleFire()
	{
		if (!(null == muzzleFire) && !(null == muzzle))
		{
			if (muzzleFxInstance == null)
			{
				GameObject gameObject = Object.Instantiate((Object)muzzleFire) as GameObject;
				Recursively.SetLayer(gameObject.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
				gameObject.transform.position = muzzle.position;
				gameObject.transform.parent = muzzle;
				gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
				muzzleFxInstance = gameObject;
			}
			ParticleEmitter particleEmitter = null;
			int childCount = muzzleFxInstance.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = muzzleFxInstance.transform.GetChild(i);
				particleEmitter = child.GetComponent<ParticleEmitter>();
				if ((bool)particleEmitter)
				{
					particleEmitter.Emit();
				}
			}
		}
	}

	private void Restart()
	{
	}

	protected void DrawCrossHair()
	{
		if (Screen.lockCursor)
		{
			GUI.depth = 35;
			Color color = GUI.color;
			GUI.color = Config.instance.crosshairColor;
			if (null != vCrossHair)
			{
				Vector2 vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2 - 8));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			if (null != hCrossHair)
			{
				Vector2 vector = new Vector2((float)(Screen.width / 2 - 8), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)(Screen.width / 2), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
			string text = string.Empty;
			if (null != localCtrl)
			{
				float horzAngle = localCtrl.GetHorzAngle();
				if ((292.5 <= (double)horzAngle && horzAngle <= 360f) || (0f <= horzAngle && horzAngle <= 67.5f))
				{
					text += "N";
				}
				else if (112.5f <= horzAngle && horzAngle <= 247.5f)
				{
					text += "S";
				}
				if (22.5f <= horzAngle && horzAngle <= 157.5f)
				{
					text += "E";
				}
				else if (202.5f <= horzAngle && horzAngle <= 337.5f)
				{
					text += "W";
				}
			}
			if (distance != float.PositiveInfinity && hitBrick.transform != null)
			{
				BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
				if (null != hitBrickProperty)
				{
					text += ": ";
					text = text + distance.ToString("0.#") + "m";
				}
			}
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 16)), text, "Label", Color.white, Color.black, TextAnchor.MiddleCenter);
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawCrossHair();
			PaletteManager.Instance.Use();
			if (statusMessage.Length > 0)
			{
				float a = 1f;
				float num = (statusDelta - (statusMessageLimit - 2f)) / 2f;
				if (num > 0f)
				{
					a = Mathf.Lerp(1f, 0f, num);
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 32)), statusMessage, "BigLabel", new Color(0f, 0f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 30)), statusMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			GUI.enabled = true;
		}
	}

	private void ShowTarget(bool show)
	{
		if (!(null == target))
		{
			if (show)
			{
				BrickInst hitBrickInst = BrickManager.Instance.GetHitBrickInst(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
				if (hitBrickInst == null)
				{
					show = false;
				}
				else
				{
					Vector3 vector = new Vector3((float)(int)hitBrickInst.PosX, (float)(int)hitBrickInst.PosY, (float)(int)hitBrickInst.PosZ);
					GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(vector);
					BoxCollider boxCollider = null;
					if (brickObjectByPos != null)
					{
						boxCollider = brickObjectByPos.GetComponent<BoxCollider>();
					}
					if (null != boxCollider)
					{
						target.GetComponent<ComposerTarget>().CenterAndSize(boxCollider.center, boxCollider.size);
						target.transform.position = vector;
						target.transform.rotation = Rot.ToQuaternion(hitBrickInst.Rot);
					}
					else
					{
						target.GetComponent<ComposerTarget>().CenterAndSize(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
						target.transform.position = vector;
						target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					}
				}
			}
			target.GetComponent<ComposerTarget>().ShowTarget(show);
		}
	}

	private bool CheckAimed()
	{
		bool result = false;
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Edit Layer")) | (1 << LayerMask.NameToLayer("BndWall"));
		Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, GetComponent<Weapon>().range, layerMask))
		{
			distance = float.PositiveInfinity;
		}
		else
		{
			if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("BndWall"))
			{
				result = true;
				hitBrick = hitInfo;
			}
			distance = Vector3.Distance(cam.transform.position, hitInfo.point);
		}
		return result;
	}

	private byte GetRotFromCameraDir()
	{
		Vector3 from = cam.transform.TransformDirection(Vector3.forward);
		from.y = 0f;
		from = from.normalized;
		float[] array = new float[4]
		{
			Vector3.Angle(from, Vector3.back),
			Vector3.Angle(from, Vector3.left),
			Vector3.Angle(from, Vector3.forward),
			Vector3.Angle(from, Vector3.right)
		};
		float num = 360f;
		int num2 = -1;
		for (int i = 0; i < 4; i++)
		{
			if (num > array[i])
			{
				num2 = i;
				num = array[i];
			}
		}
		return (byte)num2;
	}

	private byte GetNewBricksRot(Brick newBrick, Vector3 normal)
	{
		byte result = 0;
		if (newBrick.directionable)
		{
			result = (byte)((!(normal == Vector3.forward)) ? ((normal == Vector3.right) ? 1 : ((normal == Vector3.back) ? 2 : ((!(normal == Vector3.left)) ? GetRotFromCameraDir() : 3))) : 0);
		}
		return result;
	}

	private Vector3 GetNewBricksPos(Brick newBrick, Vector3 normal, Vector3 point)
	{
		Vector3 a = Brick.ToBrickCoord(normal, point);
		if (normal == Vector3.up)
		{
			return a + normal;
		}
		if (normal == Vector3.down)
		{
			return a + (float)newBrick.vert * normal;
		}
		int num = Mathf.CeilToInt((float)newBrick.horz / 2f);
		return a + (float)num * normal;
	}

	private Vector3 GetNewBricksPos2(Brick newBrick, Vector3 normal, Vector3 point)
	{
		Vector3 a = Brick.ToBrickCoord(normal, point);
		if (normal == Vector3.up)
		{
			return a + normal * (float)newBrick.vert;
		}
		if (normal == Vector3.down)
		{
			return a + (float)newBrick.vert * normal;
		}
		int num = Mathf.CeilToInt((float)newBrick.horz);
		return a + (float)num * normal;
	}

	private bool IsDefenseBrick(int seq)
	{
		if (seq == 134 || seq == 135 || seq == 136)
		{
			return true;
		}
		return false;
	}

	private bool CheckNeighborXZ(Vector3 pos)
	{
		Vector3 pos2 = pos;
		pos2.x -= 3f;
		BrickInst byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.x += 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.z -= 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.z += 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		return false;
	}

	private void PreCheckFire2Line()
	{
		if (!(null == tools) && custom_inputs.Instance.GetButtonDown("K_FIRE2"))
		{
			tools.GetLineTool()?.GoBack();
		}
	}

	private void PreCheckFire1Line()
	{
		if (UserMapInfoManager.Instance.CheckAuth(showMessage: false) && !(null == tools))
		{
			LineTool tool = tools.GetLineTool();
			if (tool != null)
			{
				Brick currentBrick = PaletteManager.Instance.GetCurrentBrick();
				if (currentBrick != null && currentBrick.IsEnable(RoomManager.Instance.CurrentRoomType))
				{
					Vector3 newBricksPos = GetNewBricksPos(currentBrick, hitBrick.normal, hitBrick.point);
					byte x = 0;
					byte y = 0;
					byte z = 0;
					if (BrickManager.Instance.ToCoord(newBricksPos, ref x, ref y, ref z) && custom_inputs.Instance.GetButtonDown("K_FIRE1"))
					{
						if (!currentBrick.IsUnit)
						{
							SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("LINE_TOOL_DOES_NOT_SUPPORT_BRICK"));
						}
						else if (tool.Phase == LineTool.PHASE.END)
						{
							((LineToolDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.LINE_TOOL, exclusive: true))?.InitDialog(ref tool, currentBrick);
						}
					}
				}
			}
		}
	}

	private void CheckFire1Line()
	{
		if (!(null == tools) && UserMapInfoManager.Instance.CheckAuth(showMessage: false))
		{
			LineTool tool = tools.GetLineTool();
			if (tool != null)
			{
				Brick currentBrick = PaletteManager.Instance.GetCurrentBrick();
				if (currentBrick != null && currentBrick.IsEnable(RoomManager.Instance.CurrentRoomType))
				{
					Vector3 newBricksPos = GetNewBricksPos(currentBrick, hitBrick.normal, hitBrick.point);
					byte newBricksRot = GetNewBricksRot(currentBrick, hitBrick.normal);
					byte x = 0;
					byte y = 0;
					byte z = 0;
					if (BrickManager.Instance.ToCoord(newBricksPos, ref x, ref y, ref z))
					{
						if (custom_inputs.Instance.GetButtonDown("K_FIRE1"))
						{
							if (!currentBrick.IsUnit)
							{
								SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("LINE_TOOL_DOES_NOT_SUPPORT_BRICK"));
							}
							else if (currentBrick.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(currentBrick.ticket) >= 0)
							{
								switch (tool.Phase)
								{
								case LineTool.PHASE.NONE:
									tool.SetStart(newBricksPos, newBricksRot);
									break;
								case LineTool.PHASE.START:
									tool.SetEnd(newBricksPos);
									break;
								case LineTool.PHASE.END:
									((LineToolDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.LINE_TOOL, exclusive: true))?.InitDialog(ref tool, currentBrick);
									break;
								}
							}
							else
							{
								SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BUY_CREATIVE_TICKET"));
							}
						}
						else if (tool.Phase == LineTool.PHASE.START)
						{
							tool.SetPreview(newBricksPos);
						}
					}
				}
			}
		}
	}

	private void CheckFire1Replace()
	{
		if (custom_inputs.Instance.GetButtonDown("K_FIRE1") && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			Brick brick = PaletteManager.Instance.GetCurrentBrick();
			Brick hitBrickTemplate = BrickManager.Instance.GetHitBrickTemplate(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
			if (hitBrickTemplate != null && ((null != brick) & brick.IsEnable(RoomManager.Instance.CurrentRoomType)))
			{
				Brick.REPLACE_CHECK rEPLACE_CHECK = brick.CheckReplace();
				if (hitBrickTemplate.CheckReplace() != 0)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("CAN_NOT_REPLACE"));
				}
				else
				{
					if (rEPLACE_CHECK != 0 || hitBrickTemplate.seq == brick.seq)
					{
						brick = null;
					}
					((ReplaceToolDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.REPLACE_TOOL, exclusive: true))?.InitDialog(hitBrickTemplate, brick);
				}
			}
		}
	}

	private void CheckFire1(bool forceDown = false)
	{
		string key = (!GlobalVars.Instance.switchLRBuild) ? "K_FIRE1" : "K_FIRE2";
		if ((custom_inputs.Instance.GetButtonDown(key) || forceDown) && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			if (AutoComposer && !fire1click)
			{
				ElapsedAutoClick = 0f;
				fire1click = true;
				if (fire2click)
				{
					fire2click = false;
				}
			}
			else if (GlobalVars.Instance.IsFeverMode() && !feverAutoFire1Click)
			{
				ElapsedFeverAutoClick = 0f;
				feverAutoFire1Click = true;
				if (feverAutoFire2Click)
				{
					feverAutoFire2Click = false;
				}
			}
			Brick currentBrick = PaletteManager.Instance.GetCurrentBrick();
			if (currentBrick != null && currentBrick.IsEnable(RoomManager.Instance.CurrentRoomType))
			{
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND)
				{
					if (currentBrick.seq == 178)
					{
						statusMessage = StringMgr.Instance.Get("CMT_POTAL_MESSAGE_03");
						return;
					}
					if (MyInfoManager.Instance.Slot < 8 && currentBrick.seq == 164)
					{
						statusMessage = StringMgr.Instance.Get("CMT_POTAL_MESSAGE_02");
						return;
					}
					if (MyInfoManager.Instance.Slot >= 8 && currentBrick.seq == 163)
					{
						statusMessage = StringMgr.Instance.Get("CMT_POTAL_MESSAGE_02");
						return;
					}
				}
				if (!Application.loadedLevelName.Contains("Tutor") && currentBrick.ticket.Length > 0 && MyInfoManager.Instance.HaveFunction(currentBrick.ticket) < 0)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BUY_CREATIVE_TICKET"));
					MissFireSound();
				}
				else if (currentBrick.maxInstancePerMap > 0 && BrickManager.Instance.CountLimitedBrick(currentBrick.GetIndex()) >= currentBrick.maxInstancePerMap)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BRICK_COUNT_EXCEED_PRE") + currentBrick.maxInstancePerMap + StringMgr.Instance.Get("BRICK_COUNT_EXCEED_POST"));
					MissFireSound();
				}
				else
				{
					if (currentBrick.GetSpawnerType() != Brick.SPAWNER_TYPE.NONE)
					{
						GameObject gameObject = GameObject.Find("Main");
						string empty = string.Empty;
						if (currentBrick.GetSpawnerType() == Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER)
						{
							if (null != gameObject)
							{
								empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_01"), StringMgr.Instance.Get("N90_RED_TEAM_SPAWNER"), BrickManager.Instance.CountLimitedBrick(currentBrick.GetIndex()) + 1, currentBrick.maxInstancePerMap);
								gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
							}
						}
						else if (currentBrick.GetSpawnerType() == Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER)
						{
							if (null != gameObject)
							{
								empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_01"), StringMgr.Instance.Get("N90_BLUE_TEAM_SPAWNER"), BrickManager.Instance.CountLimitedBrick(currentBrick.GetIndex()) + 1, currentBrick.maxInstancePerMap);
								gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
							}
						}
						else if (currentBrick.GetSpawnerType() == Brick.SPAWNER_TYPE.SINGLE_SPAWNER && null != gameObject)
						{
							empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_01"), StringMgr.Instance.Get("N90_SINGLE_SPAWNER"), BrickManager.Instance.CountLimitedBrick(currentBrick.GetIndex()) + 1, currentBrick.maxInstancePerMap);
							gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
						}
					}
					if (currentBrick.function == Brick.FUNCTION.SCRIPT && !copyRight && !Application.loadedLevelName.Contains("Tutor"))
					{
						SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("SCRIPTABLE_DEVELOPER_ONLY"));
						MissFireSound();
					}
					else if (!(hitBrick.transform == null))
					{
						Brick hitBrickTemplate = BrickManager.Instance.GetHitBrickTemplate(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
						if (hitBrickTemplate != null)
						{
							Vector3 vector = GetNewBricksPos(currentBrick, hitBrick.normal, hitBrick.point);
							byte newBricksRot = GetNewBricksRot(currentBrick, hitBrick.normal);
							if (currentBrick.seq == 135 || currentBrick.seq == 136)
							{
								BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
								if (hitBrickProperty == null)
								{
									return;
								}
								BrickInst brickInst = BrickManager.Instance.GetBrickInst(hitBrickProperty.Seq);
								if (brickInst.Template == 134 || brickInst.Template == 135 || brickInst.Template == 136)
								{
									vector = GetNewBricksPos2(currentBrick, hitBrick.normal, new Vector3((float)(int)brickInst.PosX, (float)(int)brickInst.PosY, (float)(int)brickInst.PosZ));
								}
								else
								{
									GlobalVars.Instance.bAreaCheck = true;
									if (!CheckNeighborXZ(vector))
									{
										GlobalVars.Instance.bAreaCheck = false;
										return;
									}
									if (GlobalVars.Instance.bNeighborDefense)
									{
										vector = GlobalVars.Instance.neighborPoint;
										GlobalVars.Instance.bNeighborDefense = false;
									}
									GlobalVars.Instance.bAreaCheck = false;
								}
							}
							Vector3 myPos = Vector3.zero;
							bool flag = false;
							if (Application.loadedLevelName.Contains("Tutor"))
							{
								BrickInst byPos = BrickManager.Instance.GetByPos(vector);
								if (byPos != null)
								{
									if (byPos.Template == 180)
									{
										flag = true;
										GameObject brickObject = BrickManager.Instance.GetBrickObject(byPos.Seq);
										if (null != brickObject)
										{
											SenseTrigger componentInChildren = brickObject.GetComponentInChildren<SenseTrigger>();
											if (null != componentInChildren && componentInChildren.enabled)
											{
												componentInChildren.OnTriggerEnter(brickObject.collider);
											}
											else
											{
												GameObject gameObject2 = GameObject.Find("Me");
												LocalController localController = null;
												if (null != gameObject2)
												{
													localController = gameObject2.GetComponent<LocalController>();
												}
												if (GlobalVars.Instance.eventBridge && null != localController)
												{
													localController.addStatusMsg(StringMgr.Instance.Get("TUTO_SYS31"));
													return;
												}
												if (GlobalVars.Instance.eventGravity && null != localController)
												{
													localController.addStatusMsg(StringMgr.Instance.Get("TUTO_SYS33"));
													return;
												}
											}
										}
									}
								}
								else
								{
									GameObject gameObject3 = GameObject.Find("Me");
									LocalController localController2 = null;
									if (null != gameObject3)
									{
										localController2 = gameObject3.GetComponent<LocalController>();
									}
									if (GlobalVars.Instance.eventBridge && null != localController2)
									{
										localController2.addStatusMsg(StringMgr.Instance.Get("TUTO_SYS31"));
										return;
									}
									if (GlobalVars.Instance.eventGravity && null != localController2)
									{
										localController2.addStatusMsg(StringMgr.Instance.Get("TUTO_SYS33"));
										return;
									}
								}
							}
							GameObject gameObject4 = GameObject.Find("Me");
							if (null != gameObject4)
							{
								LocalController component = gameObject4.GetComponent<LocalController>();
								if (component != null)
								{
									myPos = component.transform.position;
								}
							}
							bool flag2 = false;
							if ((!IsCoolDown() && BrickManager.Instance.IsEmpty2(currentBrick, vector, brickOnly: false) && BrickManager.Instance.IsBoxIn(currentBrick, hitBrick.point, myPos)) || flag)
							{
								byte x = 0;
								byte y = 0;
								byte z = 0;
								if (Application.loadedLevelName.Contains("Tutor"))
								{
									if (BrickManager.Instance.ToCoord(vector, ref x, ref y, ref z))
									{
										if (!BrickManager.Instance.checkAddMinMaxGravity(currentBrick.seq))
										{
											return;
										}
										Brick brick = BrickManager.Instance.GetBrick(currentBrick.seq);
										if (brick != null && brick.onlyTutor)
										{
											CreateMuzzleFire();
											FireSound();
											base.animation.CrossFade("fire");
											deltaTime = 0f;
											flag2 = true;
											MyInfoManager.Instance.IsModified = true;
											BrickManager.Instance.AddBrickCreator(tutoBrickIndex++, (byte)currentBrick.seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), newBricksRot);
										}
									}
								}
								else if (CSNetManager.Instance.Sock != null && !CSNetManager.Instance.Sock.WaitingAck && BrickManager.Instance.ToCoord(vector, ref x, ref y, ref z))
								{
									if (!BrickManager.Instance.checkAddMinMaxGravity(currentBrick.seq))
									{
										return;
									}
									CreateMuzzleFire();
									FireSound();
									P2PManager.Instance.SendPEER_COMPOSE(isDel: false);
									CSNetManager.Instance.Sock.SendCS_ADD_BRICK_REQ(currentBrick.GetIndex(), x, y, z, newBricksRot);
									base.animation.CrossFade("fire");
									deltaTime = 0f;
									flag2 = true;
								}
							}
							if (!flag2)
							{
								MissFireSound();
							}
						}
					}
				}
			}
		}
	}

	private void DelFireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip clipOutSound = GetComponent<Weapon>().clipOutSound;
			if (null != component && null != clipOutSound)
			{
				component.PlayOneShot(clipOutSound);
			}
		}
	}

	private void MissFireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip dryFireSound = GetComponent<Weapon>().dryFireSound;
			if (null != component && null != dryFireSound)
			{
				component.PlayOneShot(dryFireSound);
			}
		}
	}

	private void FireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip fireSound = GetComponent<Weapon>().fireSound;
			if (null != component && null != fireSound)
			{
				component.PlayOneShot(fireSound);
			}
		}
	}

	private void CheckScript()
	{
		if (custom_inputs.Instance.GetButtonDown("K_ACTION") && copyRight)
		{
			BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
			if (null != hitBrickProperty)
			{
				Brick brick = BrickManager.Instance.GetBrick(hitBrickProperty.Index);
				BrickInst brickInst = BrickManager.Instance.GetBrickInst(hitBrickProperty.Seq);
				if (brick != null && brickInst != null && brick.function == Brick.FUNCTION.SCRIPT)
				{
					((ScriptEditor)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SCRIPT_EDITOR, exclusive: true))?.InitDialog(hitBrickProperty, brick, brickInst);
				}
			}
		}
	}

	private void CheckFire3()
	{
		if (!Application.loadedLevelName.Contains("Tutor") && custom_inputs.Instance.GetButtonDown("K_FIRE3") && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
			if (null != hitBrickProperty)
			{
				Brick brick = BrickManager.Instance.GetBrick(hitBrickProperty.Index);
				if (brick != null)
				{
					if (brick.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(brick.ticket) >= 0)
					{
						PaletteManager.Instance.SetCurrentPalette(brick);
					}
					else
					{
						SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BUY_CREATIVE_TICKET"));
					}
				}
			}
		}
	}

	private void CheckFire2(bool forceDown = false)
	{
		if (!GlobalVars.Instance.blockDelBrick)
		{
			string key = (!GlobalVars.Instance.switchLRBuild) ? "K_FIRE2" : "K_FIRE1";
			if ((custom_inputs.Instance.GetButtonDown(key) || forceDown) && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
			{
				if (AutoComposer && !fire2click)
				{
					ElapsedAutoClick = 0f;
					fire2click = true;
					if (fire1click)
					{
						fire1click = false;
					}
				}
				else if (GlobalVars.Instance.IsFeverMode() && !feverAutoFire2Click)
				{
					ElapsedFeverAutoClick = 0f;
					feverAutoFire2Click = true;
					if (feverAutoFire1Click)
					{
						feverAutoFire1Click = false;
					}
				}
				if (!(hitBrick.transform == null))
				{
					BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
					bool flag = false;
					if (!IsCoolDown() && hitBrickProperty != null)
					{
						Brick brick = BrickManager.Instance.GetBrick(hitBrickProperty.Index);
						if (brick != null && brick.IsEnable(RoomManager.Instance.CurrentRoomType))
						{
							if (Application.loadedLevelName.Contains("Tutor"))
							{
								if (brick.onlyTutor)
								{
									deltaTime = 0f;
									flag = true;
									DelFireSound();
									CreateMuzzleFire();
									MyInfoManager.Instance.IsModified = true;
									BrickManager.Instance.DelBrick(hitBrickProperty.Seq, shrink: true);
									base.animation.CrossFade("fire");
								}
							}
							else if (brick.function == Brick.FUNCTION.SCRIPT && !copyRight)
							{
								MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SCRIPTABLE_DEVELOPER_ONLY"));
							}
							else if (CSNetManager.Instance.Sock != null && !CSNetManager.Instance.Sock.WaitingAck)
							{
								deltaTime = 0f;
								flag = true;
								DelFireSound();
								CreateMuzzleFire();
								P2PManager.Instance.SendPEER_COMPOSE(isDel: true);
								CSNetManager.Instance.Sock.SendCS_DEL_BRICK_REQ(hitBrickProperty.Seq);
								base.animation.CrossFade("fire");
							}
						}
					}
					if (!flag)
					{
						MissFireSound();
					}
				}
			}
		}
	}

	private void Update()
	{
		VerifyEditorTool();
		VerifyBattleChat();
		VerifyCamera();
		VerifyLocalController();
		updateAutoFeverKey();
		updateAutoBuildGun();
		deltaTime += Time.deltaTime;
		PaletteManager.Instance.CheckShortCut();
		PaletteManager.Instance.CheckDragAndDrop();
		bool flag = false;
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			flag = battleChat.IsChatting;
		}
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE && !Application.isLoadingLevel && !flag && custom_inputs.Instance.GetButtonDown("K_BRICK_MENU"))
		{
			TutoInput component = GameObject.Find("Main").GetComponent<TutoInput>();
			if (PaletteManager.Instance.MenuOn)
			{
				if (component != null)
				{
					component.setChangePos(set: false, new Vector2(0f, 0f));
					component.setActive(TUTO_INPUT.M_L);
					component.setClick(TUTO_INPUT.M_L);
				}
				PaletteManager.Instance.Switch(on: false);
			}
			else if (!DialogManager.Instance.IsModal)
			{
				PaletteManager.Instance.Switch(on: true);
				if (component != null)
				{
					component.setActive(TUTO_INPUT.M_L);
					component.setClick(TUTO_INPUT.M_L);
					float x = ((float)Screen.width - PaletteManager.Instance.crdSiloArea.x) / 2f + 4f;
					float y = ((float)Screen.height - PaletteManager.Instance.crdSiloArea.y) / 2f + 29f;
					component.setChangePos(set: true, new Vector2(x, y));
					component.setBlick(set: true);
					component.setBlickPos(new Rect(x, y, 64f, 64f));
				}
			}
		}
		if (feverAutoFire1Click || fire1click || feverAutoFire2Click || fire2click)
		{
			if (battleChat != null && battleChat.IsChatting)
			{
				feverAutoFire1Click = false;
				fire1click = false;
				feverAutoFire2Click = false;
				fire2click = false;
			}
			if (DialogManager.Instance.IsModal)
			{
				feverAutoFire1Click = false;
				fire1click = false;
				feverAutoFire2Click = false;
				fire2click = false;
			}
		}
		if (Screen.lockCursor && BrickManager.Instance.IsLoaded)
		{
			PreCheckBuildGun();
			if (!CheckAimed())
			{
				ShowTarget(show: false);
			}
			else
			{
				ShowTarget(show: true);
				CheckBuildGun();
			}
			CheckFireUp();
			if ((feverAutoFire1Click || fire1click || feverAutoFire2Click || fire2click) && GlobalVars.Instance.EscapeKey)
			{
				feverAutoFire1Click = false;
				fire1click = false;
				feverAutoFire2Click = false;
				fire2click = false;
				GlobalVars.Instance.EscapeKey = false;
			}
			if (statusMessage.Length > 0)
			{
				statusDelta += Time.deltaTime;
				if (statusDelta > statusMessageLimit)
				{
					statusDelta = 0f;
					statusMessage = string.Empty;
				}
			}
		}
	}

	private void PreCheckBuildGun()
	{
		string text = (!(null == tools)) ? tools.GetActiveEditorTool() : "build_tool";
		switch (text)
		{
		case "build_tool":
			_PreCheckBuildTool();
			break;
		case "line_tool":
			_PreCheckLineTool();
			break;
		case "replace_tool":
			_PreCheckReplaceTool();
			break;
		}
	}

	private bool CanFire()
	{
		return null != localCtrl && localCtrl.CanControl();
	}

	private void CheckBuildGun()
	{
		if (CanFire())
		{
			string text = (!(null == tools)) ? tools.GetActiveEditorTool() : "build_tool";
			switch (text)
			{
			case "build_tool":
				_CheckBuildTool();
				break;
			case "line_tool":
				_CheckLineTool();
				break;
			case "replace_tool":
				_CheckReplaceTool();
				break;
			}
		}
	}

	private void _PreCheckBuildTool()
	{
	}

	private void _PreCheckLineTool()
	{
		PreCheckFire1Line();
		PreCheckFire2Line();
	}

	private void _PreCheckReplaceTool()
	{
	}

	private void _CheckBuildTool()
	{
		CheckFire1();
		CheckFire2();
		CheckFire3();
		CheckScript();
	}

	private void _CheckLineTool()
	{
		CheckFire1Line();
	}

	private void _CheckReplaceTool()
	{
		CheckFire1Replace();
	}

	private void CheckFireUp()
	{
		if (!GlobalVars.Instance.switchLRBuild)
		{
			if (custom_inputs.Instance.GetButtonUp("K_FIRE1"))
			{
				feverAutoFire1Click = false;
				fire1click = false;
			}
			if (custom_inputs.Instance.GetButtonUp("K_FIRE2"))
			{
				feverAutoFire2Click = false;
				fire2click = false;
			}
		}
		else
		{
			if (custom_inputs.Instance.GetButtonUp("K_FIRE2"))
			{
				feverAutoFire1Click = false;
				fire1click = false;
			}
			if (custom_inputs.Instance.GetButtonUp("K_FIRE1"))
			{
				feverAutoFire2Click = false;
				fire2click = false;
			}
		}
	}

	private void updateAutoBuildGun()
	{
		if (AutoComposer)
		{
			if (fire1click)
			{
				ElapsedAutoClick += Time.deltaTime;
				if (ElapsedAutoClick >= 0.1f)
				{
					CheckFire1(forceDown: true);
					ElapsedAutoClick = 0f;
				}
			}
			if (fire2click)
			{
				ElapsedAutoClick += Time.deltaTime;
				if (ElapsedAutoClick >= 0.1f)
				{
					CheckFire2(forceDown: true);
					ElapsedAutoClick = 0f;
				}
			}
		}
	}

	private void updateAutoFeverKey()
	{
		if (feverAutoFire1Click)
		{
			ElapsedFeverAutoClick += Time.deltaTime;
			if (ElapsedFeverAutoClick >= 0.1f)
			{
				CheckFire1(forceDown: true);
				ElapsedFeverAutoClick = 0f;
			}
		}
		if (feverAutoFire2Click)
		{
			ElapsedFeverAutoClick += Time.deltaTime;
			if (ElapsedFeverAutoClick >= 0.1f)
			{
				CheckFire2(forceDown: true);
				ElapsedFeverAutoClick = 0f;
			}
		}
	}
}
