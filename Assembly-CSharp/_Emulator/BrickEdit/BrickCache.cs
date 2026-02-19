using _Emulator.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                aliasToId.Add(brickName, brick.index);
            }
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
                    Actor.Instance.SendChat("No block selected in palette or provided in command");
                }
                brickIndex = 0;
                return false;
            }
            brickIndex = brick.index;
            return true;
        }

    }
}
