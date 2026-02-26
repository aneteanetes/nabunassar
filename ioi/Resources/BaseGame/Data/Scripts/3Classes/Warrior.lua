Templates.Classes.Warrior = {

    class='Warrior',

    init = function(obj,props)
    
        -- сюда придём после всех предыдущих init

        obj.rage=0;
        obj.basemhp=100;
        obj.mhp=100;
        obj.hp=100;
        obj.mindmg=7
        obj.maxdmg=11

        obj.res='rage';
        obj.rescolor={255,0,0,255};
        obj.resstring = function (obj)
            return tostring(obj.rage);
        end
        
        if(obj.perks==nil) then
            obj.perks={};
        end

        table.insert(obj.perks,Templates.Perks.Experienced);
        table.insert(obj.perks,Templates.Perks.Human);

    end

}