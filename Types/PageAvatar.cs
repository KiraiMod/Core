using System;
using System.Linq;
using System.Reflection;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class PageAvatar
    {
        public static Type Type = ClassDef.Create("PageAvatar")
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .ConstructorCount(2)
            .WithMethods(
                MethodDef.Create().WithName("ChangeToSelectedAvatar")
            )
            .Setup()
            .Resolved;

        public static PropertyInfo[] Pedestal = Type?.GetProperties().Where(x => x.PropertyType == SimpleAvatarPedestal.Type).ToArray();

        static PageAvatar() => Type.LogAs(nameof(PageAvatar));
    }
}
