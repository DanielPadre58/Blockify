namespace Blockify.Infrastructure.Tools {
    public static class SqlScriptsHelper
    {
        public static string GetMigrationPath(string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Blockify", "Migrations", fileName);

        public static string GetQueryPath(string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Blockify", "Queries", fileName);

        public static string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("SQL migration file not found.", path);
            }

            return File.ReadAllText(path);
        }

        public static int GetMigrationVersion(string fileName) =>
    int.Parse(Path.GetFileName(fileName).Split('_').First());
    }
}