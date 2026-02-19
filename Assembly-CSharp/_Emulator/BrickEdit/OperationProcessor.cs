using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Emulator
{
    public class OperationProcessor : MonoBehaviour
    {

        private static OperationProcessor _instance;
        public static OperationProcessor Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (_instance != null)
                {
                    return;
                }
                _instance = value;
            }
        }

        private System.Object dataLock;

        private List<IOperation> operations;
        private volatile IOperation current;
        private ulong totalSuccess = 0;

        void Awake()
        {
            dataLock = new System.Object();
            operations = new List<IOperation>();
            UnityEngine.Object.DontDestroyOnLoad(this);
        }

        void FixedUpdate()
        {
            if (current != null || operations.Count == 0)
            {
                return;
            }
            if (!IsAuthorized())
            {
                operations.Clear();
                return;
            }
            totalSuccess = 0;
            current = operations.First();
            operations.Remove(current);
            StepCurrentOperation();
        }

        public void Enqueue(IOperation operation, bool message = true)
        {
            lock (dataLock)
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
                current = null;
                return;
            }
            totalSuccess += successCount;
            if (!current.HasNextStep())
            {
                current.Completed(totalSuccess);
                current = null;
                return;
            }
            StepCurrentOperation();
        }

        private void StepCurrentOperation()
        {
            OperationData data = current.NextStep();
            Debug.Log("Sending operation");
            if (ClientExtension.instance.SendBulkBrickRequest(data.flag, data.sourceTemplate, data.sourceRotation, data.targetTemplate, data.targetRotation, data.coordinates) == 0)
            {
                NextBulkOperation(0);
            }
        }

        public bool IsAuthorized()
        {
            return RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR && UserMapInfoManager.Instance.CheckAuth(false);
        }

    }
}
