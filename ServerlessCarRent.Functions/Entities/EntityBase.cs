using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Entities
{
    /// <summary>
    /// Base class for entities in the ServerlessCarRent application.
    /// </summary>
    /// <typeparam name="TState">The type of the entity state.</typeparam>
    public abstract class EntityBase<TState> : TaskEntity<TState>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase{TState}"/> class.
        /// </summary>
        internal EntityBase()
        {
        }
        internal EntityBase(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Gets the current state of the entity.
        /// </summary>
        /// <remarks>This method is used for unit tests. Please use the State properties in the derived classes.</remarks>
        /// <returns>The current state of the entity.</returns>
        protected internal TState GetState()
        {
            return this.State;
        }

        /// <summary>
        /// Sets the state of the entity.
        /// </summary>
        /// <param name="state">The new state of the entity.</param>
        /// <remarks>This method is used for unit tests. Please use the State properties in the derived classes.</remarks>
        protected internal void SetState(TState state)
        {
            this.State = state;
        }
    }
}
