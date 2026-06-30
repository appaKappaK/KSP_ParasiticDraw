using System;

namespace ParasiticDraw.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            TestDefaults();
            TestInvalidSettingsFallback();
            TestPresetMultipliers();
            TestDisabledSettings();
            TestTemperedBalanceTargets();
            TestDeltaClamp();

            Console.WriteLine("ParasiticDraw.Tests passed.");
            return 0;
        }

        private static void TestInvalidSettingsFallback()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();

            ParasiticDrawSettingsParser.ApplyValue(settings, "enabled", "not-a-bool");
            ParasiticDrawSettingsParser.ApplyValue(settings, "passiveDrawPreset", "not-a-preset");
            ParasiticDrawSettingsParser.ApplyValue(settings, "perPartDraw", "not-a-number");
            ParasiticDrawSettingsParser.ApplyValue(settings, "perMassTonDraw", "-5");
            ParasiticDrawSettingsParser.Finalize(settings);

            Assert(settings.Enabled, "Invalid bool should preserve fallback.");
            AssertEqual(PassiveDrawPreset.Standard, settings.PassiveDrawPreset, "Invalid preset should preserve fallback.");
            AssertNear(0.01, settings.PerPartDraw, "Invalid number should preserve fallback.");
            AssertNear(0.0, settings.PerMassTonDraw, "Negative number should sanitize to zero.");
        }

        private static void TestDefaults()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();

            Assert(settings.Enabled, "Default settings should be enabled.");
            Assert(settings.PassiveDrawEnabled, "Passive draw should be enabled by default.");
            AssertEqual(PassiveDrawPreset.Standard, settings.PassiveDrawPreset, "Default preset should be Standard.");
            AssertNear(0.01, settings.PerPartDraw, "Default part draw should match the v1 plan.");
            AssertNear(0.02, settings.PerMassTonDraw, "Default mass draw should match the v1 plan.");
        }

        private static void TestPresetMultipliers()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();
            VesselDrawSnapshot vessel = new VesselDrawSnapshot { PartCount = 10 };

            settings.PassiveDrawPreset = PassiveDrawPreset.Standard;
            double standard = PassiveDrawCalculator.CalculateEcPerSecond(settings, vessel);

            settings.PassiveDrawPreset = PassiveDrawPreset.Light;
            AssertNear(standard * 0.5, PassiveDrawCalculator.CalculateEcPerSecond(settings, vessel), "Light should be half Standard.");

            settings.PassiveDrawPreset = PassiveDrawPreset.Harsh;
            AssertNear(standard * 2.0, PassiveDrawCalculator.CalculateEcPerSecond(settings, vessel), "Harsh should be double Standard.");

            settings.PassiveDrawPreset = PassiveDrawPreset.Custom;
            AssertNear(standard, PassiveDrawCalculator.CalculateEcPerSecond(settings, vessel), "Custom should apply no preset scaling.");
        }

        private static void TestDisabledSettings()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();
            settings.Enabled = false;

            double draw = PassiveDrawCalculator.CalculateEcPerSecond(
                settings,
                new VesselDrawSnapshot { PartCount = 100, VesselMassTons = 100 });

            AssertNear(0.0, draw, "Disabled settings should produce no draw.");
        }

        private static void TestTemperedBalanceTargets()
        {
            ParasiticDrawSettings settings = ParasiticDrawSettings.Defaults();

            VesselDrawSnapshot capsule = new VesselDrawSnapshot
            {
                PartCount = 25,
                VesselMassTons = 12,
                CrewCapacity = 3,
                CurrentCrew = 3,
                CommandModuleCount = 1
            };

            AssertNear(1.29, PassiveDrawCalculator.CalculateEcPerSecond(settings, capsule), "Crewed capsule should be near v1 target.");

            VesselDrawSnapshot largeStation = new VesselDrawSnapshot
            {
                PartCount = 200,
                VesselMassTons = 500,
                CrewCapacity = 12,
                CurrentCrew = 8,
                CommandModuleCount = 4
            };

            AssertNear(14.85, PassiveDrawCalculator.CalculateEcPerSecond(settings, largeStation), "Large station Standard draw should match balance pass.");

            settings.PassiveDrawPreset = PassiveDrawPreset.Harsh;
            AssertNear(29.70, PassiveDrawCalculator.CalculateEcPerSecond(settings, largeStation), "Large station Harsh draw should match balance pass.");
        }

        private static void TestDeltaClamp()
        {
            AssertNear(0.02, PassiveDrawCalculator.ClampDeltaTime(0.02), "Normal delta should pass through.");
            AssertNear(0.0, PassiveDrawCalculator.ClampDeltaTime(10.0), "Suspicious large delta should be ignored.");
            AssertNear(0.0, PassiveDrawCalculator.ClampDeltaTime(-1.0), "Negative delta should be ignored.");
            AssertNear(0.0, PassiveDrawCalculator.ClampDeltaTime(double.NaN), "NaN delta should be ignored.");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(string.Format("{0} Expected {1}, got {2}.", message, expected, actual));
            }
        }

        private static void AssertNear(double expected, double actual, string message)
        {
            if (Math.Abs(expected - actual) > 0.000001)
            {
                throw new InvalidOperationException(string.Format("{0} Expected {1}, got {2}.", message, expected, actual));
            }
        }
    }
}
