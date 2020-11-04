using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpConfig;

namespace resinBot.Core.Config
{
    public static class Config
    {
        /// <summary>
        /// The config file
        /// </summary>
        private static Configuration Configuration = null;

        /// <summary>
        /// The section of the config containing general config
        /// </summary>
        private static Section GeneralSection = null;

        /// <summary>
        /// Main path
        /// </summary>
        private static string ConfigPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "config.txt");
            }
        }

        /// <summary>
        /// Static constructor to handle config initialization
        /// </summary>
        static Config()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    File.WriteAllText(ConfigPath, "");

                Configuration = Configuration.LoadFromFile(ConfigPath);

                // config still not found?
                if (Configuration == null)
                    throw new System.Exception("Failed to locate log file.");

                // get the general section
                GeneralSection = Configuration["General"];

                // load in default values if not found
                CreateEntryIfNull(GeneralSection, "botToken", "token-code-here", EntryType.STRING, "Bot token to connect to.");
                CreateEntryIfNull(GeneralSection, "cmdPrefix", ".", EntryType.STRING, "Command Prefix.");
                CreateEntryIfNull(GeneralSection, "maxResin", 120, EntryType.INT, "Maximum resin allowed by the game.");
                CreateEntryIfNull(GeneralSection, "minuteRefreshRate", 8, EntryType.INT, "How many minutes until 1 resin is regenerated?");

                // save changes
                Configuration.SaveToFile(ConfigPath);
            }
            catch( Exception e )
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// General config values
        /// </summary>
        public static class GENERAL
        {
            public static string BOT_TOKEN
            {
                get
                {
                    return GeneralSection["botToken"].StringValue;
                }
                set
                {
                    GeneralSection["botToken"].SetValue(value);
                    Configuration.SaveToFile(ConfigPath);
                }
            }
            public static string CMD_PREFIX
            {
                get
                {
                    return GeneralSection["cmdPrefix"].StringValue;
                }
                set
                {
                    GeneralSection["cmdPrefix"].SetValue(value);
                    Configuration.SaveToFile(ConfigPath);
                }
            }
            public static int MAX_RESIN
            {
                get
                {
                    return GeneralSection["maxResin"].IntValue;
                }
                set
                {
                    GeneralSection["maxResin"].SetValue(value);
                    Configuration.SaveToFile(ConfigPath);
                }
            }
            public static int REGEN_RATE_MINS
            {
                get
                {
                    return GeneralSection["minuteRefreshRate"].IntValue;
                }
                set
                {
                    GeneralSection["minuteRefreshRate"].SetValue(value);
                    Configuration.SaveToFile(ConfigPath);
                }
            }
        }

        /// <summary>
        /// Create the default config value if not found
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="entryType"></param>
        /// <param name="preComment"></param>
        private static void CreateEntryIfNull(Section section, string key, object defaultValue, EntryType entryType, string preComment = "")
        {
            if (string.IsNullOrWhiteSpace(section[key].StringValue))
            {
                // section doesnt exist, create it
                switch (entryType)
                {
                    case EntryType.BOOL:
                        section[key].BoolValue = (bool)defaultValue;
                        break;
                    case EntryType.FLOAT:
                        section[key].FloatValue = (float)defaultValue;
                        break;
                    case EntryType.INT:
                        section[key].IntValue = (int)defaultValue;
                        break;
                    case EntryType.STRING:
                        section[key].StringValue = (string)defaultValue;
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(preComment))
                section[key].PreComment = preComment;
        }

        /// <summary>
        /// Enum to wrap acceptable config types
        /// </summary>
        private enum EntryType { BOOL, STRING, INT, FLOAT }
    }
}
