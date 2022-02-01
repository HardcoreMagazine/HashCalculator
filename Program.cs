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
        return hashList;
    }

    //simple loop that runs on background/2nd thread
    //it'll close application on Escape/ESC press
    // - unless user presses other button OR window running for the first time
    // --- to fix ---
    static void ExitOnPress()
    {
        while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        Environment.Exit(0);
    }

    static void Main(string[] args)
    {
        Console.WriteLine(
            "#############################################################################\n" +
            "# Universal hash calculator by HardcoreMagazine                             #\n" +
            "# Official Github page: TODO                                                #\n" +
            "# To get hash-sum of file simply paste full path in the console             #\n" +
            "# Press ESC / Enter+ESC to exit program                                     #\n" +
            "#############################################################################\n"
            );
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
                    List<string> hashNames = new() { "@> MD-5: ", "@> SHA-1: ", "@> SHA-256: " };
                    for (int i = 0; (i < hashes.Count); i++)
                    {
                        Console.WriteLine(hashNames[i] +
                            BitConverter.ToString(hashes[i]).Replace("-", "").ToLower());
                    }
                    //MD5 hash should match other sources; however, SHA1 and SHA256 is
                    //not going to. The issue is based on encoding differences;
                    //also WEB pages might add extra header(s) to uploaded file
                    //which will change sha1/sha256 hashes.
                }
            }
        }
    }
}
