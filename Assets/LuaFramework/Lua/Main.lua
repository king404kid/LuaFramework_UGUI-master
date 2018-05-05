require "Network"

--主入口函数。从这里开始lua逻辑
function Main()					
	LuaHelper = LuaFramework.LuaHelper;
	resMgr = LuaHelper.GetResManager();
	resMgr:LoadPrefab("testPrefab", {"z_jk_body_5_1_skin"}, OnLoadModelFinish);
	-- resMgr:LoadPrefab("testPrefab", {"Panel"}, OnLoadPanelFinish);

	-- TestLuaFunc.Log()

	-- TestServer()
end

--加载完成后的回调--
function OnLoadModelFinish(objs)
	local go = UnityEngine.GameObject.Instantiate(objs[0]);
end

--加载完成后的回调--
function OnLoadPanelFinish(objs)
	local go = UnityEngine.GameObject.Instantiate(objs[0]);
	local parent = UnityEngine.GameObject.Find("Canvas")
	go.transform:SetParent(parent.transform)
	go.transform.localScale = Vector3.one
	go.transform.localPosition = Vector3.zero
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end

-- 测试连接服务器
function TestServer()
	local LuaHelper = LuaFramework.LuaHelper
	local networkMgr = LuaHelper.GetNetManager()
	local AppConst = LuaFramework.AppConst
	
    AppConst.SocketPort = 1234;
    AppConst.SocketAddress = "127.0.0.1";
	networkMgr:SendConnect();
end