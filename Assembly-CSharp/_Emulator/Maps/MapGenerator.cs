using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    class MapGenerator
    {

        public const byte MAP_Y_OFFSET = 30;

        public class Landscape
        {
            public byte[] bricks;
            public float[] ratios;
            public float[] distribution;
            public byte size;
            public byte height;

            public Landscape(byte[] _bricks, float[] _ratios, byte _size, byte _height)
            {
                bricks = _bricks;
                ratios = _ratios;
                size = _size;
                height = _height;
                distribution = new float[ratios.Length + 1];
                distribution[0] = 0f;

                for (int i = 0; i < ratios.Length; i++)
                {
                    distribution[i + 1] = ratios[i] + distribution[i];
                }
            }
        }

        public static MapGenerator instance = new MapGenerator();
        private Dictionary<int, Landscape> landscapeTemplates;

        MapGenerator()
        {
            landscapeTemplates = new Dictionary<int, Landscape>();
            landscapeTemplates.Add(0, new Landscape(new byte[] { 0, 1}, new float[] { 0.7f, 0.3f }, 50, 1));
            landscapeTemplates.Add(1, new Landscape(new byte[] { 0, 1 }, new float[] { 0.7f, 0.3f }, 100, 1));
            landscapeTemplates.Add(2, new Landscape(new byte[] { 0, 1 }, new float[] { 0.7f, 0.3f }, 50, 5));
            landscapeTemplates.Add(3, new Landscape(new byte[] { 8, 9 }, new float[] { 0.7f, 0.3f }, 50, 1));
            landscapeTemplates.Add(4, new Landscape(new byte[] { 8, 9 }, new float[] { 0.7f, 0.3f }, 100, 1));
            landscapeTemplates.Add(5, new Landscape(new byte[] { 8, 9 }, new float[] { 0.7f, 0.3f }, 50, 5));
            landscapeTemplates.Add(6, new Landscape(new byte[] { 10, 25 }, new float[] { 0.7f, 0.3f }, 50, 1));
            landscapeTemplates.Add(7, new Landscape(new byte[] { 10, 25 }, new float[] { 0.7f, 0.3f }, 100, 1));
            landscapeTemplates.Add(8, new Landscape(new byte[] { 10, 25 }, new float[] { 0.7f, 0.3f }, 50, 5));
            landscapeTemplates.Add(9, new Landscape(new byte[] { 9, 137, 138 }, new float[] { 0.5f, 0.25f, 0.25f }, 50, 1));
            landscapeTemplates.Add(10, new Landscape(new byte[] { 9, 137, 138 }, new float[] { 0.5f, 0.25f, 0.25f }, 100, 1));
            landscapeTemplates.Add(11, new Landscape(new byte[] { 9, 137, 138 }, new float[] { 0.5f, 0.25f, 0.25f }, 50, 5));
        }

        public int GetHashIdForTime(DateTime time)
        {
            long bin = time.ToBinary();
            int hashId = CRC32.compute(BitConverter.GetBytes(bin));
            while (ServerEmulator.instance.regMaps.Exists(x => x.Value.Map == hashId))
            {
                bin ^= hashId;
                hashId = CRC32.compute(BitConverter.GetBytes(bin));
            }

            return hashId;
        }

        private byte GetNextTemplateByDistribution(Landscape landscape)
        {
            float rnd = UnityEngine.Random.Range(landscape.distribution[0], landscape.distribution[landscape.ratios.Length]);
            for (int i = 0; i < landscape.bricks.Length; i++)
            {
                if (rnd >= landscape.distribution[i] && rnd < landscape.distribution[i + 1])
                    return landscape.bricks[i];
            }

            return landscape.bricks[0];
        }

        public UserMap GenerateInternal(Landscape landscape, int skyboxIndex)
        {
            byte size = landscape.size;
            byte height = landscape.height;


            byte remaining = (byte)((EditHelper.MAX_COORD - size) / 2);
            byte offsetSize = (byte)(size + remaining);
            byte offsetHeight = (byte)(height + MAP_Y_OFFSET);

            UserMap map = new UserMap();
            map.skybox = skyboxIndex;
            map.min.x = remaining;
            map.min.y = MAP_Y_OFFSET;
            map.min.z = remaining;
            map.max.x = offsetSize;
            map.max.y = offsetHeight;
            map.max.z = offsetSize;
            map.cenX = (map.min.x + map.max.x) * 0.5f;
            map.cenZ = (map.min.z + map.max.z) * 0.5f;

            List<int> morphes = new List<int>();

            int seq = 0;
            for (byte x = remaining; x < offsetSize; x++)
            {
                for (byte z = remaining; z < offsetSize; z++)
                {
                    for (byte y = MAP_Y_OFFSET; y < offsetHeight; y++)
                    {
                        byte template = GetNextTemplateByDistribution(landscape);
                        map.AddBrickInst(seq, template, x, y, z, 0, ref morphes);
                        seq++;
                    }
                }
            }

            return map;
        }

        public UserMap Generate(int landscapeIndex, int skyboxIndex)
        {
            return GenerateInternal(landscapeTemplates[landscapeIndex], skyboxIndex);
        }
    }
}
