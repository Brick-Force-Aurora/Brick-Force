public class ThumbnailRequest
{
	public bool IsUserMap;

	public int Id;

	public byte[] ThumbnailBuffer;

	public ThumbnailRequest(bool isUserMap, int id)
	{
		IsUserMap = isUserMap;
		Id = id;
		ThumbnailBuffer = null;
	}
}
