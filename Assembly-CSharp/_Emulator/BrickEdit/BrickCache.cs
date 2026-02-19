using System.Collections.Generic;
using UnityEngine;

namespace _Emulator
{
    public class BrickCache
    {
        private static BrickCache instance_;
        public static BrickCache Instance { get { 
                if (instance_ == null)
                    instance_ = new BrickCache();
                return instance_;
            } }

        private readonly Dictionary<string, byte> aliasToId = new Dictionary<string, byte>();

        private BrickCache() {
            StringMgr stringMgr = StringMgr.Instance;
            foreach (Brick brick in BrickManager.Instance.bricks)
            {
                string brickName = stringMgr.Get(brick.brickAlias, LangOptManager.LANG_OPT.ENGLISH);
                if (brickName.Length == 0)
                {
                    brickName = brick.brickName;
                }
                brickName = brickName.ToLower().Replace(' ', '_');
                if (aliasToId.ContainsKey(brickName))
                {
                    if (!aliasToId.ContainsKey(brickName + "_1"))
                    {
                        aliasToId.Add(brickName + "_1", aliasToId[brickName]);
                    }
                    brickName = ResolveConflict(brickName);
                }
                aliasToId.Add(brickName, brick.index);
            }
        }

        private string ResolveConflict(string baseName)
        {
            string name = baseName;
            int tries = 2;
            while (aliasToId.ContainsKey(name))
            {
                name = baseName + '_' + (tries++);
            }
            return name;
        }

        internal void Init() { }

        public bool GetBrickByName(string name, out byte brickIndex, bool message = true)
        {
            name = name.ToLower().Trim().Replace(' ', '_');
            if (!aliasToId.ContainsKey(name))
            {
                if (message)
                {
                    Actor.Instance.SendChat($"Unknown brick '{name}'");
                }
                brickIndex = 0;
                return false;
            }
            brickIndex = aliasToId[name];
            return true;
        }

        public bool GetCurrentBrick(out byte brickIndex, bool message = true)
        {
            Brick brick = PaletteManager.Instance.GetCurrentBrick();
            if (brick == null)
            {
                if (message)
                {
                    Actor.Instance.SendChat("No brick selected in palette or provided in command");
                }
                brickIndex = 0;
                return false;
            }
            brickIndex = brick.index;
            return true;
        }

    }
}
