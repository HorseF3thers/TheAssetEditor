﻿using System.Runtime.InteropServices;
using System.Text;

namespace SharedCore.ByteParsing
{
    public class ByteHelper
    {
        public static T ByteArrayToStructure<T>(byte[] bytes, int offset) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            var objectSize = GetSize<T>();
            if (offset + objectSize > bytes.Length)
                throw new Exception($"Object {typeof(T)} does not fit into the remaining buffer [offset{offset} + Size{objectSize} => byteBuffer{bytes.Length}]");

            try
            {
                var p = handle.AddrOfPinnedObject() + offset;
                return (T)Marshal.PtrToStructure(p, typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static byte[] GetBytes<T>(T data) where T : struct
        {
            var size = Marshal.SizeOf(data);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        static public byte[] CreateFixLengthString(string str, int maxLength)
        {
            var output = new byte[maxLength];
            var byteValues = Encoding.UTF8.GetBytes(str);
            for (var i = 0; i < byteValues.Length && i < maxLength; i++)
                output[i] = byteValues[i];
            return output;
        }

        public static int GetSize(Type type)
        {
            return Marshal.SizeOf(type);
        }

        public static int GetSize<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
}