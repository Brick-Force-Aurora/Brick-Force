namespace _Emulator
{
    public class BrickEditRemoveCommand : ICommand
    {
        public string Description()
        {
            return "Removes all bricks within the selection of the Swappie tool";
        }

        public void Execute(CommandContext context)
        {
            if (!context.IsInAuthorizedBuildRoom() || !EditHelper.CheckSelection())
            {
                return;
            }
            OperationProcessor.Instance.Enqueue(new DeleteOperation(EditHelper.BrickEditTool));
        }
    }
}
