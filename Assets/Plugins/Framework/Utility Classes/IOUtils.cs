using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Framework
{
    public static class IOUtils
    {
        private static uint[] _crcTable;

        /// <summary>
        /// Computes the CRC32 checksum for a block of data.
        /// </summary>
        /// <param name="bytes">The input data</param>
        /// <returns>The computed checksum</returns>
        public static uint GetCRCChecksum(byte[] bytes)
        {
            if (_crcTable == null) ComputeCRCTable();

            uint crc = 0xffffffff;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = ((crc >> 8) ^ _crcTable[index]);
            }
            return ~crc;
        }

        /// <summary>
        /// Computes the CRC32 checksum for a block of data.
        /// </summary>
        /// <param name="bytes">The input data</param>
        /// <returns>The computed checksum</returns>
        public static byte[] GetCRCChecksumBytes(byte[] bytes)
        {
            return BitConverter.GetBytes(GetCRCChecksum(bytes));
        }

        static void ComputeCRCTable()
        {
            uint temp;
            uint poly = 0xedb88320;
            _crcTable = new uint[256];

            for (uint i = 0; i < _crcTable.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = ((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                _crcTable[i] = temp;
            }
        }

        /// <summary>
        /// Converts a serializable object instance to a byte array.
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted byte array</returns>
        public static byte[] ObjectToBytes(object obj)
        {
            byte[] data = new byte[0];

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {

                binaryFormatter.Serialize(memoryStream, obj);
                data = memoryStream.ToArray();

            }

            return data;
        }

        /// <summary>
        /// Converts a byte array back into its original object.
        /// </summary>
        /// <typeparam name="T">The original object's type</typeparam>
        /// <param name="data">The bytes to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T BytesToObject<T>(byte[] data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {

                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Converts a serializable object instance to a byte array and writes it to a file.
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="filepath">The path of the file to write</param>
        /// <param name="encryption">The encryption scheme to use</param>
        /// <param name="compression">The compression scheme to use</param>
        public static void ObjectToFile(object obj, string filepath)
        {
            File.WriteAllBytes(filepath, ObjectToBytes(obj));
        }

        /// <summary>
        /// Reads a file and converts it back into its original object.
        /// </summary>
        /// <typeparam name="T">The original object's type</typeparam>
        /// <param name="filepath">The path of the file to deserialize</param>
        /// <param name="encryption">The encryption scheme to use</param>
        /// <param name="compression">The compression scheme to use</param>
        /// <returns>The deserialized object</returns>
        public static T FileToObject<T>(string filepath)
        {
            return BytesToObject<T>(File.ReadAllBytes(filepath));
        }
    }
}
