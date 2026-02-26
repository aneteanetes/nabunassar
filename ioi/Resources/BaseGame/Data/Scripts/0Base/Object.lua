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

    manamax=0,
    mana_tpl="{mana}/{manamax}",   
        
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

    resstring = function (obj)
        return (obj.mana or '')..'/'..(obj.manamax or '');
    end,

    collide = function(self,selfentity,objmap,collision)
	end,

    strike = function (self,target)
        
        local ctx = DamageContext:new();

        local dmg = math.random(self.mindmg,self.maxdmg+1)+(0.25*self.ad);

        ctx.attacked = math.floor(dmg);

        dmg = target.applydmg(target,dmg,self,ctx);

        ctx.attacker="/c[#00FFFF]"..(self.name or "player");
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

        self.hp = self.hp-math.floor(dmg);

        if(self.hp<=0) then
            self.die(self,attacker,ctx);
            attacker.kill(attacker,self,ctx);
        end
        
        local targetColor = toHexString(self.color);
        ctx.target="/c["..targetColor.."]"..loco(self.name);

        ctx:log();
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
        world.CombatSystem:Kill(self.entity);
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

    refresh = function(obj)
        
        local mods={}
        
	    if(obj.perks ~= nil) then
            for perkname, perk in pairs(obj.perks) do
                for modkey,mod in pairs(perk.mods) do
                    table.insert(mods,mod)
                end
            end
        end

        local modsByStat = Core.groupby(mods,function(m) return m.stat end)

        for statKey, oneStatTable in pairs(modsByStat) do

            local flatMods={}
            local percentMods={}
            local multipleMods={}

            for _,mod in pairs(oneStatTable) do
                obj.addmod(mod,flatMods,percentMods,multipleMods)
            end

            local base = tonumber(obj['base'..statKey]);

            obj[statKey] = obj.calculateStat(base, flatMods,percentMods,multipleMods)
        end

        obj.hp=obj.mhp;

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

    calculateStat = function (base,flatMods,percentMods,multipleMods)
        
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
        return (base+flat) * percent * multi
    end,

}