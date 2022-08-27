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
    /// Date: 27.08.2022
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //We determine the path of the txt file to be written.
            string fileName = @"C:\Users\goktu\Desktop\İlanlar.txt";

            string writeText = "";

            //We create txt file.
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Close();

            HomepageFeaturedAds homepageFeaturedAdsModel = new HomepageFeaturedAds();

            decimal productsTotalPrice = 0;
            int counter = 0;

            try
            {

                string url = "https://www.sahibinden.com/";


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
                        HtmlNode productProfilePrice = productProfileDocument.DocumentNode.SelectSingleNode("//*[@id='classifiedDetail']/div/div[2]/div[2]/h3/text()");


                        //We are assigning the product object.
                        if (homepageFeaturedAds.InnerText.Trim() == "")
                        {
                            continue;
                        }
                        else
                        {
                            productModel.ProductName = homepageFeaturedAds.InnerText.Trim();

                        }


                        if (productProfilePrice != null)
                        {
                            string[] productPriceSplit = productProfilePrice.InnerText.Trim().Split();

                            if (productPriceSplit[1] == "TL")
                            {
                                productModel.ProductPrice = productProfilePrice.InnerText.Trim();
                                decimal productPrice = Convert.ToDecimal(productModel.ProductPrice.Replace(".", "").Replace("TL", ""));
                                productsTotalPrice += productPrice;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            productModel.ProductPrice = "Not Specified";
                        }


                        Console.WriteLine($"{counter + 1}- Product Name: {productModel.ProductName} ve Product Price: {productModel.ProductPrice}\n");
                        writeText = ($"{counter + 1}- Product Name: {productModel.ProductName} ve Product Price: {productModel.ProductPrice}\n");
                        counter++;
                        File.AppendAllText(fileName, writeText);


                    }
                    homepageFeaturedAdsModel.ProductsAveragePrice = (productsTotalPrice / counter).ToString("#,##0.00") + " TL";
                    Console.WriteLine($"Homepage Featured Ads Average Price: {homepageFeaturedAdsModel.ProductsAveragePrice}");

                    writeText = $"Homepage Featured Ads Average Price: {homepageFeaturedAdsModel.ProductsAveragePrice}\n";
                    File.AppendAllText(fileName, writeText);


                }
            }
            catch (WebException)
            {
                if (productsTotalPrice != 0)
                {
                    var ProductsAveragePrice = (productsTotalPrice / counter).ToString("#,##0.00") + " TL";
                    Console.WriteLine($"Homepage Featured Ads Average Price: {ProductsAveragePrice}");

                    writeText = $"Homepage Featured Ads Average Price: {ProductsAveragePrice}\n";

                }


                writeText += "sahibinden.com has blocked the connection. Please try again later.";

                File.AppendAllText(fileName, writeText);
                Console.WriteLine("sahibinden.com has blocked the connection. Please try again later");
                Console.ReadLine();

            }


        }
    }

}




