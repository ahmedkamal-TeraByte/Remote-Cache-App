using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay
{
    [Serializable]
    public class TestObject
    {
        private byte[] bytes;
        public TestObject(int length)
        {
            bytes = new byte[length];
        }


    }
}
