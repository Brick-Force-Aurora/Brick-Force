using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Emulator
{

    public enum OperationFlag : ushort
    {
        None = 0x0,
        Delete = 0x1,
        OnlySource = 0x2,
        SourceWithRotation = 0x4,
        IncludeEmpty = 0x8,
        ExcludeSourceType = 0x10,
    }

    public struct OperationData
    {
        public ushort flag;
        public byte sourceTemplate;
        public byte sourceRotation;
        public byte targetTemplate;
        public byte targetRotation;
        public byte[] coordinates;
    }

    public class OperationProcessor : MonoBehaviour
    {

        private static OperationProcessor _instance;
        public static OperationProcessor Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = (UnityEngine.Object.FindObjectOfType(typeof(OperationProcessor)) as OperationProcessor);
                }
                return _instance;
            }
        }

        private List<IOperation> operations;
        private IOperation current;
        private ulong totalSuccess = 0;

        private void Awake()
        {
            operations = new List<IOperation>();
            UnityEngine.Object.DontDestroyOnLoad(this);
        }

        private void FixedUpdate()
        {
            if (current != null || operations.Count == 0)
            {
                return;
            }
            if (!IsAuthorized())
            {
                lock (this)
                {
                    operations.Clear();
                }
                return;
            }
            lock (this)
            {
                totalSuccess = 0;
                current = operations.First();
                operations.Remove(current);
            }
        }

        public void Enqueue(IOperation operation, bool message = true)
        {
            lock (this)
            {
                operations.Add(operation);
            }
            if (message)
            {
                Actor.Instance.SendChat("Operation enqueued");
            }
        }

        public void NextBulkOperation(uint successCount) 
        {
            if (current == null)
            {
                return;
            }
            if (!IsAuthorized()) 
            {
                lock (this)
                {
                    current = null;
                }
                return;
            }
            totalSuccess += successCount;
            if (!current.HasNextStep())
            {
                try
                {
                    current.Completed(totalSuccess);
                }
                finally
                {
                    lock (this)
                    {
                        current = null;
                    }
                }
                return;
            }
            OperationData data = current.NextStep();
            ClientExtension.instance.SendBulkBrickRequest(data.flag, data.sourceTemplate, data.sourceRotation, data.targetTemplate, data.targetRotation, data.coordinates);
        }

        public bool IsAuthorized()
        {
            return RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MAP_EDITOR || !UserMapInfoManager.Instance.CheckAuth(false);
        }

    }

    public static class OperationExtensions
    {

        private static readonly byte[] EmptyByteArray = new byte[0];

        public static bool IsSet(this ushort value, OperationFlag flag)
        {
            return (value & ((ushort) flag)) == ((ushort) flag);
        }

        public static ushort Set(this ushort value, OperationFlag flag, bool state = true)
        {
            if (value.IsSet(flag) == state)
            {
                return (ushort) (value & ~(ushort) flag);
            }
            return (ushort) (value | (ushort) flag);
        }

        public static OperationData NewData(this IOperation _)
        {
            OperationData data = new OperationData();
            data.flag = 0;
            data.sourceTemplate = 0;
            data.sourceRotation = 0;
            data.targetTemplate = 0;
            data.targetRotation = 0;
            data.coordinates = EmptyByteArray;
            return data;
        }
    }
}
