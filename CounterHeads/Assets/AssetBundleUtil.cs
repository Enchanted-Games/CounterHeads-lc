namespace CounterHeads.Assets;

using UnityEngine;

public static class AssetBundleUtil
{
    internal static AssetBundle MainAssetBundle = null!;

    internal static void LoadAssetBundle(string assetbundlePath)
    {
        if (MainAssetBundle == null)
        {
            MainAssetBundle = AssetBundle.LoadFromFile(assetbundlePath);
        }
    }

    internal static AssetBundle? GetBundle()
    {
        if (MainAssetBundle)
            return MainAssetBundle;
        Debug.LogError("There is no AssetBundle to load assets from.");
        return null;
    }
}