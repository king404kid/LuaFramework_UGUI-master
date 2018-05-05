-- !@#start
testtoolbuildscene = 
{
  --!@#definestart
  -- UnityEngine.UI.Button
  myBtn_btn=nil,
  -- GameObject
  myGo_go=nil,
  -- UnityEngine.UI.Image
  myImg_img=nil,
  -- UnityEngine.UI.Text
  myText_text=nil,

  --!@#defineend
}

function testtoolbuildscene.New(obj)     
    local s = obj or {}      
    return setmetatable(s, { __index = testtoolbuildscene })
end

function testtoolbuildscene:set_myImg_img(atlas,name)
    local sprite = UITools:loadSprite(atlas, name);
    if self.myImg_img ~= nil and sprite ~= nil then
        self.myImg_img.sprite = sprite;
    end
end
function testtoolbuildscene:set_myText_text(str)
    if self.myText_text ~= nil then
        self.myText_text.text = str;
    end
end

-------------输入监听--------------

function testtoolbuildscene:RegisterClick()
  self.lb:AddSelfClick(self.myBtn_btn.gameObject, self.myBtnClick,self);

end

-- !@#startClick

--!@#myBtnClick
function testtoolbuildscene:myBtnClick()
  self:ClickHandler()
end
--!@#myBtnClick

-- !@#endClick
-- !@#end
------------生命周期------------------


-- 开始时调用
function testtoolbuildscene:Awake(obj)

self:AddListener();
end

function testtoolbuildscene:Start()
  
end
function testtoolbuildscene:Show(data)
  
end
function testtoolbuildscene:SetTitle(titleStr)
	if self.comBackdrop ~= nil then
		self.comBackdrop:SetTitle(titleStr)
	end
end

-- 预设销毁时调用
function testtoolbuildscene:OnClose()
    self:RemoveListener();
end
function testtoolbuildscene:AddListener()
    self:RegisterClick();
end
function testtoolbuildscene:RemoveListener()
    
end
function testtoolbuildscene:OnActiveSelfLabelPage()

end
function testtoolbuildscene:OnHideSelfLabelPage()

end
function testtoolbuildscene:OnPool()
    
end

function testtoolbuildscene:ClickHandler()
  print("test")
end