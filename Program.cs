using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetAsyncMultithreadChallenge;

public sealed class Counter
{
    private int _value;

    public int Value => Volatile.Read(ref _value);

    public int Increment()
    {
        return Interlocked.Increment(ref _value);
    }
}

public sealed class DataProcessor
{
    public async Task ProcessAsync(Counter counter, int iterations, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(counter);
        ArgumentOutOfRangeException.ThrowIfNegative(iterations);

        if (iterations == 0)
        {
            return;
        }

        var tasks = Enumerable.Range(0, iterations)
            .Select(_ => IncrementAsync(counter, cancellationToken))
            .ToArray();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static async Task IncrementAsync(Counter counter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Simulate asynchronous work without blocking a thread
        await Task.Yield();
        counter.Increment();
    }
}

public static class Program
{
    public static async Task Main(string[] args)
    {
        var counter = new Counter();
        var processor = new DataProcessor();

        await processor.ProcessAsync(counter, 1000);

        Console.WriteLine($"Final Counter Value: {counter.Value}");
    }
}
