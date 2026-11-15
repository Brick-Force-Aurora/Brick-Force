using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;
using static Brick;
using UnityEngine.SocialPlatforms;
using System.Collections;
using static WindowSystemManager;
using System.Runtime.InteropServices.ComTypes;
using System.Net;
using System.Net.Sockets;

namespace _Emulator
{
    public class P2PExtension
    {
        public static P2PExtension instance = new P2PExtension();
        public bool isSteam = false;
        public bool listenSteam = false;

        public void BootupSteam()
        {
            isSteam = true;
            listenSteam = true;
            P2PManager.Instance.ResetReliables();
            P2PManager.Instance.readQueue = new Queue();
            P2PManager.Instance.recv = new P2PMsg4Recv();
            P2PManager.Instance.rendezvousPointed = true;
            peerRtt.Clear();
            MyInfoManager.Instance.PingTime = 0f;
        }
        public void AddSteam(int seq, CSteamID steamID, byte playerflag)
        {
            if (P2PManager.Instance.dic != null)
            {
                if (P2PManager.Instance.dic.ContainsKey(seq))
                {
                    //Debug.LogError("Duplicate peer seq " + seq);
                }
                else
                {
                    P2PManager.Instance.dic.Add(seq, new Peer(seq, steamID, playerflag));
                }
            }
        }

        public void ReceiveSteam(CSteamID steamID, byte[] msg)
        {
            if (!isSteam)
                return;

            if (msg == null)
            {
                Debug.LogError("ReceiveSteam (P2P): msg was null");
                return;
            }

            if (msg.Length < 8)
            {
                Debug.LogError("ReceiveSteam (P2P): msg length was " + msg.Length);
                return;
            }

            try
            {
                // Only receive from lobby members
                if (SteamLobbyManager.instance.IsCurrentMember(steamID))
                {
                    P2PMsg4Recv recv = new P2PMsg4Recv(msg);
                    recv._hdr.FromArray(msg);
                    byte id = recv.GetId();
                    ushort meta = recv.GetMeta();
                    byte src = recv.GetSrc();
                    byte dst = recv.GetDst();
                    P2PMsgBody msgBody = recv.Flush();

                    lock (P2PManager.Instance)
                    {
                        P2PManager.Instance.readQueue.Enqueue(new P2PMsg2Handle(id, msgBody, steamID, meta, src, dst));
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("ReceiveSteam (P2P): " + ex.Message);
            }
        }

        public bool HandleMessage(P2PMsg2Handle msg)
        {
            bool result = true;
            switch ((P2PExtensionOpcodes)msg._id)
            {
                case P2PExtensionOpcodes.opPeerPrivateHand:
                    HandlePeerPrivateHandSteam(msg._msg, msg._steamIDFrom);
                    break;
                case P2PExtensionOpcodes.opPeerPublicHand:
                    HandlePeerPublicHandSteam(msg._msg, msg._steamIDFrom);
                    break;
                case P2PExtensionOpcodes.opPeerPing:
                    HandlePeerPingSteam(msg._msg, msg._steamIDFrom);
                    break;
                case P2PExtensionOpcodes.opPeerPong:
                    HandlePeerPongSteam(msg._msg, msg._steamIDFrom);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        // Round trip time (seconds) per peer, smoothed.
        private readonly Dictionary<CSteamID, float> peerRtt = new Dictionary<CSteamID, float>();
        private const float rttSmoothing = 0.3f;

        // Called once per second: pings every peer, the pong gives the round trip time
        public void SendPingSteam()
        {
            if (!isSteam || P2PManager.Instance.dic == null)
                return;

            foreach (KeyValuePair<int, Peer> item in P2PManager.Instance.dic)
            {
                Peer peer = item.Value;
                if (!peer.isSteam || peer.P2pStatus == Peer.P2P_STATUS.NONE || peer.steamID == SteamUser.GetSteamID())
                    continue;

                try
                {
                    P2PMsgBody p2PMsgBody = new P2PMsgBody();
                    p2PMsgBody.Write(Time.realtimeSinceStartup);
                    P2PMsg4Send p2PMsg4Send = new P2PMsg4Send((byte)P2PExtensionOpcodes.opPeerPing, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                    SteamNetworkingManager.instance.SendMessageToPeer(peer.steamID, p2PMsg4Send);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error, " + ex.Message.ToString() + " : P2PExtension.SendPingSteam");
                }
            }
        }

        private void HandlePeerPingSteam(P2PMsgBody msg, CSteamID steamID)
        {
            msg.Read(out float sentTime);

            try
            {
                // Echo the timestamp back unchanged
                P2PMsgBody p2PMsgBody = new P2PMsgBody();
                p2PMsgBody.Write(sentTime);
                P2PMsg4Send p2PMsg4Send = new P2PMsg4Send((byte)P2PExtensionOpcodes.opPeerPong, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                SteamNetworkingManager.instance.SendMessageToPeer(steamID, p2PMsg4Send);
            }
            catch (Exception ex)
            {
                Debug.LogError("HandlePeerPingSteam Error, " + ex.Message.ToString());
            }
        }

        private void HandlePeerPongSteam(P2PMsgBody msg, CSteamID steamID)
        {
            msg.Read(out float sentTime);

            float rtt = Time.realtimeSinceStartup - sentTime;
            if (rtt < 0f)
                return;

            peerRtt.TryGetValue(steamID, out float old);
            peerRtt[steamID] = old > 0f ? old + (rtt - old) * rttSmoothing : rtt;

            // Own ping is the average over the peers still in the match; other players get it via PEER_PING
            float sum = 0f;
            int count = 0;
            foreach (KeyValuePair<int, Peer> item in P2PManager.Instance.dic)
            {
                if (item.Value.isSteam && peerRtt.TryGetValue(item.Value.steamID, out float value))
                {
                    sum += value;
                    count++;
                }
            }
            if (count > 0)
                MyInfoManager.Instance.PingTime = sum / count;
        }

        public void SendPeerPrivateHandSteam(CSteamID steamID)
        {
            try
            {
                P2PMsgBody p2PMsgBody = new P2PMsgBody();
                p2PMsgBody.Write(MyInfoManager.Instance.Seq);
                P2PMsg4Send p2PMsg4Send = new P2PMsg4Send((byte)P2PExtensionOpcodes.opPeerPrivateHand, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                SteamNetworkingManager.instance.SendMessageToPeer(steamID, p2PMsg4Send);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error, " + ex.Message.ToString() + " : P2PExtension.SendPeerPrivateHandSteam");
            }
        }

        public void SendPeerPublicHandSteam(CSteamID steamID)
        {
            try
            {
                P2PMsgBody p2PMsgBody = new P2PMsgBody();
                p2PMsgBody.Write(MyInfoManager.Instance.Seq);
                P2PMsg4Send p2PMsg4Send = new P2PMsg4Send((byte)P2PExtensionOpcodes.opPeerPublicHand, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                SteamNetworkingManager.instance.SendMessageToPeer(steamID, p2PMsg4Send);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error, " + ex.Message.ToString() + " : P2PExtension.SendPeerPublicHandSteam");
            }
        }

        private void HandlePeerPrivateHandSteam(P2PMsgBody msg, CSteamID steamID)
        {
            msg.Read(out int val);
            P2PMsgBody p2PMsgBody = new P2PMsgBody();
            p2PMsgBody.Write(MyInfoManager.Instance.Seq);

            try
            {
                P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(20, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                SteamNetworkingManager.instance.SendMessageToPeer(steamID, p2PMsg4Send);
            }
            catch (Exception ex)
            {
                Debug.LogError("HandlePeerPrivateHandSteam Error, " + ex.Message.ToString());
            }
        }

        private void HandlePeerPublicHandSteam(P2PMsgBody msg, CSteamID steamID)
        {
            msg.Read(out int val);
            P2PMsgBody p2PMsgBody = new P2PMsgBody();
            p2PMsgBody.Write(MyInfoManager.Instance.Seq);

            try
            {
                P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(4, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                SteamNetworkingManager.instance.SendMessageToPeer(steamID, p2PMsg4Send);
            }
            catch (Exception ex)
            {
                Debug.LogError("HandlePeerPublicHandSteam Error, " + ex.Message.ToString());
            }
        }
    }
}
