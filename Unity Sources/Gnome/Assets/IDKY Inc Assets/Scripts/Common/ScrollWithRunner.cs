// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ScrollWithRunner : MonoBehaviour
{
    #region Private Fields

    private bool follow;

    private float targetLastPositionX;

    private float targetLastPositionY;

    #endregion

    #region Public Fields

    /// <summary>
    /// The ratio between the target's movement and this object's movement.
    /// </summary>
    public float MoveRatio = 0.005f;

    /// <summary>
    /// When translating the X coordinate, reverse the value (i.e. Target moves +1, this object will move -.005)
    /// </summary>
    public bool ReverseXDirection = true;

    /// <summary>
    /// When translating the Y coordinate, reverse the value (i.e. Target moves +1, this object will move -.005)
    /// </summary>
    public bool ReverseYDirection = true;

    /// <summary>
    /// The target to follow.
    /// </summary>
    public GameObject Target;

    #endregion

    #region Public Methods

    public void StopFollowing()
    {
        this.follow = false;
    }

    #endregion

    #region Private Methods

    private void LateUpdate()
    {
        if (this.Target != null && this.follow)
        {
            float diffX = (this.Target.transform.position.x - this.targetLastPositionX) * this.MoveRatio;
            float diffY = (this.Target.transform.position.y - this.targetLastPositionY) * this.MoveRatio;
            this.targetLastPositionX = this.Target.transform.position.x;
            this.targetLastPositionY = this.Target.transform.position.y;

            if (this.ReverseXDirection)
            {
                diffX = -diffX;
            }

            if (this.ReverseYDirection)
            {
                diffY = -diffY;
            }

            this.transform.Translate(new Vector3(diffX, diffY, 0));
        }
    }

    private void Start()
    {
        this.targetLastPositionX = this.Target.transform.position.x;
        this.targetLastPositionY = this.Target.transform.position.y;
        this.follow = true;
    }

    #endregion
}