// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;
using UnityEngine;

public class BounceTween : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The amount of time between each peak/null
    /// </summary>
    public float BounceDuration = 0.3f;

    public bool Loop = true;

    /// <summary>
    /// The peak and null scaling
    /// </summary>
    public float[] PeaksAndNulls = new[] {1.1f, .95f, 1.05f, .95f, 1.1f};

    #endregion

    #region Private Methods

    private IEnumerator DoBounce()
    {
        do
        {
            float original = 1f;

            foreach (float scale in this.PeaksAndNulls)
            {
                this.DoScaling(scale);

                original *= scale;

                yield return new WaitForSeconds(this.BounceDuration);
            }

            // Get it back to the original size
            this.DoScaling(1/original);

            yield return new WaitForSeconds(this.BounceDuration);
        } while (this.Loop);
    }

    private void DoScaling(float scale)
    {
        TweenScale.Begin(
            this.gameObject,
            this.BounceDuration,
            Vector3.Scale(this.gameObject.transform.localScale, new Vector3(scale, scale, scale)));
    }

    private void Start()
    {
        this.StartCoroutine("DoBounce");
    }

    #endregion
}