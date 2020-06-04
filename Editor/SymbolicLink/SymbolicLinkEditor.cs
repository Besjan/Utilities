namespace Cuku.Utilities
{
	using Sirenix.OdinInspector;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities;
	using Sirenix.Utilities.Editor;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	public class SymbolicLinkEditor : OdinEditorWindow
    {
        private static Vector2 windowSize = new Vector2(500, 280);

        [MenuItem("Cuku/Utilities/Create Symbolik Link")]
        private static void OpenWindow()
        {
            var window = GetWindow<SymbolicLinkEditor>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
            window.minSize = window.maxSize = windowSize;
        }

        [PropertySpace, InlineEditor]
        public SymbolicLinkConfig Config;

        [ShowIf("IsConfigValid"), PropertySpace(50), Button(ButtonSizes.Large)]
        public void CreateSymbolicLink()
        {
            var linkDirectory = Path.Combine(Config.ParentDirectory.Value, new DirectoryInfo(Config.RealDirectory.Value).Name);

            var commandSymbolicLink = string.Format(@"New-Item -Path ""{0}"" -ItemType SymbolicLink -Value ""{1}""", linkDirectory, Config.RealDirectory.Value);

            commandSymbolicLink.ExecutePowerShellCommand(false, true);
        }

        private bool IsConfigValid()
        {
            return Config != null;
        }
    }
}
