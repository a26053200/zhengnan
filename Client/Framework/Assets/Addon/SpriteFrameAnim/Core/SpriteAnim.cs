using DG.Tweening;
using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SFA
{
    [RequireComponent(typeof(Image))]
    public class SpriteAnim : MonoBehaviour
    {
        private LuaFunction completeAction;
        private Image ImageSource;
        private int mCurrFrame = 0;
        private float mDelta = 0;

        [SerializeField]
        public List<Sprite> SpriteFrames;
        public float FPS = 15;
        public bool IsPlaying = false;
        public bool Foward = true;
        public bool AutoPlay = true;
        public bool Loop = true;
        public bool ReturnStartFrame = false;
        public bool AutoSize = true;
        [HideInInspector]public float FadeInTime = 0.0f;
        [HideInInspector]public float FadeOutTime = 0.0f;

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

        public void OnComplete(LuaFunction func)
        {
            completeAction = func;
        }

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
            ///淡入效果
            if (FadeInTime > 0)
            {
                Color color = ImageSource.color;
                color.a = 0;
                ImageSource.color = color;
                ImageSource.DOFade(1, FadeInTime).OnComplete(delegate {
                    IsPlaying = true;
                    Foward = true;
                });
            }
            else
            {
                IsPlaying = true;
                Foward = true;
            }
        }

        public void PlayReverse()
        {
            IsPlaying = true;
            Foward = false;
        }

        
        void Update()
        {
            if (!IsPlaying || 0 == FrameCount)
            {
                return;
            }

            mDelta += Time.deltaTime;
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
                        ///淡出效果
                        if (FadeOutTime > 0)
                        {
                            ImageSource.DOFade(0, FadeOutTime).OnComplete(delegate {
                                EndAction();
                            });
                        }
                        else
                        {
                            EndAction();
                        }
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
            ///lua结束回调
            if (completeAction != null)
            {
                completeAction.Call();
                completeAction.Dispose();
            }
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
#if UNITY_EDITOR
            EditorApplication.update -= OnEditUpdate;
#endif
        }
#if UNITY_EDITOR
        void OnEditUpdate()
        {
            if(!Application.isPlaying)
                Update();
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
