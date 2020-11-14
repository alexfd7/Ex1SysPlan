

using RestSharp;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace exercicio1
{
    class Program
    {

        
        static string filePath = @"D:\workspace\exercicio1\exercicio1\input\input.csv";
        static void Main(string[] args)
        {

            HttpClient client = new HttpClient();
            var items = System.IO.File.ReadAllLines(filePath)
                               .Select(x => x.Split(','))
                               .Select(x => new
                               {
                                   nome = x[0],
                                   cidade = RemoverAcentos(x[1]),
                                   idade = int.Parse(x[2])
                               })
                               .GroupBy( x => x.cidade, x => x.idade)
                               .Select(x => new
                               {
                                   cidade = x.Key,
                                   idade = Math.Round(x.Average(),2)
                               });

            
            string jsonData = JsonSerializer.Serialize(new { medias = items });
            
            var response = sendHttpPost(jsonData);

            /*Mensagem de retorno da API*/
            Console.WriteLine(response);
            
            

        }
        private static object sendHttpPost(string jsonData)
        {
            var client = new RestClient("https://zeit-endpoint.brmaeji.now.sh/api/avg");
            var request = new RestRequest(Method.POST);            
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            return client.Execute(request).Content;
            
        }


        private static string RemoverAcentos(string text)
        {
  
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString().ToLower();
          

        }


    }
}
