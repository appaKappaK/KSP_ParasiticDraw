using System;

namespace ParasiticDraw
{
    public static class ParasiticDrawSettingsParser
    {
        public static void ApplyValue(ParasiticDrawSettings settings, string key, string rawValue)
        {
            if (settings == null || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(rawValue))
            {
                return;
            }

            switch (key)
            {
                case "enabled":
                    settings.Enabled = ParseBool(rawValue, settings.Enabled);
                    break;
                case "debugLogging":
                    settings.DebugLogging = ParseBool(rawValue, settings.DebugLogging);
                    break;
                case "passiveDrawEnabled":
                    settings.PassiveDrawEnabled = ParseBool(rawValue, settings.PassiveDrawEnabled);
                    break;
                case "passiveDrawPreset":
                    settings.PassiveDrawPreset = ParsePreset(rawValue, settings.PassiveDrawPreset);
                    break;
                case "minimumPartCount":
                    settings.MinimumPartCount = ParseInt(rawValue, settings.MinimumPartCount);
                    break;
                case "baseVesselDraw":
                    settings.BaseVesselDraw = ParseDouble(rawValue, settings.BaseVesselDraw);
                    break;
                case "perPartDraw":
                    settings.PerPartDraw = ParseDouble(rawValue, settings.PerPartDraw);
                    break;
                case "perMassTonDraw":
                    settings.PerMassTonDraw = ParseDouble(rawValue, settings.PerMassTonDraw);
                    break;
                case "perCrewCapacityDraw":
                    settings.PerCrewCapacityDraw = ParseDouble(rawValue, settings.PerCrewCapacityDraw);
                    break;
                case "perCrewPresentDraw":
                    settings.PerCrewPresentDraw = ParseDouble(rawValue, settings.PerCrewPresentDraw);
                    break;
                case "perCommandModuleDraw":
                    settings.PerCommandModuleDraw = ParseDouble(rawValue, settings.PerCommandModuleDraw);
                    break;
                case "globalDrawMultiplier":
                    settings.GlobalDrawMultiplier = ParseDouble(rawValue, settings.GlobalDrawMultiplier);
                    break;
            }
        }

        public static ParasiticDrawSettings Finalize(ParasiticDrawSettings settings)
        {
            if (settings == null)
            {
                return ParasiticDrawSettings.Defaults();
            }

            settings.Sanitize();
            return settings;
        }

        private static bool ParseBool(string rawValue, bool fallback)
        {
            bool value;
            return bool.TryParse(rawValue, out value) ? value : fallback;
        }

        private static double ParseDouble(string rawValue, double fallback)
        {
            double value;
            return double.TryParse(rawValue, out value) ? value : fallback;
        }

        private static int ParseInt(string rawValue, int fallback)
        {
            int value;
            return int.TryParse(rawValue, out value) ? value : fallback;
        }

        private static PassiveDrawPreset ParsePreset(string rawValue, PassiveDrawPreset fallback)
        {
            if (string.Equals(rawValue, "Custom", StringComparison.OrdinalIgnoreCase))
            {
                return PassiveDrawPreset.Standard;
            }

            PassiveDrawPreset value;
            return Enum.TryParse(rawValue, true, out value) ? value : fallback;
        }
    }
}
