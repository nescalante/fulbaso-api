using System;

namespace Fulbaso.Common
{
    [AttributeUsage(AttributeTargets.All)]
    public class PrimaryField : Attribute
    {
        public readonly string Field;

        public PrimaryField(string field)
        {
            this.Field = field;
        }
    }

}
