using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace _Emulator
{

    public static class EditHelper
    {
        public const byte MAX_COORD = 100;

        public static readonly Vector3 HALF_UP = new Vector3(0f, 0.5f, 0f);
        public static readonly Vector3 MAP_CENTER = new Vector3(UserMap.xMax / 2f, UserMap.yMax + 5f, UserMap.zMax / 2f);

        public static BrickEditTool BrickEditTool
        {
            get
            {
                return BrickEditTool.Tool;
            }
        }

        public static Vector3 PlayerPosition
        {
            get
            {
                GameObject gameObject = GameObject.Find("Me");
                if (gameObject == null)
                {
                    return Vector3.zero;
                }
                return gameObject.transform.position + HALF_UP;
            }
        }

        public static bool CheckSelection()
        {
            BrickEditTool tool = BrickEditTool;
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

        public static bool AsBrick(this string[] tokens, int index, out byte template, bool allowPalette = true, bool optional = false)
        {
            if (tokens.Length <= index)
            {
                if (allowPalette)
                {
                    return BrickCache.Instance.GetCurrentBrick(out template);
                }
                if (!optional)
                {
                    Actor.Instance.SendChat("No brick provided in command");
                }
                template = 0;
                return optional;
            }
            if (byte.TryParse(tokens[index], out byte paletteIndex))
            {
                if (paletteIndex == 0 || paletteIndex > 10)
                {
                    Actor.Instance.SendChat($"Palette index {paletteIndex} doesn't exist");
                    template = 0;
                    return false;
                }
                return BrickCache.Instance.GetBrickFromPalette((byte) (paletteIndex - 1), out template);
            }
            return BrickCache.Instance.GetBrickByName(tokens[index], out template);
        }

        public static bool AsCoord(this string[] tokens, int index, out byte coordinate) 
        {
            if (tokens.Length <= index)
            {
                coordinate = 0;
                return false;
            }
            if (int.TryParse(tokens[index], out int parsed))
            {
                coordinate = (byte)Math.Min(Math.Max(parsed, 0), EditHelper.MAX_COORD);
                return true;
            }
            coordinate = 0;
            return false;
        }

        public static bool AsRadius(this string[] tokens, int index, out byte radius)
        {
            if (AsCoord(tokens, index, out radius)) {
                if (radius == 0)
                {
                    Actor.Instance.SendChat("Radius can't be 0");
                    return false;
                }
                return true;
            }
            Actor.Instance.SendChat("No valid radius was provided");
            return false;
        }

        public static void AsCoords(this string[] tokens, int startIndex, ref byte[] coordinates)
        {
            if (coordinates.Length != 3)
            {
                throw new ArgumentException("Coordinates array has to have a Length of 3");
            }
            int length = Math.Min(tokens.Length, startIndex + 4);
            for (int i = startIndex; i < length; i++)
            {
                if (int.TryParse(tokens[i], out int parsed))
                {
                    coordinates[i - startIndex] = (byte)Math.Min(Math.Max(parsed, 0), EditHelper.MAX_COORD);
                }
            }
        }

        public static bool IsAllowedTarget(this byte template)
        {
            Brick brick = BrickManager.Instance.GetBrick(template);
            if (brick == null || brick.maxInstancePerMap > 0)
            {
                Actor.Instance.SendChat("The provided target brick is not allowed to be used with BrickEdit.");
                return false;
            }
            return true;
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
                    UnityEngine.Object.DestroyImmediate(brickManager.dicBrickCreators[seq]);
                    brickManager.dicBrickCreators.Remove(seq);
                }
            }
        }

        public static void ToCoords(this Vector3 position, out byte x, out byte y, out byte z)
        {
            x = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.x), 0), EditHelper.MAX_COORD);
            y = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.y), 0), EditHelper.MAX_COORD);
            z = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.z), 0), EditHelper.MAX_COORD);
        }

        public static void ToCoords(this Vector3 position, ref byte[] coordinates)
        {
            if (coordinates.Length != 3)
            {
                throw new ArgumentException("Coordinates array has to have a Length of 3");
            }
            coordinates[0] = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.x), 0), EditHelper.MAX_COORD);
            coordinates[1] = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.y), 0), EditHelper.MAX_COORD);
            coordinates[2] = (byte)Math.Min(Math.Max(Mathf.FloorToInt(position.z), 0), EditHelper.MAX_COORD);
        }

        public static void RadiusToCoords(this Vector3 position, byte radius, out byte x1, out byte y1, out byte z1, out byte x2, out byte y2, out byte z2)
        {
            int minX = Mathf.FloorToInt(position.x) - radius;
            int minY = Mathf.FloorToInt(position.y) - radius;
            int minZ = Mathf.FloorToInt(position.z) - radius;
            int maxX = Mathf.FloorToInt(position.x) + radius;
            int maxY = Mathf.FloorToInt(position.y) + radius;
            int maxZ = Mathf.FloorToInt(position.z) + radius;
            x1 = (byte)Math.Min(Math.Max(minX, 0), EditHelper.MAX_COORD);
            y1 = (byte)Math.Min(Math.Max(minY, 0), EditHelper.MAX_COORD);
            z1 = (byte)Math.Min(Math.Max(minZ, 0), EditHelper.MAX_COORD);
            x2 = (byte)Math.Min(Math.Max(maxX, 0), EditHelper.MAX_COORD);
            y2 = (byte)Math.Min(Math.Max(maxY, 0), EditHelper.MAX_COORD);
            z2 = (byte)Math.Min(Math.Max(maxZ, 0), EditHelper.MAX_COORD);
        }
        public static void CacheBrick(this UserMap userMap, int seq, byte template, byte x, byte y, byte z, ushort meshCode, byte rot)
        {
            userMap.CalcCRC(seq, template);
            userMap.AddBrickInst(seq, template, x, y, z, meshCode, rot);
        }

        public static void UpdateScript(this UserMap userMap, int seq, string alias, bool enableOnAwake, bool visibleOnAwake, string commands)
        {
            BrickInst brickInst = userMap.Get(seq);
            if (brickInst != null)
            {
                Brick brick = BrickManager.Instance.GetBrick(brickInst.Template);
                if (brick != null && brick.function == Brick.FUNCTION.SCRIPT)
                {
                    brickInst.UpdateScript(alias, enableOnAwake, visibleOnAwake, commands);
                }
            }
        }

    }
}
