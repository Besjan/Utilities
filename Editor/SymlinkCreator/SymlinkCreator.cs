namespace Cuku.Utilities
{
	using Cuku.ScriptableObject;
	using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
	using System.IO;
	using UnityEditor;

    public class SymlinkCreator : OdinEditorWindow
    {
        [MenuItem("Cuku/Utilities/Create Symlink")]
        private static void OpenWindow()
        {
            GetWindow<SymlinkCreator>().Show();
        }

		[PropertySpace]
        [InfoBox("Real directory for which the link will be created for, located outside Unity project.")]
		public StringSO RealDirectory;

        [PropertySpace(50)]
        [InfoBox("Parent directory where the link will be created in, located inside Unity project.")]
        public StringSO ParentDirectory;

        [PropertySpace(50)]
        [Button]
        public void CreateSymbolicLink()
        {
            var linkDirectory = Path.Combine(ParentDirectory.Value, new DirectoryInfo(RealDirectory.Value).Name);

            UnityEngine.Debug.Log(linkDirectory + "  |  " + RealDirectory);
            var commandSymbolicLink = string.Format(@"New-Item -Path ""{0}"" -ItemType SymbolicLink -Value ""{1}""", linkDirectory, RealDirectory.Value);

            commandSymbolicLink.ExecutePowerShellCommand(false, true);
        }
    }
}
