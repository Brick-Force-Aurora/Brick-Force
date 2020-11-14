using System.Collections.Generic;
using UnityEngine;

public class Peer
{
	public enum P2P_STATUS
	{
		NONE,
		PRIVATE,
		PUBLIC,
		RELAY
	}

	private const float holePunchingTimeout = 5f;

	private int seq;

	private float deltaTime;

	private string localIp;

	private int localPort;

	private string remoteIp;

	private int remotePort;

	private byte playerFlag;

	private P2P_STATUS p2pStatus;

	private int sendPingCount;

	private Dictionary<int, P2P_STATUS> dicLinked;

	private float pingTime;

	public int Seq => seq;

	public bool HolePunchingTimeout => deltaTime > 5f;

	public string LocalIp
	{
		get
		{
			return localIp;
		}
		set
		{
			localIp = value;
		}
	}

	public int LocalPort
	{
		get
		{
			return localPort;
		}
		set
		{
			localPort = value;
		}
	}

	public string RemoteIp
	{
		get
		{
			return remoteIp;
		}
		set
		{
			remoteIp = value;
		}
	}

	public int RemotePort
	{
		get
		{
			return remotePort;
		}
		set
		{
			remotePort = value;
		}
	}

	public byte PlayerFlag => playerFlag;

	public P2P_STATUS P2pStatus
	{
		get
		{
			return p2pStatus;
		}
		set
		{
			p2pStatus = value;
		}
	}

	public int SendPingCount
	{
		get
		{
			return sendPingCount;
		}
		set
		{
			sendPingCount = value;
		}
	}

	public float PingTime
	{
		get
		{
			return pingTime;
		}
		set
		{
			pingTime = value;
		}
	}

	public Peer(int _seq, string _localIp, int _localPort, string _remoteIp, int _remotePort, byte _playerFlag)
	{
		seq = _seq;
		deltaTime = 0f;
		localIp = _localIp;
		localPort = _localPort;
		remoteIp = _remoteIp;
		remotePort = _remotePort;
		playerFlag = _playerFlag;
		p2pStatus = P2P_STATUS.NONE;
		dicLinked = new Dictionary<int, P2P_STATUS>();
		pingTime = 0f;
	}

	public bool IsWebPlayer()
	{
		return (playerFlag & 1) != 0;
	}

	public bool IsGM()
	{
		return (playerFlag & 2) != 0;
	}

	public void ForceToRelay()
	{
		p2pStatus = P2P_STATUS.RELAY;
	}

	public bool IsLinked(int with)
	{
		if (dicLinked == null)
		{
			return false;
		}
		if (!dicLinked.ContainsKey(with))
		{
			return false;
		}
		return true;
	}

	public void UpdateLink(int with, P2P_STATUS p2pStatus)
	{
		if (dicLinked != null)
		{
			if (!dicLinked.ContainsKey(with))
			{
				dicLinked.Add(with, p2pStatus);
			}
			else
			{
				dicLinked[with] = p2pStatus;
			}
		}
	}

	public void Update()
	{
		if (p2pStatus == P2P_STATUS.NONE)
		{
			deltaTime += Time.deltaTime;
		}
	}

	public void EndSession()
	{
		dicLinked.Clear();
		p2pStatus = P2P_STATUS.NONE;
		deltaTime = 0f;
		pingTime = 0f;
	}
}
