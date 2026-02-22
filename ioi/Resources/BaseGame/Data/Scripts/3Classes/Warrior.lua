Templates.Classes.Warrior = {

    class='Warrior',

    init = function(obj)
    
        -- сюда придём после всех предыдущих init

        if obj.stats == nil then
            obj.stats={}
        end

        obj.stats.rage=0;
        obj.stats.basemhp=100;
        obj.stats.mhp=100;
        obj.stats.hp=100;
        obj.stats.mindmg=7
        obj.stats.maxdmg=11

        obj.res='rage';
        obj.rescolor={255,0,0,255};
        obj.resstring = function (obj)
            return tostring(obj.stats.rage);
        end
        
        if(obj.perks==nil) then
            obj.perks={};
        end

        table.insert(obj.perks,Templates.Perks.Experienced);
        table.insert(obj.perks,Templates.Perks.Human);

    end

}