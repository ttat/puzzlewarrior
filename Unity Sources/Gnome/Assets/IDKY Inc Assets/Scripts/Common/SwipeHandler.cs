// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    #region Fields

    public Vector2 CurrentPosition;

    public Vector2 EndPosition;

    public float HorizontalSwipePercentage = 0.25f;

    public float HorizontalSwipePixels = 45f;

    public Vector2 StartPosition;

    public SwipeTriggerMode TriggerMode = SwipeTriggerMode.Percentage;

    public float VerticalSwipePercentage = 0.25f;

    public float VerticalSwipePixels = 45f;

    private float screenHeight;

    private float screenWidth;

    private int touches;

    #endregion

    #region Enums

    public enum SwipeTriggerMode
    {
        Percentage,

        Pixels
    }

    #endregion

    #region Methods

    private void Awake()
    {
        this.screenWidth = Screen.width;
        this.screenHeight = Screen.height;
    }

    private void TriggerPercentageSwiped(float deltaX, float deltaY)
    {
        float horizontalDeltaPercentage = Mathf.Abs(deltaX / this.screenWidth);
        float verticalDeltaPercentage = Mathf.Abs(deltaY / this.screenHeight);

        // Swiped to the right
        if ((horizontalDeltaPercentage >= this.HorizontalSwipePercentage) && (deltaX >= 0))
        {
            this.SendMessage("OnSwipeRight", SendMessageOptions.DontRequireReceiver);
        }

        // Swiped to the left
        if ((horizontalDeltaPercentage >= this.HorizontalSwipePercentage) && (deltaX < 0))
        {
            this.SendMessage("OnSwipeLeft", SendMessageOptions.DontRequireReceiver);
        }

        // Swiped up
        if ((verticalDeltaPercentage >= this.VerticalSwipePercentage) && (deltaY >= 0))
        {
            this.SendMessage("OnSwipeUp", SendMessageOptions.DontRequireReceiver);
        }

        // Swiped down
        if ((verticalDeltaPercentage >= this.VerticalSwipePercentage) && (deltaY < 0))
        {
            this.SendMessage("OnSwipeDown", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void TriggerPixelSwiped(float deltaX, float deltaY)
    {
        float horizontalDeltaPixels = Mathf.Abs(deltaX);
        float verticalDeltaPixels = Mathf.Abs(deltaY);

        if (horizontalDeltaPixels > verticalDeltaPixels)
        {
            if ((horizontalDeltaPixels >= this.HorizontalSwipePixels) && (deltaX >= 0))
            {
                // Swiped to the right
                this.SendMessage("OnSwipeRight", SendMessageOptions.DontRequireReceiver);
            }

            else if ((horizontalDeltaPixels >= this.HorizontalSwipePixels) && (deltaX < 0))
            {
                // Swiped to the left
                this.SendMessage("OnSwipeLeft", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                this.SendMessage("OnSwipeCanceled", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if ((verticalDeltaPixels >= this.VerticalSwipePixels) && (deltaY >= 0))
            {
                // Swiped up
                this.SendMessage("OnSwipeUp", SendMessageOptions.DontRequireReceiver);
            }
            else if ((verticalDeltaPixels >= this.VerticalSwipePixels) && (deltaY < 0))
            {
                // Swiped down
                this.SendMessage("OnSwipeDown", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                this.SendMessage("OnSwipeCanceled", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void TriggerSwiping(float deltaX, float deltaY)
    {
        float horizontalDeltaPixels = Mathf.Abs(deltaX);
        float verticalDeltaPixels = Mathf.Abs(deltaY);

        if (horizontalDeltaPixels > verticalDeltaPixels)
        {
            // Swiping to the right
            if (deltaX >= 0)
            {
                this.SendMessage("OnSwipingRight", horizontalDeltaPixels, SendMessageOptions.DontRequireReceiver);
            }

            // Swiped to the left
            if (deltaX < 0)
            {
                this.SendMessage("OnSwipingLeft", horizontalDeltaPixels, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            // Swiping up
            if (deltaY >= 0)
            {
                this.SendMessage("OnSwipingUp", verticalDeltaPixels, SendMessageOptions.DontRequireReceiver);
            }

            // Swiping down
            if (deltaY < 0)
            {
                this.SendMessage("OnSwipingDown", verticalDeltaPixels, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void Update()
    {
#if UNITY_ANDROID
        // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
        if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
        {
            return;
        }
#endif

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0) && this.touches == 0)
            {
                // Equivalent to TouchPhase.Begain
                this.touches = 1;

                // Keep track of when the touch was first detected
                this.StartPosition = Input.mousePosition;
                this.CurrentPosition = Input.mousePosition;
            }
            else if (this.touches == 1 && !Input.GetMouseButtonUp(0))
            {
                // Equivalent to TouchPhase.Moved
                Vector2 currentMousePosition = Input.mousePosition;

                // If there is a large enough change
                float xChange = Mathf.Abs(this.CurrentPosition.x - currentMousePosition.x);
                float yChange = Mathf.Abs(this.CurrentPosition.y - currentMousePosition.y);

                if (xChange >= 1 || yChange >= 1)
                {
                    // Keep track of the current touch movement
                    this.CurrentPosition = Input.mousePosition;

                    // Calculate the deltas
                    float currentDeltaX = this.CurrentPosition.x - this.StartPosition.x;
                    float currentDeltaY = this.CurrentPosition.y - this.StartPosition.y;

                    this.TriggerSwiping(currentDeltaX, currentDeltaY);
                }
            }
            else if (this.touches == 1 && Input.GetMouseButtonUp(0))
            {
                // Equivalent to TouchPhase.Ended
                this.touches = 0;

                // Keep track of when the touch was released
                this.EndPosition = Input.mousePosition;

                // Calculate the deltas
                float deltaX = this.EndPosition.x - this.StartPosition.x;
                float deltaY = this.EndPosition.y - this.StartPosition.y;

                if (this.TriggerMode == SwipeTriggerMode.Percentage)
                {
                    this.TriggerPercentageSwiped(deltaX, deltaY);
                }
                else
                {
                    this.TriggerPixelSwiped(deltaX, deltaY);
                }
            }
        }
        else
        {
            this.touches = Input.touchCount;

            if (this.touches > 0)
            {
                switch (Input.touches[0].phase)
                {
                    case TouchPhase.Began:
                        // Keep track of when the touch was first detected
                        this.StartPosition = Input.touches[0].position;
                        this.CurrentPosition = Input.touches[0].position;
                        break;

                    case TouchPhase.Moved:
                        // Keep track of the current touch movement
                        this.CurrentPosition = Input.touches[0].position;

                        // Calculate the deltas
                        float currentDeltaX = this.CurrentPosition.x - this.StartPosition.x;
                        float currentDeltaY = this.CurrentPosition.y - this.StartPosition.y;

                        this.TriggerSwiping(currentDeltaX, currentDeltaY);

                        break;

                    case TouchPhase.Ended:
                        // Keep track of when the touch was released
                        this.EndPosition = Input.touches[0].position;

                        // Calculate the deltas
                        float deltaX = this.EndPosition.x - this.StartPosition.x;
                        float deltaY = this.EndPosition.y - this.StartPosition.y;

                        if (this.TriggerMode == SwipeTriggerMode.Percentage)
                        {
                            this.TriggerPercentageSwiped(deltaX, deltaY);
                        }
                        else
                        {
                            this.TriggerPixelSwiped(deltaX, deltaY);
                        }

                        break;
                }
            }
        }
    }

    #endregion
}