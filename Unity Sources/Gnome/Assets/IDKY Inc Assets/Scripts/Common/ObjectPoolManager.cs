// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    #region Private Fields

    private Dictionary<string, Queue<GameObject>> pooledObjects;

    #endregion

    #region Public Fields

    /// <summary>
    /// A count of how many errors there were creating objects.
    /// </summary>
    public int ErrorsCreating;

    /// <summary>
    /// A count of how many times an object didn't have enough in the pool to provide for the call.
    /// </summary>
    public int NotEnoughObjectsCount;

    /// <summary>
    /// A count of how many objects were requested that didn't have an object created for it.
    /// </summary>
    public int ObjectNotAddedCount;

    /// <summary>
    /// Predefined objects to be created in the pool.
    /// </summary>
    public PooledGameObject[] RequestedObjects;

    /// <summary>
    /// A count of how many objects were returned but the key didn't exist.
    /// </summary>
    public int ReturnObjectNotFound;

    #endregion

    #region Public Methods

    public GameObject GetGameObject(string name)
    {
        GameObject obj = null;

        if (this.pooledObjects.ContainsKey(name))
        {
            Queue<GameObject> availableObjects = this.pooledObjects[name];

            lock (availableObjects)
            {
                // Dequeue if it's available
                if (availableObjects.Count > 0)
                {
                    obj = availableObjects.Dequeue();
                }
            }

            // There wasn't any available in the queue, so create a new one
            if (obj == null)
            {
                // Instantiate a new one
                obj = this.InstantiateNewGameObject(name);
            }

            obj.SetActive(true);
        }
        else
        {
            this.ObjectNotAddedCount++;
        }

        return obj;
    }

    public GameObject GetGameObject(string name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = this.GetGameObject(name);

        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }

        return obj;
    }

    public GameObject GetGameObject(string name, Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject obj = this.GetGameObject(name);

        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = parent;
        }

        return obj;
    }

    public void ReturnGameObject(string name, GameObject obj)
    {
        // Reset the position and hide
        obj.SetActive(false);
        obj.transform.position = new Vector3(0, 0, 0);
        obj.transform.rotation = new Quaternion(0, 0, 0, 0);
        obj.transform.parent = this.transform;

        if (this.pooledObjects.ContainsKey(name))
        {
            // Put it back into the queue
            Queue<GameObject> availableObjects = this.pooledObjects[name];

            lock (availableObjects)
            {
                availableObjects.Enqueue(obj);
            }
        }
        else
        {
            this.ReturnObjectNotFound++;
        }
    }

    public void ReturnGameObject(string name, GameObject obj, float waitForSeconds)
    {
        DelayedReturnObjectWrapper wrapper = new DelayedReturnObjectWrapper(name, obj, waitForSeconds);

        this.StartCoroutine("DelayedReturnGameObject", wrapper);
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        this.pooledObjects = new Dictionary<string, Queue<GameObject>>();

        // Instantiate object pool
        foreach (PooledGameObject request in this.RequestedObjects)
        {
            Queue<GameObject> objects = new Queue<GameObject>();

            try
            {
                for (int i = 0; i < request.NumberOfObjects; i++)
                {
                    GameObject obj = (GameObject) Instantiate(request.PooledObject);
                    obj.SetActive(false);
                    obj.transform.parent = this.transform;
                    obj.name = request.Name;
                    objects.Enqueue(obj);
                }
            }
            catch (Exception ex)
            {
                // Move on to the next object if there's an error
                this.ErrorsCreating++;
                continue;
            }

            this.pooledObjects[request.Name] = objects;
        }
    }

    private IEnumerator DelayedReturnGameObject(object param)
    {
        DelayedReturnObjectWrapper wrapper = (DelayedReturnObjectWrapper) param;

        yield return new WaitForSeconds(wrapper.Seconds);

		// Only return the object if it's still active
		if (wrapper.Obj.activeInHierarchy)
		{
        	// Reset the position and hide		
        	wrapper.Obj.SetActive(false);
        	wrapper.Obj.transform.position = new Vector3(0, 0, 0);
        	wrapper.Obj.transform.rotation = new Quaternion(0, 0, 0, 0);
        	wrapper.Obj.transform.parent = this.transform;

			if (wrapper.Obj.rigidbody != null)
			{
				wrapper.Obj.rigidbody.velocity = new Vector3();
			}
			
        	if (this.pooledObjects.ContainsKey(wrapper.Name))
        	{
        	    // Put it back into the queue
        	    Queue<GameObject> availableObjects = this.pooledObjects[wrapper.Name];
			
        	    lock (availableObjects)
        	    {
        	        availableObjects.Enqueue(wrapper.Obj);
        	    }
        	}
        	else
        	{
            	this.ReturnObjectNotFound++;
			}
		}
    }

    private GameObject InstantiateNewGameObject(string name)
    {
        GameObject obj = null;
        this.NotEnoughObjectsCount++;

        try
        {
            for (int i = 0; i < this.RequestedObjects.Length; i++)
            {
                if (this.RequestedObjects[i].Name.Equals(name))
                {
                    obj = (GameObject) Instantiate(this.RequestedObjects[i].PooledObject);
                    obj.SetActive(false);
                    obj.transform.parent = this.transform;
                    obj.name = this.RequestedObjects[i].Name;
                }
            }
        }
        catch (Exception ex)
        {
            this.ErrorsCreating++;
        }

        return obj;
    }

    #endregion
}

[Serializable]
public class PooledGameObject
{
    #region Public Fields

    public string Name;

    public int NumberOfObjects;

    public GameObject PooledObject;

    #endregion

    #region Constructors

    public PooledGameObject(string name, GameObject pooledObject, int NumberOfObjects)
    {
        this.Name = name;
        this.PooledObject = pooledObject;
    }

    #endregion
}

public class DelayedReturnObjectWrapper
{
    #region Constructors

    public DelayedReturnObjectWrapper(string name, GameObject obj, float seconds)
    {
        this.Name = name;
        this.Obj = obj;
        this.Seconds = seconds;
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    public GameObject Obj { get; set; }

    public float Seconds { get; set; }

    #endregion
}