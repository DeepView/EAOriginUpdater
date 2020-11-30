using System;
using System.IO;
using System.Runtime.InteropServices;
namespace EAOriginUpdater
{
   public class Program
   {
      public static void Main(string[] args)
      {
         Uri downlaodUri = GetDownloadUri();
         if (downlaodUri == null) Console.WriteLine("INFO : EA Origin do not update or not found download source!");
         else
         {
            Console.WriteLine("INFO : Would you like to download it here or use a third-party tool?");
            Console.WriteLine("[ OPTION ] now : Download now within the application.");
            Console.WriteLine("[ OPTION ] later : Download later using a third-party application.");
            string options = Console.ReadLine();
            switch (options.ToUpper())
            {
               case "NOW":
                  Console.WriteLine("INFO : Downloading...");
                  _ = new Downloader(downlaodUri.ToString(), SavedPath);
                  bool isComplete = File.Exists(SavedPath);
                  Console.WriteLine($"INFO : Download {(isComplete ? "completed" : "failed")}...");
                  break;
               case "LATER":
                  SetText(downlaodUri.ToString());
                  Console.WriteLine("INFO : The download address of EA Origin update package has been copied to the Windows Clipboard!");
                  break;
               default:
                  Console.WriteLine("WARNNING : Please enter the correct options to perform the operation of the application!");
                  break;
            }
         }
         Console.WriteLine(DateTime.Now.ToString());
         Console.Write("\nPress any key to exit...");
         Console.ReadKey(false);
      }
      [DllImport("User32")]
      internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
      [DllImport("User32")]
      internal static extern bool CloseClipboard();
      [DllImport("User32")]
      internal static extern bool EmptyClipboard();
      [DllImport("User32", CharSet = CharSet.Unicode)]
      internal static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);
      public static string SavedPath { get; private set; }
      public static void SetText(string text)
      {
         if (!OpenClipboard(IntPtr.Zero))
         {
            SetText(text);
            return;
         }
         EmptyClipboard();
         SetClipboardData(13, Marshal.StringToHGlobalUni(text));
         CloseClipboard();
      }
      public static Uri GetDownloadUri()
      {
         const string downlaodUriSubString = @"https://download.dm.origin.com/origin/live/";
         string systemDisk = Environment.SystemDirectory.Substring(0, 1);
         string originSelfUpdateDirectory = $@"{systemDisk}:\ProgramData\Origin\SelfUpdate";
         string originSelfUpdateDirectoryWithAllUsers = $@"{systemDisk}:\Documents and Settings\All Users\Origin\SelfUpdate";
         if (!Directory.Exists(originSelfUpdateDirectory) && !Directory.Exists(originSelfUpdateDirectoryWithAllUsers))
         {
            string errTips = @"You may not have the EA origin platform installed, or you may not be able to find the configuration directory for this platform.";
            Console.WriteLine($"ERROR : {errTips}\n");
            return null;
         }
         else
         {
            string[] filesCurr = Directory.GetFiles(originSelfUpdateDirectory);
            string[] filesAll = Directory.GetFiles(originSelfUpdateDirectoryWithAllUsers);
            string downlaodUri = downlaodUriSubString;
            bool isFoundedFile = false;
            foreach (string file in filesCurr)
            {
               if (new FileInfo(file).Extension == ".part")
               {
                  downlaodUri += Path.GetFileNameWithoutExtension(file);
                  isFoundedFile = true;
                  SavedPath = $@"{originSelfUpdateDirectory}\{Path.GetFileNameWithoutExtension(file)}";
                  break;
               }
            }
            if (!isFoundedFile)
            {
               foreach (string file in filesAll)
               {
                  if (new FileInfo(file).Extension == ".part")
                  {
                     downlaodUri += Path.GetFileNameWithoutExtension(file);
                     isFoundedFile = true;
                     SavedPath = $@"{originSelfUpdateDirectoryWithAllUsers}\{Path.GetFileNameWithoutExtension(file)}";
                     break;
                  }
               }
            }
            if (isFoundedFile) return new Uri(downlaodUri);
            else return null;
         }
      }
   }
}
