using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class Stack<T>
    {
        private MyList<T> _list;

        public Stack()
        {
            _list = new MyList<T>();
        }

        public void Push(T item)
        {
            _list.Add(item);
        }

        public T Pop()
        {
            var item = _list[^1];
            _list.RemoveLast();
            return item;
        }

        public T Peek()
        {
           return _list[^1];
        }

    }
}
