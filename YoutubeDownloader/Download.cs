using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownLoader {
    class Download {
        public static async Task DownloadMethod(YoutubeClient youtube, string url, string title) {
            // Obtain the variables and obtain again the information of the youtube video

            var video = await youtube.Videos.GetAsync(url);

            // Verify if the video title contains any type of invalid characters

            if (!Verification.ContainsInvalidCharacters(video.Title)) {
                // If ContainsInvalidCharacters is false, meaning that there are no invalid characters the download process will start

                /* Obtain the manifest of the video
                 * After the manifest is obtained the program will select only the audio versions of the video
                 * After obtaining the audio versions of the video, the program will get the one with the highest bit-rate
                 */

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                // Download the YouTube video, the first parameter is the Stream Info and the second one is the path where the file will be downloaded
                // In this case the file will be downloaded on the Public Music folder

                await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"));

                /* After downloading the file, the program calls the Mp3ToWav method inside of the Convert class
                 * This method will need two parameters
                 *  - The path to the file to be converted
                 *  - The path where the converted path will be
                 */

                Convert.Mp3ToWav(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.wav"));

                // Convert the color of the text on the console to green and the let the user know where the file has been downloaded

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n{title} downloaded successfully on:\n\t{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
                Console.ResetColor();
            } else {
                // If the video title contains an invalid character the text on the console will turn red and an error message will be displayed

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {video.Title} contains invalid characters, please try again using a new link");
                Console.ResetColor();

                // The Link method will be called so the user can enter a new link

                LinkVerification link = new LinkVerification();
                await LinkVerification.Link();
            }
        }
    }
}
