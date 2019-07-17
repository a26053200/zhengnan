using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SFA
{
    public class SFAUtility
    {
        
        public static void AdjustCamera()
        {
            if (Camera.main == null || !Camera.main.orthographic)
                return;

            Camera.main.orthographicSize = 1;

            float cameraWidth = Camera.main.pixelWidth;
            float cameraHeight = Camera.main.pixelHeight;
            //Debug.Log("cameraWidth: " + cameraWidth + ", cameraHeight: " + cameraHeight + ", orthographicSize: " + Camera.main.orthographicSize);

            Vector2 modelScreenMinPos = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 modelScreenMaxPos = new Vector2(float.MinValue, float.MinValue);

            Vector3 modelWorldMinPos = Vector3.zero;// studio.targetModel.GetMinPos();
            Vector3 modelWorldMaxPos = Vector3.zero;//studio.targetModel.GetMaxPos();
            //Debug.Log("modelWorldMinPos: " + modelWorldMinPos + ", modelWorldMaxPos: " + modelWorldMaxPos);
            WorldToScreenMinMaxPoints(modelWorldMinPos, modelWorldMaxPos, ref modelScreenMinPos, ref modelScreenMaxPos);
            //Debug.Log("modelScreenMinPos: " + modelScreenMinPos + ", modelScreenMaxPos: " + modelScreenMaxPos);

            //modelWorldMinPos = studio.targetModel.GetExactMinPos();
            //modelWorldMaxPos = studio.targetModel.GetExactMaxPos();
            ////Debug.Log("modelWorldMinPos: " + modelWorldMinPos + ", modelWorldMaxPos: " + modelWorldMaxPos);
            //WorldToScreenMinMaxPoints(modelWorldMinPos, modelWorldMaxPos, ref modelScreenMinPos, ref modelScreenMaxPos);
            ////Debug.Log("modelScreenMinPos: " + modelScreenMinPos + ", modelScreenMaxPos: " + modelScreenMaxPos);

            float modelScreenWidth = modelScreenMaxPos.x - modelScreenMinPos.x;
            float modelScreenHeight = modelScreenMaxPos.y - modelScreenMinPos.y;
            //Debug.Log("modelScreenWidth: " + modelScreenWidth + ", modelScreenHeight: " + modelScreenHeight);

            if (modelScreenMinPos.x < 0f)
                modelScreenWidth += Math.Abs(modelScreenMinPos.x) * 2f;
            if (modelScreenMaxPos.x > cameraWidth)
                modelScreenWidth += (modelScreenMaxPos.x - cameraWidth) * 2f;
            if (modelScreenMinPos.y < 0f)
                modelScreenHeight += Math.Abs(modelScreenMinPos.y) * 2f;
            if (modelScreenMaxPos.y > cameraHeight)
                modelScreenHeight += (modelScreenMaxPos.y - cameraHeight) * 2f;
            //Debug.Log("Resized - modelScreenWidth: " + modelScreenWidth + ", modelScreenHeight: " + modelScreenHeight);

            if (modelScreenWidth <= 0.0f && modelScreenHeight <= 0.0f)
            {
                Debug.Assert(false, "modelScreenWidth <= 0.0f && modelScreenHeight <= 0.0f");
                return;
            }

            if (modelScreenWidth > cameraWidth || modelScreenHeight > cameraHeight)
            {
                //Debug.Log("Model is bigger");
                float widthScaleRatio = modelScreenWidth / cameraWidth;
                float heightScaleRatio = modelScreenHeight / cameraHeight;
                //Debug.Log("widthScaleRatio: " + widthScaleRatio + ", heightScaleRatio: " + heightScaleRatio);
                float maxScaleRatio = Mathf.Max(widthScaleRatio, heightScaleRatio);
                Camera.main.orthographicSize *= maxScaleRatio;
            }
            else if (modelScreenWidth < cameraWidth && modelScreenHeight < cameraHeight)
            {
                //Debug.Log("Camera is bigger");
                float widthScaleRatio = cameraWidth / modelScreenWidth;
                float heightScaleRatio = cameraHeight / modelScreenHeight;
                //Debug.Log("widthScaleRatio: " + widthScaleRatio + ", heightScaleRatio: " + heightScaleRatio);
                float minScaleRatio = Mathf.Min(widthScaleRatio, heightScaleRatio);
                Camera.main.orthographicSize /= minScaleRatio;
            }
        }

        private static void WorldToScreenMinMaxPoints(Vector3 worldMinPos, Vector3 worldMaxPos, ref Vector2 screenMinPos, ref Vector2 screenMaxPos)
        {
            Vector3[] worldPositions = new Vector3[8];
            worldPositions[0] = new Vector3(worldMinPos.x, worldMinPos.y, worldMinPos.z);
            worldPositions[1] = new Vector3(worldMinPos.x, worldMaxPos.y, worldMinPos.z);
            worldPositions[2] = new Vector3(worldMinPos.x, worldMinPos.y, worldMaxPos.z);
            worldPositions[3] = new Vector3(worldMinPos.x, worldMaxPos.y, worldMaxPos.z);
            worldPositions[4] = new Vector3(worldMaxPos.x, worldMinPos.y, worldMinPos.z);
            worldPositions[5] = new Vector3(worldMaxPos.x, worldMaxPos.y, worldMinPos.z);
            worldPositions[6] = new Vector3(worldMaxPos.x, worldMinPos.y, worldMaxPos.z);
            worldPositions[7] = new Vector3(worldMaxPos.x, worldMaxPos.y, worldMaxPos.z);

            foreach (Vector3 worldPos in worldPositions)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                screenMinPos.x = Mathf.Min(screenMinPos.x, screenPos.x);
                screenMaxPos.x = Mathf.Max(screenMaxPos.x, screenPos.x);
                screenMinPos.y = Mathf.Min(screenMinPos.y, screenPos.y);
                screenMaxPos.y = Mathf.Max(screenMaxPos.y, screenPos.y);
            }
        }

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

        public static void CalcTextureBound(Texture2D tex, ScreenPoint pivot, out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = int.MaxValue; maxX = int.MinValue;
            minY = int.MaxValue; maxY = int.MinValue;

            if (tex == null)
                return;

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float alpha = tex.GetPixel(x, y).a;
                    if (alpha != 0)
                    {
                        minX = x;
                        goto ENDMINX;
                    }
                }
            }

            ENDMINX:
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = minX; x < tex.width; x++)
                {
                    float alpha = tex.GetPixel(x, y).a;
                    if (alpha != 0)
                    {
                        minY = y;
                        goto ENDMINY;
                    }
                }
            }

            ENDMINY:
            for (int x = tex.width - 1; x >= minX; x--)
            {
                for (int y = minY; y < tex.height; y++)
                {
                    float alpha = tex.GetPixel(x, y).a;
                    if (alpha != 0)
                    {
                        maxX = x;
                        goto ENDMAXX;
                    }
                }
            }

            ENDMAXX:
            for (int y = tex.height - 1; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float alpha = tex.GetPixel(x, y).a;
                    if (alpha != 0)
                    {
                        maxY = y;
                        goto ENDMAXY;
                    }
                }
            }

            ENDMAXY:
            if (pivot.x < minX)
                minX = pivot.x;
            if (pivot.x > maxX)
                maxX = pivot.x;
            if (pivot.y < minY)
                minY = pivot.y;
            if (pivot.y > maxY)
                maxY = pivot.y;
        }

        public static void CalcTextureSymmetricBound(
            bool symmetricByPivot, bool verticalSymmectric,
            ScreenPoint pivot, int maxWidth, int maxHeight,
            int minX, int maxX, int minY, int maxY,
            ref int exMinX, ref int exMaxX, ref int exMinY, ref int exMaxY)
        {
            if (pivot == null)
                return;

            if (symmetricByPivot)
            {
                int stt2pivot = pivot.x - minX;
                int pivot2end = maxX - pivot.x;
                if (stt2pivot > pivot2end)
                {
                    maxX = pivot.x + stt2pivot;
                    if (maxX >= maxWidth)
                        maxX = maxWidth - 1;
                }
                else if (pivot2end > stt2pivot)
                {
                    minX = pivot.x - pivot2end;
                    if (minX < 0)
                        minX = 0;
                }

                if (verticalSymmectric)
                {
                    stt2pivot = pivot.y - minY;
                    pivot2end = maxY - pivot.y;
                    if (stt2pivot > pivot2end)
                    {
                        maxY = pivot.y + stt2pivot;
                        if (maxY >= maxHeight)
                            maxY = maxHeight - 1;
                    }
                    else if (pivot2end > stt2pivot)
                    {
                        minY = pivot.y - pivot2end;
                        if (minY < 0)
                            minY = 0;
                    }
                }
            }

            exMinX = Mathf.Min(exMinX, minX); exMaxX = Mathf.Max(exMaxX, maxX);
            exMinY = Mathf.Min(exMinY, minY); exMaxY = Mathf.Max(exMaxY, maxY);
        }

        public static Texture2D TrimTexture(Texture2D tex, int minX, int maxX, int minY, int maxY)
        {
            if (tex == null)
                return Texture2D.whiteTexture;

            int newTexWidth = maxX - minX + 1;
            int newTexHeight = maxY - minY + 1;

            Texture2D trimmedTex = new Texture2D(newTexWidth, newTexHeight, TextureFormat.RGBA32, false);
            for (int y = 0; y < newTexHeight; y++)
            {
                for (int x = 0; x < newTexWidth; x++)
                {
                    Color color = tex.GetPixel(minX + x, minY + y);
                    trimmedTex.SetPixel(x, y, color);
                }
            }

            return trimmedTex;
        }

        public static Texture2D MoveTextureBy(Texture2D tex, int moveX, int moveY)
        {
            if (tex == null)
                return Texture2D.whiteTexture;

            int newTexWidth = tex.width + Math.Abs(moveX);
            int newTexHeight = tex.height + Math.Abs(moveY);

            Texture2D movedTex = new Texture2D(newTexWidth, newTexHeight, TextureFormat.RGBA32, false);
            for (int y = 0; y < newTexHeight; y++)
            {
                for (int x = 0; x < newTexWidth; x++)
                {
                    Color color = new Color(0f, 0f, 0f, 0f);

                    int refX = x - moveX, refY = y - moveY;
                    if (refX >= 0 && refY >= 0 && refX <= tex.width && refY <= tex.height)
                        color = tex.GetPixel(refX, refY);

                    movedTex.SetPixel(x, y, color);
                }
            }

            return movedTex;
        }

        public static void SaveTexture(string dirPath, string fileName, Texture2D tex)
        {
            try
            {
                string filePath = Path.Combine(dirPath, fileName + ".png");
                byte[] bytes = tex.EncodeToPNG();
#if UNITY_WEBPLAYER
                Debug.Log("Don't set 'Build Setting > Platform' to WebPlayer!");
#else
                File.WriteAllBytes(filePath, bytes);
#endif
            }
            catch (Exception e)
            {
                throw e;
            }
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

        public static void DrawOutline(Rect rect, Color color, float lineWidth)
        {
            Texture2D tex = Texture2D.whiteTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, lineWidth, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax - lineWidth, rect.yMin, lineWidth, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, lineWidth), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - lineWidth, rect.width, lineWidth), tex);
            GUI.color = Color.white;
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
