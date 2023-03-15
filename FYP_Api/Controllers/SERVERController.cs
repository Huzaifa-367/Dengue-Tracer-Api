using FYP_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using static System.Collections.Specialized.BitVector32;

namespace FYP_Api.Controllers
{
    public class SERVERController : ApiController
    {

        private readonly ProjectEntities db = new ProjectEntities();


        [HttpPost]
        public HttpResponseMessage NewUser()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                string email = request["email"];
                USER user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Exsist");
                }

                USER newuser = new USER
                {
                    email = email,
                    name = request["name"],
                    phone_number = request["Phone_number"],
                    role = request["role"],
                    password = request["password"],
                    home_location = request["home_location"],
                    office_location = request["office_location"]
                };


                db.USERs.Add(newuser);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Created");
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        //----------------------------------------------------------------------------//

        [HttpPost]
        public HttpResponseMessage UpdateuserImage()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                HttpPostedFile imagefile = request.Files["image"];
                

                int id = int.Parse(request["user_id"]);
                string extension = imagefile.FileName.Split('.')[1];
               // DateTime dt = DateTime.Now;
                string filename = id + "." + extension;
                // filename = filename + DateTime.Now.ToShortTimeString()+"."+extension;
                imagefile.SaveAs(HttpContext.Current.Server.
                               MapPath("~/Images/" + filename));
                USER user = db.USERs.Where(x => x.user_id == id).FirstOrDefault();
                user.image = filename;
                _ = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Uploaded");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }



        [HttpPost]

        public HttpResponseMessage UpdateUserStatus(int user_id, bool status)
        {

            try
            {
                var user = db.CASES_LOGS.Where(s => s.user_id == user_id).FirstOrDefault();
                if (user == null)
                {
                    DateTime dt = DateTime.Now;
                    var date = dt.Year + "-" + dt.Month + "-" + dt.Day;
                    int range = 5;
                    //-------------------------------------------------
                    CASES_LOGS newCase = new CASES_LOGS();

                    newCase.user_id = user_id;
                    newCase.status = status;
                    newCase.date = DateTime.Parse(date);
                    //newCase.range = range;
                    db.CASES_LOGS.Add(newCase);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, newCase);

                }
                user.status = status;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
                // Create a new instance of the Cases model


                // Assign the values of the parameters to the corresponding properties of the Cases model


                // Insert the new case into the Cases table



            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]

        public HttpResponseMessage UpdatePassword(string email, string newpassword)
        {

            try
            {
                var user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                  
                    user.password = newpassword;

                    db.SaveChanges();
                   // return Request.CreateResponse(HttpStatusCode.OK, newpass);

                }
           
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");




            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//
        [HttpGet]
        public HttpResponseMessage Login(string email, string password)
        {
            try
            {
                var user = db.USERs.Where(s => s.email == email && s.password == password).FirstOrDefault();
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "false");

                }
                return Request.CreateResponse(HttpStatusCode.OK, user);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage Showallusers()
        {
            try
            {
                var lst = db.USERs.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, lst);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage GetDengueUsers()
        {
            try
            {

                //var Query = "select from us in USER join ass in ASSIGNSECTOR join sec in SECTOR";
                var lst = db.USERs.Where(u => u.role == "user");
                var result = from u in lst
                             join c in db.CASES_LOGS on u.user_id equals c.user_id
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, u.sec_id, s.sec_name, s.lat_long, c.date, c.status };


                /*     foreach(var item in lst)
                     {
                         string quey = "insert into Dumm values('" + item + "','" + userid + "')";
                     }*/

                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //----------------------------------------------------------------------------//


        [HttpPost]
        public HttpResponseMessage NewOfficer()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                string email = request["email"];
                USER user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Officer Already Exsist");
                }

                USER newuser = new USER
                {
                    email = email,
                    name = request["name"],
                    phone_number = request["Phone_number"],
                    role = "officer",
                    password = request["password"]
                };

                db.USERs.Add(newuser);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Officer Account Created");
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage Getofficers()
        {
            try
            {
                var lst = db.USERs.Where(o => o.role == "officer").ToList();
                return Request.CreateResponse(HttpStatusCode.OK, lst);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage SearchOfficer(string name)
        {
            try
            {
                var v = db.USERs.Where(s => s.name == name).FirstOrDefault();
                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Does not Exists");
                }
                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage ResetPassword(string email)
        {
            try { 
            // check if the email is valid
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email address.");
            }

            // generate random OTP
            string otp = GenerateOTP();

            // send email with OTP to the user
            SendEmail(email, otp);

           
                return Request.CreateResponse(HttpStatusCode.OK, otp);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
                return Usercheck(email);
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public bool Usercheck(string email)
        {
            try
            {
                USER user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        private string GenerateOTP()
        {
            var random = new Random();
            var otp = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                otp.Append(random.Next(0, 9).ToString());
            }

            return otp.ToString();
        }
        private void SendEmail(string email, string otp)
        {
            // Send the OTP to the user's email address
            var client = new SmtpClient
            {
                Host = "mail.hub4web.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("dengue@hub4web.com", ")~f=WoRKO8!+")
            };
            var mail = new MailMessage
            {
                From = new MailAddress("dengue@hub4web.com"),
                To = { email },
                Subject = "Password Reset OTP",
                //Body2 = " Your OTP for resetting your password is: " + otp +
                //"  Don't share this OTP with anyone", 
                Body = "\r\nHello,\r\nWe’ve received a request to reset the password for the Dengue Tracer account associated with " + email + ". No changes have been made to your account yet.\r\nYour otp to reset password is: " + otp + "\r\n\r\nIf you did not request a new password, please ignore this email.\r\n"
            };
            client.Send(mail);
        }





        //----------------------------------------------------------------------------//



        //-------------------------------   SECTORS API'S



        [HttpPost]
        public HttpResponseMessage NewSector()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                string name = request["sec_name"];
                string lat_long = request["lat_long"];
                SECTOR sect = db.SECTORS.Where(s => s.sec_name == name && s.lat_long == lat_long).FirstOrDefault();
                if (sect != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Exsist");
                }
                SECTOR newsector = new SECTOR
                {
                    sec_name = name,
                    lat_long = lat_long,
                };
                _ = db.SECTORS.Add(newsector);
                _ = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "New Sector Added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage SearchSector(string name)
        {
            try
            {
                var v = db.SECTORS.Where(s => s.sec_name == name).FirstOrDefault();
                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Does not Exists");
                }
                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //----------------------------------------------------------------------------//


        [HttpGet]
        public HttpResponseMessage GetSectors()
        {
            try
            {
                var lst = db.SECTORS.ToList();

                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //----------------------------------------------------------------------------//


        [HttpPost]
        public HttpResponseMessage AssignUserSector(int sec_id,/*List<int> lst,*/int user_id)
        {
            try
            {

                /*     foreach(var item in lst)
                     {
                         string quey = "insert into Dumm values('" + item + "','" + userid + "')";
                     }*/
                //string quey = "insert into ASSIGNSECTORS values('" + sec_id + "','" + user_id + "')";

                HttpRequest request = HttpContext.Current.Request;
                int secc_id = int.Parse(request["sec_id"]);
                int usser_id = int.Parse(request["user_id"]);
                USER ass = db.USERs.Where(s => s.user_id == usser_id).FirstOrDefault();
                //if (ass != null)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, "Allready Assigned");
                //}

                ass.sec_id = secc_id;

                _ = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sector " + secc_id + " Assigned to User : " + usser_id);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //----------------------------------------------------------------------------//


        [HttpPost]
        public HttpResponseMessage AssignOfficerSectors(int sec_id,/*List<int> lst,*/int user_id)
        {
            try
            {

                /*     foreach(var item in lst)
                     {
                         string quey = "insert into Dumm values('" + item + "','" + userid + "')";
                     }*/
                //string quey = "insert into ASSIGNSECTORS values('" + sec_id + "','" + user_id + "')";

                HttpRequest request = HttpContext.Current.Request;
                int secc_id = int.Parse(request["sec_id"]);
                int usser_id = int.Parse(request["user_id"]);
                /*ASSIGNSECTOR ass = db.ASSIGNSECTORS.Where(s => s.sec_id == secc_id && s.user_id == usser_id).FirstOrDefault();
                if (ass != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Exsist");
                }*/
                ASSIGNSECTOR newass = new ASSIGNSECTOR
                {
                    sec_id = secc_id,
                    user_id = usser_id,
                };
                _ = db.ASSIGNSECTORS.Add(newass);
                _ = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sector " + secc_id + " Assigned to User : " + user_id);
                // return Request.CreateResponse(HttpStatusCode.OK, "Sector Assigned " + quey);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage GetUserSectors()
        {
            try
            {

                //var Query = "select from us in USER join ass in ASSIGNSECTOR join sec in SECTOR";
                var lst = db.USERs.Where(u => u.role == "user");
                var result = from u in lst

                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, s.sec_id, s.sec_name, s.lat_long };


                /*     foreach(var item in lst)
                     {
                         string quey = "insert into Dumm values('" + item + "','" + userid + "')";
                     }*/

                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //----------------------------------------------------------------------------//

        [HttpGet]
        public HttpResponseMessage GetOfficerSectors()
        {
            try
            {

                //var Query = "select from us in USER join ass in ASSIGNSECTOR join sec in SECTOR";
                var lst = db.USERs.Where(u => u.role == "officer");
                var result = from u in lst
                             join a in db.ASSIGNSECTORS on u.user_id equals a.user_id
                             join s in db.SECTORS on a.sec_id equals s.sec_id
                             select new { u.user_id, u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, s.sec_id, s.sec_name, s.lat_long };


                /*     foreach(var item in lst)
                     {
                         string quey = "insert into Dumm values('" + item + "','" + userid + "')";
                     }*/

                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
