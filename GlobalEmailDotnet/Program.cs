using Newtonsoft.Json;

namespace GlobalEmailDotnet
{
  static class Program
  {
    static void Main(string[] args)
    {
      string baseServiceUrl = @"https://globalemail.melissadata.net/";
      string serviceEndpoint = @"v4/WEB/GlobalEmail/doGlobalEmail"; //please see https://www.melissa.com/developer/global-email for more endpoints
      string license = "";
      string email = "";

      ParseArguments(ref license, ref email, args);
      CallAPI(baseServiceUrl, serviceEndpoint, license, email);
    }

    static void ParseArguments(ref string license, ref string email, string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i].Equals("--license") || args[i].Equals("-l"))
        {
          if (args[i + 1] != null)
          {
            license = args[i + 1];
          }
        }
        if (args[i].Equals("--email"))
        {
          if (args[i + 1] != null)
          {
            email = args[i + 1];
          }
        }
      }
    }

    public static async Task GetContents(string baseServiceUrl, string requestQuery)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri(baseServiceUrl);
      HttpResponseMessage response = await client.GetAsync(requestQuery);

      string text = await response.Content.ReadAsStringAsync();
      var obj = JsonConvert.DeserializeObject(text);
      var prettyResponse = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

      // Print output
      Console.WriteLine("\n=============================== OUTPUT ===============================\n");

      Console.WriteLine("API Call: ");
      string APICall = Path.Combine(baseServiceUrl, requestQuery);
      for (int i = 0; i < APICall.Length; i += 70)
      {
        if (i + 70 < APICall.Length)
        {
          Console.WriteLine(APICall.Substring(i, 70));
        }
        else
        {
          Console.WriteLine(APICall.Substring(i, APICall.Length - i));
        }
      }

      Console.WriteLine("\nAPI Response:");
      Console.WriteLine(prettyResponse);
    }

    static void CallAPI(string baseServiceUrl, string serviceEndPoint, string license, string email)
    {
      Console.WriteLine("\n============== WELCOME TO MELISSA GLOBAL EMAIL CLOUD API =============\n");

      bool shouldContinueRunning = true;
      while (shouldContinueRunning)
      {
        string inputEmail = "";
        if (string.IsNullOrEmpty(email))
        {
          Console.WriteLine("\nFill in each value to see results");
          Console.Write("Email: ");
          inputEmail = Console.ReadLine();
        }
        else
        {
          inputEmail = email;
        }

        while (string.IsNullOrEmpty(inputEmail))
        {
          Console.WriteLine("\nFill in each value to see results");

          if (string.IsNullOrEmpty(inputEmail))
          {
            Console.Write("\nEmail: ");
            inputEmail = Console.ReadLine();
          }
        }

        Dictionary<string, string> inputs = new Dictionary<string, string>()
                {
                    { "format", "json" },
                    { "email", inputEmail}
                };

        Console.WriteLine("\n=============================== INPUTS ===============================\n");
        Console.WriteLine($"\t   Base Service Url: {baseServiceUrl}");
        Console.WriteLine($"\t  Service End Point: {serviceEndPoint}");
        Console.WriteLine($"\t              Email: {inputEmail}");

        // Create Service Call
        // Set the License String in the Request
        string RESTRequest = "";

        RESTRequest += @"&id=" + Uri.EscapeDataString(license);

        // Set the Input Parameters
        foreach (KeyValuePair<string, string> kvp in inputs)
          RESTRequest += @"&" + kvp.Key + "=" + Uri.EscapeDataString(kvp.Value);

        // Build the final REST String Query
        RESTRequest = serviceEndPoint + @"?" + RESTRequest;

        // Submit to the Web Service. 
        bool success = false;
        int retryCounter = 0;

        do
        {
          try //retry just in case of network failure
          {
            GetContents(baseServiceUrl, $"{RESTRequest}").Wait();
            Console.WriteLine();
            success = true;
          }
          catch (Exception ex)
          {
            retryCounter++;
            Console.WriteLine(ex.ToString());
            return;
          }
        } while ((success != true) && (retryCounter < 5));

        bool isValid = false;
        if (!string.IsNullOrEmpty(email))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

        while (!isValid)
        {
          Console.WriteLine("\nTest another record? (Y/N)");
          string testAnotherResponse = Console.ReadLine();

          if (!string.IsNullOrEmpty(testAnotherResponse))
          {
            testAnotherResponse = testAnotherResponse.ToLower();
            if (testAnotherResponse == "y")
            {
              isValid = true;
            }
            else if (testAnotherResponse == "n")
            {
              isValid = true;
              shouldContinueRunning = false;
            }
            else
            {
              Console.Write("Invalid Response, please respond 'Y' or 'N'");
            }
          }
        }
      }

      Console.WriteLine("\n=============== THANK YOU FOR USING MELISSA CLOUD API ===============\n");
    }
  }
}
