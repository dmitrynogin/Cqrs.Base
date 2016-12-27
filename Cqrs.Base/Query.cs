using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Base
{
    public abstract class Query<TResult> : Request
    {
        protected Query()
        {
            Data = new Lazy<Task<IEnumerable<TResult>>>(ExecuteAsync);
        }

        public TaskAwaiter<IEnumerable<TResult>> GetAwaiter() =>
            Data.Value.GetAwaiter();

        Lazy<Task<IEnumerable<TResult>>> Data { get; }

        protected virtual async Task<IEnumerable<TResult>> ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await this.ValidateAsync();
                return await this.ReadAsync();
            }
            catch(Exception ex)
            {
                Error = ex;
                throw;
            }
            finally
            {
                Taken = sw.Elapsed;
                await this.LogAsync();
            }
        }
    }
}
