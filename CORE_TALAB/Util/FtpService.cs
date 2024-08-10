
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CORE_TALAB.Util
{
    public class FtpService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private static string hostFTP;
        private static string userNameFTP;
        private static string passwordFTP;
        private static string portFTP;
        private static string mediaUrl;
        public FtpService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            mediaUrl = _configuration.GetSection("Path:MediaServerC3").Value;
            userNameFTP = Utils.DecodePassword(_configuration.GetSection("FTP:UserName").Value, Utils.EncodeType.SHA_256);
            passwordFTP = Utils.DecodePassword(_configuration.GetSection("FTP:Password").Value, Utils.EncodeType.SHA_256);
            hostFTP = _configuration.GetSection("FTP:Host").Value;
            portFTP = _configuration.GetSection("FTP:Port").Value;
        }

        public string SaveFTPFile(byte[] fileBytes, string fileName, string pathString)
        {
            try
            {
                //var pathString = string.Format("{0}/{1}/{2}/", "XLVPGT", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.Hour);
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                string ftpPath = hostFTP + ":" + portFTP + "//" + (pathString + "/").Replace("/", "//") + fileName;
                client.UploadData(ftpPath, fileBytes);
                return "/" + pathString + "/" + fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveFTPFile: {0}", ex.Message);
                return null;
            }
        }

        public string SaveFTPFileDocument(byte[] fileBytes, string fileName, string pathString)
        {
            try
            {
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                string ftpPath = hostFTP + ":" + portFTP + "//" + (pathString + "/").Replace("/", "//") + fileName;
                client.UploadData(ftpPath, fileBytes);
                return "/" + pathString + "/" + fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveFTPFileDocument Status: {0}", ex.Message);
                return null;
            }
        }

        public string UploadedFile(string fileImageBase64, string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileImageBase64))
                    return null;

                string fileName = string.Format("{0}_{1}.jpeg", Guid.NewGuid().ToString(), Id);
                byte[] fileBytes = Convert.FromBase64String(fileImageBase64);

                var pathString = string.Format("{0}/{1}/", "Checkin", DateTime.Now.ToString("yyyy-MM-dd"));
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                client.UploadData(checkDirectory + "/" + fileName, fileBytes);
                return "/" + pathString + fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string FTPRemoveDirectory(string DirectoryName)
        {
            try
            {
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                var TakeFilePath = DirectoryName.Split("/");
                var FilePath = TakeFilePath.Take(TakeFilePath.Length - 1);
                foreach (var item in FilePath)
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        return null;
                    }
                }
                string ftpPath = hostFTP + ":" + portFTP + "" + (DirectoryName);
                var credentials = new NetworkCredential(userNameFTP, passwordFTP);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                request.Credentials = credentials;
                request.UsePassive = false;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string RemoveFtpDirectory(string url)
        {
            try
            {
                string ftpPath = hostFTP + ":" + portFTP + "" + (url);
                NetworkCredential credentials = new NetworkCredential(userNameFTP, passwordFTP);

                return DeleteFtpDirectory(ftpPath, credentials);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string DeleteFtpDirectory(string url, NetworkCredential credentials)
        {
            try
            {
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listRequest.Credentials = credentials;

                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                {
                    using (var reader = new System.IO.StreamReader(listResponse.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            string itemName = tokens[tokens.Length - 1];
                            string itemPath = $"{url}/{itemName}";

                            if (tokens[0][0] == 'd') // Directory
                            {
                                DeleteFtpDirectory(itemPath, credentials);
                            }
                            else // File
                            {
                                FtpWebRequest deleteRequest = (FtpWebRequest)WebRequest.Create(itemPath);
                                deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                                deleteRequest.Credentials = credentials;

                                using (FtpWebResponse deleteResponse = (FtpWebResponse)deleteRequest.GetResponse())
                                {

                                }
                            }
                        }
                    }
                }

                FtpWebRequest removeDirRequest = (FtpWebRequest)WebRequest.Create(url);
                removeDirRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                removeDirRequest.Credentials = credentials;

                using (FtpWebResponse removeDirResponse = (FtpWebResponse)removeDirRequest.GetResponse())
                {
                    return removeDirResponse.StatusDescription;
                }
            }
            catch (WebException ex)
            {
                _logger.LogError("RemoveFTPFileImage Status: {0}", ex.Message);
                return null;
            }
        }

        public string RemoveFTPFileDocument(string fileName)
        {
            try
            {
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                var TakeFilePath = fileName.Split("/");
                var FilePath = TakeFilePath.Take(TakeFilePath.Length - 1);
                foreach (var item in FilePath)
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        return null;
                    }
                }
                string ftpPath = hostFTP + ":" + portFTP + "" + (fileName);
                var credentials = new NetworkCredential(userNameFTP, passwordFTP);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RemoveFTPFileDocument Status: {0}", ex.Message);
                return null;
            }
        }

        public Byte[] DownloadFile(string fileNamePath)
        {
            try
            {
                byte[] fileData = null;

                string ftpfullpath = string.Format("{0}:{1}/{2}", hostFTP, portFTP, fileNamePath);
                using (WebClient request = new WebClient())
                {
                    if (!string.IsNullOrEmpty(userNameFTP) || !string.IsNullOrEmpty(passwordFTP))
                        request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                    fileData = request.DownloadData(ftpfullpath);
                }

                if (fileData != null && fileData.Length > 0)
                    return fileData;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public bool GetDirectoryExits(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //long size = response.ContentLength;
                response.Close();

                return true;
            }
            catch (WebException e)
            {
                _logger.LogInformation("GetDirectoryExits: {0}", e.Message);
                return false;
            }
        }

        public Byte[] DownloadFileByUrl0(string fileNamePath)
        {
            try
            {
                byte[] fileData = null;
                var url = new Uri(mediaUrl + fileNamePath);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (WebClient request = new WebClient())
                {
                    fileData = request.DownloadData(url);
                }

                if (fileData != null && fileData.Length > 0)
                    return fileData;
            }
            catch (Exception ex)
            {
                _logger.LogError("DownloadFileByUrl: " + ex.Message);
                return null;
            }
            return null;
        }
        public async Task<Byte[]> DownloadFileByUrl(string fileNamePath)
        {
            try
            {

                byte[] fileData = null;
                var url = new Uri(mediaUrl + fileNamePath);

                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                };

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    fileData = await httpClient.GetByteArrayAsync(url).ConfigureAwait(false);
                }

                if (fileData != null && fileData.Length > 0)
                    return fileData;
            }
            catch (Exception ex)
            {
                _logger.LogError("DownloadFileByUrl ", ex.ToString());
                return null;
            }
            return null;
        }

        public bool CreateDirectory(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using var resp = (FtpWebResponse)request.GetResponse();
                if (resp.StatusCode == FtpStatusCode.PathnameCreated)
                {
                    resp.Close();
                    return true;
                }
                resp.Close();
                return false;

            }
            catch (WebException e)
            {
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                _logger.LogError("FTPService CreateDirectory:" + status);
                return false;
            }
        }

        public string ConvertImageToBase64(byte[] imageBytes)
        {
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}
