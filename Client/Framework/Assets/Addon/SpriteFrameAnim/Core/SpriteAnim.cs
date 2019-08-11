#if USE_SFA_LUA
using LuaInterface;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SFA
{
    [RequireComponent(typeof(Image))]
    public class SpriteAnim : MonoBehaviour
    {
#if USE_SFA_LUA
        private LuaFunction completeAction;
#endif
        private Image ImageSource;
        private int mCurrFrame = 0;
        private float mDelta = 0;

        public Action overHandler;
        public List<Sprite> SpriteFrames;
        public float FPS = 15;
        public bool IsPlaying = false;
        public bool Foward = true;
        public bool AutoPlay = true;
        public bool Loop = true;
        public bool ReturnStartFrame = false;
        public bool AutoSize = true;

        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        public int CurrFrame
        {
            get
            {
                return mCurrFrame;
            }
        }
#if USE_SFA_LUA
        public void OnComplete(LuaFunction func)
        {
            completeAction = func;
        }
#endif
        public void Awake()
        {
            ImageSource = GetComponent<Image>();
        
#if UNITY_EDITOR
        
            EditorApplication.update -= OnEditUpdate;
            EditorApplication.update += OnEditUpdate;
#endif
        }

        void Start()
        {
            if (AutoPlay)
            {
                if (Foward)
                {
                    Play();
                }
                else
                {
                    PlayReverse();
                }
            }
        }

        public void SetSprite(int idx)
        {
            ImageSource.sprite = SpriteFrames[idx];
            if(AutoSize)
                ImageSource.SetNativeSize();
        }

        public void Play()
        {
            IsPlaying = true;
            Foward = true;
        }

        public void PlayReverse()
        {
            IsPlaying = true;
            Foward = false;
        }


        public void Update()
        {
            if (!IsPlaying || 0 == FrameCount)
            {
                return;
            }
#if UNITY_EDITOR
            mDelta += Time.fixedDeltaTime;
#else
            mDelta += Time.deltaTime;
#endif

            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                if (Foward)
                {
                    mCurrFrame++;
                }
                else
                {
                    mCurrFrame--;
                }

                if (mCurrFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        mCurrFrame = 0;
                    }
                    else
                    {
                        IsPlaying = false;
                        EndAction();
                        if (ReturnStartFrame)
                        {
                            mCurrFrame = 0;
                        }
                        else
                        {
                            return;
                        }

                    }
                }
                else if (mCurrFrame < 0)
                {
                    if (Loop)
                    {
                        mCurrFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }

                SetSprite(mCurrFrame);
            }
        }

        private void EndAction()
        {
            if (overHandler != null)
                overHandler();
#if USE_SFA_LUA
                ///lua结束回调
            if (completeAction != null)
            {
                completeAction.BeginPCall();
                completeAction.PCall();
                completeAction.EndPCall();
                completeAction.Dispose();
            }
#endif
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            mCurrFrame = 0;
            SetSprite(mCurrFrame);
            IsPlaying = false;
        }

        public void Rewind()
        {
            mCurrFrame = 0;
            SetSprite(mCurrFrame);
            Play();
        }

        void OnDisable()
        {
            if (ReturnStartFrame)
            {
                mCurrFrame = 0;
                SetSprite(mCurrFrame);
            }
        }
#if UNITY_EDITOR
        void OnEditUpdate()
        {
            if (!this)
                return;
            Update();
            EditorUtility.SetDirty(gameObject);
            SceneView.RepaintAll();
            if (SceneView.lastActiveSceneView)
                SceneView.lastActiveSceneView.Repaint();
        }
        void OnDestroy()
        {
            EditorApplication.update -= OnEditUpdate;
        }
        void OnApplicationQuit()
        {
            EditorApplication.update -= OnEditUpdate;
        }
#endif
    }
}
