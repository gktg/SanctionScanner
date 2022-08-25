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

            try
            {
                int milliseconds = 3000;
                Thread.Sleep(milliseconds);


                string url = "https://www.sahibinden.com/";

                List<Product> productList = new List<Product>();




                using (WebClient client = new WebClient())
                {
                    HtmlDocument sahibindenDocument = new HtmlDocument();

                    //We make the webclient act like a browser to bypass the Sahibinden.com site, and we add headers for this.
                    client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    string html = client.DownloadString(url);
                    sahibindenDocument.LoadHtml(html);

                    //We take the "Anasayfa Vitrin" as the "homepageFeaturedAdsList". We use HtmlAgilityPack for this.
                    List<HtmlNode> homepageFeaturedAdsList = sahibindenDocument.DocumentNode.SelectNodes("//ul[@class='vitrin-list clearfix']//li//a").ToList();

                    HomepageFeaturedAds homepageFeaturedAdsModel = new HomepageFeaturedAds();

                    decimal productsTotalPrice = 0;


                    //foreach (HtmlNode urun in anasayfaVitrinList)
                    foreach (HtmlNode homepageFeaturedAds in homepageFeaturedAdsList)
                    {
                        //We create the product object.
                        Product productModel = new Product();

                        int milliseconds2 = 5000;
                        Thread.Sleep(milliseconds2);

                        //We are assigning the profile url of the product.
                        string productProfilUrl = url + homepageFeaturedAds.Attributes["href"].Value;

                        //We do the same thing we did for sahibinden.com in the product profile, and we get the html of the page.
                        HtmlDocument urunProfilDocument = new HtmlDocument();
                        client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                        client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                        string html2 = client.DownloadString(productProfilUrl);
                        urunProfilDocument.LoadHtml(html2);

                        //We get the price of the product from the page.
                        HtmlNode urunProfilFiyat = urunProfilDocument.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()");


                        //We are assigning the product object.
                        productModel.ProductName = homepageFeaturedAds.InnerText.Trim();
                        if (urunProfilFiyat != null)
                        {
                            productModel.ProductPrice = urunProfilFiyat.InnerText.Trim();
                            decimal productPrice = Convert.ToDecimal(productModel.ProductPrice.Replace(".", "").Replace("TL", ""));
                            productsTotalPrice += productPrice;
                        }
                        else
                        {
                            productModel.ProductPrice = "";
                        }

                        productList.Add(productModel);


                    }
                    homepageFeaturedAdsModel.ProductList = productList;
                    homepageFeaturedAdsModel.ProductsAveragePrice = productsTotalPrice / homepageFeaturedAdsList.Count();




                    //We determine the path of the txt file to be written.
                    string fileName = @"C:\Users\goktu\Desktop\İlanlar.txt";

                    string writeText = "";


                    for (int x = 0; x < productList.Count(); x++)
                    {
                        Console.WriteLine($"{x + 1}- İlan Adı: {productList[x].ProductName} ve İlan Fiyatı: {productList[x].ProductPrice}\n");
                        writeText += ($"{x + 1}- İlan Adı: {productList[x].ProductName} ve İlan Fiyatı: {productList[x].ProductPrice}\n");
                    }
                    Console.WriteLine($"Anasayfa Vitrin Ortalama Fiyat: {homepageFeaturedAdsModel.ProductsAveragePrice}");
                    writeText += $"Anasayfa Vitrin Ortalama Fiyat: {homepageFeaturedAdsModel.ProductsAveragePrice}";

                    //We create txt file.
                    FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Close();
                    File.AppendAllText(fileName, writeText);


                    Console.ReadLine();


                }
            }
            catch (WebException)
            {

                Console.WriteLine("Sahibinden.com bağlantıyı engellemiş. Lütfen sonra tekrardan deneyiniz.");
                Console.ReadLine();

            }








        }
    }

    class HomepageFeaturedAds
    {
        public List<Product> ProductList { get; set; }
        public decimal ProductsAveragePrice { get; set; }

    }


    class Product
    {
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
    }
}




