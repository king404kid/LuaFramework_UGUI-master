using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LoadResourceTest : MonoBehaviour
{
    public AssetBundleManifest m_Manifest = null;
    private LoadType m_loadType = LoadType.LOAD_FROM_FILE;
    private SyncType m_syncType = SyncType.Sync;
    private string UI_BUNDLE_NAME = "ui";

    // Use this for initialization
    void Start() {
        LoadManifest();
        string path = "AssetBundles/Windows/ui/prefab/model/z_jk_body_5_1_skin.unity3d";
        string assetBundleName = "prefab/model/z_jk_body_5_1_skin.unity3d";
        string assetName = "z_jk_body_5_1_skin";
        switch (m_loadType) {
            case LoadType.LOAD_FROM_FILE:
                if (m_syncType == SyncType.Sync) {
                    TryLoadFromFile(assetBundleName, assetName);
                } else {
                    StartCoroutine(TryLoadFromFileAsync(assetBundleName, assetName));
                }
                break;
            case LoadType.LOAD_FROM_MEMORY:
                if (m_syncType == SyncType.Sync) {
                    TryLoadFromMemory(assetBundleName, assetName);
                } else {
                    StartCoroutine(TryLoadFromMemoryAsync(assetBundleName, assetName));
                }
                break;
            case LoadType.UNITY_WEB_REQUEST:
                StartCoroutine(TryUnityWebRequest(assetBundleName, assetName));
                break;
            case LoadType.WWW:
                StartCoroutine(TryWWW(assetBundleName, assetName));
                break;
            case LoadType.LOAD_FROM_CACHE_OR_DOWNLOAD:
                StartCoroutine(TryLoadFromCacheOrDownload(assetBundleName, assetName));
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// 从内存加载，异步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator TryLoadFromMemoryAsync(string assetBundleName, string assetName) {
        string url;
        AssetBundleCreateRequest request;
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dependencies.Length; i++) {
            string abName = dependencies[i];
            url = GetAssetUrl(abName);
            request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(url));
            yield return request;
            AssetBundle ab1 = request.assetBundle;
        }
        url = GetAssetUrl(assetBundleName);
        request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(url));
        yield return request;
        AssetBundle ab = request.assetBundle;
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// 从内存加载，同步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    void TryLoadFromMemory(string assetBundleName, string assetName) {
        AssetBundle ab = LoadAssetBundle(assetBundleName, true, false);
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// 从磁盘加载，异步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator TryLoadFromFileAsync(string assetBundleName, string assetName) {
        string url;
        AssetBundleCreateRequest request;
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dependencies.Length; i++) {
            string abName = dependencies[i];
            url = GetAssetUrl(abName);
            request = AssetBundle.LoadFromFileAsync(url);
            yield return request;
            AssetBundle ab1 = request.assetBundle;
        }
        url = GetAssetUrl(assetBundleName);
        request = AssetBundle.LoadFromFileAsync(url);
        yield return request;
        AssetBundle ab = request.assetBundle;
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// 从磁盘加载，同步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    void TryLoadFromFile(string assetBundleName, string assetName) {
        AssetBundle ab = LoadAssetBundle(assetBundleName, true, true);
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// 加载assetbundle，包括其依赖
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="loadDependencies"></param>
    /// <returns></returns>
    protected AssetBundle LoadAssetBundle(string assetBundleName, bool loadDependencies, bool fromFile) {
        if (loadDependencies) {
            LoadDependencies(assetBundleName, fromFile);
        }
        string url = GetAssetUrl(assetBundleName);
        if (fromFile) {
            return AssetBundle.LoadFromFile(url);
        }
        return AssetBundle.LoadFromMemory(File.ReadAllBytes(url));
    }

    /// <summary>
    /// 加载依赖
    /// </summary>
    /// <param name="assetBundleName"></param>
    void LoadDependencies(string assetBundleName, bool fromFile) {
        if (m_Manifest == null) {
            return;
        }
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        if (dependencies == null) {
            return;
        }
        for (int i = 0; i < dependencies.Length; i++) {
            if (string.IsNullOrEmpty(dependencies[i]) == false) {
                LoadAssetBundle(dependencies[i], false, fromFile);
            }
        }
    }

    /// <summary>
    /// 从远程加载
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator TryUnityWebRequest(string assetBundleName, string assetName) {
        //使用UnityWbRequest  服务器加载使用http本地加载使用file
        //string uri = @"file:///C:\Users\Administrator\Desktop\AssetBundleProject\AssetBundles\model.ab";
        //string uri = @"http://localhost/AssetBundles\model.ab";

        string url;
        UnityWebRequest request;
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dependencies.Length; i++) {
            string abName = dependencies[i];
            url = GetAssetUrl(abName);        // 这个加不加"file:///"都可以
            request = UnityWebRequest.GetAssetBundle(url);
            yield return request.Send();
            DownloadHandlerAssetBundle.GetContent(request);   // 这一步是必须的
        }
        url = GetAssetUrl(assetBundleName);
        request = UnityWebRequest.GetAssetBundle(url);
        yield return request.Send();
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// WWW加载
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    IEnumerator TryWWW(string assetBundleName, string assetName) {
        string url;
        WWW www;
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dependencies.Length; i++) {
            string abName = dependencies[i];
            url = "file:///" + GetAssetUrl(abName);   // 这里没有加"file:///"会报错
            www = new WWW(url);
            yield return www;
            AssetBundle ab1 = www.assetBundle;   // 这一步是必须的
        }
        url = "file:///" + GetAssetUrl(assetBundleName);
        www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false) {
            Debug.Log(www.error);
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// LoadFromCacheOrDownload加载
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    IEnumerator TryLoadFromCacheOrDownload(string assetBundleName, string assetName) {
        string url;
        WWW www;
        string[] dependencies = m_Manifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dependencies.Length; i++) {
            string abName = dependencies[i];
            url = "file:///" + GetAssetUrl(abName);   // 这里没有加"file:///"会报错
            www = WWW.LoadFromCacheOrDownload(url, 1);
            yield return www;
            //AssetBundle ab1 = www.assetBundle;   // 这一步不是必须的
        }
        url = "file:///" + GetAssetUrl(assetBundleName);
        www = WWW.LoadFromCacheOrDownload(url, 1);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false) {
            Debug.Log(www.error);
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        GameObject go = ab.LoadAsset<GameObject>(assetName);
        GameObject.Instantiate(go);
    }

    /// <summary>
    /// 加载manifest
    /// </summary>
    private void LoadManifest() {
        string url = GetAssetUrl(UI_BUNDLE_NAME);
        AssetBundle ab = AssetBundle.LoadFromFile(url);
        if (ab == null) {
            return;
        }
        AssetBundleManifest abm = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest", typeof(Object));
        ab.Unload(false);
        if (abm != null) {
            m_Manifest = abm;
        }
    }

    /// <summary>
    /// 根据assetBundleName获取磁盘的详细url
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    string GetAssetUrl(string assetBundleName) {
        string localPath = Application.dataPath;
        int index = localPath.LastIndexOf("/");
        if (index != -1) {
            localPath = localPath.Substring(0, index + 1);
        }
        localPath = localPath + "AssetBundles/Windows/ui/" + assetBundleName;
        return localPath;
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    void ClearCache() {
        Caching.CleanCache();
    }

    /// <summary>
    /// 加载方式
    /// </summary>
    public enum LoadType
    {
        LOAD_FROM_FILE = 0,
        LOAD_FROM_MEMORY = 1,
        UNITY_WEB_REQUEST = 2,
        WWW = 3,
        LOAD_FROM_CACHE_OR_DOWNLOAD = 4,
    }

    /// <summary>
    /// 同步异步
    /// </summary>
    public enum SyncType
    {
        Sync = 0,
        Async = 1,
    }
}