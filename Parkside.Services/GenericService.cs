﻿using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace exp.NET6.Services.DBServices
{
    public class GenericService : IGenericService
    {
        private (string, string) ExtrageImagBase64(string dataUri)
        {
            string delim = ";base64,"; // Delimitatorul dintre tipul de imagine și codificarea Base64

            if (!dataUri.Contains(delim))
            {
                throw new ArgumentException("Image format is not valid");
            }

            // Separă tipul de imagine și codificarea Base64
            string[] parts = dataUri.Split(delim, 2, StringSplitOptions.None);
            string[] parts2 = parts[0].Split('/');

            string tipImag = parts2[1];
            string codificareBase64 = parts[1];

            return (tipImag, codificareBase64);
        }

        private string Base64Type(string data)
        {
            string type;
            switch (data.ToUpper())
            {
                case "IVBOR":
                    type = "png";
                    break;
                case "/9J/4":
                    type = "jpg";
                    break;
                default:
                    type = "jpg";
                    break;
            }
            string format = "data:image/" + type + ";base64,";
            return format;
        }

        public string? GetImagePath(string? newImgBase64, string? oldImgUrl, string folderName)
        {
            string? filePath = File.Exists(oldImgUrl) ? oldImgUrl : null;

            if (!String.IsNullOrWhiteSpace(newImgBase64))
            {
                var (imgType, ImageBase64) = ExtrageImagBase64(newImgBase64);
                byte[] imageBytes = Convert.FromBase64String(ImageBase64);
                string input = Path.Combine(Directory.GetCurrentDirectory());
                //string pattern = @"C:\\Projects\\5.Delicitate\\";

                string pattern = @"C:\\Users\\[^\\]+\\source\\repos\\";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(input);

                if (match.Success)
                {
                    string path = match.Value;
                    string folder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("PathForImages")["path"];
                    string baseFolderPath = path + folder;

                    //string baseFolderPath = "C:\\Users\\epentilescu\\source\\repos\\OnlineShop-Administration-App\\exp.net6.backend\\Images";
                    string folderPath = Path.Combine(baseFolderPath, folderName);

                    if (!File.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string randomFileName;
                    do
                    {
                        randomFileName = Path.GetRandomFileName() + "." + imgType;
                    }
                    while (File.Exists(randomFileName)); //Genereaza nume de fisier pana gaseste unul care nu exista in folder

                    if (!String.IsNullOrWhiteSpace(filePath))
                    {
                        var brandImage = Convert.ToBase64String(File.ReadAllBytes(filePath));
                        brandImage = Base64Type(brandImage.Substring(0, 5)) + brandImage;

                        if (brandImage != newImgBase64)
                            File.Delete(filePath);
                    }

                    filePath = Path.Combine("\\" + folderName + "\\", randomFileName);
                    string writePath = Path.Combine(folderPath, randomFileName);
                    File.WriteAllBytes(writePath, imageBytes);
                }
            }
            return filePath;
        }

        public string? GetImgBase64(string? filePath)
        {
            if (filePath != null)
            {
                string input = Path.Combine(Directory.GetCurrentDirectory());
                string pattern = @"C:\\Users\\[^\\]+\\source\\repos\\";
                //string pattern = @"C:\\Projects\\5.Delicitate\\";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(input);

                if (match.Success)
                {
                    string path = match.Value;
                    string folder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("PathForImages")["path"];
                    string baseFolderPath = path + folder;
                    string folderPath = baseFolderPath + filePath;

                    if (!File.Exists(folderPath))
                        return null;

                    string image = Convert.ToBase64String(File.ReadAllBytes(folderPath));

                    if (String.IsNullOrEmpty(image))
                        return null;

                    image = Base64Type(image.Substring(0, 5)) + image;

                    return image;
                }
            }

            return null;
        }
        public bool ValidateEmail(string emailAddress)
        {
            // regex pt validare email
            // https://github.com/jquense/yup/issues/507
            // / ^(([^<> ()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)| (".+"))@((\[[0 - 9]{ 1,3}\.[0 - 9]{ 1,3}\.[0 - 9]{ 1,3}\.[0 - 9]{ 1,3}])| (([a - zA - Z\-0 - 9] +\.)+[a - zA - Z]{ 2,}))$/
            try
            {
                var email = new MailAddress(emailAddress);
                return email.Address == emailAddress.Trim();
            }
            catch
            {
                return false;
            }
        }
        public bool ValidatePhoneNumber(string number)
        {
            string motif = @"^([\+]?40[-]?|[0])?[1-9][0-9]{8}$";

            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }

        public bool ValidatePostalCode(string postalCode)
        {
            string motif = @"^[0-9]{6}$";

            if (postalCode != null) return Regex.IsMatch(postalCode, motif);
            else return false;
        }
        public bool ValidateDay(string day)
        {
            var daysOfTheWeek = new List<string>()
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday",
        };

            if (daysOfTheWeek.Contains(day))
                return true;
            return false;
        }

        public int GetDayNumber(string day)
        {
            var daysOfTheWeek = new List<string>()
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday",
        };

            return daysOfTheWeek.IndexOf(day);
        }

        public bool ValidateTimeFormat(string time)
        {
            string motif = @"^([0-1][0-9]|2[0-3]):[0-5][0-9]$";
            if (time != null) return Regex.IsMatch(time, motif);
            else return false;
        }

        public bool ValidateDate(string start, string end)
        {
            try
            {
                DateTime startDate = DateTime.Parse(start);
                DateTime endDate = DateTime.Parse(end);

                if (startDate > endDate)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateTime(string start, string end)
        {
            if (ValidateTimeFormat(start) && ValidateTimeFormat(end))
            {
                if (DateTime.Parse(start) < DateTime.Parse(end))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public string GeneratePassword()
        {
            Random random = new Random();
            char[] keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890qwzwaesxrdctfvygbuhnertyuiopasdfghjklzxcvbnm!@#$%^&*(){}|[]',./<>? ".ToCharArray();

            var password = Enumerable
                .Range(1, 35)
                .Select(k => keys[random.Next(0, keys.Length - 1)])
                .Aggregate("", (e, c) => e + c);

            return password;
        }
    }
}
