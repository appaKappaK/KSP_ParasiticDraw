namespace ParasiticDraw
{
    public sealed class ModuleParasiticDraw : PartModule
    {
        [KSPField(guiActive = false, guiActiveEditor = false, isPersistant = false, guiFormat = "F2")]
        public double ecRate;

        [KSPField(guiActive = false, guiActiveEditor = false, isPersistant = false, guiFormat = "F2")]
        public double actualECRate;

        public void SetDrawRate(double drawRate)
        {
            ecRate = drawRate < 0.0 ? 0.0 : drawRate;
            actualECRate = ecRate;
        }
    }
}
