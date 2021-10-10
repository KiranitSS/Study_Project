using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents abstract class which implements <see cref="ICommandHandler"/>.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <inheritdoc/>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return commandHandler;
        }

        /// <inheritdoc/>
        public virtual AppCommandRequest Handle(AppCommandRequest request)
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
