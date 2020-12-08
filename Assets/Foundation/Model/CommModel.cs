using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Foundation.Model
{
    [Serializable]
    public class CommModel
    {
        public byte[] Code = new byte[4];
        public int TestNumber;

        public CommModel()
        {
            Code = Encoding.ASCII.GetBytes("c0de");
            TestNumber = 5;
        }

        public byte[] GetBytes()
        {
            int byteSize = Code.Length + sizeof(int) * 1;
            byte[] bytes = new byte[byteSize];
            int index = 0;
            Code.CopyTo(bytes, index);
            index += Code.Length;
            BitConverter.GetBytes(TestNumber).CopyTo(bytes, index);

            return bytes;
        }
    }
}
