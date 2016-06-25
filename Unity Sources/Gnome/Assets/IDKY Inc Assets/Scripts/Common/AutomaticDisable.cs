// ----------------------------------------------
// 
//             2 Upper
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class AutomaticDisable : MonoBehaviour
{
    #region Fields

    private float endTime;

    private float startTime;

    #endregion

    #region Public Methods and Operators

    public void Enable(float timeBeforeDisable)
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(false);
        }

        this.startTime = Time.time;
        this.endTime = this.startTime + timeBeforeDisable;

        this.gameObject.SetActive(true);
    }

    #endregion

    #region Methods

    private void Update()
    {
        if (Time.time > this.endTime)
        {
            this.gameObject.SetActive(false);
        }
    }

    #endregion
}