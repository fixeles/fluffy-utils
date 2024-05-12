using UnityEditor;
using UnityEngine;

namespace FPS
{
    public abstract class ScriptableObjectCreator<T> where T : ScriptableObject
    {
        protected static void TryCreate()
        {
#if !CORE_DEV
            var type = typeof(T);
            if (Resources.Load<T>(type.Name) != null)
                return;

            if (!AssetDatabase.IsValidFolder("Assets/FPS"))
                AssetDatabase.CreateFolder("Assets", "FPS");

            if (!AssetDatabase.IsValidFolder("Assets/FPS/Resources"))
                AssetDatabase.CreateFolder("Assets/FPS", "Resources");


            var reference = Utils.Editor.GetAllInstances<T>()[0];
            try
            {
                var newObject = Object.Instantiate(reference);
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