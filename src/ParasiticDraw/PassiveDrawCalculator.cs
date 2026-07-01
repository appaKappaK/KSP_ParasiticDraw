using System;

namespace ParasiticDraw
{
    public static class PassiveDrawCalculator
    {
        public static double CalculateEcPerSecond(ParasiticDrawSettings settings, VesselDrawSnapshot vessel)
        {
            if (settings == null || vessel == null || !settings.Enabled || !settings.PassiveDrawEnabled)
            {
                return 0.0;
            }

            settings.Sanitize();

            if (vessel.PartCount < settings.MinimumPartCount)
            {
                return 0.0;
            }

            double baseDraw =
                settings.BaseVesselDraw
                + NonNegative(vessel.PartCount) * settings.PerPartDraw
                + NonNegative(vessel.VesselMassTons) * settings.PerMassTonDraw
                + NonNegative(vessel.CrewCapacity) * settings.PerCrewCapacityDraw
                + NonNegative(vessel.CurrentCrew) * settings.PerCrewPresentDraw
                + NonNegative(vessel.CommandModuleCount) * settings.PerCommandModuleDraw;

            return Math.Max(0.0, baseDraw * settings.PresetMultiplier * settings.GlobalDrawMultiplier);
        }

        public static double ClampDeltaTime(double deltaTime)
        {
            if (double.IsNaN(deltaTime) || double.IsInfinity(deltaTime) || deltaTime <= 0.0)
            {
                return 0.0;
            }

            if (deltaTime > ParasiticDrawSettings.MaxDeltaTimeSeconds)
            {
                return 0.0;
            }

            return deltaTime;
        }

        private static double NonNegative(double value)
        {
            return value < 0.0 ? 0.0 : value;
        }
    }
}
