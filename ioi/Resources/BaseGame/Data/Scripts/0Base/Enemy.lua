Templates.Base.Enemy = {
	collide = function(self,world,hostobjmap,objmap)
		if objmap.Entity["type"]=='player' then
			objmap:StopMove();
			hostobjmap:StopMove();
			world.CombatSystem:StartCombat(hostobjmap.Entity);
		end
	end
}