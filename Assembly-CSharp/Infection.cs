public class Infection
{
	private int host;

	private int newZombie;

	public int Host => host;

	public int NewZombie => newZombie;

	public Infection(int _host, int _newZombie)
	{
		host = _host;
		newZombie = _newZombie;
	}
}
