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
            // Clear the console for better user experience

            Console.Clear();

            // Create a new WaveOutEvent, this will later play the music

            var mediaPlayer = new WaveOutEvent();

            // Obtain the Public Music folder and filter everything so that only wav files are played

            string folderPaht = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            var files = Directory.GetFiles(folderPaht, "*.wav");
            long fileSize = 0;

            // Check if there are .wav files inside of the folder, if not, display an error message

            if (files.Length == 0)
            {
                Console.WriteLine($"There are no compatible files on {folderPaht}");
                return;
            }

            // Start playing every file in alphabetical order

            foreach (var file in files)
            {
                // Obtain the info of every file

                System.IO.FileInfo fileInfo = new FileInfo(file);
                fileSize = fileInfo.Length;

                // Obtain the total duration of the file in milliseconds using AudioFileReader

                var audioFile = new AudioFileReader(file);
                int duration = (int)Math.Round(audioFile.TotalTime.TotalMilliseconds);

                // Cut the string so that only the title of the song is displayed

                int length = file.Length - 26;
                string subString = file.Substring(22, length);

                // Change the color of the text to  magenta and show the song that is currently playing

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Now playing {subString}");

                // Start the song

                mediaPlayer.Init(audioFile);
                mediaPlayer.Play();

                // After the song starts playing the program will sleep for the song duration, after this mediaPlayer is disposed

                Thread.Sleep(duration);
                mediaPlayer.Dispose();
            }

            // When all the songs are finished show the user a message about it

            Console.ResetColor();
            Console.WriteLine("Playlist finished");

        }
    }
}