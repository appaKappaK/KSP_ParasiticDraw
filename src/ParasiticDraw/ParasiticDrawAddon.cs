using System;
using System.Linq;
using UnityEngine;

namespace ParasiticDraw
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public sealed class ParasiticDrawAddon : MonoBehaviour
    {
        private const string ElectricCharge = "ElectricCharge";

        private ParasiticDrawSettings settings;
        private Vessel lastVessel;

        public void Start()
        {
            settings = SettingsLoader.Load();

            Debug.Log(string.Format(
                "[ParasiticDraw] Loaded v1.1.0. Enabled: {0}, Passive draw: {1}, Preset: {2}, Minimum parts: {3}",
                settings.Enabled,
                settings.PassiveDrawEnabled,
                settings.PassiveDrawPreset,
                settings.MinimumPartCount));

            if (settings.DebugLogging)
            {
                Debug.Log("[ParasiticDraw] Loaded settings. Passive draw enabled: " + settings.PassiveDrawEnabled);
            }
        }

        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            if (settings == null)
            {
                settings = SettingsLoader.Load();
            }

            if (!settings.Enabled || !settings.PassiveDrawEnabled)
            {
                return;
            }

            Vessel vessel = FlightGlobals.ActiveVessel;
            if (vessel == null || !vessel.loaded || vessel.rootPart == null)
            {
                return;
            }

            double deltaTime = PassiveDrawCalculator.ClampDeltaTime(TimeWarp.deltaTime);
            if (vessel != lastVessel)
            {
                lastVessel = vessel;
                return;
            }

            if (deltaTime <= 0.0)
            {
                return;
            }

            double drawPerSecond = PassiveDrawCalculator.CalculateEcPerSecond(settings, CreateSnapshot(vessel));
            UpdateReporterModules(vessel, drawPerSecond);
            if (drawPerSecond <= 0.0)
            {
                return;
            }

            double requested = drawPerSecond * deltaTime;
            double consumed = vessel.rootPart.RequestResource(ElectricCharge, requested);

            if (settings.DebugLogging && consumed + 0.000001 < requested)
            {
                Debug.Log(string.Format(
                    "[ParasiticDraw] Shortfall: {0:F3} EC on {1}",
                    requested - consumed,
                    vessel.vesselName));
            }
        }

        private static VesselDrawSnapshot CreateSnapshot(Vessel vessel)
        {
            VesselDrawSnapshot snapshot = new VesselDrawSnapshot();
            if (vessel == null || vessel.Parts == null)
            {
                return snapshot;
            }

            snapshot.PartCount = vessel.Parts.Count;
            snapshot.VesselMassTons = Math.Max(0.0, vessel.GetTotalMass());
            snapshot.CrewCapacity = vessel.Parts.Sum(part => Math.Max(0, part.CrewCapacity));
            snapshot.CurrentCrew = vessel.GetCrewCount();
            snapshot.CommandModuleCount = vessel.Parts.Count(part => part.Modules != null && part.Modules.Contains("ModuleCommand"));

            return snapshot;
        }

        private static void UpdateReporterModules(Vessel vessel, double drawPerSecond)
        {
            if (vessel == null || vessel.Parts == null)
            {
                return;
            }

            ModuleParasiticDraw[] reporters = vessel.Parts
                .SelectMany(part => part.Modules.OfType<ModuleParasiticDraw>())
                .ToArray();

            double drawPerReporter = reporters.Length > 0 ? drawPerSecond / reporters.Length : 0.0;

            foreach (ModuleParasiticDraw reporter in reporters)
            {
                reporter.SetDrawRate(drawPerReporter);
            }
        }
    }
}
