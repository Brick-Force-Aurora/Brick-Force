public class RankMap
{
	private int rowNo;

	private int rank;

	private int rankChg;

	private RegMap regMap;

	public int RowNo => rowNo;

	public int Rank => rank;

	public int RankChg => rankChg;

	public RegMap OrgMap => regMap;

	public bool IsLatest => regMap.IsLatest;

	public RankMap(int rn, int rk, int rc, RegMap mp)
	{
		rowNo = rn;
		rank = rk;
		rankChg = rc;
		regMap = mp;
	}
}
