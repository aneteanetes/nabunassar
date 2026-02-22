Templates.Classes.Warrior = {
    init = function(obj)
    
        -- сюда придём после всех предыдущих init

        obj.class='Warrior';


        if obj.stats == nil then
            obj.stats={}
        end

        obj.stats.rage=0;
        obj.stats.basemhp=100;
        obj.stats.mhp=100;
        obj.stats.hp=100;
        obj.stats.mindmg=7
        obj.stats.maxdmg=11
        
        if(obj.perks==nil) then
            obj.perks={};
        end

        table.insert(obj.perks,Templates.Perks.Experienced);
        table.insert(obj.perks,Templates.Perks.Human);

    end

}