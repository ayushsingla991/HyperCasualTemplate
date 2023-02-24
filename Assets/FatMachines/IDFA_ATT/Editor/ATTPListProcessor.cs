#if UNITY_IOS
using FM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class ATTPListProcessor {
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {
        if (buildTarget == BuildTarget.iOS) {
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;

            TextAsset skAdIds = (TextAsset)Resources.Load("sdkadnetworks");
            Dictionary<string, object> data = Json.Deserialize(skAdIds.text)as Dictionary<string, object>;

            string desc = data["desc"] as string;
            List<string> networks = (data["skadIds"] as List<object>).Select(x => x as string).ToList();

            if (networks.Count > 0) {
                PlistElementArray sdkIdArr = rootDict.CreateArray("SKAdNetworkItems");
                for (int i = 0; i < networks.Count; i++) {
                    PlistElementDict sdkId = sdkIdArr.AddDict();
                    sdkId.SetString("SKAdNetworkIdentifier", networks[i]);
                }
            }

            // App Tracking Transparency Description
            rootDict.SetString("NSUserTrackingUsageDescription", desc);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif