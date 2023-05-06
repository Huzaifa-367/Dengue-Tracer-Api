using FYP_Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Windows.UI.Xaml.Shapes;

namespace FYP_Api.Controllers
{
    public class SERVERController : ApiController
    {

        private readonly ProjectEntities db = new ProjectEntities();


        [HttpPost]
        public HttpResponseMessage SignUp()
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
                    sec_id = int.Parse(request["sec_id"]),
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
                    DateTime dat = DateTime.Now;
                    var dt = dat.Date.ToShortDateString();

                    //int range = 5;
                    //-------------------------------------------------
                    CASES_LOGS newCase = new CASES_LOGS();

                    newCase.user_id = user_id;
                    newCase.status = status;
                    newCase.startdate = DateTime.Parse(dt);
                    //newCase.range = range;
                    db.CASES_LOGS.Add(newCase);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, newCase);

                }
                user.status = status;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage CreateNotification(int case_id, bool status)
        {
            try
            {
                var user = db.CASES_LOGS.Where(s => s.case_id == case_id).FirstOrDefault();
                if (user == null)
                {
                    DateTime dat = DateTime.Now;
                    var dt = dat.Date.ToShortDateString();

                    NOTIFICATION newCase = new NOTIFICATION();

                    newCase.case_id = case_id;
                    newCase.type = false;
                    newCase.title = "New case in your area";
                    newCase.date = DateTime.Parse(dt);

                    db.NOTIFICATIONs.Add(newCase);
                    db.SaveChanges();

                    //// Show a notification for the new case
                    //string notificationText = "New case registered";
                    //ShowNotification(notificationText);

                    return Request.CreateResponse(HttpStatusCode.OK, newCase);
                }

                user.status = status;
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage ShowallNotifications()
        {
            try
            {
                var lst = db.NOTIFICATIONs.OrderByDescending(n => n.date).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //private void ShowNotification(string text)
        //{
        //    // Create the notification
        //    ToastNotification notification = new ToastNotification(CreateToastXml(text));

        //    // Show the notification
        //    ToastNotificationManager.CreateToastNotifier().Show(notification);
        //}

        //private XmlDocument CreateToastXml(string text)
        //{
        //    // Create the toast XML document
        //    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

        //    // Set the text of the toast
        //    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
        //    toastTextElements[0].InnerText = text;

        //    return toastXml;
        //}


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
                else if (user.role.ToLower() == "user")
                {

                    var result = from u in db.USERs
                                 join c in db.CASES_LOGS
                                     on u.user_id equals c.user_id into caselogs
                                 from c in caselogs.DefaultIfEmpty()
                                 join s in db.SECTORS on u.sec_id equals s.sec_id
                                 where u.email == email && u.password == password
                                 select new
                                 {
                                     u.user_id, u.name, u.email, u.phone_number, u.image, u.role,
                                     u.home_location, u.office_location,u.sec_id,
                                     s.sec_name, s.description,
                                     startdate = (DateTime?)c.startdate,
                                     status = (bool?)(c != null ? c.status : false),
                                     enddate = (DateTime?)c.enddate
                                 };
                return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {

                    var admin = from u in db.USERs.Where(s => s.email == email && s.password == password)
                    select new
                               {
                                   u.user_id, u.image, u.name,u.email, u.phone_number,
                                   u.role, u.home_location, u.office_location, u.sec_id,
                               };
                    return Request.CreateResponse(HttpStatusCode.OK, admin);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //[HttpGet]
        //public HttpResponseMessage Login(string email, string password)
        //{
        //    try
        //    {
        //        var user = db.USERs.Where(s => s.email == email && s.password == password).FirstOrDefault();
        //        if (user == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, "false");
        //        }
        //        else
        //        {
        //            var result = from u in db.USERs
        //                         where u.email == email && u.password == password
        //                         join c in db.CASES_LOGS on u.user_id equals c.user_id
        //                         join s in db.SECTORS on u.sec_id equals s.sec_id

        //                         select new { u.user_id, u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, u.sec_id, s.sec_name, s.description, c.startdate, c.status, c.enddate };

        //            return Request.CreateResponse(HttpStatusCode.OK, result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}


        //[HttpGet]
        //public HttpResponseMessage Login(string email, string password)
        //{
        //    try
        //    {
        //        var user = db.USERs.Where(s => s.email == email && s.password == password).FirstOrDefault();
        //        if (user == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, "false");
        //        }
        //        else if (user.role == "user")
        //        {

        //            var result = from u in db.USERs
        //                         join c in db.CASES_LOGS
        //                             on u.user_id equals c.user_id into caselogs
        //                         from c in caselogs.DefaultIfEmpty()
        //                         join s in db.SECTORS on u.sec_id equals s.sec_id
        //                         where u.email == email && u.password == password
        //                         select new
        //                         {
        //                             u.user_id,
        //                             u.image,
        //                             u.name,
        //                             u.email,
        //                             u.phone_number,
        //                             u.role,
        //                             u.home_location,
        //                             u.office_location,
        //                             u.sec_id,
        //                             s.sec_name,
        //                             s.description,
        //                             startdate = (DateTime?)c.startdate,
        //                             status = (bool?)(c != null ? c.status : false),
        //                             enddate = (DateTime?)c.enddate
        //                         };

        //            // Convert result to a list of dictionaries
        //            var dataList = result.ToList().Select(r => new Dictionary<string, object>
        //    {
        //        { "user_id", r.user_id },
        //        { "name", r.name },
        //        { "email", r.email },
        //        { "image", r.image },
        //        { "phone_number", r.phone_number },
        //        { "role", r.role },
        //        { "home_location", r.home_location },
        //        { "office_location", r.office_location },
        //        { "sec_id", r.sec_id },
        //        { "sec_name", r.sec_name },
        //        { "description", r.description },
        //        { "startdate", r.startdate },
        //        { "status", r.status },
        //        { "enddate", r.enddate }
        //    });

        //            return Request.CreateResponse(HttpStatusCode.OK, dataList);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, user);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}


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
        public HttpResponseMessage GetDengueCasesByDate()
        {
            try
            {
                var lst = db.USERs.Where(u => u.role == "user");
                var result = from u in lst
                             join c in db.CASES_LOGS on u.user_id equals c.user_id
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.sec_id, s.sec_name, c.startdate, c.status };

                var casesByDate = result
                    .GroupBy(c => c.startdate) // Group cases by date
                    .Select(g => new { Date = g.Key, Count = g.Count() }) // Project to date and count
                    .OrderBy(d => d.Date); // Order by date
               // var cases = casesByDate.OrderByDescending(n=> n.Date);
                return Request.CreateResponse(HttpStatusCode.OK, casesByDate.OrderByDescending(n => n.Date));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [HttpGet]
        public HttpResponseMessage GetDengueCasesByMonth()
        {
            try
            {
                var lst = db.USERs.Where(u => u.role == "user").ToList();
                var result = from u in lst
                             join c in db.CASES_LOGS on u.user_id equals c.user_id
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.sec_id, s.sec_name, c.startdate, c.status };

                var casesByMonth = result
                    .GroupBy(c => new { Year = c.startdate.HasValue ? c.startdate.Value.Year : (int?)null, Month = c.startdate.HasValue ? c.startdate.Value.Month : (int?)null })
                    .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Count = g.Count() })
                    .OrderBy(d => d.Month);

                return Request.CreateResponse(HttpStatusCode.OK, casesByMonth.OrderByDescending(n => n.Month));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDengueCasesByYear()
        {
            try
            {
                var lst = db.USERs.Where(u => u.role == "user").ToList();
                var result = from u in lst
                             join c in db.CASES_LOGS on u.user_id equals c.user_id
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.sec_id, s.sec_name, c.startdate, c.status };

                var casesByYear = result
                                  .GroupBy(c => new { Year = c.startdate.HasValue ? c.startdate.Value.Year : (int?)null })
                                  .Select(g => new { Year = g.Key.Year.ToString(), Count = g.Count() })
                                  .OrderBy(d => d.Year);

                return Request.CreateResponse(HttpStatusCode.OK, casesByYear.OrderByDescending(n => n.Year));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetDengueUsersByDate(int daysToSubtract)
        {
            try
            {
                var targetDate = DateTime.Today.AddDays(-daysToSubtract);
                var lst = db.USERs.Where(u => u.role == "user");
                var result = from u in lst
                             join c in db.CASES_LOGS on u.user_id equals c.user_id
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             where c.startdate == targetDate
                             select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, u.sec_id, s.sec_name, s.description, c.startdate, c.status, c.enddate };

                return Request.CreateResponse(HttpStatusCode.OK, result.OrderByDescending(n => n.startdate));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult GetDengueCasesByDateRange(string from, string to)
        {
            try
            {
                DateTime fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);

                var casesInRange = db.CASES_LOGS
                    .Where(c => c.startdate.HasValue && c.startdate.Value >= fromDate && c.startdate.Value < toDate)
                    .GroupBy(c => DbFunctions.TruncateTime(c.startdate))
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(d => d.Date);

                return Ok(casesInRange);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


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
                             select new { u.user_id, u.name, u.email, u.phone_number, u.role, u.home_location, u.office_location, u.sec_id, s.sec_name, s.description, c.startdate, c.status, c.enddate };


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
        public HttpResponseMessage CreateOfficerAndAssignSector()
        
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                string email = request["email"];
                USER user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Officer Already Exist");
                }

                USER newuser = new USER
                {
                    email = email,
                    name = request["name"],
                    phone_number = request["Phone_number"],
                    role = "officer",
                    password = request["password"],
                };

                // Assign sector to user
                int sectorId;
                if (!int.TryParse(request["sec_id"], out sectorId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid sector ID");
                }

                ASSIGNSECTOR ass = db.ASSIGNSECTORS.Where(s => s.sec_id == sectorId && s.user_id == newuser.user_id).FirstOrDefault();
                if (ass != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sector already assigned to this user");
                }

                newuser = db.USERs.Add(newuser);
                db.SaveChanges();

                ASSIGNSECTOR newass = new ASSIGNSECTOR
                {
                    sec_id = sectorId,
                    user_id = newuser.user_id,
                };
                _ = db.ASSIGNSECTORS.Add(newass);
                _ = db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Officer account created and sector assigned");
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
            try
            {
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
            catch (Exception)
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
        public HttpResponseMessage SavePolygons(dynamic data)
        {
            try
            {
                // Parse the data from the request body
                string secName = data.secName;
                int threshold = data.threshold;
                string description = data.description;
                List<double[]> latLongs = data.latLongs.ToObject<List<double[]>>();
               
                // First, check if the sector already exists
                var existingSector = db.SECTORS.FirstOrDefault(s => s.sec_name == secName);
                if (existingSector == null)
                {
                    // Create a new sector if it doesn't exist
                    var sector = new SECTOR
                {
                    sec_name = secName,
                    threshold = threshold,
                    description = description
                };

                // Add the sector to the database
                db.SECTORS.Add(sector);
                db.SaveChanges();

                // Loop through the latLongs and add each point to the database as a new polygon
                foreach (var latLong in latLongs)
                {
                    var polygon = new POLYGON
                    {
                        sec_id = sector.sec_id,
                        lat_long = string.Join(",", latLong)
                    };
                    db.POLYGONS.Add(polygon);
                }
                db.SaveChanges();
                }
                else
                {
                    // Loop through the latLongs and add each point to the database as a new polygon
                    return Request.CreateResponse(HttpStatusCode.OK, "Sector Already Exsist");
                }
                // If the sector already exists, just add the polygons
                // Return a success response
                return Request.CreateResponse(HttpStatusCode.OK, "Sector saved successfully");
            }
            catch (Exception ex)
            {
                // Return an error response if any exception occurs
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        [HttpGet]
        public HttpResponseMessage GetAllSectors()
        {
            try
            {
                // Retrieve all sectors from the database
                var sectors = db.SECTORS.ToList();
                // Create a list to store the sector data as JSON objects
                var sectorData = new List<object>();
                // Loop through each sector and extract the necessary data
                foreach (var sector in sectors)
                {
                    var polygons = db.POLYGONS.Where(p => p.sec_id == sector.sec_id).ToList();

                    var latLongs = polygons.Select(p => p.lat_long).ToList();

                    var sectorObject = new
                    {
                        sec_id = sector.sec_id,
                        sec_name = sector.sec_name,
                        threshold = sector.threshold,
                        description = sector.description,
                        latLongs = latLongs
                    };

                    sectorData.Add(sectorObject);
                }

                // Return the sector data as a JSON object
                return Request.CreateResponse(HttpStatusCode.OK, sectorData);
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
                //int secc_id = sec_id;
                //int usser_id = user_id;
                USER ass = db.USERs.Where(s => s.user_id == user_id).FirstOrDefault();
                //if (ass != null)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, "Allready Assigned");
                //}

                ass.sec_id = sec_id;

                _ = db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sector " + sec_id + " Assigned to User : " + user_id);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //----------------------------------------------------------------------------//

        [HttpPost]
        public HttpResponseMessage AssignOfficerSectors(int sec_id, int user_id)
        {
            try
            {
                ASSIGNSECTOR ass = db.ASSIGNSECTORS.Where(s => s.sec_id == sec_id && s.user_id == user_id).FirstOrDefault();
                if (ass == null)
                {
                    ASSIGNSECTOR newass = new ASSIGNSECTOR
                    {
                        sec_id = sec_id,
                        user_id = user_id,
                    };
                    _ = db.ASSIGNSECTORS.Add(newass);
                    _ = db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK,"Assigned");
                }
                else
                {
                    ass.sec_id = sec_id;
                    _ = db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Updated");
                }
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
                var lst = db.USERs.Where(u => u.role == "user");
                var result = from u in lst
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             select new
                             {
                                 user_id = u.user_id,
                                 name = u.name,
                                 email = u.email,
                                 phone_number = u.phone_number,
                                 role = u.role,
                                 home_location = u.home_location,
                                 office_location = u.office_location,
                                 sector = new
                                 {
                                     sec_id = s.sec_id,
                                     sec_name = s.sec_name,
                                     threshold = s.threshold,
                                     description = s.description,
                                     latLongs = db.POLYGONS.Where(p => p.sec_id == s.sec_id)
                                                         .Select(p => p.lat_long)
                                                         .ToList()
                                 }
                             };
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
                var lst = db.USERs.Where(u => u.role == "officer");
                var result = from u in lst
                             join a in db.ASSIGNSECTORS on u.user_id equals a.user_id
                             join s in db.SECTORS on a.sec_id equals s.sec_id
                             select new
                             {
                                 u.user_id,
                                 u.name,
                                 u.email,
                                 u.phone_number,
                                 u.role,
                                 u.home_location,
                                 u.office_location,
                                 sector = new
                                 {
                                     s.sec_id,
                                     s.sec_name,
                                     s.threshold,
                                     s.description,
                                     latLongs = db.POLYGONS.Where(p => p.sec_id == s.sec_id).Select(p => p.lat_long).ToList()
                                 }
                             };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
