using DG.Tweening;
using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SFA
{
    [RequireComponent(typeof(Image))]
    public class SpriteAnim : MonoBehaviour
    {
        
        private Image ImageSource;
        private int mCurFrame = 0;
        private float mDelta = 0;

        public float FPS = 5;
        public List<Sprite> SpriteFrames;
        public bool IsPlaying = false;
        public bool Foward = true;
        public bool AutoPlay = false;
        public bool Loop = false;
        public bool ReturnStartFrame = false;
        public float FadeInTime = 0.0f;
        public float FadeOutTime = 0.0f;

        private LuaFunction completeAction;
        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        public void OnComplete(LuaFunction func)
        {
            completeAction = func;
        }

        void Awake()
        {
            ImageSource = GetComponent<Image>();
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

        private void SetSprite(int idx)
        {
            ImageSource.sprite = SpriteFrames[idx];
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
                    mCurFrame++;
                }
                else
                {
                    mCurFrame--;
                }

                if (mCurFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        mCurFrame = 0;
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
                            mCurFrame = 0;
                        }
                        else
                        {
                            return;
                        }

                    }
                }
                else if (mCurFrame < 0)
                {
                    if (Loop)
                    {
                        mCurFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }

                SetSprite(mCurFrame);
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
            mCurFrame = 0;
            SetSprite(mCurFrame);
            IsPlaying = false;
        }

        public void Rewind()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            Play();
        }

        void OnDisable()
        {
            if (ReturnStartFrame)
            {
                mCurFrame = 0;
                SetSprite(mCurFrame);
            }
        }
    }
}
