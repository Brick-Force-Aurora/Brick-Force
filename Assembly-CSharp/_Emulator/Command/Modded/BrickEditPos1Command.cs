using System;
using UnityEngine;

namespace _Emulator
{
    public class BrickEditPos1Command : ICommand
    {
        public string Description()
        {
            return "Sets the first position of the selection tool";
        }

        public void Execute(CommandContext context)
        {
            if (!context.IsInAuthorizedBuildRoom())
            {
                return;
            }
            context.Reader.ReadTokens(0, 3, out string[] tokens);
            byte[] coordinates = new byte[3];
            GameObject gameObject = GameObject.Find("Me");
            Vector3 position = Vector3.zero;
            if (gameObject != null)
            {
                position = gameObject.transform.position;
            }
            coordinates[0] = (byte)Mathf.FloorToInt(Mathf.Clamp(position.x, 0f, 255f));
            coordinates[1] = (byte)Mathf.FloorToInt(Mathf.Clamp(position.y, 0f, 255f));
            coordinates[2] = (byte)Mathf.FloorToInt(Mathf.Clamp(position.z, 0f, 255f));
            for (int i = 0; i < tokens.Length; i++)
            {
                if (int.TryParse(tokens[i], out int value))
                {
                    coordinates[i] = (byte)Math.Min(Math.Max(value, 0), 255);
                }
            }
            ReplaceTool tool = EditHelper.ReplaceTool;
            if (tool == null)
            {
                return;
            }
            tool.SetPos1(coordinates[0], coordinates[1], coordinates[2]);
        }
    }
}
