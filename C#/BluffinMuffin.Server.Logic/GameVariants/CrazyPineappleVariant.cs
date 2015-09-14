using System;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Server.DataTypes.Attributes;
using BluffinMuffin.Server.Logic.GameModules;

namespace BluffinMuffin.Server.Logic.GameVariants
{
    [GameVariant(GameSubTypeEnum.CrazyPineapple)]
    public class CrazyPineappleVariant : AbstractGameVariant
    {
        public override int NbCardsInHand => 3;

        public override Type InitModuleType => typeof (InitCrazyPineappleGameModule);
    }
}
