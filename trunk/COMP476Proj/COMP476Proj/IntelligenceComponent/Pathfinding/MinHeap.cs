using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj
{
    class MinHeap<T> where T : IComparable<T>
    {
        public List<T> data = new List<T>();
        public T Top { get { return data[0]; } }

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

        public bool IsEmpty()
        {
            return data.Count == 0;
        }

        public void Clear()
        {
            data.Clear();
        }

        public T Select(T obj)
        {
            if (data.Contains(obj))
                return data[data.IndexOf(obj)];
            return default(T);
        }
    }
}
