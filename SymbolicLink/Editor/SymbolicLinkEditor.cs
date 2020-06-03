namespace Cuku.Utilities
{
	using Sirenix.OdinInspector;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities;
	using Sirenix.Utilities.Editor;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	public class SymbolicLinkEditor : OdinEditorWindow
    {
        private static Vector2 windowSize = new Vector2(500, 280);

        [MenuItem("Cuku/Utilities/Create Symlink")]
        private static void OpenWindow()
        {
            var window = GetWindow<SymbolicLinkEditor>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
            window.minSize = window.maxSize = windowSize;
        }

        protected override IEnumerable<object> GetTargets()
        {
            return new object[] { SymbolicLinkConfig.Instance, this };
        }

		[PropertySpace(50), Button(ButtonSizes.Large)]
        public void CreateSymbolicLink()
        {
            var linkDirectory = Path.Combine(SymbolicLinkConfig.Instance.ParentDirectory.Value, new DirectoryInfo(SymbolicLinkConfig.Instance.RealDirectory.Value).Name);

            var commandSymbolicLink = string.Format(@"New-Item -Path ""{0}"" -ItemType SymbolicLink -Value ""{1}""", linkDirectory, SymbolicLinkConfig.Instance.RealDirectory.Value);

            commandSymbolicLink.ExecutePowerShellCommand(false, true);
        }
    }
}
