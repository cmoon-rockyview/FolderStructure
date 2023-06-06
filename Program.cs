using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FolderPermission
{
    class Program
    {
        static void Main(string[] args)
        {
            string DrivePath = @"D:\G_Drive\";
            int startTier = 1;
            if (args.Length > 0)
            {
                DrivePath = args[0];
                startTier = int.Parse(args[1]);
            }


            //CopyDirectoryStructure(@"G:\", @"D:\G_Drive\");

            TraverseDirectory(DrivePath, startTier);

            Console.WriteLine("Done");
        }

        public static void CopyDirectoryStructure(string sourceDir, string destDir)
        {
            // Check if the source directory exists
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }

            // Create destination directory if it doesn't exist
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
                Console.WriteLine("destDir is " + destDir);
            }

            
        }

        public static void TraverseDirectory(string directoryPath, int baseTier)
        {
            // Make sure the directory exists before trying to process it
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            Console.WriteLine($"Directory: {directoryPath}, Tier: {baseTier}");

            // Get the subdirectories for the specified directory.
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            foreach (string subdir in subdirectories)
            {
                TraverseDirectory(subdir, baseTier + 1);

                SetPermissions(subdir, baseTier + 1);
            }
        }

        public static void SetPermissions(string directoryPath, int tier)
        {
       
            // Fetch the current security settings for the given directory
            var directoryInfo = new DirectoryInfo(directoryPath);
            var directorySecurity = directoryInfo.GetAccessControl();


            // If we're at the 4th tier or beyond, we want to allow read/write for all
            if (tier >= 4)
            {
                var everyoneGroup = new NTAccount("IT_GIS");
                var everyoneAccessRule = new FileSystemAccessRule(
                    everyoneGroup,
                    FileSystemRights.Modify | FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.ListDirectory,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow
                );
                directorySecurity.SetAccessRule(everyoneAccessRule);
            }

            // Apply the new security settings
            directoryInfo.SetAccessControl(directorySecurity);
        }


    }
}
