namespace FileCabinetApp.CommandHandler
{
    public interface ICommandHandler
    {
        public void SetNext(ICommandHandler commandHandler);

        public AppCommandRequest Handle(AppCommandRequest request);
    }
}
