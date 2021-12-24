namespace UnityMaskCreator.Channels
{
    public static partial class Channels
    {
        public struct MaskInput
        {
            public string path;
            public Channel channel;
        }
        public enum Channel { R, G, B, A }
    }
}
