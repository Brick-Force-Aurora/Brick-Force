using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    public class Actor
    {
        private static readonly Actor _actor = new Actor();
        public static Actor Instance { get { return _actor; } }
        

        public void SendChat(string message)
        {
            GameObject main = GameObject.Find("Main");
            if (main == null) { return; }
            main.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, "", message));
        }

        public void ShowMessage(string message)
        {
            MessageBoxMgr.Instance.AddMessage(message);
        }

        public void ShowDelayedMessage(string message)
        {
            ShowDelayedMessage(0.05f, message);
        }

        public void ShowDelayedMessage(float delaySeconds, string message)
        {
            BuildOption.Instance.StartCoroutine(ShowMessageLater(delaySeconds, message));
        }
        private IEnumerator ShowMessageLater(float delaySeconds, string message)
        {
            yield return new WaitForSeconds(delaySeconds);
            ShowMessage(message);
        }

    }
}
