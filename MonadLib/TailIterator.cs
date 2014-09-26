using System;
using System.Collections;
using System.Collections.Generic;

namespace MonadLib
{
    internal class TailIterator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TailIterator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("Unexpected call to IEnumerable.GetEnumerator() in TailIterator<T>");
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            throw new InvalidOperationException("Unexpected call to Reset() in TailIterator<T>");
        }

        public T Current
        {
            get { return _enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get
            {
                throw new InvalidOperationException("Unexpected call to IEnumerator.Current() in TailIterator<T>");
            }
        }
    }
}
