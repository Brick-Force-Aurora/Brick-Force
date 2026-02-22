using System;
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

        private readonly Dictionary<string, int> limits = new Dictionary<string, int>() {
                { "decorative_armor", 200 },
                { "blue_banner", 100 },
                { "red_banner", 100 },
                { "blue_team_spawner", 8 },
                { "red_team_spawner", 8 },
                { "deathmatch_spawner", 16 },
                { "flag_spawn_point", 1 },
                { "blue_flag_capture_point", 1 },
                { "red_flag_capture_point", 1 },
                { "bomb_site", 2 },
                { "entrance_portal", 1 },
                { "monster_path", 50 },
                { "exit_portal", 1 },
                { "turret", 2 },
                { "vulcan_turret", 2 },
                { "gravity_down", 10 },
                { "gravity_down_fragile", 10 },
                { "gravity_up", 10 },
                { "gravity_up_fragile", 10 },
                { "wooden_door", 50 },
                { "red_portal", 2 },
                { "blue_portal", 2 },
                { "green_portal", 2 },
                { "train_car", 2 },
                { "red_brick_2", 1000 }, // Some kind of barrier
        };

        public readonly int[] palette = new int[10];
        private readonly Dictionary<string, byte> aliasToId = new Dictionary<string, byte>();

        private BrickCache() {}

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

        internal void Init() {
            aliasToId.Clear();
            StringMgr stringMgr = StringMgr.Instance;
            int paletteIndex = 0;
            foreach (Brick brick in BrickManager.Instance.bricks)
            {
                string brickName = stringMgr.Get(brick.brickAlias, LangOptManager.LANG_OPT.ENGLISH);
                if (brickName.Length == 0)
                {
                    brickName = brick.brickName;
                }
                brickName = brickName.ToLower().Replace(' ', '_').Replace("(", "").Replace(")", "");
                if (limits.ContainsKey(brickName))
                {
                    brick.maxInstancePerMap = limits[brickName];
                } else
                {
                    brick.maxInstancePerMap = -1;
                    if (paletteIndex < palette.Length)
                    {
                        palette[paletteIndex++] = brick.seq;
                    }
                }
                if (aliasToId.ContainsKey(brickName))
                {
                    Debug.Log("Duplicated brick: " + brickName);
                    if (!aliasToId.ContainsKey(brickName + "_1"))
                    {
                        aliasToId.Add(brickName + "_1", aliasToId[brickName]);
                    }
                    brickName = ResolveConflict(brickName);
                }
                aliasToId.Add(brickName, brick.index);
            }
        }

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
            Brick brick = PaletteManager.Instance.palette[PaletteManager.Instance.currentPalette];
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

        public bool GetBrickFromPalette(byte paletteIndex, out byte brickIndex)
        {
            paletteIndex = (byte) Math.Max(Math.Min(paletteIndex, PaletteManager.Instance.palette.Length), 0);
            Brick brick = PaletteManager.Instance.palette[paletteIndex];
            if (brick == null)
            {
                Actor.Instance.SendChat($"No brick selected in palette index {paletteIndex}");
                brickIndex = 0;
                return false;
            }
            brickIndex = brick.index;
            return true;
        }

    }
}
