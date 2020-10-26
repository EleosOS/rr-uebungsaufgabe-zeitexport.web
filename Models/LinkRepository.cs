using System.Collections.Generic;

namespace ZeitexportWeb.Models
{
    public class LinkRepository
    {
        private static List<string> links = new List<string>();

        public static IEnumerable<string> Links => links;

        public static void AddLink(string link)
        {
            var index = links.IndexOf(link);

            if (index > -1)
            {
                links.RemoveAt(index);
            }

            links.Add(link);

            if (links.Count > 5)
            {
                links.RemoveAt(6);
            }
        }
    }
}
