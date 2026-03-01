Templates.Classes.Warrior = {

    class='Warrior',

    rage=0,

    basemhp=100,
    mhp=100,
    hp=20,
    basemindmg=2,
    basemaxdmg=4,

    res='rage',
    rescolor={255,0,0,255},
    resstring = function (obj)
        return tostring(obj.rage);
    end,

    init = function(obj,props)
    
        obj.stats_upd.rage=0;

        if(obj.perks==nil) then
            obj.perks={};
        end

        table.insert(obj.perks,Templates.Perks.Experienced);
        table.insert(obj.perks,Templates.Perks.Human);

    end,

    levelup = function (obj)
        obj:add("basemindmg",1);
        obj:add("basemaxdmg",3);
        obj:add("basemhp",3);
        obj:add("spoints",1);
        obj:add("ppoints",1);

        obj:applyheal(obj.basemhp);

        world.LogSystem.Log(obj:coloredName().." /cd"..loco("leveledup")..": +1-3 DMG, +3HP, +1SP, +1PP!");
    end,

}