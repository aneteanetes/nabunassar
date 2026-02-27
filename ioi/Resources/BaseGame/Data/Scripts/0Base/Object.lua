Templates.Base.Object = {
    -- ids
    id="baseobject",
    
    -- leveling
    level=1,
    exp=0,

    icon='#',
    color= { 255,255,255,255},
    colorback = { 255,255,255,255},

    res='mana',
    rescolor={0,0,255,255},

    gold=100,

    desc="description",

    mana=0,
    manamax=0,
        
    -- base
    ap=0,
    ad=0,
    def=0,
    mdef=0,

    -- hp
    basemhp=1,
    mhp=1,
    hp=1,

    -- damage
    mindmg=1,
    maxdmg=1,

    stats_upd = {
        mana=0,
        manamax=0,  
        ap=0,
        ad=0,
        def=0,
        mdef=0,
        mhp=1,
        mindmg=1,
        maxdmg=1
    },

    mods={},

    resstring = function (obj)
        return (obj.mana or '')..'/'..(obj.manamax or '');
    end,

    getName = function (self)
        local name = "";
        if (self.name == nil or self.name == "") then
            name = self.namevalue;
        else
            name = loco(self.name);
        end
        return name;
    end,

    collide = function(self,selfentity,objmap,collision)
	end,

    combatturn = function(self,target)
    end,

    strike = function (self,target)
        
        local ctx = DamageContext:new();

        local dmg = math.random(self.mindmg,self.maxdmg+1)+(0.25*self.ad);

        ctx.attacked = math.floor(dmg);

        dmg = target.applydmg(target,dmg,self,ctx);
        
        ctx:log();

        if(ctx.died==true) then
            world.CombatSystem:Kill(target.entity);
        else
            target.combatturn(target,self);
        end
    end,

    defence = function (self,target)

        table.insert(self.mods, {
            id="defenceinbattle_def",    
            type=Templates.Base.Mod.Type.Flat,
            value= self.def == 0 and 1 or self.def,
            stat="def",
            turns=1
        });

        table.insert(self.mods, {
            id="defenceinbattle_mdef",    
            type=Templates.Base.Mod.Type.Flat,
            value=self.mdef == 0 and 1 or self.mdef,
            stat="mdef",
            turns=1
        });

        self.refresh(self);
        
        local targetColor = toHexString(self.color);
        local name = "/c["..targetColor.."]"..self:getName();

        world.CombatSystem.LogCombat(name.." /cd"..loco("defstand").."!");

        target.combatturn(target,self);
    end,

    flee = function (self,target)
        
        local targetColor = toHexString(self.color);
        local name = "/c["..targetColor.."]"..self:getName();

        world.CombatSystem.LogCombat(name.." /cd"..loco("tryflee"));

        if math.random(100) <= (50+self.ap) then
            
            if(self.type=="player") then
                world.LogSystem.Log(loco("youm").." "..loco("successflee").."!");
            end

            world.CombatSystem.LogCombat(name.." /cd"..loco("successflee").."!");
            world.CombatSystem.EndCombat();
        else
            target.combatturn(target,self);
        end

    end,

    applydmg = function (self,dmg,attacker,ctx)

        -- before
        dmg = self.beforedmg(self,dmg,attacker,ctx);
        
        -- usual
        local def = self.def * 0.75;

        dmgdefed = math.clamp(dmg-def,0,dmg);

        local defround = math.floor(def);
        ctx.defed= defround;

        --after calucaltion
        mitigated = self.afterdmg(self,dmgdefed,attacker,ctx);
        
        -- round damage
        mitigated = math.floor(mitigated);

        ctx.dmg=mitigated;

        self.hp = self.hp-mitigated;

        if(self.hp<=0) then
            self.die(self,attacker,ctx);
            attacker.kill(attacker,self,ctx);
        end
        
        local targetColor = toHexString(self.color);

        ctx.target="/c["..targetColor.."]"..self:getName();
    end,

    beforedmg=function (self,dmg,attacker,ctx)
	    -- щиты
        return dmg;
    end,

    afterdmg=function (self,dmg,attacker,ctx)
	    -- отражение урона
        return dmg;
    end,

    die=function (self,killer,ctx)
	    ctx.died=true;
        self.died=true;
    end,

    kill=function (self,target,ctx)
	    -- on kill
    end,

    destroy=function(self)
	    -- on destroy
    end,

    -- autoinit
    init = function (obj,props)

        -- for all nested objects
        obj.perks={}
    end,

    tick = function (obj)

        if(obj.mods ~= nil) then
            local fordel = {}

            for key, mod in pairs(obj.mods) do
                mod.turns = mod.turns - 1;
                if(mod.turns==0) then
                    table.insert(fordel,key);
                end
            end

            for key, mod in pairs(fordel) do
                obj.mods[key] = nil;
            end
        end

        obj.refresh(obj);
    end,

    refresh = function(obj)

        local mods={}
        
	    if(obj.mods ~= nil) then
            for _, mod in pairs(obj.mods) do
                table.insert(mods,mod)
            end
        end
        
	    if(obj.perks ~= nil) then
            for perkname, perk in pairs(obj.perks) do
                for modkey,mod in pairs(perk.mods) do
                    table.insert(mods,mod)
                end
            end
        end

        local modsByStat = Core.groupby(mods,function(m) return m.stat end)

        for statKey,v in pairs(obj.stats_upd) do
            local flatMods={}
            local percentMods={}
            local multipleMods={}

            local allstatmods = modsByStat[statKey];

            if allstatmods ~= nil then
                for _,mod in pairs(allstatmods) do
                    obj.addmod(mod,flatMods,percentMods,multipleMods)
                end
            end

            local base = tonumber(obj['base'..statKey]);

            obj[statKey] = obj.calculateStat(base, flatMods, percentMods, multipleMods,statKey);
        end
    end,

    addmod = function (mod,flatMods,percentMods,multipleMods)
	    if(mod.type==Templates.Base.Mod.Type.Flat) then
            table.insert(flatMods,mod)
        elseif mod.type == Templates.Base.Mod.Type.Percent then
            table.insert(percentMods,mod)
        elseif mod.type == Templates.Base.Mod.Type.Multiple then
            table.insert(multipleMods,mod)
        end
    end,

    calculateStat = function (base,flatMods,percentMods,multipleMods,statkey)
        
        if(base==nil) then
            base=0
        end

        local flat=0
        for _,mod in pairs(flatMods) do
            flat=flat+mod.value
        end

        local percent =1
        for _,mod in pairs(percentMods) do
            percent=percent+mod.value
        end

        local multi =1
        for _,mod in pairs(multipleMods) do
            multi=multi*mod.value
        end

        local value = (base+flat) * percent * multi;

        --print(statkey.." : (base+flat) * percent * multi = "..value.." ("..base.." + "..flat..") * "..percent.." * "..multi)

        return value;
    end,

}