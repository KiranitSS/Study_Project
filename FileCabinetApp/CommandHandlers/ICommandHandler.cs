namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        public void SetNext(ICommandHandler commandHandler);

        public AppCommandRequest Handle(AppCommandRequest request);
    }
}
