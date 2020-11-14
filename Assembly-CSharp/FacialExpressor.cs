using UnityEngine;

public class FacialExpressor : MonoBehaviour
{
	public enum EXPRESSION
	{
		DEFAULT,
		ANGRY,
		CRY,
		DEATH,
		DIZZY,
		FEAR,
		LAUGH,
		LOSS,
		LOVE,
		NONSENSE,
		RAGE,
		SHOUT,
		SHY,
		SIGH,
		SUPRISED
	}

	private SkinnedMeshRenderer smrFace;

	public ExpressionSet[] expression;

	private PlayerProperty pp;

	private EXPRESSION curExpression;

	private ExpressionSet curExprSet;

	private EXPRESSION[] zombieExpression = new EXPRESSION[15]
	{
		EXPRESSION.ANGRY,
		EXPRESSION.ANGRY,
		EXPRESSION.ANGRY,
		EXPRESSION.DEATH,
		EXPRESSION.DIZZY,
		EXPRESSION.FEAR,
		EXPRESSION.DIZZY,
		EXPRESSION.LOSS,
		EXPRESSION.ANGRY,
		EXPRESSION.NONSENSE,
		EXPRESSION.RAGE,
		EXPRESSION.SHOUT,
		EXPRESSION.DIZZY,
		EXPRESSION.DIZZY,
		EXPRESSION.SUPRISED
	};

	private void Start()
	{
	}

	public void ChangeFace(SkinnedMeshRenderer smr, string setName)
	{
		ExpressionSet expressionSet = null;
		int num = 0;
		while (expressionSet == null && num < expression.Length)
		{
			if (expression[num].name == setName)
			{
				expressionSet = expression[num];
			}
			num++;
		}
		curExprSet = expressionSet;
		if (curExprSet == null)
		{
			Debug.LogError("ERROR, Fail to get Expression Set for " + setName);
		}
		else
		{
			smrFace = smr;
			smr.material = curExprSet.material[(int)curExpression];
		}
		SetExpression(EXPRESSION.DEFAULT);
	}

	private void OnFeel(EXPRESSION expr)
	{
		SetExpression(expr);
	}

	private void SetExpression(EXPRESSION expr)
	{
		EXPRESSION eXPRESSION = ConvertExpression4Zombie(expr);
		if (curExprSet.material.Length > (int)eXPRESSION)
		{
			curExpression = eXPRESSION;
			if (smrFace != null)
			{
				smrFace.material = curExprSet.material[(int)eXPRESSION];
			}
		}
	}

	private void VerifyPlayerProperty()
	{
		if (pp == null)
		{
			pp = GetComponent<PlayerProperty>();
		}
	}

	private EXPRESSION ConvertExpression4Zombie(EXPRESSION expr)
	{
		EXPRESSION result = expr;
		VerifyPlayerProperty();
		if (BuildOption.Instance.Props.IsSupportMode(Room.ROOM_TYPE.ZOMBIE) && RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && null != pp && pp.Desc != null && ZombieVsHumanManager.Instance.IsZombie(pp.Desc.Seq) && EXPRESSION.DEFAULT <= expr && (int)expr < zombieExpression.Length)
		{
			result = zombieExpression[(int)expr];
		}
		return result;
	}
}
