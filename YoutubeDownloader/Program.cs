// All of the files in this project will have a great amount of comments so that everyone that reads this code can understand it

using System;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MusicPlayer;
using CliWrap.EventStream;
using NAudio.Wave;

namespace YoutubeDownLoader;

public class Program
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

            LinkVerification link = new LinkVerification();
            await LinkVerification.Link();
        } else
        {
            // If the input of the user is not 1 nor 2 the program will show an error message and restart the program for the user  to make a new selection

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{key} is not a valid input, try again.");
            Program main = new Program();
            await Program.Main();
            Console.ResetColor();
        }
    }
}
