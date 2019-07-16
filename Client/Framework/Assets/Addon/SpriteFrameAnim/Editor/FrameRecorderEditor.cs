using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace SFA
{
    [CustomEditor(typeof(FrameRecorder))]
    public class FrameRecorderEditor : Editor
    {

        FrameRecorder frameRecorder;
        List<Texture2D> textures;

        private void OnEnable()
        {
            frameRecorder = target as FrameRecorder;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SFAUtility.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Output Path:");
            GUIStyle style01 = new GUIStyle("button");
            style01.fixedWidth = 30;
            if (GUILayout.Button("...", style01))
            {
                frameRecorder.exportPath = EditorUtility.OpenFolderPanel("", "", "");
            }

            GUIStyle style02 = new GUIStyle("label");
            style02.fontStyle = FontStyle.Italic;
            EditorGUILayout.LabelField(frameRecorder.exportPath, style02);

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Record"))
            {
                frameRecorder.StartRecord(BakeSpriteSheet);
            }
                
            if (GUILayout.Button("Open Export Folder"))
            {
                Application.OpenURL("file://" + frameRecorder.exportPath);
            }

            if (GUILayout.Button("Create Quad Mesh"))
            {
                GameObject quad = GameObject.Find("Quad");
                MeshFilter mf = quad.GetComponent<MeshFilter>();
                Mesh mesh = Instantiate(mf.sharedMesh) as Mesh;
                AssetDatabase.CreateAsset(mesh, SFAUtility.Absolute2Relativity(frameRecorder.exportPath + "/quad.mesh"));
                AssetDatabase.SaveAssets();
            }
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

                SFAUtility.SaveTexture2PNG(spriteSheet, frameRecorder.exportPath + "/sheet.png");

                SaveSpriteUVAnimPrefab(texRects, frameRecorder.exportPath, "sheet");

                AssetDatabase.Refresh();
                return;
                // meta 文件
                //for (int i = 0; i < texRects.Length; i++)
                //{
                //    Texture2D tex = textures[i];
                //    float newX = texRects[i].x * spriteSheet.width;
                //    float newY = texRects[i].y * spriteSheet.height;
                //    texRects[i] = new Rect(newX, newY, (float)tex.width, (float)tex.height);
                //}

                //string filePath = Path.Combine(frameRecorder.exportPath, "/sheet.png");
                //int assetIndex = filePath.IndexOf("Assets");
                //if (assetIndex < 0)
                //    return;
                //filePath = filePath.Substring(assetIndex, filePath.Length - assetIndex);

                //TextureImporter texImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
                //if (texImporter != null)
                //{
                //    texImporter.textureType = TextureImporterType.Sprite;
                //    texImporter.spriteImportMode = SpriteImportMode.Multiple;
                //    texImporter.maxTextureSize = maxAtlasSize;

                //    int texCount = textures.Count;
                //    SpriteMetaData[] metaData = new SpriteMetaData[texCount];
                //    for (int i = 0; i < texCount; i++)
                //    {
                //        metaData[i].name = "sheet" + i;
                //        metaData[i].rect = texRects[i];
                //        metaData[i].alignment = (int)SpriteAlignment.Custom;
                //        metaData[i].pivot = new Vector2((float)pivots[i].x / (float)textures[i].width,
                //                                        (float)pivots[i].y / (float)textures[i].height);
                //    }
                //    texImporter.spritesheet = metaData;

                //    AssetDatabase.ImportAsset(filePath);
                //}
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        private void SaveSpriteUVAnimPrefab(Rect[] texRects, string filePath, string fileName)
        {
            GameObject old = GameObject.Find("fileName");
            DestroyImmediate(old);
            string path = Path.Combine(filePath, fileName + ".prefab");
            if (File.Exists(path))
                File.Delete(path);
            GameObject uvOrgPrefab = new GameObject(fileName);
            MeshFilter mf = uvOrgPrefab.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = uvOrgPrefab.AddComponent<MeshRenderer>();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(SFAUtility.Absolute2Relativity(frameRecorder.exportPath + "/quad.mesh"));
            SpriteUVAnim anim = uvOrgPrefab.AddComponent<SpriteUVAnim>();
            anim.spriteRects = SFAUtility.Rects2Vector4Array(texRects);
            string matPath = Path.Combine(filePath, fileName + ".mat");
            SaveSpriteAnimMaterial(texRects, frameRecorder.exportPath + "/sheet.png", matPath);
            meshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(SFAUtility.Absolute2Relativity(matPath));
            //PrefabUtility.SaveAsPrefabAsset(uvOrgPrefab, path);
        }

        private void SaveSpriteAnimMaterial(Rect[] texRects, string texturePath, string matPath)
        {
            if (File.Exists(matPath))
                File.Delete(matPath);
            Material mat = new Material(Shader.Find("SFA/UVAnim"));
            mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(SFAUtility.Absolute2Relativity(texturePath));
            mat.SetFloat("_Speed", 2);
            mat.SetVectorArray("_Rects", SFAUtility.Rects2Vector4Array(texRects));
            AssetDatabase.CreateAsset(mat, SFAUtility.Absolute2Relativity(matPath));
            AssetDatabase.SaveAssets();
        }
    }
}
