using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace SFA
{
    public class FrameRecorder : MonoBehaviour
    {
        public const float StandOrthographicSize = 7;

        public Camera cam;
        public ImageFormat imageFormat = ImageFormat.PNG;
        public int MaxSize = 2048;
        public Vector2Int resolution = new Vector2Int(2048, 2048);
        public Vector2Int cutSize = new Vector2Int(128,128);
        public int FPS = 30;
        public int frameCount = 10;
        [Range(0.01f, 10)]
        public float recordScale = 1;

        //导出路径
        [HideInInspector]public string exportPath;

        int rangeStart;
        int rangeEnd;
        int currectFrame;

        void Awake()
        {
            if (!cam)
                cam = Camera.main;
        }
        void OnEnable ()
        {
            if (!cam)
                cam = Camera.main;
        }
        // Use this for initialization
        void Start()
        {
            if (!cam)
                cam = Camera.main;
            Time.captureFramerate = FPS;
        }
        private void Update()
        {
            if (cam)
            {
                cam.orthographicSize = (1 / recordScale) * StandOrthographicSize;
            }
        }

#if UNITY_EDITOR
        public void StartRecord(UnityAction<List<Texture2D>> callback)
        {
            StartCoroutine(DoStartRecordCo(callback));
        }

        IEnumerator DoStartRecordCo(UnityAction<List<Texture2D>> callback)
        {
            List<Texture2D> textures = new List<Texture2D>();
            if (!cam)
                cam = Camera.main;
            float orgOrthographicSize = cam.orthographicSize;
            cam.orthographicSize = (1 / recordScale) * StandOrthographicSize;
            for (int i = 0; i < frameCount; i++)
            {
                EditorUtility.DisplayProgressBar("Recording", 
                    string.Format("{0}/{1}",i,frameCount), 
                    (float)i / (float)frameCount);
                yield return new WaitForEndOfFrame();
                Texture2D tex = RecordFrame(i);
                textures.Add(tex);
            }
            cam.orthographicSize = orgOrthographicSize;
            EditorUtility.ClearProgressBar();
            callback(textures);
        }

        public Texture2D RecordFrame(int i)
        {
            RenderTexture rt = new RenderTexture(resolution.x, resolution.y, 24);
            cam.targetTexture = rt;
            Texture2D tex = SFAUtility.CutoutTexture(cam, resolution, cutSize, exportPath);
            //SFAUtility.SaveTexture2PNG(tex, string.Format(exportPath + "/{0}_{1}.{2}", gameObject.name, i, imageFormat.ToString().ToLower()));
            cam.targetTexture = null;
            GameObject.DestroyImmediate(rt);
            return tex;
        }
#endif
    }
}
