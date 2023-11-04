using UnityEngine;

namespace DefaultNamespace
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            Database.Initialize();
        }
    }
}