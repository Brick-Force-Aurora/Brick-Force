using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace _Emulator
{
    public struct BulkChange
    {
        public int sequence;
        public byte index, rotation;
        public ushort code;
        public Vector3 position;
        public int[] morphes;

        public BulkChange(int sequence, byte index, byte rotation, ushort code, byte x, byte y, byte z, int[] morphes)
        {
            this.sequence = sequence;
            this.index = index;
            this.rotation = rotation;
            this.code = code;
            this.position = new Vector3(x, y, z);
            this.morphes = morphes;
        }
    }

    public struct BulkChunk
    {
        public byte x, y, z, index;
        public BulkChange[] changes;
    }

    public class BulkChangeProcessor : MonoBehaviour
    {

        private static BulkChangeProcessor _instance;
        public static BulkChangeProcessor Instance
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

        public int chunksPerFrame = 8;
        public int changesPerFrame = 400;
        public int morphsPerFrame = 800;

        private List<int> removalMorphs = new List<int>();
        private List<int> pendingMorphs = new List<int>();
        private List<BulkChange> pendingChanges = new List<BulkChange>();
        private List<GameObject> pendingChunks = new List<GameObject>();
        private bool cleared = false;
        private System.Object dataLock;

        void Awake()
        {
            dataLock = new System.Object();
            UnityEngine.Object.DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MAP_EDITOR)
            {
                if (cleared)
                {
                    return;
                }
                cleared = true;
                lock (dataLock)
                {
                    removalMorphs.Clear();
                    pendingChunks.Clear();
                    pendingChanges.Clear();
                    pendingMorphs.Clear();
                }
                return;
            } else if (cleared)
            {
                cleared = false;
            }

            // First we morph
            if (pendingMorphs.Count > 0)
            {
                int morphSeq;
                BrickManager brickManager = BrickManager.Instance;
                for (int i = 0; i < morphsPerFrame; i++)
                {
                    if (pendingMorphs.Count == 0)
                    {
                        return;
                    }
                    morphSeq = pendingMorphs.First();
                    pendingMorphs.RemoveAt(0);
                    brickManager.Morph(morphSeq, ref pendingChunks);
                }
                return;
            }
            // Then we update chunks
            if (pendingChunks.Count != 0)
            {
                for (int i = 0; i < chunksPerFrame; i++)
                {
                    if (pendingChunks.Count == 0)
                    {
                        return;
                    }
                    GameObject chunkObj;
                    chunkObj = pendingChunks.First();
                    pendingChunks.Remove(chunkObj);
                    BrickChunk chunkComp = chunkObj.GetComponent<BrickChunk>();
                    if (chunkComp == null)
                    {
                        continue;
                    }
                    chunkComp.Merge();
                }
                return;
            }
            // Then we apply new bricks
            if (pendingChanges.Count > 0)
            {
                BulkChange change;
                BrickManager brickManager = BrickManager.Instance;
                for (int i = 0; i < changesPerFrame; i++)
                {
                    if (pendingChanges.Count == 0)
                    {
                        return;
                    }
                    change = pendingChanges.First();
                    pendingChanges.RemoveAt(0);
                    brickManager.Create(change.sequence, change.code, change.index, change.position, change.rotation, false);
                    pendingMorphs.AddRange(change.morphes);
                }
                return;
            }
            // We have to requeue removal morphs to ensure all colliders and rendered meshes are removed
            if (removalMorphs.Count > 0)
            {
                pendingMorphs.AddRange(removalMorphs);
                removalMorphs.Clear();
                return;
            }
        }

        // The morphs list is expected to only be removals
        // Morphs for brick additions should be in each BulkChange.
        public void Push(ref List<int> morphs, ref List<BulkChange> changes)
        {
            lock (dataLock)
            {
                pendingChanges.AddRange(changes);
                removalMorphs.AddRange(morphs);
                pendingMorphs.AddRange(morphs);
            }
        }

    }
}
