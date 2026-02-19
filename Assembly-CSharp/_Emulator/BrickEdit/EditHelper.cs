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
                    if (main == null)
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

        public static bool AsBrick(this string[] tokens, int index, out byte template)
        {
            if (tokens.Length <= index)
            {
                return BrickCache.Instance.GetCurrentBrick(out template);
            }
            return byte.TryParse(tokens[index], out template) || BrickCache.Instance.GetBrickByName(tokens[index], out template);
        }

    }
}
