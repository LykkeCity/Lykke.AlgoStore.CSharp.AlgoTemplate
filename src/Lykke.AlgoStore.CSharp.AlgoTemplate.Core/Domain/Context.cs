﻿using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// The <see cref="IContext"/> implementation provided to an algo when running.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// Access point to functions and function results for an
        /// algo <see cref="IFunctionProvider"/>
        /// </summary>
        public IFunctionProvider Functions { get; set; }

        public IOrderProvider Orders { get; set; }

        public IActions Actions { get; set; }
    }
}
