namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        public ICommandHandler SetNext(ICommandHandler commandHandler);

        public AppCommandRequest Handle(AppCommandRequest request);
    }
}
