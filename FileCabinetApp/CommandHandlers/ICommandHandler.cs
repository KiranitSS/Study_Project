namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents interface for chain of handlers.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next handler.
        /// </summary>
        /// <param name="commandHandler">Handler which sets as next in chain.</param>
        /// <returns>Returns next handler back.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles request and send it to next handler.
        /// </summary>
        /// <param name="request">Request for handling.</param>
        /// <returns>Next request for handling.</returns>
        public AppCommandRequest Handle(AppCommandRequest request);
    }
}
