using System.Security.Cryptography;
using System.Text;

class Program
{
    static string? ReadUserInput()
    {
        Console.Write(">> ");
        return Console.ReadLine();
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
                Console.WriteLine("@> An error occurred during file processing");
            }
        }
        else
        {
            Console.WriteLine("@> Specified file does not exist or could not be found.");
        }
        return fs;
    }

    static List<byte[]> CalculateHashes(FileStream bytes)
    {
        List<byte[]> hashList = new List<byte[]>();
        byte[] temp = new byte[] {};
        byte[] defaultErrorMessage = Encoding.ASCII.GetBytes("calculation error x_x");
        //all 'calculators' locked inside try-catch block
        //due to present garbage collector: it may accidently delete *unused* filestream
        // --------- md5 ---------
        try
        {
            using var md5 = MD5.Create();
            temp = md5.ComputeHash(bytes);
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        // --------- sha1 ---------
        try
        {
            using var sha1 = SHA1.Create();
            temp = sha1.ComputeHash(bytes);
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        // --------- sha256 ---------
        try
        {
            using var sha256 = SHA256.Create();
            temp = sha256.ComputeHash(bytes);
        }
        catch { temp = defaultErrorMessage; }
        finally { hashList.Add(temp); }
        return hashList;
    }

    //Key press handler -- works on parallel thread:
    //ESC - exit
    //expandable
    static void ProcessKeyPress()
    {
        while (true)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                Environment.Exit(0);
            }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine(
            "#############################################################################\n" +
            "# Universal hash calculator by HardcoreMagazine                             #\n" +
            "# Official Github page: https://github.com/HardcoreMagazine/HashCalculator  #\n" +
            "# To get hash-sum of file simply paste full path in the console             #\n" +
            "# Press ESC / Enter+ESC to exit program                                     #\n" +
            "#############################################################################\n");
        Task task = Task.Run(ProcessKeyPress); //V1 - not sure if anything changes
        while (true)
        {
            //Task task = Task.Run(ProcessKeyPress); //V2 - not sure if anything changes
            string? path = ReadUserInput();
            if (path != null)
            {
                path = path.Replace('/', '\\').Replace("\"", "");
                //"\"" may appear on Win 11 machines where "Copy as path" option is present
                FileStream? fs = CreateFileStream(path);
                if (fs != null)
                {
                    List<byte[]> hashes = CalculateHashes(fs);
                    List<string> hashNames = new() { "MD5", "SHA-1", "SHA-256" };
                    for (int i = 0; i < hashes.Count; i++)
                    {
                        Console.WriteLine("@> " + hashNames[i] + ": " +
                            BitConverter.ToString(hashes[i]).Replace("-", "").ToLower());
                    }
                    fs.Close();
                    //MD5 hash should match other sources; however, SHA-1 and SHA-256 is
                    //not going to. The issue is based on encoding differences;
                    //also WEB pages might add extra bits to file after upload
                    //which will change outcome.
                }
            }
        }
    }
}
