using System.Data.Objects.DataClasses;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    internal class ReferenceHelper
    {
        public static T GetReference<T>(EntityReference reference) where T : EntityObject
        {
            return ((EntityReference<T>)reference).Value;
        }
    }
}
