DamageContext = {}
DamageContext.__index = DamageContext

function DamageContext:new()
    local obj = {
        attacker="",
        target="",
        attacked=0,
        dmg = 0,
        defed=0,
        died=false,
    }
    setmetatable(obj, self)
    return obj
end

function DamageContext:log()
    local white = " /c[#F5DEB3]";
    local orange = " /c[#f57207]";
    local defcolor = " /c[#026300]";

    local log={}
    table.insert(log,self.target.." /cd"..loco("getting")..orange..tostring(self.dmg).." /cd"..loco("dmgplural"));
    table.insert(log,"/cd"..loco("deals")..white..tostring(self.attacked));
    table.insert(log,"/cd"..loco("defeddmg")..defcolor..tostring(self.defed));


    local msg = table.concat(log,"/cd, ");
    world.CombatSystem.LogCombat(msg.."/cd!");

    if(self.died==true) then
        world.CombatSystem.LogCombat(self.target.." /cd"..loco("diedcombat").."!");
    end
end