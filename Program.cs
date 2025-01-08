// See https://aka.ms/new-console-template for more information


using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SitemapGenerator.Models;

Console.WriteLine("Enter Your Domain :");
var domain = Console.ReadLine();
var linksList = new List<LinksModel>();
var selectList = new List<string>();
Console.WriteLine("Enter Your Select Tag");
var selecttags = Console.ReadLine();
selectList = selecttags.Split(",").ToList();
linksList.Add(new LinksModel(){Link = domain,IsCheck = false});

IWebDriver driver = new ChromeDriver();
do
{
    var link = linksList.Where(x => x.IsCheck == false).ToList()[0];
    driver.Navigate().GoToUrl(link.Link);
    // Thread.Sleep(1000);
    var linksInPage = driver.FindElements(By.TagName("a")).ToList();
    foreach (var l in linksInPage)
    {
        var href = domain + l.GetDomAttribute("href");
        if (href == domain + "/")continue;
        if (selectList.Any(x => href.Contains(x)) == true)
        {
            if (linksList.Any(x=>x.Link == href) == false)
            {
                linksList.Add(new LinksModel(){Link = href , IsCheck = false});
            }
        }
    }

    link.IsCheck = true;

} while (linksList.Any(x=>x.IsCheck == false));

StringBuilder sb = new StringBuilder();
sb.Append("<?xml version='1.0' encoding='UTF-8' ?><urlset xmlns = 'http://www.sitemaps.org/schemas/sitemap/0.9'>");

foreach (var links in linksList)
{
    sb.Append("<url><loc>" + Uri.UnescapeDataString(links.Link) +
              "</loc><changefreq>" + "daily" + "</changefreq><priority>1</priority></url>");
}
sb.Append("</urlset>");
string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string sitemapFilePath = Path.Combine(desktopPath, $"{domain.Replace("https://","")}-sitemap.xml");
File.WriteAllText(sitemapFilePath, sb.ToString());

driver.Quit();
driver.Close();