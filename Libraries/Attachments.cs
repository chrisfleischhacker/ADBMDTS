using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Odbc;
using System.Configuration;

namespace ADBMDTS
{
    public class Attachment
    {
        public Attachment() { }

        private Attachment(
            int Logx,
            int ERefNo
            ) { }

        public int Logx { get; set; }        
        public int ERefNo { get; set; }

       public static Attachment AttachmentItem(OdbcDataReader reader)
        {
            Attachment attachment = new Attachment();
            if (reader.IsClosed)
                reader.Read();

            attachment.Logx = Convert.ToInt32(reader["LogX"]);
            attachment.ERefNo = Convert.ToInt32(reader["ERefNo"]);
            return attachment;
        }

       public static void ClearAttachmentChangedFlag(Attachment a)
       {
           ClearAttachmentChangedFlag(
                a.Logx
               , a.ERefNo
               );
       }

       public static void ClearAttachmentChangedFlag(int _logx, int _erefno)
       {
           string update;
           update = "update _Attachments set changed = 0 where logx = " + _logx + " and erefno = " + _erefno;
           OdbcConnection conn = new OdbcConnection(ConfigurationManager.AppSettings["adbmdsn"]);
           OdbcCommand cmdUpdateAttachment = new OdbcCommand(update, conn);
           try
           {
               conn.Open();
               cmdUpdateAttachment.ExecuteNonQuery();
           }
           catch (Exception eODBC)
           {
               string xxx = eODBC.Message;
           }
           finally
           {
               conn.Close();
           }
       }
    }

    public class AttachmentCollection : List<Attachment>
    {
    }

    public class Attachments 
    {
        public static AttachmentCollection GetCollection(string selection)
        {
            AttachmentCollection ac = new AttachmentCollection();
            OdbcConnection conn = new OdbcConnection(ConfigurationManager.AppSettings["adbmdsn"]);
            OdbcCommand cmdSalesman = new OdbcCommand(selection, conn);
            try
            {
                conn.Open();
                OdbcDataReader dr;
                dr = cmdSalesman.ExecuteReader();
                FillList(ac, dr);
                dr.Close();
            }
            catch (Exception eODBC)
            {
                string xxx = eODBC.Message;
            }
            finally
            {
                conn.Close();
            }
            return ac;
        }

        public static void FillList(AttachmentCollection coll, OdbcDataReader reader)
        {
            FillList(coll, reader, -1, 0);
        }

        public static void FillList(AttachmentCollection coll, OdbcDataReader reader, int totalRows, int firstRow)
        {
            int index = 0;
            bool readMore = true;

            while (reader.Read())
            {
                if (index >= firstRow && readMore)
                {
                    if (coll.Count >= totalRows && totalRows > 0)
                        readMore = false;
                    else
                    {
                        Attachment attachmentitem = Attachment.AttachmentItem(reader);
                        coll.Add(attachmentitem);
                    }
                }
                index++;
            }
        }
    }
}
