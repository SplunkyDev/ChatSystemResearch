using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will follow service locator pattern. This is a singleton class that acts as a container.   
/// This will contain the instances and can be accessed by the type registered. 
/// The life cycle is dependant on unity life cycle as its a monobehaviour
/// </summary>
public class DependencyContainer : MonoBehaviour
{
    #region Public fields
    public static DependencyContainer instance;
    #endregion

    #region Private fields
    private Dictionary<Type, object> m_dicContainer = new Dictionary<Type, object>();
    #endregion
    
    #region Instance reference
    #endregion
        
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject.transform.parent);
            return;
        }
        
        DontDestroyOnLoad(gameObject.transform.parent);
        instance = this;
        InitializeDependencies();
    }

    private void OnDestroy()
    {
       //TODO: Clean up self here, check if this can be handled better
       Type[] arrKey = new Type[m_dicContainer.Count];
       int count = 0;
       foreach (var item in m_dicContainer)
       {
           //Caching the key into an array
           arrKey[count] = item.Key;
           count++;
       }

       count = 0;
       while (m_dicContainer.Count > 0)
       {
           //using the key array to get the dat from dictionary 
           if(m_dicContainer[arrKey[count]] != null)
           {
               Debug.Log($"The current type being disposed {m_dicContainer[arrKey[count]]?.GetType()}");
               IDisposable disposable = m_dicContainer[arrKey[count]] as IDisposable;
               disposable?.Dispose();
           }
           m_dicContainer.Remove(arrKey[count]);
           count++;
       }
    }
    
    private void InitializeDependencies()
    {
        //Instantiate and cache instance to container here.
        //Can manage lifecycle of other instances 
    }
    
    /// <summary>
    /// Add the type and instance to the container
    /// </summary>
    /// <param name="a_instance">This is the instance of the object</param>
    /// <typeparam name="T">This is the type of the object</typeparam>
    /// <returns> If it has been successfully added will return true, else false</returns>
    public bool RegisterToContainer<T>(object a_instance) where T : class
    {
        
        if(!(a_instance is T))
        {
            Debug.LogError($"Registering {typeof(T)} but instance is {(a_instance.GetType())}");
            return false;
        }
        
        Type t = typeof(T);
        
        if (m_dicContainer.ContainsKey(t))
        {
            Debug.LogWarning($"{t} type has already been added");
            return false;
        }

        m_dicContainer.Add(t, a_instance);
        return true;
    }
    
    /// <summary>
    /// Remove the type and instance from the container
    /// </summary>
    /// <param name="a_Object">This is the instance of the object</param>
    /// <typeparam name="T">This is the type of the object</typeparam>
    /// <returns> If it has been successfully removed will return true, else false</returns>
    public bool DeregisterFromContainer<T>(object a_instance) where T : class
    {
        if(!(a_instance is T))
        {
            Debug.LogError($"De-registering {typeof(T)} but instance is {(a_instance.GetType())}");
            return false;
        }
        
        Type t = typeof(T);
        if (m_dicContainer.ContainsKey(t))
        {
            object value;
            m_dicContainer.TryGetValue(t, out value);
            if (!ReferenceEquals(value, a_instance))
            {
                Debug.LogWarning($"The instance of type {t} in the container and the instance being removed is different");
                return false;
            }

            if(value != null)
            {
                Debug.Log($"The current type being disposed {value?.GetType()}");
                IDisposable disposable = value as IDisposable;
                disposable?.Dispose();
            }
            m_dicContainer.Remove(t);
            return true;
        }
        
        Debug.LogWarning($"{t} type NOT FOUND");
        return false;

    }

    /// <summary>
    /// Getter method to get instance of an object required
    /// </summary>
    /// <typeparam name="T">Type of the instance that is being retrieved</typeparam>
    /// <returns>Instance of type T</returns>
    public T GetFromContainer<T>() where T : class
    {
        Type t = typeof(T);
        if (m_dicContainer.ContainsKey(t))
        {
            object value;
            m_dicContainer.TryGetValue(t, out value);
            return value as T;
        }
        else
        {
            Debug.LogWarning($" {t} not found in the container");
        }

        return null;
    }
}