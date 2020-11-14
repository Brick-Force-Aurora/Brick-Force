public class BrickInst
{
	public int Seq;

	public byte Template;

	public byte PosX;

	public byte PosY;

	public byte PosZ;

	public ushort Code;

	public byte Rot;

	public BfScript BrickForceScript;

	public int pathcnt;

	public BrickInst(int seq, byte template, byte x, byte y, byte z, ushort code, byte rot)
	{
		Seq = seq;
		Template = template;
		PosX = x;
		PosY = y;
		PosZ = z;
		Rot = rot;
		Code = code;
		BrickForceScript = null;
		pathcnt = 0;
	}

	public void UpdateScript(string alias, bool enableOnAwake, bool visibleOnAwake, string commands)
	{
		BrickForceScript = new BfScript(alias, enableOnAwake, visibleOnAwake, commands);
	}
}
