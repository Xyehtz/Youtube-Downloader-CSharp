using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownLoader {
    class Verification {
        // This method will obtain the youtube title of the video

        public static bool ContainsInvalidCharacters(string title) {
            // Inside of this method a list will be created containing all of the invalid characters on windows

            char[] invalidCharacters = Path.GetInvalidFileNameChars();

            /* Verify if an invalid character is present inside of the title
             * If the title contains a valid character it will return true
             */

            return title.IndexOfAny(invalidCharacters) != -1;
        }
    }
}
