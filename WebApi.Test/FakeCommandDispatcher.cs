using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Test
{
    internal class FakeCommandDispatcher : ICommandDispatcher
    {
        private object? _command;
        private object? _reqult;
        private Exception? _exception;

        public Task Dispatch(ICommand command, CancellationToken ct = default)
        {
            _command = command;

            if (_exception != null)
            {
                throw _exception;
            }

            return Task.CompletedTask;
        }

        public Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken ct = default)
        {
            _command = command;

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

        public object? GetCommand()
        {
            return _command;
        }

        public T GetCommand<T>()
        {
            return (T)_command;
        }


    }
}
