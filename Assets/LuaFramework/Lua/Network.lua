Network = {};
 
--协议
Protocal = {
	Connect		= '101';	--连接服务器
	Exception   = '102';	--异常掉线
	Disconnect  = '103';	--正常断线   
	Message		= '104';	--接收消息
}
 
--Socket消息--
function Network.OnSocket(key, data)
	if key == tonumber(Protocal.Connect) then
		LuaFramework.Util.Log('OnSocket Connect');		
	else
		LuaFramework.Util.Log('OnSocket Other');	
	end
end