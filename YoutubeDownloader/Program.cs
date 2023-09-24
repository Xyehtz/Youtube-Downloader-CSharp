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

            // Here, basic information like the title, author and duration will be saved on three diferent variables

            string title = obtainVideo.Title;
            var author = obtainVideo.Author;
            var duration = obtainVideo.Duration;

            // Show the information to the user, to confirm if the video is indeed the one selected by the user

            Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}");

            /* The program calls the download method to start the download of the video
             * For this to properlly work the program will give the youtube variable and the youtubeLink and title strings
             */

            Download download = new Download();
            await Download.DownloadMethod(youtube, youtubeLink, title);

            PlayLoop player = new PlayLoop();
            PlayLoop.Play();

        } else if (youtubeLink.Contains("https://www.youtube.com/playlist?list=")) {
            var youtube = new YoutubeClient();

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
        // Obtain the new

        var video = await youtube.Videos.GetAsync(url);

        if (!Verification.ContainsInvalidCharacters(video.Title))
        {
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"));

            Convert.Mp3ToWav(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.wav"));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{title} downloaded succesfully on:\n\t{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
            Console.ResetColor();
        } else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {video.Title} contains invalid characters, please try again using a new link");
            Console.ResetColor();

            Program.Methods link = new Program.Methods();
            await Program.Methods.Link();
        }
    }
}

class PlaylistDownload
{
    public static async Task PlaylistDownloadMethod(YoutubeClient youtube, string url)
    {
        var playlist = youtube.Playlists.GetAsync(url);

        await foreach ( var video in youtube.Playlists.GetVideosAsync(url))
        {
            var title = video.Title;
            var author = video.Author;
            var duration = video.Duration;

            Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}\n");
        }

        int i = 0;

        await foreach (var video in youtube.Playlists.GetVideosAsync(url))
        { 
            if (!Verification.ContainsInvalidCharacters(video.Title))
            {
                var title = video.Title;

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"));

                Convert.Mp3ToWav(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.wav"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Succesfuly downloaded {video.Title}. Total downloads: {i}");

                i++;
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Skipping {video.Title} due to it contining a special character, no windows file can contain special characters\n");
                Console.ResetColor();
                continue;
            }
            Console.ResetColor();
        }

        Console.WriteLine($"{i} files have been downloaded on:\n\t{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
    }
}

class Verification
{
    public static bool ContainsInvalidCharacters(string title)
    {
        char[] invalidCharacters = Path.GetInvalidFileNameChars();
        return title.IndexOfAny(invalidCharacters) != -1;
    }
}

class Convert
{
    public static void Mp3ToWav(string mp3File, string outputFile)
    {
        using (MediaFoundationReader reader = new MediaFoundationReader(mp3File))
        {
            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
            {
                WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                File.Delete(mp3File);
            }
        }
    }
}

class PlayLoop
{
    public static void Play()
    {
        while (true)
        {
            Console.WriteLine("Do you want to play the songs inside your folder?\nType 1 to play\t\t Type 2 to exit");
            string number = Console.ReadLine();

            if (number == "1")
            {
                MusicPlayer.Program.Player();
            }
            else if (number == "2")
            {
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Error, please follow the instructions");
            }
        }
    }
}