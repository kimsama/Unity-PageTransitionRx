using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SwipeGesture), typeof(HorizontalLayoutGroup))]
public class TurnPage : MonoBehaviour
{
    public float PageWidth = 1080;

    private RectTransform rectTransform;
    private SwipeGesture swipeGesture;
    private Tween moveAnimation;
    private int pageCount;
    private int currentPage = 1;

    void Awake()
    {
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;  // Tween 생성시 자동 실행시키지 
    }

    void OnEnable() 
    { 
        this.rectTransform  =  this.GetComponent<RectTransform>(); 
        this.swipeGesture  =  this.GetComponent<SwipeGesture>(); 
        this.pageCount  =  this.transform.childCount ; 

        // next 
        this . swipeGesture 
            . OnSwipeLeft 
            . Where ( _  =>  currentPage  <  pageCount )  // 최대 페이지 이전 인 경우에만 진행 
            . Where ( _  =>  this.moveAnimation  ==  null  ||  ! this.moveAnimation.IsPlaying ())  // 애니메이션 실행 중이 아니 
            . Subscribe ( _  => 
            { 
                this.currentPage ++; 
                this.moveAnimation  =  this.rectTransform 
                    .DOAnchorPosX ( rectTransform.anchoredPosition.x  -  this.PageWidth,  1.0f ) 
                    .SetEase ( Ease.OutBounce ) 
                    .Play (); 
            }); 

        // back 
        this . swipeGesture 
            . OnSwipeRight 
            . Where ( _  =>  currentPage  >  1 )  // 1 페이지 이상인 경우에만 돌아갈 
            . Where ( _  =>  this.moveAnimation == null || !this.moveAnimation.IsPlaying())  // 애니메이션 실행 중이 없다 
            . Subscribe ( _  => 
            { 
                this.currentPage--; 
                this.moveAnimation = this.rectTransform 
                    .DOAnchorPosX (rectTransform.anchoredPosition.x  +  this.PageWidth, 1.0f) 
                    .SetEase (Ease.OutBounce) 
                    .Play(); 
            } ); 
    }
}