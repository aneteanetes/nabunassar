Templates.Base.Object = {
    -- ids
    id="baseobject",
    
    -- leveling
    level=1,
    exp=0,

    icon='#',
    color= { 255,255,255,255},
    colorback = { 255,255,255,255},

    -- autoinit
    init = function (obj)

        -- for all nested objects 

        obj.stats = {
            -- resource
            manamax=0,
            mana_tpl="{mana}/{manamax}",   
        
            -- base
            ap=0,
            ad=0,
            arm=0,
            mres=0,

            -- hp
            basemhp=1,
            mhp=1,
            hp=1,

            -- damage
            mindmg=1,
            maxdmg=1,
        }

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

            local base = tonumber(obj.stats['base'..statKey]);

            print('base'..statKey)

            obj.stats[statKey] = obj.calculateStat(base, flatMods,percentMods,multipleMods)
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