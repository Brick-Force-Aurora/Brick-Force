namespace _Emulator
{
    public class BrickEditReplaceCommand : ICommand
    {
        public string Description()
        {
            return "Sets all bricks within the selection of the selection tool to the provided brick";
        }

        public void Execute(CommandContext context)
        {
            context.Reader.ReadTokens(1, 2, out string[] tokens);
            if (!context.IsInAuthorizedBuildRoom() || !EditHelper.CheckSelection() || !tokens.AsBrick(0, out byte sourceTemplate, allowPalette: false) || !tokens.AsBrick(1, out byte targetTemplate))
            {
                return;
            }
            if (!targetTemplate.IsAllowedTarget())
            {
                return;
            }
            OperationProcessor.Instance.Enqueue(new ReplaceOperation(EditHelper.BrickEditTool, sourceTemplate, targetTemplate));
        }
    }
}
