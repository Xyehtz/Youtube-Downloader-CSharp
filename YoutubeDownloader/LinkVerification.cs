using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace YoutubeDownLoader {
    public class  LinkVerification {
        public static async Task Link() {
            // This method will show a new message to the user to paste the url of the video or playlist to download

            Console.WriteLine("Paste the URL of the video or playlist to download");

            // youtubeLink will save the link given by the user

            string youtubeLink = Console.ReadLine();

            /* This if statement will do the following
             * Obtain the youtube link gave by the user
             * Check if the link contains either the base link for a video or a playlist
             */

            if (youtubeLink.Contains("https://www.youtube.com/watch?v=")) {
                // After the link has been verified the program will start the Youtube client to obtain the basic information of the video

                var youtube = new YoutubeClient();
                var obtainVideo = await youtube.Videos.GetAsync(youtubeLink);

                // Here, basic information like the title, author and duration will be saved on three different variables

                string title = obtainVideo.Title;
                var author = obtainVideo.Author;
                var duration = obtainVideo.Duration;

                // Show the information to the user, to confirm if the video is indeed the one selected by the user

                Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}");

                /* The program calls the download method to start the download of the video
                 * For this to properly work the program will give the youtube variable and the youtubeLink and title strings
                 */

                Download download = new Download();
                await Download.DownloadMethod(youtube, youtubeLink, title);

                // After downloading the files the program will call the Play method that will start playing the songs

                PlayLoop player = new PlayLoop();
                PlayLoop.Play();

            } else if (youtubeLink.Contains("https://www.youtube.com/playlist?list=")) {

                // If the link is from a playlist the youtube client will be created

                var youtube = new YoutubeClient();

                /* After creating the YouTube client the PlaylistDownloadMethod inside of the PlaylistDownload will be called to start the download
                 * - This method will receive the youtube client and the youtube link
                 */

                PlaylistDownload download = new PlaylistDownload();
                await PlaylistDownload.PlaylistDownloadMethod(youtube, youtubeLink);

                PlayLoop player = new PlayLoop();
                PlayLoop.Play();
            } else {
                Console.WriteLine("Sorry, but it seems to be a problem with the link you've provided, try to add a new link");
            }
        }
    }
}
