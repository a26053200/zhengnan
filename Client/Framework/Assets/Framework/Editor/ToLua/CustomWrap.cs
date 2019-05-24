using System;
using UnityEngine;
using System.Collections.Generic;

using BindType = ToLuaMenu.BindType;
using Framework;
using UnityEngine.UI;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/11 0:51:47</para>
/// </summary> 
public static class CustomWrap
{
    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static BindType[] typeList =
    {
        //================
        // UnityEngine
        //================
        _GT(typeof(DateTime)),
        _GT(typeof(Rect)),
        _GT(typeof(PlayerPrefs)),
        _GT(typeof(Touch)),
        _GT(typeof(AudioRolloffMode)),
        _GT(typeof(UnityEngine.SceneManagement.SceneManager)),
        _GT(typeof(UnityEngine.SceneManagement.Scene)),
        _GT(typeof(UnityEngine.EventSystems.PointerEventData)),
        _GT(typeof(UnityEngine.EventSystems.PointerEventData.InputButton)),
        _GT(typeof(UnityEngine.EventSystems.EventTrigger)),
        _GT(typeof(UnityEngine.EventSystems.EventTrigger.Entry)),
        _GT(typeof(UnityEngine.EventSystems.EventTriggerType)),
        //================
        // UnityEngine.UI
        //================
        //_GT(typeof(RectTransform)),
        //_GT(typeof(Text)),
        //_GT(typeof(Image)),
        //_GT(typeof(Slider)),
        //_GT(typeof(CanvasGroup)),
        _GT(typeof(InputField)),
        _GT(typeof(ScrollRect)),
        _GT(typeof(Button)),
        _GT(typeof(Toggle)),
        _GT(typeof(ContentSizeFitter)),
        _GT(typeof(ContentSizeFitter.FitMode)),
        //================
        // DoTween
        //================
        _GT(typeof(RectTransform)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
        _GT(typeof(Image)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
        _GT(typeof(Text)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
        _GT(typeof(Slider)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
        _GT(typeof(CanvasGroup)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions46)),
        //================
        // FrameWork Core
        //================
        _GT(typeof(EventTriggerListener)),
        _GT(typeof(Logger)),
        _GT(typeof(AssetsManager)),
        _GT(typeof(SceneManager)),
        _GT(typeof(GameManager)),
        _GT(typeof(MonoBehaviourManager)),
        _GT(typeof(NetworkManager)),
        _GT(typeof(LuaHelper)),
        _GT(typeof(LuaMonoBehaviour)),
        _GT(typeof(StringUtils)),
        _GT(typeof(SystemUtils)),
        
        //================
        // 3rd
        //================
        _GT(typeof(AStar.Grid)),
        _GT(typeof(AStar.Path)),
        _GT(typeof(AStar.Node)),
        _GT(typeof(AStar.PathRequestManager)),

        /// Custom Components
        _GT(typeof(ListView)),
        _GT(typeof(LuaListViewAdapter)),
        _GT(typeof(LuaListViewCell)),
    };
}

