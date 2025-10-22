using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    internal class Chunk
    {
        private int _startIndex;
        private int _endIndex;
        private int _size;
        public int StartIndex { get { return _startIndex; } set { _startIndex = value; } }
        public int EndIndex { get { return _endIndex; } set { _endIndex = value; } }
        public int Size { get { return _size; } set { _size = value; } }
        public Chunk(int startindex, int endindex, int size)
        {
            StartIndex = startindex;
            EndIndex = endindex;
            Size = size;
        }
    }
}
