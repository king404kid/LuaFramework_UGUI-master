using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GuideEditor
{
    public class GuideEditorWin : EditorWindow
    {
        static private GuideEditorWin _instance;
        private GuideSaveData data;       // 管理引导列表
        private int tagType = 1;          // 控制列表和编辑页面
        private bool isListDirty = true;
        private string loadPath = "";     // 文本路径
        private Vector2 listPos = new Vector2();
        private int addGuideId = 0;
        private List<GuideData> guideDataList;
        private GuideData curGuideData;
        private Vector2 stepPos = new Vector2();
        private int insertPosIndex = 0;
        private int curStepIndex = 0;
        private GameObject curTargetObj;
        private GameObject oldTargetObj;
        private string[] dirStrs = new string[] { "左上", "右上", "左下", "右下" };
        private string[] speTagStrs = new string[] { "普通", "上折叠按钮", "下折叠按钮", "主界面", "右边或者下边的菜单按钮", "关闭下折叠", "打开顶部按钮" };


        public static GuideEditorWin Instance {
            get {
                if (_instance == null) {
                    _instance = new GuideEditorWin();
                    _instance.minSize = _instance.maxSize = new Vector2(600, 600);
                    _instance.titleContent = new GUIContent("新手引导");
                }
                return GuideEditorWin._instance;
            }
        }

        [MenuItem("Tools/新手引导编辑", false, 105)]
        static void ShowWin() {
            GuideEditorWin.Instance.ShowAndLoad();
        }

        void OnGUI() {
            ShowCtrl();
        }

        private void ShowAndLoad() {
            GuideEditorWin.Instance.Show();
            LoadFileData();
        }

        private void ShowCtrl() {
            switch (tagType) {
                case 1://列表
                    ShowList();
                    break;
                case 2://编辑引导
                    ShowDataEdit();
                    break;
            }
        }

        /// <summary>
        /// 切换列表和编辑界面
        /// </summary>
        /// <param name="id"></param>
        private void ChangeTag(int id) {
            tagType = id;
            if (tagType == 2) {
                curStepIndex = 0;
            }
        }

        private void SetDefaultPath() {
            loadPath = Application.dataPath + "/UINew/Config/GuideConfig.txt";
        }

        /// <summary>
        /// 显示列表页面
        /// </summary>
        private void ShowList() {
            ShowLine("引导列表");
            if (data == null) {
                LoadFileData();
            }
            int count = data.GetCount();
            listPos = GUILayout.BeginScrollView(listPos, GUILayout.Width(600), GUILayout.Height(450));
            if (count == 0) {
                GUILayout.Label("没有数据，请先添加");
            } else {
                ShowGuideList();
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            addGuideId = EditorGUILayout.IntField(addGuideId, GUILayout.Width(150));
            if (GUILayout.Button("添加引导", GUILayout.Width(100))) {
                AddGuide(addGuideId);
            }
            if (GUILayout.Button("保存", GUILayout.Width(100))) {
                SaveGuide();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示列表
        /// </summary>
        private void ShowGuideList() {
            GUILayout.BeginHorizontal();
            GUILayout.Box("引导id", GUILayout.Width(100));
            GUILayout.Box("强引导", GUILayout.Width(50));
            GUILayout.Box("引导备注", GUILayout.Width(200));
            GUILayout.Box("步骤数量", GUILayout.Width(50));
            GUILayout.Box("操作", GUILayout.Width(170));
            GUILayout.EndHorizontal();
            if (guideDataList == null || isListDirty == true) {
                guideDataList = data.GetDataList();
                isListDirty = false;
            }
            for (int i = 0; i < guideDataList.Count; i++) {
                ShowOneGuide(guideDataList[i]);
            }
        }

        private void ShowOneGuide(GuideData guideData) {
            GUILayout.BeginHorizontal();
            GUILayout.Box(guideData.id + "", GUILayout.Width(100));
            if (guideData == null) {
                return;
            }
            guideData.isForce = EditorGUILayout.Toggle(guideData.isForce, GUILayout.Width(50));
            guideData.desc = EditorGUILayout.TextField(guideData.desc, GUILayout.Width(200));
            GUILayout.Label(guideData.GetCount() + "", GUILayout.Width(50));
            if (GUILayout.Button("编辑", GUILayout.Width(80))) {
                EditGuide(guideData.id);
            }
            if (GUILayout.Button("删除", GUILayout.Width(80))) {
                DelGuide(guideData.id);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示编辑界面
        /// </summary>
        private void ShowDataEdit() {
            ShowLine("引导步骤编辑");
            if (GUILayout.Button("返回", GUILayout.Width(100))) {
                ChangeTag(1);
            }
            if (curGuideData != null) {
                ShowGuideTitle(curGuideData);
                ShowGuideSteps(curGuideData);
            }
        }

        private void ShowGuideTitle(GuideData d) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("引导编号：", GUILayout.Width(100));
            GUILayout.Label(d.id + "", GUILayout.Width(150));
            GUILayout.Label("引导备注：", GUILayout.Width(100));
            GUILayout.Label(d.desc, GUILayout.Width(150));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示编辑界面列表
        /// </summary>
        /// <param name="d"></param>
        private void ShowGuideSteps(GuideData d) {
            ShowLine("步骤");
            stepPos = GUILayout.BeginScrollView(stepPos, GUILayout.Width(580), GUILayout.Height(400));
            if (d.GetCount() > 0) {
                List<GuideSubData> list = d.GetData();
                if (list != null) {
                    for (int i = 0; i < list.Count; i++) {
                        ShowLine("", 500);
                        ShowOneStep(list[i]);
                    }
                }
            } else {
                GUILayout.Label("请先添加步骤");
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新增步骤", GUILayout.Width(100))) {
                curGuideData.InsertData();
            }
            GUILayout.Label("", GUILayout.Width(100));
            insertPosIndex = EditorGUILayout.IntField(insertPosIndex, GUILayout.Width(50));
            if (GUILayout.Button("插入步骤", GUILayout.Width(100))) {
                curGuideData.InsertData(insertPosIndex - 1);
            }
            if (GUILayout.Button("保存", GUILayout.Width(100))) {
                SaveGuide();
            }
            GUILayout.EndHorizontal();
            if (Application.isPlaying == true) {
                if (GUILayout.Button("播放", GUILayout.Height(40))) {
                    PlayGuide(d.id);
                }
            }
        }

        private void ShowOneStep(GuideSubData d) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(d.id + "", GUILayout.Width(50));
            GUILayout.Label(d.desc, GUILayout.Width(150));
            if (GUILayout.Button("编辑", GUILayout.Width(80))) {
                curStepIndex = d.id;
                RefreshStepData(d);
                return;
            }
            if (GUILayout.Button("删除", GUILayout.Width(80))) {
                curGuideData.DelData(d.id);
                return;
            }
            GUILayout.EndHorizontal();
            if (curStepIndex == d.id) {
                ShowStepEdit(d);
            }
        }
        
        private void RefreshStepData(GuideSubData d) {
            if (string.IsNullOrEmpty(d.path)) {
                curTargetObj = null;
            } else {
                curTargetObj = GameObject.Find(d.path);
            }
        }

        /// <summary>
        /// 显示单个步骤编辑
        /// </summary>
        /// <param name="d"></param>
        private void ShowStepEdit(GuideSubData d) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("备注:", GUILayout.Width(120));
            d.desc = EditorGUILayout.TextField(d.desc, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("目标按钮：", GUILayout.Width(120));
            curTargetObj = (GameObject)EditorGUILayout.ObjectField(curTargetObj, typeof(GameObject), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            if (curTargetObj != null && curTargetObj != oldTargetObj) {
                d.path = GetObjPath(curTargetObj);
                oldTargetObj = curTargetObj;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("相对路径:", GUILayout.Width(120));
            GUILayout.TextArea(d.path, GUILayout.Width(300), GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("按钮类型:", GUILayout.Width(120));
            d.btnType = EditorGUILayout.Popup(d.btnType, speTagStrs, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("箭头方向:", GUILayout.Width(120));
            d.direction = EditorGUILayout.Popup(d.direction, dirStrs, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("对话内容:", GUILayout.Width(120));
            d.content = EditorGUILayout.TextField(d.content, GUILayout.Width(300), GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("x偏移:", GUILayout.Width(120));
            d.offsetX = EditorGUILayout.IntField(d.offsetX, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("y偏移:", GUILayout.Width(120));
            d.offsetY = EditorGUILayout.IntField(d.offsetY, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("触发功能:", GUILayout.Width(120));
            d.systemOpenFuncName = EditorGUILayout.TextField(d.systemOpenFuncName, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("语音:", GUILayout.Width(120));
            d.dialogMusic = EditorGUILayout.TextField(d.dialogMusic, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("延迟触发显示:", GUILayout.Width(120));
            d.delayShowTime = float.Parse(EditorGUILayout.TextField(d.delayShowTime.ToString(), GUILayout.Width(300)));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("触发等级上限:", GUILayout.Width(120));
            d.triggerMaxPlayerLevel = int.Parse(EditorGUILayout.TextField(d.triggerMaxPlayerLevel.ToString(), GUILayout.Width(300)));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("关键帧:", GUILayout.Width(120));
            d.isKeyFrame = EditorGUILayout.Toggle(d.isKeyFrame, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("是否触发主线任务:", GUILayout.Width(120));
            d.isTriggerMainTask = EditorGUILayout.Toggle(d.isTriggerMainTask, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("副本内的时候是否可以执行触发引导:", GUILayout.Width(120));
            d.inDupCanTrigger = EditorGUILayout.Toggle(d.inDupCanTrigger, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("vip就不触发?:", GUILayout.Width(120));
            d.vipNotTrigger = EditorGUILayout.Toggle(d.vipNotTrigger, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("出来就显示黑背景?:", GUILayout.Width(120));
            d.isNeedBlack = EditorGUILayout.Toggle(d.isNeedBlack, GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        private string GetObjPath(GameObject obj) {
            string str = obj.name;
            Transform parentObj = obj.transform.parent;
            while (parentObj != null) {
                str = parentObj.name + "/" + str;
                parentObj = parentObj.transform.parent;
            }
            return str;
        }

        #region 实现
        private void LoadFileData() {
            SetDefaultPath();
            if (FileTools.IsFileExist(loadPath) == false) {
                data = new GuideSaveData();
                ChangeTag(1);
            } else {
                string str = FileTools.ReadFileText(loadPath, true);
                data = AzhaoJson.Json.ToObject<GuideSaveData>(str);
                ChangeTag(1);
            }
        }

        /// <summary>
        /// 添加引导
        /// </summary>
        /// <param name="id"></param>
        private void AddGuide(int id) {
            if (data.IsGuideExist(id) == true) {
                ShowTips("id已经存在:" + id);
                return;
            }
            data.AddGuide(id);
            isListDirty = true;
        }

        /// <summary>
        /// 编辑引导
        /// </summary>
        /// <param name="id"></param>
        private void EditGuide(int id) {
            curGuideData = data.GetGuideData(id);
            if (curGuideData != null) {
                ChangeTag(2);
            }
        }

        /// <summary>
        /// 删除引导
        /// </summary>
        /// <param name="id"></param>
        private void DelGuide(int id) {
            data.DelGuide(id);
            isListDirty = true;
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void SaveGuide() {
            string str = AzhaoJson.Json.ToJson(data);
            FileTools.SaveFile(loadPath, str, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ShowTips("保存成功：" + loadPath);
        }

        private void PlayGuide(int id) {
            //string luaStr = "modelMgr:instance(\"GuideModel\"):LoadGuideConfig()";
            //LuaFramework.LuaManager.Instance.luaState.DoString(luaStr);

            //luaStr = "modelMgr:instance(\"GuideModel\"):DoingGuide(" + id + ",1)";
            //LuaFramework.LuaManager.Instance.luaState.DoString(luaStr);
        }
        #endregion


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
    }
}