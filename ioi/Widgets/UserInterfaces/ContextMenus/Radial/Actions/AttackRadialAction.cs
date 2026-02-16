using ioi.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioi.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class AttackRadialAction : RadialMenuAction
    {
        public AttackRadialAction(RadialMenu menu) : base(menu, Direction.Left, "attack")
        {
        }
    }
}
