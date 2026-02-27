Templates.enemy.rat = {
    -- ids
    id="moveable",
    
    init = function (obj,props)
        local components = obj["_components"];

        table.insert(components,"Templates.Base.Enemy");        
        table.insert(components,"Templates.Races.animal");

        if props.class ~= nil then
            table.insert(components,"Templates.Classes."..props.class);
        else
            table.insert(components,"Templates.Classes.bruiser");
        end

        table.insert(components,"Templates.Base.Moveable");
        
        obj.name="rat_enemy";
        obj.type="enemy";
	    obj.speed = 0.15;
        obj.idleSpeed=10;
        obj.idleAmplitude=.5;
        obj.stepSleepMS=0;
        obj.pathSleepMS=0;

        obj.movearea = {
            x=-9,
            y=-9,
            w=19,
            h=9
        };

        obj.exp=1;

        obj.ad=5;
        obj.def=3;
        obj.mdef=2;

        -- hp
        obj.basemhp=10;
        obj.mhp=10;
        obj.hp=10;

        -- damage
        obj.basemindmg=1;
        obj.basemaxdmg=2;

        obj.icon='r';
        obj.color={173,113,56};

    end

}