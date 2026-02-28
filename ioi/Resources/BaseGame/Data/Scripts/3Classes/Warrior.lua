Templates.Classes.Warrior = {

    class='Warrior',

    rage=0,

    basemhp=100,
    mhp=100,
    hp=50,
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

    end

}