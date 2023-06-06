using FYP_Api.HelperClasses;
using FYP_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;




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

        [HttpPost]
        public HttpResponseMessage ProfileUpdate()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                string email = request["email"];
                USER user = db.USERs.Where(s => s.email == email).FirstOrDefault();
                if (user != null)
                {
                    user.name = request["name"];
                    user.sec_id = int.Parse(request["sec_id"]);
                    user.phone_number = request["Phone_number"];
                    user.role = request["role"];
                    user.password = request["password"];
                    user.home_location = request["home_location"];
                    user.office_location = request["office_location"];

                    db.SaveChanges();
                    //
                    new Thread(() => SetNotification(user.user_id)).Start();


                    return Request.CreateResponse(HttpStatusCode.OK, "Updated");
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "Failed");
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
                if (!isRunning)
                {
                    isRunning = true;
                    new Thread(setStatus).Start();
                }

                var user = db.CASES_LOGS.FirstOrDefault(s => s.user_id == user_id);

                if (user == null)
                {
                    user = new CASES_LOGS(); // Create a new instance of CASES_LOGS
                    user.user_id = user_id;
                    user.status = status;
                    user.startdate = DateTime.Now.Date;
                    db.CASES_LOGS.Add(user);
                }
                else
                {
                    if (user.enddate == null && status == false)
                    {
                        user.enddate = DateTime.Now;
                        user.status = status;
                    }
                    else
                    {
                        user = new CASES_LOGS(); // Create a new instance of CASES_LOGS
                        user.user_id = user_id;
                        user.status = status;
                        user.startdate = DateTime.Now.Date;
                        db.CASES_LOGS.Add(user);
                    }
                }

                db.SaveChanges();

                new Thread(() => SetNotification(user_id)).Start();

                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        private void SetNotification(int userId)
        {
            try
            {
                var user = db.USERs.Find(userId);
                var sector = db.SECTORS.Find(user.sec_id);
                var cases = (from c in db.CASES_LOGS
                             join u in db.USERs on c.user_id equals u.user_id
                             where u.sec_id == user.sec_id
                             select c).ToList();
                var percentage = (cases.Count * 100) / sector.threshold;

                NOTIFICATION nOTIFICATION = new NOTIFICATION();
                //
                nOTIFICATION.date = DateTime.Now;
                //nOTIFICATION.percnt = percentage;
                nOTIFICATION.sec_id = user.sec_id;
                nOTIFICATION.status = 0;
                nOTIFICATION.user_id = user.user_id;
                db.NOTIFICATIONs.Add(nOTIFICATION);
                db.SaveChanges();

                if (percentage == 25)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;

                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }
                if (percentage == 50)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;

                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }
                if (percentage == 75)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;

                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }
                if (percentage == 85)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;
                    nOTIFICATION.type = true;

                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }
                if (percentage == 95)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;
                    nOTIFICATION.type = true;


                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }

                if (percentage > 95 && percentage <= 100)
                {
                    nOTIFICATION.percnt = percentage;
                    nOTIFICATION.sec_id = user.sec_id;
                    nOTIFICATION.status = 0;
                    nOTIFICATION.user_id = user.user_id;
                    nOTIFICATION.type = true;


                    db.NOTIFICATIONs.Add(nOTIFICATION);
                    db.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                // Handle the exception accordingly
                // For example, log the error or display a message
            }
        }



        /*  [HttpGet]
          public HttpResponseMessage ShowallNotifications(int user_id, int? radius)
          {
              try
              {
                  var user = db.USERs.Find(user_id);
                  List<NOTIFICATION> locationBased = new List<NOTIFICATION>();
                  List<NOTIFICATION> sectorBased = new List<NOTIFICATION>();

                  if (user.role == "user")
                  {
                      sectorBased = db.NOTIFICATIONs.Where(s => s.sec_id == user.sec_id).ToList();

                      foreach (var notification in db.NOTIFICATIONs)
                      {
                          var user2 = db.USERs.Find(notification.user_id);
                          if (CalculateDistance(user.home_location.Split(',')[0], user.home_location.Split(',')[1], user2.home_location.Split(',')[0], user2.home_location.Split(',')[1]) <= radius)
                          {
                              locationBased.Add(notification);
                          }
                      }
                  }
                  else if (user.role == "officer")
                  {
                      var sectors = db.ASSIGNSECTORS.Where(s => s.user_id == user.user_id).ToList();
                      foreach (var sector in sectors)
                      {
                          sectorBased.AddRange(db.NOTIFICATIONs.Where(s => s.sec_id == sector.sec_id).ToList());
                      }
                  }
                  else
                  {
                      sectorBased = db.NOTIFICATIONs.ToList();
                  }

                  var locationBasedCount = locationBased.Count;
                  var sectorBasedCount = sectorBased.Count;

                  return Request.CreateResponse(HttpStatusCode.OK, new
                  {
                      locationBased = locationBased.OrderByDescending(s => s.date),
                      sectorBased = sectorBased.OrderByDescending(s => s.date),
                      locationBasedCount,
                      sectorBasedCount
                  });
              }
              catch (Exception ex)
              {
                  return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
              }
          }
  */






        [HttpGet]
        public HttpResponseMessage ShowallNotifications2(int user_id, int? radius)
        {
            try
            {
                var user = db.USERs.Find(user_id);
                List<NOTIFICATION> locationBased = new List<NOTIFICATION>();
                List<NOTIFICATION> sectorBased = new List<NOTIFICATION>();

                if (user.role == "user")
                {
                    sectorBased = db.NOTIFICATIONs.Where(s => s.sec_id == user.sec_id && s.type != null).ToList();

                    foreach (var notification in db.NOTIFICATIONs)
                    {
                        if (notification.type == null)
                        {
                            var user2 = db.USERs.Find(notification.user_id);
                            // var user2 = db.USERs.Where(s => s.user_id == notification.user_id);
                            if (CalculateDistance(user.home_location.Split(',')[0], user.home_location.Split(',')[1], user2.home_location.Split(',')[0], user2.home_location.Split(',')[1]) <= radius)
                            {
                                locationBased.Add(notification);
                            }
                        }
                    }
                }
                else if (user.role == "officer")
                {
                    var sectors = db.ASSIGNSECTORS.Where(s => s.user_id == user.user_id).ToList();
                    foreach (var sector in sectors)
                    {
                        sectorBased.AddRange(db.NOTIFICATIONs.Where(s => s.sec_id == sector.sec_id && s.type != null).ToList());
                    }
                }
                else
                {
                    sectorBased = db.NOTIFICATIONs.Where(s => s.type != null).ToList();
                }

                var locationBasedCount = locationBased.Count;
                var sectorBasedCount = sectorBased.Count;

                var locationBasedResponse = locationBased.OrderByDescending(s => s.date)
                    .Select(s => new
                    {
                        s.date,
                        //s.percnt,
                        s.type,
                        s.status,
                        s.sec_id,
                        // sec_name = db.SECTORS.Find(s.sec_id)?.sec_name
                    });

                var sectorBasedResponse = sectorBased.OrderByDescending(s => s.date)
                    .Select(s => new
                    {
                        s.date,
                        s.percnt,
                        s.type,
                        s.status,
                        s.sec_id,
                        sec_name = db.SECTORS.Find(s.sec_id)?.sec_name
                    });

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    locationBased = locationBasedResponse,
                    sectorBased = sectorBasedResponse,
                    locationBasedCount,
                    sectorBasedCount
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        public double CalculateDistance(string lat1, string lon1, string lat2, string lon2)
        {
            // Radius of the Earth in kilometers
            const double EarthRadius = 6371;

            // Convert latitude and longitude to radians
            double lat1Rad = ToRadians(double.Parse(lat1));
            double lon1Rad = ToRadians(double.Parse(lon1));
            double lat2Rad = ToRadians(double.Parse(lat2));
            double lon2Rad = ToRadians(double.Parse(lon2));

            // Calculate the differences in latitude and longitude
            double latDiff = lat2Rad - lat1Rad;
            double lonDiff = lon2Rad - lon1Rad;

            // Calculate the haversine of half the latitudinal difference
            double haversineLat = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2);

            // Calculate the haversine of half the longitudinal difference
            double haversineLon = Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

            // Calculate the square of the haversine of the angular distance
            double haversine = haversineLat + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * haversineLon;

            // Calculate the central angle using the inverse haversine function
            double centralAngle = 2 * Math.Asin(Math.Sqrt(haversine));

            // Calculate the distance using the central angle and the Earth's radius
            double distance = EarthRadius * centralAngle;

            return distance;
        }

        public double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }




        /*
                [HttpPost]
                public HttpResponseMessage CreateNotification(int sec_id, bool status)
                {
                    try
                    {
                        var user = db.CASES_LOGS.Where(s => s.case_id == sec_id).FirstOrDefault();
                        if (user == null)
                        {
                            DateTime dat = DateTime.Now;
                            var dt = dat.Date.ToShortDateString();

                            NOTIFICATION newCase = new NOTIFICATION();

                            newCase.sec_id = sec_id;
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


                [HttpPost]
                public HttpResponseMessage CreateNotification(int user_id, int sec_id, string title, bool type, bool status)
                {
                    try
                    {
                        DateTime date = DateTime.Now.Date;
                        NOTIFICATION newCase = new NOTIFICATION();

                        newCase.user_id = user_id;
                        newCase.sec_id = sec_id;
                        newCase.title = title;
                        newCase.type = type;
                        newCase.status = status;
                        newCase.date = date;

                        db.NOTIFICATIONs.Add(newCase);
                        db.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, newCase);
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }*/


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
                if (!isRunning)
                {
                    isRunning = true;
                    new Thread(setStatus).Start();
                }
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
                                     u.user_id,
                                     u.name,
                                     u.email,
                                     u.phone_number,
                                     u.image,
                                     u.role,
                                     u.home_location,
                                     u.office_location,
                                     u.sec_id,
                                     s.sec_name,
                                     s.description,
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
                                    u.user_id,
                                    u.image,
                                    u.name,
                                    u.email,
                                    u.phone_number,
                                    u.role,
                                    u.home_location,
                                    u.office_location,
                                    u.sec_id,
                                };
                    return Request.CreateResponse(HttpStatusCode.OK, admin);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        // [HttpGet]

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




        [HttpGet]
        public HttpResponseMessage Login2(string email, string password)
        {
            try
            {
                if (!isRunning)
                {
                    isRunning = true;
                    new Thread(setStatus).Start();
                }

                var user = db.USERs.Where(s => s.email == email && s.password == password).FirstOrDefault();

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "false");
                }
                else if (user.role.ToLower() == "user")
                {
                    var result = from u in db.USERs
                                 join c in db.CASES_LOGS on u.user_id equals c.user_id into caselogs
                                 from c in caselogs.DefaultIfEmpty()
                                 join s in db.SECTORS on u.sec_id equals s.sec_id
                                 where u.email == email && u.password == password
                                 select new
                                 {
                                     u.user_id,
                                     u.name,
                                     u.email,
                                     u.phone_number,
                                     u.image,
                                     u.role,
                                     u.home_location,
                                     u.password,
                                     u.office_location,
                                     u.sec_id,
                                     s.sec_name,
                                     s.description,
                                     startdate = (DateTime?)c.startdate,
                                     status = (bool?)(c != null ? c.status : false),
                                     enddate = (DateTime?)c.enddate
                                 };
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else if (user.role.ToLower() == "officer")
                {
                    var officer = from u in db.USERs
                                  join a in db.ASSIGNSECTORS on u.user_id equals a.user_id
                                  join s in db.SECTORS on a.sec_id equals s.sec_id
                                  where u.email == email && u.password == password
                                  group s by new { u.user_id, u.name, u.email, u.phone_number, u.password, u.image, u.role, u.home_location, u.office_location, u.sec_id } into g
                                  select new
                                  {
                                      g.Key.user_id,
                                      g.Key.name,
                                      g.Key.email,
                                      g.Key.phone_number,
                                      g.Key.image,
                                      g.Key.password,
                                      g.Key.role,
                                      g.Key.home_location,
                                      g.Key.office_location,
                                      g.Key.sec_id,
                                      sectors = g.Select(se => new
                                      {
                                          se.sec_id,
                                          se.sec_name,
                                          se.description
                                      })
                                  };
                    return Request.CreateResponse(HttpStatusCode.OK, officer.FirstOrDefault());
                }
                else
                {
                    var admin = from u in db.USERs.Where(s => s.email == email && s.password == password)
                                select new
                                {
                                    u.user_id,
                                    u.image,
                                    u.name,
                                    u.email,
                                    u.password,
                                    u.phone_number,
                                    u.role,
                                    u.home_location,
                                    u.office_location,
                                    u.sec_id,
                                };
                    return Request.CreateResponse(HttpStatusCode.OK, admin);
                }
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





        static bool isRunning = false;


        private void setStatus()
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                var dt = DateTime.Now;
                var currentDate = DateTime.Now.Date;
                var startDateThreshold = currentDate.AddDays(-7);

                var casesByDate = db.CASES_LOGS
                    .AsEnumerable()
                    .Where(c =>
                        (c.enddate == null && DateTime.Now.Subtract(c.startdate).Days >= 7)).ToList();
                for (int i = 0; i < casesByDate.Count; i++)
                {
                    casesByDate[i].enddate = DateTime.Now;

                }
                db.SaveChanges();
                Thread.Sleep(720000);
                setStatus();
            }
            catch (Exception ex) { }
        }
        /*
                [HttpGet]
                public HttpResponseMessage GetDengueCasesByDatee()
                {
                    try
                    {
                        ProjectEntities db = new ProjectEntities();
                        var lst = db.USERs.Where(u => u.role == "user");
                        var result = from u in lst
                                     join c in db.CASES_LOGS on u.user_id equals c.user_id
                                     join s in db.SECTORS on u.sec_id equals s.sec_id
                                     select new { u.name, u.email, u.phone_number, u.role, u.home_location, u.sec_id, s.sec_name, c.startdate, c.enddate, c.status };

                        var currentDate = DateTime.Now.Date;
                        var startDateThreshold = currentDate.AddDays(-7);

                        var cases = db.CASES_LOGS.Where(s=>s.startdate==DateTime.Now||s.enddate==null).ToList();
                        var d = new {
                            Date = DateTime.Now,
                            count = cases.Count
                        };// Order by date

                        // Update enddate for cases with null enddate that have exceeded 6 days
                        var casesToUpdate = result
                            .Where(c => c.enddate == null && c.startdate <= startDateThreshold.AddDays(-6))
                            .ToList();



                        db.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, casesByDate.OrderByDescending(n => n.Date));
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
        */




        //----------------------------------------------------------------------------//

        [HttpGet]
        public IHttpActionResult GetDengueCasesByDate()
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = minimumDate;

                    // Calculate the count for the current date
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => s.startdate.Date == minimumDate.Date ||
                                    (s.startdate.Date < minimumDate.Date && (s.enddate == null || s.enddate >= minimumDate)));

                    /*caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => s.startdate.Date <= minimumDate.Date &&
                                   (s.enddate == null || s.enddate >= minimumDate));
*/
                    cases.Add(caseData);
                    minimumDate = minimumDate.AddDays(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        public IHttpActionResult GetDengueCasesBySectorDate(int sec_id)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = minimumDate;

                    // Calculate the count for the current date and sector
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => (s.startdate.Date == minimumDate.Date ||
                                     (s.startdate.Date < minimumDate.Date && (s.enddate == null || s.enddate >= minimumDate))) &&
                                    db.USERs.Any(u => u.user_id == s.user_id && u.sec_id == sec_id));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddDays(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        public IHttpActionResult GetDengueCasesByMonth()
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = new DateTime(minimumDate.Year, minimumDate.Month, 1);

                    // Calculate the count for the current month
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => (s.startdate.Year == minimumDate.Year && s.startdate.Month == minimumDate.Month ||
                                     (s.startdate.Year < minimumDate.Year || (s.startdate.Year == minimumDate.Year && s.startdate.Month < minimumDate.Month)) &&
                                     (s.enddate == null || s.enddate.Value.Year > minimumDate.Year || (s.enddate.Value.Year == minimumDate.Year && s.enddate.Value.Month >= minimumDate.Month))));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddMonths(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }






        [HttpGet]
        public IHttpActionResult GetDengueCasesBySectorMonth(int sec_id)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = new DateTime(minimumDate.Year, minimumDate.Month, 1);

                    // Calculate the count for the current month and sector
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => (s.startdate.Year == minimumDate.Year && s.startdate.Month == minimumDate.Month ||
                                     (s.startdate.Year < minimumDate.Year || (s.startdate.Year == minimumDate.Year && s.startdate.Month < minimumDate.Month)) &&
                                     (s.enddate == null || s.enddate.Value.Year > minimumDate.Year || (s.enddate.Value.Year == minimumDate.Year && s.enddate.Value.Month >= minimumDate.Month))) &&
                                    db.USERs.Any(u => u.user_id == s.user_id && u.sec_id == sec_id));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddMonths(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public IHttpActionResult GetDengueCasesByYear()
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = new DateTime(minimumDate.Year, 1, 1);

                    // Calculate the count for the current year
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => (s.startdate.Year == minimumDate.Year ||
                                     (s.startdate.Year < minimumDate.Year) &&
                                     (s.enddate == null || s.enddate.Value.Year > minimumDate.Year)));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddYears(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDengueCasesBySectorYear(int sec_id)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Now.Date;

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = new DateTime(minimumDate.Year, 1, 1);

                    // Calculate the count for the current year and sector
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => (s.startdate.Year == minimumDate.Year ||
                                     (s.startdate.Year < minimumDate.Year) &&
                                     (s.enddate == null || s.enddate.Value.Year > minimumDate.Year)) &&
                                    db.USERs.Any(u => u.user_id == s.user_id && u.sec_id == sec_id));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddYears(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
        public IHttpActionResult GetDengueCasesByDate(int daysToSubtract)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                DateTime currentDate = DateTime.Today.AddDays(-daysToSubtract);

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = minimumDate;

                    // Calculate the count for the current date
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => s.startdate.Date == minimumDate.Date ||
                                    (s.startdate.Date < minimumDate.Date && (s.enddate == null || s.enddate >= minimumDate)));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddDays(1);
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        public IHttpActionResult GetDengueCasesByDate2(int daysToSubtract)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                List<UserInfo> users = new List<UserInfo>();  // List to store users

                DateTime currentDate = DateTime.Today.AddDays(-daysToSubtract);

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = minimumDate;

                    // Calculate the count for the current date
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => s.startdate.Date == minimumDate.Date ||
                                    (s.startdate.Date < minimumDate.Date && (s.enddate == null || s.enddate >= minimumDate)));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddDays(1);
                }

                // Retrieve users based on the selected date
                users = GetUsersByDate(currentDate, db);

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    users = users,  // Include users in the result
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Method to retrieve users based on the selected date
        private List<UserInfo> GetUsersByDate(DateTime selectedDate, ProjectEntities db)
        {
            var users = db.USERs
                .Where(u => u.role == "user")
                .Join(db.CASES_LOGS, u => u.user_id, c => c.user_id, (u, c) => new { u, c })
                .Join(db.SECTORS, x => x.u.sec_id, s => s.sec_id, (x, s) => new { x.u, x.c, s })
                .Select(x => new UserInfo
                {
                    user_id = x.u.user_id,
                    name = x.u.name,
                    email = x.u.email,
                    phone_number = x.u.phone_number,
                    role = x.u.role,
                    home_location = x.u.home_location,
                    office_location = x.u.office_location,
                    sec_id = (int)x.u.sec_id,
                    sec_name = x.s.sec_name,
                    description = x.s.description,
                    startdate = x.c.startdate,
                    status = (bool)x.c.status,
                    enddate = x.c.enddate
                })
                .ToList();

            var filteredUsers = users
                .Where(u => u.startdate.Date == selectedDate.Date && (u.enddate == null || u.enddate >= selectedDate))
                .ToList();

            return filteredUsers;
        }




        /// <summary>
        ///
        /// </summary>
        /// <param name=" "NOt working"daysToSubtract"></param>
        /// <returns></returns>

        [HttpGet]
        public IHttpActionResult GetDengueCasesByDate3(int daysToSubtract)
        {
            try
            {
                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();
                List<UserInfo> users = new List<UserInfo>();  // List to store users

                DateTime currentDate = DateTime.Today.AddDays(-daysToSubtract);

                var minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                while (minimumDate <= currentDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = minimumDate;

                    // Calculate the count for the current date
                    caseData.count = db.CASES_LOGS
                        .Count(s => DbFunctions.TruncateTime(s.startdate) == DbFunctions.TruncateTime(minimumDate) ||
                                    (DbFunctions.TruncateTime(s.startdate) < DbFunctions.TruncateTime(minimumDate) &&
                                    (s.enddate == null || DbFunctions.TruncateTime(s.enddate) >= DbFunctions.TruncateTime(minimumDate))));

                    cases.Add(caseData);
                    minimumDate = minimumDate.AddDays(1);
                }

                // Retrieve users for the selected date
                var usersQuery = db.USERs
    .Where(u => u.role == "user")
    .Join(db.CASES_LOGS, u => u.user_id, c => c.user_id, (u, c) => new { u, c })
    .Join(db.SECTORS, x => x.u.sec_id, s => s.sec_id, (x, s) => new { x.u, x.c, s })
    .Where(x => DbFunctions.TruncateTime(x.c.startdate) == DbFunctions.TruncateTime(currentDate) &&
                (x.c.enddate == null || DbFunctions.TruncateTime(x.c.enddate) >= DbFunctions.TruncateTime(currentDate) || x.c.enddate > currentDate))
    .Select(x => new UserInfo
    {
        user_id = x.u.user_id,
        name = x.u.name,
        email = x.u.email,
        phone_number = x.u.phone_number,
        role = x.u.role,
        home_location = x.u.home_location,
        office_location = x.u.office_location,
        sec_id = (int)x.u.sec_id,
        sec_name = x.s.sec_name,
        description = x.s.description,
        startdate = x.c.startdate,
        status = (bool)x.c.status,
        enddate = x.c.enddate
    });


                users = usersQuery.ToList();

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date),
                    users = users,  // Include users in the result
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }







        /*      [HttpGet]
              public IHttpActionResult GetDengueUsersByDate2(int daysToSubtract)
              {
                  try
                  {
                      DateTime targetDate = DateTime.Today.AddDays(-daysToSubtract);
                      List<UserInfo> users = GetUsersByDate(targetDate);
                      List<CasesHelperClass> cases = GetCasesByDate(targetDate, users);
                      int maxValue = cases.Max(s => s.count) + 5;

                      var result = new
                      {
                          cases = cases.OrderByDescending(s => s.date),
                          maxValue
                      };

                      return Ok(result);
                  }
                  catch (Exception ex)
                  {
                      return InternalServerError(ex);
                  }
              }

              private List<UserInfo> GetUsersByDate(DateTime targetDate)
              {
                  return db.USERs
                      .Where(u => u.role == "user")
                      .Join(db.CASES_LOGS, u => u.user_id, c => c.user_id, (u, c) => new { u, c })
                      .Join(db.SECTORS, x => x.u.sec_id, s => s.sec_id, (x, s) => new { x.u, x.c, s })
                      .Where(x => x.c.startdate == targetDate)
                      .Select(x => new UserInfo
                      {    
                          user_id =x.u.user_id,
                          name = x.u.name,
                          email = x.u.email,
                          phone_number = x.u.phone_number,
                          role = x.u.role,
                          home_location = x.u.home_location,
                          office_location = x.u.office_location,
                          sec_id = (int)x.u.sec_id,
                          sec_name = x.s.sec_name,
                          description = x.s.description,
                          startdate = x.c.startdate,
                          status = (bool)x.c.status,
                          enddate = x.c.enddate
                      })
                      .ToList();
              }

              private List<CasesHelperClass> GetCasesByDate(DateTime targetDate, List<UserInfo> users)
              {

                  List<CasesHelperClass> cases = new List<CasesHelperClass>();
                  DateTime minimumDate = db.CASES_LOGS.OrderBy(s => s.startdate).Select(s => s.startdate).FirstOrDefault();

                  while (minimumDate <= targetDate)
                  {
                      CasesHelperClass caseData = new CasesHelperClass();
                      caseData.date = minimumDate;
                      caseData.count = CalculateCaseCount(minimumDate, users);
                      caseData.users = GetUsersForDate(minimumDate, users);

                      cases.Add(caseData);
                      minimumDate = minimumDate.AddDays(1);
                  }

                  return cases.OrderByDescending(s => s.date).ToList();
              }

              private int CalculateCaseCount(DateTime date, List<UserInfo> users)
              {
                  return users.Count(u => u.startdate.Date == date.Date);
              }

              private List<UserInfo> GetUsersForDate(DateTime date, List<UserInfo> users)
              {
                  return users.Where(u => u.startdate.Date == date.Date).ToList();
              }

      */



        /*  [HttpGet]
          public IHttpActionResult GetDengueUsersByDate2(int daysToSubtract)
          {
              try
              {
                  DateTime targetDate = DateTime.Today.AddDays(-daysToSubtract);
                  List<UserInfo> users = GetUsersByDate(targetDate);
                  List<CasesHelperClass> cases = GetCasesByDate(targetDate, users);
                  int maxValue = cases.Max(s => s.count) + 5;

                  var result = new
                  {
                      cases = cases.OrderByDescending(s => s.date),
                      maxValue
                  };

                  return Ok(result);
              }
              catch (Exception ex)
              {
                  return InternalServerError(ex);
              }
          }



          // ...

          private List<UserInfo> GetUsersByDate(DateTime targetDate)
          {
              var users = db.USERs
                  .Where(u => u.role == "user")
                  .Join(db.CASES_LOGS, u => u.user_id, c => c.user_id, (u, c) => new { u, c })
                  .Join(db.SECTORS, x => x.u.sec_id, s => s.sec_id, (x, s) => new { x.u, x.c, s })
                  .Select(x => new UserInfo
                  {
                      user_id = x.u.user_id,
                      name = x.u.name,
                      email = x.u.email,
                      phone_number = x.u.phone_number,
                      role = x.u.role,
                      home_location = x.u.home_location,
                      office_location = x.u.office_location,
                      sec_id = (int)x.u.sec_id,
                      sec_name = x.s.sec_name,
                      description = x.s.description,
                      startdate = x.c.startdate,
                      status = (bool)x.c.status,
                      enddate = x.c.enddate
                  })
                  .ToList();

              var filteredUsers = users
                  .Where(u => u.startdate.Date <= targetDate.Date && (u.enddate == null || u.enddate >= targetDate))
                  .ToList();

              return filteredUsers;
          }






          // ...

          private List<CasesHelperClass> GetCasesByDate(DateTime targetDate, List<UserInfo> users)
          {
              List<CasesHelperClass> cases = new List<CasesHelperClass>();

              // Calculate counts for remaining dates
              var caseCounts = db.CASES_LOGS
                  .Where(c => SqlFunctions.DateDiff("day", c.startdate, targetDate) != 0 && (c.enddate == null || c.enddate >= targetDate))
                  .GroupBy(c => SqlFunctions.DateDiff("day", c.startdate, targetDate))
                  .Select(g => new { DateDiff = g.Key, Count = g.Count() })
                  .ToList();

              // Add case data for the selected date
              CasesHelperClass selectedDateCase = new CasesHelperClass();
              selectedDateCase.date = targetDate;
              selectedDateCase.count = CalculateCaseCount(targetDate, users);
              selectedDateCase.users = GetUsersForDate(targetDate, users);
              cases.Add(selectedDateCase);

              // Add case data for the remaining dates
              foreach (var caseCount in caseCounts)
              {
                  DateTime date = targetDate.AddDays(-(int)caseCount.DateDiff);
                  CasesHelperClass caseData = new CasesHelperClass();
                  caseData.date = date;
                  caseData.count = caseCount.Count;
                  cases.Add(caseData);
              }

              return cases.OrderByDescending(s => s.date).ToList();
          }

          private int CalculateCaseCount(DateTime date, List<UserInfo> users)
          {
              return users.Count(u => u.startdate.Date == date.Date);
          }

          private List<UserInfo> GetUsersForDate(DateTime date, List<UserInfo> users)
          {
              return users.Where(u => u.startdate.Date == date.Date).ToList();
          }*/





        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>


        [HttpGet]
        public IHttpActionResult GetDengueCasesByDateRange(string from, string to)
        {
            try
            {
                DateTime fromDate;
                DateTime toDate;

                if (!DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate))
                {
                    return BadRequest("Invalid 'from' date format. Please provide the date in yyyy-MM-dd format.");
                }

                if (!DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                {
                    return BadRequest("Invalid 'to' date format. Please provide the date in yyyy-MM-dd format.");
                }

                toDate = toDate.AddDays(1); // Increment the 'to' date by 1 to include the entire day

                ProjectEntities db = new ProjectEntities();
                List<CasesHelperClass> cases = new List<CasesHelperClass>();

                while (toDate <= fromDate)
                {
                    CasesHelperClass caseData = new CasesHelperClass();
                    caseData.date = toDate;

                    // Calculate the count for the current date
                    caseData.count = db.CASES_LOGS.AsEnumerable()
                        .Count(s => s.startdate.Date == toDate.Date ||
                                    (s.startdate.Date < toDate.Date && (s.enddate == null || s.enddate >= toDate)));

                    cases.Add(caseData);
                    toDate = toDate.AddDays(1);
                }

                if (!cases.Any())
                {
                    // Handle the case when no data is available in the given date range
                    return Ok(new { cases = Enumerable.Empty<CasesHelperClass>(), maxValue = 0 });
                }

                var result = new
                {
                    cases = cases.OrderByDescending(s => s.date.ToString("yyyy-MM")),
                    maxValue = cases.Max(s => s.count) + 5
                };

                return Ok(result);
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
                    // Get the users in the current sector
                    var usersInSector = db.USERs.Where(u => u.sec_id == sector.sec_id && u.role == "user");

                    // Get the case logs for the users in the current sector
                    //var caseLogs = db.CASES_LOGS.Where(c => usersInSector.Any(u => u.user_id == c.user_id) && c.status == true);
                    var caseLogs = db.CASES_LOGS.Where(c => usersInSector.Any(u => u.user_id == c.user_id));


                    // Count the number of case logs in the current sector
                    var numCases = caseLogs.Count();

                    var polygons = db.POLYGONS.Where(p => p.sec_id == sector.sec_id).ToList();

                    var latLongs = polygons.Select(p => p.lat_long).ToList();

                    var sectorObject = new
                    {
                        sec_id = sector.sec_id,
                        sec_name = sector.sec_name,
                        threshold = sector.threshold,
                        description = sector.description,
                        latLongs = latLongs,
                        total_cases = numCases
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
        public HttpResponseMessage GetUserSectors()
        {
            try
            {
                var result = from u in db.USERs
                             join s in db.SECTORS on u.sec_id equals s.sec_id
                             let sector_users = db.USERs.Where(us => us.sec_id == u.sec_id)
                             let sector_cases = db.CASES_LOGS.Where(c => sector_users.Any(us => us.user_id == c.user_id))
                             //let sector_cases = db.CASES_LOGS.Where(c => sector_users.Any(us => us.user_id == c.user_id) && c.status == true)

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
                                                         .ToList(),
                                     total_cases = sector_cases.Count(),
                                     //sector_users = sector_users.Select(us => new
                                     //{
                                     //    us.user_id,
                                     //    us.name,
                                     //    us.email,
                                     //    us.phone_number,
                                     //    us.role,
                                     //    us.home_location,
                                     //    us.office_location,
                                     //    total_cases = db.CASES_LOGS.Where(c => c.user_id == us.user_id && c.status == true).Count()
                                     //}).ToList()
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
                             let sector_users = db.USERs.Where(us => us.sec_id == s.sec_id)
                             let sector_cases = db.CASES_LOGS.Where(c => sector_users.Any(us => us.user_id == c.user_id))
                             //let sector_cases = db.CASES_LOGS.Where(c => sector_users.Any(us => us.user_id == c.user_id) && c.status == true)

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
                                     latLongs = db.POLYGONS.Where(p => p.sec_id == s.sec_id).Select(p => p.lat_long).ToList(),
                                     total_cases = sector_cases.Count(),
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
                    return Request.CreateResponse(HttpStatusCode.OK, "Assigned");
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




    }
}
