Templates.Enemies.Rat = {
    -- ids
    id="moveable",
    
    init = function (obj)
        obj.name="rat_enemy";
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

        obj.stats.ad=5;
        obj.stats.def=3;
        obj.stats.mdef=2;

        -- hp
        obj.stats.basemhp=10;
        obj.stats.mhp=10;
        obj.stats.hp=10;

        -- damage
        obj.stats.mindmg=1;
        obj.stats.maxdmg=2;

        obj.icon='r';
        obj.color={173,113,56};

    end

}