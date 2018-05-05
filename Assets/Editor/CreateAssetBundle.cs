using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public class CreateAssetBundle
{
    public static void SetAssetName(string assetname, string bundlename, bool clearABName) {
        AssetImporter ai = AssetImporter.GetAtPath(assetname);
        if (ai != null) {
            bool isDirty = false;
            if (clearABName) {
                ai.assetBundleName = "";
                ai.SaveAndReimport();
                return;
            }
            string packing_name = StringTools.RemoveExName(bundlename);
            if (ai is TextureImporter) {
                TextureImporter ti = (TextureImporter)ai;
                if (ti.textureType != TextureImporterType.Sprite) {
                    ti.textureType = TextureImporterType.Sprite;
                    isDirty = true;
                }
                if (ti.spriteImportMode != SpriteImportMode.Single) {
                    isDirty = true;
                    ti.spriteImportMode = SpriteImportMode.Single;
                }
                if (ti.spritePackingTag != packing_name) {
                    ti.spritePackingTag = packing_name;
                    isDirty = true;
                }
                if (ti.mipmapEnabled == true) {
                    ti.mipmapEnabled = false;
                    isDirty = true;
                }
            }
            bundlename = bundlename.ToLower();
            if (ai.assetBundleName != bundlename) {
                ai.assetBundleName = bundlename;
                isDirty = true;
            }
            if (isDirty) {
                ai.SaveAndReimport();
            }
        }
    }
}