namespace _Emulator
{
    public class BrickEditSetHollowCommand : ICommand
    {
        public string Description()
        {
            return "Sets all bricks on the edge of the selection of the selection tool to the provided brick";
        }

        public void Execute(CommandContext context)
        {
            context.Reader.ReadTokens(0, 1, out string[] tokens);
            if (!context.IsInAuthorizedBuildRoom() || !EditHelper.CheckSelection() || !tokens.AsBrick(0, out byte template))
            {
                return;
            }
            if (!template.IsAllowedTarget())
            {
                return;
            }
            OperationProcessor.Instance.Enqueue(new SetHollowOperation(EditHelper.BrickEditTool, template));
        }
    }
}
