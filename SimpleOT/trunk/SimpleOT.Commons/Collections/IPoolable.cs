using System;

namespace SimpleOT.Commons.Collections
{
	public interface IPoolable<T> where T : new()
	{
		Pool<T> Pool{get;set;}
		void Clear();
		void Release();
	}
}

