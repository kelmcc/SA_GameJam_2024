using UnityEditor;

namespace Framework
{
    public class ScriptableEnumModificationProcessor : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (deletedAssets.Length > 0)
            {
                ScriptableEnum.MarkAssetsForReload();
            }
        }

    }
}
