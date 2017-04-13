using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SwipeGesture : MonoBehaviour
{
    public float TotalSeconds = 1.0f;
    public float ThresholdSenconds = 1.0f;
    public float ThresholdDistance = 100.0f;

    private Subject<Unit> onSwipeLeft = new Subject<Unit>();
    public IObservable<Unit> OnSwipeLeft
    {
        get { return onSwipeLeft; }
    }

    private Subject<Unit> onSwipeRight = new Subject<Unit>();
    public IObservable<Unit> OnSwipeRight
    {
        get { return onSwipeRight; }
    }

    private Subject<Unit> onSwipeDown = new Subject<Unit>();
    public IObservable<Unit> OnSwipeDown
    {
        get { return onSwipeDown; }
    }

    private Subject<Unit> onSwipeUp = new Subject<Unit>();
    public IObservable<Unit> OnSwipeUp
    {
        get { return onSwipeUp; }
    }

    private Vector2 beginPosition;
    private DateTime beginTime;

    void OnEnable() 
    { 
        var  eventTrigger = this.gameObject.AddComponent < ObservableEventTrigger > (); 

        eventTrigger 
            . OnBeginDragAsObservable () 
            . TakeUntilDisable ( this ) 
            . Where ( eventData => eventData.pointerDrag.gameObject == this.gameObject ) 
            . Select ( eventData => eventData . position ) 
            . Subscribe ( position => 
            { 
                this . beginPosition =  position ; 
                this . beginTime  =  DateTime.Now ; 
            }); 

        var  onEndDragObservable  =  eventTrigger 
            . OnEndDragAsObservable () 
            . TakeUntilDisable ( this ) 
            . Where ( eventData => (DateTime.Now - this.beginTime).TotalSeconds < this.ThresholdSenconds ) 
            . Select ( eventData  => eventData.position ) 
            . Share (); 

        // left 
        onEndDragObservable 
            . Where ( position  =>  beginPosition.x  >  position.x ) 
            . Where ( position  =>  Mathf . Abs ( beginPosition . x  -  position.x ) >= this.ThresholdDistance ) 
            . Subscribe ( _  =>  onSwipeLeft . OnNext ( Unit.Default )) ; 

        // right 
        onEndDragObservable 
            . Where ( position => position.x  >  beginPosition . x ) 
            . Where ( position => Mathf . Abs ( position . x  -  beginPosition.x ) >= this.ThresholdDistance ) 
            . Subscribe ( _  => onSwipeRight . OnNext ( Unit.Default )); 

        // down 
        onEndDragObservable 
            . Where ( position => beginPosition.y > position.y ) 
            . Where ( position => Mathf.Abs (beginPosition.y  -  position.y ) >= this.ThresholdDistance ) 
            . Subscribe ( _ => onSwipeDown.OnNext (Unit.Default)); 

        // up 
        onEndDragObservable 
            . Where ( position => position.y > beginPosition.y ) 
            . Where ( position => Mathf . Abs ( position.y - beginPosition.y ) >=  this.ThresholdDistance ) 
            . Subscribe ( _ => onSwipeUp.OnNext (Unit.Default)); 

    }
}