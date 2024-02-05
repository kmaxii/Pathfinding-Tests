using System;
using System.Collections.Generic;
using System.Linq;

namespace DetectionBehaviour
{
    // Priority Queue implementation (if needed)
    public class PriorityQueue<TElement, TPriority> where TElement : notnull
    {
        private SortedDictionary<TPriority, Queue<TElement>> elements = new SortedDictionary<TPriority, Queue<TElement>>();

        public void Enqueue(TElement item, TPriority priority)
        {
            if (!elements.ContainsKey(priority))
                elements[priority] = new Queue<TElement>();

            elements[priority].Enqueue(item);
        }

        public TElement Dequeue()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("The queue is empty");

            var firstPair = elements.First();
            var item = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0)
                elements.Remove(firstPair.Key);

            return item;
        }

        public bool Contains(TElement item)
        {
            return elements.Any(pair => pair.Value.Contains(item));
        }

        public int Count => elements.Sum(pair => pair.Value.Count);
    }
}