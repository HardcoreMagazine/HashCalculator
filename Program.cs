using System.Security.Cryptography;

class Program
{
    static string? ReadUserInput()
    {
        Console.Write(">> ");
        string? input = Console.ReadLine();
        if (input == "q" || input == "quit")
        {
            Environment.Exit(0);
            return null;
        }
        else
        {
            return input;
        }
    }

    //Creates byte-stream out of specified file
    //some sources say you shouldnt return FileStream and use byte[] instead -- I don't see reason why
    //might reconsider later
    static FileStream? CreateFileStream(string path)
    {
        FileStream? fs = null;
        if (File.Exists(path))
        {
            try
            {
                fs = File.OpenRead(path);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("@> An error occurred during file processing");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("@> Specified file does not exist or could not be found.");
            Console.ResetColor();
        }
        return fs;
    }

    static List<byte[]> CalculateHashes(FileStream fs)
    {
        List<byte[]> hashList = new List<byte[]>();
        // --------- md5 ---------
        hashList.Add(MD5.Create().ComputeHash(fs));
        fs.Seek(0, SeekOrigin.Begin);
        //reset ~StreamReader pointer to origin point
        //is mandatory -- otherwise you'll get equal
        //SHA1/SHA256 hashes for 2 different files
        // --------- sha1 ---------
        hashList.Add(SHA1.Create().ComputeHash(fs));
        fs.Seek(0, SeekOrigin.Begin);
        // --------- sha256 ---------
        hashList.Add(SHA256.Create().ComputeHash(fs));
        return hashList;
    }

    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            "############################################################################\n" +
            "# Universal hash calculator by HardcoreMagazine                            #\n" +
            "# Official Github page: https://github.com/HardcoreMagazine/HashCalculator #\n" +
            "# To get hash-sum of file simply paste full path in the console            #\n" +
            "# Type \"q\" or \"quit\" to exit program                                   #\n" +
            "############################################################################\n");
        Console.ResetColor();
        while (true)
        {
            string? path = ReadUserInput();
            if (path != null) 
            {
                path = path.Replace('/', '\\').Replace("\"", "");
                //"\"" may appear on Win 11 machines where "Copy as path" option is present
                FileStream? fs = CreateFileStream(path);
                if (fs != null)
                {
                    List<byte[]> hashes = CalculateHashes(fs);
                    fs.Close();
                    List<string> hashNames = new() { "MD5", "SHA1", "SHA256" };
                    for (int i = 0; i < hashes.Count; i++)
                    {
                        if (hashes[i] != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("@> " + hashNames[i] + ": " + 
                                BitConverter.ToString(hashes[i]).Replace("-", "").ToLower());
                            Console.ResetColor();
                        }

                        else
                        {
                            Console.ForegroundColor= ConsoleColor.DarkRed;
                            Console.WriteLine("@> " + hashNames[i] + ": " + "calculation error x_x");
                            Console.ResetColor();
                        }
                    }
                    //MD5 hash should match other sources; however, SHA1 and SHA256
                    //may be different. The issue is based on encoding differences;
                    //also some WEB pages might add extra bits to file header after upload
                    //which will change outcome.
                }
            }
            else 
            { 
                Console.WriteLine("Incorrect/empty path entered, retry.");
            }
        }
    }
}
