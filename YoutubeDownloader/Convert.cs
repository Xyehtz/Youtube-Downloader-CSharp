using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownLoader {
    class Convert {
        /* This method will receive two strings
         * - The first string will be the path to the mp3 file
         * - The second string will be the path were the wav file will be located
         */

        public static void Mp3ToWav(string mp3File, string outputFile) {
            // Read the mp3 file using MediaFoundatioReader

            using (MediaFoundationReader reader = new MediaFoundationReader(mp3File)) {
                // Using WaveStream we will use the reader variable to obtain the PcmStream of the mp3

                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader)) {
                    // Using WaveFileWriter the program will write the Wav file using PcmStream and putting it on the output file

                    WaveFileWriter.CreateWaveFile(outputFile, pcmStream);

                    // When the conversion is finished delete the original mp3 file to save disk storage

                    File.Delete(mp3File);
                }
            }
        }
    }
}
