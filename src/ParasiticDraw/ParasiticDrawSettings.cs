using System;

namespace ParasiticDraw
{
    public enum PassiveDrawPreset
    {
        Light,
        Standard,
        Harsh,
        Custom
    }

    public sealed class ParasiticDrawSettings
    {
        public const double MaxDeltaTimeSeconds = 5.0;

        public bool Enabled { get; set; }
        public bool DebugLogging { get; set; }
        public bool PassiveDrawEnabled { get; set; }
        public PassiveDrawPreset PassiveDrawPreset { get; set; }
        public int MinimumPartCount { get; set; }
        public double BaseVesselDraw { get; set; }
        public double PerPartDraw { get; set; }
        public double PerMassTonDraw { get; set; }
        public double PerCrewCapacityDraw { get; set; }
        public double PerCrewPresentDraw { get; set; }
        public double PerCommandModuleDraw { get; set; }
        public double GlobalDrawMultiplier { get; set; }

        public static ParasiticDrawSettings Defaults()
        {
            return new ParasiticDrawSettings
            {
                Enabled = true,
                DebugLogging = false,
                PassiveDrawEnabled = true,
                PassiveDrawPreset = PassiveDrawPreset.Standard,
                MinimumPartCount = 0,
                BaseVesselDraw = 0.05,
                PerPartDraw = 0.01,
                PerMassTonDraw = 0.02,
                PerCrewCapacityDraw = 0.15,
                PerCrewPresentDraw = 0.05,
                PerCommandModuleDraw = 0.15,
                GlobalDrawMultiplier = 1.0
            };
        }

        public double PresetMultiplier
        {
            get
            {
                switch (PassiveDrawPreset)
                {
                    case PassiveDrawPreset.Light:
                        return 0.5;
                    case PassiveDrawPreset.Harsh:
                        return 2.0;
                    case PassiveDrawPreset.Custom:
                    case PassiveDrawPreset.Standard:
                    default:
                        return 1.0;
                }
            }
        }

        public void Sanitize()
        {
            MinimumPartCount = MinimumPartCount < 0 ? 0 : MinimumPartCount;
            BaseVesselDraw = NonNegative(BaseVesselDraw);
            PerPartDraw = NonNegative(PerPartDraw);
            PerMassTonDraw = NonNegative(PerMassTonDraw);
            PerCrewCapacityDraw = NonNegative(PerCrewCapacityDraw);
            PerCrewPresentDraw = NonNegative(PerCrewPresentDraw);
            PerCommandModuleDraw = NonNegative(PerCommandModuleDraw);
            GlobalDrawMultiplier = NonNegative(GlobalDrawMultiplier);
        }

        private static double NonNegative(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0.0)
            {
                return 0.0;
            }

            return value;
        }
    }
}
