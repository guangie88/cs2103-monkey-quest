// Main Contributors: Frank

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public class XMLMap
    {
        private Map map;
        private IList<MapObject> mapObjects;

        public Map MapObject
        {
            get { return map; }
        }

        public IList<MapObject> MapTileObjects
        {
            get { return mapObjects; }
        }

        public XMLMap(string mappath)
        {
            String xmlSource = ReadMap(mappath);

            XElement xmlItems = XElement.Parse(xmlSource);

            // Set rows and cols for map
            map = new Map();
            MapAttribs(map, xmlItems);

            // Map Static child elements
            mapObjects = new List<MapObject>();

            IEnumerable<XElement> elements = from el in xmlItems.Elements("static")
                                             select el;
            foreach (XElement el in elements)
            {
                StaticMapObject o = new StaticMapObject();
                MapAttribs(o, el);
                mapObjects.Add(o);
            }

            elements = from el in xmlItems.Elements("tile")
                       select el;
            foreach (XElement el in elements)
            {
                TileMapObject o = new TileMapObject();
                MapAttribs(o, el);
                mapObjects.Add(o);
            }
        }

        private string ReadMap(String mappath)
        {
            String xmlSource = "";

            StreamReader SR;
            string S;
            //SR = File.OpenText("../../../MonkeyQuest/Resources/Maps/map.xml");
            SR = File.OpenText(mappath);
            S = SR.ReadLine();
            while (S != null)
            {
                xmlSource += S;
                S = SR.ReadLine();
            }
            SR.Close();

            return xmlSource;
        }

        private IEnumerable<XAttribute> ReadAttrib(XElement xmlItems)
        {
            IEnumerable<XAttribute> attList = from at in xmlItems.Attributes()
                                              select at;

            return attList;
        }

        private void MapAttribs(Object o, XElement xmlItems)
        {
            Type t = o.GetType();
            Debug.WriteLine(t.ToString());

            IEnumerable<XAttribute> rootAttList = ReadAttrib(xmlItems);
            foreach (XAttribute att in rootAttList)
            {
                // sorry, no support for capitalization in .NET String class
                String propertyname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(att.Name.LocalName);
                PropertyInfo p = t.GetProperty(propertyname);

                if (p != null)
                {
                    int result;

                    try
                    {
                        Int32.TryParse(att.Value.ToString(), out result);
                        p.SetValue(o, result, null);
                    }
                    catch
                    {
                        p.SetValue(o, att.Value.ToString(), null);
                    }
                }
            }
        }
    }

    public class Map
    {
        private int maprows, mapcols, time;

        public int Rows
        {
            set { maprows = value; }
            get { return maprows; }
        }

        public int Columns
        {
            set { mapcols = value; }
            get { return mapcols; }
        }

        public int Time
        {
            set { time = value; }
            get { return time; }
        }

        public override String ToString()
        {
            return "[map rows=" + Rows + " columns=" + Columns + "]";
        }
    }

    public class MapObject
    {
        private int x, y;

        public int X
        {
            set { x = value; }
            get { return x; }
        }

        public int Y
        {
            set { y = value; }
            get { return y; }
        }
    }

    public class StaticMapObject : MapObject
    {
        private string id;

        public String Identifier
        {
            set { id = value; }
            get { return id; }
        }

        public override String ToString()
        {
            return "[static identifier=" + Identifier + " x=" + X + " y=" + Y + "]";
        }
    }

    public class TileMapObject : MapObject
    {
        private string type, property;

        public String Type
        {
            set { type = value; }
            get { return type; }
        }

        public String Property
        {
            set { property = value; }
            get { return property; }
        }

        public override String ToString()
        {
            return "[tile type=" + Type + " property=" + Property + " x=" + X + " y=" + Y + "]";
        }
    }
}
