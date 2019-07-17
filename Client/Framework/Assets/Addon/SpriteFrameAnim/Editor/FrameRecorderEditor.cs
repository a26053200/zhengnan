using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;

namespace SFA
{
    [CustomEditor(typeof(FrameRecorder))]
    public class FrameRecorderEditor : Editor
    {
        static string ExportPath = "Assets/Res/Effects/SpriteFrames/";
        FrameRecorder frameRecorder;
        List<Texture2D> textures;
        string prefabPath;
        string prefabName;

        private void OnEnable()
        {
            frameRecorder = target as FrameRecorder;
            prefabPath = SFAUtility.GetPrefabPath(frameRecorder.gameObject);
            prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            frameRecorder.exportPath = ExportPath + prefabName + "/";
            if (!Directory.Exists(ExportPath))
                Directory.CreateDirectory(ExportPath);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SFAUtility.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Output Path:");
            if (GUILayout.Button("Explorer"))
            {
                if (Directory.Exists(frameRecorder.exportPath))
                    EditorUtility.RevealInFinder(frameRecorder.exportPath);
                //Application.OpenURL("file://" + SFAUtility.Relativity2Absolute(frameRecorder.exportPath));
                else
                    Debug.Log("Not yet started recording");
            }
            GUIStyle style02 = new GUIStyle("label");
            style02.fontStyle = FontStyle.Italic;
            EditorGUILayout.LabelField(frameRecorder.exportPath, style02);
            
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Record"))
            {
                if (!Directory.Exists(frameRecorder.exportPath))
                    Directory.CreateDirectory(frameRecorder.exportPath);
                frameRecorder.StartRecord(BakeSpriteSheet);
            }

            //if (GUILayout.Button("Create Quad Mesh"))
            //{
            //    GameObject quad = GameObject.Find("Quad");
            //    MeshFilter mf = quad.GetComponent<MeshFilter>();
            //    Mesh mesh = Instantiate(mf.sharedMesh) as Mesh;
            //    AssetDatabase.CreateAsset(mesh, frameRecorder.exportPath + "/quad.mesh");
            //    AssetDatabase.SaveAssets();
            //}
            if (frameRecorder.cam)
            {
                frameRecorder.cam.orthographicSize = (1 / frameRecorder.recordScale) * FrameRecorder.StandOrthographicSize;
            }
        }

        private void BakeSpriteSheet(List<Texture2D> textures)//, List<ScreenPoint> pivots, string viewName)
        {
            try
            {
                int maxAtlasSize = frameRecorder.MaxSize;
                Texture2D spriteSheet = new Texture2D(maxAtlasSize, maxAtlasSize, TextureFormat.ARGB32, false);

                Rect[] texRects = spriteSheet.PackTextures(textures.ToArray(), 0, maxAtlasSize);

                string pngFilePath = frameRecorder.exportPath + prefabName + ".png";
                SFAUtility.SaveTexture2PNG(spriteSheet, pngFilePath);
                AssetDatabase.Refresh();
                
                // meta 文件
                for (int i = 0; i < texRects.Length; i++)
                {
                    Texture2D tex = textures[i];
                    float newX = texRects[i].x * spriteSheet.width;
                    float newY = texRects[i].y * spriteSheet.height;
                    texRects[i] = new Rect(newX, newY, (float)tex.width, (float)tex.height);
                }
                TextureImporter texImporter = (TextureImporter)AssetImporter.GetAtPath(pngFilePath);
                if (texImporter != null)
                {
                    texImporter.textureType = TextureImporterType.Sprite;
                    texImporter.spriteImportMode = SpriteImportMode.Multiple;
                    texImporter.maxTextureSize = maxAtlasSize;
                    texImporter.wrapMode = TextureWrapMode.Repeat;

                    int texCount = textures.Count;
                    SpriteMetaData[] metaData = new SpriteMetaData[texCount];
                    for (int i = 0; i < texCount; i++)
                    {
                        metaData[i].name = prefabName + i;
                        metaData[i].rect = texRects[i];
                        metaData[i].alignment = (int)SpriteAlignment.Custom;
                        metaData[i].pivot = new Vector2(0.5f,0.5f);
                    }
                    texImporter.spritesheet = metaData;
                    texImporter.SaveAndReimport();

                    AssetDatabase.ImportAsset(pngFilePath);
                }
                AssetDatabase.Refresh();
                SaveSpriteAnimPrefab(frameRecorder.exportPath);
                //SaveSpriteUVAnimPrefab(texRects, frameRecorder.exportPath, "sheet");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void SaveSpriteAnimPrefab(string filePath)
        {
            GameObject canvas = GameObject.Find("Canvas");
            string path = Path.Combine(filePath, prefabName + ".prefab");
            if (File.Exists(path))
                File.Delete(path);
            Transform old = canvas.transform.Find(prefabName);
            if(old)
                DestroyImmediate(old.gameObject);
            GameObject uvOrgPrefab = new GameObject(prefabName);
            uvOrgPrefab.layer = LayerMask.NameToLayer("UI");
            uvOrgPrefab.transform.SetParent(canvas.transform);
            uvOrgPrefab.transform.localPosition = Vector3.zero;
            uvOrgPrefab.transform.localScale = Vector3.one;
            Image img = uvOrgPrefab.AddComponent<Image>();
            SpriteAnim anim = uvOrgPrefab.AddComponent<SpriteAnim>();
            string texPath = frameRecorder.exportPath + prefabName + ".png";
            //这样加载才可以加载所有的Sprite;数组第一个为整张材质贴图
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(texPath);
            Sprite[] sprites = new Sprite[objs.Length - 1];
            for (int i = 0; i < objs.Length - 1; i++)
            {
                Sprite sp = objs[i + 1] as Sprite;
                sprites[i] = sp;
            }
            img.sprite = sprites[0];
            anim.SpriteFrames = new List<Sprite>(sprites);
            PrefabUtility.SaveAsPrefabAssetAndConnect(uvOrgPrefab, path, InteractionMode.AutomatedAction);
            DestroyImmediate(uvOrgPrefab);
        }

        //private void SaveSpriteUVAnimPrefab(Rect[] texRects, string filePath, string fileName)
        //{
        //    GameObject old = GameObject.Find("fileName");
        //    DestroyImmediate(old);
        //    string path = Path.Combine(filePath, fileName + ".prefab");
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    GameObject uvOrgPrefab = new GameObject(fileName);
        //    MeshFilter mf = uvOrgPrefab.AddComponent<MeshFilter>();
        //    MeshRenderer meshRenderer = uvOrgPrefab.AddComponent<MeshRenderer>();
        //    mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(frameRecorder.exportPath + "/quad.mesh");
        //    SpriteUVAnim anim = uvOrgPrefab.AddComponent<SpriteUVAnim>();
        //    anim.spriteRects = SFAUtility.Rects2Vector4Array(texRects);
        //    string matPath = Path.Combine(filePath, fileName + ".mat");
        //    SaveSpriteAnimMaterial(texRects, frameRecorder.exportPath + "/sheet.png", matPath);
        //    meshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        //    PrefabUtility.SaveAsPrefabAsset(uvOrgPrefab, path);
        //}

        private void SaveSpriteAnimMaterial(Rect[] texRects, string texturePath, string matPath)
        {
            if (File.Exists(matPath))
                File.Delete(matPath);
            Material mat = new Material(Shader.Find("SFA/UVAnim"));
            mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            mat.SetFloat("_Speed", 2);
            mat.SetVectorArray("_Rects", SFAUtility.Rects2Vector4Array(texRects));
            AssetDatabase.CreateAsset(mat, SFAUtility.Absolute2Relativity(matPath));
            AssetDatabase.SaveAssets();
        }
    }
}
