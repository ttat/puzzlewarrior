// ----------------------------------------------
// 
//             2 Upper
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using Soomla.Store;

using UnityEngine;

public class StoreFront : MonoBehaviour
{
    #region Fields

    private bool enabledIab;

    private bool started;

    #endregion
    
    #region Methods

#if UNITY_ANDROID && !UNITY_EDITOR

    private void OnDisable()
    {
        // Stop Iab Service
        if (this.started && this.enabledIab)
        {
            SoomlaStore.StartIabServiceInBg();
            this.enabledIab = false;
        }
    }

    private void OnEnable()
    {
        // Start Iab Service
        if (this.started)
        {
            SoomlaStore.StopIabServiceInBg();
            this.enabledIab = true;
        }
    }

#endif

    private void Start()
    {
        this.StartCoroutine(this.Started());
    }

    private IEnumerator Started()
    {
        yield return new WaitForSeconds(1);

        this.started = true;
    }

    #endregion
}