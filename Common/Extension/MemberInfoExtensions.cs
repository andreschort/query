using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extension
{
    public static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).FieldType;

                case MemberTypes.Method:
                    return ((MethodInfo) memberInfo).ReturnType;

                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).PropertyType;
            }

            return null;
        }
    }
}
