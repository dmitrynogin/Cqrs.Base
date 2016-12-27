using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Base
{
    public interface IValidator<TRequest>
        where TRequest : Request
    {
        Task ValidateAsync(TRequest request);
    }

    static class Validator
    {
        // Invoke all matching registered validators
        public static Task ValidateAsync(this Request request) =>
            Task.WhenAll(
                ServiceLocator.Current
                    .GetAllInstances(typeof(IValidator<>)
                        .MakeGenericType(request.GetType()))
                    .Select(validator =>
                        (Task)validator
                            .GetType()
                            .GetMethod("ValidateAsync", new[] { request.GetType() })
                            .Invoke(validator, new[] { request })));
    }
}
