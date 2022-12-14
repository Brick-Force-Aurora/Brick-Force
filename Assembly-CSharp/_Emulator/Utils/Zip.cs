using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace _Emulator
{
    class Zip
    {
        public static byte[] Compress(byte[] input)
        {
            // Create the compressor with highest level of compression  
            Deflater compressor = new Deflater();
            compressor.SetLevel(Deflater.BEST_COMPRESSION);

            // Give the compressor the data to compress  
            compressor.SetInput(input);
            compressor.Finish();

            /* 
             * Create an expandable byte array to hold the compressed data. 
             * You cannot use an array that's the same size as the orginal because 
             * there is no guarantee that the compressed data will be smaller than 
             * the uncompressed data. 
             */
            MemoryStream bos = new MemoryStream(input.Length);

            // Compress the data  
            byte[] buf = new byte[1024];
            while (!compressor.IsFinished)
            {
                int count = compressor.Deflate(buf);
                bos.Write(buf, 0, count);
            }

            // Get the compressed data  
            return bos.ToArray();
        }

        /*public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.Read(output, )
            }
            return output.ToArray();
        }*/
    }
}
