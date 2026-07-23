using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Generic MonoBehaviour singleton base. Lazily finds an existing instance in the
    /// scene, or creates one on first access if none exists.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                }

                if (instance == null)
                {
                    var owner = new GameObject(typeof(T).Name);
                    instance = owner.AddComponent<T>();
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
        }
    }
}
