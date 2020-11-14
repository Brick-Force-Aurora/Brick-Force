public class ResultUnitEscape : ResultUnit
{
	public int param;

	public ResultUnitEscape(bool _red, int _seq, string _nickname, int _kill, int _death, int _assist, int _score, int _point, int _xp, int _mission, int _prevXp, int _nextXp, long _buff, int _param)
		: base(_red, _seq, _nickname, _kill, _death, _assist, _score, _point, _xp, _mission, _prevXp, _nextXp, _buff)
	{
		param = _param;
	}

	public int Compare(ResultUnitEscape ru)
	{
		if (kill == ru.kill)
		{
			if (score == ru.score)
			{
				return -seq.CompareTo(ru.seq);
			}
			return -score.CompareTo(ru.score);
		}
		return -kill.CompareTo(ru.kill);
	}
}
