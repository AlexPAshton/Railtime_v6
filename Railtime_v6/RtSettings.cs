using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Railtime_v6
{
    public class RtSettingPair
    {
        public string Key;
        public string Value;

        public RtSettingPair(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }

    static class RtSettings
    {
        public const int ZERO = 0;
        public const int ONE = 1;
        public const string DEFAULTFILE = "Settings.cfg";
        public const char SETTINGPAIRSEPERATOR = '|';
        public const char SETTINGSEPERATOR = '\n';

        public static string SaveLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";

        public static bool DoSettingsExist()
        {
            if (File.Exists(SaveLocation + DEFAULTFILE))
                return true;
            return false;
        }

        public static void CreateSettings(RtSettingPair[] Settings)
        {
            string FileOutput = string.Empty;

            for (int i = ZERO; i< Settings.Length;i++)
            {
                FileOutput += Settings[i].Key + SETTINGPAIRSEPERATOR + Settings[i].Value;

                if (i != Settings.Length - ONE)
                    FileOutput += SETTINGSEPERATOR;
            }

            File.WriteAllText(SaveLocation + DEFAULTFILE, FileOutput);
        }

        public static string ReadSetting(string Key)
        {
            string FileInput = File.ReadAllText(SaveLocation + DEFAULTFILE);

            foreach (string SettingLine in FileInput.Split(SETTINGSEPERATOR))
            {
                string[] SettingParts = SettingLine.Split(SETTINGPAIRSEPERATOR);

                if (SettingParts[ZERO] == Key)
                    return SettingParts[ONE];
            }

            throw new RtSettingNotFoundException(Key);
        }

        public static void ChangeSetting(string Key, string NewValue)
        {
            bool SettingChanged = false;
            string FileInput = File.ReadAllText(SaveLocation + DEFAULTFILE);

            foreach (string SettingLine in FileInput.Split(SETTINGPAIRSEPERATOR))
            {
                string[] SettingParts = SettingLine.Split(SETTINGSEPERATOR);

                if (SettingParts[ZERO] == Key)
                {
                    FileInput.Replace(SettingParts[ZERO] + SETTINGPAIRSEPERATOR + SettingParts[ONE], SettingParts[ZERO] + SETTINGPAIRSEPERATOR + NewValue);
                    SettingChanged = true;
                }
            }

            if (SettingChanged)
                File.WriteAllText(SaveLocation + DEFAULTFILE, FileInput);
            else
                throw new RtSettingNotFoundException(Key);
        }

        public static void DeleteSettings()
        {
            File.Delete(SaveLocation + DEFAULTFILE);
        }
    }
}