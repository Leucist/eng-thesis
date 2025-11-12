namespace LevelEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Level Editor...");
            
            try
            {
                var app = new LevelEditorApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}