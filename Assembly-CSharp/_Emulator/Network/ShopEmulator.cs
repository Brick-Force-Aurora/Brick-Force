using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    class ShopEmulator
    {
        private Dictionary<string, Good> dic;

        public void LoadAndSave()
        {
            Debug.Log("http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/");
            string text = Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(text))
            {
                Debug.LogError("ERROR, Fail to find directory for Resources");
            }
            string text2 = Path.Combine(text, "Template/shop.txt");
            string text3 = Path.Combine(text, "Template/shopCategory.txt");
            CSVLoader cSVLoader = new CSVLoader();
            CSVLoader cSVLoader2 = new CSVLoader();
            cSVLoader.SecuredLoadAndSave(text2, "Config\\shop.txt");
            cSVLoader2.SecuredLoadAndSave(text3, "Config\\shopCategory.txt");
        }

        public void ParseData()
        {
            dic = new Dictionary<string, Good>();
            //FileStream fileStream = File.OpenRead("Config\\shopCategory.txt");
            //StreamReader streamReader = new StreamReader(fileStream, Encoding.ASCII);
            string resourceName = "_Emulator.DATA.shopCategory.txt";

            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            StreamReader streamReader = new StreamReader(stream);
            streamReader.ReadLine();

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] splitLine = line.Split(',');
                splitLine[0] = splitLine[0].ToLower();
                TItem tItem = TItemManager.Instance.Get<TItem>(splitLine[0]);
                if (dic.ContainsKey(splitLine[0]))
                {
                    Debug.LogError("Duplicated good found for " + splitLine[0]);
                }
                else if (tItem == null)
                {
                    Debug.LogError("Fail to find item template : " + splitLine[0]);
                }
                else
                {
                    string code = splitLine[0];
                    int isNew = Int32.Parse(splitLine[1]);
                    int isHot = Int32.Parse(splitLine[2]);
                    int isVisible = Int32.Parse(splitLine[3]);
                    int isSpecialOffer = Int32.Parse(splitLine[5]);
                    int isOfferOnce = Int32.Parse(splitLine[6]);
                    int isGiftable = Int32.Parse(splitLine[7]);
                    int isPromo = Int32.Parse(splitLine[8]);
                    int rebuyInvisible = Int32.Parse(splitLine[9]);
                    sbyte minLevelFp = (sbyte)Int32.Parse(splitLine[10]);
                    sbyte minLevelTk = (sbyte)Int32.Parse(splitLine[11]);
                    tItem._StarRate = Int32.Parse(splitLine[4]);
                    dic.Add(splitLine[0], new Good(code, tItem, isNew == 1, isHot == 1, isVisible == 1, isSpecialOffer == 1,
                        isOfferOnce == 1, isGiftable == 1, isPromo == 1, rebuyInvisible == 1, (sbyte)Int32.Parse(splitLine[10]), (sbyte)Int32.Parse(splitLine[11])));
                }
            }
            resourceName = "_Emulator.DATA.shop.txt";
            stream = assembly.GetManifestResourceStream(resourceName);
            streamReader = new StreamReader(stream);
            streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] splitLine = line.Split(',');
                int option = Int32.Parse(splitLine[1]);
                int pointPrice = Int32.Parse(splitLine[2]);
                int brickPrice = Int32.Parse(splitLine[3]);
                int cashPrice = Int32.Parse(splitLine[4]);
                int cashBack = Int32.Parse(splitLine[5]);
                splitLine[0] = splitLine[0].ToLower();
                if (!dic.ContainsKey(splitLine[0]))
                {
                    Debug.Log("Fail to find good : %d" + splitLine[0]);
                }
                else
                {
                    dic[splitLine[0]].AddPrice(option, pointPrice, brickPrice, cashPrice, cashBack, 0, 1, 0, 0, 0, 0, 314816281);
                }
            }
            ShopManager.Instance.dic = dic;
        }
    }
}
