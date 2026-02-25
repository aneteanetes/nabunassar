Templates.Enemies.Rat = {
    -- ids
    id="moveable",
    
    init = function (obj)
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

        obj.ad=5;
        obj.def=3;
        obj.mdef=2;

        -- hp
        obj.basemhp=10;
        obj.mhp=10;
        obj.hp=10;

        -- damage
        obj.mindmg=1;
        obj.maxdmg=2;

        obj.icon='r';
        obj.color={173,113,56};

    end

}