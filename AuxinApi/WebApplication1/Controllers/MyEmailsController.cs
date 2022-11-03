using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class MyEmailsController : ApiController
    {
        private readonly DbEntities db = new DbEntities(); 
        public IHttpActionResult GetAllEmails(int StartIndx, bool Unread)
        {
            try
            {
                var allEmails = db.MyEmails.ToList().Select(oo => new
                {
                    Id = oo.Id,
                    Name = oo.Name,
                    EmailId = oo.EmailId,
                    Message = oo.Message,
                    IsRead = oo.IsRead,
                    CreatedAt = oo.CreatedAt,
                    CreatedMonth = oo.CreatedAt.ToString("MMM, yyyy"),
                    CreatedDay = oo.CreatedAt.ToString("dd MMM"),
                }).ToList()
                .Where(cc => (Unread != true) || (cc.IsRead != true));

                var takeEmails = allEmails
                .OrderByDescending(dd => dd.CreatedAt).ToList()
                .Skip(StartIndx).Take(20);

                var res = takeEmails
                .GroupBy(vv => vv.CreatedMonth)
                .Select(ss => new
                {
                    CreatedMonth = ss.Key,
                    Emails = ss.ToList()
                }).ToList();

                return Ok(new ResultModel() {
                    Success = true,
                    Data = new
                    {
                        TotalEmails = allEmails.Count(), 
                        ListData = takeEmails,
                        GroupData = res,
                    },
                    ErrorMessage = string.Empty,
                    TechDetails= string.Empty,
                });
            }
            catch(Exception ex)
            {
                return Ok(new ResultModel()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Error getting data",
                    TechDetails = ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + (ex.InnerException == null ? "" : ex.InnerException.ToString())
                });
            } 
        }
        
        public IHttpActionResult GetEmailsCount(bool isRead)
        {
            try
            {
                var emails = db.MyEmails
                    .Where(oo => oo.IsRead != true)
                    .Select(oo => new MyEmailsModel()
                    {
                        Id = oo.Id,
                        Name = oo.Name,
                        EmailId = oo.EmailId,
                        Message = oo.Message,
                        IsRead = oo.IsRead,
                        CreatedAt = oo.CreatedAt,
                    }).ToList();
                return Ok(new ResultModel()
                {
                    Success = true,
                    Data =  emails.Count(),
                    ErrorMessage = string.Empty,
                    TechDetails = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResultModel()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Error getting data",
                    TechDetails = ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + (ex.InnerException == null ? "" : ex.InnerException.ToString())
                });
            }
        }

        [Route("api/MyEmails/MarkAsRead")]
        public IHttpActionResult PostMarkEmailAsRead(int key)
        {
            try
            {
                var emails = db.MyEmails.Where(oo => oo.Id == key).ToList();

                if (emails.Any())
                {
                    MyEmails obj = emails.First();

                    obj.IsRead = true;

                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                     
                    return Ok(new ResultModel()
                    {
                        Success = true,
                        Data = "READ",
                        ErrorMessage = string.Empty,
                        TechDetails = string.Empty,
                    });
                }else
                {
                    return Ok(new ResultModel()
                    {
                        Success = false,
                        Data = "UNREAD",
                        ErrorMessage = string.Empty,
                        TechDetails = string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultModel()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Error getting data",
                    TechDetails = ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + (ex.InnerException == null ? "" : ex.InnerException.ToString())
                });
            }
        }
         
        public IHttpActionResult PostEmail(MyEmails obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ResultModel()
                    {
                        Success = false,
                        Data = "",
                        ErrorMessage = "Invalid request",
                        TechDetails = "Invalid request",
                    });
                } 

                MyEmails objEmail = new MyEmails();

                objEmail.Id = 0;
                objEmail.Name = obj.Name;
                objEmail.EmailId = obj.EmailId;
                objEmail.Message = obj.Message;
                objEmail.IsRead = false;
                objEmail.CreatedAt = DateTime.Now;

                db.MyEmails.Add(objEmail);
                db.SaveChanges();

                //SendEmail(obj);

                return Ok(new ResultModel()
                {
                    Success = true,
                    Data = "Email sent successfully",
                    ErrorMessage = string.Empty,
                    TechDetails = string.Empty,
                });  
            }
            catch (Exception ex)
            {
                return Ok(new ResultModel()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Error getting data",
                    TechDetails = ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + (ex.InnerException == null ? "" : ex.InnerException.ToString())
                });
            }
        }

        private void SendEmail(MyEmails obj)
        {
            try
            {
                // Saving Photo to folder
                //var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/PHP/");
                var mappedPath = @"http://localhost:60851/";

                string apiUrl = Path.Combine(mappedPath, "send-email.php");

                var inputData = new
                {
                    name= obj.Name,
                    email= obj.EmailId,
                    message= obj.Message
                };

                string inputJson = JsonConvert.SerializeObject(inputData);

                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;

                string json = client.UploadString(apiUrl, inputJson);
                 
            }
            catch(Exception ex)
            {

            }
        }

        [Route("api/MyEmails/Delete")]
        public IHttpActionResult DeleteDeleteEmail(int key)
        {
            try
            {
                var emails = db.MyEmails.Where(oo => oo.Id == key).ToList();

                if (emails.Any())
                {
                    MyEmails obj = emails.First();

                    obj.IsRead = true;

                    db.MyEmails.Remove(obj);
                    db.SaveChanges();

                    return Ok(new ResultModel()
                    {
                        Success = true,
                        Data = "READ",
                        ErrorMessage = string.Empty,
                        TechDetails = string.Empty,
                    });
                }
                else
                {
                    return Ok(new ResultModel()
                    {
                        Success = false,
                        Data = "UNREAD",
                        ErrorMessage = string.Empty,
                        TechDetails = string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultModel()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Error getting data",
                    TechDetails = ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + (ex.InnerException == null ? "" : ex.InnerException.ToString())
                });
            }
        }

    }
}
