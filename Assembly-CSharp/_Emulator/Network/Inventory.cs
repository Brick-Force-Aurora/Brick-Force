using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    class Inventory
    {
        public int seq;
        public List<Item> equipment;
        public Item[] weaponChg;
        public Item[] shooterTools;
        public Item[] activeSlots;
        public string[] equipmentString;
        public string[] weaponChgString;
        public const int maxItems = 400;

        public Inventory(int _seq)
        {
            equipment = new List<Item>();
            weaponChg = new Item[5];
            shooterTools = new Item[5];
            activeSlots = new Item[16];
            seq = _seq;

            LoadInventoryFromMemory();

            /*if (csv != null)
                LoadInventoryFromMemory();
            else
                LoadInventoryFromDisk();*/
            //Sort();
        }

        public Inventory(int _seq, List<Item> _equipment)
        {
            equipment = _equipment;
            weaponChg = new Item[5];
            shooterTools = new Item[5];
            activeSlots = new Item[16];
            seq = _seq;

            /*if (csv != null)
                LoadInventoryFromMemory();
            else
                LoadInventoryFromDisk();*/
            //Sort();
        }

        public void Apply()
        {
            MyInfoManager.Instance.inventory.Clear();
            foreach (Item item in equipment)
            {
                MyInfoManager.Instance.SetItem(item.Seq, item.Code, item.Usage, item.Remain, 0, 1000);
            }
            GameObject mainObject = GameObject.Find("Main");
            Lobby lobby = mainObject.GetComponent<Lobby>();
            Briefing4TeamMatch roomLobby = mainObject.GetComponent<Briefing4TeamMatch>();
            if (lobby != null)
            {
                lobby.mirror.mySelf.GetComponent<LookCoordinator>().Reset();
                lobby.mirror.Start();
            }

            if (roomLobby != null)
            {
                roomLobby.mirror.mySelf.GetComponent<LookCoordinator>().Reset();
                roomLobby.mirror.Start();
            }
        }

        public Item AddItem(TItem template, bool sort = false, int amount = -1)
        {
            if (equipment.Count >= maxItems)
                return null;

            if (equipment.Exists(x => x.Template.code == template.code))
                return null;

            int seqSeed = seq + 1;
            byte[] baseSeq = new byte[8];
            byte[] seed = Encoding.UTF8.GetBytes(template.name);
            byte[] codeSeed = Encoding.UTF8.GetBytes(template.code);
            for (int i = 0; i < seed.Length && i < 5; i++)
                baseSeq[i] = (byte)(seed[i] ^ seed[seed.Length - 1 - i]);

            for (int i = 0; i < 3; i++)
                baseSeq[i] ^= codeSeed[i];

            long itemSeq = BitConverter.ToInt64(baseSeq, 0) * seqSeed;
            Item.USAGE usage = Item.USAGE.UNEQUIP;
            Item item = new Item(itemSeq, template, template.code, usage, amount, 0, 1000);
            equipment.Add(item);

            if (sort)
                Sort();

            UpdateCSV();

            return item;
        }

        public void AddWeaponSlot(long seq, sbyte slot)
        {
            Item item = equipment.Find(x => x.Seq == seq && x.IsWeaponSlotAble);
            Item oldItem = equipment.Find(x => x.toolSlot == slot && x.IsWeaponSlotAble);

            if (oldItem != null)
                oldItem.toolSlot = -1;


            if (item != null)
                item.toolSlot = slot;

            GenerateActiveChange();
        }

        public void AddToolSlot(long seq, sbyte slot)
        {
            Item item = equipment.Find(x => x.Seq == seq && x.IsShooterSlotAble);
            Item oldItem = equipment.Find(x => x.toolSlot == slot && x.IsShooterSlotAble);

            if (oldItem != null)
                oldItem.toolSlot = -1;


            if (item != null)
                item.toolSlot = slot;

            GenerateActiveTools();
        }

        public void RemoveItem(Item item)
        {
            equipment.Remove(item);
            GenerateActiveSlots();
            GenerateActiveTools();
            GenerateActiveChange();
        }

        public void RemoveItem(long seq)
        {
            Item item = equipment.Find(x => x.Seq == seq);
            equipment.Remove(item);
            GenerateActiveSlots();
            GenerateActiveTools();
            GenerateActiveChange();
        }

        public void Sort()
        {
            equipment = equipment.OrderBy(x => x.Template.slot).ToList();
        }

        public void GenerateActiveSlots()
        {
            activeSlots = new Item[16];
            List<Item> activeItems = equipment.FindAll(x => x.Usage == Item.USAGE.EQUIP && x.Template.type < TItem.TYPE.SPECIAL);
            equipmentString = new string[activeItems.Count];
            for (int i = 0; i < activeItems.Count; i++)
            {
                equipmentString[i] = activeItems[i].Code;
                int index = SlotToIndex(activeItems[i].Template.slot);
                activeSlots[index] = activeItems[i];
            }
        }

        public void GenerateActiveTools()
        {
            shooterTools = new Item[5];
            List<Item> activeTools = equipment.FindAll(x => x.IsShooterSlotAble && x.toolSlot >= 0);
            for (int i = 0; i < activeTools.Count && i < shooterTools.Length; i++)
            {
                shooterTools[activeTools[i].toolSlot] = activeTools[i];
            }
        }

        public void GenerateActiveChange()
        {
            weaponChg = new Item[5];
            List<Item> activeChange = equipment.FindAll(x => x.IsWeaponSlotAble && x.toolSlot >= 0);
            weaponChgString = new string[activeChange.Count];
            for (int i = 0; i < activeChange.Count && i < weaponChg.Length; i++)
            {
                weaponChgString[i] = activeChange[i].Code;
                weaponChg[activeChange[i].toolSlot] = activeChange[i];
            }
        }

        public void UpdateCSV(string filePath = "Config\\Inventory.csv")
        {
            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in equipment)
                    {
                        // Write the slot and code in the desired format
                        writer.WriteLine($"{item.Template.slot};{item.Code}");
                    }
                }

                Debug.Log($"Equipment successfully saved to {filePath}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save equipment to {filePath}: {ex.Message}");
            }
        }

        /*public void UpdateCSV()
        {
            csv.Save("Config\\InventoryBackup.csv");
            GenerateActiveSlots();
            GenerateActiveTools();
            GenerateActiveChange();

            List<Item> nonEquippedItems = equipment.FindAll(x => x.Usage == Item.USAGE.UNEQUIP && x.toolSlot < 0);
            int row = 0;
            int slot = 0;
            int count = activeSlots.Length;
            for (; row < count; row++, slot++)
            {
                csv._rows[row][1] = activeSlots[slot] == null ? "" : activeSlots[slot].Code;
            }

            slot = 0;
            count += shooterTools.Length;
            for (; row < count; row++, slot++)
            {
                csv._rows[row][1] = shooterTools[slot] == null ? "" : shooterTools[slot].Code;
            }

            slot = 0;
            count += weaponChg.Length;
            for (; row < count; row++, slot++)
            {
                csv._rows[row][1] = weaponChg[slot] == null ? "" : weaponChg[slot].Code;
            }
            int remainCount = csv.Rows - 26;
            csv._rows.RemoveRange(26, remainCount);

            csv._rows.Add(new string[] { "Inventory", nonEquippedItems[0].Code });

            for (int i = 1; i < nonEquippedItems.Count; i++)
            {
                csv._rows.Add(new string[] { "", nonEquippedItems[i].Code });
            }

            Apply();
        }

        public void Save()
        {
            csv.Save("Config\\Inventory.csv");
        }

        public void LoadInventoryFromDisk(string path = "Config\\Inventory.csv")
        {
            equipment.Clear();
            csv = new CSVLoader();
            csv.Load(path);
            if (csv._rows.Count >= maxItems)
                csv._rows.RemoveRange(maxItems, csv._rows.Count - maxItems);

            LoadInventoryFromMemory();
        }*/

        public void LoadInventoryFromMemory()
        {
            string filePath = "Config\\Inventory.csv";

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Create and write the startingGear data
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var category in DummyData.startingGear)
                    {
                        string categoryName = category[0];
                        for (int i = 1; i < category.Length; i++)
                        {
                            writer.WriteLine($"{categoryName};{category[i]}");
                        }
                    }
                }
                Debug.Log($"Inventory file created at {filePath}.");
            }

            // Read data from the CSV file
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2)
                    continue;

                string categoryName = parts[0];
                string code = parts[1];

                if (!string.IsNullOrEmpty(code))
                {
                    TItem template = TItemManager.Instance.Get<TItem>(code);
                    if (template == null)
                        continue;

                    Item item = AddItem(template);

                    if (item != null)
                    {
                        if (categoryName != "NONE")
                        {
                            item.Usage = Item.USAGE.EQUIP;
                        }
                        else if (item.Code == "s92" || item.Code == "s08" || item.Code == "s09" || item.Code == "s07")
                        {
                            //Check the Brick Guns, because they dont have a slot
                            item.Usage = Item.USAGE.EQUIP;
                        }
                    }
                }
            }

            GenerateActiveSlots();
            GenerateActiveTools();
            GenerateActiveChange();
        }

        public static int SlotToIndex(TItem.SLOT slot)
        {
            switch (slot)
            {
                case TItem.SLOT.UPPER:
                    return 5;
                case TItem.SLOT.LOWER:
                    return 6;
                case TItem.SLOT.MELEE:
                    return 2;
                case TItem.SLOT.MAIN:
                    return 0;
                case TItem.SLOT.AUX:
                    return 1;
                case TItem.SLOT.BOMB:
                    return 3;
                case TItem.SLOT.HEAD:
                    return 4;
                case TItem.SLOT.FACE:
                    return 11;
                case TItem.SLOT.BACK:
                    return 10;
                case TItem.SLOT.LEG:
                    return 12;
                case TItem.SLOT.SASH1:
                    return 14;
                case TItem.SLOT.SASH2:
                    return 14;
                case TItem.SLOT.SASH3:
                    return 14;
                case TItem.SLOT.KIT:
                    return 13;
                case TItem.SLOT.LAUNCHER:
                    return 7;
                case TItem.SLOT.MAGAZINE_L:
                    return 8;
                case TItem.SLOT.MAGAZINE_R:
                    return 9;
                case TItem.SLOT.CHARACTER:
                    return 15;
            }

            return -1;
        }
    }
}
