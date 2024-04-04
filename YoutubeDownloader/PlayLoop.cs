using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownLoader {
    class PlayLoop {
        public static void Play() {
            // This while loop will help the program to work if the user enters an invalid option

            while (true) {
                // This will display the options on the console and wait for the user to enter an option

                Console.WriteLine("Do you want to play the songs inside your folder?\nType 1 to play\t\t Type 2 to exit");
                string number = Console.ReadLine();

                // If the user enters 1 the music will start to play

                if (number == "1") {
                    MusicPlayer.Program.Player();
                } else if (number == "2")

                  // If the user enters 2 the program will close

                  {
                    Environment.Exit(1);
                } else

                  // If the user enters an invalid option the method will restart

                  {
                    Console.WriteLine("Error, please follow the instructions");
                }
            }
        }
    }
}
