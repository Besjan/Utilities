namespace Cuku.Utilities
{
    using Cuku.ScriptableObject;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.IO;
    using UnityEditor;
	using UnityEngine;

	public class SymlinkEditorWindow : OdinEditorWindow
    {
        private static Vector2 windowSize = new Vector2(500, 280);

        [MenuItem("Cuku/Utilities/Create Symlink")]
        private static void OpenWindow()
        {
            var window = GetWindow<SymlinkEditorWindow>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
            window.minSize = window.maxSize = windowSize;
        }

        protected override object GetTarget()
        {   
            return SymlinkEditor.Instance;
        }
    }

    [GlobalConfig("Assets/Cuku/Utilities/Editor/Symlink/SOs", UseAsset = true)]
    public class SymlinkEditor : GlobalConfig<SymlinkEditor>
    {
        [PropertySpace, LabelWidth(100)]
        [InfoBox("Real directory for which the link will be created for, located outside Unity project.")]
        public StringSO RealDirectory;

        [PropertySpace(50), LabelWidth(100)]
        [InfoBox("Parent directory where the link will be created in, located inside Unity project.")]
        public StringSO ParentDirectory;

        [PropertySpace(50), Button(ButtonSizes.Large)]
        public void CreateSymbolicLink()
        {
            var linkDirectory = Path.Combine(Instance.ParentDirectory.Value, new DirectoryInfo(Instance.RealDirectory.Value).Name);

            UnityEngine.Debug.Log(linkDirectory + "  |  " + Instance.RealDirectory);
            var commandSymbolicLink = string.Format(@"New-Item -Path ""{0}"" -ItemType SymbolicLink -Value ""{1}""", linkDirectory, Instance.RealDirectory.Value);

            commandSymbolicLink.ExecutePowerShellCommand(false, true);
        }
    }
}
