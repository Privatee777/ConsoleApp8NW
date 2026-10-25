using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("        HAVA MƏLUMAT SİSTEMİ (WEATHER APP)        ");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# Console App");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nŞəhərin adını daxil edin (Çıxış üçün 'exit' yazın): ");
                Console.ResetColor();

                string cityName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(cityName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ Xəta: Şəhər adı boş ola bilməz!");
                    Console.ResetColor();
                    continue;
                }

                if (cityName.ToLower() == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Proqramdan çıxılır. Uğurlar! Sağ olun!");
                    Console.ResetColor();
                    break;
                }

                try
                {
                    string url = $"https://wttr.in/{Uri.EscapeDataString(cityName)}?format=j1";

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Məlumatlar yüklənir, zəhmət olmasa gözləyin...");
                    Console.ResetColor();

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ Xəta baş verdi! Status Kod: {(int)response.StatusCode} ({response.ReasonPhrase})");
                        Console.WriteLine("Daxil etdiyiniz şəhər adının düzgün olduğundan əmin olun.");
                        Console.ResetColor();
                        continue;
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    JsonElement root = doc.RootElement;

                    string country = root.GetProperty("nearest_area")[0].GetProperty("country")[0].GetProperty("value").GetString();

                    var currentCondition = root.GetProperty("current_condition")[0];
                    string tempC = currentCondition.GetProperty("temp_C").GetString();
                    string feelsLikeC = currentCondition.GetProperty("FeelsLikeC").GetString();
                    string humidity = currentCondition.GetProperty("humidity").GetString();
                    string windSpeed = currentCondition.GetProperty("windspeedKmph").GetString();
                    string weatherDescEn = currentCondition.GetProperty("weatherDesc")[0].GetProperty("value").GetString();

                    string weatherDescAz = weatherDescEn.ToLower() switch
                    {
                        "clear" or "sunny" => "Açıq / Günəşli",
                        "partly cloudy" => "Az buludlu",
                        "cloudy" => "Buludlu",
                        "overcast" => "Tutqun",
                        "mist" => "Dumanlı",
                        "fog" => "Qatı duman",
                        "patchy rain nearby" or "light rain" => "Zəif yağış",
                        "moderate rain" => "Yağışlı",
                        "heavy rain" => "Güclü yağış",
                        "snow" => "Qarlı",
                        "thunderstorm" => "İldırımlı və şimşəkli",
                        "smoky haze" or "haze" => "Dumanlı / QİD",
                        _ => weatherDescEn
                    };

                    string formattedCityName = char.ToUpper(cityName[0]) + cityName.Substring(1).ToLower();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n--------------------------------------------------");
                    Console.WriteLine($" 🌍 ŞƏHƏR: {formattedCityName}, {country}");
                    Console.WriteLine("--------------------------------------------------");
                    Console.ResetColor();

                    Console.WriteLine($" 🌡️  Temperatur          : {tempC} °C");
                    Console.WriteLine($" 🌡️  Hiss edilən         : {feelsLikeC} °C");
                    Console.WriteLine($" ☁️  Hava şəraiti        : {weatherDescAz}");
                    Console.WriteLine($" 💧 Rütubət            : {humidity} %");
                    Console.WriteLine($" 💨 Küləyin sürəti     : {windSpeed} km/saat");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("--------------------------------------------------");
                    Console.ResetColor();
                }
                catch (HttpRequestException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Şəbəkə xətası baş verdi: {ex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Gözlənilməz xəta baş verdi: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}