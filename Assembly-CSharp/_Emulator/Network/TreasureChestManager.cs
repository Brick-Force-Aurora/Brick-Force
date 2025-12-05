using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace _Emulator
{
    public class TreasureChestManager
    {
        private static TreasureChestManager _instance;

        private Dictionary<int, List<int>> rareTiles = new Dictionary<int, List<int>>();
        private List<TcStatus> chests = new List<TcStatus>();
        private JsonData chestDefs;

        public static TreasureChestManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TreasureChestManager();
                return _instance;
            }
        }

        private TreasureChestManager()
        {
            LoadChestDefinitions();
            InitializeChests();
        }

        public List<int> GetRareTiles(int seq) => rareTiles[seq];
        public TcStatus[] ToArray() => chests.ToArray();
        public TcStatus Get(int chestSeq) => chests.Find(c => c.Seq == chestSeq);
        private Dictionary<int, byte[]> chestBitmasks = new Dictionary<int, byte[]>();

        // ------------------------------------------
        // LOAD JSON DEFINITIONS
        // ------------------------------------------
        private void LoadChestDefinitions()
        {
            string resourceName = "_Emulator.DATA.TreasureChests.json";
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);

            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                chestDefs = JsonMapper.ToObject(json);
            }
        }

        // ------------------------------------------
        // CREATE CHESTS BASED ON JSON
        // ------------------------------------------
        private void InitializeChests()
        {
            chests.Clear();
            rareTiles.Clear();

            int seq = 1;
            int index = 0;
            System.Random rng = new System.Random();

            foreach (var key in chestDefs.Keys) // each chest type
            {
                JsonData def = chestDefs[key];

                int coin = (int)def["coin"];
                int token = (int)def["token"];
                JsonData items = def["items"];

                int maxOpens = items.Count;
                int rareCount = 0;

                // Count rare items
                for (int i = 0; i < items.Count; i++)
                    if ((bool)items[i]["isKey"]) rareCount++;

                // Create TcStatus
                TcStatus tc = new TcStatus(
                    seq,
                    index,
                    maxOpens,
                    maxOpens,
                    rareCount,
                    rareCount,
                    coin,
                    token,
                    key.ToString()
                );

                int maskBytes = (maxOpens + 7) / 8;

                // All bits = 1 → tile available
                byte[] mask = new byte[maskBytes];
                for (int m = 0; m < mask.Length; m++)
                    mask[m] = 0xFF;

                chestBitmasks[tc.Seq] = mask;

                List<int> rareList = new List<int>();
                // JSON contains exactly 300 items
                for (int i = 0; i < maxOpens; i++)
                {
                    JsonData pick = items[i]; // ← direct, sequential, no randomness

                    tc.AddExpectations(new TcTItem
                    {
                        code = pick["code"].ToString(),
                        opt = (int)pick["opt"],
                        isKey = (bool)pick["isKey"]
                    });
                    if ((bool)pick["isKey"])
                    {
                        rareList.Add(i);
                    }
                }

                chests.Add(tc);
                rareTiles.Add(seq, rareList);

                seq++;
                index++;
            }
        }

        public void OpenTile(int chestSeq, int index)
        {
            byte[] mask = chestBitmasks[chestSeq];

            int byteIndex = index / 8;
            int bitIndex = index % 8; // 0..7

            // MSB-first: tile 0 uses bit 7 (128), tile 7 uses bit 0 (1)
            int bitMask = 1 << (7 - bitIndex);   // same pattern as your array[]

            // If already 0, do nothing
            if ((mask[byteIndex] & bitMask) == 0)
                return;

            // Set bit to 0 → tile opened
            mask[byteIndex] = (byte)(mask[byteIndex] & ~bitMask);
        }


        public bool IsTileOpened(int chestSeq, int index)
        {
            byte[] mask = chestBitmasks[chestSeq];

            int byteIndex = index / 8;
            int bitIndex = index % 8;

            return ((mask[byteIndex] >> bitIndex) & 1) == 0;
        }

        public byte[] GetBitmask(int seq)
        {
            return chestBitmasks[seq];
        }
    }
}
