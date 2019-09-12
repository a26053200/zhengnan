using UnityEngine;

namespace Framework
{
    public class ShowFPS : MonoBehaviour
    {
        //系统最大帧频    
        private float _maxFrameRate;

        //更新的时间间隔
        private float _updateInterval = 0.5F;

        //最后的时间间隔
        private float _lastInterval;

        //帧[中间变量 辅助]
        private int _frames = 0;

        //当前的帧率
        private float _fps;

        private GUIStyle _guiStyle;

        void Start()
        {
            //Application.targetFrameRate=60;
            _maxFrameRate = Application.targetFrameRate;
            _updateInterval = Time.realtimeSinceStartup;
            _frames = 0;
            _guiStyle = new GUIStyle();
            _guiStyle.normal.textColor = Color.red;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(4, 4, 200, 200), string.Format("fps:{0}/{1}", _fps.ToString("f2"), _maxFrameRate),
                _guiStyle);
        }

        void Update()
        {
            ++_frames;
            if (Time.realtimeSinceStartup > _lastInterval + _updateInterval)
            {
                _fps = _frames / (Time.realtimeSinceStartup - _lastInterval);
                _frames = 0;
                _lastInterval = Time.realtimeSinceStartup;
            }
        }
    }
}