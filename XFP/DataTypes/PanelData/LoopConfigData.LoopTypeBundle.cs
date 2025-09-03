namespace Xfp.DataTypes.PanelData
{
    public partial class LoopConfigData
    {
        public class LoopTypeBundle
        {
            public CTecDevices.ObjectTypes Protocol { get; set; }
            public int NumLoops { get; set; }

            public static LoopTypeBundle Parse(byte[] data)
                => new()
                {
                    Protocol    = data[2] switch { 0 => CTecDevices.ObjectTypes.XfpApollo, 4 => CTecDevices.ObjectTypes.XfpCast, _ => CTecDevices.ObjectTypes.NotSet },
                    NumLoops    = data[3],
                };
        }
    }
}
