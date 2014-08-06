using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureEditor
{
    public class FileDateCreated
    {
        public string path;
        public DateTime date;
        public DateTimeOffset dateOffset;

        public FileDateCreated(string pathLocal, DateTime dateLocal)
        {
            path = pathLocal;
            date = dateLocal;
        }

        public FileDateCreated(string pathLocal, DateTimeOffset dateLocal)
        {
            path = pathLocal;
            date = ConvertFromDateTimeOffset(dateLocal);
        }

        static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            else
                return dateTime.DateTime;
        }
    }
}
