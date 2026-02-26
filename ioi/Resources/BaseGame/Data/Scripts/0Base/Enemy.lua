Templates.Base.Enemy = {
	collide = function(self,selfentity,objmap,collision)
		if collision.Entity["type"]=='player' then
			collision:StopMove();
			objmap:StopMove();
			world.CombatSystem:StartCombat(selfentity);
		end
	end
}