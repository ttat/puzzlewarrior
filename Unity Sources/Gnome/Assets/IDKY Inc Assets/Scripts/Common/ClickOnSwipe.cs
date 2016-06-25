// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ClickOnSwipe : MonoBehaviour
{
    #region SwipeDirection enum

    public enum SwipeDirection
    {
        Up,

        Down,

        Left,

        Right
    }

    #endregion

    #region Private Fields

    private bool activated = true;

    #endregion

    #region Public Fields
	
	public bool ActivateOnEnable = true;
	
    public SwipeDirection DirectionToHandle = SwipeDirection.Up;

    #endregion

    #region Private Methods

    private void ActivateClickOnSwipe()
    {
        this.activated = true;
    }

    private void DeactivateClickOnSwipe()
    {
        this.activated = false;
    }
	
	private void OnEnable()
	{
		if (this.ActivateOnEnable)
		{
			this.ActivateClickOnSwipe();
		}
	}
	
    private void OnSwipeDown()
    {
        if (this.DirectionToHandle == SwipeDirection.Down && this.activated)
        {
            this.SendMessage("OnClick");
        }
    }

    private void OnSwipeLeft()
    {
        if (this.DirectionToHandle == SwipeDirection.Left && this.activated)
        {
            this.SendMessage("OnClick");
        }
    }

    private void OnSwipeRight()
    {
        if (this.DirectionToHandle == SwipeDirection.Right && this.activated)
        {
            this.SendMessage("OnClick");
        }
    }

    private void OnSwipeUp()
    {
        if (this.DirectionToHandle == SwipeDirection.Up && this.activated)
        {
            this.SendMessage("OnClick");
        }
    }

    #endregion
}