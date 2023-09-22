using System;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MusicPlayer;
using CliWrap.EventStream;

namespace Program;

public class Methods
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Paste the URL of the video to download");
        string youtubeLink = Console.ReadLine();

        if (youtubeLink.Contains("https://www.youtube.com/watch?v="))
        {
            var youtube = new YoutubeClient();
            var obtainVideo = await youtube.Videos.GetAsync(youtubeLink);

            string title = obtainVideo.Title;
            var author = obtainVideo.Author;
            var duration = obtainVideo.Duration;

            Console.WriteLine($"\nTitle:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}");

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
        var video = await youtube.Videos.GetAsync(url);

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

        await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"));

        Console.WriteLine($"\n{title} downloaded succesfully on:\t{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
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

        int i = 1;

        await foreach (var video in youtube.Playlists.GetVideosAsync(url))
        {
            if (!Verification.ContainsInvalidCharacters(video.Title))
            {
                var title = video.Title;

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{title}.mp3"));

                Console.WriteLine($"Succesfuly downloaded {title}. Total downloads: {i}");

                i++;
            } else
            {
                Console.WriteLine($"Skipping {video.Title} due to it contining a special character, no windows file can contain special characters\n");
                continue;
            }
            
        }

        Console.WriteLine($"{i} files have been downloaded on {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))}");
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