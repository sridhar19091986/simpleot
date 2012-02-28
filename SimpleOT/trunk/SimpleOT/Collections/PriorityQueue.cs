/*
 * This code implements priority queue which uses min-heap as underlying storage
 * 
 * Copyright (C) 2010 Alexey Kurakin
 * www.avk.name
 * alexey[ at ]kurakin.me
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleOT.Collections
{
    /// <summary>
    /// Priority queue based on binary heap,
    /// Elements with minimum priority dequeued first
    /// </summary>
    /// <typeparam name="T">Type of values</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _baseHeap;

        /// <summary>
        /// Initializes a new instance of priority queue with specified initial capacity
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        public PriorityQueue(int capacity)
        {
            _baseHeap = new List<T>(capacity);
        }
		
        /// <summary>
        /// Initializes a new instance of priority queue
        /// </summary>
		public PriorityQueue() : this(8)
		{
		}

        /// <summary>
        /// Enqueues element into priority queue
        /// </summary>
        /// <param name="priority">element priority</param>
        /// <param name="value">element value</param>
        public void Enqueue(T value)
        {
            _baseHeap.Add(value);
            // heapify after insert, from end to beginning
            HeapifyFromEndToBeginning(_baseHeap.Count - 1);
        }

        /// <summary>
        /// Dequeues element with minimum priority and return its priority and value as <see cref="KeyValuePair{TPriority,TValue}"/> 
        /// </summary>
        /// <returns>priority and value of the dequeued element</returns>
        /// <remarks>
        /// Method throws <see cref="InvalidOperationException"/> if priority queue is empty
        /// </remarks>
        public T Dequeue()
        {
            if (!IsEmpty)
            {
				var result = _baseHeap[0];
				
	            if (_baseHeap.Count <= 1)
	                _baseHeap.Clear();
				else
				{
		            _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
		            _baseHeap.RemoveAt(_baseHeap.Count - 1);
		
		            // heapify
		            HeapifyFromBeginningToEnd(0);
				}
				
                return result;
            }
            else
                throw new InvalidOperationException("Priority queue is empty");
        }

        /// <summary>
        /// Returns priority and value of the element with minimun priority, without removing it from the queue
        /// </summary>
        /// <returns>priority and value of the element with minimum priority</returns>
        /// <remarks>
        /// Method throws <see cref="InvalidOperationException"/> if priority queue is empty
        /// </remarks>
        public T Peek()
        {
            if (!IsEmpty)
                return _baseHeap[0];
            else
                throw new InvalidOperationException("Priority queue is empty");
        }

        /// <summary>
        /// Gets whether priority queue is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _baseHeap.Count == 0; }
        }

        private int HeapifyFromEndToBeginning(int pos)
        {
            if (pos >= _baseHeap.Count) return -1;

            while (pos > 0)
            {
                int parentPos = (pos - 1) / 2;
                if (_baseHeap[parentPos].CompareTo(_baseHeap[pos]) > 0)
                {
		            var val = _baseHeap[parentPos];
            		_baseHeap[parentPos] = _baseHeap[pos];
            		_baseHeap[pos] = val;

                    pos = parentPos;
                }
                else break;
            }
            return pos;
        }

        private void HeapifyFromBeginningToEnd(int pos)
        {
            if (pos >= _baseHeap.Count) return;

            // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];

            while (true)
            {
                // on each iteration exchange element with its smallest child
                int smallest = pos;
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                if (left < _baseHeap.Count && _baseHeap[smallest].CompareTo(_baseHeap[left]) > 0)
                    smallest = left;
                if (right < _baseHeap.Count && _baseHeap[smallest].CompareTo(_baseHeap[right]) > 0)
                    smallest = right;

                if (smallest != pos)
                {
		            var val = _baseHeap[smallest];
            		_baseHeap[smallest] = _baseHeap[pos];
            		_baseHeap[pos] = val;
					
                    pos = smallest;
                }
                else break;
            }
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            _baseHeap.Clear();
        }

        /// <summary>
        /// Gets number of elements in the priority queue
        /// </summary>
        public int Count
        {
            get { return _baseHeap.Count; }
        }
    }
}
