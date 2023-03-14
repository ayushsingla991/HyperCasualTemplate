// #define IAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if IAP
using UnityEngine.Purchasing;
#endif

public enum IAPType { Consumable, NonConsumable, Subscription }
public class IAP : MonoBehaviour {

    public static IAP instance;

#if IAP
    private static IStoreController m_StoreController;
    private static IAppleExtensions m_AppleExtenstions;
    private static IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
    private static IExtensionProvider m_StoreExtensionProvider;
#endif

    System.Action<string, string> OnItemPurchased;

#if IAP
    Dictionary<IAPType, ProductType> iapType = new Dictionary<IAPType, ProductType> { { IAPType.Consumable, ProductType.Consumable },
        { IAPType.NonConsumable, ProductType.NonConsumable },
        { IAPType.Subscription, ProductType.Subscription }
    };
#endif

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    void Start() {
#if IAP
        if (m_StoreController == null) {
            InitializePurchasing();
        }
#endif
    }

    public static void AddOnItemPurchaseListener(System.Action<string, string> _OnItemPurhcased) {
        if (instance == null) {
            return;
        }
        instance.OnItemPurchased += _OnItemPurhcased;
    }

    public static void RemoveOnItemPurchaseListener(System.Action<string, string> _OnItemPurhcased) {
        if (instance == null) {
            return;
        }
        instance.OnItemPurchased -= _OnItemPurhcased;
    }

    bool IsInitialized() {
        // Only say we are initialized if both the Purchasing references are set.
#if IAP
        return m_StoreController != null;
#endif
        return false;
    }

    public static string GetProductLocalPrice(string productId) {
#if IAP
        if (m_StoreController == null) {
            return "0";
        }
        return m_StoreController.products.WithID(productId).metadata.localizedPriceString;
#endif
        return "0";
    }

#if IAP
    // Apple app store promotional IAP check
    private void OnPromotionalPurchase(Product item) {
        Debug.Log("Attempted promotional purchase: " + item.definition.id);
#if UNITY_IOS
        StartCoroutine(ContinuePromotionalPurchases());
#endif
    }
#endif

    private IEnumerator ContinuePromotionalPurchases() {
        Debug.Log("Continuing promotional purchases in 5 seconds");
        yield return new WaitForSeconds(5);
        Debug.Log("Continuing promotional purchases now");
#if UNITY_IOS && IAP
        m_AppleExtenstions.ContinuePromotionalPurchases(); // iOS and tvOS only
#endif
    }
    // Apple app store promotional IAP check

    public void InitializePurchasing() {
        if (IsInitialized()) {
            return;
        }

#if IAP
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (KeyValuePair<string, IAPType> iapId in IAP.iapId) {
            builder.AddProduct(iapId.Key, iapType[iapId.Value]);
        }
        builder.Configure<IAppleConfiguration>().SetApplePromotionalPurchaseInterceptorCallback(OnPromotionalPurchase);

        UnityPurchasing.Initialize(this, builder);
#endif
    }

#if IAP
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

#if UNITY_IOS
        m_AppleExtenstions = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
        m_AppleExtenstions.RegisterPurchaseDeferredListener(product => { });
#elif UNITY_ANDROID
        m_GooglePlayStoreExtensions = m_StoreExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();
#endif
    }
#endif

    public static bool IsSubscriptionActive(string productId) {
#if IAP
        Product item = m_StoreController.products.WithID(productId);
        Dictionary<string, string> introductory_info_dict = new Dictionary<string, string>();
#if UNITY_IOS
        introductory_info_dict = m_AppleExtenstions.GetIntroductoryPriceDictionary();
#endif
        if (item.definition.type == ProductType.Subscription) {
            string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
#if UNITY_ANDROID
            intro_json = null;
#endif
            SubscriptionManager p = new SubscriptionManager(item, intro_json);
            SubscriptionInfo info = p.getSubscriptionInfo();

            Debug.Log("isSubscribed: " + info.isSubscribed().ToString());
            Debug.Log("isExpired: " + info.isExpired().ToString());
            Debug.Log("isCancelled: " + info.isCancelled().ToString());

            return bool.Parse(info.isSubscribed().ToString()) || !bool.Parse(info.isExpired().ToString()) || !bool.Parse(info.isCancelled().ToString());
        }
#endif
        return false;
    }

    public static bool HasReceipt(string productId) {
#if IAP
        return m_StoreController.products.WithID(productId).receipt != null;
#endif
        return false;
    }

    public static void BuyProduct(string productId) {
        LoadingPopup.Show("Initiating...");
        instance._BuyProduct(productId);
    }

    void _BuyProduct(string productId) {
        if (IsInitialized()) {
#if IAP
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.hasReceipt && productId == K.IAP.remove_ads) {
                OnItemPurchased(productId, product.receipt);
                InfoPopup.Show("Already purchased. Enjoy Ads Free Version", "OK");
            } else
            if (product != null && product.availableToPurchase) {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            } else {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                InfoPopup.Show("Transaction Failed!", "OK");
            }
#endif
        } else {
            Debug.Log("BuyProductID FAIL. Not initialized.");
#if UNITY_IOS
            InfoPopup.Show("Can't connect to Apple's servers", "OK");
#elif UNITY_ANDROID
            InfoPopup.Show("Can't connect to Google's servers", "OK");
#endif
        }
    }

    public static void RestoreTransactions() {
#if IAP
        if (instance == null) {
            return;
        }
        if (m_StoreExtensionProvider == null) {
            return;
        }
        LoadingPopup.Show("Please Wait...");
#if UNITY_IOS
        m_AppleExtenstions.RestoreTransactions((bool result) => {
            if (result) {
                InfoPopup.Show("All transactions restored successfully!", "OK");
            } else {
                InfoPopup.Show("Something went wrong!", "OK");
            }
        });
#elif UNITY_ANDROID
        m_GooglePlayStoreExtensions.RestoreTransactions((bool result) => {
            if (result) {
                InfoPopup.Show("All transactions restored successfully!", "OK");
            } else {
                InfoPopup.Show("Something went wrong!", "OK");
            }
        });
#endif
#endif
    }

#if IAP
    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
        Debug.Log("purchased: " + args.purchasedProduct.definition.id);
        if (OnItemPurchased != null) {
            string productId = args.purchasedProduct.definition.id;
            OnItemPurchased(productId, args.purchasedProduct.receipt);
        }
        InfoPopup.Show("Purchase Successfull!", "OK");
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        InfoPopup.Show("Purchase Failed! Reason: " + failureReason, "OK");
    }
#endif

}