using System;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using YoutubeExplode.Converter;

namespace Program;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Please enter the YouTube link below");
        string youtubeLink = Console.ReadLine();

        var youtube = new YoutubeClient();

        var obtainVideo = await youtube.Videos.GetAsync(youtubeLink);

        string title;

        if (youtubeLink.Contains("https://www.youtube.com/watch?v="))
        {
            title = obtainVideo.Title;
            var author = obtainVideo.Author;
            var duration = obtainVideo.Duration;

            Console.WriteLine($"Title:\t\t{title}\nAuthor:\t\t{author}\nDuration:\t{duration}");
        }
        else
        {
            Console.WriteLine("Sorry, but it seems to be a problem with the link you've provided, try to add a new link");
        }



        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(youtubeLink);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        //string savePath = "C:\\downloadtest";

        //using (var httpClient = new HttpClient())
        //{
        //    var videoStream = await httpClient.GetStreamAsync(streamInfo.Url);
        //    using (var fileStream = File.OpenWrite(savePath))
        //    {
        //        await videoStream.CopyToAsync(fileStream);
        //    }
        //}

        var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
        Console.WriteLine(stream);

        string fileName = obtainVideo.Title;

        await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic), $"{fileName}.mp3"));

        //string origin = "YoutubeDownloader\\bin\\Debug\\net6.0\\video.webm";
        //string destiny = @"G:\My Drive";

        //try
        //{
        //    File.Move(origin, destiny);
        //    Console.WriteLine("Exito");
        //} catch (IOException e) {
        //    Console.WriteLine(e);
        //}

    }
}