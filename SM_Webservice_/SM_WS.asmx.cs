using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Services.Protocols;
//using System.Web.Script.Serialization;
using System.Web.Script.Services;
//using System.Globalization;

namespace SM_Webservice_
{
    /// <summary>
    /// Summary description for SM_WS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SM_WS : System.Web.Services.WebService
    {
        public SqlConnection Connection;
        public string TokenString = string.Empty;
        public SqlDataAdapter sqlDataAdapter = null;
        public SqlTransaction trans = null;
        public SqlCommand cmnd = null;
        SqlParameter[] SQL_PARAM, Param;
        private int connectiontimeout;

        private ConManager conManager;
        public DataTable cmndt, tdt;
        public string dt, sts, msg = "", mail_id = "", result;
        public string num = "";




        //[WebMethod]
        //[ScriptMethod(UseHttpGet = true)]
        //public void GetRole()
        //{
        //    try
        //    {
        //        Param = new SqlParameter[1];
        //        Param[0] = new SqlParameter("@SP_TYPE", "ROLE");
        //         dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
        //         dt = dt.Replace("[", "");
        //         dt = dt.Replace("]", "");

        //         //Context.Response.Write("[" +
        //         //           "{\"Result\":[" + dt + "]}" +

        //         //           "]");
        //         Context.Response.Write("["+dt+"]");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());

        //        Context.Response.Write("[" +
        //                  "{\"Result\":[" + "{\"CODE\":\"02\"}" + "]}" +

        //                   "]");
        //    }
        //    finally { }
        //}


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetRole()
        {
            try
            {
                Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@SP_TYPE", "ROLE");

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCompany()
        {
            try
            {
                Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@SP_TYPE", "COMPANIES");

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetUserCompany(string US_Id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_USER_COMP");
                Param[1] = new SqlParameter("US_ID", US_Id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }
        //old Login
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Login(string Uname, string Pwd, string Company_id, string MangeRole)
        {
            try
            {
                Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@SP_TYPE", "LOGIN");
                Param[1] = new SqlParameter("@UNAME", Uname);
                Param[2] = new SqlParameter("@PWD", Pwd);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@mode", MangeRole);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }
        //new Login 
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Login_New(string Number, string Pwd)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "LOGIN_US");
                Param[1] = new SqlParameter("@UNAME", Number);
                Param[2] = new SqlParameter("@PWD", Pwd);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Otp_Req(string Number)
        {
            try
            {
                string otp = RandomDigits(4);
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "OTP_REQ");
                Param[1] = new SqlParameter("@UNAME", Number);
                Param[2] = new SqlParameter("@PWD", otp);

                tdt = SQLReturnDT("SM_SP", CommandType.StoredProcedure, Param);
                if (tdt.Rows[0]["CODE"].ToString() == "01")
                {
                    //Number = "9600161227";
                    Forgot_SMS(Number, tdt.Rows[0]["MSG"].ToString());
                }


                return JsonConvert.SerializeObject(tdt);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Otp_Validation(string Number, string OTP)
        {
            try
            {

                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "OTP_VAL");
                Param[1] = new SqlParameter("@UNAME", Number);
                Param[2] = new SqlParameter("@PWD", OTP);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Change_Password(string Number, string Password)
        {
            try
            {

                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "PASSWORD_CHANGE");
                Param[1] = new SqlParameter("@UNAME", Number);
                Param[2] = new SqlParameter("@PWD", Password);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCategory(string US_Id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GETCATEGORY");
                Param[1] = new SqlParameter("@US_ID", US_Id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetVendor(string Category_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GETVENDOR");
                Param[1] = new SqlParameter("@CATEGORY_ID", Category_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetProduct(string Category_id, string Vendor_id, string mode)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "GETPRODUCT");
                Param[1] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[2] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[3] = new SqlParameter("@mode", mode);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddProduct(string Category_id, string Vendor_id, string ProductName)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "ADD_PRODUCT");
                Param[1] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[2] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[3] = new SqlParameter("@PRODUCT_NME", ProductName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        string sp;
        Image image;
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertOrderbyCredit(string Company_id, string User_id, string Category_id, string Vendor_id,
                            string Product_id, string Qty, string Assign_To, string Quantityunit, string ImageName, string Imagepath, string Is_ImageAvailable)
        {
            try
            {


                string porderid = "SM-P" + RandomDigits(10);
                string filename;
                //string Svrpath = HttpContext.Current.Server.MapPath(@"./IMG/");
                string Svrpath="C:/Inetpub/vhosts/sunmarineteam.in/httpdocs/SMG/IMG/";
                sp = Svrpath;
                //WriteLog("TEST "+sp);
                //string Svrpath="http://sunmarineteam.in/SMG/IMG/";

                if (Is_ImageAvailable == "1")
                {
                    filename = porderid + ".png";

                    byte[] bytes = Convert.FromBase64String(ImageName);

                    
                    using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                    {
                        //using (Bitmap bm2 = new Bitmap(ms))
                        //{
                        //   // ms.Write(bytes, 0, bytes.Length);
                        //    image = Image.FromStream(ms,true);

            //            //}
                        image = Image.FromStream(ms, true);
                        image.Save(Svrpath + filename, System.Drawing.Imaging.ImageFormat.Png);

                    }

                    Imagepath = "http://sunmarineteam.in/SMG/IMG/" + filename;
                }
                else
                {
                    filename = "inprogress";
                    Imagepath = "inprogress";
                }


                Param = new SqlParameter[12];
                Param[0] = new SqlParameter("@SP_TYPE", "INS_PUR_ORD_CR");
                Param[1] = new SqlParameter("@US_ID", User_id);
                Param[2] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[5] = new SqlParameter("@PRODUCT_ID", Product_id);
                Param[6] = new SqlParameter("@PUR_ID", porderid);
                Param[7] = new SqlParameter("@QTY", Qty);
                Param[8] = new SqlParameter("@ASSGN_TO", Assign_To);
                Param[9] = new SqlParameter("@QTY_UNIT", Quantityunit);
                Param[10] = new SqlParameter("@IMG_PATH", Imagepath);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + sp + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCashBal(string User_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GETCASHBAL");
                Param[1] = new SqlParameter("@US_ID", User_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertOrderbyCash(string Company_id, string User_id, string Category_id, string Vendor_id,
                            string Product_id, string Qty, string Amount, string Quantityunit, string Imagepath, string NewBalance)
        {
            try
            {
                string aorderid = "SM-P" + RandomDigits(10);

                Param = new SqlParameter[13];
                Param[0] = new SqlParameter("@SP_TYPE", "INS_PUR_ORD_CASH");
                Param[1] = new SqlParameter("@US_ID", User_id);
                Param[2] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[5] = new SqlParameter("@PRODUCT_ID", Product_id);
                Param[6] = new SqlParameter("@PUR_ID", aorderid);
                Param[7] = new SqlParameter("@QTY", Qty);
                Param[8] = new SqlParameter("@AMT", Amount);
                Param[9] = new SqlParameter("@QTY_UNIT", Quantityunit);
                Param[10] = new SqlParameter("@IMG_PATH", Imagepath);
                Param[11] = new SqlParameter("@NEW_BAL", NewBalance);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPurchase_Ack(string User_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_PURCHASE_ACK");
                Param[1] = new SqlParameter("@US_ID", User_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPurchase_Cash_Ack(string User_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_PURCHASE_CASH_ACK");
                Param[1] = new SqlParameter("@US_ID", User_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Confirm_Order_CR(string Trans_id, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "CONFIRM_ORDER_CR_HM");
                Param[1] = new SqlParameter("@PUR_ID", Trans_id);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Reject_Order_CR(string Trans_id, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "REJECT_ORDER_CR_HM");
                Param[1] = new SqlParameter("@PUR_ID", Trans_id);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);



                //SQL_PARAM = new SqlParameter[2];
                //SQL_PARAM[0] = new SqlParameter("@SP_TYPE", "GET_SMS_VEND");
                //SQL_PARAM[1] = new SqlParameter("@PUR_ID", Trans_id);

                //cmndt = SQLReturnDT("SM_SP", CommandType.StoredProcedure, SQL_PARAM);

                //if (cmndt.Rows.Count > 0 && cmndt.Rows[0]["CODE"].ToString() == "01")
                //{
                //    Forgot_SMS(cmndt.Rows[0]["NUM"].ToString(), cmndt.Rows[0]["MSG"].ToString());
                //}

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Final_Confirm_Order_CR(string Trans_id, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "FINAL_CONFIRM_ORDER_CR_HM");
                Param[1] = new SqlParameter("@PUR_ID", Trans_id);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);


              


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCash_Ack(string User_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_CASH_ACK");
                Param[1] = new SqlParameter("@US_ID", User_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetExpense(string User_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_EXPENSE");
                Param[1] = new SqlParameter("@US_ID", User_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetApprovalExpense(string Username)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "Get_Expense_app");
                Param[1] = new SqlParameter("@UNAME", Username);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPurchaseList(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_PURCHASES_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string FinalPurchaseList(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "FIANL_PURCHASES_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string PM_Qty_List(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "PM_QTY_PURCHASES_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string PurchaseReassignUser(string UserId, string trans_id)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "PURCHASE_REASSIGN");
                Param[1] = new SqlParameter("@US_ID", UserId);
                Param[2] = new SqlParameter("@PUR_ID", trans_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string EditVendor(string Vendorname)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "EDIT_VENDOR_PM");
                Param[1] = new SqlParameter("@VENDOR_ID", Vendorname);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }


        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ProcessPurchasesOrder(string Transid, string Mode, string Amount, string Reassignto, string vendorid,
                                        string Demandqty, string Supplyqty, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[9];
                Param[0] = new SqlParameter("@SP_TYPE", "processPurchaseOrder_pending");
                Param[1] = new SqlParameter("@VENDOR_ID", vendorid);
                Param[2] = new SqlParameter("@PUR_ID", Transid);
                Param[3] = new SqlParameter("@mode", Mode);
                Param[4] = new SqlParameter("@AMT", Amount);
                Param[5] = new SqlParameter("@ASSGN_TO", Reassignto);
                Param[6] = new SqlParameter("@QTY_UNIT", Demandqty);
                Param[7] = new SqlParameter("@QTY", Supplyqty);
                Param[8] = new SqlParameter("@ACK_ID", Ack_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string FinalPurchasesOrder(string Transid, string Mode, string Amount, string Reassignto, string vendorid, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[7];
                Param[0] = new SqlParameter("@SP_TYPE", "Final_processPurchaseOrder");
                Param[1] = new SqlParameter("@VENDOR_ID", vendorid);
                Param[2] = new SqlParameter("@PUR_ID", Transid);
                Param[3] = new SqlParameter("@mode", Mode);
                Param[4] = new SqlParameter("@AMT", Amount);
                Param[5] = new SqlParameter("@ASSGN_TO", Reassignto);
                Param[6] = new SqlParameter("@ACK_ID", Ack_id);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string PM_Qty_Order(string Transid, string Demandqty, string Supplyqty, string Reassignto, string vendorid, string Ack_id)
        {
            try
            {
                Param = new SqlParameter[7];
                Param[0] = new SqlParameter("@SP_TYPE", "PM_QTY_CHECK_CONFIRM");
                Param[1] = new SqlParameter("@VENDOR_ID", vendorid);
                Param[2] = new SqlParameter("@PUR_ID", Transid);
                Param[3] = new SqlParameter("@QTY_UNIT", Demandqty);
                Param[4] = new SqlParameter("@QTY", Supplyqty);
                Param[5] = new SqlParameter("@ASSGN_TO", Reassignto);
                Param[6] = new SqlParameter("@ACK_ID", Ack_id);

                SQL_PARAM = new SqlParameter[2];
                SQL_PARAM[0] = new SqlParameter("@SP_TYPE", "GET_SMS_VEND");
                SQL_PARAM[1] = new SqlParameter("@PUR_ID", Transid);

                cmndt = SQLReturnDT("SM_SP", CommandType.StoredProcedure, SQL_PARAM);

                if (cmndt.Rows.Count > 0 && cmndt.Rows[0]["CODE"].ToString() == "01")
                {
                    Forgot_SMS(cmndt.Rows[0]["NUM"].ToString(), cmndt.Rows[0]["MSG"].ToString());
                }



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UploadInvoiceImage(string trans_id, string Imagepath, string ImageName, string ImageName2)
        {
            string Imagepath2;
            try
            {
                string filename;
                string Svrpath = HttpContext.Current.Server.MapPath("./IMG/");


                filename = trans_id + "pm.png";

                byte[] bytes = Convert.FromBase64String(ImageName);


                Image image, img;
                using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                {
                    image = Image.FromStream(ms);
                    image.Save(Svrpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                }
                Imagepath = "http://sunmarineteam.in/SMG/IMG/" + filename;

                if (ImageName2 != null && ImageName2 != "")
                {
                    filename = trans_id + "pm2.png";
                    byte[] bytes2 = Convert.FromBase64String(ImageName2);
                    using (MemoryStream ms2 = new MemoryStream(bytes2, 0, bytes2.Length))
                    {
                        img = Image.FromStream(ms2);
                        img.Save(Svrpath + filename, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    Imagepath2 = "http://sunmarineteam.in/SMG/IMG/" + filename;
                }
                else
                {
                    Imagepath2 = "inprogress";
                }

                Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@SP_TYPE", "IMAGE_UPLOAD");
                Param[1] = new SqlParameter("@PUR_ID", trans_id);
                Param[2] = new SqlParameter("@IMG_PATH", Imagepath);
                Param[3] = new SqlParameter("@IMG_PATH2", Imagepath2);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertPurchaseExpense(string Amt, string Comments, string Company_Id, string User_Id, string Qty, string Category_Id, string Vendor_id, string Unit, string Assign_To)
        {
            try
            {
                string Purexpe = "SM-P" + RandomDigits(10);

                Param = new SqlParameter[9];
                Param[0] = new SqlParameter("@SP_TYPE", "InsPurmgrExpenses");
                Param[1] = new SqlParameter("@US_ID", User_Id);
                Param[2] = new SqlParameter("@CATEGORY_ID", Category_Id);
                Param[3] = new SqlParameter("@COMP_ID", Company_Id);
                Param[4] = new SqlParameter("@PUR_ID", Purexpe);
                Param[5] = new SqlParameter("@AMT", Amt);
                Param[6] = new SqlParameter("@QTY", Qty);
                Param[7] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[8] = new SqlParameter("@QTY_UNIT", Unit);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]"; }
            finally { }
        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateExpense(string Trans_id, string Ack_id, string Chk, string Mode)
        {
            try
            {
                Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@SP_TYPE", "CONFIRM_EXP");
                Param[1] = new SqlParameter("@PUR_ID", Trans_id);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);
                Param[3] = new SqlParameter("@mode", Chk);
                Param[4] = new SqlParameter("@ROLE_ID", Mode);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetBookingCompanyReport(string Company_id, string Chk)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_BOOKING_COMP_REPORT");
                Param[1] = new SqlParameter("@COMP_ID", Company_id);
                Param[2] = new SqlParameter("@mode", Chk);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetApporval(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_APPROVALS_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        /// <summary>
        /// NEW CHANGE [30-1-2020]

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPre_Received_Apporval(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_RECEIVED_APPORVAL_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Purchase_Pre_Received_Apporval_conform(string Trans_ID, string Ack_id, string Status)
        {
            try
            {
                Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@SP_TYPE", "APPROVALS_PRE_RECD_CONFIRM");
                Param[1] = new SqlParameter("@PUR_ID", Trans_ID);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);
                Param[3] = new SqlParameter("@CATEGORY_ID", Status);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        /// <summary>
        /// ENDS
        /// </summary>

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Purchase_Apporval_conform(string Trans_ID, string vendorid, string Demandqty, string Supplyqty)
        {
            try
            {
                Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@SP_TYPE", "APPROVALS_CONFIRM");
                Param[1] = new SqlParameter("@PUR_ID", Trans_ID);
                Param[2] = new SqlParameter("@VENDOR_ID", vendorid);
                Param[3] = new SqlParameter("@QTY_UNIT", Demandqty);
                Param[4] = new SqlParameter("@QTY", Supplyqty);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Purchase_Apporval_Reject(string Trans_ID)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "APPROVALS_REJECT");
                Param[1] = new SqlParameter("@ACK_ID", Trans_ID);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCashPurchaseApporval(string UserName)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_CASH_LIST");
                Param[1] = new SqlParameter("@UNAME", UserName);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Purchase_Cash_Apporval(string Trans_ID)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "APPROVALS_CASH");
                Param[1] = new SqlParameter("@ACK_ID", Trans_ID);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Purchase_Cash_Reject(string Trans_ID)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "REJECT_CASH");
                Param[1] = new SqlParameter("@ACK_ID", Trans_ID);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string InsertFarmer(string Farmer, string Village, string Refby, string Mobile, string Qty, string Salinity,
                                    string Company_id, string Addedby, string Farmermobile, string Priority_flag, string datePick)
        {
            try
            {
                string farmerid = "SM-BK" + RandomDigits(10);
                //  string format = "MMM dd yyyy";
                //  string from = "dd-m-yyyy";
                //  DateTime dateTime = DateTime.ParseExact(datePick, from, System.Globalization.CultureInfo.InvariantCulture);



                Param = new SqlParameter[14];
                Param[0] = new SqlParameter("@SP_TYPE", "INS_FARMER");
                Param[1] = new SqlParameter("@FARMER", Farmer);
                Param[2] = new SqlParameter("@UNAME", Village);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@ACK_ID", Refby);
                Param[5] = new SqlParameter("@MOB", Mobile);
                Param[6] = new SqlParameter("@PUR_ID", farmerid);
                Param[7] = new SqlParameter("@QTY", Qty);
                Param[8] = new SqlParameter("@PRODUCT_NME", Salinity);
                Param[9] = new SqlParameter("@ASSGN_BY", Addedby);
                Param[10] = new SqlParameter("@IMG_PATH", Farmermobile);
                Param[11] = new SqlParameter("@TO_DATE", Priority_flag);
                Param[12] = new SqlParameter("@DEL_RES", "notyetdeleted");
                Param[13] = new SqlParameter("@DATE", datePick);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete_Farmer(string Farmer_id, string Reason)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "DEL_FARMER");
                Param[1] = new SqlParameter("@FARMER", Farmer_id);
                Param[2] = new SqlParameter("@DEL_RES", Reason);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetBooking_Farmer(string CompanyId, string Head)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_BOOKING_FARMER");
                Param[1] = new SqlParameter("@COMP_ID", CompanyId);
                Param[2] = new SqlParameter("@mode", Head);



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string InsertCashCd(string User_id, string Company_Id, string Amt, string Assigned_by,
                    string Assigned_by_id)
        {
            try
            {
                Param = new SqlParameter[6];
                Param[0] = new SqlParameter("@SP_TYPE", "INS_CASH_CD");
                Param[1] = new SqlParameter("@US_ID", User_id);
                Param[2] = new SqlParameter("@ASSGN_BY", Assigned_by);
                Param[3] = new SqlParameter("@COMP_ID", Company_Id);
                Param[4] = new SqlParameter("@AMT", Amt);
                Param[5] = new SqlParameter("@ASSGN_BY_ID", Assigned_by_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string Updated_Cash_Ack_Req(string User_id, string Ack_id, string Amt)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "UpdatedCashAcknowledgements");
                Param[1] = new SqlParameter("@US_ID", User_id);
                Param[2] = new SqlParameter("@ACK_ID", Ack_id);
                Param[3] = new SqlParameter("@AMT", Amt);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string Updated_Cash_Reject_Ack_Req(string Ack_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "UpdatedCashREJECTAcknowledgements");
                Param[1] = new SqlParameter("@ACK_ID", Ack_id);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string Updated_Cash_Re_Assign_Ack_Req(string Ack_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "UpdatedCashRE_ASSIGNAcknowledgements");
                Param[1] = new SqlParameter("@ACK_ID", Ack_id);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string Ack_Req(string usname)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "Acknowledgements_pending");
                Param[1] = new SqlParameter("@ACK_ID", usname);



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }



        //[ScriptMethod(UseHttpGet = true)]
        //public string GetSecurityCodeValid(string Code)
        //{
        //    try
        //    {
        //        Param = new SqlParameter[2];
        //        Param[0] = new SqlParameter("@SP_TYPE", "GET_SECURITY_CODE");
        //        Param[1] = new SqlParameter("@UNAME", Code);
        //        return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
        //    }
        //    finally { }
        //}

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetUserBal(string Role, string Username)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_USER_BAL");
                Param[1] = new SqlParameter("@mode", Role);
                Param[2] = new SqlParameter("@UNAME", Username);
                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetDelayRpt()
        {
            try
            {
                Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_DELAY_RPT");

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetSalesRpt(string Company_Id, string FromDate, string ToDate)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_SALES_RPT");
                Param[1] = new SqlParameter("@TO_DATE", ToDate);
                Param[2] = new SqlParameter("@FROM_DATE", FromDate);
                Param[3] = new SqlParameter("@COMP_ID", Company_Id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetPurchaseRpt(string Company_Id, string FromDate, string ToDate)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_PURCHASE_RPT");
                Param[1] = new SqlParameter("@TO_DATE", ToDate);
                Param[2] = new SqlParameter("@FROM_DATE", FromDate);
                Param[3] = new SqlParameter("@COMP_ID", Company_Id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddVendor(string Category_id, string Vendor_Name)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "ADDVENDOR");
                Param[1] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[2] = new SqlParameter("@VENDOR_ID", Vendor_Name);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        /// <summary>
        /// Tanks
        /// </summary>
        /// <returns></returns>


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GET_BH()
        {

            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_BH");


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Tankinfo()
        {

            try
            {
                Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@SP_TYPE", "TANKS_BOOK_STATUS");



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string TankShineTank()
        {

            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "TANKS_LIST");


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]

        public string Get_Tank(string company)
        {

            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_TANKS_LIST");
                Param[1] = new SqlParameter("@COMP_ID", company);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetTankforPack(string CompanyId, string TankId, string BookedQty)
        {

            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_TANK_BOOK");
                Param[1] = new SqlParameter("@COMP_ID", CompanyId);
                Param[2] = new SqlParameter("@ACK_ID", TankId);
                Param[3] = new SqlParameter("@QTY", BookedQty);



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetTankBookQty(string Booking_id)
        {

            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_BOOKING_TANK_QTY");
                Param[1] = new SqlParameter("@COMP_ID", Booking_id);




                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateTankPack(string CompanyId, string frmtank_id, string totank_id, string booking_id, string frmqtr, string toqty, string salinity)
        {

            try
            {
                Param = new SqlParameter[8];
                Param[0] = new SqlParameter("@SP_TYPE", "UPDATED_TANK_PACK");
                Param[1] = new SqlParameter("@COMP_ID", CompanyId);
                Param[2] = new SqlParameter("@ACK_ID", booking_id);
                Param[3] = new SqlParameter("@QTY", frmqtr);
                Param[4] = new SqlParameter("@QTY_UNIT", toqty);
                Param[5] = new SqlParameter("@FARMER", frmtank_id);
                Param[6] = new SqlParameter("@US_ID", totank_id);
                Param[7] = new SqlParameter("@ASSGN_BY", salinity);



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetTanKQty(string CompanyId, string TankId)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_TANK_QTY");
                Param[1] = new SqlParameter("@COMP_ID", CompanyId);
                Param[2] = new SqlParameter("@ACK_ID", TankId);




                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetCurrentBooking(string CompanyId, string TankId)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_CURRENT_BOOKING");
                Param[1] = new SqlParameter("@company_id", CompanyId);
                Param[2] = new SqlParameter("@tankno", TankId);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetTankBookingSum(string CompanyId, string TankId)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_SUM_BOOKING");
                Param[1] = new SqlParameter("@company_id", CompanyId);
                Param[2] = new SqlParameter("@tankno", TankId);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetSalesReport(string CompanyId, string Head)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_SALES");
                Param[1] = new SqlParameter("@COMP_ID", CompanyId);
                Param[2] = new SqlParameter("@mode", Head);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertTankBooking(string tankno, string employeeid, string CompanyId, string forcompanyid, string bookedby, string quantity,
                            string salinity, string farmername, string addedby, string mobile, string refby, string amount, string village,
                            string farmerbookingid, string farmerdate, string farmermobile, string calcavailablequant)
        {

            try
            {

                List<string> lsttankno;



                tankno = tankno.TrimEnd(',');



                lsttankno = tankno.Split(',').ToList();


                if (lsttankno.Count == 0)
                {
                    return "[{\"CODE\":\"02\",\"MSG\":\"Please Check details..!!!!\"}]";
                }
                else
                {
                    for (int i = 0; i < lsttankno.Count; i++)
                    {
                        Param = new SqlParameter[15];
                        Param[0] = new SqlParameter("@tankno", lsttankno[i].ToString());
                        Param[1] = new SqlParameter("@employeeid", employeeid);
                        Param[2] = new SqlParameter("@companyid", CompanyId);
                        Param[3] = new SqlParameter("@bookedby", bookedby);
                        Param[4] = new SqlParameter("@forcompanyid", forcompanyid);
                        Param[5] = new SqlParameter("@farmername", farmername);
                        Param[6] = new SqlParameter("@village", village);
                        Param[7] = new SqlParameter("@refby", refby);
                        Param[8] = new SqlParameter("@mobile", mobile);
                        Param[9] = new SqlParameter("@quantity", quantity);
                        Param[10] = new SqlParameter("@salinity", salinity);
                        Param[11] = new SqlParameter("@farmerbookingid", farmerbookingid);
                        Param[12] = new SqlParameter("@addedby", addedby);
                        Param[13] = new SqlParameter("@farmerdate", farmerdate);
                        Param[14] = new SqlParameter("@farmermobile", farmermobile);

                        dt = SQLReturnJson("SP_Insert_Booking", CommandType.StoredProcedure, Param);
                    }
                }
                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateFarmer(string Farmer, string Village, string Refby, string Mobile, string Qty, string Salinity, string farmerid,
                                    string Company_id, string Addedby, string Farmermobile, string Priority_flag, string datePick)
        {
            try
            {


                Param = new SqlParameter[14];
                Param[0] = new SqlParameter("@SP_TYPE", "UPD_FARMER");
                Param[1] = new SqlParameter("@FARMER", Farmer);
                Param[2] = new SqlParameter("@UNAME", Village);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@ACK_ID", Refby);
                Param[5] = new SqlParameter("@MOB", Mobile);
                Param[6] = new SqlParameter("@PUR_ID", farmerid);
                Param[7] = new SqlParameter("@QTY", Qty);
                Param[8] = new SqlParameter("@PRODUCT_NME", Salinity);
                Param[9] = new SqlParameter("@ASSGN_BY", Addedby);
                Param[10] = new SqlParameter("@IMG_PATH", Farmermobile);
                Param[11] = new SqlParameter("@TO_DATE", Priority_flag);
                Param[12] = new SqlParameter("@DEL_RES", "notyetdeleted");
                Param[13] = new SqlParameter("@DATE", datePick);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertTankSales(string tankno, string employeeid, string CompanyId, string farmername, string village, string refby, string mobile,
                        string quantity, decimal saleQty, string salinity, decimal sm_unit_rate, decimal sm_gross_total, decimal sm_refer_fee,
                            decimal sm_discount, decimal sm_net_amt, string Booking_id, string BookedBy, decimal Cashsales, decimal BankDeposit,
                        string ChequeBank, string ChequeNo, decimal ChequeAmt, string ChequeDt, decimal bag, decimal BagUnit, decimal Wav_Discount, string Pincode)
        {

            try
            {
                string salestransid = "SM-Pk" + RandomDigits(10);

                Param = new SqlParameter[28];
                Param[0] = new SqlParameter("@tankno", tankno);
                Param[1] = new SqlParameter("@employeeid", employeeid);
                Param[2] = new SqlParameter("@companyid", CompanyId);
                Param[3] = new SqlParameter("@bookedby", BookedBy);
                Param[4] = new SqlParameter("@saleQty", saleQty);
                Param[5] = new SqlParameter("@farmername", farmername);
                Param[6] = new SqlParameter("@village", village);
                Param[7] = new SqlParameter("@refby", refby);
                Param[8] = new SqlParameter("@mobile", mobile);
                Param[9] = new SqlParameter("@quantity", quantity);
                Param[10] = new SqlParameter("@salinity", salinity);
                Param[11] = new SqlParameter("@sm_unit_rate", sm_unit_rate);
                Param[12] = new SqlParameter("@sm_gross_total", sm_gross_total);
                Param[13] = new SqlParameter("@sm_refer_fee", sm_refer_fee);
                Param[14] = new SqlParameter("@sm_discount", sm_discount);
                Param[15] = new SqlParameter("@sm_net_amt", sm_net_amt);
                Param[16] = new SqlParameter("@Booking_id", Booking_id);
                Param[17] = new SqlParameter("@Cashsales", Cashsales);
                Param[18] = new SqlParameter("@BankDeposit", BankDeposit);
                Param[19] = new SqlParameter("@ChequeBank", ChequeBank);
                Param[20] = new SqlParameter("@ChequeNo", ChequeNo);
                Param[21] = new SqlParameter("@ChequeAmt", ChequeAmt);
                Param[22] = new SqlParameter("@ChequeDt", ChequeDt);
                Param[23] = new SqlParameter("@bag", bag);
                Param[24] = new SqlParameter("@BagUnit", BagUnit);
                Param[25] = new SqlParameter("@Wav_Discount", Wav_Discount);
                Param[26] = new SqlParameter("@Trans_id", salestransid);
                Param[27] = new SqlParameter("@pincode", Pincode);
                //    Param28] = new SqlParameter("@SP_TYPE", "SP_insertTankSales");


                return dt = SQLReturnJson("SP_insertTankSales", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetFinalSalesAdminApporval()
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GETSALESAPPORVAL");

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Getsales_pending(string Booking_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_PENDING_SALES");
                Param[1] = new SqlParameter("@ACK_id", Booking_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Sales_Apporval(string Booking_id, string Flag, string Refname, string Bookedby, string id, string Trans_id)
        {
            try
            {
                Param = new SqlParameter[7];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_SALES_CONF");
                Param[1] = new SqlParameter("@ACK_ID", Booking_id);
                Param[2] = new SqlParameter("@mode", Flag);
                Param[3] = new SqlParameter("@refby", Refname);
                Param[4] = new SqlParameter("@addedby", Bookedby);
                Param[5] = new SqlParameter("@DEL_RES", id);
                Param[6] = new SqlParameter("@MOB", Trans_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string InsertBookingTankSales(string employeeid, string CompanyId, string farmername, string village, string mobile,
                            string quantity, decimal saleQty, string salinity, decimal sm_unit_rate, decimal sm_gross_total, decimal sm_refer_fee,
                            decimal sm_discount, decimal sm_net_amt, string Booking_id, decimal Cashsales, decimal BankDeposit,
                        string ChequeBank, string ChequeNo, decimal ChequeAmt, string ChequeDt, string Pincode, string Cheque_Amt,
            string bank_name, string bank_trans_id)
        {

            try
            {
                string salestransid = "SM-SN" + RandomDigits(10);

                Param = new SqlParameter[24];

                Param[0] = new SqlParameter("@quantity", quantity);
                Param[1] = new SqlParameter("@salinity", salinity);
                Param[2] = new SqlParameter("@sm_gross_total", sm_gross_total);
                Param[3] = new SqlParameter("@sm_refer_fee", sm_refer_fee);
                Param[4] = new SqlParameter("@sm_discount", sm_discount);
                Param[5] = new SqlParameter("@sm_net_amt", sm_net_amt);
                Param[6] = new SqlParameter("@Booking_id", Booking_id);
                Param[7] = new SqlParameter("@Cashsales", Cashsales);
                Param[8] = new SqlParameter("@BankDeposit", BankDeposit);
                Param[9] = new SqlParameter("@ChequeBank", ChequeBank);
                Param[10] = new SqlParameter("@ChequeNo", ChequeNo);
                Param[11] = new SqlParameter("@ChequeAmt", ChequeAmt);
                Param[12] = new SqlParameter("@ChequeDt", ChequeDt);
                Param[13] = new SqlParameter("@Trans_id", salestransid);
                Param[14] = new SqlParameter("@sm_unit_rate", sm_unit_rate);
                Param[15] = new SqlParameter("@employeeid", employeeid);
                Param[16] = new SqlParameter("@companyid", CompanyId);
                Param[17] = new SqlParameter("@pincode", Pincode);
                Param[18] = new SqlParameter("@farmername", farmername);
                Param[19] = new SqlParameter("@village", village);
                Param[20] = new SqlParameter("@mobile", mobile);
                Param[21] = new SqlParameter("@cheque_Amt", Cheque_Amt);
                Param[22] = new SqlParameter("@bank_name", bank_name);
                Param[23] = new SqlParameter("@bank_transid", bank_trans_id);

                return dt = SQLReturnJson("Sales_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateBookingTankSales(string employeeid, string trans_id, string CompanyId, string farmername, string village, string mobile,
                            string quantity, decimal saleQty, string salinity, decimal sm_unit_rate, decimal sm_gross_total, decimal sm_refer_fee,
                            decimal sm_discount, decimal sm_net_amt, string Booking_id, decimal Cashsales, decimal BankDeposit,
                        string ChequeBank, string ChequeNo, decimal ChequeAmt, string ChequeDt, string Pincode, string Cheque_Amt,
            string bank_name, string bank_trans_id)
        {

            try
            {
                string salestransid = trans_id;

                Param = new SqlParameter[24];

                Param[0] = new SqlParameter("@quantity", quantity);
                Param[1] = new SqlParameter("@salinity", salinity);
                Param[2] = new SqlParameter("@sm_gross_total", sm_gross_total);
                Param[3] = new SqlParameter("@sm_refer_fee", sm_refer_fee);
                Param[4] = new SqlParameter("@sm_discount", sm_discount);
                Param[5] = new SqlParameter("@sm_net_amt", sm_net_amt);
                Param[6] = new SqlParameter("@Booking_id", Booking_id);
                Param[7] = new SqlParameter("@Cashsales", Cashsales);
                Param[8] = new SqlParameter("@BankDeposit", BankDeposit);
                Param[9] = new SqlParameter("@ChequeBank", ChequeBank);
                Param[10] = new SqlParameter("@ChequeNo", ChequeNo);
                Param[11] = new SqlParameter("@ChequeAmt", ChequeAmt);
                Param[12] = new SqlParameter("@ChequeDt", ChequeDt);
                Param[13] = new SqlParameter("@Trans_id", salestransid);
                Param[14] = new SqlParameter("@sm_unit_rate", sm_unit_rate);
                Param[15] = new SqlParameter("@employeeid", employeeid);
                Param[16] = new SqlParameter("@companyid", CompanyId);
                Param[17] = new SqlParameter("@pincode", Pincode);
                Param[18] = new SqlParameter("@farmername", farmername);
                Param[19] = new SqlParameter("@village", village);
                Param[20] = new SqlParameter("@mobile", mobile);
                Param[21] = new SqlParameter("@cheque_Amt", Cheque_Amt);
                Param[22] = new SqlParameter("@bank_name", bank_name);
                Param[23] = new SqlParameter("@bank_transid", bank_trans_id);

                return dt = SQLReturnJson("Update_Sales_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdatedTankMaintances(string companyid, decimal quantity, string salinity, string tankno, string plsize, string packstatus,
                            decimal availablequantity, string Prawn_Src, decimal TankBookingSum)
        {

            try
            {
                Param = new SqlParameter[9];
                Param[0] = new SqlParameter("@company_id", companyid);
                Param[1] = new SqlParameter("@tankno", tankno);
                Param[2] = new SqlParameter("@quantity", quantity);
                Param[3] = new SqlParameter("@salinity", salinity);
                Param[4] = new SqlParameter("@plsize", plsize);
                Param[5] = new SqlParameter("@packstatus", packstatus);
                Param[6] = new SqlParameter("@availablequantity", availablequantity);
                Param[7] = new SqlParameter("@Prawn_Src", Prawn_Src);
                Param[8] = new SqlParameter("@TankBookingSum", TankBookingSum);

                return dt = SQLReturnJson("SP_insertTankMaintanenceData", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string DeleteBookingFarmer(string Booking_id, string deletereason, string Quantity, string companyid, string tankno)
        {
            try
            {


                List<string> lsttankid;
                List<string> lstquantity;


                Quantity = Quantity.TrimEnd(',');
                tankno = tankno.TrimEnd(',');



                lstquantity = Quantity.Split(',').ToList();
                lsttankid = tankno.Split(',').ToList();

                if (lstquantity.Count != lsttankid.Count || lstquantity.Count == 0)
                {
                    return "[{\"CODE\":\"02\",\"MSG\":\"Please Check details..!!!!\"}]";
                }
                else
                {

                    for (int i = 0; i < lsttankid.Count; i++)
                    {

                        Param = new SqlParameter[5];
                        Param[0] = new SqlParameter("@Booking_id", Booking_id);
                        Param[1] = new SqlParameter("@deletereason", deletereason);
                        Param[2] = new SqlParameter("@deleted_qty", lstquantity[i].ToString());
                        Param[3] = new SqlParameter("@company_id", companyid);
                        Param[4] = new SqlParameter("@tank_id", lsttankid[i].ToString());
                        dt = SQLReturnJson("SP_deleteBooking", CommandType.StoredProcedure, Param);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateBoikingTankFarmer(string Booking_id, string Qty, string Company_id, string Tankno, string Salinity, string date, string Tank_ava_qty)
        {

            try
            {
                Param = new SqlParameter[8];
                Param[0] = new SqlParameter("@SP_TYPE", "UPDATE_BOOKING");
                Param[1] = new SqlParameter("@ACK_ID", Booking_id);
                Param[2] = new SqlParameter("@QTY", Qty);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@US_ID", Tankno);
                Param[5] = new SqlParameter("@salinity", Salinity);
                Param[6] = new SqlParameter("@DATE", date);
                Param[7] = new SqlParameter("@QTY_UNIT", Tank_ava_qty);



                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }



        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetbookingsalesHistory(string Username)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GETSALESUMFARMER");
                Param[1] = new SqlParameter("@UNAME", Username);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetSalesQty(string Booking_id)
        {

            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GETSALESUMFARMERQTY");
                Param[1] = new SqlParameter("@ACK_ID", Booking_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Get_Sales_Cr_Trans(string Tans_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_CREDIT_TRANS");
                Param[1] = new SqlParameter("@ACK_id", Tans_id);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetSalesApporval(string Username)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_SALES_CR");
                Param[1] = new SqlParameter("@UNAME", Username);
                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetNotification(string UserName, string Comapny_id)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_NOTIFICATION");
                Param[1] = new SqlParameter("@UNAME", UserName);
                Param[2] = new SqlParameter("@COMP_ID", Comapny_id);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        /// <summary>
        /// sM_aDMIN
        /// </summary>
        /// <returns></returns>

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ADMIN_PRODUCT()
        {
            try
            {
                Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_ADM_PRODUCT");


                return dt = SQLReturnJson("SM_ADM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ADMIN_VENDOR(string Vendor_id)
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "GET_ADM_VENDOR");
                Param[1] = new SqlParameter("@VENDOR_ID", Vendor_id);


                return dt = SQLReturnJson("SM_ADM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ADMIN_ADD_OTHER_EXPENSE(string Company_id, string User_id, string Category_id, string Vendor_id,
                            string Product_id, string Qty, string Assign_To, string Quantityunit, string Amt)
        {
            try
            {
                string porderid = "SM-E" + RandomDigits(10);

                Param = new SqlParameter[12];
                Param[0] = new SqlParameter("@SP_TYPE", "ADD_OTHER_EXP");
                Param[1] = new SqlParameter("@US_ID", User_id);
                Param[2] = new SqlParameter("@CATEGORY_ID", Category_id);
                Param[3] = new SqlParameter("@COMP_ID", Company_id);
                Param[4] = new SqlParameter("@VENDOR_ID", Vendor_id);
                Param[5] = new SqlParameter("@PRODUCT_ID", Product_id);
                Param[6] = new SqlParameter("@PUR_ID", porderid);
                Param[7] = new SqlParameter("@QTY", Qty);
                Param[8] = new SqlParameter("@ASSGN_TO", Assign_To);
                Param[9] = new SqlParameter("@QTY_UNIT", Quantityunit);
                Param[10] = new SqlParameter("@AMT", Amt);

                return dt = SQLReturnJson("SM_ADM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + sp + "\"}]"; }
            finally { }

        }


        /// <summary>
        /// FINAL SALE SETTLE

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Get_final_settle(string UserName)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "FINAL_SLAES_SETTLE");
                Param[1] = new SqlParameter("@UNAME", UserName);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + sp + "\"}]"; }
            finally { }
        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Final_BH_Settle(string Trans_id, string CreditAmt, string CreditDate, string CreditBank, string Payment_Num,
                                    string CompanyId, decimal Wav_Discount, decimal sm_discount, decimal web_reffee, decimal Purchase_bal,
                                    string Remark)
        {
            try
            {
                Param = new SqlParameter[12];
                Param[0] = new SqlParameter("@Credit_type", "Sales");
                Param[1] = new SqlParameter("@Trans_id", Trans_id);
                Param[2] = new SqlParameter("@CreditAmt", CreditAmt);
                Param[3] = new SqlParameter("@CreditDate", CreditDate);
                Param[4] = new SqlParameter("@CreditBank", CreditBank);
                Param[5] = new SqlParameter("@Payment_Num", Payment_Num);
                Param[6] = new SqlParameter("@Company_id", CompanyId);
                Param[7] = new SqlParameter("@Wav_Discount", Wav_Discount);
                Param[8] = new SqlParameter("@Discount", sm_discount);
                Param[9] = new SqlParameter("@webReffee", web_reffee);
                Param[10] = new SqlParameter("@pur_balance", Purchase_bal);
                Param[11] = new SqlParameter("@remark", Remark);

                return dt = SQLReturnJson("SP_UpdateCredit_Discount", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + sp + "\"}]"; }
            finally { }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Update_BH_Compen(string Booking_id,string Company_id,string Qty)
        {
            try
            {
                Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@SP_TYPE", "BH_COMPENS");
                Param[1] = new SqlParameter("@ACK_ID", Booking_id);
                Param[2] = new SqlParameter("@COMP_ID",Company_id );
                Param[3] = new SqlParameter("@QTY", Qty);

                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        /// </summary>
        /// <returns></returns>

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ADMIN_OTHER_EXP_LIST()
        {
            try
            {
                Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@SP_TYPE", "OTHER_LIST");



                return dt = SQLReturnJson("SM_ADM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        /// <summary>
        /// /END
        /// </summary>
        /// <returns></returns>
        /// 


        #region Accountant ws

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Get_Accountant_Pur_Rpt(string UserName)
        {
            try
            {
                Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@SP_TYPE", "ACCT_PUR_RPT");
                Param[1] = new SqlParameter("@UNAME", UserName);


                return dt = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }
        }

        #endregion


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string GetSMS()
        {
            string mob;
            string val = "";
            try
            {
                if (ConfigurationManager.AppSettings["stopsms"].ToString() == "NO")
                {
                    mob = ConfigurationManager.AppSettings["number"].ToString();
                    SMS(mob);
                    val = "OTP is sent to " + mob;
                }
                return "[{\"CODE\":\"01\",\"MSG\":\"" + val + "\",\"OTP\":\"" + msg + "\"}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]";
            }
            finally { }

        }

        public void SMS(string num)
        {

            //string URL = "http://pay4sms.in/sendsms/?token=96ea6c139ddc7de0e0b1697c6b1e0950&credit=2&sender=SMTOTP&message=SMT%20CODE%20is%203564&number=9600161227"
            string URL = "http://pay4sms.in/sendsms/?token=96ea6c139ddc7de0e0b1697c6b1e0950&credit=2&sender=SMTOTP&";

            msg = RandomDigits(4);
            URL = URL + "&message=SMT%20CODE%20is%20" + msg + "&number=" + num + "";

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            request.Accept = "*/*";
            request.Timeout = 300000;
            // request.ProtocolVersion = HttpVersion.Version10;
            //request.ContentLength = data.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            //requestWriter.Write(data);
            requestWriter.Close();

            try
            {
                // get the response
                System.Net.WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                responseReader.Close();
                // string NoteText = response + "-------------------" + num + "------------------------" + msg + "---------------" + URL;
                //System.IO.File.WriteAllText(@"D:\Printlog\Smslog\" + docid + ".txt", NoteText);
            }
            catch (System.Net.WebException we)
            {
                string webExceptionMessage = we.Message;
                // System.IO.File.WriteAllText(@"D:\Printlog\Smslog\" + docid + "Excep" + ".txt", webExceptionMessage + URL + we.ToString());
            }
            catch (Exception)
            {
                // no need to do anything special here....
            }
        }


        public void Forgot_SMS(string num, string Msg)
        {

            //string URL = "http://pay4sms.in/sendsms/?token=96ea6c139ddc7de0e0b1697c6b1e0950&credit=2&sender=SMTOTP&message=SMT%20CODE%20is%203564&number=9600161227"
            string URL = "http://pay4sms.in/sendsms/?token=96ea6c139ddc7de0e0b1697c6b1e0950&credit=2&sender=SMTOTP&";

            //msg = RandomDigits(4);
            URL = URL + "&message=" + Msg + "&number=" + num + "";

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            request.Accept = "*/*";
            request.Timeout = 300000;
            // request.ProtocolVersion = HttpVersion.Version10;
            //request.ContentLength = data.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            //requestWriter.Write(data);
            requestWriter.Close();

            try
            {
                // get the response
                System.Net.WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                responseReader.Close();
                // string NoteText = response + "-------------------" + num + "------------------------" + msg + "---------------" + URL;
                //System.IO.File.WriteAllText(@"D:\Printlog\Smslog\" + docid + ".txt", NoteText);
            }
            catch (System.Net.WebException we)
            {
                string webExceptionMessage = we.Message;
                // System.IO.File.WriteAllText(@"D:\Printlog\Smslog\" + docid + "Excep" + ".txt", webExceptionMessage + URL + we.ToString());
            }
            catch (Exception)
            {
                // no need to do anything special here....
            }
        }
        //[WebMethod]
        //[ScriptMethod(UseHttpGet = true)]
        //public void TanksList()
        //{
        //    try
        //    {
        //        string tankinfo, sts;

        //        Param = new SqlParameter[1];
        //        Param[0] = new SqlParameter("@SP_TYPE", "TANKS_LIST");



        //        sts = SQLReturnJson("SM_SP", CommandType.StoredProcedure, Param);


        //        sts = sts.Replace("[", "");
        //        sts = sts.Replace("]", "");

        //        SQL_PARAM = new SqlParameter[1];
        //        SQL_PARAM[0] = new SqlParameter("@SP_TYPE", "TANKS_BOOK_STATUS");


        //        tankinfo = SQLReturnJson("SM_SP", CommandType.StoredProcedure, SQL_PARAM);




        //        tankinfo = tankinfo.Replace("[", "");
        //        tankinfo = tankinfo.Replace("]", "");

        //        Context.Response.Write("[" +
        //                   "{\"Tanks Info\":[" + tankinfo + "]}," + "{\"Result\":[" + sts + "]}" +

        //                   "]");

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());

        //        Context.Response.Write("[{\"CODE\":\"02\",\"MSG\":\"EXCEP\",\"DESC\":\"" + ex.ToString() + "\"}]");
        //    }
        //    finally { }
        //}


        //--------------------------------------------------------
        public string SQLReturnJson(string CommandText, CommandType Commandtype, params SqlParameter[] parameters_values)
        {
            this.conManager = new ConManager();
            cmnd = new SqlCommand();
            DataTable dtz = new DataTable();

            try
            {
                cmnd.Connection = conManager.sqlConnection;
                cmnd.CommandText = CommandText;
                cmnd.CommandType = Commandtype;
                // cmnd.CommandTimeout = connectiontimeout;
                if (parameters_values.Length > 0)
                {
                    foreach (SqlParameter param in parameters_values)
                    {

                        if (param != null) { cmnd.Parameters.Add(param); }

                    }
                }

                sqlDataAdapter = new SqlDataAdapter(cmnd);
                cmnd.Transaction = trans;

                sqlDataAdapter.Fill(dtz);
                cmnd.Dispose();
                sqlDataAdapter.Dispose();
                if (dtz != null && dtz.Rows.Count > 0)
                {

                    /*  //serializ.................
                      JavaScriptSerializer serializer = new JavaScriptSerializer();

                      List<Dictionary<String, Object>> tableRows = new List<Dictionary<String, Object>>();

                      Dictionary<String, Object> row;

                      foreach (DataRow dr in dtz.Rows)
                      {
                          row = new Dictionary<String, Object>();
                          foreach (DataColumn col in dtz.Columns)
                          {
                              row.Add(col.ColumnName, dr[col]);
                          }
                          tableRows.Add(row);
                      }
                      return serializer.Serialize(tableRows);
                     */
                    return JsonConvert.SerializeObject(dtz);
                }
                return "[{\"CODE\":\"02\",\"MSG\":\"NO DATA !!!!\"}]";
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); if (trans != null) trans.Rollback(); return ex.ToString(); }
            finally
            {
                dt = null;
                sqlDataAdapter = null; cmnd = null;
                if (Connection != null)
                {
                    conManager.CloseConnection();
                }
                Connection = null;
            }

        }
        public DataTable SQLReturnDT(string CommandText, CommandType Commandtype, params SqlParameter[] parameters_values)
        {
            this.conManager = new ConManager();
            cmnd = new SqlCommand();
            DataTable dtz = new DataTable();

            try
            {
                cmnd.Connection = conManager.sqlConnection;
                cmnd.CommandText = CommandText;
                cmnd.CommandType = Commandtype;
                // cmnd.CommandTimeout = connectiontimeout;
                if (parameters_values.Length > 0)
                {
                    foreach (SqlParameter param in parameters_values)
                    {

                        if (param != null) { cmnd.Parameters.Add(param); }

                    }
                }

                sqlDataAdapter = new SqlDataAdapter(cmnd);
                cmnd.Transaction = trans;

                sqlDataAdapter.Fill(dtz);
                cmnd.Dispose();
                sqlDataAdapter.Dispose();

                return dtz;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); if (trans != null) trans.Rollback(); return null; }
            finally
            {
                dt = null;
                sqlDataAdapter = null; cmnd = null;
                if (Connection != null)
                {
                    conManager.CloseConnection();
                }
                Connection = null;
            }

        }

        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = @"D:\Log\";
            logFilePath = logFilePath + "PC-Log-KOT" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }

        public string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }



    }
}

