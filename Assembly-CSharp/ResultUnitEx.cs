public class ResultUnitEx : ResultUnit
{
	public int param;

	public ResultUnitEx(bool _red, int _seq, string _nickname, int _kill, int _death, int _assist, int _score, int _point, int _xp, int _mission, int _prevXp, int _nextXp, long _buff, int _param)
		: base(_red, _seq, _nickname, _kill, _death, _assist, _score, _point, _xp, _mission, _prevXp, _nextXp, _buff)
	{
		param = _param;
	}

	public int Compare(ResultUnitEx ru)
	{
		if (score == ru.score)
		{
			if (kill == ru.kill)
			{
				return death.CompareTo(ru.death);
			}
			return -kill.CompareTo(ru.kill);
		}
		return -score.CompareTo(ru.score);
	}
}
