#define TRACE

using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Configuration;

namespace Rayonnance
{
	public static class SimpleSmtpClient
	{
		static void Main()
		{
			try
			{
				ReadAllSettings();

				// SmtpClient is configured through system.net/mailSettings/smtp/network section of SimpleSmtpClient.exe.config  
				var smtp = new SmtpClient();
				smtp.DeliveryFormat = SmtpDeliveryFormat.International;
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

				// these settings are read from appSettings section of SimpleSmtpClient.exe.config
				string fromEmail = ReadSetting("From");
				string toEmail = ReadSetting("To");
				string messageBody = ReadSetting("Body");
				string messageSubject = ReadSetting("Subject");

				MailMessage msg = new MailMessage();
				msg.From = new MailAddress(fromEmail);
				msg.To.Add(new MailAddress(toEmail));
				msg.Body = messageBody;
				msg.BodyEncoding = System.Text.Encoding.UTF8;
				msg.Subject = messageSubject;
				msg.SubjectEncoding = System.Text.Encoding.UTF8;

				smtp.Send(msg);
			}
			catch(Exception ex)
			{
				Trace.WriteLineIf(ex.InnerException != null, ex.InnerException.Message);
				Trace.WriteLine(ex.InnerException != null, ex.InnerException.StackTrace);
				Trace.WriteLine(ex.Message);
				Trace.WriteLine(ex.StackTrace);
			}
			Console.WriteLine("press any key to continue ...");
			Console.ReadKey(false);
		}

		static void ReadAllSettings()
		{
			try
			{
				var appSettings = ConfigurationManager.AppSettings;

				if (appSettings.Count == 0)
				{
					Trace.WriteLine("AppSettings is empty.");
				}
				else
				{
					foreach (var key in appSettings.AllKeys)
					{
						Trace.WriteLine(string.Format("Key: {0} Value: {1}", key, appSettings[key]));
					}
				}
			}
			catch (ConfigurationErrorsException)
			{
				Trace.WriteLine("Error reading app settings");
			}
		}

		static string ReadSetting(string key)
		{
			try
			{
				var appSettings = ConfigurationManager.AppSettings;
				string result = appSettings[key] ?? "Not Found";
				Trace.WriteLine(result);
				return result;
			}
			catch (ConfigurationErrorsException)
			{
				Trace.WriteLine("Error reading app settings");
				return string.Empty;
			}
		}

		static void AddUpdateAppSettings(string key, string value)
		{
			try
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;
				if (settings[key] == null)
				{
					settings.Add(key, value);
				}
				else
				{
					settings[key].Value = value;
				}
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
			}
			catch (ConfigurationErrorsException)
			{
				Trace.WriteLine("Error writing app settings");
			}
		}

	}
}
