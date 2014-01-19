using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Would you like to split (0) or recombine (1) the files?");
            string line = Console.ReadLine();

            int choice = Convert.ToInt32(line);

            if(choice == 0)
            {
                System.Console.WriteLine("Split how many ways?");
                string answer = Console.ReadLine();
                int splitWays = Convert.ToInt32(answer);

                SplitFile(splitWays);
            }
            else
            {
                Recombine();
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        public static void SplitFile(int numFiles)
        {
            // Original file that will be split
            string fileToRead = @"C:\Users\Chris\Documents\File Seperator Test\Test File.txt";

            // Convert the file into a byte array
            byte[] byteMe = FileToByteArray(fileToRead);

            // Base name of the new files
            string fileToWrite = @"C:\Users\Chris\Documents\File Seperator Test\Write File ";

            // Each space in the array will be a List of bytes representing a seperated file
            List<byte>[] data = new List<byte>[numFiles];

            for (int i = 0; i < numFiles; i++)
            {
                data[i] = new List<byte>(); // Initialize each space in the array
            }

            int place = 0;  // Keep track of which service we're on

            // Go through the original file byte by byte
            for (int i = 0; i < byteMe.Length; i++)
            {
                data[place].Add(byteMe[i]);

                place++;

                if (place >= numFiles)
                {
                    place = 0;
                }
            }

            // Write the split files
            for (int i = 0; i < numFiles; i++)
            {
                ByteArrayToFile(fileToWrite + i + ".txt", data[i].ToArray());
            }

            // Generate a key to tell the program how to recombine
            string text = fileToWrite + System.Environment.NewLine + numFiles;
            
            System.IO.File.WriteAllText(@"C:\Users\Chris\Documents\File Seperator Test\key.txt", text);

        }

        public static void Recombine()
        {
            int numFiles;
            string storageLoaction;

            // Read the key file
            System.IO.StreamReader file = 
                new System.IO.StreamReader(@"C:\Users\Chris\Documents\File Seperator Test\key.txt");

            storageLoaction = file.ReadLine();
            numFiles = Convert.ToInt32(file.ReadLine());

            file.Close();

            // Read in all of the seperated files one by one
            Queue<byte>[] data = new Queue<byte>[numFiles];

            int totalBytes = 0;

            for(int i = 0; i < numFiles; i++)
            {
                data[i] = new Queue<byte>();

                byte[] hold = FileToByteArray(storageLoaction + i + ".txt");

                totalBytes += hold.Length;

                for(int j = 0; j < hold.Length; j++)
                {
                    data[i].Enqueue(hold[j]);
                }
            }

            // Put the bytes back in the original order
            byte[] originalFile = new byte[totalBytes];

            int place = 0;

            while(data[0].Any())
            {
                for (int i = 0; i < data.Length; i++ )
                {
                    if(data[i].Any())
                    {
                        originalFile[place] = data[i].Dequeue();

                        place++;
                    }
                }
            }

            // Convert the bytes back to a file
            ByteArrayToFile(@"C:\Users\Chris\Documents\File Seperator Test\combined.txt", originalFile);
        }

        // Function to get byte array from a file
        public static byte[] FileToByteArray(string _FileName)
        {
            byte[] _Buffer = null;

            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;

                // read entire file into buffer
                _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                // close file reader
                _FileStream.Close();
                _FileStream.Dispose();
                _BinaryReader.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            return _Buffer;
        }

        // Function to save byte array to a file
        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }
    }
}
