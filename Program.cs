using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace WAVBitDepthEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("File to edit (without .wav): ");
            string origFileName = Console.ReadLine();
            byte[] origAudio = null;

            try
            {
                origAudio = File.ReadAllBytes("C:\\Users\\Isaac Barden\\source\\repos\\WAVBitDepthEditor\\audio\\" + origFileName + ".wav");
            }
            catch
            {
                Console.WriteLine("File not found.");
                Environment.Exit(0);
            }
            List<byte> outputAudio = new List<byte>();

            string dataFinder = BitConverter.ToString(origAudio, 0, 500);

            int startIndex = ((dataFinder.IndexOf("64-61-74-61")/3)+8);

            for (int i = startIndex; i < origAudio.Length; i += 4)
            {
                var sampleFloat = BitConverter.ToSingle(origAudio, i);

                float normalizedFloat = (sampleFloat) * Int16.MaxValue;

                Int16 sampleInt16 = Convert.ToInt16(normalizedFloat);
                outputAudio.Add((byte)(sampleInt16 & 0xFFu));
                outputAudio.Add((byte)((sampleInt16 >> 8) & 0xFFu));
            }
            byte[] newFile = outputAudio.ToArray();

            uint samples = (uint)(newFile.Length / 2);
            ushort channels = BitConverter.ToUInt16(origAudio, 0x16);
            uint samplerate = BitConverter.ToUInt32(origAudio, 0x18);
            ushort samplelength = 2; //bytes

            FileStream fs = new FileStream("C:\\Users\\Isaac Barden\\source\\repos\\WAVBitDepthEditor\\audio\\" + origFileName + "_out.wav", FileMode.Create);
            BinaryWriter wr = new BinaryWriter(fs);

            wr.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            wr.Write(36 + samples * channels * samplelength);
            wr.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
            wr.Write(16);
            wr.Write((ushort)1);
            wr.Write(channels);
            wr.Write(samplerate);
            wr.Write(samplerate * samplelength * channels);
            wr.Write((ushort)(samplelength * channels));
            wr.Write((ushort)(8 * samplelength));
            wr.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            wr.Write(samples * samplelength);
            wr.Write(newFile);
        }
    }
}
