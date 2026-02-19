using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace _Emulator
{

    public static class EditHelper
    {
        private static EditorTools _editorTools;

        private static EditorTools EditorTools
        {
            get
            {
                if (_editorTools == null)
                {
                    GameObject main = GameObject.Find("Main");
                    if (main != null)
                    {
                        _editorTools = main.GetComponent<EditorTools>();
                    }
                }
                return _editorTools;
            }
        }

        public static ReplaceTool ReplaceTool
        {
            get
            {
                EditorTools editorTools = EditorTools;
                if (editorTools == null)
                {
                    return null;
                }
                return editorTools.GetReplaceTool();
            }
        }

        public static bool CheckSelection()
        {
            ReplaceTool tool = ReplaceTool;
            if (tool == null) {
                return false;
            }
            if (!tool.HasPos1)
            {
                Actor.Instance.SendChat("Please set Position 1 first.");
                return false;
            }
            if (!tool.HasPos2)
            {
                Actor.Instance.SendChat("Please set Position 2 first.");
                return false;
            }
            return true;
        }

    }

    public static class EditHelperExtensions
    {
        public static bool IsInAuthorizedBuildRoom(this CommandContext context)
        {
            if (context.RoomType != Room.ROOM_TYPE.MAP_EDITOR)
            {
                Actor.Instance.SendChat("This command can only be executed in build mode");
                return false;
            }
            return UserMapInfoManager.Instance.CheckAuth(true);
        }

        public static bool AsBrick(this string[] tokens, int index, out byte template, bool allowPalette = true)
        {
            if (tokens.Length <= index)
            {
                if (allowPalette)
                {
                    return BrickCache.Instance.GetCurrentBrick(out template);
                }
                Actor.Instance.SendChat("No brick provided in command");
                template = 0;
                return false;
            }
            if (byte.TryParse(tokens[index], out byte paletteIndex))
            {
                return BrickCache.Instance.GetBrickFromPalette(paletteIndex, out template);
            }
            return BrickCache.Instance.GetBrickByName(tokens[index], out template);
        }

        public static bool IsAllowedTarget(this byte template)
        {
            Brick brick = BrickManager.Instance.GetBrick(template);
            if (brick == null || brick.maxInstancePerMap <= 0)
            {
                Actor.Instance.SendChat("The provided target brick is not allowed to be used with BrickEdit.");
                return false;
            }
            return true;
        }

        public static bool AddBrickBulk(this BrickManager brickManager, int seq, byte x, byte y, byte z, byte index, byte rotation, ref List<int> morphes)
        {
            return brickManager.userMap.AddBrickInst(seq, index, x, y, z, rotation, ref morphes) && brickManager.Create(seq, brickManager.userMap.GetMeshCode(seq), index, new Vector3(x, y, z), rotation, combineMesh: true);
        }

        public static void UpdateBrickChunksBulk(this BrickManager brickManager, ref List<int> morphes)
        {
            List<GameObject> modifiedChunks = new List<GameObject>();
            foreach (int item in morphes)
            {
                brickManager.Morph(item, ref modifiedChunks);
            }
            foreach (GameObject item2 in modifiedChunks)
            {
                BrickChunk component = item2.GetComponent<BrickChunk>();
                if (null != component)
                {
                    component.Merge();
                }
            }
        }

        public static bool DeleteBrickBulk(this BrickManager brickManager, int seq, ref List<int> morphes)
        {
            try
            {
                return brickManager.userMap.DelBrickInst(seq, ref morphes);
            }
            finally
            {
                if (brickManager.dicBrickCreators.ContainsKey(seq))
                {
                    Object.DestroyImmediate(brickManager.dicBrickCreators[seq]);
                    brickManager.dicBrickCreators.Remove(seq);
                }
            }
        }

    }
}
