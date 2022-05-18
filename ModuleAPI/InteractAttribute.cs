using System;
using System.Reflection;

namespace KiraiMod.Core.ModuleAPI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InteractAttribute : MemberAttribute
    {
        public Action Invoke;

        public InteractAttribute(string Path) : base(Path) { }

        public override void Setup(Type Type, MemberInfo minfo)
        {
            base.Setup(Type, minfo);
            if (minfo is MethodInfo method)
            {
                if (method.ReturnType != typeof(void)) throw new Exception("method must return void");
                if (method.GetParameters().Length != 0) throw new Exception("method must have no parameters");

                Invoke = (Action)Delegate.CreateDelegate(typeof(Action), method);
            }
        }
    }
}
