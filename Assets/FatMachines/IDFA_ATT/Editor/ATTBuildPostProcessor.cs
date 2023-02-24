#if UNITY_IOS
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class ATTBuildPostProcessor {

    [PostProcessBuild]
    public static void AddBuildSettings(BuildTarget buildTarget, string buildPath) {
        if (buildTarget == BuildTarget.iOS) {
            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projectPath);

            string targetId = "";
#if UNITY_2019_3_OR_NEWER
            targetId = project.GetUnityFrameworkTargetGuid();
#else
            targetId = project.TargetGuidByName("Unity-iPhone");
#endif

            // Required System Frameworks
            project.AddFrameworkToProject(targetId, "AdSupport.framework", false);
            project.AddFrameworkToProject(targetId, "AppTrackingTransparency.framework", false);
            project.AddFrameworkToProject(targetId, "StoreKit.framework", false);
            
            project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
        }
    }

}
#endif