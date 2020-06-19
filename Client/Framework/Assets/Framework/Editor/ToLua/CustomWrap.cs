using System;
using BitBenderGames;
using UnityEngine;
using DG.Tweening;
using EasyList;
using Framework;
using PathCreation;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

using BindType = ToLuaMenu.BindType;
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
        _GT(typeof(AudioListener)),
        _GT(typeof(RuntimePlatform)),
        _GT(typeof(DateTime)),
        _GT(typeof(Rect)),
        _GT(typeof(PlayerPrefs)),
        _GT(typeof(LayerMask)),
        _GT(typeof(Touch)),
        _GT(typeof(HideFlags)),
        _GT(typeof(AudioRolloffMode)),
        _GT(typeof(RectTransformUtility)),
        _GT(typeof(RuntimeAnimatorController)),
        _GT(typeof(UnityEngine.SceneManagement.SceneManager)),
        _GT(typeof(UnityEngine.SceneManagement.Scene)),
        _GT(typeof(UnityEngine.EventSystems.PointerEventData)),
        _GT(typeof(UnityEngine.EventSystems.RaycastResult)),
        _GT(typeof(UnityEngine.EventSystems.PointerEventData.InputButton)),
        _GT(typeof(UnityEngine.EventSystems.EventTrigger)),
        _GT(typeof(UnityEngine.EventSystems.EventTrigger.Entry)),
        _GT(typeof(UnityEngine.EventSystems.EventTriggerType)),

        _GT(typeof(AnimatorUpdateMode)),
        _GT(typeof(AnimatorStateInfo)),
        _GT(typeof(TextMesh)),
        _GT(typeof(RawImage)),
        _GT(typeof(VideoPlayer)),
        
        _GT(typeof(SpriteRenderer)),
        _GT(typeof(Texture3D)),
        _GT(typeof(MeshFilter)),
        _GT(typeof(Mesh)),
        //================
        // Packages
        //================
        _GT(typeof(ShadowCastingMode)),
        
        //================
        // UnityEngine.UI
        //================
        //_GT(typeof(RectTransform)),
        //_GT(typeof(Text)),
        _GT(typeof(Image.Type)),
        _GT(typeof(ToggleGroup)),
        _GT(typeof(Canvas)),
        _GT(typeof(Sprite)),
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
        _GT(typeof(UpdateType)),
        //================
        // FrameWork Core
        //================
        _GT(typeof(LuaReflect)),
        _GT(typeof(GlobalConsts)),
        _GT(typeof(ShowFPS)),
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
        _GT(typeof(ReignRequest)),
        _GT(typeof(ReignResponse)),
        //Components
        _GT(typeof(Ticker)),
        _GT(typeof(Hud)),
        _GT(typeof(AroundPoint)),
        _GT(typeof(AroundPosition)),
        _GT(typeof(Chains)),
        _GT(typeof(ClickFeedback)),
        _GT(typeof(AutoMove)),
        _GT(typeof(AutoPath)),
        _GT(typeof(AttachCamera)),
        _GT(typeof(SpriteList)),
        
        //==============
        // Image Renderer
        //==============
        //_GT(typeof(GaussianBlur)),
        
        //================
        // 3rd
        //================
        //Astar
        _GT (typeof(AStar.PathRequestManager)),
        _GT (typeof(AStar.Path)),
        _GT (typeof(AStar.Grid)),
        _GT (typeof(AStar.Node)),
        _GT (typeof(AStar.DebugPoints)),
        //_GT (typeof(VertexEffects.CircleOutline)),
        //_GT (typeof(VertexEffects.BoxOutline)),
        //Live2D
        //_GT (typeof(live2d.Live2D)),
        //_GT (typeof(live2d.framework.Live2DAnimator)),
        //_GT (typeof(live2d.framework.Live2DImage)),
        //FastBehavior
        _GT(typeof(FastBehavior.StateMachine)),
        _GT(typeof(FastBehavior.FastLuaBehavior)),
        _GT(typeof(FastBehavior.StateMachineManager)),

        // Custom Components
        _GT(typeof(ListView)),
        _GT(typeof(ListPositionView)),
        _GT(typeof(ListViewBase)),
        _GT(typeof(LuaListViewAdapter)),
        _GT(typeof(LuaListViewCell)),
        _GT(typeof(ListItemEventListener)),
        
        //Path Creator
        _GT(typeof(PathCreator)),
        _GT(typeof(VertexPath)),
        //_GT(typeof(BezierPath)),
        
        // Mobile Touch Camera
        _GT(typeof(MobileTouchCamera)),
        _GT(typeof(TouchInputController)),
    };
}

