namespace _Emulator
{
    public class BrickEditRemoveNearCommand : ICommand
    {
        public string Description()
        {
            return "Removes all bricks within the provided radius around the player";
        }

        public void Execute(CommandContext context)
        {
            context.Reader.ReadTokens(1, 2, out string[] tokens);
            if (!context.IsInAuthorizedBuildRoom() || !tokens.AsRadius(0, out byte radius) || !tokens.AsBrick(1, out byte sourceTemplate, allowPalette: false, optional: true))
            {
                return;
            }
            EditHelper.PlayerPosition.RadiusToCoords(radius, out byte x1, out byte y1, out byte z1, out byte x2, out byte y2, out byte z2);
            OperationProcessor.Instance.Enqueue(new DeleteOperation(x1, y1, z1, x2, y2, z2, sourceTemplate, tokens.Length == 2));
        }
    }
}
