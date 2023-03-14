using UnityEngine;

public class K : MonoBehaviour {

    public static class Tag {
        public static string Player = "Player";
    }

    public static class Prefs {
        public static string GDPR = "GDPR";
        public static string AdsRemoved = "AdsRemoved";
        public static string Level = "Level";
        public static string Sound = "Sound";
        public static string Taptic = "Taptic";
    }

    public static class RemoteConfig {
        public static string first_banner_ad_level = "first_banner_ad_level";
        public static string first_int_ad_level = "first_int_ad_level";
        public static string first_ad_time = "first_ad_time";
        public static string ad_interval = "ad_interval";
    }

    public static class Event {
        public static string banner_requested = "banner_requested";
        public static string banner_loaded = "banner_loaded";
        public static string interstitial_requested = "interstitial_requested";
        public static string interstitial_shown = "interstitial_shown";
        public static string interstitial_closed = "interstitial_closed";
        public static string interstitial_loaded = "interstitial_loaded";
        public static string interstitial_failed_to_load = "interstitial_failed_to_load";
        public static string interstitial_clicked = "interstitial_clicked";
        public static string rewarded_video_shown = "rewarded_video_shown";
        public static string rewarded_closed_no_reward = "rewarded_closed_no_reward";
    }

    public static class IAP {
        public static string remove_ads = "remove_ads";
    }

}