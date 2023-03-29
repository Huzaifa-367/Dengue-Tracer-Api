//using FYP_Api.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using Windows.Data.Xml.Dom;
//using Windows.UI.Notifications;

//namespace FYP_Api.Controllers
//{
//    public class NotificationController : ApiController
//    {
//        private readonly ProjectEntities db = new ProjectEntities();

//        [HttpGet]
//        public List<ItemLists> GetNotifications()
//        {
//            List<ItemLists> notifitems = new List<ItemLists>();
//            try
//            {
//                var data = db.NOTIFICATIONs.ToList();
//                foreach (var item in data)
//                {
//                    ItemLists newItem = new ItemLists()
//                    {
//                        title = item.title,
//                        description = item.description,
//                        alert = item.alert,
//                        datetime = item.datetime
//                    };
//                    notifitems.Add(newItem);
//                }

//                return notifitems;
//            }
//            catch (Exception ex)
//            {
//                // handle exception
//                return null;
//            }
//        }

//        [HttpPost]
//        public HttpResponseMessage UpdateUserStatus(int user_id, bool status)
//        {
//            try
//            {
//                var user = db.CASES_LOGS.Where(s => s.user_id == user_id).FirstOrDefault();
//                if (user == null)
//                {
//                    DateTime dat = DateTime.Now;
//                    var dt = dat.Date.ToShortDateString();

//                    CASES_LOGS newCase = new CASES_LOGS();

//                    newCase.user_id = user_id;
//                    newCase.status = status;
//                    newCase.startdate = DateTime.Parse(dt);

//                    db.CASES_LOGS.Add(newCase);
//                    db.SaveChanges();

//                    // Show a notification for the new case
//                    string notificationText = "New case registered";
//                    ShowNotification(notificationText);

//                    return Request.CreateResponse(HttpStatusCode.OK, newCase);
//                }

//                user.status = status;
//                db.SaveChanges();

//                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
//            }
//            catch (Exception ex)
//            {
//                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
//            }
//        }

//        private void ShowNotification(string text)
//        {
//            // Create the notification
//            ToastNotification notification = new ToastNotification(CreateToastXml(text));

//            // Show the notification
//            ToastNotificationManager.CreateToastNotifier().Show(notification);
//        }

//        private XmlDocument CreateToastXml(string text)
//        {
//            // Create the toast XML document
//            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

//            // Set the text of the toast
//            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
//            toastTextElements[0].InnerText = text;

//            return toastXml;
//        }
//    }
//}
