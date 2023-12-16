[System.Serializable]
public class PlayerData
{
    //TO-DO : Keep track of stats.
    public int[] kitIndices = new int[9];
    public int paintJobIndex;
    public string vehicleName;

    public PlayerData(string vehicleName, KitIndiceSettings kit_Preset, PaintJobSettings paint_Preset)
    {
        kitIndices = new int[]
        {
            kit_Preset.bonnet,
            kit_Preset.fender,
            kit_Preset.frontBumper,
            kit_Preset.rearBumper,
            kit_Preset.rollCage,
            kit_Preset.roofScoop,
            kit_Preset.sideSkirt,
            kit_Preset.spoiler,
            kit_Preset.rims
        };

        paintJobIndex = paint_Preset.paintJobIndex;

        this.vehicleName = vehicleName;
    }
}