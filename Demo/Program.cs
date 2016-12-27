using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Cqrs.Base;
using Microsoft.Practices.ServiceLocation;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync());
        }

        async static Task MainAsync()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<IntSequence>().AsImplementedInterfaces();
            builder.RegisterType<RangeValidator>().AsImplementedInterfaces();
            builder.RegisterType<RangeLogger>().AsImplementedInterfaces();

            var container = builder.Build();
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);

            foreach (var i in await new IntRange(10, 5))
                Console.WriteLine(i);

            try
            {
                await new IntRange(10, 100);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    class IntRange : Query<int>
    {
        public IntRange(int start, int count)
        {
            Start = start;
            Count = count;
        }

        public int Start { get; }
        public int Count { get; }
    }

    class RangeValidator : IValidator<IntRange>
    {
        public Task ValidateAsync(IntRange subject) =>
            subject.Count < 0 || subject.Count > 10 ?
                Task.FromException(new ArgumentOutOfRangeException()) :
                Task.CompletedTask;
    }

    class IntSequence : IReader<IntRange, int>
    {
        public Task<IEnumerable<int>> ReadAsync(IntRange subject) =>
            Task.FromResult(
                Enumerable.Range(subject.Start, subject.Count));
    }

    class RangeLogger : ILogger<IntRange>
    {
        public Task LogAsync(IntRange e) =>
            Console.Out.WriteLineAsync($"Time taken {e.Taken}");        
    }
}
