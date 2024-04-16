using Framework;

namespace WebApi.Test
{
    internal class FakeQueryDispatcher : IQueryDispatcher
    {
        private object? _query;
        private object? _reqult;
        private Exception? _exception;

        public Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            _query = query;

            if (_exception != null)
            { 
                throw _exception;
            }
            
            return Task.FromResult((TResult)_reqult);
        }

        public void SetResult(object result)
        {
            _reqult = result;
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
        }

        public object? GetQuery()
        {
            return _query;
        }

        public T GetQuery<T>()
        {
            return (T)_query;
        }
    }
}
