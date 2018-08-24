using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Tools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public class CheckMapResTool : EditorWindow
{
    static private CheckMapResTool _instance;
    private int _titleSelectIndex = 0;
    private string[] _titleStrs = new string[] { "全部", "检查Static", "Default-Material", "Lagacy Shader", "替换Layer", "检查Light" };
    private Object _checkFolder;
    private List<StaticObject> _staticObjects;
    private int _checkStaticTotal;
    private Vector2 _staticResultPos = new Vector2();
    private List<string> _staticCheckList = new List<string>() { "WorldStage", "MapEffect", "MapModel", "MapModel1", "MapModelNear", "MapPendant", "MapModelBig", "MapBase", "MapNav", "MapTrigger", "Clipper", "MapModelSmall" };
    private List<string> _addRuntimeCompList = new List<string>() { "MapModel", "MapModel1", "MapModelNear", "MapPendant", "MapModelBig", "MapModelSmall" };
    private List<DfMaterialObject> _dfMaterialObjects;
    private int _checkDfMaterialTotal;
    private Vector2 _dfMaterialResultPos = new Vector2();
    private List<Material> _shaderObjects;
    private int _checkShaderTotal;
    private Vector2 _shaderResultPos = new Vector2();
    private int _checkLayerTotal;
    private int _modifyLayerTotal;
    private List<int> _layerList = new List<int>();
    private List<LightObject> _lightObjects;
    private int _checkLightTotal;
    private Vector2 _lightResultPos = new Vector2();
    private MainObject _mainObject;
    private Vector2 _mainResultPos = new Vector2();

    public static CheckMapResTool Instance {
        get {
            if (_instance == null) {
                _instance = (CheckMapResTool)EditorWindow.GetWindow(typeof(CheckMapResTool));
                _instance.titleContent = new GUIContent("检查地图资源");
                _instance.maxSize = new Vector2(500, 650);
                _instance.minSize = new Vector2(500, 650);
            }
            return CheckMapResTool._instance;
        }
    }

    [MenuItem("Tools/检查地图资源", false, 106)]
    static void ShowWin() {
        CheckMapResTool.Instance.Show();
    }

    void OnGUI() {
        ShowSelectTitle();
        if (_layerList.Count == 0) {
            int[] temp = { 0, 9, 10, 11, 12, 13, 14, 15, 16, 19, 20, 25 };
            for (int i = 0; i < temp.Length; i++) {
                _layerList.Add(temp[i]);
            }
        }
        if (_titleSelectIndex == 0) {
            ShowMainCtrl();
            ShowMainResult();
        } else if (_titleSelectIndex == 1) {
            ShowStaticCtrl();
            ShowStaticResult();
        } else if (_titleSelectIndex == 2) {
            ShowDfMaterialCtrl();
            ShowDfMaterialResult();
        } else if (_titleSelectIndex == 3) {
            ShowShaderCtrl();
            ShowShaderResult();
        } else if (_titleSelectIndex == 4) {
            ShowLayerCtrl();
            ShowLayerResult();
        } else if (_titleSelectIndex == 5) {
            ShowLightCtrl();
            ShowLightResult();
        }
    }

    private void ShowSelectTitle() {
        _titleSelectIndex = GUILayout.SelectionGrid(_titleSelectIndex, _titleStrs, 4, GUILayout.Width(480), GUILayout.Height(50));
        ShowLine();
    }

    private void ShowMainCtrl() {
        ShowLine("操作");
        GUILayout.Label("用于检查当前场景的各种问题");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            CheckMain();
        }
        if (GUILayout.Button("修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyMain();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowMainResult() {
        ShowLine("结果");
        if (_mainObject == null) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("搜索结果如下：", GUILayout.Width(200));
        GUILayout.EndHorizontal();
        _mainResultPos = GUILayout.BeginScrollView(_mainResultPos, GUILayout.Width(450), GUILayout.Height(410));
        for (int i = 0; i < _mainObject.staticList.Count; i++) {
            if (i == 0) {
                GUILayout.Label("----------static：" + _mainObject.staticList.Count + "个----------", GUILayout.Width(200));
            }
            EditorGUILayout.ObjectField(_mainObject.staticList[i], typeof(Object), GUILayout.Width(300));
        }
        for (int i = 0; i < _mainObject.dfMaterialList.Count; i++) {
            if (i == 0) {
                GUILayout.Label("----------material：" + _mainObject.dfMaterialList.Count + "个----------", GUILayout.Width(200));
            }
            EditorGUILayout.ObjectField(_mainObject.dfMaterialList[i], typeof(Object), GUILayout.Width(300));
        }
        for (int i = 0; i < _mainObject.shaderList.Count; i++) {
            if (i == 0) {
                GUILayout.Label("----------shader：" + _mainObject.shaderList.Count + "个----------", GUILayout.Width(200));
            }
            EditorGUILayout.ObjectField(_mainObject.shaderList[i], typeof(Object), GUILayout.Width(300));
        }
        for (int i = 0; i < _mainObject.layerList.Count; i++) {
            if (i == 0) {
                GUILayout.Label("----------layer：" + _mainObject.layerList.Count + "个----------", GUILayout.Width(200));
            }
            EditorGUILayout.ObjectField(_mainObject.layerList[i], typeof(Object), GUILayout.Width(300));
        }
        for (int i = 0; i < _mainObject.lightList.Count; i++) {
            if (i == 0) {
                GUILayout.Label("----------light：" + _mainObject.lightList.Count + "个----------", GUILayout.Width(200));
            }
            EditorGUILayout.ObjectField(_mainObject.lightList[i], typeof(Object), GUILayout.Width(300));
        }
        GUILayout.EndScrollView();
    }

    private void CheckMain() {
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null) {
            ShowTips("请打开一个场景");
            return;
        }
        _mainObject = new MainObject();
        GameObject[] goList = scene.GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllMain(go.transform);
            }
        }
        ShowTips("检查完成");
    }

    private void ModifyMain() {
        if (_mainObject == null) {
            ShowTips("请先检查");
            return;
        }
        for (int i = 0; i < _mainObject.staticList.Count; i++) {
            GameObject go = _mainObject.staticList[i] as GameObject;
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(go);
            int lightFlag = (int)(flags & StaticEditorFlags.LightmapStatic);
            if (lightFlag > 0) {
                GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.LightmapStatic);
            } else {
                GameObjectUtility.SetStaticEditorFlags(go, 0);
            }
        }
        for (int i = 0; i < _mainObject.dfMaterialList.Count; i++) {
            (_mainObject.dfMaterialList[i] as MeshRenderer).material = null;
        }
        Shader sd = Shader.Find("Mobile/Diffuse");
        for (int i = 0; i < _mainObject.shaderList.Count; i++) {
            Material mt = (_mainObject.shaderList[i] as GameObject).GetComponent<MeshRenderer>().sharedMaterial;
            if (mt.shader.name != "Mobile/Diffuse") {
                mt.shader = sd;
                EditorUtility.SetDirty(mt);
            }
        }
        for (int i = 0; i < _mainObject.layerList.Count; i++) {
            GameObject go = _mainObject.layerList[i] as GameObject;
            if (_layerList.IndexOf(go.layer) < 0) {
                go.layer = 0;
            }
        }
        for (int i = 0; i < _mainObject.lightList.Count; i++) {
            GameObject go = _mainObject.lightList[i] as GameObject;
            Light light = go.GetComponent<Light>();
            //SceneLight sl = go.GetComponent<SceneLight>();
            //ShadowConfig sc = go.GetComponent<ShadowConfig>();
            light.enabled = false;
            //sl.lit = light;
            //sc.lit = light;
        }
        _mainObject = null;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        ShowTips("修改完成");
    }

    private void LookupAllMain(Transform tf) {
        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(tf.gameObject);
        if (flags != 0 && (flags != StaticEditorFlags.LightmapStatic)) {
            _mainObject.staticList.Add(tf.gameObject);
        }
        MeshRenderer renderer = tf.GetComponent<MeshRenderer>();
        if (renderer && renderer.enabled == false && renderer.sharedMaterial != null && renderer.sharedMaterial.ToString() == "Default-Material (UnityEngine.Material)") {
            _mainObject.dfMaterialList.Add(renderer);
        }
        if (renderer && renderer.sharedMaterial) {
            Material mt = renderer.sharedMaterial;
            if (mt && mt.shader.name == "Legacy Shaders/Diffuse" && mt.color == Color.white) {
                _mainObject.shaderList.Add(tf.gameObject);
            }
        }
        if (_layerList.IndexOf(tf.gameObject.layer) < 0) {
            _mainObject.layerList.Add(tf.gameObject);
        }
        Light light = tf.GetComponent<Light>();
        //SceneLight sl = tf.GetComponent<SceneLight>();
        //ShadowConfig sc = tf.GetComponent<ShadowConfig>();
        //if (light != null && sl != null && sc != null && (light.enabled || sl.lit == null || sc.lit == null)) {
        if (light != null && light.enabled == true) {
            _mainObject.lightList.Add(tf.gameObject);
        }
        foreach (Transform item in tf) {
            LookupAllMain(item);
        }
    }

    private void ShowStaticCtrl() {
        ShowLine("操作");
        GUILayout.Label("作用：检查Static，把非Lightmap的勾选都去掉");
        GUILayout.Label("请把需要查找的文件或文件夹(ART/Scene，ART/Scene2)拖到下面的框中");
        GUILayout.Label("如果没有指定，则默认选取ART/Scene和ART/Scene2");
        _checkFolder = EditorGUILayout.ObjectField(_checkFolder, typeof(Object), false, GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            CheckStatic();
        }
        if (GUILayout.Button("全部修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyStatic();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowStaticResult() {
        ShowLine("结果");
        if (_staticObjects == null || _staticObjects.Count == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("搜索结果：" + _staticObjects.Count + "个，一共搜索：" + _checkStaticTotal + "个", GUILayout.Width(200));
        GUILayout.EndHorizontal();
        _staticResultPos = GUILayout.BeginScrollView(_staticResultPos, GUILayout.Width(450), GUILayout.Height(410));
        for (int i = 0; i < _staticObjects.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_staticObjects[i].obj, typeof(Object), GUILayout.Width(300));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void CheckStatic() {
        _staticObjects = new List<StaticObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkStaticTotal = filePaths.Count;
        for (int i = 0; i < _checkStaticTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在检查文件：", filename + "---" + i + "/" + _checkStaticTotal, (float)i / _checkStaticTotal);
            CheckOneStaticFile(filePaths[i], 1);
        }
        EditorUtility.ClearProgressBar();
    }

    private void CheckOneStaticFile(string path, int op) {
        Scene scene = EditorSceneManager.OpenScene(path);
        string relativePath = path.Replace(FilePathHelper.GetProjectPath(), "");
        StaticObject staticObj = new StaticObject();
        staticObj.hasStatic = false;
        GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllStatic(go.transform, staticObj, op);
                //if (op == 2 && _addRuntimeCompList.IndexOf(go.name) > -1 && go.GetComponent<RuntimeBatch>() == null) {
                //    go.AddComponent<RuntimeBatch>();
                //}
            }
        }
        if (staticObj.hasStatic) {
            staticObj.obj = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
            staticObj.path = path;
            _staticObjects.Add(staticObj);
        }
        if (op == 2 && staticObj.hasStatic) {
            EditorSceneManager.SaveScene(scene);
        }
    }

    // op=1查找，op=2替换
    private void LookupAllStatic(Transform tf, StaticObject staticObj, int op) {
        if (op == 1 && staticObj.hasStatic) {
            return;
        }
        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(tf.gameObject);
        //if (flags != 0 && (flags & StaticEditorFlags.LightmapStatic) == 0) {
        if (flags != 0 && (flags != StaticEditorFlags.LightmapStatic)) {
            staticObj.hasStatic = true;
            if (op == 2) {
                int lightFlag = (int)(flags & StaticEditorFlags.LightmapStatic);
                if (lightFlag > 0) {
                    GameObjectUtility.SetStaticEditorFlags(tf.gameObject, StaticEditorFlags.LightmapStatic);
                } else {
                    GameObjectUtility.SetStaticEditorFlags(tf.gameObject, 0);
                }
            }
        }
        foreach (Transform item in tf) {
            LookupAllStatic(item, staticObj, op);
        }
    }

    private void ModifyStatic() {
        _staticObjects = new List<StaticObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        for (int i = 0; i < filePaths.Count; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在修改文件：", filename + "---" + i + "/" + filePaths.Count, (float)i / filePaths.Count);
            CheckOneStaticFile(filePaths[i], 2);
        }
        EditorUtility.ClearProgressBar();
        //for (int i = 0; i < _staticObjects.Count; i++) {
        //    string filename = FilePathHelper.GetFileName(_staticObjects[i].path);
        //    EditorUtility.DisplayProgressBar("正在修改文件：", filename + "---" + i + "/" + _staticObjects.Count, (float)i / _staticObjects.Count);
        //    CheckOneStaticFile(_staticObjects[i].path, 2);
        //}
        //EditorUtility.ClearProgressBar();
        _staticObjects = null;
        ShowTips("修改完成");
    }

    private void ShowDfMaterialCtrl() {
        ShowLine("操作");
        GUILayout.Label("作用：把没有用的MeshRenderer却使用Default-Material的干掉其Material");
        GUILayout.Label("请把需要查找的文件或文件夹(ART/Scene，ART/Scene2)拖到下面的框中");
        GUILayout.Label("如果没有指定，则默认选取ART/Scene和ART/Scene2");
        _checkFolder = EditorGUILayout.ObjectField(_checkFolder, typeof(Object), GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            CheckDfMaterial();
        }
        if (GUILayout.Button("全部修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyDfMaterial();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowDfMaterialResult() {
        ShowLine("结果");
        if (_dfMaterialObjects == null || _dfMaterialObjects.Count == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("搜索结果：" + _dfMaterialObjects.Count + "个，一共搜索：" + _checkDfMaterialTotal + "个", GUILayout.Width(200));
        GUILayout.EndHorizontal();
        _dfMaterialResultPos = GUILayout.BeginScrollView(_dfMaterialResultPos, GUILayout.Width(450), GUILayout.Height(410));
        for (int i = 0; i < _dfMaterialObjects.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_dfMaterialObjects[i].obj, typeof(Object), GUILayout.Width(300));
            if (GUILayout.Button(_dfMaterialObjects[i].isOpen ? "缩起" : "展开", GUILayout.Width(50), GUILayout.Height(20))) {
                OpenDfMaterial(_dfMaterialObjects[i]);
            }
            if (GUILayout.Button("修改", GUILayout.Width(50), GUILayout.Height(20))) {
                ModifySingleDfMaterial(_dfMaterialObjects[i]);
            }
            GUILayout.EndHorizontal();
            if (_dfMaterialObjects[i].isOpen) {
                GUILayout.Label("-------------开始-------------", GUILayout.Width(200));
                GUILayout.Label("一共：" + _dfMaterialObjects[i].details.Count + "个子项", GUILayout.Width(200));
                for (int j = 0; j < _dfMaterialObjects[i].details.Count; j++) {
                    EditorGUILayout.ObjectField(_dfMaterialObjects[i].details[j], typeof(Object), GUILayout.Width(300));
                }
                GUILayout.Label("-------------结束-------------", GUILayout.Width(200));
            }
        }
        GUILayout.EndScrollView();
    }

    private void OpenDfMaterial(DfMaterialObject dfMaterialObject) {
        bool newFlag = !dfMaterialObject.isOpen;
        for (int j = 0; j < _dfMaterialObjects.Count; j++) {
            _dfMaterialObjects[j].isOpen = false;
        }
        dfMaterialObject.isOpen = newFlag;
        if (newFlag) {
            dfMaterialObject.details = new List<Object>();
            Scene scene = EditorSceneManager.OpenScene(dfMaterialObject.path);
            GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goList) {
                if (go != null) {
                    LookupAllDfMaterial(go.transform, dfMaterialObject, 3);
                }
            }
        }
    }

    private void ModifySingleDfMaterial(DfMaterialObject dfMaterialObject) {
        //string filename = FilePathHelper.GetFileName(dfMaterialObject.path);
        string filename = Path.GetFileNameWithoutExtension(dfMaterialObject.path);
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.name != filename) {
            scene = EditorSceneManager.OpenScene(dfMaterialObject.path);
        }
        GameObject[] goList = scene.GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllDfMaterial(go.transform, dfMaterialObject, 2);
            }
        }
        EditorSceneManager.SaveScene(scene);
        for (int i = 0; i < _dfMaterialObjects.Count; i++) {
            if (dfMaterialObject.path == _dfMaterialObjects[i].path) {
                _dfMaterialObjects.RemoveAt(i);
            }
        }
    }

    private void CheckDfMaterial() {
        _dfMaterialObjects = new List<DfMaterialObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkDfMaterialTotal = filePaths.Count;
        for (int i = 0; i < _checkDfMaterialTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在检查文件：", filename + "---" + i + "/" + _checkDfMaterialTotal, (float)i / _checkDfMaterialTotal);
            CheckOneDfMaterialFile(filePaths[i], 1);
        }
        EditorUtility.ClearProgressBar();
    }

    private void ModifyDfMaterial() {
        _dfMaterialObjects = new List<DfMaterialObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkDfMaterialTotal = filePaths.Count;
        for (int i = 0; i < _checkDfMaterialTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在修改文件：", filename + "---" + i + "/" + _checkDfMaterialTotal, (float)i / _checkDfMaterialTotal);
            CheckOneDfMaterialFile(filePaths[i], 2);
        }
        EditorUtility.ClearProgressBar();
        _dfMaterialObjects = null;
        ShowTips("修改完成");
    }

    private void CheckOneDfMaterialFile(string path, int op) {
        Scene scene = EditorSceneManager.OpenScene(path);
        string relativePath = path.Replace(FilePathHelper.GetProjectPath(), "");
        DfMaterialObject dfMaterialObj = new DfMaterialObject();
        dfMaterialObj.hasDf = false;
        GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllDfMaterial(go.transform, dfMaterialObj, op);
            }
        }
        if (dfMaterialObj.hasDf) {
            dfMaterialObj.obj = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
            dfMaterialObj.path = path;
            _dfMaterialObjects.Add(dfMaterialObj);
        }
        if (op == 2 && dfMaterialObj.hasDf) {
            EditorSceneManager.SaveScene(scene);
        }
    }

    // op=1查找，op=2替换, op=3详细
    private void LookupAllDfMaterial(Transform tf, DfMaterialObject dfMaterialObj, int op) {
        if (op == 1 && dfMaterialObj.hasDf) {
            return;
        }
        MeshRenderer renderer = tf.GetComponent<MeshRenderer>();
        if (renderer && renderer.sharedMaterial == null) {
            Debug.Log("");
        }
        if (renderer && renderer.enabled == false && renderer.sharedMaterial != null && renderer.sharedMaterial.ToString() == "Default-Material (UnityEngine.Material)") {
            dfMaterialObj.hasDf = true;
            if (op == 2) {
                renderer.material = null;
            } else if (op == 3) {
                dfMaterialObj.details.Add(tf.gameObject);
            }
        }
        foreach (Transform item in tf) {
            LookupAllDfMaterial(item, dfMaterialObj, op);
        }
    }

    private void ShowShaderCtrl() {
        ShowLine("操作");
        GUILayout.Label("作用：检查Lagacy Shader/Diffuse，把其替换成Mobile Shader/Diffuse");
        GUILayout.Label("请把需要查找的文件或文件夹(ART/Scene，ART/Scene2)拖到下面的框中");
        GUILayout.Label("如果没有指定，则默认选取ART/Scene和ART/Scene2");
        _checkFolder = EditorGUILayout.ObjectField(_checkFolder, typeof(Object), GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            CheckShader();
        }
        if (GUILayout.Button("全部修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyShader();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowShaderResult() {
        ShowLine("结果");
        if (_shaderObjects == null || _shaderObjects.Count == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("搜索结果：" + _shaderObjects.Count + "个，一共搜索：" + _checkShaderTotal + "个", GUILayout.Width(200));
        GUILayout.EndHorizontal();
        _shaderResultPos = GUILayout.BeginScrollView(_shaderResultPos, GUILayout.Width(450), GUILayout.Height(410));
        for (int i = 0; i < _shaderObjects.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_shaderObjects[i], typeof(Object), GUILayout.Width(300));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void CheckShader() {
        _shaderObjects = new List<Material>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetAllFiles(dirPaths[i], "mat");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkShaderTotal = filePaths.Count;
        for (int i = 0; i < _checkShaderTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            string relativePath = filePaths[i].Replace(FilePathHelper.GetProjectPath(), "");
            EditorUtility.DisplayProgressBar("正在检查文件：", filename + "---" + i + "/" + _checkShaderTotal, (float)i / _checkShaderTotal);
            Material mt = AssetDatabase.LoadAssetAtPath<Material>(relativePath);
            if (mt && mt.shader.name == "Legacy Shaders/Diffuse" && mt.color == Color.white) {
                _shaderObjects.Add(mt);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private void ModifyShader() {
        if (_shaderObjects == null || _shaderObjects.Count == 0) {
            ShowTips("请先检查完再修改");
            return;
        }
        Shader sd = Shader.Find("Mobile/Diffuse");
        for (int i = 0; i < _shaderObjects.Count; i++) {
            Material mt = _shaderObjects[i];
            mt.shader = sd;
            EditorUtility.SetDirty(mt);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _shaderObjects = null;
        ShowTips("修改完成");
    }

    private void ShowLayerCtrl() {
        if (_layerList.Count == 0) {
            int[] temp = { 0, 9, 10, 11, 12, 13, 14, 15, 16, 19, 20, 25 };
            for (int i = 0; i < temp.Length; i++) {
                _layerList.Add(temp[i]);
            }
        }
        ShowLine("操作");
        GUILayout.Label("请把需要查找的文件或文件夹(ART/Scene，ART/Scene2)拖到下面的框中");
        GUILayout.Label("如果没有指定，则默认选取ART/Scene和ART/Scene2");
        _checkFolder = EditorGUILayout.ObjectField(_checkFolder, typeof(Object), GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("全部修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyLayer();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowLayerResult() {
        ShowLine("结果");
        if (_checkLayerTotal == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("修改了：" + _modifyLayerTotal + "个，一共搜索：" + _checkLayerTotal + "个", GUILayout.Width(200));
    }

    private void CheckOneLayerFile(string path) {
        int num = 0;
        Scene scene = EditorSceneManager.OpenScene(path);
        string relativePath = path.Replace(FilePathHelper.GetProjectPath(), "");
        GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                num += LookupAllLayer(go.transform);
            }
        }
        if (num > 0) {
            _modifyLayerTotal++;
            EditorSceneManager.SaveScene(scene);
        }
    }

    private int LookupAllLayer(Transform tf) {
        int num = 0;
        //if (tf.gameObject.layer != 0 && tf.gameObject.layer != 13) {
        if (_layerList.IndexOf(tf.gameObject.layer) < 0) {
            tf.gameObject.layer = 0;
            num++;
        }
        foreach (Transform item in tf) {
            num += LookupAllLayer(item);
        }
        return num;
    }

    private void ModifyLayer() {
        _modifyLayerTotal = 0;
        _checkLayerTotal = 0;
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkLayerTotal = filePaths.Count;
        for (int i = 0; i < filePaths.Count; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在修改文件：", filename + "---" + i + "/" + filePaths.Count, (float)i / filePaths.Count);
            CheckOneLayerFile(filePaths[i]);
        }
        EditorUtility.ClearProgressBar();
        ShowTips("修改完成");
    }

    private void ShowLightCtrl() {
        ShowLine("操作");
        GUILayout.Label("请把需要查找的文件或文件夹(ART/Scene，ART/Scene2)拖到下面的框中");
        GUILayout.Label("如果没有指定，则默认选取ART/Scene和ART/Scene2");
        _checkFolder = EditorGUILayout.ObjectField(_checkFolder, typeof(Object), GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            CheckLight();
        }
        if (GUILayout.Button("全部修改", GUILayout.Width(120), GUILayout.Height(50))) {
            ModifyLight();
        }
        GUILayout.EndHorizontal();
    }

    private void ShowLightResult() {
        ShowLine("结果");
        if (_lightObjects == null || _lightObjects.Count == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("搜索结果：" + _lightObjects.Count + "个，一共搜索：" + _checkLightTotal + "个", GUILayout.Width(200));
        GUILayout.EndHorizontal();
        _lightResultPos = GUILayout.BeginScrollView(_lightResultPos, GUILayout.Width(450), GUILayout.Height(410));
        for (int i = 0; i < _lightObjects.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_lightObjects[i].obj, typeof(Object), GUILayout.Width(300));
            if (GUILayout.Button(_lightObjects[i].isOpen ? "缩起" : "展开", GUILayout.Width(50), GUILayout.Height(20))) {
                OpenLight(_lightObjects[i]);
            }
            if (GUILayout.Button("修改", GUILayout.Width(50), GUILayout.Height(20))) {
                ModifySingleLight(_lightObjects[i]);
            }
            GUILayout.EndHorizontal();
            if (_lightObjects[i].isOpen) {
                GUILayout.Label("-------------开始-------------", GUILayout.Width(200));
                GUILayout.Label("一共：" + _lightObjects[i].details.Count + "个子项", GUILayout.Width(200));
                for (int j = 0; j < _lightObjects[i].details.Count; j++) {
                    EditorGUILayout.ObjectField(_lightObjects[i].details[j], typeof(Object), GUILayout.Width(300));
                }
                GUILayout.Label("-------------结束-------------", GUILayout.Width(200));
            }
        }
        GUILayout.EndScrollView();
    }

    private void OpenLight(LightObject lightObject) {
        bool newFlag = !lightObject.isOpen;
        for (int j = 0; j < _lightObjects.Count; j++) {
            _lightObjects[j].isOpen = false;
        }
        lightObject.isOpen = newFlag;
        if (newFlag) {
            lightObject.details = new List<Object>();
            Scene scene = EditorSceneManager.OpenScene(lightObject.path);
            GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in goList) {
                if (go != null) {
                    LookupAllLight(go.transform, lightObject, 3);
                }
            }
        }
    }

    private void ModifySingleLight(LightObject lightObject) {
        //string filename = FilePathHelper.GetFileName(dfMaterialObject.path);
        string filename = Path.GetFileNameWithoutExtension(lightObject.path);
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.name != filename) {
            scene = EditorSceneManager.OpenScene(lightObject.path);
        }
        GameObject[] goList = scene.GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllLight(go.transform, lightObject, 2);
            }
        }
        EditorSceneManager.SaveScene(scene);
        for (int i = 0; i < _lightObjects.Count; i++) {
            if (lightObject.path == _lightObjects[i].path) {
                _lightObjects.RemoveAt(i);
            }
        }
    }

    private void CheckLight() {
        _lightObjects = new List<LightObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkLightTotal = filePaths.Count;
        for (int i = 0; i < _checkLightTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在检查文件：", filename + "---" + i + "/" + _checkLightTotal, (float)i / _checkLightTotal);
            CheckOneLightFile(filePaths[i], 1);
        }
        EditorUtility.ClearProgressBar();
    }

    private void ModifyLight() {
        _lightObjects = new List<LightObject>();
        string objPath = AssetDatabase.GetAssetPath(_checkFolder);
        List<string> dirPaths = new List<string>();
        List<string> filePaths = new List<string>();
        if (string.IsNullOrEmpty(objPath)) {
            dirPaths.Add("Assets/ART/Scene");
            dirPaths.Add("Assets/ART2/Scene");
        } else if (FileManager.IsDirectoryExists(objPath)) {
            dirPaths.Add(objPath);
        } else {
            objPath = FilePathHelper.GetProjectPath() + objPath;
            filePaths.Add(objPath);
        }
        for (int i = 0; i < dirPaths.Count; i++) {
            List<string> files = FileManager.GetSubFiles(dirPaths[i], "unity");
            for (int j = 0; j < files.Count; j++) {
                filePaths.Add(files[j]);
            }
        }
        _checkLightTotal = filePaths.Count;
        for (int i = 0; i < _checkDfMaterialTotal; i++) {
            string filename = FilePathHelper.GetFileName(filePaths[i]);
            EditorUtility.DisplayProgressBar("正在修改文件：", filename + "---" + i + "/" + _checkLightTotal, (float)i / _checkLightTotal);
            CheckOneDfMaterialFile(filePaths[i], 2);
        }
        EditorUtility.ClearProgressBar();
        _dfMaterialObjects = null;
        ShowTips("修改完成");
    }

    private void CheckOneLightFile(string path, int op) {
        Scene scene = EditorSceneManager.OpenScene(path);
        string relativePath = path.Replace(FilePathHelper.GetProjectPath(), "");
        LightObject lightObj = new LightObject();
        lightObj.hasLight = false;
        GameObject[] goList = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in goList) {
            if (go != null) {
                LookupAllLight(go.transform, lightObj, op);
            }
        }
        if (lightObj.hasLight) {
            lightObj.obj = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
            lightObj.path = path;
            _lightObjects.Add(lightObj);
        }
        if (op == 2 && lightObj.hasLight) {
            EditorSceneManager.SaveScene(scene);
        }
    }

    // op=1查找，op=2替换, op=3详细
    private void LookupAllLight(Transform tf, LightObject lightObj, int op) {
        if (op == 1 && lightObj.hasLight) {
            return;
        }
        Light light = tf.GetComponent<Light>();
        //SceneLight sl = tf.GetComponent<SceneLight>();
        //ShadowConfig sc = tf.GetComponent<ShadowConfig>();
        //if (light != null && sl != null && sc != null && (light.enabled || sl.lit == null || sc.lit == null)) {
        if (light != null && light.enabled == true) {
            lightObj.hasLight = true;
            if (op == 2) {
                light.enabled = false;
                //sl.lit = light;
                //sc.lit = light;
            } else if (op == 3) {
                lightObj.details.Add(tf.gameObject);
            }
        }
        foreach (Transform item in tf) {
            LookupAllLight(item, lightObj, op);
        }
    }

    #region 其他显示

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="content"></param>
    private void ShowTips(string content) {
        EditorUtility.DisplayDialog("提示", content, "确定");
    }

    /// <summary>
    /// 显示横条分割
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    private void ShowLine(string t = "", float w = -1, float h = -1) {
        string content = "";
        float ww;
        float hh;
        if (!string.IsNullOrEmpty(t)) {
            content = t;
        }
        if (string.IsNullOrEmpty(content)) {
            if (w < 0) {
                ww = this.maxSize.x;
            } else {
                ww = w;
            }
            if (h < 0) {
                hh = 5;
            } else {
                hh = h;
            }
        } else {
            if (w < 0) {
                ww = this.maxSize.x;
            } else {
                ww = w;
            }
            if (h < 0) {
                hh = 20;
            } else {
                hh = h;
            }
        }
        GUILayout.Box(content, GUILayout.Width(ww), GUILayout.Height(hh));
    }
    #endregion

    public class StaticObject
    {
        public bool hasStatic;
        public Object obj;
        public string path;
    }

    public class DfMaterialObject
    {
        public bool hasDf;
        public Object obj;
        public string path;
        public bool isOpen;
        public List<Object> details = new List<Object>();
    }

    public class LightObject
    {
        public bool hasLight;
        public Object obj;
        public string path;
        public bool isOpen;
        public List<Object> details = new List<Object>();
    }

    public class MainObject
    {
        public List<Object> staticList = new List<Object>();
        public List<Object> dfMaterialList = new List<Object>();
        public List<Object> shaderList = new List<Object>();
        public List<Object> layerList = new List<Object>();
        public List<Object> lightList = new List<Object>();
    }
}