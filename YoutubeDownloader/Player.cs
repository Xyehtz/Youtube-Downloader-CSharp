using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using NAudio;
using NAudio.Wave;

namespace MusicPlayer
{
    class Program
    {
        public static void Player()
        {
            Console.Clear();
            var mediaPlayer = new WaveOutEvent();
            string folderPaht = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            var files = Directory.GetFiles(folderPaht, "*.wav");
            long fileSize = 0;

            if (files.Length == 0)
            {
                Console.WriteLine($"There are no compatible files on {folderPaht}");
                return;
            }

            foreach (var file in files)
            {
                System.IO.FileInfo fileInfo = new FileInfo(file);
                fileSize = fileInfo.Length;

                var audioFile = new AudioFileReader(file);
                int duration = (int)Math.Round(audioFile.TotalTime.TotalMilliseconds);

                int length = file.Length - 26;
                string subString = file.Substring(22, length);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Now playing {subString}");

                mediaPlayer.Init(audioFile);
                mediaPlayer.Play();

                Thread.Sleep(duration);
                mediaPlayer.Dispose();
            }
            Console.ResetColor();
            Console.WriteLine("Playlist finished");

        }
    }
}