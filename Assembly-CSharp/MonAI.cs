using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonAI : MonoBehaviour
{
	public enum MON_TYPE
	{
		NORMAL,
		FLASH,
		HIDE,
		HEAL,
		BOSS
	}

	private class CTrace
	{
		public int id;

		public int seq;

		public CTrace(int _id, int _seq)
		{
			id = _id;
			seq = _seq;
		}
	}

	private MON_TYPE monType;

	public Vector3 vStart;

	public Vector3 vTarget;

	protected GameObject eff;

	public bool bMoving;

	public ArrayList traces;

	public ArrayList paths;

	public bool bLastTarget;

	public bool bArrived;

	public bool IsUpDown;

	private bool bHitMark;

	private float timerHitMark;

	public float rigidity;

	public float flyHeight = 5f;

	public float moveSpeed = 2f;

	public LocalController localCtrl;

	public MonController monCtrl;

	public MonProperty monProp;

	public Vector3 vCur = Vector3.zero;

	public float deltaTime;

	public float shootDelay;

	public float maxShootDelay = 5f;

	public bool bDie;

	public float timerWait;

	private int readyP2P;

	public float timerSendP2PMove;

	public SkinnedMeshRenderer smr;

	public Color orgColor;

	public bool bRedTeam;

	private Vector3 prevPos = Vector3.zero;

	private float dtPrevPosDelta;

	private float dtPrevPosMax = 3f;

	private float deltaTimeColHit;

	private int created;

	private Vector3 vReset = Vector3.zero;

	public MON_TYPE MonType
	{
		get
		{
			return monType;
		}
		set
		{
			monType = value;
		}
	}

	public Vector3 StartPosition
	{
		get
		{
			return vStart;
		}
		set
		{
			vStart = value;
			vStart.y += flyHeight;
			vCur = value;
			vCur.y += flyHeight;
		}
	}

	public Vector3 TargetPosition
	{
		set
		{
			vTarget = value;
		}
	}

	public virtual void Die()
	{
		bDie = true;
		if (monType == MON_TYPE.FLASH)
		{
			GlobalVars.Instance.SwitchFlashbang(bVis: true, base.transform.position, ignoreDistance: true);
		}
	}

	private void Start()
	{
		deltaTime = 0f;
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		if (null == smr)
		{
			Debug.LogError("Fail to get skinned mesh renderer for flags");
		}
		else if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				if (null == skinnedMeshRenderer)
				{
					Debug.LogError("Fail to get skinned mesh renderer for flags");
				}
				else if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
				{
					skinnedMeshRenderer.enabled = false;
				}
			}
		}
		orgColor = smr.material.color;
		GameObject gameObject = GameObject.Find("Me");
		if ((bool)gameObject)
		{
			localCtrl = gameObject.GetComponent<LocalController>();
			if (localCtrl == null)
			{
				Debug.LogError("[MonAI.cs] localCtrl is null");
			}
		}
		monProp = GetComponent<MonProperty>();
		if (monProp == null)
		{
			Debug.LogError("[MonAI.cs] MonProperty is null");
		}
		monCtrl = GetComponent<MonController>();
		if ((bool)(monCtrl = null))
		{
			Debug.LogError("[MonAI.cs] MonController is null");
		}
		traces = new ArrayList();
		paths = new ArrayList();
		changeTexture();
	}

	public void Reset()
	{
		bDie = false;
		base.transform.up = new Vector3(0f, 1f, 0f);
		MonController component = GetComponent<MonController>();
		component.Reset();
	}

	public void CheckColHit()
	{
		if (monProp.Desc.colHit)
		{
			deltaTimeColHit += Time.deltaTime;
			if (deltaTimeColHit > 0.5f)
			{
				deltaTimeColHit = 0f;
				monProp.Desc.colHit = false;
			}
		}
	}

	private void CheckHit()
	{
		if (monProp.Desc.IsHit)
		{
			bHitMark = true;
			timerHitMark = 0f;
			monProp.Desc.IsHit = false;
		}
	}

	public void ApplyRigidity()
	{
		monProp.Desc.rigidity = Mathf.Lerp(monProp.Desc.rigidity, 0f, Time.deltaTime);
	}

	public float CalcRigidSpeed(float speed)
	{
		if (speed < 0f)
		{
			return 0f;
		}
		float num = 1f - monProp.Desc.rigidity;
		if (num < 0f)
		{
			num = 0f;
		}
		return speed * num;
	}

	private bool CheckTraces(int seq, int template)
	{
		if (template != 135 && template != 136)
		{
			return true;
		}
		for (int i = 0; i < traces.Count; i++)
		{
			CTrace cTrace = (CTrace)traces[i];
			if (cTrace.seq == seq)
			{
				return true;
			}
		}
		if (template == 136)
		{
			bLastTarget = true;
		}
		return false;
	}

	public virtual void changeTexture()
	{
	}

	public bool FindWay(Vector3 cen)
	{
		cen.y -= flyHeight;
		Vector3 vector = cen;
		BrickInst byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos == null)
		{
			Debug.LogError("bi == null");
		}
		Vector3 item = cen;
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		vector.x -= 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		vector = cen;
		vector.x += 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		vector = cen;
		vector.z -= 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		vector = cen;
		vector.z += 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		vector = cen;
		vector.y -= 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		vector = cen;
		vector.y += 3f;
		byPos = BrickManager.Instance.GetByPos(vector);
		if (byPos != null && !CheckTraces(byPos.Seq, byPos.Template))
		{
			item = vector;
			item.y += flyHeight;
			list.Add(item);
			list2.Add(byPos.Seq);
		}
		if (list.Count == 0)
		{
			EraseWay(cen);
			return false;
		}
		if (list.Count == 1)
		{
			if (readyP2P == 0)
			{
				readyP2P = 1;
			}
			vTarget = list[0];
			traces.Add(new CTrace(created++, list2[0]));
		}
		else
		{
			List<int> list3 = new List<int>();
			Vector3 vDefenseEnd = GlobalVars.Instance.vDefenseEnd;
			int num = 65535;
			int num2 = -1;
			for (int i = 0; i < list.Count; i++)
			{
				int num3 = (int)Vector3.Distance(vDefenseEnd, list[i]);
				if (num > num3)
				{
					num = num3;
					num2 = i;
				}
			}
			list3.Add(num2);
			for (int j = 0; j < list.Count; j++)
			{
				if (j != num2)
				{
					int num4 = (int)Vector3.Distance(vDefenseEnd, list[j]);
					if (num == num4)
					{
						list3.Add(j);
					}
				}
			}
			if (list3.Count == 1)
			{
				if (readyP2P == 0)
				{
					readyP2P = 1;
				}
				vTarget = list[list3[0]];
				traces.Add(new CTrace(created++, list2[list3[0]]));
			}
			else
			{
				int index = Random.Range(0, list3.Count);
				vTarget = list[index];
				traces.Add(new CTrace(created++, list2[index]));
				if (readyP2P == 0)
				{
					readyP2P = 1;
				}
			}
		}
		if (Mathf.Abs(vTarget.y - vCur.y) > 1f)
		{
			IsUpDown = true;
		}
		return true;
	}

	private void EraseTrace(int eraseId)
	{
		int num = -1;
		for (int i = 0; i < traces.Count; i++)
		{
			CTrace cTrace = (CTrace)traces[i];
			if (cTrace.id == eraseId)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			traces.RemoveAt(num);
		}
		else
		{
			Debug.LogError("(EraseTrace) not found: " + eraseId);
		}
	}

	private int traceCreatedID(int seq)
	{
		for (int i = 0; i < traces.Count; i++)
		{
			CTrace cTrace = (CTrace)traces[i];
			if (cTrace.seq == seq)
			{
				return cTrace.id;
			}
		}
		Debug.LogError("(traceCreatedID) not found: " + seq);
		return -1;
	}

	public void EraseWay(Vector3 cen)
	{
		Vector3 pos = cen;
		BrickInst byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos == null)
		{
			Debug.LogError("bi == null");
		}
		int num = -1;
		pos.x -= 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null)
		{
			num = traceCreatedID(byPos.Seq);
		}
		pos = cen;
		pos.x += 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null && (num == -1 || num > traceCreatedID(byPos.Seq)))
		{
			num = traceCreatedID(byPos.Seq);
		}
		pos = cen;
		pos.z -= 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null && (num == -1 || num > traceCreatedID(byPos.Seq)))
		{
			num = traceCreatedID(byPos.Seq);
		}
		pos = cen;
		pos.z += 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null && (num == -1 || num > traceCreatedID(byPos.Seq)))
		{
			num = traceCreatedID(byPos.Seq);
		}
		pos = cen;
		pos.y -= 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null && (num == -1 || num > traceCreatedID(byPos.Seq)))
		{
			num = traceCreatedID(byPos.Seq);
		}
		pos = cen;
		pos.y += 3f;
		byPos = BrickManager.Instance.GetByPos(pos);
		if (byPos != null && (num == -1 || num > traceCreatedID(byPos.Seq)))
		{
			num = traceCreatedID(byPos.Seq);
		}
		if (num == -1)
		{
			Debug.LogError("(EraseWay)Perfect Error");
		}
		else
		{
			EraseTrace(num);
		}
	}

	public void MoveP2P(float x, float y, float z, float viewx, float viewy, float viewz)
	{
		vStart = vCur;
		vTarget.x = x;
		vTarget.y = y;
		vTarget.z = z;
		deltaTime = 0f;
		Vector3 forward = new Vector3(viewx, viewy, viewz);
		base.transform.forward = forward;
		if (readyP2P == 0)
		{
			readyP2P = 1;
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				if (null != skinnedMeshRenderer)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
		}
	}

	public virtual void ActiveHealEff()
	{
		eff = (Object.Instantiate((Object)MonManager.Instance.healEff, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
	}

	public virtual void ActiveHealerEff()
	{
	}

	public virtual void LastCommand()
	{
	}

	public void ResetMove()
	{
		vReset = BrickManager.Instance.GetNearstDefenseBrickPos(base.transform.position);
		if (vReset == Vector3.zero)
		{
			Debug.LogError("BRICK not found");
		}
		else
		{
			vReset.y += flyHeight;
			bMoving = false;
			deltaTime = 0f;
			vStart = vReset;
			base.transform.position = vReset;
			vCur = vReset;
		}
	}

	public virtual void updateSkill()
	{
	}

	public virtual void updateHide()
	{
	}

	public virtual void updateSelfHeal()
	{
	}

	public virtual void updateAreaHeal()
	{
	}

	private Vector3 UpdateAIP2P()
	{
		Vector3 vector = vCur;
		float magnitude = (vTarget - vStart).magnitude;
		float num = magnitude / moveSpeed;
		deltaTime += Time.deltaTime;
		float num2 = deltaTime / num;
		if (num2 >= 1f)
		{
			num2 = 1f;
		}
		return Vector3.Lerp(vStart, vTarget, num2);
	}

	private Vector3 UpdateAI()
	{
		Vector3 vector = vCur;
		ApplyRigidity();
		float num = CalcRigidSpeed(moveSpeed);
		if (!bMoving)
		{
			if (!FindWay(vector))
			{
				FindWay(vector);
			}
			bMoving = true;
			if (!IsUpDown)
			{
				Vector3 forward = vTarget - vStart;
				forward.Normalize();
				base.transform.forward = forward;
			}
			if (readyP2P == 1 && MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				readyP2P = 2;
				P2PManager instance = P2PManager.Instance;
				int tblID = monProp.Desc.tblID;
				int typeID = monProp.Desc.typeID;
				int seq = monProp.Desc.Seq;
				float x = vCur.x;
				float y = vCur.y;
				float z = vCur.z;
				Vector3 forward2 = base.transform.forward;
				float x2 = forward2.x;
				Vector3 forward3 = base.transform.forward;
				float y2 = forward3.y;
				Vector3 forward4 = base.transform.forward;
				instance.SendPEER_MON_GEN(tblID, typeID, seq, x, y, z, x2, y2, forward4.z);
			}
		}
		else
		{
			float num2 = Vector3.Distance(vTarget, vStart);
			deltaTime += Time.deltaTime;
			float num3 = deltaTime * (num / num2);
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			vector = Vector3.Lerp(vStart, vTarget, num3);
			if (!IsUpDown)
			{
				Vector3 forward5 = vTarget - vStart;
				forward5.Normalize();
				base.transform.forward = forward5;
			}
			if (num3 >= 1f)
			{
				deltaTime = 0f;
				bMoving = false;
				IsUpDown = false;
				vStart = vTarget;
				if (bLastTarget)
				{
					rigidity = 0f;
					bArrived = true;
				}
			}
		}
		return vector;
	}

	private void CheckPositionPrev()
	{
		dtPrevPosDelta += Time.deltaTime;
		if (dtPrevPosDelta > dtPrevPosMax)
		{
			dtPrevPosDelta = 0f;
			prevPos = base.transform.position;
		}
		if (prevPos == base.transform.position)
		{
			base.transform.position = GlobalVars.Instance.vDefenseEnd;
		}
	}

	private void Update()
	{
		if (BrickManager.Instance.IsLoaded && !bDie && (MyInfoManager.Instance.Seq == RoomManager.Instance.Master || readyP2P != 0))
		{
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				vCur = UpdateAI();
			}
			else
			{
				vCur = UpdateAIP2P();
			}
			base.transform.position = vCur;
			CheckPositionPrev();
			if (bHitMark)
			{
				timerHitMark += Time.deltaTime;
				float num = timerHitMark;
				num *= 4f;
				if (num > 1f)
				{
					num = 2f - num;
				}
				float num2 = Mathf.Lerp(0f, orgColor.g, num);
				float num3 = Mathf.Lerp(0f, orgColor.b, num);
				Material material = smr.material;
				float g = orgColor.g - num2;
				float b = orgColor.b - num3;
				Color color = smr.material.color;
				material.color = new Color(1f, g, b, color.a);
				if (timerHitMark > 0.5f)
				{
					Material material2 = smr.material;
					float r = orgColor.r;
					float g2 = orgColor.g;
					float b2 = orgColor.b;
					Color color2 = smr.material.color;
					material2.color = new Color(r, g2, b2, color2.a);
					bHitMark = false;
				}
			}
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master && readyP2P == 2)
			{
				timerSendP2PMove += Time.deltaTime;
				if (timerSendP2PMove >= 0.4f)
				{
					timerSendP2PMove = 0f;
					P2PManager instance = P2PManager.Instance;
					int seq = monProp.Desc.Seq;
					float x = vCur.x;
					float y = vCur.y;
					float z = vCur.z;
					Vector3 forward = base.transform.forward;
					float x2 = forward.x;
					Vector3 forward2 = base.transform.forward;
					float y2 = forward2.y;
					Vector3 forward3 = base.transform.forward;
					instance.SendPEER_MON_MOVE(seq, x, y, z, x2, y2, forward3.z);
				}
			}
			CheckColHit();
			CheckHit();
			if (eff != null)
			{
				eff.transform.position = base.transform.position;
			}
			switch (monType)
			{
			case MON_TYPE.FLASH:
				updateSkill();
				break;
			case MON_TYPE.HIDE:
				updateHide();
				break;
			case MON_TYPE.HEAL:
				updateSelfHeal();
				break;
			case MON_TYPE.BOSS:
				updateAreaHeal();
				break;
			}
			if (bArrived)
			{
				MonProperty component = GetComponent<MonProperty>();
				if (component.Desc.tblID == 4)
				{
					MonManager.Instance.BossUnVisibleAll(component.Desc.bRedTeam);
				}
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
				{
					P2PManager.Instance.SendPEER_MON_DIE(component.Desc.Seq, arrived: true);
					if (component.Desc.bRedTeam)
					{
						DefenseManager.Instance.CoreLifeBlue -= monProp.Desc.coreToDmg;
					}
					else
					{
						DefenseManager.Instance.CoreLifeRed -= monProp.Desc.coreToDmg;
					}
					CSNetManager.Instance.Sock.SendCS_CORE_HP_REQ(DefenseManager.Instance.CoreLifeRed, DefenseManager.Instance.CoreLifeBlue);
				}
				MonManager.Instance.Remove(component.Desc.Seq);
				bDie = true;
			}
		}
	}
}
