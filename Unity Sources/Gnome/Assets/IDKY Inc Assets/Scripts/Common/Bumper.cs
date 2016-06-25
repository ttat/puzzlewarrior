// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class Bumper : MonoBehaviour
{
    #region Fields

    public float Force = 500f;

    public float NormalizedMass = 10f;

    #endregion

    #region Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ragdoll")
        {
            Rigidbody rb = collision.gameObject.rigidbody;

            if (rb != null)
            {
                foreach (ContactPoint point in collision.contacts)
                {
                    float massNormalized = rb.mass / this.NormalizedMass;
                    rb.AddForce(-1 * point.normal * this.Force * massNormalized, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ragdoll")
        {
            Rigidbody rb = collision.gameObject.rigidbody;

            if (rb != null)
            {
                foreach (ContactPoint point in collision.contacts)
                {
                    float massNormalized = rb.mass / this.NormalizedMass;
                    rb.AddForce(-1 * point.normal * this.Force * massNormalized, ForceMode.Impulse);
                }
            }
        }
    }

    #endregion
}