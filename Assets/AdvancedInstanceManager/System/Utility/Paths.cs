using System.IO;
using UnityEngine;

namespace InstanceManager.Utility
{

    /// <summary>Common paths in instance manager.</summary>
    static class Paths
    {

        /// <summary>The path to the project, outside of Assets folder.</summary>
        public static string project { get; } = new DirectoryInfo(Application.dataPath).Parent.FullName.ToCrossPlatformPath();

        /// <summary>The path to the folder above the project, two levels above Assets folder.</summary>
        public static string aboveProject { get; } = new DirectoryInfo(project).Parent.FullName.ToCrossPlatformPath();

        /// <summary>Gets the path to the specified secondary instance.</summary>
        public static string InstancePath(string id) => $"{aboveProject}/{Application.productName}{InstanceSeparatorChar}{id}";

        public const char InstanceSeparatorChar = '﹕';

        public static string primaryEventsFile => PrimaryInstancePath() + "/.instance-events";

        /// <summary>Returns the folder of the primary instance.</summary>
        public static string PrimaryInstancePath()
        {
            var name = Path.GetFileName(project);
            return aboveProject + "/" +
                (name.Contains(InstanceSeparatorChar.ToString())
                ? name.Remove(name.IndexOf(InstanceSeparatorChar))
                : name);
        }

    }

}
