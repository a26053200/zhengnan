
using BM;
using UnityEditor;

/// <summary>
/// 打包平台对接接口
/// </summary>
public class ABBuild
{
    //[MenuItem("Packager/Build IOS Assets TexComp")]
    public static void PackIOSTexComp()
    {
        BundleManager.BuildAssetBundle(BuildTarget.iOS, Language.zh_CN);
    }
    
    public static void PackIOS()
    {
        BundleManager.BuildAssetBundle(BuildTarget.iOS, Language.zh_CN);
    }
    
    public static void PackAndroid()
    {
        BundleManager.BuildAssetBundle(BuildTarget.Android, Language.zh_CN);
    }
}
