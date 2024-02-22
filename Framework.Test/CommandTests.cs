using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Test
{
    public class CommandTests
    {
        IServiceProvider _serviceProvider;

        public CommandTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();
            serviceCollection.AddScoped<ICommandHandler<TestCommandWithoutResult>, TestHandlerWithoutResult>();
            serviceCollection.AddScoped<ICommandHandler<TestCommandWithResult, int>, TestHandlerWithResult>();
            serviceCollection.AddScoped<Store>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task ShouldPerformCommand_WithoutResult_WhenHandlingThroughDispatcher()
        {
            var store = _serviceProvider.GetRequiredService<Store>();
            var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();

            await dispatcher.Dispatch(new TestCommandWithoutResult { Value = 42 }, new CancellationToken());

            Assert.Equal(42, await store.GetValue());
        }

        [Fact]
        public async Task ShouldPerformCommand_WithResult_WhenHandlingThroughDispatcher()
        {
            var store = _serviceProvider.GetRequiredService<Store>();
            var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();

            var answer = await dispatcher.Dispatch(new TestCommandWithResult { Value = 42 }, new CancellationToken());

            Assert.Equal(42, await store.GetValue());
            Assert.Equal(42 * 2, answer);
        }

        [Fact]
        public async Task ShouldPerformCommand_WithoutResult_WhenHandlingThroughCommandHandler()
        {
            var store = _serviceProvider.GetRequiredService<Store>();
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithoutResult>>();

            await handler.Handle(new TestCommandWithoutResult { Value = 42 }, new CancellationToken());

            Assert.Equal(42, await store.GetValue());
        }

        [Fact]
        public async Task ShouldPerformCommand_WithResult_WhenHandlingThroughCommandHandler()
        {
            var store = _serviceProvider.GetRequiredService<Store>();
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithResult, int>>();

            var answer = await handler.Handle(new TestCommandWithResult { Value = 42 }, new CancellationToken());

            Assert.Equal(42, await store.GetValue());
            Assert.Equal(42 * 2, answer);
        }

        [Fact(Skip = "Only relevant during debug")]
        //[Fact]
        public async Task PerformanceTest_ForCommandWithoutResult()
        {
            int testIterations = 1000000;

            // Measure time multiple times for each method to get a good average

            var timeDispatcher1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });
            var timeHandler1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithoutResult>>();
                await handler.Handle(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher2 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });
            var timeHandler2 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithoutResult>>();
                await handler.Handle(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher3 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });
            var timeHandler3 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithoutResult>>();
                await handler.Handle(new TestCommandWithoutResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher = timeDispatcher1 + timeDispatcher2 + timeDispatcher3;
            var timeHandler = timeHandler1 + timeHandler2 + timeHandler3;

            var factor = timeDispatcher / timeHandler;

            var timeWithReflectionAvg = timeDispatcher / (3 * testIterations);
            var timeHandlerAvg = timeHandler / (3 * testIterations);
        }

        [Fact(Skip = "Only relevant during debug")]
        //[Fact]
        public async Task PerformanceTest_ForCommandWithResult()
        {
            int testIterations = 1000000;

            // Measure time multiple times for each method to get a good average

            var timeDispatcher1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });
            var timeHandler1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithResult, int>>();
                await handler.Handle(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher2 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });
            var timeHandler2 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithResult, int>>();
                await handler.Handle(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher3 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
                await dispatcher.Dispatch(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });
            var timeHandler3 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithResult, int>>();
                await handler.Handle(new TestCommandWithResult(), new CancellationToken());
                return 0;
            });

            var timeDispatcher = timeDispatcher1 + timeDispatcher2 + timeDispatcher3;
            var timeHandler = timeHandler1 + timeHandler2 + timeHandler3;

            var factor = timeDispatcher / timeHandler;

            var timeWithReflectionAvg = timeDispatcher / (3 * testIterations);
            var timeHandlerAvg = timeHandler / (3 * testIterations);
        }

        private async Task<TimeSpan> MeasureExecutionTime<TResult>(int testIterations, Func<Task<TResult>> testMethod)
        {
            var sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < testIterations; i++)
            {
                await testMethod();
            }
            sw.Stop();

            return sw.Elapsed;
        }

        class TestCommandWithoutResult : TestInterface1, ICommand, TestInterface2
        {
            public int Value { get; init; }
        }

        class TestCommandWithResult : TestInterface1, ICommand<int>, TestInterface2
        {
            public int Value { get; init; }
        }

        class TestHandlerWithoutResult(Store store) : ICommandHandler<TestCommandWithoutResult>
        {
            public async Task Handle(TestCommandWithoutResult command, CancellationToken ct)
            {
                await store.SetValue(command.Value);
            }
        }

        class TestHandlerWithResult(Store store) : ICommandHandler<TestCommandWithResult, int>
        {
            public async Task<int> Handle(TestCommandWithResult command, CancellationToken ct)
            {
                await store.SetValue(command.Value);
                return (await store.GetValue()) * 2;
            }
        }

        // Interfaces to verify that Query can be handled correct even if it implements additional interfaces
        interface TestInterface1 { }
        interface TestInterface2 { }

        // Service class to verify that QueryHandler can have a service injected
        class Store
        {
            private int value = 0;
            public async Task SetValue(int newValue) { value = newValue; await Task.CompletedTask; }
            public async Task<int> GetValue() => await Task.FromResult(value);
        }
    }
}
