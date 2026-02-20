namespace _Emulator
{
    public class BrickEditRemoveCommand : ICommand
    {
        public string Description()
        {
            return "Removes all bricks within the selection of the selection tool";
        }

        public void Execute(CommandContext context)
        {
            context.Reader.ReadTokens(0, 1, out string[] tokens);
            if (!context.IsInAuthorizedBuildRoom() || !EditHelper.CheckSelection() || !tokens.AsBrick(0, out byte sourceTemplate, allowPalette: false, optional: true))
            {
                return;
            }
            OperationProcessor.Instance.Enqueue(new DeleteOperation(EditHelper.BrickEditTool, sourceTemplate, tokens.Length == 1));
        }
    }
}
