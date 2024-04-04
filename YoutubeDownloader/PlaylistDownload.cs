using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownLoader {
    class PlaylistDownload {
        public static async Task PlaylistDownloadMethod(YoutubeClient youtube, string url) {
            // The method will start by obtaining the information inside of the playlist

            var playlist = youtube.Playlists.GetAsync(url);

            // After obtaining the information of the playlist the method will go through all the videos inside the playlist

            await foreach (var video in youtube.Playlists.GetVideosAsync(url)) {
                // For every video inside the playlist the program will obtain the title, author and the duration

                var title = video.Title;
                var author = video.Author;
                var duration = video.Duration;

                // The information will be displayed to the user to check if everything is ok

                Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}\n");
            }

            // Declare the download counter and let the user know that the download will start

            int i = 1;
            Console.WriteLine("The download will start in a few seconds");

            // Go through every video in the playlist

            await foreach (var video in youtube.Playlists.GetVideosAsync(url)) {
                // Check if the videos have an invalid character on it's title

                if (!Verification.ContainsInvalidCharacters(video.Title)) {
                    /* If the title of the video doesn't contain any invalid character the program will obtain the manifest of the video 
                     * - After obtaining the manifest the program will take only the mp3 options with the highest bit-rate
                     */

                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                    var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                    // Start the download of the file

                    await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{video.Title}.mp3"));

                    // Call the Mp3ToWav method to convert the file into a .Wav audio file

                    Convert.Mp3ToWav(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{video.Title}.mp3"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{video.Title}.wav"));

                    // Turn the color text to green and let the user know that the download is complete

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Successfully downloaded {video.Title}. Total downloads: {i}");

                    // Update the download counter

                    i++;
                } else {
                    // If the video title contains invalid characters turn the text color to red and write an error message

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Skipping {video.Title} due to it containing a special character, no windows file can contain special characters\n");
                    Console.ResetColor();
                    continue;
                }
                // Reset the text color

                Console.ResetColor();
            }

            // After every file has been downloaded let the user know

            Console.WriteLine($"{i} files have been downloaded on:\n\t{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
        }
    }
}
