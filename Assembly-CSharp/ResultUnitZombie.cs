public class ResultUnitZombie : ResultUnit
{
	public int winCount;

	public ResultUnitZombie(bool _red, int _seq, string _nickname, int _kill, int _death, int _assist, int _score, int _point, int _xp, int _mission, int _prevXp, int _nextXp, long _buff)
		: base(_red, _seq, _nickname, _kill, _death, _assist, _score, _point, _xp, _mission, _prevXp, _nextXp, _buff)
	{
		winCount = _kill + _death;
	}

	public int Compare(ResultUnitZombie ru)
	{
		if (score == ru.score)
		{
			if (winCount == ru.winCount)
			{
				return -seq.CompareTo(ru.seq);
			}
			return -winCount.CompareTo(ru.winCount);
		}
		return -score.CompareTo(ru.score);
	}
}
