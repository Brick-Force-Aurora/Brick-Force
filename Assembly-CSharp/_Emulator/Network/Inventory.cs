using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
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

        public Inventory(int _seq, bool load = false)
        {
            equipment = new List<Item>();
            weaponChg = new Item[5];
            shooterTools = new Item[5];
            activeSlots = new Item[19];
            seq = _seq;

            if (load)
                LoadInventoryFromDisk();
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

        public Item CreateItem(TItem template, bool sort = false, int amount = -1, Item.USAGE usage = Item.USAGE.UNEQUIP)
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
            Item item = new Item(itemSeq, template, template.code, usage, amount, 0, 1000);

            return item;
        }

        public Item AddItem(TItem template, bool sort = false, int amount = -1, Item.USAGE usage = Item.USAGE.UNEQUIP)
        {
            var item = CreateItem(template, sort, amount, usage);
            if (item == null)
                return null;

            equipment.Add(item);

            if (sort)
                Sort();

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
            UpdateActiveEquipment();
        }

        public void RemoveItem(long seq)
        {
            Item item = equipment.Find(x => x.Seq == seq);
            equipment.Remove(item);
            UpdateActiveEquipment();
        }

        public void Sort()
        {
            equipment = equipment.OrderBy(x => x.Template.slot).ToList();
        }

        public void GenerateActiveSlots()
        {
            activeSlots = new Item[19];
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

        public void UpdateActiveEquipment()
        {
            GenerateActiveSlots();
            GenerateActiveTools();
            GenerateActiveChange();
        }

        // Do not call on server with default path
        public void Save(string filePath = "Config\\Inventory.json")
        {
            try
            {
                UpdateActiveEquipment();
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                var data = new JsonData();
                foreach (var item in equipment)
                {
                    var itemData = new JsonData();
                    itemData["name"] = item.Template.Name;
                    itemData["code"] = item.Code;
                    itemData["usage"] = item.Usage.ToString();
                    itemData["slot"] = item.Template.slot.ToString();
                    itemData["weapon_change"] = weaponChg.Contains(item);
                    itemData["toolbar"] = shooterTools.Contains(item);
                    itemData["toolslot"] = item.toolSlot;
                    itemData["is_upgraded"] = item.IsUpgradedItem();
                    itemData["amount"] = item.Amount;
                    itemData["remain"] = item.Remain;
                    data.Add(itemData);
                }

                StringBuilder stringBuilder = new StringBuilder();
                JsonWriter writer = new JsonWriter(stringBuilder)
                {
                    PrettyPrint = true,
                    IndentValue = 2
                };

                JsonMapper.ToJson(data, writer);
                File.WriteAllText(filePath, stringBuilder.ToString());

                Debug.Log($"Equipment successfully saved to {filePath}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save equipment to {filePath}: {ex.Message}");
            }
        }

        // Do not call on server with default path
        public void LoadInventoryFromDisk(string filePath = "Config\\Inventory.json")
        {
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                foreach (var category in DummyData.startingGear)
                {
                    string categoryName = category[0];
                    for (int i = 1; i < category.Length; i++)
                    {
                        TItem template = TItemManager.Instance.Get<TItem>(category[i]);
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
                                //check for all build guns but only if in starting gear
                                item.Usage = Item.USAGE.EQUIP;
                            }
                        }
                    }
                }
            }

            else
            {
                try
                {
                    equipment.Clear();
                    var json = File.ReadAllText(filePath);
                    var data = JsonMapper.ToObject(json);
                    foreach (JsonData item in data)
                    {
                        string categoryName = (string)item["slot"];
                        string code = (string)item["code"];
                        int toolslot = (int)item["toolslot"];
                        Item.USAGE usage = (Item.USAGE)Enum.Parse(typeof(Item.USAGE), (string)item["usage"], true);
                        if (!string.IsNullOrEmpty(code))
                        {
                            TItem template = TItemManager.Instance.Get<TItem>(code);
                            if (template == null)
                                continue;

                            Item addedItem = AddItem(template, false, -1, usage);

                            if (addedItem != null)
                            {
                                addedItem.toolSlot = Convert.ToSByte(toolslot);
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Debug.LogError("LoadInventoryFromDisk: " + ex.Message);
                }
            }

            UpdateActiveEquipment();
        }

        public static int SlotToIndex(TItem.SLOT slot)
        {
            switch (slot)
            {
                case TItem.SLOT.UPPER:
                    return 0;
                case TItem.SLOT.LOWER:
                    return 1;
                case TItem.SLOT.MELEE:
                    return 2;
                case TItem.SLOT.MAIN:
                    return 4;
                case TItem.SLOT.AUX:
                    return 3;
                case TItem.SLOT.BOMB:
                    return 5;
                case TItem.SLOT.HEAD:
                    return 6;
                case TItem.SLOT.FACE:
                    return 7;
                case TItem.SLOT.BACK:
                    return 8;
                case TItem.SLOT.LEG:
                    return 9;
                case TItem.SLOT.SASH1:
                    return 10;
                case TItem.SLOT.SASH2:
                    return 11;
                case TItem.SLOT.SASH3:
                    return 12;
                case TItem.SLOT.KIT:
                    return 16;
                case TItem.SLOT.LAUNCHER:
                    return 13;
                case TItem.SLOT.MAGAZINE_L:
                    return 14;
                case TItem.SLOT.MAGAZINE_R:
                    return 15;
                case TItem.SLOT.CHARACTER:
                    return 17;
                case TItem.SLOT.NUM:
                    return 18;
            }

            return -1;
        }
    }
}
