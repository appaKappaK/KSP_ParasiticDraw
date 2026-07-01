using System;
using System.IO;

namespace ParasiticDraw
{
    public static class SettingsLoader
    {
        public const string SettingsNodeName = "PARASITIC_DRAW_SETTINGS";
        public const string RelativeSettingsPath = "GameData/ParasiticDraw/PluginData/ParasiticDraw/Settings.cfg";

        public static ParasiticDrawSettings Load()
        {
            return ApplyDifficultySettings(LoadConfigSettings());
        }

        public static ParasiticDrawSettings LoadConfigSettings()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();
            string path = Path.Combine(KSPUtil.ApplicationRootPath, RelativeSettingsPath);
            ConfigNode config = ConfigNode.Load(path);

            if (config == null)
            {
                settings.Sanitize();
                return settings;
            }

            ConfigNode node = config.name == SettingsNodeName ? config : config.GetNode(SettingsNodeName);
            if (node == null)
            {
                settings.Sanitize();
                return settings;
            }

            ParasiticDrawSettingsParser.ApplyValue(settings, "enabled", GetValue(node, "enabled"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "debugLogging", GetValue(node, "debugLogging"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "passiveDrawEnabled", GetValue(node, "passiveDrawEnabled"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "passiveDrawPreset", GetValue(node, "passiveDrawPreset"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "minimumPartCount", GetValue(node, "minimumPartCount"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "baseVesselDraw", GetValue(node, "baseVesselDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "perPartDraw", GetValue(node, "perPartDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "perMassTonDraw", GetValue(node, "perMassTonDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "perCrewCapacityDraw", GetValue(node, "perCrewCapacityDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "perCrewPresentDraw", GetValue(node, "perCrewPresentDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "perCommandModuleDraw", GetValue(node, "perCommandModuleDraw"));
            ParasiticDrawSettingsParser.ApplyValue(settings, "globalDrawMultiplier", GetValue(node, "globalDrawMultiplier"));

            return ParasiticDrawSettingsParser.Finalize(settings);
        }

        private static ParasiticDrawSettings ApplyDifficultySettings(ParasiticDrawSettings settings)
        {
            ParasiticDrawDifficultySettings difficulty = HighLogic.CurrentGame == null || HighLogic.CurrentGame.Parameters == null
                ? null
                : HighLogic.CurrentGame.Parameters.CustomParams<ParasiticDrawDifficultySettings>();

            if (difficulty == null)
            {
                return settings;
            }

            settings.Enabled = difficulty.enabled;
            settings.PassiveDrawEnabled = difficulty.enabled;
            settings.PassiveDrawPreset = difficulty.ToPreset();
            settings.MinimumPartCount = difficulty.minimumPartCount;
            settings.GlobalDrawMultiplier = difficulty.globalDrawMultiplier;
            settings.Sanitize();

            return settings;
        }

        private static string GetValue(ConfigNode node, string key)
        {
            return node.HasValue(key) ? node.GetValue(key) : null;
        }
    }
}
