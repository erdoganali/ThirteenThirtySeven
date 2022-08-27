// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text.RegularExpressions;

public class Program
{

    static void Main(string[] args)
    {
        var url = new AddressItem { hostUri = "https://tretton37.com", uriSuffix = "" };
        List<AddressItem> stack = new();

        Getlink(url, stack);

        Console.WriteLine();
        Console.ReadLine();
    }

    public static async void Getlink(AddressItem url, List<AddressItem> stack)
    {
        using (WebClient w = new WebClient())
        {

            if (IsAddressExists(url.ToString()))
            {
                Console.WriteLine($"{url.ToString()} Downloading................");

                var sorce = w.DownloadString(new Uri(url.ToString()));
                //TODO: SAVE file

                var list = FindAddress(url, sorce);
                Parallel.ForEach(list, (item) =>
                {
                    //  Console.WriteLine("thread no:" + Thread.CurrentThread.ManagedThreadId);
                    if (!stack.Contains(item))
                    {
                        stack.Add(item);
                        Getlink(item, stack);
                    }
                });


            }
        }

    }

    private static bool IsAddressExists(string url)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return (response.StatusCode == HttpStatusCode.OK);
            }
        }
        catch
        {
            return false;
        }
    }

    private static List<AddressItem> FindAddress(AddressItem address, string file)
    {
        List<AddressItem> list = new();
        MatchCollection matches = Regex.Matches(file
              , @"<(a).*?href=(\"")/(.+?)(\""|').*?>"
              , RegexOptions.Singleline);

        AddressItem i = new AddressItem();
        foreach (Match m in matches)
        {
            i.hostUri = address.hostUri;
            i.uriSuffix = m.Groups[3].Value;
            list.Add(i);
        }

        return list;
    }

    public struct AddressItem
    {
        public string hostUri;
        public string uriSuffix;


        public override string ToString()
        {
            return hostUri + "/" + uriSuffix;
        }
    }

}