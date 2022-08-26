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
                int millisecondsForHomePage = 3000;
                Thread.Sleep(millisecondsForHomePage);

                string url = "https://www.sahibinden.com/";

                List<Product> productList = new List<Product>();

                using (WebClient client = new WebClient())
                {
                    //This two code blocks make our WebClient act like browser.
                    string accept = "Accept: text / html, application / xhtml + xml, */*";
                    string userAgent = "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

                    HtmlDocument sahibindenDocument = new HtmlDocument();

                    //We make the webclient act like a browser to bypass the Sahibinden.com site, and we add headers for this.
                    client.Headers.Add(accept);
                    client.Headers.Add(userAgent);
                    string html = client.DownloadString(url);
                    sahibindenDocument.LoadHtml(html);

                    //We take the "Anasayfa Vitrin" as the "homepageFeaturedAdsList". We use HtmlAgilityPack for this.
                    List<HtmlNode> homepageFeaturedAdsList = sahibindenDocument.DocumentNode.SelectNodes("//ul[@class='vitrin-list clearfix']//li//a").ToList();

                    homepageFeaturedAdsList.RemoveRange(5, 50);
                    HomepageFeaturedAds homepageFeaturedAdsModel = new HomepageFeaturedAds();

                    decimal productsTotalPrice = 0;


                    foreach (HtmlNode homepageFeaturedAds in homepageFeaturedAdsList)
                    {
                        //We create the product object.
                        Product productModel = new Product();

                        int millisecondsForProfile = 5000;
                        Thread.Sleep(millisecondsForProfile);

                        //We are assigning the profile url of the product.
                        string productProfilUrl = url + homepageFeaturedAds.Attributes["href"].Value;

                        //We do the same thing we did for sahibinden.com in the product profile, and we get the html of the page.
                        HtmlDocument productProfileDocument = new HtmlDocument();
                        client.Headers.Add(accept);
                        client.Headers.Add(userAgent);
                        string html2 = client.DownloadString(productProfilUrl);
                        productProfileDocument.LoadHtml(html2);

                        //We get the price of the product from the page.
                        HtmlNode productPriceProfile = productProfileDocument.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()");


                        //We are assigning the product object.
                        if (homepageFeaturedAds.InnerText.Trim() == "")
                        {
                            break;
                        }
                        else
                        {
                            productModel.ProductName = homepageFeaturedAds.InnerText.Trim();

                        }
                        if (productPriceProfile != null)
                        {
                            productModel.ProductPrice = productPriceProfile.InnerText.Trim();
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
                    homepageFeaturedAdsModel.ProductsAveragePrice = (productsTotalPrice / homepageFeaturedAdsList.Count()).ToString("#,##0.00")+" TL";




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
        public string ProductsAveragePrice { get; set; }

    }


    class Product
    {
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
    }
}




