public class ResultUnit
{
	public bool red;

	public int seq;

	public string nickname;

	public int kill;

	public int death;

	public int assist;

	public int score;

	public int point;

	public int xp;

	public int mission;

	public int prevXp;

	public int nextXp;

	public long buff;

	public ResultUnit(bool _red, int _seq, string _nickname, int _kill, int _death, int _assist, int _score, int _point, int _xp, int _mission, int _prevXp, int _nextXp, long _buff)
	{
		red = _red;
		seq = _seq;
		nickname = _nickname;
		kill = _kill;
		death = _death;
		assist = _assist;
		score = _score;
		point = _point;
		xp = _xp;
		mission = _mission;
		prevXp = _prevXp;
		nextXp = _nextXp;
		buff = _buff;
	}

	public int Compare(ResultUnit ru)
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
