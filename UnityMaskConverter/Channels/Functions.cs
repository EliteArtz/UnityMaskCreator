using System.Collections.Generic;

namespace UnityMaskCreator.Channels
{
    public static partial class Channels
    {
        public static Channel LayerChannel(string layer)
        {
            switch (layer)
            {
                case "metallic":
                    return Channel.R;
                case "ao":
                    return Channel.G;
                case "height":
                    return Channel.B;
                case "smoothness":
                    return Channel.A;
                default:
                    return Channel.R;
            }
        }

        public static bool IsChannelUsed(Channel channel)
        {
            List<MaskInput> images = Main.images;
            if (images.Count == 0) return false;

            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].channel == channel) return true;
            }

            return false;
        }
    }
}
