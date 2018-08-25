using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class AtlasImporter : AssetPostprocessor
{

    void OnPostprocessTexture(Texture2D texture)
    {
       
        TextureImporter ti = assetImporter as TextureImporter;
        string atlas = "Assets/UINew/atlas/";
        string textures = "Assets/UINew/texture/";
        bool dirty = false;
        if (ti.assetPath.StartsWith(atlas))
        {
            if(ti.textureType!= TextureImporterType.Sprite)
            {
                dirty = true;
                ti.textureType = TextureImporterType.Sprite;
            }
            if (ti.mipmapEnabled)
            {
                dirty = true;
                ti.mipmapEnabled = false;
            }
			if (!ti.sRGBTexture)
			{
				dirty = true;
				ti.sRGBTexture = true;
			}
			if (ti.filterMode!= FilterMode.Bilinear)
			{
				dirty = true;
				ti.filterMode = FilterMode.Bilinear;
			}
            if (!ti.alphaIsTransparency)
            {
                dirty = true;
                ti.alphaIsTransparency = true;
            }
            if (ti.isReadable)
            {
                dirty = true;
                ti.isReadable = false;
            }
            if (ti.wrapMode != TextureWrapMode.Clamp)
            {
                dirty = true;
                ti.wrapMode = TextureWrapMode.Clamp;
            }
            if (ti.spriteImportMode != SpriteImportMode.Single)
            {
                dirty = true;
                ti.spriteImportMode = SpriteImportMode.Single;
            }
            string dir = ti.assetPath.Substring(atlas.Length);
            dir = "atlas/" + dir.Substring(0, dir.IndexOf("/"));
            if (ti.spritePackingTag != dir)
            {
                dirty = true;
                ti.spritePackingTag = dir;
            }
            string bundlename = (dir + ".unity3d").ToLower();
            if (ti.assetBundleName != bundlename)
            {
                dirty = true;
                ti.assetBundleName = bundlename;
            }


			TextureImporterPlatformSettings setting = ti.GetPlatformTextureSettings("Android");
			if (setting.overridden == false) {
				setting.overridden = true;
				dirty = true;
			}
			if (setting.format != TextureImporterFormat.ETC2_RGBA8) {
				setting.format = TextureImporterFormat.ETC2_RGBA8;
				dirty = true;
			}
			if (setting.allowsAlphaSplitting != false) {
				setting.allowsAlphaSplitting = false;
				dirty = true;
			}
            //if (setting.textureCompression != TextureImporterCompression.CompressedHQ) {
            //	setting.textureCompression = TextureImporterCompression.CompressedHQ;
            //	dirty = true;
            //}
            if (setting.compressionQuality != 50)
            {
                setting.compressionQuality = 50;
                dirty = true;
            }

            if (dirty) {
				ti.SetPlatformTextureSettings (setting);
			}

            //******************************iOS******************************//

            setting = ti.GetPlatformTextureSettings("iPhone");
            if (setting.overridden == false)
            {
                setting.overridden = true;
                dirty = true;
            }

            TextureImporterFormat iOSFormat=checkiOSFormat(ti.assetPath,ti);
            if (setting.format != iOSFormat)
            {
                setting.format = iOSFormat;
                dirty = true;
            }

            if (setting.allowsAlphaSplitting != false)
            {
                setting.allowsAlphaSplitting = false;
                dirty = true;
            }
            if (setting.textureCompression != TextureImporterCompression.CompressedHQ)
            {
                setting.textureCompression = TextureImporterCompression.CompressedHQ;
                dirty = true;
            }
            if (setting.compressionQuality < 100)
            {
                setting.compressionQuality = 100;
                dirty = true;
            }

            if (dirty)
            {
                ti.SetPlatformTextureSettings(setting);
            }

            //******************************iOS******************************//

			setting = ti.GetPlatformTextureSettings("Standalone");
			if (setting.overridden == false) {
				setting.overridden = true;
				dirty = true;
			}

			if (setting.textureCompression != TextureImporterCompression.Uncompressed) {
				setting.textureCompression = TextureImporterCompression.Uncompressed;
				dirty = true;
			}

//			if (setting.format != TextureImporterFormat.ARGB32) {
//				setting.format = TextureImporterFormat.ARGB32;
//				dirty = true;
//			}

			if (dirty) {
				ti.SetPlatformTextureSettings (setting);
			}

        }
     
        else if (ti.assetPath.StartsWith(textures))
        {
			
        
            if (ti.textureType != TextureImporterType.Sprite)
            {
                dirty = true;
                ti.textureType = TextureImporterType.Sprite;
            }
            if (ti.mipmapEnabled)
            {
                dirty = true;
                ti.mipmapEnabled = false;
            }
			if (!ti.sRGBTexture)
			{
				dirty = true;
				ti.sRGBTexture = true;
			}
			if (ti.filterMode!= FilterMode.Bilinear)
			{
				dirty = true;
				ti.filterMode = FilterMode.Bilinear;
			}
            if (!ti.alphaIsTransparency)
            {
                dirty = true;
                ti.alphaIsTransparency = true;
            }
            if (ti.isReadable)
            {
                dirty = true;
                ti.isReadable = false;
            }
            if (ti.wrapMode != TextureWrapMode.Clamp)
            {
                dirty = true;
                ti.wrapMode = TextureWrapMode.Clamp;
            }
            if (ti.spriteImportMode != SpriteImportMode.Single)
            {
                dirty = true;
                ti.spriteImportMode = SpriteImportMode.Single;
            }

			if(ti.assetPath.EndsWith(".jpg")){
				if (ti.alphaSource != TextureImporterAlphaSource.None)
				{
					dirty = true;
					ti.alphaSource = TextureImporterAlphaSource.None;
				}	
			}

            string s = ti.assetPath.Replace(textures, "texture/");
            string ext = System.IO.Path.GetExtension(s);
            s = s.Replace(ext, "");
            string sequence = "Assets/UINew/texture/xuliezhen/";
            if (ti.assetPath.StartsWith(sequence))
            {
                if (!string.IsNullOrEmpty(ti.spritePackingTag))
                {
                    dirty = true;
                    ti.spritePackingTag = null;
                }
                ti.spritePackingTag = null;
            }
            else
            {
                if (ti.spritePackingTag != s)
                {
                    dirty = true;
                    ti.spritePackingTag = s;
                }
            }
            string bundlename = s + ".unity3d";
            bundlename = bundlename.ToLower();
            if (ti.assetBundleName != bundlename)
            {
                dirty = true;
                ti.assetBundleName = bundlename;
            }


            //******************************iOS******************************//

            TextureImporterPlatformSettings setting = ti.GetPlatformTextureSettings("iPhone");
            if (setting.overridden == false)
            {
                setting.overridden = true;
                dirty = true;
            }


            TextureImporterFormat iOSFormat = checkiOSFormat(ti.assetPath,ti);
            if (setting.format != iOSFormat)
            {
                setting.format = iOSFormat;
                dirty = true;
            }

           

            if (setting.allowsAlphaSplitting != false)
            {
                setting.allowsAlphaSplitting = false;
                dirty = true;
            }
            if (setting.textureCompression != TextureImporterCompression.CompressedHQ)
            {
                setting.textureCompression = TextureImporterCompression.CompressedHQ;
                dirty = true;
            }
            if (setting.compressionQuality < 100)
            {
                setting.compressionQuality = 100;
                dirty = true;
            }

            if (dirty)
            {
                ti.SetPlatformTextureSettings(setting);
            }

            //******************************iOS******************************//



        }

        if (dirty)
        {
            ti.SaveAndReimport();
        }            

    }

    TextureImporterFormat checkiOSFormat(string filename,TextureImporter ti)
    {
        Char delimiter = '|';
        string rgba16names = "zidongwabaozhong.png|zidongxunluzhong.png|zidongzhandoouzhong.png|gensuiduizhangzhong.png|liaotiaoanniu.png|60jigaobiedansheng1.png|feisheng.png|150jifeishengchengxian.png|180jifeishenhuanzhuang.png" +
                              "touxiangkuang.png|suochuanniu.png|suojinanniu 1.png|renwuanniuxuanzhong.png|renwuanniumoren.png|hongdiantishi.png|suojinjiantou1.png|suojinjiantou2.png|shengguoanniu.png|shuxingdian.png|touxiananniu.png|nan.png|nv.png|bangpai01_0004.png" +
                              "bangpai01_0005.png|tongyongshuruwenziditu.png|jinengkuang4.png|xuanxiangye1.png|duanzao.png|baoshi.png|qizhi02.png|qizhi01.png|guojiabeijing.png|fenleiye004.png|diwen2.png|zhanduihuizhang_0000.png|zhanduihuizhang_0001.png|yuandi1.png|yuandi2.png|shuxingdian.png|fenleiye004.png" +
                              "jianhao.png|zuidahua.png|tongyongshuruwenziditu.png|fenleiye004.png|lianhua.png|diyi.png|dier.png|disan.png|qianwangchognzhi.png|tongyongjindutiaoditu.png|tongyongjindutiao.png|xinpatayouhua_0009.png|liaotianyuananniu.png" +
                              "fahongbaotubiao.png|liaotianzhuangban.png|laba.png|huatongxiao.png|anliu2a.png|anliu2.png|dipan.png|taozhuang.png|fazhangtubiao.png|cebianfenlei01.png|cebianfenlei02.png|guizubeijing.png|gou01.png|renwu.png|xinpatayouhua_0001.png|xinxi.png|jinyanfubenbeijing10.png" +
                              "guojialingdi.png|guowangpingxuan_0001.png|zhuanguo.png|xiang.png|lv.png|hong.png|dituhong.png|tujiancheng.png|tujianhong.png|tujianlan.png|tujianlv.png|tujianzi.png|fuwenta_0001.png|youlianniu.png" +
                              "1jie.png|2jie.png|3jie.png|4jie.png|5jie.png|6jie.png|7jie.png|8jie.png|9jie.png|10jie.png|11jie.png|12jie.png|13jie.png|14jie.png|15jie.png|bangpai01_0004.png|bangpai01_0005.png|dangqianzenbao.png|danqianjianhun.png|danqianjianhun.png|shengxiaoanniu.png|shengxiaoanniuwenzi1.png";


       
        string[] substrings = rgba16names.Split(delimiter);

        foreach (var substring in substrings)
        {
            if (filename.IndexOf(substring) >= 0)
            {
                return TextureImporterFormat.RGBA16;
            }
        }

        if (filename.IndexOf("tongyonganniu") >= 0 || filename.IndexOf("wenzimiaoshu") >= 0 || filename.IndexOf("chenghaoUI") >= 0 ||
            filename.IndexOf("feishengtouxiangzi") >= 0 || filename.IndexOf("touxian") >= 0 || filename.IndexOf("biaoqing") >= 0 || filename.IndexOf("xianfenlei") >= 0 || filename.IndexOf("zhanduixitong") >= 0 || filename.IndexOf("pinzhikuang") >= 0)
        {
            return TextureImporterFormat.RGBA16;
        }


        if (filename.IndexOf("gongnengtubiao") >= 0 || filename.IndexOf("chuangjianjiaose") >= 0|| filename.IndexOf("fenleiye04.png") >= 0 ) 
        {
            return TextureImporterFormat.RGBA32;
        }

       

        if (filename.IndexOf("xuliezhen") >= 0)
        {
            Sprite target = (Sprite)AssetDatabase.LoadAssetAtPath(filename, typeof(Sprite));

            Debug.Log("target.texture.width:" + target.texture.width);
            Debug.Log("target.texture.height:" + target.texture.height);
            if (target.texture.width == target.texture.height) 
            {
                int intWith = target.texture.width;
                if ((intWith & intWith - 1) == 0) 
                {
                    return TextureImporterFormat.PVRTC_RGBA4;
                }
            }

            return TextureImporterFormat.RGBA16;

        }


        string rgba32names = "duihuakuang.png|vipanniu 1.png";

      
        substrings = rgba32names.Split(delimiter);

        foreach (var substring in substrings)
        {
            if (filename.IndexOf(substring) >= 0)
            {
                return TextureImporterFormat.RGBA32;
            }
        }

        return TextureImporterFormat.PVRTC_RGBA4;

    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        string atlas = "Assets/UINew/atlas/";
        string textures = "Assets/UINew/texture/";
        for (int i = 0; i < movedAssets.Length; i++)
        {
            if (movedAssets[i].Contains(atlas) || movedAssets[i].Contains(textures) || movedFromAssetPaths[i].Contains(atlas) || movedFromAssetPaths[i].Contains(textures))
            {
                AssetImporter.GetAtPath(movedAssets[i]).SaveAndReimport();
            }
        }
    }
}
