// All of the files in this project will have a great amount of comments so that everyone that reads this code can understand it

using System;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MusicPlayer;
using CliWrap.EventStream;
using NAudio.Wave;

namespace Program;

public class Methods
{
    // This is the main method of this project, the project will start from this method

    public static async Task Main()
    {
        // The first thing that will appear on the console is this message, letting the user know what to do

        Console.WriteLine("Type:\n1. To play music\t\t2. To download and listen music");

        // key will save the selection done by the user

        string key = Console.ReadLine();

        // This if statement will verify the input of the user and depending on it will do something

        if (key == "1")
        {
            // If the user types 1 the program will run the Play method, this will end up playing the music

            PlayLoop player = new PlayLoop();
            PlayLoop.Play();
        } else if (key == "2")
        {
            // When the user types 2 the program will call the Link method and start the process of downloading music

            Program.Methods link = new Program.Methods();
            await Program.Methods.Link();
        } else
        {
            // If the input of the user is not 1 nor 2 the program will show an error message and restart the program for the user  to make a new selection

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{key} is not a valid input, try again.");
            Program.Methods main = new Program.Methods();
            await Program.Methods.Main();
            Console.ResetColor();
        }
    }
    public static async Task Link()
    {
        // This method will show a new message to the user to paste the url of the video or playlist to download

        Console.WriteLine("Paste the URL of the video or playlist to download");

        // youtubeLink will save the link given by the user

        string youtubeLink = Console.ReadLine();

        /* This if statement will do the following
         * Obtain the youtube link gave by the user
         * Check if the link contains either the base link for a video or a playlist
         */

        if (youtubeLink.Contains("https://www.youtube.com/watch?v="))
        {
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
        }
        else
        {
            Console.WriteLine("Sorry, but it seems to be a problem with the link you've provided, try to add a new link");
        }
    }

}

class Download
{
    public static async Task DownloadMethod(YoutubeClient youtube, string url, string title)
    {
        // Obtain the variables and obtain again the information of the youtube video

        var video = await youtube.Videos.GetAsync(url);

        // Verify if the video title contains any type of invalid characters

        if (!Verification.ContainsInvalidCharacters(video.Title))
        {
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
        } else
        {
            // If the video title contains an invalid character the text on the console will turn red and an error message will be displayed

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {video.Title} contains invalid characters, please try again using a new link");
            Console.ResetColor();

            // The Link method will be called so the user can enter a new link

            Program.Methods link = new Program.Methods();
            await Program.Methods.Link();
        }
    }
}

class PlaylistDownload
{
    public static async Task PlaylistDownloadMethod(YoutubeClient youtube, string url)
    {
        // The method will start by obtaining the information inside of the playlist

        var playlist = youtube.Playlists.GetAsync(url);

        // After obtaining the information of the playlist the method will go through all the videos inside the playlist

        await foreach ( var video in youtube.Playlists.GetVideosAsync(url))
        {
            // For every video inside the playlist the program will obtain the title, author and the duration

            var title = video.Title;
            var author = video.Author;
            var duration = video.Duration;

            // The information will be displayed to the user to check if everything is ok

            Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}\n");
        }

        // Declare the download counter and let the user know that the download will start

        int i = 0;
        Console.WriteLine("The download will start in a few seconds");

        // Go through every video in the playlist

        await foreach (var video in youtube.Playlists.GetVideosAsync(url))
        { 
            // Check if the videos have an invalid character on it's title

            if (!Verification.ContainsInvalidCharacters(video.Title))
            {
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
            } else
            {
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

class Verification
{
    // This method will obtain the youtube title of the video

    public static bool ContainsInvalidCharacters(string title)
    {
        // Inside of this method a list will be created containing all of the invalid characters on windows

        char[] invalidCharacters = Path.GetInvalidFileNameChars();

        /* Verify if an invalid character is present inside of the title
         * If the title contains a valid character it will return true
         */

        return title.IndexOfAny(invalidCharacters) != -1;
    }
}

class Convert
{
    /* This method will receive two strings
     * - The first string will be the path to the mp3 file
     * - The second string will be the path were the wav file will be located
     */

    public static void Mp3ToWav(string mp3File, string outputFile)
    {
        // Read the mp3 file using MediaFoundatioReader

        using (MediaFoundationReader reader = new MediaFoundationReader(mp3File))
        {
            // Using WaveStream we will use the reader variable to obtain the PcmStream of the mp3

            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
            {
                // Using WaveFileWriter the program will write the Wav file using PcmStream and putting it on the output file

                WaveFileWriter.CreateWaveFile(outputFile, pcmStream);

                // When the conversion is finished delete the original mp3 file to save disk storage

                File.Delete(mp3File);
            }
        }
    }
}

class PlayLoop
{
    public static void Play()
    {
        // This while loop will help the program to work if the user enters an invalid option

        while (true)
        {
            // This will display the options on the console and wait for the user to enter an option

            Console.WriteLine("Do you want to play the songs inside your folder?\nType 1 to play\t\t Type 2 to exit");
            string number = Console.ReadLine();

            // If the user enters 1 the music will start to play

            if (number == "1")
            {
                MusicPlayer.Program.Player();
            }
            else if (number == "2")

            // If the user enters 2 the program will close

            {
                Environment.Exit(1);
            }
            else

            // If the user enters an invalid option the method will restart

            {
                Console.WriteLine("Error, please follow the instructions");
            }
        }
    }
}