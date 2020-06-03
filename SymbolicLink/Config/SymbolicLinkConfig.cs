namespace Cuku.Utilities
{
	using Cuku.ScriptableObject;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using System;

    [Serializable]
	[GlobalConfig("Assets/Cuku/Utilities/SymbolicLink/Config/Data", UseAsset = true)]
    public class SymbolicLinkConfig : GlobalConfig<SymbolicLinkConfig>
    {
		[InfoBox("Real directory for which the link will be created for, located outside Unity project.", InfoMessageType.None)]
		[PropertySpace, InlineEditor]
        public StringSO RealDirectory;

		[PropertySpace(50), InlineEditor]
		[InfoBox("Parent directory where the link will be created in, located inside Unity project.", InfoMessageType.None)]
        public StringSO ParentDirectory;
    }
}
