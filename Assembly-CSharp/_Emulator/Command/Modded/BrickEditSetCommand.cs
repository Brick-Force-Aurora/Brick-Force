namespace _Emulator
{
    public class BrickEditSetCommand : ICommand
    {
        public string Description()
        {
            return "Sets all bricks within the selection of the Swappie tool to the provided brick";
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
            OperationProcessor.Instance.Enqueue(new SetOperation(EditHelper.ReplaceTool, template));
        }
    }
}
