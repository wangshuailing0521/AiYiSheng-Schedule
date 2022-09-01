using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WSL.AIYISHENG.Schedule
{
	public static class ApiHelper
	{
		public static string HttpRequest(string url, string data, string method = "PUT", string contentType = "application/json", Encoding encoding = null)
		{
			byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(data);
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = method;
			httpWebRequest.Timeout = 150000;
			httpWebRequest.AllowAutoRedirect = false;
			if (!string.IsNullOrEmpty(contentType))
			{
				httpWebRequest.ContentType = contentType;
			}
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
			}
			Stream stream = null;
			string result = null;
			try
			{
				if (bytes != null)
				{
					httpWebRequest.ContentLength = bytes.Length;
					stream = httpWebRequest.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
				}
				else
				{
					httpWebRequest.ContentLength = 0L;
				}
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					Stream responseStream = httpWebResponse.GetResponseStream();
					byte[] bytes2 = ReadFully(responseStream);
					responseStream.Close();
					result = Encoding.UTF8.GetString(bytes2);
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				httpWebRequest = null;
				stream = null;
			}
			return result;
		}

		public static byte[] ReadFully(Stream stream)
		{
			byte[] array = new byte[512];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				while (true)
				{
					int num = stream.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					memoryStream.Write(array, 0, num);
				}
				return memoryStream.ToArray();
			}
		}

		private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}

		public static string HttpPost(string url, string body)
		{
			Encoding uTF = Encoding.UTF8;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.Accept = "text/html, application/xhtml+xml, */*";
			httpWebRequest.ContentType = "application/json";
			byte[] bytes = uTF.GetBytes(body);
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}

		public static string HttpPut(string url, string body)
		{
			Encoding uTF = Encoding.UTF8;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "PUT";
			httpWebRequest.Accept = "text/html, application/xhtml+xml, */*";
			httpWebRequest.ContentType = "application/json";
			byte[] bytes = uTF.GetBytes(body);
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}

		public static string HttpGet(string url, Dictionary<string, object> dic, out int count)
		{
			string result = "";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(url);
			if (dic.Count > 0)
			{
				stringBuilder.Append("?");
				int num = 0;
				foreach (KeyValuePair<string, object> item in dic)
				{
					if (num > 0)
					{
						stringBuilder.Append("&");
					}
					stringBuilder.AppendFormat("{0}={1}", item.Key, item.Value);
					num++;
				}
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(stringBuilder.ToString());
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			WebHeaderCollection headers = httpWebResponse.Headers;
			count = Convert.ToInt32(headers["X-Total-Count"]);
			Stream responseStream = httpWebResponse.GetResponseStream();
			try
			{
				using (StreamReader streamReader = new StreamReader(responseStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			finally
			{
				responseStream.Close();
			}
			return result;
		}

		private static CspParameters GetCspKey()
		{
			return new CspParameters
			{
				KeyContainerName = "chait"
			};
		}

		public static string Encrypt(string palinData)
		{
			if (string.IsNullOrWhiteSpace(palinData))
			{
				return null;
			}
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(GetCspKey()))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(palinData);
				byte[] inArray = rSACryptoServiceProvider.Encrypt(bytes, fOAEP: false);
				return Convert.ToBase64String(inArray);
			}
		}

		public static string RSAEncrypt(string publickey, string content)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(publickey);
			byte[] inArray = rSACryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(content), fOAEP: false);
			return Convert.ToBase64String(inArray);
		}

		public static string GetPasswordSalt()
		{
			byte[] array = new byte[16];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
			{
				randomNumberGenerator.GetBytes(array);
			}
			return Convert.ToBase64String(array);
		}
	}

}
