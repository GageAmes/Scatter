using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterTest
{
    class Program
    {
        private const int MaxPatternLengthMultiplier = 5;       // Multiplies the number of splits to determine the max pattern length

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
            string fileToRead = "Test File.txt";

            // Convert the file into a byte array
            byte[] byteMe = FileToByteArray(fileToRead);

            // Base name of the new files
            string fileToWrite = "Write File ";

            // Pattern for splitting the files
            int[] pattern = GenerateSplitPattern(numFiles);

            // Each space in the array will be a List of bytes representing a seperated file
            List<byte>[] data = new List<byte>[numFiles];

            for (int i = 0; i < numFiles; i++)
            {
                data[i] = new List<byte>(); // Initialize each space in the array
            }

            // Go through the original file byte by byte
            for (int i = 0; i < byteMe.Length; i++)
            {
                int serviceIndex = pattern[i % pattern.Length];
                data[serviceIndex].Add(byteMe[i]);
            }

            // Write the split files
            for (int i = 0; i < numFiles; i++)
            {
                ByteArrayToFile(fileToWrite + i + ".txt", data[i].ToArray());
            }

            // Generate a key to tell the program how to recombine
            WriteKeyFile("key.txt", fileToWrite, numFiles, pattern);
        }

        public static void Recombine()
        {
            int numFiles;
            string storageLoaction;
            int[] pattern;

            // Read the key file
            ReadKeyFile("key.txt", out storageLoaction, out numFiles, out pattern);

            // Read in all of the seperated files one by one
            Queue<byte>[] data = new Queue<byte>[numFiles];

            int totalBytes = 0;

            for(int i = 0; i < numFiles; i++)
            {
                byte[] fileBytes = FileToByteArray(storageLoaction + i + ".txt");

                totalBytes += fileBytes.Length;

                data[i] = new Queue<byte>(fileBytes);
            }

            // Put the bytes back in the original order
            byte[] originalFile = new byte[totalBytes];

            for (int i = 0; i < totalBytes; i++)
            {
                int serviceIndex = pattern[i % pattern.Length];
                originalFile[i] = data[serviceIndex].Dequeue();
            }

            // Convert the bytes back to a file
            ByteArrayToFile("combined.txt", originalFile);
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

        private static int[] GenerateSplitPattern(int serviceCount)
        {
            Random rand = new Random();
            int patternLength = rand.Next(serviceCount, serviceCount * MaxPatternLengthMultiplier);
            int[] pattern = new int[patternLength];

            for (int i = 0; i < patternLength; i++)
            {
                pattern[i] = rand.Next(serviceCount);
            }

            return pattern;
        }

        private static void WriteKeyFile(string keyFileName, string splitFileNameBase, int numServices, int[] pattern)
        {
            StringBuilder text = new StringBuilder();

            text.AppendLine(splitFileNameBase);
            text.AppendLine(numServices.ToString());

            foreach (int i in pattern)
            {
                text.AppendLine(i.ToString());
            }

            System.IO.File.WriteAllText(keyFileName, text.ToString());
        }

        private static void ReadKeyFile(string keyFileName, out string splitFileNameBase, out int numServices, out int[] pattern)
        {
            System.IO.StreamReader file = new System.IO.StreamReader("key.txt");
            string[] patternStrings;
            List<int> patternList = new List<int>();

            splitFileNameBase = file.ReadLine();
            numServices = Convert.ToInt32(file.ReadLine());
            patternStrings = file.ReadToEnd().Split(Environment.NewLine.ToArray());

            file.Close();

            // Recover the pattern from the file
            foreach (string s in patternStrings)
            {
                int index;

                if (int.TryParse(s, out index))
                {
                    patternList.Add(index);
                }
            }

            pattern = patternList.ToArray();
        }
    }
}
