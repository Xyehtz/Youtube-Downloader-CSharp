using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using NAudio.Wave;

namespace MusicPlayer
{
    class Program
    {
        public static void Player()
        {
            var mediaPlayer = new WaveOutEvent();
            string musicFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            string[] musicFiles = Directory.GetFiles(musicFolderPath, "*.mp3");

            foreach (string musicFile in musicFiles)
            {
                var audioFile = new AudioFileReader(musicFile);

                int lenght = musicFile.Length - 26;
                string subString = musicFile.Substring(22, lenght);
                Console.WriteLine($"Now playing {subString}");

                mediaPlayer.Init(audioFile);
                mediaPlayer.Play();

                Console.ReadKey();
                mediaPlayer.Dispose();
            }
        }
}
}
