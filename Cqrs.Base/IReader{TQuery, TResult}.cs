using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Base
{
    public interface IReader<in TQuery, TResult>
        where TQuery : Query<TResult>
    {
        Task<IEnumerable<TResult>> ReadAsync(TQuery query);
    }

    static class Reader
    {
        // Invoke all matching registered readers and concatenate resultsets
        public async static Task<IEnumerable<TResult>> ReadAsync<TResult>(this Query<TResult> query) =>
            (await Task.WhenAll(
                ServiceLocator.Current
                    .GetAllInstances(typeof(IReader<,>)
                        .MakeGenericType(query.GetType(), typeof(TResult)))
                    .Select(reader =>
                        (Task<IEnumerable<TResult>>)reader
                            .GetType()
                            .GetMethod("ReadAsync", new[] { query.GetType() })
                            .Invoke(reader, new[] { query }))))
            .SelectMany(rr => rr)
            .ToArray();
    }
}
