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
            serviceCollection.AddSingleton<PipelineLogger>();

            var middlewareRegistry = new QueryMiddlewareRegistry(serviceCollection);
            middlewareRegistry.Register<TestMiddlwareA>();
            middlewareRegistry.Register<TestMiddlwareB>();

            serviceCollection.AddScoped<IQueryDispatcher, QueryDispatcher>();
            serviceCollection.AddSingleton(middlewareRegistry);
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

        [Fact]
        public async Task ShouldCallMiddlewareInRightOrder_WhenHandlingQuery()
        {
            var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
            var logger = _serviceProvider.GetRequiredService<PipelineLogger>();

            var answer = await dispatcher.Dispatch(new TestQuery(), new CancellationToken());

            Assert.Collection(logger.Messages,
                msg => Assert.Equal("Entering Middleware A", msg),
                msg => Assert.Equal("Entering Middleware B", msg),
                msg => Assert.Equal("Entering Handler", msg),
                msg => Assert.Equal("Exiting Handler", msg),
                msg => Assert.Equal("Exiting Middleware B", msg),
                msg => Assert.Equal("Exiting Middleware A", msg)
            );
        }

        [Fact(Skip = "Only relevant during debug")]
        //[Fact]
        public async Task PerformanceTest()
        {
            int testIterations = 1000000;

            // Measure time multiple times for each method to get a good average

            var timeDispatcher1 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler1 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
                return await handler.Handle(new TestQuery(), new CancellationToken());
            });

            var timeDispatcher2 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler2 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var handler = _serviceProvider.GetRequiredService<IQueryHandler<TestQuery, int>>();
                return await handler.Handle(new TestQuery(), new CancellationToken());
            });

            var timeDispatcher3 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
                var dispatcher = _serviceProvider.GetRequiredService<IQueryDispatcher>();
                return await dispatcher.Dispatch(new TestQuery(), new CancellationToken());
            });
            var timeHandler3 = await MeasureExecutionTime<int>(testIterations, async () =>
            {
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

        class TestHandler(Store store, PipelineLogger logger) : IQueryHandler<TestQuery, int>
        {
            public async Task<int> Handle(TestQuery query, CancellationToken ct)
            {
                logger.Add("Entering Handler");
                var value = await store.GetValue();
                logger.Add("Exiting Handler");
                return value;
            }
        }

        class TestMiddlwareA(PipelineLogger logger) : IQueryMiddleware
        {
            public async Task<TResult> Handle<TResult>(IQuery<TResult> query, Func<IQuery<TResult>, CancellationToken, Task<TResult>> next, CancellationToken ct = new CancellationToken())
            {
                logger.Add("Entering Middleware A");
                var result = await next(query, ct);
                logger.Add("Exiting Middleware A");
                return result;
            }
        }

        class TestMiddlwareB(PipelineLogger logger) : IQueryMiddleware
        {
            public async Task<TResult> Handle<TResult>(IQuery<TResult> query, Func<IQuery<TResult>, CancellationToken, Task<TResult>> next, CancellationToken ct = new CancellationToken())
            {
                logger.Add("Entering Middleware B");
                var result = await next(query, ct);
                logger.Add("Exiting Middleware B");
                return result;
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

        class PipelineLogger
        {
            public IList<string> Messages { get; } = new List<string>();
            public void Add(string message)
            {
                Messages.Add(message);
            }
        }
    }
}