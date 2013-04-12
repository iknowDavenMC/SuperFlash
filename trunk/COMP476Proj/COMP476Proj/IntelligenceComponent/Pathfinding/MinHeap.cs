using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj
{
    /// <summary>
    /// Implementation of a MinHeap
    /// </summary>
    /// <typeparam name="T">T must implement IComparable</typeparam>
    class MinHeap<T> where T : IComparable<T>
    {
        public List<T> data = new List<T>(); // Complete list of nodes
        public T Top { get { return data[0]; } }

        /// <summary>
        /// Add an item to the heap
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Enqueue(T item)
        {
            data.Add(item);
            int childInd = data.Count - 1;
            while (childInd > 0)
            {
                int parentInd = (childInd - 1) / 2;
                if (data[childInd].CompareTo(data[parentInd]) >= 0)
                    break;
                T tmp = data[childInd];
                data[childInd] = data[parentInd];
                data[parentInd] = tmp;
                childInd = parentInd;
            }
        }

        /// <summary>
        /// Remove the top item in the heap (ie: the one with the lowest value)
        /// </summary>
        /// <returns>The removed item</returns>
        public T Dequeue()
        {
            int lastInd = data.Count - 1;
            T top = data[0];
            data[0] = data[lastInd];
            data.RemoveAt(lastInd);

            --lastInd;
            int parentInd = 0;
            while (true)
            {
                int childInd = parentInd * 2 + 1;
                if (childInd > lastInd)
                    break;
                int rightChild = childInd + 1;
                if (rightChild <= lastInd && data[rightChild].CompareTo(data[childInd]) < 0)
                    childInd = rightChild;
                if (data[parentInd].CompareTo(data[childInd]) <= 0)
                    break;
                T tmp = data[parentInd];
                data[parentInd] = data[childInd];
                data[childInd] = tmp;
                parentInd = childInd;
            }
            return top;
        }

        /// <summary>
        /// Check if the heap is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return data.Count == 0;
        }

        /// <summary>
        /// Clear the heap of all data
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }
    }
}
