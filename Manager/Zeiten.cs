using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace ZeitexportWeb.Manager
{
    public class Zeiten
    {
        public static int GetKalenderwoche(DateTime Datum)
        {
            var deCulture = new CultureInfo("DE-de");
            return deCulture.Calendar.GetWeekOfYear(Datum, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static List<Models.ZeitItem> LoadByProjektnummer(string projektnummer, string constring)
        {
            var result = new List<Models.ZeitItem>();

            var qry = @"SELECT 
                        P.Projektnummer
                        ,P.Projektbezeichnung
                        ,Z.Datum
                        ,Z.ZeitVon
                        ,Z.ZeitBis
                        ,Z.Stunden
                        ,Z.Notiz
                        ,ISNULL(R.Ressourcename,Z.Ressource)
                        ,Z.PosID
                        FROM BCSPjmProjekteVorgaengeZeiten AS Z
                        INNER JOIN BCSPjmProjekte AS P ON P.ProjektID = Z.ProjektID AND P.Mandant = Z.Mandant
                        INNER JOIN BCSPjmRessourcen AS R ON R.Ressourcenummer = Z.Ressource AND R.Mandant = Z.Mandant
                        WHERE P.Projektnummer = @Projektnummer";

            //Verbindung zur Datenbank herstellen
            using (var con = new SqlConnection(constring))
            {
                con.Open();

                // Eine Abfrage ausführen
                using (var cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Projektnummer", projektnummer);

                    //Ergebnisse der Abfrage auslesen
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new Models.ZeitItem();
                            result.Add(item);
                            //Prüfen, ob Wert vorhanden, weil DBNull != null in c#
                            if (!dr.IsDBNull(0))
                                item.Projektnummer = dr.GetString(0);
                            if (!dr.IsDBNull(1))
                                item.Projektbezeichnung = dr.GetString(1);
                            if (!dr.IsDBNull(2))
                                item.Datum = dr.GetDateTime(2);
                            if (!dr.IsDBNull(3))
                                item.ZeitVon = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4))
                                item.ZeitBis = dr.GetDateTime(4);
                            if (!dr.IsDBNull(5))
                                item.Stunden = dr.GetSqlMoney(5).ToDouble();
                            if (!dr.IsDBNull(6))
                                item.Notiz = dr.GetString(6);
                            if (!dr.IsDBNull(7))
                                item.Ressource = dr.GetString(7);
                        }
                    }
                }
                con.Close();
            }
            return result;
        }
    }
}
