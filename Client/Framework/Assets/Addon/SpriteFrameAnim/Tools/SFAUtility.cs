using System.IO;
using UnityEditor;
using UnityEngine;

namespace SFA
{
    public class SFAUtility
    {
        /// <summary>
        /// 根据相机渲染的内容，截取一张Texture
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="makeShadow"></param>
        /// <param name="shadowOpacity"></param>
        /// <returns></returns>
        public static Texture2D CutoutTexture(Camera camera, Vector2Int resolution, Vector2Int cutSize, string path)
        {
            if (camera == null)
                return Texture2D.whiteTexture;

            Color tempBgColor = camera.backgroundColor;
            CameraClearFlags tempFlags = camera.clearFlags;

            camera.depthTextureMode = DepthTextureMode.None;
            camera.clearFlags = CameraClearFlags.SolidColor;

            Rect readRect = new Rect(resolution.x / 2 - cutSize.x / 2, resolution.y / 2 - cutSize.y / 2, cutSize.x, cutSize.y);
            camera.backgroundColor = Color.black;
            camera.Render();
            RenderTexture.active = camera.targetTexture;
            Texture2D texOnBlack = new Texture2D(cutSize.x, cutSize.y, TextureFormat.RGB24, false);
            texOnBlack.ReadPixels(readRect, 0, 0);
            texOnBlack.Apply();
            //SaveTexture2PNG(texOnBlack, path + "/black.png");

            readRect = new Rect(resolution.x / 2 - cutSize.x / 2, resolution.y / 2 - cutSize.y / 2, cutSize.x, cutSize.y);
            camera.backgroundColor = Color.white;
            camera.Render();
            RenderTexture.active = camera.targetTexture;
            Texture2D texOnWhite = new Texture2D(cutSize.x, cutSize.y, TextureFormat.RGB24, false);
            texOnWhite.ReadPixels(readRect, 0, 0);
            texOnWhite.Apply();
            //SaveTexture2PNG(texOnWhite, path + "/white.png");

            Texture2D tex = new Texture2D(cutSize.x, cutSize.y, TextureFormat.ARGB32, false);
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    Color pixelOnBlack = texOnBlack.GetPixel(x, y);
                    Color pixelOnWhite = texOnWhite.GetPixel(x, y);

                    float redDiff = pixelOnWhite.r - pixelOnBlack.r;
                    float greenDiff = pixelOnWhite.g - pixelOnBlack.g;
                    float blueDiff = pixelOnWhite.b - pixelOnBlack.b;
                    float grayDiff = pixelOnWhite.grayscale - pixelOnBlack.grayscale;
                    float minDiff = Mathf.Min(redDiff, greenDiff);
                    minDiff = Mathf.Min(minDiff, blueDiff);
                    minDiff = Mathf.Min(minDiff, grayDiff);

                    float alpha = 1.0f - (minDiff);
                    Color color = Color.clear;
                    if (alpha != 0)
                    {
                        color = pixelOnBlack / alpha;
                    }
                    color.a = alpha;

                    tex.SetPixel(x, y, color);
                }
            }
            camera.backgroundColor = tempBgColor;
            camera.clearFlags = tempFlags;
            return tex;
        }


        public static Vector4[] Rects2Vector4Array(Rect[] rects)
        {
            Vector4[] v4s = new Vector4[rects.Length];
            for (int i = 0; i < rects.Length; i++)
            {
                v4s[i] = new Vector4(rects[i].x, rects[i].y, rects[i].width, rects[i].height);
            }
            return v4s;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 创建asset配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public static void CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Not select files, select files first! ");
                return;
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        public static string GetPrefabPath(GameObject targetGameObject)
        {
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(targetGameObject);
            if (prefab != null)
                return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
            else
                return null;
        }

        public static void Space()
        //制作名单
        {
            GUILayout.Space(20);
            GUIStyle style = new GUIStyle("ProjectBrowserBottomBarBg");
            style.fixedHeight = 2.5f;
            EditorGUILayout.LabelField(GUIContent.none, style);
            GUILayout.Space(-10);
        }

        /// <summary>
        /// 绝对路径->相对路径
        /// </summary>
        public static string Absolute2Relativity(string path)
        {
            string temp = path.Substring(path.IndexOf("Assets"));
            temp = temp.Replace('\\', '/');
            return temp;
        }

        /// <summary>
        /// 相对路径->绝对路径
        /// </summary>
        public static string Relativity2Absolute(string path)
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            path = files[0].DirectoryName;
            return path;
        }

        public static void SaveTexture2PNG(Texture2D tex, string filePath)
        {
            byte[] bytes = tex.EncodeToPNG();
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllBytes(filePath, bytes);
        }

#endif
    }
}
