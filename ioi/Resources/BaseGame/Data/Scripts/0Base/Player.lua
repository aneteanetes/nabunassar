Templates.Base.Player = {
	collide = function(self,world,hostobjmap,objmap)
		if objmap.Entity["type"]=='enemy' then
			objmap:StopMove();
			hostobjmap:StopMove();
			world.CombatSystem:StartCombat(objmap.Entity);
		end
	end
}