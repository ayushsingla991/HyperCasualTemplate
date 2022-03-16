#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class ATTPListProcessor {
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {
        if (buildTarget == BuildTarget.iOS) {
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;

            if(IDFA.sdkAdIds.Count > 0){
                PlistElementArray sdkIdArr = rootDict.CreateArray("SKAdNetworkItems");
                for(int i=0; i<IDFA.sdkAdIds.Count; i++){
                    PlistElementDict sdkId = sdkIdArr.AddDict();
                    sdkId.SetString("SKAdNetworkIdentifier", IDFA.sdkAdIds[i]);
                }
            }

            // App Tracking Transparency Description
            rootDict.SetString("NSUserTrackingUsageDescription", IDFA.idfaDesc);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif