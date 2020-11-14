using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    class InventoryGUI : MonoBehaviour
    {
        public static InventoryGUI instance;
        public bool hidden = true;
        private const float heightOffset = 20f;
        private const float buttonWidth = 120f;
        private bool ranGUI = false;
        private Dictionary<string, TItem> sortedItems;
        private Rect iconGUIRect = new Rect(0f, heightOffset, 200f, 0f);
        private Rect inventoryGUIRect = new Rect(205f, heightOffset, 0f, 0f);
        private Rect sortTextFieldRect = new Rect(0f, 1f, 99f, 18f);
        private Rect sortIconsButtonRect = new Rect(100f, 1f, 100f, 18f);
        private Rect updateButtonRect = new Rect(205f, 1f, buttonWidth, 18f);
        private Rect saveButtonRect = new Rect(205f + buttonWidth, 1f, buttonWidth, 18f);
        private Rect loadButtonRect = new Rect(205f + (buttonWidth + 1f) * 2f, 1f, buttonWidth, 18f);
        private Rect sortButtonRect = new Rect(205f + (buttonWidth + 1f) * 3f, 1f, buttonWidth, 18f);
        private Rect showNamesButtonRect = new Rect(205f + (buttonWidth + 1f) * 4f, 1f, buttonWidth, 18f);
        private float scrollViewHeight = 1050f;
        private Vector2 iconScrollPosition = Vector2.zero;
        private Vector2 inventoryScrollPosition = Vector2.zero;
        private bool showNames = false;
        private bool sortInventory = true;
        private bool sortIcons = true;
        private string sortText = "";
        private string lastSortText = "";

        private void FitToScreen()
        {
            iconGUIRect.height = Screen.height - heightOffset;
            scrollViewHeight = Screen.height - 30f - heightOffset;
            inventoryGUIRect.height = Screen.height;
            inventoryGUIRect.width = Screen.width - inventoryGUIRect.x - 5f;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
                hidden = !hidden;
        }


        private void OnGUI()
        {
            if (hidden)
                return;

            if (ClientExtension.instance.clientConnected && TItemManager.Instance != null && ClientExtension.instance.inventory != null)
            {
                if (!ranGUI)
                { 
                    sortedItems = TItemManager.Instance.dic.OrderByDescending(x => x.Value.slot).ToDictionary(x => x.Key, x => x.Value);
                    ranGUI = true;
                }
                FitToScreen();
                iconGUIRect = GUI.Window(102, iconGUIRect, IconGUIWindow, "Items");
                inventoryGUIRect = GUI.Window(103, inventoryGUIRect, InventoryGUIWindow, "Inventory");

                sortText = GUI.TextField(sortTextFieldRect, sortText);

                sortIcons = GUI.Toggle(sortIconsButtonRect, sortIcons, "Sort", "Button");

                if (GUI.Button(updateButtonRect, "Update Inventory"))
                {
                    ClientExtension.instance.inventory.UpdateCSV();
                    ClientExtension.instance.SendInventoryCSV();
                }

                if (GUI.Button(saveButtonRect, "Save Inventory"))
                {
                    ClientExtension.instance.inventory.UpdateCSV();
                    ClientExtension.instance.inventory.Save();
                    ClientExtension.instance.SendInventoryCSV();
                }

                if (GUI.Button(loadButtonRect, "Load Inventory"))
                {
                    ClientExtension.instance.inventory.LoadInventoryFromDisk();
                    if (sortInventory)
                        ClientExtension.instance.inventory.Sort();
                    ClientExtension.instance.SendInventoryCSV();
                }

                sortInventory = GUI.Toggle(sortButtonRect, sortInventory, "Sort Inventory", "Button");

                showNames = GUI.Toggle(showNamesButtonRect, showNames, "Show Names", "Button");
            }
        }
        private void IconGUIWindow(int winID)
        {
            Dictionary<string, TItem> activeDic = sortIcons ? sortedItems : TItemManager.Instance.dic;
            if (sortText != lastSortText)
            {
                lastSortText = sortText;
                int j = 0;
                foreach (KeyValuePair<string, TItem> item in activeDic)
                {
                    if (item.Value.Name.ToLower().Contains(sortText.ToLower()))
                    {
                        iconScrollPosition.y = 92f * j;
                        break;
                    }
                    j++;
                }
            }

            iconScrollPosition = GUI.BeginScrollView(new Rect(5f, 20f, 190f, scrollViewHeight), iconScrollPosition, new Rect(0f, 20f, 161f, 92f * activeDic.Keys.Count - 92f), false, true);
            int i = 0;
            foreach (KeyValuePair<string, TItem> item in activeDic)
            {
                Rect buttonRect = new Rect(0f, 92f * i + 20f, 167f, 91f);
                Rect labelRect = new Rect(buttonRect);
                labelRect.y -= 4;
                labelRect.x += 1;
                if (GUI.Button(buttonRect, item.Value.CurIcon()))
                {
                    GlobalVars.Instance.PlaySoundItemInstall();
                    ClientExtension.instance.inventory.AddItem(item.Value, sortInventory);
                }
                if (showNames)
                    GUI.Label(labelRect, item.Value.Name);
                i++;
            }
            GUI.EndScrollView();
        }

        private void InventoryGUIWindow(int winID)
        {
            Item needsRemove = null;
            int row = 0;
            int col = 0;
            int maxCols = Mathf.FloorToInt(inventoryGUIRect.width / 168f);
            int maxRows = ClientExtension.instance.inventory.equipment.Count / maxCols + 1;
            float totalHeight = 92f * maxRows;
            if (totalHeight < scrollViewHeight)
                totalHeight = scrollViewHeight;

            inventoryScrollPosition = GUI.BeginScrollView(new Rect(5f, 20f, inventoryGUIRect.width - 10f, scrollViewHeight), inventoryScrollPosition, new Rect(0f, 20f, 0f, totalHeight), false, true);
            foreach (Item item in ClientExtension.instance.inventory.equipment)
            {
                if (col >= maxCols)
                {
                    col = 0;
                    row++;
                }

                Rect buttonRect = new Rect(168f * col, 92f * row + 20f, 167f, 91f);
                Rect labelRect = new Rect(buttonRect);
                labelRect.y -= 4;
                labelRect.x += 1;
                Color oldColor = GUI.backgroundColor;
                if (item.Usage == Item.USAGE.EQUIP)
                    GUI.backgroundColor = Color.red;;

                if (GUI.Button(buttonRect, item.Template.CurIcon()))
                {
                    GlobalVars.Instance.PlaySoundItemInstall();
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (item.Usage == Item.USAGE.EQUIP)
                            CSNetManager.Instance.Sock.SendCS_UNEQUIP_REQ(item.Seq);
                        else
                            CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(item.Seq);
                    }

                    else
                        needsRemove = item;
                }
                GUI.backgroundColor = oldColor;
                if (showNames)
                    GUI.Label(labelRect, item.Template.Name);
                col++;
            }
            GUI.EndScrollView();

            if (needsRemove != null)
                ClientExtension.instance.inventory.RemoveItem(needsRemove);

            GUI.Label(new Rect(6f, 0, 100, 100), "Count: " + ClientExtension.instance.inventory.equipment.Count + "/" + Inventory.maxItems);
        }
    }
}
