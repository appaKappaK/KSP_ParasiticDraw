namespace ParasiticDraw
{
    public sealed class ParasiticDrawDifficultySettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Enabled")]
        public bool enabled = true;

        [GameParameters.CustomIntParameterUI("Preset Multiplier", minValue = 0, maxValue = 3, toolTip = "Applied before the global multiplier.")]
        public int passiveDrawPreset = 1;

        [GameParameters.CustomIntParameterUI("Minimum Part Count", minValue = 0, maxValue = 100, stepSize = 1, displayFormat = "N0", toolTip = "Vessels below this part count are ignored. Higher values can still be set in Settings.cfg.")]
        public int minimumPartCount = 0;

        [GameParameters.CustomFloatParameterUI("Global Draw Multiplier", minValue = 0f, maxValue = 20f, stepCount = 201, displayFormat = "0.0", toolTip = "Multiplier applied after preset scaling.")]
        public float globalDrawMultiplier = 1f;

        public ParasiticDrawDifficultySettings()
        {
            ApplyConfigDefaults(SettingsLoader.LoadConfigSettings());
        }

        public override string Title
        {
            get { return "ParasiticDraw"; }
        }

        public override GameParameters.GameMode GameMode
        {
            get { return GameParameters.GameMode.ANY; }
        }

        public override string Section
        {
            get { return "ParasiticDraw"; }
        }

        public override string DisplaySection
        {
            get { return "ParasiticDraw"; }
        }

        public override int SectionOrder
        {
            get { return 1; }
        }

        public override bool HasPresets
        {
            get { return true; }
        }

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    passiveDrawPreset = 0;
                    minimumPartCount = 10;
                    globalDrawMultiplier = 1f;
                    break;
                case GameParameters.Preset.Moderate:
                case GameParameters.Preset.Normal:
                    passiveDrawPreset = 1;
                    minimumPartCount = 0;
                    globalDrawMultiplier = 1f;
                    break;
                case GameParameters.Preset.Hard:
                    passiveDrawPreset = 2;
                    minimumPartCount = 0;
                    globalDrawMultiplier = 1f;
                    break;
            }
        }

        private void ApplyConfigDefaults(ParasiticDrawSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            enabled = settings.Enabled && settings.PassiveDrawEnabled;
            passiveDrawPreset = FromPreset(settings.PassiveDrawPreset);
            minimumPartCount = settings.MinimumPartCount;
            globalDrawMultiplier = (float)settings.GlobalDrawMultiplier;
        }

        public PassiveDrawPreset ToPreset()
        {
            switch (passiveDrawPreset)
            {
                case 0:
                    return PassiveDrawPreset.Light;
                case 2:
                    return PassiveDrawPreset.Harsh;
                case 3:
                    return PassiveDrawPreset.Strong;
                case 1:
                default:
                    return PassiveDrawPreset.Standard;
            }
        }

        private static int FromPreset(PassiveDrawPreset preset)
        {
            switch (preset)
            {
                case PassiveDrawPreset.Light:
                    return 0;
                case PassiveDrawPreset.Harsh:
                    return 2;
                case PassiveDrawPreset.Strong:
                    return 3;
                case PassiveDrawPreset.Standard:
                default:
                    return 1;
            }
        }
    }
}
