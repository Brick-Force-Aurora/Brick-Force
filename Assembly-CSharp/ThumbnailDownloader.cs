using System.Collections.Generic;
using UnityEngine;

public class ThumbnailDownloader : MonoBehaviour
{
	private Dictionary<int, ThumbnailRequest> dicRegMap;

	private Dictionary<int, ThumbnailRequest> dicUserMap;

	private Queue<ThumbnailRequest> queue;

	private bool waitingForResponse;

	public float requestCycle = 1f;

	private static ThumbnailDownloader _instance;

	public static ThumbnailDownloader Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ThumbnailDownloader)) as ThumbnailDownloader);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ThumbnailDownloader Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		queue = new Queue<ThumbnailRequest>();
		dicRegMap = new Dictionary<int, ThumbnailRequest>();
		dicUserMap = new Dictionary<int, ThumbnailRequest>();
		waitingForResponse = false;
		Object.DontDestroyOnLoad(this);
	}

	public void Clear()
	{
		queue.Clear();
		dicRegMap.Clear();
		dicUserMap.Clear();
		waitingForResponse = false;
	}

	private void Update()
	{
		if (!waitingForResponse && queue.Count > 0)
		{
			ThumbnailRequest thumbnailRequest = queue.Peek();
			if (thumbnailRequest != null)
			{
				waitingForResponse = true;
				CSNetManager.Instance.Sock.SendCS_DOWNLOAD_THUMBNAIL_REQ(thumbnailRequest.IsUserMap, thumbnailRequest.Id);
			}
		}
	}

	public void Enqueue(bool isUserMap, int id)
	{
		if (isUserMap)
		{
			if (dicUserMap.ContainsKey(id))
			{
				return;
			}
		}
		else if (dicRegMap.ContainsKey(id))
		{
			return;
		}
		ThumbnailRequest thumbnailRequest = new ThumbnailRequest(isUserMap, id);
		queue.Enqueue(thumbnailRequest);
		if (isUserMap)
		{
			dicUserMap.Add(id, thumbnailRequest);
		}
		else
		{
			dicRegMap.Add(id, thumbnailRequest);
		}
	}

	public ThumbnailRequest Dequeue(bool success)
	{
		ThumbnailRequest thumbnailRequest = null;
		if (queue.Count > 0)
		{
			thumbnailRequest = queue.Dequeue();
			if (thumbnailRequest != null && success)
			{
				if (thumbnailRequest.IsUserMap)
				{
					dicUserMap.Remove(thumbnailRequest.Id);
				}
				else
				{
					dicRegMap.Remove(thumbnailRequest.Id);
				}
			}
		}
		waitingForResponse = false;
		return thumbnailRequest;
	}

	public void Stack(byte[] data2)
	{
		if (queue.Count > 0)
		{
			ThumbnailRequest thumbnailRequest = queue.Peek();
			if (thumbnailRequest != null)
			{
				if (thumbnailRequest.ThumbnailBuffer == null)
				{
					thumbnailRequest.ThumbnailBuffer = new byte[data2.Length];
					data2.CopyTo(thumbnailRequest.ThumbnailBuffer, 0);
				}
				else
				{
					byte[] thumbnailBuffer = thumbnailRequest.ThumbnailBuffer;
					thumbnailRequest.ThumbnailBuffer = new byte[thumbnailBuffer.Length + data2.Length];
					thumbnailBuffer.CopyTo(thumbnailRequest.ThumbnailBuffer, 0);
					data2.CopyTo(thumbnailRequest.ThumbnailBuffer, thumbnailBuffer.Length);
				}
			}
		}
	}
}
