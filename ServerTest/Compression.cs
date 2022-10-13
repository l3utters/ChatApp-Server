using System.IO;
using System.IO.Compression;

namespace ServerTest
{
    class Compression
    {

        //public static byte[] CompressBytes(byte[] data)
        //{
        //    MemoryStream output = new MemoryStream();
        //    using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
        //    {
        //        dstream.Write(data, 0, data.Length);
        //    }
        //    return output.ToArray();
        //}

        //public static byte[] DecompressBytes(byte[] data)
        //{
        //    MemoryStream input = new MemoryStream(data);
        //    MemoryStream output = new MemoryStream();
        //    using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
        //    {
        //        dstream.CopyTo(output);
        //    }
        //    return output.ToArray();
        //}

        public static byte[] CompressBytes(byte[] bytes)
        {
            //Simply write the bytes to memory using the .Net compression stream
            MemoryStream output = new MemoryStream();
            GZipStream gzip = new GZipStream(output, CompressionMode.Compress, true);
            gzip.Write(bytes, 0, bytes.Length);
            gzip.Close();
            return output.ToArray();
        }

        public static byte[] DecompressBytes(byte[] bytes)
        {
            //Use the .Net decompression stream in memory
            MemoryStream input = new MemoryStream();
            input.Write(bytes, 0, bytes.Length);
            input.Position = 0;

            GZipStream gzip = new GZipStream(input, CompressionMode.Decompress, true);
            MemoryStream output = new MemoryStream();

            byte[] buff = new byte[64]; //Compressed bytes are read in 64 bytes at a time
            int read = -1;
            read = gzip.Read(buff, 0, buff.Length);
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }

            gzip.Close();
            return output.ToArray();
        }
    }
}
