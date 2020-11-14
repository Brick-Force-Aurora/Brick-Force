public class ZombieVirus
{
	private bool active;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public bool IsReallyActive => RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode && active;

	public float SpeedFactor => (!IsReallyActive) ? 1f : 1.15f;

	public float MaxHpFactor => (!IsReallyActive) ? 0f : 3f;

	public ZombieVirus()
	{
		active = false;
	}
}
