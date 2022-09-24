using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace Wonnasmith
{
    public class Initialization : MonoBehaviour
    {
        //Init:
        void Awake()
        {
            //singleton initialization:
            InitializeSingleton();
        }

        //Private Methods:
        void InitializeSingleton()
        {
            foreach (Component item in GetComponents<Component>())
            {
                string baseType;

#if NETFX_CORE
                baseType = item.GetType ().GetTypeInfo ().BaseType.ToString ();
#else
                baseType = item.GetType().BaseType.ToString();
#endif

                if (baseType.Contains("Singleton") && baseType.Contains("Wonnasmith"))
                {
                    MethodInfo m;

#if NETFX_CORE
                    m = item.GetType ().GetTypeInfo ().BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
#else
                    m = item.GetType().BaseType.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
#endif

                    m.Invoke(item, new Component[] { item });
                }
            }
        }
    }
}