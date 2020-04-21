using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CursorOSI
{
    public static class XmlReaderHelper
    {
        public static DataTable ReadToTable(string xmlPath)
        {
            if (!File.Exists(xmlPath))
            {
                return null;
            }
            StreamReader reader = new StreamReader(xmlPath);
            string source = reader.ReadToEnd();
            reader.Close();
            if (!string.IsNullOrEmpty(source))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    StrStream = new StringReader(source);
                    //获取StrStream中的数据  
                    Xmlrdr = new XmlTextReader(StrStream);
                    //ds获取Xmlrdr中的数据                 
                    ds.ReadXml(Xmlrdr);
                    return ds.Tables[0];
                }
                catch (Exception e)
                {
                    throw e;
                    //return null;
                }
                finally
                {
                    //释放资源  
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            return null;
        }
        public static void DataTableToXml(DataTable dt, string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }
            dt.WriteXml(xmlPath);
        }
    }
}
