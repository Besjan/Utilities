namespace Cuku.Utilities
{
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
        [FolderPath(AbsolutePath = true)]
        public string RealDirectory;

        [PropertySpace(50)]
        [InfoBox("Parent directory where the link will be created in, located inside Unity project.")]
        [FolderPath(AbsolutePath = true)]
        public string ParentDirectory;

        [PropertySpace(50)]
        [Button]
        public void CreateSymbolicLink()
        {
            var linkDirectory = Path.Combine(ParentDirectory, new DirectoryInfo(RealDirectory).Name);

            var commandSymbolicLink = string.Format(@"New-Item -Path ""{0}"" -ItemType SymbolicLink -Value ""{1}""", linkDirectory, RealDirectory);

            commandSymbolicLink.ExecutePowerShellCommand(false, true);
        }
    }
}
