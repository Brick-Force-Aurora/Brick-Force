public class CMPlayer
{
	private int xp;

	private int rank;

	private string nickname;

	private int kill;

	private int assist;

	private int death;

	private int score;

	public int Xp => xp;

	public int Rank => rank;

	public string Nickname => nickname;

	public string Record => kill.ToString() + "/" + assist.ToString() + "/" + death.ToString();

	public string Score => score.ToString();

	public CMPlayer(int _xp, int _rank, string _nickname, int _kill, int _assist, int _death, int _score)
	{
		xp = _xp;
		rank = _rank;
		nickname = _nickname;
		kill = _kill;
		assist = _assist;
		death = _death;
		score = _score;
	}
}
