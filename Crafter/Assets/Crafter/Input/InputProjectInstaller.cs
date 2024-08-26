using Zenject;

namespace Crafter.Input
{
    public class InputProjectInstaller : ScriptableObjectInstaller<InputProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputProjectManager>().AsSingle().NonLazy();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Crafter/Installers/Project/Input Installer", priority = 0)]
        public static void CreateInstaller()
        {
            string directory = "Assets/";

            if (UnityEditor.Selection.objects.Length > 0)
            {
                directory = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.objects[0]);
            }
            
            InputProjectInstaller instance = CreateInstance<InputProjectInstaller>();
            instance.name = "Input_Project_Installer";
            UnityEditor.AssetDatabase.CreateAsset(instance, $"{directory}/{instance.name}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}