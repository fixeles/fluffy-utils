using UnityEditor;
using UnityEngine;

namespace FPS
{
    //todo: remove inheritance. use attribute
    public abstract class ScriptableObjectCreator<T> where T : ScriptableObject
    {
        protected static void TryCreate()
        {
#if !CORE_DEV
            var type = typeof(T);
            if (Resources.Load<T>(type.Name) != null)
                return;
            Utils.Editor.TryCreateResourcesFolder();

            var instances = Utils.Editor.GetAllInstances<T>();
            try
            {
                T newObject = instances.Length == 0 ? ScriptableObject.CreateInstance<T>() : Object.Instantiate(instances[0]);
                AssetDatabase.CreateAsset(newObject, $"Assets/FPS/Resources/{type.Name}.asset");
            }
            catch
            {
                // ignored
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}