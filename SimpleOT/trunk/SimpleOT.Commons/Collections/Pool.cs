using System;
using System.Collections.Generic;

namespace SimpleOT.Commons.Collections
{
	public class Pool<T> where T : new()
	{
		private Stack<T> _stack;
		private int _count;
		private int _minSize;
		private int _maxSize;
		
		public Pool (int minSize, int maxSize)
		{
			_minSize = minSize;
			_maxSize = maxSize;
			
			_stack = new Stack<T> (_maxSize);
		}
		
		public T Get ()
		{
			lock (_stack) {
				if (_count < _minSize || _stack.Count == 0)
					return Create ();
				
				return _stack.Pop ();
			}
		}
		
		public void Put(T value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			
			lock(_stack)
			{
				if(_stack.Count < _maxSize)
					_stack.Push(value);
				else
					Dispose(value);
			}
		}
		
		private T Create ()
		{
			_count++;
			T t = new T();
			
			if(t is IPoolable<T>)
				((IPoolable<T>)t).Pool = this;
			
			return t;
		}
				
		private void Dispose (T value)
		{
			if(value != null)
				_count--;
		}
	}
}

