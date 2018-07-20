using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grp2
{
    internal class Program
    {
        private static DateTime GetIDMSDate(string s)
        {
            DateTime.TryParseExact(s, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out var date);
            return date;
        }
        private static void Main(string[] args)
        {
            try
            {
                var membersDoc = XDocument.Load("s:\\FareGroupMembers_v1.0.xml");
                var mapping = membersDoc.Descendants("FareGroup")
                    .Select(x =>
                    new
                    {
                        grp = x.Element("Nlc").Value,
                        members = x.Elements("FareLocation").Select(y => y.Element("Nlc").Value).ToList()
                    }).ToDictionary(d=>d.grp, d=>d.members);

                var faregroupMembers = mapping.SelectMany(x => x.Value).ToHashSet();

                var permDoc = XDocument.Load("s:\\FareGroupPermittedStations_v1.0.xml");

                var permList = permDoc.Descendants("PermittedStations")
                    .Select(x => new
                    {
                        grp = x.Attribute("FareGroupNlc").Value,
                        loc = x.Attribute("FareLocationNlc").Value,
                        route = x.Attribute("RouteCode").Value,
                        startDate = GetIDMSDate(x.Attribute("StartDate").Value),
                        endDate = GetIDMSDate(x.Attribute("EndDate").Value),
                        permittedList = x.Elements("Crs").Select(y => y.Value).ToList()
                    }
                    ).ToDictionary(d=>(grp:d.grp, loc:d.loc, d.route, d.startDate, d.endDate), d=>d.permittedList);

                var permIndividuals = permList.Where(x => faregroupMembers.Contains(x.Key.grp) || faregroupMembers.Contains(x.Key.loc));

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
