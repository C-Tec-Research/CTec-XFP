using System.Windows.Media.Imaging;

namespace Xfp.UI.Util
{
    internal class Bitmaps
    {
        /// <summary>
        /// Get a BitmapImage from the XfpTools resources in the "UI/Images" folder.
        /// NB: the image is presumed to be a .png if a .jpg, .gif or .bmp suffix is not found.
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        internal static BitmapImage GetBitmap(string imageName) => CTecUtil.Utils.BitmapUtil.GetBitmap("XfpTools", "UI/Images", imageName);
    }
}
