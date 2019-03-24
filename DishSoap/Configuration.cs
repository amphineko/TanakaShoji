using System.Configuration;
using NLog;

namespace DishSoap
{
    internal class Configuration
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static Configuration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Logger.Info($"Using config {config.FilePath}");
        }

        public static bool IsBot => ConfigurationManager.AppSettings["IsBot"] == "yes";

        public static string Token => ConfigurationManager.AppSettings["Token"];

        public static string TokenType => ConfigurationManager.AppSettings["TokenType"];
    }
}