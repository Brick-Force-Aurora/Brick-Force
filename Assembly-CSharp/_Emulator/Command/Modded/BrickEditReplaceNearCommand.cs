namespace _Emulator
{
    public class BrickEditReplaceNearCommand : ICommand
    {
        public string Description()
        {
            return "Sets all bricks within the provided radius around the player to the provided brick";
        }

        public void Execute(CommandContext context)
        {
            context.Reader.ReadTokens(2, 3, out string[] tokens);
            if (!context.IsInAuthorizedBuildRoom() || !tokens.AsRadius(0, out byte radius) || !tokens.AsBrick(1, out byte sourceTemplate, allowPalette: false) || !tokens.AsBrick(2, out byte targetTemplate))
            {
                return;
            }
            if (!targetTemplate.IsAllowedTarget())
            {
                return;
            }
            EditHelper.PlayerPosition.RadiusToCoords(radius, out byte x1, out byte y1, out byte z1, out byte x2, out byte y2, out byte z2);
            OperationProcessor.Instance.Enqueue(new ReplaceOperation(EditHelper.BrickEditTool, x1, y1, z1, x2, y2, z2, sourceTemplate, targetTemplate));
        }
    }
}
