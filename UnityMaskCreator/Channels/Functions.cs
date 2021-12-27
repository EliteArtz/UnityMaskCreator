using System.Collections.Generic;

namespace UnityMaskCreator.Channels
{
    public static partial class Channels
    {
        public static void SwitchColorChannelDestinations(List<MaskInput> imgs, Channel oldChannel, Channel newChannel)
        {
            if (imgs.Count == 0) return;

            var i1 = imgs.FindIndex(image => image.channel == newChannel);
            var i2 = imgs.FindIndex(image => image.channel == oldChannel);
            if (i1 != -1)
                imgs[i1] = new MaskInput { path = imgs[i1].path, channel = oldChannel };
            if (i2 != -1)
                imgs[i2] = new MaskInput { path = imgs[i2].path, channel = newChannel };
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
