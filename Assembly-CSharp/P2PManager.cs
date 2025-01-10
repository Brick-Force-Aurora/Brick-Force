using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using _Emulator;
using UnityEngine;

public class P2PManager : MonoBehaviour
{
	public const byte PEER_MOVE = 2;

	public const byte PEER_PUBLIC_HAND = 3;

	public const byte PEER_PUBLIC_SHAKE = 4;

	public const byte PEER_THROW = 5;

	public const byte PEER_SWAP_WEAPON = 6;

	public const byte PEER_RELOAD = 7;

	public const byte PEER_FIRE = 8;

	public const byte PEER_SHOOT = 9;

	public const byte PEER_DIE = 10;

	public const byte PEER_RESPAWN = 11;

	public const byte PEER_SLASH = 12;

	public const byte PEER_PIERCE = 13;

	public const byte PEER_PROJECTILE = 14;

	public const byte PEER_BOMBED = 15;

	public const byte PEER_PROJECTILE_FLY = 16;

	public const byte PEER_PROJECTILE_KABOOM = 17;

	public const byte PEER_PING = 18;

	public const byte PEER_PRIVATE_HAND = 19;

	public const byte PEER_PRIVATE_SHAKE = 20;

	public const byte PEER_BRICK_HITPOINT = 21;

	public const byte PEER_COMPOSE = 22;

	public const byte PEER_WEAPON_STATUS = 23;

	public const byte PEER_RANDEZVOUS_PING = 24;

	public const byte PEER_RANDEZVOUS_PONG = 25;

	public const byte PEER_INITIATE = 26;

	public const byte PEER_RELIABLE_ACK = 27;

	public const byte PEER_FALL_DOWN = 28;

	public const byte PEER_CANNON_MOVE = 29;

	public const byte PEER_CANNON_FIRE = 30;

	public const byte PEER_HIT_BRICK = 31;

	public const byte PEER_HIT_BRICKMAN = 32;

	public const byte PEER_HIT_INVINCIBLE_ARMOR = 33;

	public const byte PEER_ENABLE_HANDBOMB = 34;

	public const byte PEER_RELAY_HAND = 35;

	public const byte PEER_RELAY_SHAKE = 36;

	public const byte PEER_MON_GEN = 37;

	public const byte PEER_MON_DIE = 38;

	public const byte PEER_MON_MOVE = 39;

	public const byte PEER_MON_SHOOT = 40;

	public const byte PEER_CORE_HP = 41;

	public const byte PEER_MON_FIRE = 42;

	public const byte PEER_MON_HIT = 43;

	public const byte PEER_SPECTATOR = 44;

	public const byte PEER_MONBOMB_CREATE = 45;

	public const byte PEER_MONBOMB_MOVE = 46;

	public const byte PEER_BLAST_TIME = 47;

	public const byte PEER_HIT_IMPACT = 48;

	public const byte PEER_CTF_STATE = 49;

	public const byte PEER_INSTALLING_BOMB = 50;

	public const byte PEER_UNINSTALLING_BOMB = 51;

	public const byte PEER_P2PSTATUS = 52;

	public const byte PEER_DIR = 53;

	public const byte PEER_CONSUME = 54;

	public const byte PEER_UNINVINCIBLE = 55;

	public const byte PEER_NUMWAVE = 56;

	public const byte PEER_MON_ADDPOINT = 57;

	public const byte PEER_DF_HEALER = 58;

	public const byte PEER_DF_SELFHEAL = 59;

	public const byte PEER_HP = 60;

	public const byte PEER_BIG_WPN_FIRE = 61;

	public const byte PEER_BIG_WPN_FLY = 62;

	public const byte PEER_BIG_WPN_KABOOM = 63;

	public const byte PEER_BOOST = 64;

	public const byte PEER_SENSEBEAM = 65;

	public const byte PEER_CONNECTION_STATUS = 66;

	public const byte PEER_LEAVE = 67;

	public const byte PEER_RANDEZVOUS_STATUS = 68;

	public const byte PEER_BIG_FIRE = 69;

	public const byte PEER_PORTAL = 70;

	public const byte PEER_BRICK_ANIM = 71;

	public const byte PEER_INVISIBLILITY = 72;

	public const byte PEER_FIRE_NEW = 73;

	public const byte PEER_HIT_BRICK_NEW = 74;

	public const byte PEER_MON_HIT_NEW = 75;

	public const byte PEER_HIT_BRICKMAN_NEW = 76;

	public const byte PEER_HIT_IMPACT_NEW = 77;

	public const byte PEER_MOVE_W = 78;

	public const byte PEER_FIRE_ACTION_W = 79;

	public const byte PEER_FIRE_W = 80;

	public const byte PEER_DIR_W = 81;

	public const byte PEER_HIT_BRICKMAN_W = 82;

	public const byte PEER_CREATE_ACTIVE_ITEM = 83;

	public const byte PEER_EAT_ACTIVE_ITEM_REQ = 84;

	public const byte PEER_EAT_ACTIVE_ITEM_ACK = 85;

	public const byte PEER_GUN_ANIM = 86;

	public const byte PEER_STATE_FEVER = 87;

	public const byte PEER_USE_ACTIVE_ITEM = 88;

	public const byte PEER_BLACKHOLE_EFF = 89;

	public const byte PEER_BUNGEE_BREAK_INTO_REQ = 90;

	public const byte PEER_BUNGEE_BREAK_INTO_ACK = 91;

	public const byte PEER_BUNGEE_BREAK_INTO_ACTIVEITEM_LIST = 92;

	public const byte PEER_BRICK_ANIM_CROSSFADE = 93;

	public const byte PEER_TRAIN_ROTATE = 94;

	public const byte PEER_CLIENTINFO = 95;

	public Socket sock;

	private long prevServerTick = 9223372036854775807L;

	public Queue readQueue;

	private ushort reliableIndex;

	public Queue<P2PMsg4Send> queueReliable;

	private ushort processedReliableIndex;

	public Dictionary<int, Peer> dic;

	public string rendezvousIp;

	public int rendezvousPort;

	public bool OutputDebug = true;

	public string localIp = string.Empty;

	public string remoteIp;

	public int remotePort;

	public P2PMsg4Recv recv;

	public bool rendezvousPointed;

	public float handshakeTime;

	private float reliableTime;

	private float deltaTime;

	private int pingOverCount;

	private static P2PManager _instance;

	public int RendezvousPort
	{
		get
		{
			return rendezvousPort;
		}
		set
		{
			rendezvousPort = value;
		}
	}

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

	public bool RendezvousPointed => rendezvousPointed;

	public static P2PManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(P2PManager)) as P2PManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the P2PManager Instance");
				}
			}
			return _instance;
		}
	}

	private void ClearSock()
	{
		if (sock != null)
		{
			try
			{
				sock.Shutdown(SocketShutdown.Both);
				if (sock != null)
				{
					sock.Close();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " P2PManager.ClearSock");
			}
			sock = null;
		}
	}

	private void Start()
	{
		recv = null;
		sock = null;
	}

	public void StartSession()
	{
		handshakeTime = 0f;
		deltaTime = 0f;
		NoCheat.Instance.ResetSpeedHack();
		prevServerTick = 9223372036854775807L;
	}

	public void ResetReliables()
	{
		reliableIndex = 0;
		reliableTime = 0f;
		processedReliableIndex = 0;
		queueReliable.Clear();
	}

	public void EndSession()
	{
		foreach (KeyValuePair<int, Peer> item in dic)
		{
			item.Value.EndSession();
		}
		NoCheat.Instance.ResetSpeedHack();
		prevServerTick = 9223372036854775807L;
	}

	private void SendPing()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(MyInfoManager.Instance.PingTime);
		Say(18, p2PMsgBody);
	}

	private void SendP2pStatus()
	{
		return;
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				p2PMsgBody.Write(dic.Count);
				foreach (KeyValuePair<int, Peer> item in dic)
				{
					bool val = item.Value.P2pStatus == Peer.P2P_STATUS.RELAY;
					p2PMsgBody.Write(item.Value.Seq);
					p2PMsgBody.Write(val);
				}
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(52, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.RandezvousPing");
			}
		}
	}

	private void SendRandezvousPing()
	{
		return;
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				p2PMsgBody.Write(Time.time);
				p2PMsgBody.Write(!rendezvousPointed);
				p2PMsgBody.Write((byte)MyInfoManager.Instance.Slot);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(24, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.RandezvousPing");
			}
		}
	}

	private void SendClientInfo()
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(CSNetManager.Instance.BfServer);
				p2PMsgBody.Write(CSNetManager.Instance.BfPort);
				p2PMsgBody.Write(RoomManager.Instance.CurrentRoom);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(95, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.ClientInfo");
			}
		}
	}

	private void ReceiveFromCallback(IAsyncResult ar)
	{
		if (sock != null)
		{
			try
			{
				EndPoint end_point = new IPEndPoint(0L, 0);
				int num = sock.EndReceiveFrom(ar, ref end_point);
				if (num > 0)
				{
					recv.Io += num;
					for (P2PMsg4Recv.MsgStatus status = recv.GetStatus(byte.MaxValue); status == P2PMsg4Recv.MsgStatus.COMPLETE; status = recv.GetStatus(byte.MaxValue))
					{
						byte id = recv.GetId();
						ushort meta = recv.GetMeta();
						byte src = recv.GetSrc();
						byte dst = recv.GetDst();
						P2PMsgBody msg = recv.Flush();
						lock (this)
						{
							readQueue.Enqueue(new P2PMsg2Handle(id, msg, (IPEndPoint)end_point, meta, src, dst));
						}
					}
				}
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.ReceiveFromCallback");
			}
			Listen();
		}
	}

	private void Listen()
	{
		if (sock != null && recv != null)
		{
			try
			{
				IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.None, rendezvousPort);
				EndPoint remote_end = iPEndPoint;
				recv.Io = 0;
				sock.BeginReceiveFrom(recv.Buffer, 0, recv.Buffer.Length, SocketFlags.None, ref remote_end, ReceiveFromCallback, sock);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.Listen");
			}
		}
	}

	public void Bootup(string _rendezvousIp, int _rendezvousPort)
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			localIp = string.Empty;
			Security.PrefetchSocketPolicy(_rendezvousIp, 843);
		}
		else
		{
			FetchLocalIp();
		}
		rendezvousIp = _rendezvousIp;
		rendezvousPort = _rendezvousPort;
		Open();
		SendRandezvousPing();
	}

	private void Open()
	{
		try
		{
			Close();
			ResetReliables();
			remoteIp = string.Empty;
			remotePort = 0;
			readQueue = new Queue();
			recv = new P2PMsg4Recv();
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			uint num = 2147483648u;
			uint num2 = 402653184u;
			uint ioctl_code = num | num2 | 0xC;
			sock.IOControl((int)ioctl_code, new byte[1]
			{
				Convert.ToByte(value: false)
			}, null);
			EndPoint local_end = new IPEndPoint(IPAddress.Any, rendezvousPort);
			sock.Bind(local_end);
		}
		catch (SocketException ex)
		{
			Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.OnStart");
		}
		Listen();
	}

	private void Close()
	{
		ClearSock();
		if (readQueue != null)
		{
			readQueue.Clear();
			readQueue = null;
		}
		recv = null;
		rendezvousPointed = false;
	}

	public void Shutdown()
	{
		SendPEER_LEAVE();
		Close();
		EndSession();
		RemoveAll();
	}

	private void CheckP2pingDone()
	{
		if (MyInfoManager.Instance.Status == 3)
		{
			bool flag = false;
			BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Status == 2)
				{
					flag = true;
					if (OutputDebug)
					{
						Debug.Log(" Someone is loading yet " + array[i].Nickname + "(" + array[i].Seq + ")");
					}
				}
			}
			bool flag2 = false;
			foreach (KeyValuePair<int, Peer> item in dic)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(item.Value.Seq);
				if (desc != null && desc.NeedP2ping && item.Value.P2pStatus == Peer.P2P_STATUS.NONE)
				{
					flag2 = true;
					if (OutputDebug)
					{
						Debug.Log(" Doesnt have connection yet " + desc.Nickname + "(" + item.Value.Seq + ")");
					}
				}
			}
			if (!flag && !flag2)
			{
				CSNetManager.Instance.Sock.SendCS_P2P_COMPLETE_REQ();
				NoCheat.Instance.SetSpeedHack();
			}
		}
	}

	public void FetchLocalIp()
	{
		try
		{
			localIp = string.Empty;
			string hostName = Dns.GetHostName();
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length <= 0)
			{
				Debug.LogError("Fail to get local ip addresses ");
			}
			else
			{
				localIp = hostAddresses[0].ToString();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error " + ex.Message.ToString() + " P2PManager.FetchLocalIp ");
		}
	}

	private void OnApplicationQuit()
	{
		Shutdown();
		_instance = null;
	}

	public void Refresh(int seq, string localIp, int localPort, string remoteIp, int remotePort)
	{
		if (dic != null && dic.ContainsKey(seq))
		{
			dic[seq].LocalIp = localIp;
			dic[seq].LocalPort = localPort;
			dic[seq].RemoteIp = remoteIp;
			dic[seq].RemotePort = remotePort;
		}
	}

	public void Add(int seq, string localIp, int localPort, string remoteIp, int remotePort, byte playerflag)
	{
		if (dic != null)
		{
			if (dic.ContainsKey(seq))
			{
				//Debug.LogError("Duplicate peer seq " + seq);
			}
			else
			{
				dic.Add(seq, new Peer(seq, localIp, localPort, remoteIp, remotePort, playerflag));
			}
		}
	}

	public void Remove(int seq)
	{
		if (dic.ContainsKey(seq))
		{
			dic.Remove(seq);
		}
	}

	public Peer Get(int seq)
	{
		if (!dic.ContainsKey(seq))
		{
			return null;
		}
		return dic[seq];
	}

	public void RemoveAll()
	{
		dic.Clear();
	}

	private void Awake()
	{
		dic = new Dictionary<int, Peer>();
		queueReliable = new Queue<P2PMsg4Send>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void HandshakeComplete(int with, Peer.P2P_STATUS p2pStatus)
	{
		Peer peer = Get(with);
		if (peer != null && peer.P2pStatus == Peer.P2P_STATUS.NONE)
		{
			peer.P2pStatus = p2pStatus;
			SendPEER_INITIATE(with);
		}
	}

	private void Handshake()
	{
		if (MyInfoManager.Instance.Status == 3 || MyInfoManager.Instance.Status == 4)
		{
			bool flag = false;
			handshakeTime += Time.deltaTime;
			if (handshakeTime > 0.1f)
			{
				handshakeTime = 0f;
				flag = true;
			}
			foreach (KeyValuePair<int, Peer> item in dic)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(item.Value.Seq);
				if (desc != null && item.Value.P2pStatus == Peer.P2P_STATUS.NONE && (desc.Status == 3 || desc.Status == 4))
				{
					if (flag)
					{
						if (!BuildOption.Instance.Props.UseP2pHolePunching || item.Value.IsWebPlayer() || item.Value.HolePunchingTimeout)
						{
							SendPEER_RELAY_HAND(item.Value.Seq);
							if (OutputDebug)
							{
								Debug.Log("SendPEER_RELAY_HAND to " + item.Value.Seq.ToString());
							}
						}
						else
						{
							SendPEER_PRIVATE_HAND(item.Value.LocalIp, item.Value.LocalPort);
							if (OutputDebug)
							{
								Debug.Log("SendPEER_PRIVATE_HAND to " + item.Value.Seq.ToString());
								Debug.Log(">addr: " + item.Value.LocalIp);
								Debug.Log(">port: " + item.Value.LocalPort.ToString());
							}
							SendPEER_PUBLIC_HAND(item.Value.RemoteIp, item.Value.RemotePort);
							if (OutputDebug)
							{
								Debug.Log("SendPEER_PUBLIC_HAND to " + item.Value.Seq.ToString());
								Debug.Log(">addr: " + item.Value.RemoteIp);
								Debug.Log(">port: " + item.Value.RemotePort.ToString());
							}
						}
					}
					item.Value.Update();
				}
			}
		}
	}

	private void SendPEER_RELIABLE_ACK(uint reliable)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(reliable);
		P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(27, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
		if (iPEndPoint != null && p2PMsg4Send != null)
		{
			sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
		}
	}

	private void ReliableSend(uint to, byte id, P2PMsgBody mb)
	{
		if (queueReliable == null)
		{
			queueReliable = new Queue<P2PMsg4Send>();
		}
		queueReliable.Enqueue(new P2PMsg4Send(id, ++reliableIndex, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot(to), mb, byte.MaxValue));
		if (queueReliable.Count <= 1)
		{
			reliableTime = 0f;
			SendReliable();
		}
	}

	public static byte Seq2Slot(uint seq)
	{
		if (seq == uint.MaxValue)
		{
			return byte.MaxValue;
		}
		if (MyInfoManager.Instance.Seq == seq)
		{
			return (byte)MyInfoManager.Instance.Slot;
		}
		BrickManDesc desc = BrickManManager.Instance.GetDesc((int)seq);
		if (desc == null)
		{
			return byte.MaxValue;
		}
		return (byte)desc.Slot;
	}

	private void SendReliable()
	{
		if (sock != null && queueReliable != null && queueReliable.Count > 0)
		{
			P2PMsg4Send p2PMsg4Send = queueReliable.Peek();
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
			if (iPEndPoint != null && p2PMsg4Send != null)
			{
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
			}
		}
	}

	public void Say(byte id, P2PMsgBody mb)
	{
		try
		{
			bool flag = false;
			foreach (KeyValuePair<int, Peer> item in dic)
			{
				if (item.Value.P2pStatus != 0)
				{
					P2PMsg4Send p2PMsg4Send = null;
					IPEndPoint iPEndPoint = null;
					if (item.Value.P2pStatus == Peer.P2P_STATUS.PRIVATE)
					{
						p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot((uint)item.Key), mb, byte.MaxValue);
						iPEndPoint = new IPEndPoint(IPAddress.Parse(item.Value.LocalIp), item.Value.LocalPort);
					}
					else if (item.Value.P2pStatus == Peer.P2P_STATUS.PUBLIC)
					{
						p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot((uint)item.Key), mb, byte.MaxValue);
						iPEndPoint = new IPEndPoint(IPAddress.Parse(item.Value.RemoteIp), item.Value.RemotePort);
					}
					else if (!flag)
					{
						flag = true;
						p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, mb, byte.MaxValue);
						iPEndPoint = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
					}
					if (iPEndPoint != null && p2PMsg4Send != null)
					{
						sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
					}
				}
			}
		}
		catch (SocketException ex)
		{
			Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.Say");
		}
	}

	public void Whisper(int to, byte id, P2PMsgBody mb)
	{
		if (dic.ContainsKey(to) && dic[to].P2pStatus != 0)
		{
			try
			{
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot((uint)to), mb, byte.MaxValue);
				IPEndPoint iPEndPoint = null;
				iPEndPoint = ((dic[to].P2pStatus == Peer.P2P_STATUS.PRIVATE) ? new IPEndPoint(IPAddress.Parse(dic[to].LocalIp), dic[to].LocalPort) : ((dic[to].P2pStatus != Peer.P2P_STATUS.PUBLIC) ? new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort) : new IPEndPoint(IPAddress.Parse(dic[to].RemoteIp), dic[to].RemotePort)));
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.Whisper");
			}
		}
	}

	private void OnGUI()
	{
	}

	private void Update()
	{
		if (((P2PExtension.instance.isSteam && P2PExtension.instance.listenSteam) || sock != null) && recv != null && readQueue != null)
		{
			Handshake();
			CheckP2pingDone();
			reliableTime += Time.deltaTime;
			if (reliableTime > 0.1f)
			{
				reliableTime = 0f;
				SendReliable();
			}
			deltaTime += Time.deltaTime;
			if (deltaTime > 1f)
			{
				deltaTime = 0f;
				SendPEER_CONNECTION_STATUS();
				SendRandezvousPing();
				SendP2pStatus();
				SendPing();
			}

			lock (this)
			{
				while (readQueue.Count > 0)
				{
					P2PMsg2Handle p2PMsg2Handle = (P2PMsg2Handle)readQueue.Peek();
					ushort meta = p2PMsg2Handle._meta;
					bool flag = true;
					if (meta != 65535)
					{
						SendPEER_RELIABLE_ACK(meta);
						if (processedReliableIndex >= meta)
						{
							flag = false;
						}
						else
						{
							processedReliableIndex = meta;
						}
					}
					flag = true;
					if (flag)
					{
						if (!P2PExtension.instance.HandleMessage(p2PMsg2Handle))
                        switch (p2PMsg2Handle._id)
						{
						case 26:
							HandlePEER_INITIATE(p2PMsg2Handle._msg);
							break;
						case 2:
							HandlePEER_MOVE(p2PMsg2Handle._msg);
							break;
						case 5:
							HandlePEER_THROW(p2PMsg2Handle._msg);
							break;
						case 6:
							HandlePEER_SWAP_WEAPON(p2PMsg2Handle._msg);
							break;
						case 7:
							HandlePEER_RELOAD(p2PMsg2Handle._msg);
							break;
						case 8:
							HandlePEER_FIRE(p2PMsg2Handle._msg);
							break;
						case 9:
							HandlePEER_SHOOT(p2PMsg2Handle._msg);
							break;
						case 10:
							HandlePEER_DIE(p2PMsg2Handle._msg);
							break;
						case 11:
							HandlePEER_RESPAWN(p2PMsg2Handle._msg);
							break;
						case 12:
							HandlePEER_SLASH(p2PMsg2Handle._msg);
							break;
						case 13:
							HandlePEER_PIERCE(p2PMsg2Handle._msg);
							break;
						case 14:
							HandlePEER_PROJECTILE(p2PMsg2Handle._msg);
							break;
						case 15:
							HandlePEER_BOMBED(p2PMsg2Handle._msg);
							break;
						case 16:
							HandlePEER_PROJECTILE_FLY(p2PMsg2Handle._msg);
							break;
						case 17:
							HandlePEER_PROJECTILE_KABOOM(p2PMsg2Handle._msg);
							break;
						case 18:
							HandlePEER_PING(p2PMsg2Handle._msg);
							break;
						case 21:
							HandlePEER_BRICK_HITPOINT(p2PMsg2Handle._msg);
							break;
						case 22:
							HandlePEER_COMPOSE(p2PMsg2Handle._msg);
							break;
						case 23:
							HandlePEER_WEAPON_STATUS(p2PMsg2Handle._msg);
							break;
                        case 24:
                            SendRandezvousPing();
                            break;
                        case 25:
							HandlePEER_RANDEZVOUS_PONG(p2PMsg2Handle._msg);
							break;
						case 27:
							HandlePEER_RELIABLE_ACK(p2PMsg2Handle._msg);
							break;
						case 28:
							HandlePEER_FALL_DOWN(p2PMsg2Handle._msg);
							break;
						case 29:
							HandlePEER_CANNON_MOVE(p2PMsg2Handle._msg);
							break;
						case 30:
							HandlePEER_CANNON_FIRE(p2PMsg2Handle._msg);
							break;
						case 31:
							HandlePEER_HIT_BRICK(p2PMsg2Handle._msg);
							break;
						case 32:
							HandlePEER_HIT_BRICKMAN(p2PMsg2Handle._msg);
							break;
						case 34:
							HandlePEER_ENABLE_HANDBOMB(p2PMsg2Handle._msg);
							break;
						case 20:
							HandlePEER_PRIVATE_SHAKE(p2PMsg2Handle._msg);
							break;
						case 4:
							HandlePEER_PUBLIC_SHAKE(p2PMsg2Handle._msg);
							break;
						case 36:
							HandlePEER_RELAY_SHAKE(p2PMsg2Handle._msg);
							break;
						case 19:
							HandlePEER_PRIVATE_HAND(p2PMsg2Handle._msg, p2PMsg2Handle._recvFrom);
							break;
						case 3:
							HandlePEER_PUBLIC_HAND(p2PMsg2Handle._msg, p2PMsg2Handle._recvFrom);
							break;
						case 35:
							HandlePEER_RELAY_HAND(p2PMsg2Handle._msg, p2PMsg2Handle._recvFrom);
							break;
						case 37:
							HandlePEER_MON_GEN(p2PMsg2Handle._msg);
							break;
						case 38:
							HandlePEER_MON_DIE(p2PMsg2Handle._msg);
							break;
						case 39:
							HandlePEER_MON_MOVE(p2PMsg2Handle._msg);
							break;
						case 43:
							HandlePEER_MON_HIT(p2PMsg2Handle._msg);
							break;
						case 44:
							HandlePEER_SPECTATOR(p2PMsg2Handle._msg);
							break;
						case 47:
							HandlePEER_BLAST_TIME(p2PMsg2Handle._msg);
							break;
						case 48:
							HandlePEER_HIT_IMPACT(p2PMsg2Handle._msg);
							break;
						case 50:
							HandlePEER_INSTALLING_BOMB(p2PMsg2Handle._msg);
							break;
						case 51:
							HandlePEER_UNINSTALLING_BOMB(p2PMsg2Handle._msg);
							break;
                                

                        case 52:
                                SendP2pStatus();
                                break;
                        case 53:
							HandlePEER_DIR(p2PMsg2Handle._msg);
							break;
						case 54:
							HandlePEER_CONSUME(p2PMsg2Handle._msg);
							break;
						case 55:
							HandlePEER_UNINVINCIBLE(p2PMsg2Handle._msg);
							break;
						case 56:
							HandlePEER_NUMWAVE(p2PMsg2Handle._msg);
							break;
						case 57:
							HandlePEER_MON_ADDPOINT(p2PMsg2Handle._msg);
							break;
						case 58:
							HandlePEER_DF_HEALER(p2PMsg2Handle._msg);
							break;
						case 59:
							HandlePEER_DF_SELFHEAL(p2PMsg2Handle._msg);
							break;
						case 60:
							HandlePEER_HP(p2PMsg2Handle._msg);
							break;
						case 61:
							HandlePEER_BIG_WPN_FIRE(p2PMsg2Handle._msg);
							break;
						case 62:
							HandlePEER_BIG_WPN_FLY(p2PMsg2Handle._msg);
							break;
						case 63:
							HandlePEER_BIG_WPN_KABOOM(p2PMsg2Handle._msg);
							break;
						case 64:
							HandlePEER_BOOST(p2PMsg2Handle._msg);
							break;
						case 65:
							HandlePEER_SENSEBEAM(p2PMsg2Handle._msg);
							break;
						case 66:
							HandlePEER_CONNECTION_STATUS(p2PMsg2Handle._msg);
							break;
						case 68:
							HandlePEER_RANDEZVOUS_STATUS(p2PMsg2Handle._msg);
							break;
						case 69:
							HandlePEER_BIG_FIRE(p2PMsg2Handle._msg);
							break;
						case 70:
							HandlePEER_PORTAL(p2PMsg2Handle._msg);
							break;
						case 71:
							HandlePEER_BRICK_ANIM(p2PMsg2Handle._msg);
							break;
						case 72:
							HandlePEER_INVISIBLILITY(p2PMsg2Handle._msg);
							break;
						case 73:
							HandlePEER_FIRE_NEW(p2PMsg2Handle._msg);
							break;
						case 74:
							HandlePEER_HIT_BRICK_NEW(p2PMsg2Handle._msg);
							break;
						case 75:
							HandlePEER_MON_HIT_NEW(p2PMsg2Handle._msg);
							break;
						case 76:
							HandlePEER_HIT_BRICKMAN_NEW(p2PMsg2Handle._msg);
							break;
						case 77:
							HandlePEER_HIT_IMPACT_NEW(p2PMsg2Handle._msg);
							break;
						case 78:
							HandlePEER_MOVE_W(p2PMsg2Handle._msg);
							break;
						case 79:
							HandlePEER_FIRE_ACTION_W(p2PMsg2Handle._msg);
							break;
						case 80:
							HandlePEER_FIRE_W(p2PMsg2Handle._msg);
							break;
						case 81:
							HandlePEER_DIR_W(p2PMsg2Handle._msg);
							break;
						case 82:
							HandlePEER_HIT_BRICKMAN_W(p2PMsg2Handle._msg);
							break;
						case 83:
							HandlePEER_CREATE_ACTIVE_ITEM(p2PMsg2Handle._msg);
							break;
						case 84:
							HandlePEER_EAT_ACTIVE_ITEM_REQ(p2PMsg2Handle._msg);
							break;
						case 85:
							HandlePEER_EAT_ACTIVE_ITEM_ACK(p2PMsg2Handle._msg);
							break;
						case 86:
							HandlePEER_GUN_ANIM(p2PMsg2Handle._msg);
							break;
						case 87:
							HandlePEER_STATE_FEVER(p2PMsg2Handle._msg);
							break;
						case 88:
							HandlePEER_USE_ACTIVE_ITEM(p2PMsg2Handle._msg);
							break;
						case 89:
							HandlePEER_BLACKHOLE_EFF(p2PMsg2Handle._msg);
							break;
						case 90:
							HandlePEER_BUNGEE_BREAK_INTO_REQ(p2PMsg2Handle._msg);
							break;
						case 91:
							HandlePEER_BUNGEE_BREAK_INTO_ACK(p2PMsg2Handle._msg);
							break;
						case 92:
							HandlePEER_BUNGEE_BREAK_INTO_ACTIVEITEM_LIST(p2PMsg2Handle._msg);
							break;
						case 93:
							HandlePEER_BRICK_ANIM_CROSSFADE(p2PMsg2Handle._msg);
							break;
						case 94:
							HandlePEER_TRAIN_ROTATE(p2PMsg2Handle._msg);
							break;
						default:
							Debug.LogError("Unknown msg encountered " + p2PMsg2Handle._id);
							break;
						}
					}
					readQueue.Dequeue();
				}
			}
		}
	}

	public Peer[] ToArray()
	{
		List<Peer> list = new List<Peer>();
		foreach (KeyValuePair<int, Peer> item in dic)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public bool IsConnected(int a, int b)
	{
		if (dic == null || a == b)
		{
			return false;
		}
		bool result = false;
		if (a == MyInfoManager.Instance.Seq)
		{
			if (dic.ContainsKey(b))
			{
				result = (dic[b].P2pStatus != 0 && dic[b].IsLinked(a));
			}
		}
		else if (b == MyInfoManager.Instance.Seq)
		{
			if (dic.ContainsKey(a))
			{
				result = (dic[a].P2pStatus != 0 && dic[a].IsLinked(b));
			}
		}
		else if (dic.ContainsKey(a) && dic.ContainsKey(b))
		{
			Peer peer = dic[a];
			Peer peer2 = dic[b];
			result = (peer.IsLinked(b) && peer2.IsLinked(a));
		}
		return result;
	}

	private void HandlePEER_CONNECTION_STATUS(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out int val4);
			if (dic.ContainsKey(val))
			{
				dic[val].UpdateLink(val3, (Peer.P2P_STATUS)val4);
			}
		}
	}

	private void HandlePEER_RANDEZVOUS_STATUS(P2PMsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int _);
			msg.Read(out byte _);
		}
	}

	public void SendPEER_RANDEZVOUS_STATUS()
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody msgBody = new P2PMsgBody();
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(68, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, msgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.RandezvousPing");
			}
		}
	}

	public void SendPEER_LEAVE()
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(67, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.RandezvousPing");
			}
		}
	}

	private void SendPEER_CONNECTION_STATUS()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		List<Peer> list = new List<Peer>();
		foreach (KeyValuePair<int, Peer> item in dic)
		{
			if (item.Value.P2pStatus != 0)
			{
				list.Add(item.Value);
			}
		}
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		int count = list.Count;
		p2PMsgBody.Write(count);
		for (int i = 0; i < count; i++)
		{
			p2PMsgBody.Write(list[i].Seq);
			p2PMsgBody.Write((int)list[i].P2pStatus);
		}
		Say(66, p2PMsgBody);
	}

	private void HandlePEER_PING(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out float val2);
		if (dic.ContainsKey(val))
		{
			dic[val].PingTime = val2;
		}
	}

	private void HandlePEER_RANDEZVOUS_PONG(P2PMsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out int val2);
		msg.Read(out float val3);
		msg.Read(out long val4);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_RANDEZVOUS_PONG");
		}
		if (prevServerTick != 9223372036854775807L)
		{
			NoCheat.Instance.CheckServerTime(prevServerTick, val4);
		}
		prevServerTick = val4;
		rendezvousPointed = true;
		MyInfoManager.Instance.PingTime = Time.time - val3;
		if (localIp.Length <= 0)
		{
			localIp = val;
		}
		if (remoteIp != val || remotePort != val2)
		{
			remoteIp = val;
			remotePort = val2;
			CSNetManager.Instance.Sock.SendCS_RANDEZVOUS_POINT_REQ(localIp, rendezvousPort, remoteIp, remotePort);
			if (OutputDebug)
			{
				Debug.Log("HandlePEER_RANDEZVOUS_PONG " + val + " " + val2);
			}
		}
		if (MyInfoManager.Instance.PingTime > 1f)
		{
			pingOverCount++;
		}
		else
		{
			pingOverCount = 0;
		}
		if (pingOverCount >= 3)
		{
			GlobalVars.Instance.SetForceClosed(set: false);
			GlobalVars.Instance.tutorFirstScriptOn = true;
			Squad curSquad = SquadManager.Instance.CurSquad;
			if (curSquad != null)
			{
				Instance.Shutdown();
				CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
				CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
				SquadManager.Instance.Leave();
				Application.LoadLevel("Squading");
			}
			else if (!Application.loadedLevelName.Contains("Tutor"))
			{
				Instance.Shutdown();
				CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
				GlobalVars.Instance.GotoLobbyRoomList = true;
				Application.LoadLevel("Lobby");
			}
			else
			{
				Instance.Shutdown();
				CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
				Application.LoadLevel("BfStart");
			}
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_NOT_ENOUGH"));
		}
	}

	private void HandlePEER_RELAY_HAND(P2PMsgBody msg, IPEndPoint recvFrom)
	{
		msg.Read(out int val);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_RELAY_HAND from " + val.ToString());
		}
		if (sock != null)
		{
			try
			{
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(36, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot((uint)val), p2PMsgBody, byte.MaxValue);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, recvFrom);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString());
			}
		}
	}

	private void HandlePEER_RELAY_SHAKE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_RELAY_SHAKE with " + val);
		}
		HandshakeComplete(val, Peer.P2P_STATUS.RELAY);
	}

	private void HandlePEER_PRIVATE_HAND(P2PMsgBody msg, IPEndPoint recvFrom)
	{
		msg.Read(out int val);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_PRIVATE_HAND from " + val.ToString());
		}
		if (sock != null && BuildOption.Instance.Props.UseP2pHolePunching)
		{
			try
			{
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(20, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, recvFrom);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString());
			}
		}
	}

	private void HandlePEER_PRIVATE_SHAKE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_PRIVATE_SHAKE with " + val);
		}
		if (BuildOption.Instance.Props.UseP2pHolePunching)
		{
			HandshakeComplete(val, Peer.P2P_STATUS.PRIVATE);
		}
	}

	private void HandlePEER_PUBLIC_HAND(P2PMsgBody msg, IPEndPoint recvFrom)
	{
		msg.Read(out int val);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_PUBLIC_HAND from " + val.ToString());
		}
		if (sock != null && BuildOption.Instance.Props.UseP2pHolePunching)
		{
			P2PMsgBody p2PMsgBody = new P2PMsgBody();
			p2PMsgBody.Write(MyInfoManager.Instance.Seq);
			try
			{
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(4, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, recvFrom);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString());
			}
		}
	}

	private void HandlePEER_PUBLIC_SHAKE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		if (OutputDebug)
		{
			Debug.Log("HandlePEER_PUBLIC_SHAKE with " + val);
		}
		if (BuildOption.Instance.Props.UseP2pHolePunching)
		{
			HandshakeComplete(val, Peer.P2P_STATUS.PUBLIC);
		}
	}

	private void SendPEER_RELAY_HAND(int to)
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(35, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), Seq2Slot((uint)to), p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(rendezvousIp), rendezvousPort);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.SendPEER_RELAY_HAND");
			}
		}
	}

	public void SendPEER_PRIVATE_HAND(string address, int port)
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(19, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(address), port);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.SendPEER_PRIVATE_HAND");
			}
		}
	}

	public void SendPEER_PUBLIC_HAND(string address, int port)
	{
		if (sock != null)
		{
			try
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(3, ushort.MaxValue, Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
				IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(address), port);
				sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.SendPEER_PUBLIC_HAND");
			}
		}
	}

	private void SendPEER_INITIATE(int to)
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			EquipCoordinator component2 = gameObject.GetComponent<EquipCoordinator>();
			if (null != component2 && null != component)
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				BF_PeerINITIATE bF_PeerINITIATE = default(BF_PeerINITIATE);
				bF_PeerINITIATE.cc = (int)component.ControlContext;
				bF_PeerINITIATE.dead = component.IsDead;
				bF_PeerINITIATE.invisibility = MyInfoManager.Instance.isInvisibilityOn;
				bF_PeerINITIATE.curWeaponType = component2.CurrentWeapon;
				bF_PeerINITIATE.empty = MyInfoManager.Instance.IsGunEmpty;
				p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				p2PMsgBody.Write(bF_PeerINITIATE.bitvector1);
				p2PMsgBody.Write(component.MoveSpeed);
				p2PMsgBody.Write(component.VertSpeed);
				GlobalVars instance = GlobalVars.Instance;
				Vector3 moveDir = component.MoveDir;
				float x = moveDir.x;
				Vector3 moveDir2 = component.MoveDir;
				float y = moveDir2.y;
				Vector3 moveDir3 = component.MoveDir;
				uint val = instance.NormalToUByte4(x, y, moveDir3.z);
				p2PMsgBody.Write(val);
				GlobalVars instance2 = GlobalVars.Instance;
				Vector3 position = gameObject.transform.position;
				ushort val2 = instance2.Float32toFloat16(position.x);
				GlobalVars instance3 = GlobalVars.Instance;
				Vector3 position2 = gameObject.transform.position;
				ushort val3 = instance3.Float32toFloat16(position2.y);
				GlobalVars instance4 = GlobalVars.Instance;
				Vector3 position3 = gameObject.transform.position;
				ushort val4 = instance4.Float32toFloat16(position3.z);
				p2PMsgBody.Write(val2);
				p2PMsgBody.Write(val3);
				p2PMsgBody.Write(val4);
				p2PMsgBody.Write(component.GetHorzAngle());
				p2PMsgBody.Write(component.GetVertAngle());
				p2PMsgBody.Write(MyInfoManager.Instance.IsBelow12());
				ReliableSend((uint)to, 26, p2PMsgBody);
			}
		}
	}

	private void HandlePEER_INITIATE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out float val3);
		msg.Read(out float val4);
		msg.Read(out uint val5);
		msg.Read(out ushort val6);
		msg.Read(out ushort val7);
		msg.Read(out ushort val8);
		msg.Read(out float val9);
		msg.Read(out float val10);
		msg.Read(out bool val11);
		Peer peer = Instance.Get(val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null && peer != null)
		{
			BF_PeerINITIATE bF_PeerINITIATE = default(BF_PeerINITIATE);
			bF_PeerINITIATE.bitvector1 = val2;
			Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val6), GlobalVars.Instance.Float16toFloat32(val7), GlobalVars.Instance.Float16toFloat32(val8));
			Vector3 dir = GlobalVars.Instance.UByte4ToNormal(val5);
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Move((LocalController.CONTROL_CONTEXT)bF_PeerINITIATE.cc, val3, val4, pos, 0.3f);
				component.Rotate(dir, val9, val10);
				component.SetWeapon((Weapon.TYPE)bF_PeerINITIATE.curWeaponType);
				component.IsChild = val11;
				if (bF_PeerINITIATE.dead)
				{
					component.Die(-1);
				}
				if (bF_PeerINITIATE.empty)
				{
					component.GunAnim(Weapon.TYPE.MAIN, 1);
				}
			}
			LookCoordinator component2 = gameObject.GetComponent<LookCoordinator>();
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (null != component2 && desc != null)
			{
				desc.IsGM = peer.IsGM();
				if (BuildOption.Instance.IsNetmarble && peer.IsGM())
				{
					Color value = new Color(1f, 0.82f, 0f, 1f);
					SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
					{
						skinnedMeshRenderer.material.SetColor("_OutlineColor", value);
						if (skinnedMeshRenderer.materials != null && skinnedMeshRenderer.materials.Length > 0)
						{
							for (int j = 0; j < skinnedMeshRenderer.materials.Length; j++)
							{
								skinnedMeshRenderer.materials[j].SetColor("_OutlineColor", value);
							}
						}
					}
					MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in componentsInChildren2)
					{
						meshRenderer.material.SetColor("_OutlineColor", value);
						if (meshRenderer.materials != null && meshRenderer.materials.Length > 0)
						{
							for (int l = 0; l < meshRenderer.materials.Length; l++)
							{
								meshRenderer.materials[l].SetColor("_OutlineColor", value);
							}
						}
					}
				}
				else if (desc.IsHostile())
				{
					Color value2 = new Color(1f, 0f, 0f, 0.5f);
					SkinnedMeshRenderer[] componentsInChildren3 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren3)
					{
						skinnedMeshRenderer2.material.SetColor("_OutlineColor", value2);
						if (skinnedMeshRenderer2.materials != null && skinnedMeshRenderer2.materials.Length > 0)
						{
							for (int n = 0; n < skinnedMeshRenderer2.materials.Length; n++)
							{
								skinnedMeshRenderer2.materials[n].SetColor("_OutlineColor", value2);
							}
						}
					}
					MeshRenderer[] componentsInChildren4 = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer2 in componentsInChildren4)
					{
						meshRenderer2.material.SetColor("_OutlineColor", value2);
						if (meshRenderer2.materials != null && meshRenderer2.materials.Length > 0)
						{
							for (int num2 = 0; num2 < meshRenderer2.materials.Length; num2++)
							{
								meshRenderer2.materials[num2].SetColor("_OutlineColor", value2);
							}
						}
					}
				}
			}
			if (desc != null)
			{
				if (null != component && !component.IsLocallyControlled)
				{
					desc.IsInvisibilityOn = bF_PeerINITIATE.invisibility;
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION && RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
				{
					MonManager.Instance.SendMonAll();
				}
			}
		}
	}

	private void HandlePEER_RELIABLE_ACK(P2PMsgBody msg)
	{
		msg.Read(out ushort val);
		if (queueReliable != null && queueReliable.Count > 0)
		{
			P2PMsg4Send p2PMsg4Send = queueReliable.Peek();
			if (p2PMsg4Send != null && p2PMsg4Send.Meta == val)
			{
				queueReliable.Dequeue();
			}
		}
	}

	private short ToShort(float val, float factor)
	{
		return (short)(val * factor);
	}

	private float ToFloat(short val, float factor)
	{
		return (float)val / factor;
	}

	public void SendPEER_DIE(int shooter)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(shooter);
		ReliableSend(uint.MaxValue, 10, p2PMsgBody);
	}

	private void HandlePEER_DIE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Die(val);
			}
		}
	}

	public void SendPEER_UNINVINCIBLE()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		ReliableSend(uint.MaxValue, 55, p2PMsgBody);
	}

	private void HandlePEER_UNINVINCIBLE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.UnInvincible();
			}
		}
	}

	public void SendPEER_RESPAWN()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		ReliableSend(uint.MaxValue, 11, p2PMsgBody);
	}

	private void HandlePEER_RESPAWN(P2PMsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Respawn();
			}
		}
	}

	public void SendPEER_HIT_IMPACT(int layer, Vector3 point, Vector3 normal)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		uint val = GlobalVars.Instance.NormalToUByte4(normal.x, normal.y, normal.z);
		p2PMsgBody.Write(layer);
		p2PMsgBody.Write(point.x);
		p2PMsgBody.Write(point.y);
		p2PMsgBody.Write(point.z);
		p2PMsgBody.Write(val);
		Say(48, p2PMsgBody);
	}

	public void HandlePEER_HIT_IMPACT(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out uint val5);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val2), GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4));
		Vector3 toDirection = GlobalVars.Instance.UByte4ToNormal(val5);
		GameObject impact = VfxOptimizer.Instance.GetImpact(val);
		if (impact != null)
		{
			VfxOptimizer.Instance.CreateFx(impact, pos, Quaternion.FromToRotation(Vector3.up, toDirection), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
		}
	}

	public void SendPEER_HIT_IMPACT_NEW(HitImpactPacket hitImpact)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(hitImpact.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(hitImpact.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(hitImpact.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(hitImpact.firePacket.shootdir.x, hitImpact.firePacket.shootdir.y, hitImpact.firePacket.shootdir.z);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(hitImpact.hitpoint.x);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(hitImpact.hitpoint.y);
		ushort val7 = GlobalVars.Instance.Float32toFloat16(hitImpact.hitpoint.z);
		uint val8 = GlobalVars.Instance.NormalToUByte4(hitImpact.hitnml.x, hitImpact.hitnml.y, hitImpact.hitnml.z);
		p2PMsgBody.Write(hitImpact.firePacket.shooter);
		p2PMsgBody.Write(hitImpact.firePacket.slot);
		p2PMsgBody.Write(hitImpact.firePacket.usID);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(hitImpact.layer);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		p2PMsgBody.Write(val7);
		p2PMsgBody.Write(val8);
		Say(77, p2PMsgBody);
	}

	public void HandlePEER_HIT_IMPACT_NEW(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out uint val7);
		msg.Read(out byte val8);
		msg.Read(out ushort val9);
		msg.Read(out ushort val10);
		msg.Read(out ushort val11);
		msg.Read(out uint val12);
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6));
		Vector3 direction = GlobalVars.Instance.UByte4ToNormal(val7);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val9), GlobalVars.Instance.Float16toFloat32(val10), GlobalVars.Instance.Float16toFloat32(val11));
		Vector3 toDirection = GlobalVars.Instance.UByte4ToNormal(val12);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && val2 >= 0)
			{
				component.Fire((Weapon.TYPE)val2, val3, origin, direction);
			}
		}
		GameObject impact = VfxOptimizer.Instance.GetImpact(val8);
		if (impact != null)
		{
			VfxOptimizer.Instance.CreateFx(impact, pos, Quaternion.FromToRotation(Vector3.up, toDirection), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
		}
	}

	public void SendPEER_BLAST_TIME(float delta)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(delta);
		Say(47, p2PMsgBody);
	}

	public void HandlePEER_BLAST_TIME(P2PMsgBody msg)
	{
		msg.Read(out float val);
		InstalledBomb installedBomb = null;
		GameObject gameObject = GameObject.Find("InstalledClockBomb");
		if (null != gameObject)
		{
			installedBomb = gameObject.GetComponent<InstalledBomb>();
			if (null != installedBomb)
			{
				installedBomb.SetDeltaTime(val);
			}
		}
	}

	public void SendPEER_SPECTATOR()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		bool isSpectator = MyInfoManager.Instance.IsSpectator;
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(isSpectator);
		Say(44, p2PMsgBody);
	}

	private void HandlePEER_SPECTATOR(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null && desc != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && !component.IsLocallyControlled)
			{
				desc.IsSpectator = val2;
			}
		}
	}

	public void SendPEER_CANNON_FIRE(int cannon, int shooter, Vector3 origin, Vector3 dir)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		uint val = GlobalVars.Instance.NormalToUByte4(dir.x, dir.y, dir.z);
		p2PMsgBody.Write(cannon);
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(origin.x);
		p2PMsgBody.Write(origin.y);
		p2PMsgBody.Write(origin.z);
		p2PMsgBody.Write(val);
		Say(30, p2PMsgBody);
	}

	private void HandlePEER_CANNON_FIRE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 direction = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject brickObject = BrickManager.Instance.GetBrickObject(val);
		if (null == brickObject)
		{
			Debug.LogError("ERROR, Fail to get cannon brick ");
		}
		else
		{
			GadgetCannon componentInChildren = brickObject.GetComponentInChildren<GadgetCannon>();
			if (null != componentInChildren)
			{
				componentInChildren.Fire(val, val2, origin, direction);
			}
		}
	}

	public void SendPEER_CANNON_MOVE(int cannon, int shooter, float x, float y)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		short val = ToShort(x, 10f);
		short val2 = ToShort(y, 10f);
		p2PMsgBody.Write(cannon);
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		Say(29, p2PMsgBody);
	}

	private void HandlePEER_CANNON_MOVE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out short val3);
		msg.Read(out short val4);
		float x = ToFloat(val3, 10f);
		float y = ToFloat(val4, 10f);
		GameObject brickObject = BrickManager.Instance.GetBrickObject(val);
		if (null == brickObject)
		{
			Debug.LogError("ERROR, Fail to get cannon brick ");
		}
		else
		{
			GadgetCannon componentInChildren = brickObject.GetComponentInChildren<GadgetCannon>();
			if (null != componentInChildren)
			{
				componentInChildren.Move(val, val2, x, y);
			}
		}
	}

	public void SendPEER_COMPOSE(bool isDel)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(isDel);
		Say(22, p2PMsgBody);
	}

	private void HandlePEER_COMPOSE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Compose(val2);
			}
		}
	}

	public void SendPEER_FALL_DOWN()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		Say(28, p2PMsgBody);
	}

	private void HandlePEER_FALL_DOWN(P2PMsgBody msg)
	{
		msg.Read(out int val);
		if (val != MyInfoManager.Instance.Seq)
		{
			GameObject gameObject = BrickManManager.Instance.Get(val);
			if (null != gameObject)
			{
				TPController component = gameObject.GetComponent<TPController>();
				if (null != component)
				{
					component.GetHit(-1, -1);
				}
			}
		}
	}

	public void SendPEER_PIERCE(int slasher, int pierced, int damage, float rigidity, int weaponBy)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)damage;
		ushort val2 = (ushort)weaponBy;
		p2PMsgBody.Write(slasher);
		p2PMsgBody.Write(pierced);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(rigidity);
		p2PMsgBody.Write(val2);
		Say(13, p2PMsgBody);
	}

	private void HandlePEER_PIERCE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out float val4);
		msg.Read(out ushort val5);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			if (val2 == MyInfoManager.Instance.Seq)
			{
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					LocalController component = gameObject.GetComponent<LocalController>();
					if (null != component)
					{
						component.GetHit(val, val3, val4, val5, -1, autoHealPossible: true, checkZombie: true);
						SendPEER_HP(val, val2, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component.HitPoint), component.GetMaxHp(), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.Armor), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.MaxArmor));
					}
				}
				GameObject gameObject2 = GameObject.Find("Main");
				if (null != gameObject2 && val3 > 0)
				{
					gameObject2.BroadcastMessage("OnDirectionalHit", val);
				}
			}
			else
			{
				GameObject gameObject3 = BrickManManager.Instance.Get(val2);
				if (null != gameObject3)
				{
					TPController component2 = gameObject3.GetComponent<TPController>();
					if (null != component2)
					{
						component2.GetHit(val3, val2);
					}
				}
			}
		}
	}

	public void SendPEER_BRICK_HITPOINT(int brick, int hp)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)hp;
		p2PMsgBody.Write(brick);
		p2PMsgBody.Write(val);
		Say(21, p2PMsgBody);
	}

	private void HandlePEER_BRICK_HITPOINT(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		BrickManager.Instance.SetBrickHitpoint(val, val2);
	}

	public void SendPEER_BOMBED(int bomber, int bombed, int damage, float rigidity, int weaponBy)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)damage;
		ushort val2 = (ushort)weaponBy;
		p2PMsgBody.Write(bomber);
		p2PMsgBody.Write(bombed);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(rigidity);
		p2PMsgBody.Write(val2);
		Say(15, p2PMsgBody);
	}

	private void HandlePEER_BOMBED(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out float val4);
		msg.Read(out ushort val5);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			if (val2 == MyInfoManager.Instance.Seq)
			{
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					LocalController component = gameObject.GetComponent<LocalController>();
					if (null != component)
					{
						component.GetHit(val, val3, val4, val5, -1, autoHealPossible: true, checkZombie: false);
						SendPEER_HP(val, val2, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component.HitPoint), component.GetMaxHp(), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.Armor), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.MaxArmor));
					}
				}
			}
			else
			{
				GameObject gameObject2 = BrickManManager.Instance.Get(val2);
				if (null != gameObject2)
				{
					TPController component2 = gameObject2.GetComponent<TPController>();
					if (null != component2)
					{
						component2.GetHit(val3, val2);
					}
				}
			}
		}
	}

	public void SendPEER_HP(int shooter, int hitman, int hp, int hpMax, int armor, int maxArmor)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		BF_PeerCurMax bF_PeerCurMax = default(BF_PeerCurMax);
		bF_PeerCurMax.cur = (uint)hp;
		bF_PeerCurMax.max = (uint)hpMax;
		BF_PeerCurMax bF_PeerCurMax2 = default(BF_PeerCurMax);
		bF_PeerCurMax2.cur = (uint)armor;
		bF_PeerCurMax2.max = (uint)maxArmor;
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(hitman);
		p2PMsgBody.Write(bF_PeerCurMax.bitvector1);
		p2PMsgBody.Write(bF_PeerCurMax2.bitvector1);
		Whisper(shooter, 60, p2PMsgBody);
	}

	private void HandlePEER_HP(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out uint val3);
		msg.Read(out uint val4);
		BF_PeerCurMax bF_PeerCurMax = default(BF_PeerCurMax);
		BF_PeerCurMax bF_PeerCurMax2 = default(BF_PeerCurMax);
		bF_PeerCurMax.bitvector1 = val3;
		bF_PeerCurMax2.bitvector1 = val4;
		if (MyInfoManager.Instance.Seq == val)
		{
			GameObject gameObject = BrickManManager.Instance.Get(val2);
			if (null != gameObject)
			{
				PlayerProperty component = gameObject.GetComponent<PlayerProperty>();
				if (null != component)
				{
					component.Desc.Hp = (int)bF_PeerCurMax.cur;
					component.Desc.MaxHp = (int)bF_PeerCurMax.max;
					component.Desc.Armor = (int)bF_PeerCurMax2.cur;
					component.Desc.MaxArmor = (int)bF_PeerCurMax2.max;
				}
				TPController component2 = gameObject.GetComponent<TPController>();
				if (null != component2)
				{
					component2.OnHitEvent();
				}
			}
			GameObject gameObject2 = GameObject.Find("Me");
			if (null != gameObject2)
			{
				EquipCoordinator component3 = gameObject2.GetComponent<EquipCoordinator>();
				if (null != component3)
				{
					component3.SetShootEnermyEffect();
				}
			}
		}
	}

	public void SendPEER_SHOOT(int shooter, int hit, int damage, float rigidity, int weaponBy, int hitpart, bool lucky, float rateOfFire)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		BF_PeerSHOOT bF_PeerSHOOT = default(BF_PeerSHOOT);
		bF_PeerSHOOT.damage = damage;
		bF_PeerSHOOT.hitpart = hitpart;
		bF_PeerSHOOT.lucky = lucky;
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(hit);
		p2PMsgBody.Write(bF_PeerSHOOT.bitvector1);
		p2PMsgBody.Write(rigidity);
		p2PMsgBody.Write(weaponBy);
		p2PMsgBody.Write(rateOfFire);
		Say(9, p2PMsgBody);
	}

	private void HandlePEER_SHOOT(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out float val4);
		msg.Read(out int val5);
		msg.Read(out float _);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			BF_PeerSHOOT bF_PeerSHOOT = default(BF_PeerSHOOT);
			bF_PeerSHOOT.bitvector1 = val3;
			if (val2 == MyInfoManager.Instance.Seq)
			{
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					LocalController component = gameObject.GetComponent<LocalController>();
					if (null != component && NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component.HitPoint) > 0)
					{
						component.SetLucky(bF_PeerSHOOT.lucky);
						component.GetHit(val, bF_PeerSHOOT.damage, val4, val5, bF_PeerSHOOT.hitpart, autoHealPossible: true, checkZombie: false);
						SendPEER_HP(val, val2, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component.HitPoint), component.GetMaxHp(), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.Armor), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component.MaxArmor));
					}
				}
				GameObject gameObject2 = GameObject.Find("Main");
				if (null != gameObject2 && bF_PeerSHOOT.damage > 0)
				{
					gameObject2.BroadcastMessage("OnDirectionalHit", val);
				}
			}
			else
			{
				GameObject gameObject3 = BrickManManager.Instance.Get(val2);
				if (null != gameObject3)
				{
					TPController component2 = gameObject3.GetComponent<TPController>();
					if (null != component2)
					{
						component2.GetHit(bF_PeerSHOOT.damage, val2);
					}
				}
			}
		}
	}

	public void SendPEER_SLASH(int slasher, float slashSpeed)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(slasher);
		p2PMsgBody.Write(slashSpeed);
		Say(12, p2PMsgBody);
	}

	private void HandlePEER_SLASH(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out float val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			component.Slash(val2);
		}
	}

	public void SendPEER_PROJECTILE_FLY(int thrower, int projectile, Vector3 pos, Vector3 rot, float range)
	{
		ushort val = (ushort)projectile;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(range);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		Say(16, p2PMsgBody);
	}

	private void HandlePEER_PROJECTILE_FLY(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		msg.Read(out ushort val7);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 rot = GlobalVars.Instance.UByte4ToNormal(val6);
		float range = GlobalVars.Instance.Float16toFloat32(val7);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.ProjectileFly(val2, pos, rot, range);
			}
		}
	}

	public void SendPEER_PROJECTILE_KABOOM(int thrower, int projectile)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		short val = (short)projectile;
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		ReliableSend(uint.MaxValue, 17, p2PMsgBody);
	}

	private void HandlePEER_PROJECTILE_KABOOM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out short val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.ProjectileKaboom(val2);
			}
		}
	}

	public void SendPEER_PROJECTILE(int thrower, int projectile, Vector3 pos, Vector3 rot)
	{
		ushort val = (ushort)projectile;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		ReliableSend(uint.MaxValue, 14, p2PMsgBody);
	}

	private void HandlePEER_PROJECTILE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 rot = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.ThrowProjectile(val2, pos, rot);
			}
		}
	}

	public void SendPEER_SENSEBEAM(int thrower, int projectile, Vector3 pos, Vector3 rot)
	{
		ushort val = (ushort)projectile;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		ReliableSend(uint.MaxValue, 65, p2PMsgBody);
	}

	private void HandlePEER_SENSEBEAM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 normal = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.SetSenseBeam(0, val2, pos, normal);
			}
		}
	}

	public void SendPEER_BIG_WPN_FIRE(int thrower, int launcher, int projectile, Vector3 pos, Vector3 rot)
	{
		byte val = (byte)launcher;
		ushort val2 = (ushort)projectile;
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val6 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		ReliableSend(uint.MaxValue, 61, p2PMsgBody);
	}

	private void HandlePEER_BIG_WPN_FIRE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out uint val7);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6));
		Vector3 rot = GlobalVars.Instance.UByte4ToNormal(val7);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.FireRW(val2, val3, pos, rot);
			}
		}
	}

	public void SendPEER_BIG_WPN_FLY(int thrower, int projectile, Vector3 pos, Vector3 rot)
	{
		ushort val = (ushort)projectile;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		Say(62, p2PMsgBody);
	}

	private void HandlePEER_BIG_WPN_FLY(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 rot = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.FlyRW(val2, pos, rot);
			}
		}
	}

	public void SendPEER_BIG_WPN_KABOOM(int thrower, int projectile, Vector3 pos, Vector3 rot, bool viewColeff)
	{
		ushort val = (ushort)projectile;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(rot.x, rot.y, rot.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(thrower);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(viewColeff);
		ReliableSend(uint.MaxValue, 63, p2PMsgBody);
	}

	private void HandlePEER_BIG_WPN_KABOOM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		msg.Read(out bool val7);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 rot = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && desc != null)
			{
				component.KaBoomRW(val2, pos, rot, val7);
			}
		}
	}

	public void SendPEER_BOOST(int seq, int brickSeq, int boost)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)boost;
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(brickSeq);
		p2PMsgBody.Write(val);
		ReliableSend(uint.MaxValue, 64, p2PMsgBody);
	}

	private void HandlePEER_BOOST(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out byte val3);
		int num = val3;
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				switch (num)
				{
				case 159:
					component.OnTrampoline(val2);
					break;
				case 160:
					component.OnBoost(val2);
					break;
				}
			}
		}
	}

	public void SendPEER_FIRE(int seq, int slot, int tile, Vector3 origin, Vector3 direction)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		short val = ToShort(origin.x, 100f);
		short val2 = ToShort(origin.y, 100f);
		short val3 = ToShort(origin.z, 100f);
		short val4 = ToShort(direction.x, 100f);
		short val5 = ToShort(direction.y, 100f);
		short val6 = ToShort(direction.z, 100f);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(slot);
		p2PMsgBody.Write(tile);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		Say(8, p2PMsgBody);
	}

	private void HandlePEER_FIRE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out short val4);
		msg.Read(out short val5);
		msg.Read(out short val6);
		msg.Read(out short val7);
		msg.Read(out short val8);
		msg.Read(out short val9);
		Vector3 origin = new Vector3(ToFloat(val4, 100f), ToFloat(val5, 100f), ToFloat(val6, 100f));
		Vector3 direction = new Vector3(ToFloat(val7, 100f), ToFloat(val8, 100f), ToFloat(val9, 100f));
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Fire((Weapon.TYPE)val2, val3, origin, direction);
			}
		}
	}

	public void SendPEER_FIRE_NEW(FirePacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.shootdir.x, cls.shootdir.y, cls.shootdir.z);
		BF_PeerFIRE bF_PeerFIRE = default(BF_PeerFIRE);
		bF_PeerFIRE.slot = cls.slot;
		bF_PeerFIRE.ammoId = cls.usID;
		p2PMsgBody.Write(cls.shooter);
		p2PMsgBody.Write(bF_PeerFIRE.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		Say(73, p2PMsgBody);
	}

	private void HandlePEER_FIRE_NEW(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		BF_PeerFIRE bF_PeerFIRE = default(BF_PeerFIRE);
		bF_PeerFIRE.bitvector1 = val2;
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 direction = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Fire((Weapon.TYPE)bF_PeerFIRE.slot, bF_PeerFIRE.ammoId, origin, direction);
			}
		}
	}

	public void SendPEER_FIRE_W(int to, FirePacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.shootdir.x, cls.shootdir.y, cls.shootdir.z);
		BF_PeerFIRE bF_PeerFIRE = default(BF_PeerFIRE);
		bF_PeerFIRE.slot = cls.slot;
		bF_PeerFIRE.ammoId = cls.usID;
		p2PMsgBody.Write(cls.shooter);
		p2PMsgBody.Write(bF_PeerFIRE.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		Whisper(to, 80, p2PMsgBody);
	}

	private void HandlePEER_FIRE_W(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		BF_PeerFIRE bF_PeerFIRE = default(BF_PeerFIRE);
		bF_PeerFIRE.bitvector1 = val2;
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 direction = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Fire((Weapon.TYPE)bF_PeerFIRE.slot, bF_PeerFIRE.ammoId, origin, direction);
			}
		}
	}

	public void SendPEER_FIRE_ACTION_W(int to, FireSndPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(cls.shooter);
		p2PMsgBody.Write(cls.slot);
		Whisper(to, 79, p2PMsgBody);
	}

	private void HandlePEER_FIRE_ACTION_W(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.FireAction((Weapon.TYPE)val2);
			}
		}
	}

	public void SendPEER_BIG_FIRE(int seq, int slot, int tile, Vector3 origin, Vector3 direction)
	{
		byte val = (byte)slot;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(origin.x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(origin.y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(origin.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(direction.x, direction.y, direction.z);
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(tile);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		ReliableSend(uint.MaxValue, 69, p2PMsgBody);
	}

	private void HandlePEER_BIG_FIRE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out int val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out uint val7);
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6));
		Vector3 direction = GlobalVars.Instance.UByte4ToNormal(val7);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && desc != null)
			{
				component.Fire((Weapon.TYPE)val2, val3, origin, direction);
			}
		}
	}

	public void SendPEER_PORTAL(int brickSeq, bool ppongOnly = false)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(brickSeq);
		p2PMsgBody.Write(ppongOnly);
		ReliableSend(uint.MaxValue, 70, p2PMsgBody);
	}

	private void HandlePEER_PORTAL(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate((UnityEngine.Object)GlobalVars.Instance.fxPortalPPong, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
		}
		if (!val3)
		{
			BrickManager.Instance.portalFX(val2);
		}
	}

	public void SendPEER_BRICK_ANIM(int index, int brickSeq, string strAnim)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)index;
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(brickSeq);
		p2PMsgBody.Write(strAnim);
		Say(71, p2PMsgBody);
	}

	private void HandlePEER_BRICK_ANIM(P2PMsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		BrickManager.Instance.AnimationPlay(val, val2, val3, IsP2P: true);
	}

	public void SendPEER_BRICK_ANIM_CROSSFADE(int brickSeq, string strAnim)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(brickSeq);
		p2PMsgBody.Write(strAnim);
		Say(93, p2PMsgBody);
	}

	private void HandlePEER_BRICK_ANIM_CROSSFADE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		BrickManager.Instance.AnimationCrossFade(val, val2, IsP2P: true);
	}

	public void SendPEER_INVISIBLILITY(bool isInvisibilityOn)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(isInvisibilityOn);
		if (isInvisibilityOn)
		{
			GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.INVISIBLE_USE);
		}
		ReliableSend(uint.MaxValue, 72, p2PMsgBody);
	}

	private void HandlePEER_INVISIBLILITY(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null && desc != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && !component.IsLocallyControlled)
			{
				desc.IsInvisibilityOn = val2;
			}
		}
	}

	public void SendPEER_THROW()
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		Say(5, p2PMsgBody);
	}

	private void HandlePEER_THROW(P2PMsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			component.Throw();
		}
	}

	public void SendPEER_RELOAD(int seq, int slot)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)slot;
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		Say(7, p2PMsgBody);
	}

	private void HandlePEER_RELOAD(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
        GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Reload((Weapon.TYPE)val2);
			}
		}
	}

	public void SendPEER_ENABLE_HANDBOMB(int seq, bool enable)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(enable);
		Say(34, p2PMsgBody);
	}

	private void HandlePEER_ENABLE_HANDBOMB(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (null != gameObject && desc != null)
		{
			if (desc.IsHidePlayer)
			{
				val2 = false;
			}
			LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
			if (null != component)
			{
				component.EnableHandbomb(val2);
			}
		}
	}

	public void SendPEER_WEAPON_STATUS(int seq, int slot)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)slot;
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		Say(23, p2PMsgBody);
	}

	private void HandlePEER_WEAPON_STATUS(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
			if (null != component)
			{
				component.ChangeWeapon((Weapon.TYPE)val2);
			}
		}
	}

	public void SendPEER_SWAP_WEAPON(int seq, int slot)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)slot;
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		Say(6, p2PMsgBody);
	}

	private void HandlePEER_SWAP_WEAPON(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.SwapWeapon((Weapon.TYPE)val2);
			}
		}
	}

	public void SendPEER_CONSUME(int player, string function)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(player);
		p2PMsgBody.Write(function);
		Say(54, p2PMsgBody);
	}

	private void HandlePEER_CONSUME(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				if (val2 == "heal")
				{
					component.Heal();
				}
				else if (val2 == "speedup")
				{
					component.Speedup();
				}
				else if (val2 == "speedup_end")
				{
					component.SpeedupEnd();
				}
			}
		}
	}

	public void SendPEER_DIR(int seq, Vector3 dir, float xAngle, float yAngle)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		uint val = GlobalVars.Instance.NormalToUByte4(dir.x, dir.y, dir.z);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(xAngle);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(yAngle);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		Say(53, p2PMsgBody);
	}

	private void HandlePEER_DIR(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		Vector3 dir = GlobalVars.Instance.UByte4ToNormal(val2);
		float xAngle = GlobalVars.Instance.Float16toFloat32(val3);
		float yAngle = GlobalVars.Instance.Float16toFloat32(val4);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Rotate(dir, xAngle, yAngle);
			}
		}
	}

	public void SendPEER_DIR_W(int to, int seq, Vector3 dir, float xAngle, float yAngle)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		uint val = GlobalVars.Instance.NormalToUByte4(dir.x, dir.y, dir.z);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(xAngle);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(yAngle);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		Whisper(to, 81, p2PMsgBody);
	}

	private void HandlePEER_DIR_W(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		Vector3 dir = GlobalVars.Instance.UByte4ToNormal(val2);
		float xAngle = GlobalVars.Instance.Float16toFloat32(val3);
		float yAngle = GlobalVars.Instance.Float16toFloat32(val4);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Rotate(dir, xAngle, yAngle);
			}
		}
	}

	public void SendPEER_MOVE(int seq, int cc, float speed, float vSpeed, Vector3 pos, bool isDead, bool isRegularSend)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(speed);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(vSpeed);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(pos.z);
		BF_PeeMOVE bF_PeeMOVE = default(BF_PeeMOVE);
		bF_PeeMOVE.cc = (byte)cc;
		bF_PeeMOVE.isDead = isDead;
		bF_PeeMOVE.isRegularSend = isRegularSend;
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(bF_PeeMOVE.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		Say(2, p2PMsgBody);
	}

	private void HandlePEER_MOVE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out ushort val7);
		BF_PeeMOVE bF_PeeMOVE = default(BF_PeeMOVE);
		bF_PeeMOVE.bitvector1 = val2;
		bool isDead = bF_PeeMOVE.isDead;
		float speed = GlobalVars.Instance.Float16toFloat32(val3);
		float vSpeed = GlobalVars.Instance.Float16toFloat32(val4);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6), GlobalVars.Instance.Float16toFloat32(val7));
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				if (component.IsDead && !isDead)
				{
					component.Respawn();
				}
				else if (!component.IsDead && isDead)
				{
					component.Die(-1);
				}
				component.Move((LocalController.CONTROL_CONTEXT)bF_PeeMOVE.cc, speed, vSpeed, pos, 0.3f);
			}
		}
	}

	public void SendPEER_MOVE_W(int to, int cc, float speed, float vSpeed, Vector3 pos, bool isDead, bool isRegularSend)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(speed);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(vSpeed);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(pos.z);
		BF_PeeMOVE bF_PeeMOVE = default(BF_PeeMOVE);
		bF_PeeMOVE.cc = (byte)cc;
		bF_PeeMOVE.isDead = isDead;
		bF_PeeMOVE.isRegularSend = isRegularSend;
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(bF_PeeMOVE.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		Whisper(to, 78, p2PMsgBody);
	}

	private void HandlePEER_MOVE_W(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out ushort val7);
		BF_PeeMOVE bF_PeeMOVE = default(BF_PeeMOVE);
		bF_PeeMOVE.bitvector1 = val2;
		bool isDead = bF_PeeMOVE.isDead;
		float speed = GlobalVars.Instance.Float16toFloat32(val3);
		float vSpeed = GlobalVars.Instance.Float16toFloat32(val4);
		Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6), GlobalVars.Instance.Float16toFloat32(val7));
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (gameObject != null)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				if (component.IsDead && !isDead)
				{
					component.Respawn();
				}
				else if (!component.IsDead && isDead)
				{
					component.Die(-1);
				}
				component.Move((LocalController.CONTROL_CONTEXT)bF_PeeMOVE.cc, speed, vSpeed, pos, 0.3f);
			}
		}
	}

	public void SendPEER_HIT_BRICK(int seq, int index, Vector3 pos, Vector3 normal, bool isBullet)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(normal.x, normal.y, normal.z);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(index);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(isBullet);
		if (!isBullet)
		{
			p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		}
		Say(31, p2PMsgBody);
	}

	private void HandlePEER_HIT_BRICK(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		msg.Read(out bool val7);
		Vector3 vector = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 vector2 = GlobalVars.Instance.UByte4ToNormal(val6);
		if (val7)
		{
			Brick brick = BrickManager.Instance.GetBrick(val2);
			GameObject brickObject = BrickManager.Instance.GetBrickObject(val);
			if (null != brickObject && brick != null)
			{
				BrickProperty componentInChildren = brickObject.GetComponentInChildren<BrickProperty>();
				if (null != componentInChildren)
				{
					Texture2D bulletMark = brick.GetBulletMark();
					GameObject bulletImpact = brick.GetBulletImpact();
					if (null != bulletMark)
					{
						GameObject gameObject = VfxOptimizer.Instance.CreateFxImmediate(BrickManager.Instance.bulletMark, vector, Quaternion.FromToRotation(Vector3.forward, -vector2), VfxOptimizer.VFX_TYPE.BULLET_MARK);
						if (gameObject != null)
						{
							GameObject mesh = brickObject;
							if (brick.NeedChunkOptimize())
							{
								mesh = brickObject.transform.parent.gameObject;
							}
							BulletMark component = gameObject.GetComponent<BulletMark>();
							component.GenerateDecal(bulletMark, mesh, brickObject);
						}
					}
					if (null != bulletImpact)
					{
						VfxOptimizer.Instance.CreateFx(bulletImpact, vector, Quaternion.FromToRotation(Vector3.up, vector2), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
					}
				}
			}
		}
		else
		{
			msg.Read(out int val8);
			if (val8 != MyInfoManager.Instance.Seq)
			{
				GameObject gameObject2 = BrickManManager.Instance.Get(val8);
				if (null != gameObject2)
				{
					Weapon.TYPE tYPE = Weapon.TYPE.COUNT;
					TPController component2 = gameObject2.GetComponent<TPController>();
					if (null != component2)
					{
						tYPE = component2.GetCurrentWeaponType();
					}
					if (tYPE == Weapon.TYPE.MAIN || tYPE == Weapon.TYPE.AUX)
					{
						GdgtGun componentInChildren2 = gameObject2.GetComponentInChildren<GdgtGun>();
						if (componentInChildren2 != null)
						{
							componentInChildren2.createBullet(vector, vector2, val, val2, 1);
						}
					}
				}
			}
		}
	}

	public void SendPEER_HIT_BRICK_NEW(HitBrickPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.firePacket.shootdir.x, cls.firePacket.shootdir.y, cls.firePacket.shootdir.z);
		p2PMsgBody.Write(cls.firePacket.shooter);
		p2PMsgBody.Write(cls.firePacket.slot);
		p2PMsgBody.Write(cls.firePacket.usID);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(cls.isBullet);
		p2PMsgBody.Write(cls.brickseq);
		p2PMsgBody.Write(cls.hitpoint.x);
		p2PMsgBody.Write(cls.hitpoint.y);
		p2PMsgBody.Write(cls.hitpoint.z);
		p2PMsgBody.Write(cls.hitnml.x);
		p2PMsgBody.Write(cls.hitnml.y);
		p2PMsgBody.Write(cls.hitnml.z);
		p2PMsgBody.Write(cls.destructable);
		if (cls.destructable)
		{
			p2PMsgBody.Write(cls.layer);
			p2PMsgBody.Write(cls.damage);
		}
		if (!cls.isBullet)
		{
			p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		}
		Say(74, p2PMsgBody);
	}

	private void HandlePEER_HIT_BRICK_NEW(P2PMsgBody msg)
	{
		byte val = byte.MaxValue;
		ushort val2 = 0;
		msg.Read(out int val3);
		msg.Read(out byte val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out ushort val7);
		msg.Read(out ushort val8);
		msg.Read(out uint val9);
		msg.Read(out bool val10);
		msg.Read(out int val11);
		msg.Read(out float val12);
		msg.Read(out float val13);
		msg.Read(out float val14);
		msg.Read(out float val15);
		msg.Read(out float val16);
		msg.Read(out float val17);
		msg.Read(out bool val18);
		if (val18)
		{
			msg.Read(out val);
			msg.Read(out val2);
		}
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val6), GlobalVars.Instance.Float16toFloat32(val7), GlobalVars.Instance.Float16toFloat32(val8));
		Vector3 vector = GlobalVars.Instance.UByte4ToNormal(val9);
		Vector3 vector2 = new Vector3(val12, val13, val14);
		Vector3 vector3 = new Vector3(val15, val16, val17);
		GameObject gameObject = BrickManManager.Instance.Get(val3);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && val4 >= 0)
			{
				component.Fire((Weapon.TYPE)val4, val5, origin, vector);
			}
		}
		if (val10)
		{
			Brick brick = BrickManager.Instance.GetBrick(val5);
			GameObject brickObject = BrickManager.Instance.GetBrickObject(val11);
			if (null != brickObject && brick != null)
			{
				BrickProperty componentInChildren = brickObject.GetComponentInChildren<BrickProperty>();
				if (null != componentInChildren)
				{
					Texture2D bulletMark = brick.GetBulletMark();
					GameObject bulletImpact = brick.GetBulletImpact();
					if (null != bulletMark)
					{
						GameObject gameObject2 = VfxOptimizer.Instance.CreateFxImmediate(BrickManager.Instance.bulletMark, vector2, Quaternion.FromToRotation(Vector3.forward, -vector3), VfxOptimizer.VFX_TYPE.BULLET_MARK);
						if (gameObject2 != null)
						{
							GameObject mesh = brickObject;
							if (brick.NeedChunkOptimize())
							{
								mesh = brickObject.transform.parent.gameObject;
							}
							BulletMark component2 = gameObject2.GetComponent<BulletMark>();
							component2.GenerateDecal(bulletMark, mesh, brickObject);
						}
					}
					if (null != bulletImpact)
					{
						VfxOptimizer.Instance.CreateFx(bulletImpact, vector2, Quaternion.FromToRotation(Vector3.up, vector3), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
					}
				}
			}
		}
		else
		{
			msg.Read(out int val19);
			if (val19 != MyInfoManager.Instance.Seq)
			{
				GameObject gameObject3 = BrickManManager.Instance.Get(val19);
				if (null != gameObject3 && (val4 == 2 || val4 == 1))
				{
					GdgtGun[] componentsInChildren = gameObject3.GetComponentsInChildren<GdgtGun>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i] != null && componentsInChildren[i].GetComponent<Weapon>().slot == (Weapon.TYPE)val4)
						{
							componentsInChildren[i].createBullet(vector2, vector, val11, val5, 1);
						}
					}
				}
			}
		}
		if (val18)
		{
			if (val2 > 0)
			{
				BrickManager.Instance.SetBrickHitpoint(val11, val2);
			}
			GameObject impact = VfxOptimizer.Instance.GetImpact(val);
			if (impact != null)
			{
				VfxOptimizer.Instance.CreateFx(impact, vector2, Quaternion.FromToRotation(Vector3.up, vector3), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
			}
		}
	}

	public void SendPEER_HIT_BRICK_W(int to, HitBrickPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.firePacket.shootdir.x, cls.firePacket.shootdir.y, cls.firePacket.shootdir.z);
		p2PMsgBody.Write(cls.firePacket.shooter);
		p2PMsgBody.Write(cls.firePacket.slot);
		p2PMsgBody.Write(cls.firePacket.usID);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(cls.isBullet);
		p2PMsgBody.Write(cls.brickseq);
		p2PMsgBody.Write(cls.hitpoint.x);
		p2PMsgBody.Write(cls.hitpoint.y);
		p2PMsgBody.Write(cls.hitpoint.z);
		p2PMsgBody.Write(cls.hitnml.x);
		p2PMsgBody.Write(cls.hitnml.y);
		p2PMsgBody.Write(cls.hitnml.z);
		p2PMsgBody.Write(cls.destructable);
		if (cls.destructable)
		{
			p2PMsgBody.Write(cls.layer);
			p2PMsgBody.Write(cls.damage);
		}
		if (!cls.isBullet)
		{
			p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		}
		Whisper(to, 74, p2PMsgBody);
	}

	public void SendPEER_HIT_BRICKMAN(int shooter, int seq, int part, Vector3 pos, Vector3 normal, bool lucky, int curammo, Vector3 dir)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(normal.x, normal.y, normal.z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(dir.x, dir.y, dir.z);
		BF_PEER_HIT_BRICKMAN bF_PEER_HIT_BRICKMAN = default(BF_PEER_HIT_BRICKMAN);
		bF_PEER_HIT_BRICKMAN.lucky = lucky;
		bF_PEER_HIT_BRICKMAN.part = part;
		bF_PEER_HIT_BRICKMAN.curammo = curammo;
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(bF_PEER_HIT_BRICKMAN.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		Say(32, p2PMsgBody);
	}

	private void HandlePEER_HIT_BRICKMAN(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out uint val7);
		msg.Read(out uint val8);
		BF_PEER_HIT_BRICKMAN bF_PEER_HIT_BRICKMAN = default(BF_PEER_HIT_BRICKMAN);
		bF_PEER_HIT_BRICKMAN.bitvector1 = val3;
		if (val2 != MyInfoManager.Instance.Seq)
		{
			Vector3 vector = new Vector3(GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6));
			Vector3 toDirection = GlobalVars.Instance.UByte4ToNormal(val7);
			Vector3 nml = GlobalVars.Instance.UByte4ToNormal(val8);
			GameObject gameObject = BrickManManager.Instance.Get(val2);
			if (null != gameObject)
			{
				HitPart[] componentsInChildren = gameObject.GetComponentsInChildren<HitPart>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].part == (HitPart.TYPE)bF_PEER_HIT_BRICKMAN.part)
					{
						GlobalVars.Instance.hitParent = componentsInChildren[i].transform;
						GameObject pfb = componentsInChildren[i].hitImpact;
						if (bF_PEER_HIT_BRICKMAN.lucky && null != componentsInChildren[i].luckyImpact)
						{
							pfb = componentsInChildren[i].luckyImpact;
						}
						VfxOptimizer.Instance.CreateFx(pfb, vector, Quaternion.FromToRotation(Vector3.up, toDirection), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
						break;
					}
				}
				GlobalVars.Instance.hitBirckman = val2;
				GameObject gameObject2 = BrickManManager.Instance.Get(val);
				if (null != gameObject2)
				{
					Weapon.TYPE tYPE = Weapon.TYPE.COUNT;
					TPController component = gameObject2.GetComponent<TPController>();
					if (null != component)
					{
						tYPE = component.GetCurrentWeaponType();
					}
					if (tYPE == Weapon.TYPE.MAIN || tYPE == Weapon.TYPE.AUX)
					{
						GdgtGun componentInChildren = gameObject2.GetComponentInChildren<GdgtGun>();
						if (componentInChildren != null)
						{
							componentInChildren.createBullet(vector, nml, -1, bF_PEER_HIT_BRICKMAN.curammo, 0);
						}
					}
				}
			}
		}
		else
		{
			GameObject gameObject3 = BrickManManager.Instance.Get(val);
			if (null != gameObject3)
			{
				GdgtGun componentInChildren2 = gameObject3.GetComponentInChildren<GdgtGun>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.destroyBullet(bF_PEER_HIT_BRICKMAN.curammo);
				}
			}
		}
	}

	public void SendPEER_HIT_BRICKMAN_NEW(HitManPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.firePacket.shootdir.x, cls.firePacket.shootdir.y, cls.firePacket.shootdir.z);
		BF_PeerHITBRICKMAN_NEW bF_PeerHITBRICKMAN_NEW = default(BF_PeerHITBRICKMAN_NEW);
		bF_PeerHITBRICKMAN_NEW.slot = cls.firePacket.slot;
		bF_PeerHITBRICKMAN_NEW.ammoId = cls.firePacket.usID;
		bF_PeerHITBRICKMAN_NEW.damage = cls.damage;
		bF_PeerHITBRICKMAN_NEW.hitpart = cls.hitPart;
		bF_PeerHITBRICKMAN_NEW.lucky = (uint)(cls.bLucky ? 1 : 0);
		p2PMsgBody.Write(cls.firePacket.shooter);
		p2PMsgBody.Write(bF_PeerHITBRICKMAN_NEW.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.x);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.y);
		ushort val7 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.z);
		uint val8 = GlobalVars.Instance.NormalToUByte4(cls.hitnml.x, cls.hitnml.y, cls.hitnml.z);
		p2PMsgBody.Write(cls.hitMan);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		p2PMsgBody.Write(val7);
		p2PMsgBody.Write(val8);
		p2PMsgBody.Write(cls.rigidity);
		p2PMsgBody.Write(cls.weaponBy);
		Say(76, p2PMsgBody);
	}

	private void HandlePEER_HIT_BRICKMAN_NEW(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		BF_PeerHITBRICKMAN_NEW bF_PeerHITBRICKMAN_NEW = default(BF_PeerHITBRICKMAN_NEW);
		bF_PeerHITBRICKMAN_NEW.bitvector1 = val2;
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 vector = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && bF_PeerHITBRICKMAN_NEW.slot >= 0)
			{
				component.Fire((Weapon.TYPE)bF_PeerHITBRICKMAN_NEW.slot, (int)bF_PeerHITBRICKMAN_NEW.ammoId, origin, vector);
			}
		}
		msg.Read(out int val7);
		msg.Read(out ushort val8);
		msg.Read(out ushort val9);
		msg.Read(out ushort val10);
		msg.Read(out uint val11);
		msg.Read(out float val12);
		msg.Read(out ushort val13);
		Vector3 vector2 = new Vector3(GlobalVars.Instance.Float16toFloat32(val8), GlobalVars.Instance.Float16toFloat32(val9), GlobalVars.Instance.Float16toFloat32(val10));
		Vector3 toDirection = GlobalVars.Instance.UByte4ToNormal(val11);
		if (val7 != MyInfoManager.Instance.Seq)
		{
			GameObject gameObject2 = BrickManManager.Instance.Get(val7);
			if (null != gameObject2)
			{
				HitPart[] componentsInChildren = gameObject2.GetComponentsInChildren<HitPart>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].part == (HitPart.TYPE)bF_PeerHITBRICKMAN_NEW.hitpart)
					{
						GlobalVars.Instance.hitParent = componentsInChildren[i].transform;
						GameObject pfb = componentsInChildren[i].hitImpact;
						if (bF_PeerHITBRICKMAN_NEW.lucky == 1 && null != componentsInChildren[i].luckyImpact)
						{
							pfb = componentsInChildren[i].luckyImpact;
						}
						VfxOptimizer.Instance.CreateFx(pfb, vector2, Quaternion.FromToRotation(Vector3.up, toDirection), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
						break;
					}
				}
				GlobalVars.Instance.hitBirckman = val7;
				GameObject gameObject3 = BrickManManager.Instance.Get(val);
				if (null != gameObject3 && (bF_PeerHITBRICKMAN_NEW.slot == 2 || bF_PeerHITBRICKMAN_NEW.slot == 1))
				{
					GdgtGun componentInChildren = gameObject3.GetComponentInChildren<GdgtGun>();
					if (componentInChildren != null)
					{
						componentInChildren.createBullet(vector2, vector, -1, (int)bF_PeerHITBRICKMAN_NEW.ammoId, 0);
					}
				}
			}
		}
		else
		{
			GameObject gameObject4 = BrickManManager.Instance.Get(val);
			if (null != gameObject4)
			{
				GdgtGun componentInChildren2 = gameObject4.GetComponentInChildren<GdgtGun>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.destroyBullet((int)bF_PeerHITBRICKMAN_NEW.ammoId);
				}
			}
		}
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			if (val7 == MyInfoManager.Instance.Seq)
			{
				GameObject gameObject5 = GameObject.Find("Me");
				if (null != gameObject5)
				{
					LocalController component2 = gameObject5.GetComponent<LocalController>();
					if (null != component2 && NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component2.HitPoint) > 0)
					{
						component2.SetLucky((bF_PeerHITBRICKMAN_NEW.lucky == 1) ? true : false);
						component2.GetHit(val, (int)bF_PeerHITBRICKMAN_NEW.damage, val12, val13, (int)bF_PeerHITBRICKMAN_NEW.hitpart, autoHealPossible: true, checkZombie: false);
						SendPEER_HP(val, val7, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component2.HitPoint), component2.GetMaxHp(), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component2.Armor), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component2.MaxArmor));
					}
				}
				GameObject gameObject6 = GameObject.Find("Main");
				if (null != gameObject6 && (int)bF_PeerHITBRICKMAN_NEW.damage > 0)
				{
					gameObject6.BroadcastMessage("OnDirectionalHit", val);
				}
			}
			else
			{
				GameObject gameObject7 = BrickManManager.Instance.Get(val7);
				if (null != gameObject7)
				{
					TPController component3 = gameObject7.GetComponent<TPController>();
					if (null != component3)
					{
						component3.GetHit((int)bF_PeerHITBRICKMAN_NEW.damage, val7);
					}
				}
			}
		}
	}

	public void SendPEER_HIT_BRICKMAN_W(int to, HitManPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.firePacket.shootdir.x, cls.firePacket.shootdir.y, cls.firePacket.shootdir.z);
		BF_PeerHITBRICKMAN_NEW bF_PeerHITBRICKMAN_NEW = default(BF_PeerHITBRICKMAN_NEW);
		bF_PeerHITBRICKMAN_NEW.slot = cls.firePacket.slot;
		bF_PeerHITBRICKMAN_NEW.ammoId = cls.firePacket.usID;
		bF_PeerHITBRICKMAN_NEW.damage = cls.damage;
		bF_PeerHITBRICKMAN_NEW.hitpart = cls.hitPart;
		bF_PeerHITBRICKMAN_NEW.lucky = (uint)(cls.bLucky ? 1 : 0);
		p2PMsgBody.Write(cls.firePacket.shooter);
		p2PMsgBody.Write(bF_PeerHITBRICKMAN_NEW.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.x);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.y);
		ushort val7 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.z);
		uint val8 = GlobalVars.Instance.NormalToUByte4(cls.hitnml.x, cls.hitnml.y, cls.hitnml.z);
		p2PMsgBody.Write(cls.hitMan);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		p2PMsgBody.Write(val7);
		p2PMsgBody.Write(val8);
		p2PMsgBody.Write(cls.rigidity);
		p2PMsgBody.Write(cls.weaponBy);
		Whisper(to, 82, p2PMsgBody);
	}

	private void HandlePEER_HIT_BRICKMAN_W(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		msg.Read(out int val7);
		msg.Read(out ushort val8);
		msg.Read(out ushort val9);
		msg.Read(out ushort val10);
		msg.Read(out uint val11);
		msg.Read(out float val12);
		msg.Read(out ushort val13);
		BF_PeerHITBRICKMAN_NEW bF_PeerHITBRICKMAN_NEW = default(BF_PeerHITBRICKMAN_NEW);
		bF_PeerHITBRICKMAN_NEW.bitvector1 = val2;
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 vector = GlobalVars.Instance.UByte4ToNormal(val6);
		Vector3 vector2 = new Vector3(GlobalVars.Instance.Float16toFloat32(val8), GlobalVars.Instance.Float16toFloat32(val9), GlobalVars.Instance.Float16toFloat32(val10));
		Vector3 toDirection = GlobalVars.Instance.UByte4ToNormal(val11);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Fire((Weapon.TYPE)bF_PeerHITBRICKMAN_NEW.slot, (int)bF_PeerHITBRICKMAN_NEW.ammoId, origin, vector);
			}
		}
		if (val7 != MyInfoManager.Instance.Seq)
		{
			GameObject gameObject2 = BrickManManager.Instance.Get(val7);
			if (null != gameObject2)
			{
				HitPart[] componentsInChildren = gameObject2.GetComponentsInChildren<HitPart>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].part == (HitPart.TYPE)bF_PeerHITBRICKMAN_NEW.hitpart)
					{
						GlobalVars.Instance.hitParent = componentsInChildren[i].transform;
						GameObject pfb = componentsInChildren[i].hitImpact;
						if (bF_PeerHITBRICKMAN_NEW.lucky == 1 && null != componentsInChildren[i].luckyImpact)
						{
							pfb = componentsInChildren[i].luckyImpact;
						}
						VfxOptimizer.Instance.CreateFx(pfb, vector2, Quaternion.FromToRotation(Vector3.up, toDirection), VfxOptimizer.VFX_TYPE.BULLET_IMPACT);
						break;
					}
				}
				GlobalVars.Instance.hitBirckman = val7;
				if ((null != gameObject && bF_PeerHITBRICKMAN_NEW.slot == 2) || bF_PeerHITBRICKMAN_NEW.slot == 1)
				{
					GdgtGun componentInChildren = gameObject.GetComponentInChildren<GdgtGun>();
					if (componentInChildren != null)
					{
						componentInChildren.createBullet(vector2, vector, -1, (int)bF_PeerHITBRICKMAN_NEW.ammoId, 0);
					}
				}
				TPController component2 = gameObject2.GetComponent<TPController>();
				if (null != component2)
				{
					component2.GetHit((int)bF_PeerHITBRICKMAN_NEW.damage, val7);
				}
			}
		}
		else
		{
			if (null != gameObject)
			{
				GdgtGun componentInChildren2 = gameObject.GetComponentInChildren<GdgtGun>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.destroyBullet((int)bF_PeerHITBRICKMAN_NEW.ammoId);
				}
				GameObject gameObject3 = GameObject.Find("Main");
				if (null != gameObject3 && (int)bF_PeerHITBRICKMAN_NEW.damage > 0)
				{
					gameObject3.BroadcastMessage("OnDirectionalHit", val);
				}
			}
			GameObject gameObject4 = GameObject.Find("Me");
			if (null != gameObject4)
			{
				LocalController component3 = gameObject4.GetComponent<LocalController>();
				if (null != component3 && NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component3.HitPoint) > 0)
				{
					component3.SetLucky((bF_PeerHITBRICKMAN_NEW.lucky == 1) ? true : false);
					component3.GetHit(val, (int)bF_PeerHITBRICKMAN_NEW.damage, val12, val13, (int)bF_PeerHITBRICKMAN_NEW.hitpart, autoHealPossible: true, checkZombie: false);
					SendPEER_HP(val, val7, NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.HIT_POINT, component3.HitPoint), component3.GetMaxHp(), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component3.Armor), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.ARMOR, component3.MaxArmor));
				}
			}
		}
	}

	public void SendPEER_MON_GEN(int tblID, int type, int seq, float x, float y, float z, float vx, float vy, float vz)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)tblID;
		byte val2 = (byte)type;
		ushort val3 = (ushort)seq;
		ushort val4 = GlobalVars.Instance.Float32toFloat16(x);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(y);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(z);
		uint val7 = GlobalVars.Instance.NormalToUByte4(vx, vy, vz);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		p2PMsgBody.Write(val7);
		ReliableSend(uint.MaxValue, 37, p2PMsgBody);
	}

	private void HandlePEER_MON_GEN(P2PMsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out byte val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out ushort val6);
		msg.Read(out uint val7);
		Vector3 vector = new Vector3(GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5), GlobalVars.Instance.Float16toFloat32(val6));
		Vector3 vector2 = GlobalVars.Instance.UByte4ToNormal(val7);
		MonManager.Instance.MonGenerateP2P(val, val2, val3, vector.x, vector.y, vector.z, vector2.x, vector2.y, vector2.z);
	}

	public void SendPEER_MON_DIE(int seq, bool arrived)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)seq;
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(arrived);
		ReliableSend(uint.MaxValue, 38, p2PMsgBody);
	}

	private void HandlePEER_MON_DIE(P2PMsgBody msg)
	{
		msg.Read(out ushort val);
		msg.Read(out bool val2);
		GameObject gameObject = MonManager.Instance.Get(val);
		if (gameObject != null)
		{
			if (!val2)
			{
				MonProperty component = gameObject.GetComponent<MonProperty>();
				component.Desc.Xp = 0;
			}
			else
			{
				MonManager.Instance.Remove(val);
			}
		}
	}

	public void SendPEER_NUMWAVE(int wave)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		byte val = (byte)wave;
		p2PMsgBody.Write(val);
		ReliableSend(uint.MaxValue, 56, p2PMsgBody);
	}

	private void HandlePEER_NUMWAVE(P2PMsgBody msg)
	{
		msg.Read(out byte val);
		DefenseManager.Instance.CurWave = val;
	}

	public void SendPEER_MON_MOVE(int seq, float x, float y, float z, float viewx, float viewy, float viewz)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)seq;
		ushort val2 = GlobalVars.Instance.Float32toFloat16(x);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(y);
		ushort val4 = GlobalVars.Instance.Float32toFloat16(z);
		uint val5 = GlobalVars.Instance.NormalToUByte4(viewx, viewy, viewz);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		p2PMsgBody.Write(val5);
		Say(39, p2PMsgBody);
	}

	private void HandlePEER_MON_MOVE(P2PMsgBody msg)
	{
		msg.Read(out ushort val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out uint val5);
		GameObject gameObject = MonManager.Instance.Get(val);
		if (gameObject != null)
		{
			MonDesc desc = MonManager.Instance.GetDesc(val);
			MonAI aIClass = MonManager.Instance.GetAIClass(gameObject, desc.tblID);
			Vector3 vector = new Vector3(GlobalVars.Instance.Float16toFloat32(val2), GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4));
			Vector3 vector2 = GlobalVars.Instance.UByte4ToNormal(val5);
			if (aIClass != null)
			{
				aIClass.MoveP2P(vector.x, vector.y, vector.z, vector2.x, vector2.y, vector2.z);
			}
			else
			{
				Debug.LogError("//HandlePEER_MON_MOVE// error seq: " + val);
			}
		}
	}

	public void SendPEER_MON_ADDPOINT(int seq, int point)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(point);
		ReliableSend((uint)seq, 57, p2PMsgBody);
	}

	private void HandlePEER_MON_ADDPOINT(P2PMsgBody msg)
	{
		msg.Read(out int val);
		Defense component = GameObject.Find("Main").GetComponent<Defense>();
		if (component != null)
		{
			component.AddDefensePoint(bRed: true, val);
		}
	}

	public void SendPEER_DF_HEALER(int seq)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = (ushort)seq;
		p2PMsgBody.Write(val);
		Say(58, p2PMsgBody);
	}

	private void HandlePEER_DF_HEALER(P2PMsgBody mb)
	{
		mb.Read(out ushort val);
		MonDesc desc = MonManager.Instance.GetDesc(val);
		if (desc != null)
		{
			aiHealer aiHealer = (aiHealer)MonManager.Instance.GetAIClass(desc.Seq, desc.tblID);
			if (aiHealer != null)
			{
				aiHealer.ActiveHealerEff();
			}
		}
	}

	public void SendPEER_DF_SELFHEAL(int seq, int Hp)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(Hp);
		Say(59, p2PMsgBody);
	}

	[RPC]
	private void HandlePEER_DF_SELFHEAL(P2PMsgBody mb)
	{
		mb.Read(out ushort val);
		mb.Read(out ushort val2);
		MonDesc desc = MonManager.Instance.GetDesc(val);
		if (desc != null)
		{
			desc.Xp = val2;
			aiSelfHeal aiSelfHeal = (aiSelfHeal)MonManager.Instance.GetAIClass(desc.Seq, desc.tblID);
			if (aiSelfHeal != null)
			{
				aiSelfHeal.ActiveHealEff();
			}
		}
	}

	public void SendPEER_MON_HIT(int shooter, int monseq, int damage, float rigidity, Vector3 ammopos, Vector3 ammodir, int curammo)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(shooter);
		p2PMsgBody.Write(monseq);
		p2PMsgBody.Write(damage);
		p2PMsgBody.Write(rigidity);
		p2PMsgBody.Write(ammopos.x);
		p2PMsgBody.Write(ammopos.y);
		p2PMsgBody.Write(ammopos.z);
		p2PMsgBody.Write(ammodir.x);
		p2PMsgBody.Write(ammodir.y);
		p2PMsgBody.Write(ammodir.z);
		p2PMsgBody.Write(curammo);
		Say(43, p2PMsgBody);
	}

	private void HandlePEER_MON_HIT(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out float val4);
		msg.Read(out float val5);
		msg.Read(out float val6);
		msg.Read(out float val7);
		msg.Read(out float val8);
		msg.Read(out float val9);
		msg.Read(out float val10);
		msg.Read(out int val11);
		MonDesc desc = MonManager.Instance.GetDesc(val2);
		if (desc != null)
		{
			desc.LogAttacker(val, val3);
			desc.atkedSeq = val;
			desc.Xp -= val3;
			desc.rigidity = val4;
			desc.IsHit = true;
			if (desc.typeID == 2)
			{
				aiHide aiHide = (aiHide)MonManager.Instance.GetAIClass(desc.Seq, desc.tblID);
				if (aiHide != null)
				{
					if (aiHide.IsHide)
					{
						return;
					}
					if (aiHide.CanApply)
					{
						aiHide.setHide(bSet: true);
					}
				}
			}
			GameObject gameObject = MonManager.Instance.Get(val2);
			if (gameObject != null)
			{
				GlobalVars.Instance.hitParent = gameObject.transform;
			}
			else
			{
				Debug.LogError("(mon hit) not found. mon object: " + val2);
			}
			GlobalVars.Instance.hitBirckman = val2;
			GameObject gameObject2 = BrickManManager.Instance.Get(val);
			if (null != gameObject2)
			{
				Weapon.TYPE tYPE = Weapon.TYPE.COUNT;
				TPController component = gameObject2.GetComponent<TPController>();
				if (null != component)
				{
					tYPE = component.GetCurrentWeaponType();
				}
				if (tYPE == Weapon.TYPE.MAIN || tYPE == Weapon.TYPE.AUX)
				{
					GdgtGun componentInChildren = gameObject2.GetComponentInChildren<GdgtGun>();
					if (componentInChildren != null)
					{
						Vector3 pnt = new Vector3(val5, val6, val7);
						Vector3 nml = new Vector3(val8, val9, val10);
						componentInChildren.createBullet(pnt, nml, val2, val11, 2);
					}
				}
			}
		}
	}

	public void SendPEER_MON_HIT_NEW(HitMonPacket cls)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(cls.firePacket.shootpos.z);
		uint val4 = GlobalVars.Instance.NormalToUByte4(cls.firePacket.shootdir.x, cls.firePacket.shootdir.y, cls.firePacket.shootdir.z);
		SlotAmmoDamage slotAmmoDamage = default(SlotAmmoDamage);
		slotAmmoDamage.slot = cls.firePacket.slot;
		slotAmmoDamage.ammoId = cls.firePacket.usID;
		slotAmmoDamage.damage = cls.damage;
		p2PMsgBody.Write(cls.firePacket.shooter);
		p2PMsgBody.Write(slotAmmoDamage.bitvector1);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		p2PMsgBody.Write(val4);
		ushort val5 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.x);
		ushort val6 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.y);
		ushort val7 = GlobalVars.Instance.Float32toFloat16(cls.hitpoint.z);
		p2PMsgBody.Write(cls.hitMon);
		p2PMsgBody.Write(cls.rigidity);
		p2PMsgBody.Write(val5);
		p2PMsgBody.Write(val6);
		p2PMsgBody.Write(val7);
		Say(75, p2PMsgBody);
	}

	private void HandlePEER_MON_HIT_NEW(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		msg.Read(out ushort val5);
		msg.Read(out uint val6);
		SlotAmmoDamage slotAmmoDamage = default(SlotAmmoDamage);
		slotAmmoDamage.bitvector1 = val2;
		Vector3 origin = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
		Vector3 vector = GlobalVars.Instance.UByte4ToNormal(val6);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component && slotAmmoDamage.slot >= 0)
			{
				component.Fire((Weapon.TYPE)slotAmmoDamage.slot, (int)slotAmmoDamage.ammoId, origin, vector);
			}
		}
		msg.Read(out ushort val7);
		msg.Read(out float val8);
		msg.Read(out ushort val9);
		msg.Read(out ushort val10);
		msg.Read(out ushort val11);
		int num = val7;
		MonDesc desc = MonManager.Instance.GetDesc(num);
		if (desc != null)
		{
			desc.LogAttacker(val, (int)slotAmmoDamage.damage);
			desc.atkedSeq = val;
			desc.Xp -= (int)slotAmmoDamage.damage;
			desc.rigidity = val8;
			desc.IsHit = true;
			if (desc.typeID == 2)
			{
				aiHide aiHide = (aiHide)MonManager.Instance.GetAIClass(desc.Seq, desc.tblID);
				if (aiHide != null)
				{
					if (aiHide.IsHide)
					{
						return;
					}
					if (aiHide.CanApply)
					{
						aiHide.setHide(bSet: true);
					}
				}
			}
			GameObject gameObject2 = MonManager.Instance.Get(num);
			if (gameObject2 != null)
			{
				GlobalVars.Instance.hitParent = gameObject2.transform;
				GlobalVars.Instance.hitBirckman = num;
			}
			GameObject gameObject3 = BrickManManager.Instance.Get(val);
			if (null != gameObject3 && (slotAmmoDamage.slot == 2 || slotAmmoDamage.slot == 1))
			{
				GdgtGun componentInChildren = gameObject3.GetComponentInChildren<GdgtGun>();
				if (componentInChildren != null)
				{
					Vector3 pnt = new Vector3(GlobalVars.Instance.Float16toFloat32(val9), GlobalVars.Instance.Float16toFloat32(val10), GlobalVars.Instance.Float16toFloat32(val11));
					componentInChildren.createBullet(pnt, vector, num, (int)slotAmmoDamage.ammoId, 2);
				}
			}
		}
	}

	public void SendPEER_INSTALLING_BOMB(bool installing)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(installing);
		ReliableSend(uint.MaxValue, 50, p2PMsgBody);
	}

	private void HandlePEER_INSTALLING_BOMB(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Install(val2);
			}
		}
	}

	public void SendPEER_UNINSTALLING_BOMB(bool uninstalling)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(uninstalling);
		ReliableSend(uint.MaxValue, 51, p2PMsgBody);
	}

	private void HandlePEER_UNINSTALLING_BOMB(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.Uninstall(val2);
			}
		}
	}

	public void SendPEER_CREATE_ACTIVE_ITEM(int seq, Vector3 pos)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.z);
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		ReliableSend(uint.MaxValue, 83, p2PMsgBody);
	}

	private void HandlePEER_CREATE_ACTIVE_ITEM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val2), GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4));
			ActiveItemManager.Instance.CreateActiveItem(val, pos);
		}
	}

	public void SendPEER_EAT_ACTIVE_ITEM_REQ(int seq, int userSeq)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(userSeq);
		ReliableSend((uint)RoomManager.Instance.Master, 84, p2PMsgBody);
	}

	private void HandlePEER_EAT_ACTIVE_ITEM_REQ(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			ActiveItemManager.Instance.EatItem(val, val2);
		}
	}

	public void SendPEER_EAT_ACTIVE_ITEM_ACK(int seq, int userSeq, int useItemType)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(userSeq);
		p2PMsgBody.Write(useItemType);
		ReliableSend(uint.MaxValue, 85, p2PMsgBody);
	}

	private void HandlePEER_EAT_ACTIVE_ITEM_ACK(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			ActiveItemManager.Instance.DeleteItem(val, val2, val3);
		}
	}

	public void SendPEER_GUN_ANIM(int seq, sbyte slot, sbyte anim)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(seq);
		p2PMsgBody.Write(slot);
		p2PMsgBody.Write(anim);
		Say(86, p2PMsgBody);
	}

	private void HandlePEER_GUN_ANIM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		msg.Read(out sbyte val3);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.GunAnim((Weapon.TYPE)val2, val3);
			}
		}
	}

	public void SendPEER_STATE_FEVER(bool isOn)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(isOn);
		ReliableSend(uint.MaxValue, 87, p2PMsgBody);
	}

	private void HandlePEER_STATE_FEVER(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		GameObject gameObject = BrickManManager.Instance.Get(val);
		if (null != gameObject)
		{
			TPController component = gameObject.GetComponent<TPController>();
			if (null != component)
			{
				component.setFever(val2);
			}
		}
	}

	public void SendPEER_USE_ACTIVE_ITEM(int userSeq, int itemType)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(userSeq);
		p2PMsgBody.Write(itemType);
		ReliableSend(uint.MaxValue, 88, p2PMsgBody);
	}

	private void HandlePEER_USE_ACTIVE_ITEM(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			ActiveItemManager.Instance.UseItem(val, val2);
		}
	}

	public void SendPEER_BLACKHOLE_EFF(Vector3 pos)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		ushort val = GlobalVars.Instance.Float32toFloat16(pos.x);
		ushort val2 = GlobalVars.Instance.Float32toFloat16(pos.y);
		ushort val3 = GlobalVars.Instance.Float32toFloat16(pos.z);
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		ReliableSend(uint.MaxValue, 89, p2PMsgBody);
	}

	private void HandlePEER_BLACKHOLE_EFF(P2PMsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out ushort val2);
		msg.Read(out ushort val3);
		msg.Read(out ushort val4);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			UnityEngine.Object.Instantiate(position: new Vector3(GlobalVars.Instance.Float16toFloat32(val2), GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4)), original: GlobalVars.Instance.fxPortalPPong, rotation: Quaternion.Euler(0f, 0f, 0f));
		}
	}

	public void SendPEER_BUNGEE_BREAK_INTO_REQ(int userSeq)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		p2PMsgBody.Write(userSeq);
		ReliableSend(uint.MaxValue, 90, p2PMsgBody);
	}

	private void HandlePEER_BUNGEE_BREAK_INTO_REQ(P2PMsgBody msg)
	{
		msg.Read(out int val);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				SendPEER_BUNGEE_BREAK_INTO_ACTIVEITEM_LIST(val);
			}
			SendPEER_BUNGEE_BREAK_INTO_ACK(val);
		}
	}

	public void SendPEER_BUNGEE_BREAK_INTO_ACK(int userSeq)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		bool val = GlobalVars.Instance.StateFever == 1;
		bool val2 = false;
		bool val3 = false;
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if ((bool)component)
			{
				val2 = (component.GetCurrentWeaponBy() == Weapon.BY.BRICK_BOOMER);
				val3 = (component.GetCurrentWeaponBy() == Weapon.BY.KG440);
			}
		}
		p2PMsgBody.Write(MyInfoManager.Instance.Seq);
		p2PMsgBody.Write(val);
		p2PMsgBody.Write(val2);
		p2PMsgBody.Write(val3);
		ReliableSend((uint)userSeq, 91, p2PMsgBody);
	}

	private void HandlePEER_BUNGEE_BREAK_INTO_ACK(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		msg.Read(out bool val3);
		msg.Read(out bool val4);
		if (!MyInfoManager.Instance.IsNotPlaying())
		{
			GameObject gameObject = BrickManManager.Instance.Get(val);
			if (gameObject != null)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (component != null)
				{
					if (val3)
					{
						component.EquipBrickBoom();
					}
					if (val4)
					{
						component.EquipSmokeBomb();
					}
					TPController component2 = gameObject.GetComponent<TPController>();
					if (component2 != null)
					{
						component2.setFever(val2);
					}
				}
			}
		}
	}

	public void SendPEER_BUNGEE_BREAK_INTO_ACTIVEITEM_LIST(int breakIntoUser)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		Dictionary<int, GameObject> activeItemDictionary = ActiveItemManager.Instance.GetActiveItemDictionary();
		p2PMsgBody.Write(activeItemDictionary.Count);
		foreach (KeyValuePair<int, GameObject> item in activeItemDictionary)
		{
			Vector3 position = item.Value.gameObject.transform.position;
			ushort val = GlobalVars.Instance.Float32toFloat16(position.x);
			ushort val2 = GlobalVars.Instance.Float32toFloat16(position.y);
			ushort val3 = GlobalVars.Instance.Float32toFloat16(position.z);
			p2PMsgBody.Write(item.Key);
			p2PMsgBody.Write(val);
			p2PMsgBody.Write(val2);
			p2PMsgBody.Write(val3);
		}
		ReliableSend((uint)breakIntoUser, 92, p2PMsgBody);
	}

	private void HandlePEER_BUNGEE_BREAK_INTO_ACTIVEITEM_LIST(P2PMsgBody msg)
	{
		bool flag = MyInfoManager.Instance.IsNotPlaying();
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out ushort val3);
			msg.Read(out ushort val4);
			msg.Read(out ushort val5);
			if (!flag)
			{
				Vector3 pos = new Vector3(GlobalVars.Instance.Float16toFloat32(val3), GlobalVars.Instance.Float16toFloat32(val4), GlobalVars.Instance.Float16toFloat32(val5));
				ActiveItemManager.Instance.CreateActiveItem(val2, pos);
			}
		}
	}

	public void SendPEER_TRAIN_ROTATE(int trainID, Vector3 foward)
	{
		P2PMsgBody p2PMsgBody = new P2PMsgBody();
		uint val = GlobalVars.Instance.NormalToUByte4(foward.x, foward.y, foward.z);
		p2PMsgBody.Write(trainID);
		p2PMsgBody.Write(val);
		ReliableSend(uint.MaxValue, 94, p2PMsgBody);
	}

	private void HandlePEER_TRAIN_ROTATE(P2PMsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out uint val2);
		Vector3 fwd = GlobalVars.Instance.UByte4ToNormal(val2);
		TrainManager.Instance.SetRotation(val, fwd);
	}
}
