using System;
using System.Web;

namespace Fulbaso.EntityFramework
{
    public static class EntityUtil
    {
        private const string DATACONTEXT_ITEMS_KEY = "DataContextKey";

        [ThreadStatic]
        private static ObjectContextEntities _context = null;

        public static ObjectContextEntities Context
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    if (_context == null) _context = NewContext();

                    return _context;
                }
                else
                {
                    if (!HttpContext.Current.Items.Contains(DATACONTEXT_ITEMS_KEY))
                    {
                        HttpContext.Current.Items[DATACONTEXT_ITEMS_KEY] = new ObjectContextEntities();
                    }

                    return (ObjectContextEntities)HttpContext.Current.Items[DATACONTEXT_ITEMS_KEY];
                }
            }
        }

        public static ObjectContextEntities NewContext()
        {
            return new ObjectContextEntities();
        }
    }
}
