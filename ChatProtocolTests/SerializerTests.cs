using System;
using System.IO;
using ChatProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatProtocolTests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void ReadWriteInt32()
        {
            byte[] data;
            using(var stream = new MemoryStream())
            {
                stream.WriteInt32(983572650);
                data = stream.ToArray();
            }

            using(var stream = new MemoryStream(data))
            {
                Assert.AreEqual(983572650, stream.ReadInt32());
            }
        }

        [TestMethod]
        public void ReadWriteMinValueForInt32()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteInt32(int.MinValue);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual(int.MinValue, stream.ReadInt32());
            }
        }

        [TestMethod]
        public void ReadWriteMaxValueForInt32()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteInt32(int.MaxValue);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual(int.MaxValue, stream.ReadInt32());
            }
        }
        [TestMethod]
        public void ReadWriteRussianString()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteString("привет, как дела");
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual("привет, как дела", stream.ReadString());
            }
        }
        [TestMethod]
        public void ReadWriteEnglishString()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteString("hi, how are you");
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual("hi, how are you", stream.ReadString());
            }
        }
        [TestMethod]
        public void ReadWriteEmptyString()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteString("");
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual("", stream.ReadString());
            }
        }

        [TestMethod]
        public void ReadWriteNullString()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                stream.WriteString(null);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                Assert.AreEqual(null, stream.ReadString());
            }
        }
    }
}
