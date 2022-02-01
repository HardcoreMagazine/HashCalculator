using System.Security.Cryptography;
using System.Text;

class Program
{
    static string? GetUserInput()
    {
        Console.Write(">> ");
        return Console.ReadLine();
    }

    //some sources say you shouldnt return FileStream and use byte[] instead -- I dont see why not
    //might reconsider later
    static FileStream? CreateFileStream(string path)
    {
        FileStream? fs = null;
        try
        {
            fs = File.OpenRead(path);
        }
        catch
        {
            Console.WriteLine("@> An error occurred during file processing.");
        }
        return fs;
    }

    static List<byte[]> CalculateHashes(FileStream bytes)
    {
        List<byte[]> hashList = new List<byte[]>();
        byte[] temp = new byte[] {};
        byte[] defaultErrorMessage = Encoding.ASCII.GetBytes("calculation error x_x");
        // --------- md5 ---------
        try
        {
            using (var md5 = MD5.Create())
            {
                temp = md5.ComputeHash(bytes);
            }
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        // --------- sha256 ---------
        try
        {
            using (var sha256 = SHA256.Create())
            {
                temp = sha256.ComputeHash(bytes);
            }
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        GC.Collect(); //forces use of garbage collector
                      //SHA256 strings may be very large, up to few TBs size
        // --------- sha1 ---------
        try
        {
            using (var sha1 = SHA1.Create())
            {
                temp = sha1.ComputeHash(bytes);
            }
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        return hashList;
    }

    //print byte array as string
    static void PrintByteArray(byte[] array)
    {
        /*
        for (int i = 0; i < array.Length; i++)
        {
            Console.Write($"{array[i]:X2}");
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
        */
        Console.WriteLine(BitConverter.ToString(array).Replace("-", "").ToLower());
        //0cc175b9c0f1b6a831c399e269772661 -- md5
        //86f7e437faa5a7fce15d1ddcb9eaeaea377667b8 -- sha1
        //ca978112ca1bbdcafac231b39a23dc4da786eff8147c4e72b9807785afee48bb -- sha256
    }

    //simple loop that runs on background/2nd thread
    //it'll close application on Escape/ESC press
    // - unless user presses other button OR window running for the first time
    // --- to fix ---
    static void ExitOnPress()
    {
        while (true)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
            else { }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine(
            "#############################################################################\n" +
            "# Universal hash calculator by HardcoreMagazine                             #\n" +
            "# Official Github page: TODO                                                #\n" +
            "# To get hash-sum of file simply paste full path in the console             #\n" +
            "# Press ESC to exit program                                                 #\n" +
            "#############################################################################\n"
            );
        //simple loop that keeps program running unless user closes it OR presses [button]
        //"ESC" in this case
        Task task = Task.Run(ExitOnPress);
        while (true)
        {
            string? path = GetUserInput();
            if (path != null)
            {
                FileStream? fs = CreateFileStream(path);
                if (fs != null)
                {
                    List<byte[]> hashes = CalculateHashes(fs);
                    List<string> hashNames = new() { "@> MD-5: ", "@> SHA-256: ", "@> SHA-1: " };
                    for (int i = 0; (i < hashes.Count); i++)
                    {
                        Console.Write(hashNames[i]);
                        PrintByteArray(hashes[i]);
                    }
                }
            }
        }
    }
}
