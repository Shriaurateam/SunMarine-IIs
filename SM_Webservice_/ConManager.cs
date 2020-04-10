using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace SM_Webservice_
{
    public class ConManager
    {
        public SqlConnection sqlConnection
        {
            get;
            set;
        }
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Aura"].ConnectionString;
            }
        }
        public ConManager()
        {
            OpenConnetion();
        }
        public void OpenConnetion()
        {
            if (this.sqlConnection == null || this.sqlConnection.State == ConnectionState.Closed) { this.sqlConnection = new SqlConnection(this.ConnectionString); }
        }

        public void CloseConnection()
        {
            if (this.sqlConnection != null || this.sqlConnection.State != ConnectionState.Closed || this.sqlConnection.State != ConnectionState.Broken) { this.sqlConnection.Close(); }
            this.sqlConnection = null;
        }

        //---------------------
    }
}