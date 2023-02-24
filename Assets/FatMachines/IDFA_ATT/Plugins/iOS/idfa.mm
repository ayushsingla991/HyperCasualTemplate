#import <AppTrackingTransparency/ATTrackingManager.h>
#import <StoreKit/SKAdNetwork.h>

typedef void (*OnIDFAStatus)(int idfaStatus);
void RequestIDFA(OnIDFAStatus callback){
    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
        // Tracking authorization completed. Start loading ads here.
            if (status == ATTrackingManagerAuthorizationStatusDenied) {
                callback(0);
            } else if (status == ATTrackingManagerAuthorizationStatusAuthorized) {
                callback(1);
            } else if (status == ATTrackingManagerAuthorizationStatusNotDetermined) {
                callback(2);
            }  else if (status == ATTrackingManagerAuthorizationStatusRestricted) {
                callback(3);
            }
        }];
    }
}

extern "C"{
    void _RequestIDFA(OnIDFAStatus callback){
        RequestIDFA(callback);
    }
}
