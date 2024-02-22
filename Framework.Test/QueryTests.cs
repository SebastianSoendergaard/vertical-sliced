using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Test
{
    public class QueryTests
    {
        IServiceProvider _serviceProvider;

        public QueryTests() 
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IQueryDispatcher, QueryDispatcher>();
            serviceCollection.AddScoped<IQueryHandler<TestQuery, int>, TestHandler>();
            serviceCollection.AddScoped<Store>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task ShouldPerformQuery_WhenHandlingThroughDispatcher()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();

            var answer = await dispatcher.Dispatch(new TestQuery(), new CancellationToken());

            Assert.Equal(42, answer);
        }

        [Fact]
        public async Task ShouldPerformQuery_WhenHandlingDirectlyThroughQueryHandler()
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();

            var answer = await handler.Handle(new TestQuery(), new CancellationToken());

            Assert.Equal(42, answer);
        }

        [Fact(Skip = "Only relevant during debug")]
        public async Task PerformanceTest()
        {
            int testIterations = 1000000;

            // Measure time multiple times for each method to get a good average

            var timeDispatcher1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler1 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
                return await handler.Handle(new TestQuery(), new CancellationToken());
            });

            var timeDispatcher2 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler2 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
                return await handler.Handle(new TestQuery(), new CancellationToken());
            });

            var timeDispatcher3 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler3 = await MeasureExecutionTime<int>(testIterations, async () => {
                var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
                return await handler.Handle(new TestQuery(), new CancellationToken());
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

        class TestQuery : TestInterface1, IQuery<int>, TestInterface2
        {
        }

        class TestHandler(Store store) : IQueryHandler<TestQuery, int>
        {
            public async Task<int> Handle(TestQuery query, CancellationToken ct)
            {
                return await store.GetValue();
            }
        }

        // Interfaces to verify that Query can be handled correct even if it implements additional interfaces
        interface TestInterface1 { }
        interface TestInterface2 { }

        // Service class to verify that QueryHandler can have a service injected
        class Store
        {
            public async Task<int> GetValue() => await Task.FromResult(42);
        }
    }
}