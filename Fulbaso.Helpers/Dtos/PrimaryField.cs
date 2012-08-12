using System;

namespace Fulbaso.Helpers
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
