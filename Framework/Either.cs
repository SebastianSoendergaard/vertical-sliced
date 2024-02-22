using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class Either<TLeft, TRight>
    {
        private readonly TLeft? _left;
        private readonly TRight? _right;
        private readonly bool _isLeft;

        public Either(TLeft? left)
        {
            _left = left;
            _isLeft = true;
        }

        public Either(TRight? right)
        {
            _right = right;
            _isLeft = false;
        }

        public T Match<T>(Func<TLeft?, T> leftFunc, Func<TRight?, T> rightFunc)
        {
            if (leftFunc == null)
            {
                throw new ArgumentNullException(nameof(leftFunc));
            }

            if (rightFunc == null)
            {
                throw new ArgumentNullException(nameof(rightFunc));
            }

            return this._isLeft ? leftFunc(_left) : rightFunc(_right);
        }

        /// <summary>
        /// If right value is assigned, execute an action on it.
        /// </summary>
        /// <param name="rightAction">Action to execute.</param>
        public void DoRight(Action<TRight?> rightAction)
        {
            if (rightAction == null)
            {
                throw new ArgumentNullException(nameof(rightAction));
            }

            if (!_isLeft)
            {
                rightAction(_right);
            }
        }

        public TLeft? LeftOrDefault() => this.Match(l => l, r => default);

        public TRight? RightOrDefault() => this.Match(l => default, r => r);

        public static implicit operator Either<TLeft, TRight>(TLeft? left) => new(left);

        public static implicit operator Either<TLeft, TRight>(TRight? right) => new(right);
    }
}
