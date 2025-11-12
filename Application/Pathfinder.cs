namespace Application
{
    public static class Pathfinder
    {
        // To cache the Solution path // todo? may be expanded to all the directory paths 
        private static string? _solutionDirectory;

        public static string FindSolutionDirectory()
        {
            // Current directory of the executing process
            string startDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var directory = new DirectoryInfo(startDirectory);

            // Go up until we find solution file (.sln)
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Application.sln")))
            {
                directory = directory.Parent;
            }

            // Return path if it's found
            if (directory != null)
            {
                return directory.FullName;
            }

            // Otherwise exception is thrown
            throw new DirectoryNotFoundException("Solution root was not found.");
        }

        public static string GetSolutionDirectory() {
            _solutionDirectory ??= FindSolutionDirectory();
            return _solutionDirectory;
        }

        public static string GetSourceFolder() => Path.Combine(GetSolutionDirectory(), "Application", "Source");

        public static string GetAudioFolder() => Path.Combine(GetSourceFolder(), "Audio");

        public static string GetGraphicsFolder() => Path.Combine(GetSourceFolder(), "Graphics");

        public static string GetTexturesFolder()        => Path.Combine(GetGraphicsFolder(), "Textures");
        public static string GetBackgroundDirectory()   => Path.Combine(GetGraphicsFolder(), "Backgrounds");

        public static string GetFullTextureFilePath(string pathToImage) => Path.Combine(GetTexturesFolder(), pathToImage);


        /// <summary>
        /// Returns a list of all files inside the specified directory and its subdirectories.
        /// </summary>
        public static List<string> GetAllFilesRecursive(string rootFolder)
        {
            if (!Directory.Exists(rootFolder))
                throw new DirectoryNotFoundException($"Directory not found: {rootFolder}");

            return new List<string>(Directory.GetFiles(rootFolder, "*", SearchOption.AllDirectories));
        }

        /// <summary>
        /// Returns all file paths under the Graphics folder (including subdirectories).
        /// </summary>
        public static List<string> GetAllTexturePaths() => GetAllFilesRecursive(GetGraphicsFolder());

        private static string GetWorldPath(string name, string folderName)
        {
            string folder   = Path.Combine(GetSourceFolder(), folderName);
            string filePath = Path.Combine(folder, $"{name}.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"World file not found: {filePath}");

            return filePath;
        }

        public static string GetWorldTemplatePath(string name) => GetWorldPath(name, "Worlds");
        public static string GetWorldSavePath(string name) => GetWorldPath(name, "Saves");

        // * Prefabs *
        private static string GetPrefabsFolder() => Path.Combine(GetSourceFolder(), "Prefabs");

        public static string GetTilePrefabsDirectory()          => Path.Combine(GetPrefabsFolder(), "Tiles");
        public static string GetCharacterPrefabsDirectory()     => Path.Combine(GetPrefabsFolder(), "Characters");

        public static string GetTilePrefabPath(string name) => Path.Combine(GetTilePrefabsDirectory(), $"{name}.json");
        public static string GetCharacterPrefabPath(string name) => Path.Combine(GetCharacterPrefabsDirectory(), $"{name}.json");
    }
}