namespace FileCabinetApp.CommandHandler
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler.SetNext(commandHandler);
        }

        public AppCommandRequest Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                return this.nextHandler.Handle(request);
            }
            else
            {
                return null;
            }

        }
    }
}
