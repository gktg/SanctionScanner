using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using HtmlAgilityPack;


namespace SanctionScanner
{
    /// <summary>
    /// Author: Servet Göktuğ Türkan
    /// Date: 25.08.2022
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {



            int milliseconds = 3000;
            Thread.Sleep(milliseconds);


            string url = "https://www.sahibinden.com/";

            List<Sahibinden> urunList = new List<Sahibinden>();


            using (WebClient client = new WebClient())
            {
                HtmlDocument sahibindenDocument = new HtmlDocument();
                //We make the webclient act like a browser to bypass the Sahibinden.com site, and we add headers for this.
                client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                string html = client.DownloadString(url);
                sahibindenDocument.LoadHtml(html);

                //We take the "Anasayfa Vitrin" as the "anasayfaVitrinList". We use HtmlAgilityPack for this.
                IEnumerable<HtmlNode> anasayfaVitrinList = sahibindenDocument.DocumentNode.SelectNodes("//ul[@class='vitrin-list clearfix']//li//a");

                //We create the product object.
                Sahibinden urunModel = new Sahibinden();

                decimal urunFiyatToplami = 0;


                foreach (HtmlNode urun in anasayfaVitrinList)
                {

                    int milliseconds2 = 5000;
                    Thread.Sleep(milliseconds2);

                    //We are assigning the profile url of the product.
                    string urunProfilUrl = url + urun.Attributes["href"].Value;

                    //We do the same thing we did for sahibinden.com in the product profile, and we get the html of the page.
                    HtmlDocument urunProfilDocument = new HtmlDocument();
                    client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    string html2 = client.DownloadString(urunProfilUrl);
                    urunProfilDocument.LoadHtml(html2);

                    //We get the price of the product from the page.
                    HtmlNode urunProfilFiyat = urunProfilDocument.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()");


                    //We are assigning the product object.
                    urunModel.UrunAdi = urun.InnerText.Trim();
                    if (urunProfilFiyat != null)
                    {
                        urunModel.UrunFiyati = urunProfilFiyat.InnerText.Trim();
                        decimal urunFiyat = Convert.ToDecimal(urunModel.UrunFiyati.Replace(".", "").Replace("TL", ""));
                        urunFiyatToplami += urunFiyat;
                    }
                    else
                    {
                        urunModel.UrunFiyati = "";
                    }

                    urunList.Add(urunModel);

                }
                urunModel.UrunlerOrtalamFiyat = urunFiyatToplami / anasayfaVitrinList.Count();





                //We determine the path of the txt file to be written.
                string fileName = @"C:\Users\goktu\Desktop\İlanlar.txt";

                string writeText = "";


                for (int x = 0; x < urunList.Count(); x++)
                {
                    Console.WriteLine($"{x + 1}- İlan Adı: {urunList[x].UrunAdi} ve İlan Fiyatı: {urunList[x].UrunFiyati}\n");
                    writeText += ($"{x + 1}- İlan Adı: {urunList[x].UrunAdi} ve İlan Fiyatı: {urunList[x].UrunFiyati}\n");
                }


                //We create txt file.
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Close();
                File.AppendAllText(fileName, writeText);


                Console.ReadLine();


            }






        }
    }



    class Sahibinden
    {
        public string UrunAdi { get; set; }
        public string UrunFiyati { get; set; }
        public decimal UrunlerOrtalamFiyat { get; set; }
        public string Url { get; set; }
    }
}




