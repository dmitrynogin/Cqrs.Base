using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Base
{
    public interface ILogger<in TEvent>
        where TEvent : Event
    {
        Task LogAsync(TEvent e);
    }

    static class Logger
    {
        // Invoke all matching registered loggers
        public static Task LogAsync(this Event e) =>
            Task.WhenAll(
                ServiceLocator.Current
                    .GetAllInstances(typeof(ILogger<>)
                        .MakeGenericType(e.GetType()))
                    .Select(logger => 
                        (Task)logger
                            .GetType()
                            .GetMethod("LogAsync", new[] { e.GetType() })
                            .Invoke(logger, new[] { e })));
    }
}
