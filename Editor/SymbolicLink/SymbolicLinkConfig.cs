namespace Cuku.Utilities
{
	using Sirenix.OdinInspector;

    public class SymbolicLinkConfig : SerializedScriptableObject
    {
		[InfoBox("Real directory for which the link will be created for, located outside Unity project.", InfoMessageType.None)]
		[PropertySpace, FolderPath(AbsolutePath = true, RequireExistingPath = true)]
        public string RealDirectory;

		[PropertySpace(50), FolderPath(AbsolutePath = true, RequireExistingPath = true)]
		[InfoBox("Parent directory where the link will be created in, located inside Unity project.", InfoMessageType.None)]
        public string ParentDirectory;
    }
}
