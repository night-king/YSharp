using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace YSharp.SDK
{
    /// <summary>
    /// IP转换为位置信息
    /// </summary>
    public class IP2RegionHelper
    {
        //IP地址正则表达式
        public const string IP_Regex = @"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)";


        /// <summary>
        /// 获取地理位置信息
        /// 仅仅中国返回省市信息，其他国家值返回国家
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static LocationResult Convert(string ip)
        {
            var millseconds = 0;
            if (new Regex(IP_Regex).IsMatch(ip) == false)
            {
                return null;
            }
            var sp = new System.Diagnostics.Stopwatch();
            sp.Start();
            var locationResult = fromIP2Region(ip);
            if (locationResult == null)
            {
                locationResult = fromIp138(ip);
                if (locationResult == null)
                {
                    locationResult = fromTaobao(ip);
                }
            }
            sp.Stop();
            millseconds = (int)sp.ElapsedMilliseconds;
            return locationResult == null ? null : new LocationResult(locationResult.from, locationResult.country, locationResult.province, locationResult.city, millseconds);
        }

        private static LocationResult fromIP2Region(string ip)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "db", "ip2region.db");
                if (File.Exists(path))
                {
                    using (var _search = new IP2Region.DbSearcher(path))
                    {
                        var region = _search.MemorySearch(ip);
                        if (region != null && !string.IsNullOrEmpty(region.Region))
                        {
                            var location = region.Region;//国家|区域|省份|城市|ISP
                            if (!string.IsNullOrEmpty(location))
                            {
                                var arr = location.Replace("0", "").Split("|");
                                var country = arr.Length > 0 ? arr[0] : "";
                                var province = arr.Length > 2 ? arr[2] : "";
                                var city = arr.Length > 3 ? arr[3] : "";
                                var isp = arr.Length > 4 ? arr[4] : "";
                                return new LocationResult("ip2region", country, province, city, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        private static LocationResult fromIp138(string ip)
        {
            try
            {
                var url = string.Format("http://www.ip138.com/ips138.asp?action=2&ip=" + ip);
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,zh-TW;q=0.8");
                    client.DefaultRequestHeaders.Add("Accept", "text/html;charset=gb2312");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.62 Safari/537.36");
                    var result = client.GetAsync(url).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var sm = result.Content.ReadAsStreamAsync().Result;
                        using (StreamReader sr = new StreamReader(sm, Encoding.Default))
                        {
                            var html = sr.ReadToEnd();
                            var regex = new Regex(@"本站数据：(.*)  </li>");
                            if (regex.IsMatch(html))
                            {
                                var match = regex.Match(html).Value;
                                if (match.Length > 10)
                                {
                                    var matchEnd = match.IndexOf("</li>");
                                    var location = match.Substring(5, matchEnd - 5).Trim();//江苏省镇江市 电信 or 日本 or 美国 or 德国 巴伐利亚
                                    if (!string.IsNullOrEmpty(location))
                                    {
                                        var country = "";
                                        var province = "";
                                        var city = "";
                                        var s1 = location.IndexOf(' ') > 0 ? location.Split(' ')[0] : location;
                                        if (location.StartsWith("中国") || (location.Contains("省") && location.Contains("市")))
                                        {
                                            country = "中国";
                                            province = location.Split("省")[0];
                                            city = location.Split("省")[1].Replace("市", "");
                                        }
                                        else
                                        {
                                            country = location;
                                        }
                                        return new LocationResult("ip138.com", country, province, city, 0);

                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        private static LocationResult fromTaobao(string ip)
        {
            try
            {
                var url = string.Format("http://ip.taobao.com/service/getIpInfo.php?ip=" + ip);
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var result = client.GetAsync(url).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var json = result.Content.ReadAsStringAsync().Result;
                        var taobaoLocation = JsonConvert.DeserializeObject<TaobaoLocation>(json);

                        if (taobaoLocation != null && taobaoLocation.code == 0 && taobaoLocation.data != null)
                        {
                            var country = taobaoLocation.data.country;
                            var province = country == "中国" ? taobaoLocation.data.region : "";
                            var city = country == "中国" ? taobaoLocation.data.city : "";
                            return new LocationResult("ip.taobao.com", country, province, city, 0);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }

    public class LocationResult
    {
        public string from { set; get; }
        public string country { set; get; }
        public string province { set; get; }
        public string city { set; get; }
        public int ElapsedMilliseconds { set; get; }

        public LocationResult(string from, string country, string province, string city, int elapsedMilliseconds)
        {
            this.from = from;
            this.country = country;
            this.province = province;
            this.city = city;
            this.ElapsedMilliseconds = elapsedMilliseconds;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}", country, province, city);
        }
    }


    public class TaobaoLocation
    {
        public int code { set; get; }
        public TaobaoLocationData data { set; get; }
    }

    public class TaobaoLocationData
    {
        public string ip { set; get; }
        public string country { set; get; }
        public string area { set; get; }
        public string region { set; get; }
        public string city { set; get; }
        public string county { set; get; }
        public string isp { set; get; }
        public string country_id { set; get; }
        public string area_id { set; get; }
        public string region_id { set; get; }
        public string city_id { set; get; }
        public string county_id { set; get; }
        public string isp_id { set; get; }

    }
}
