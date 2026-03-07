Templates.spawner.rat = {
	init = function(obj)
		-- body
	end,
	
    collide = function(self,selfentity,objmap,collision)
		if collision.Entity["type"]=='player' then
			world.SpawnSystem.SpawnObjectMap("enemy","rat",{ class="bruiser"},11,4,"Consolas1",81);
		end
	end,

}