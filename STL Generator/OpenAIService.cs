using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace STL_Generator
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string Model = "gpt-3.5-turbo";
        private const string SystemPrompt = @"You are an assistant that generates OpenSCAD code for 3D printable models.

Strict rules:
- Only generate 3D objects (cube, sphere, cylinder, polyhedron, etc.) at the top level.
- If the user requests a 2D shape (like a circle or square), extrude it to 3D using linear_extrude().
- Always specify all required parameters for each OpenSCAD function.
- Do not use 2D primitives (circle, square, polygon) at the top level unless they are inside a 3D extrusion.
- The output must be a complete, valid OpenSCAD script that can be rendered and exported as STL without errors.
- Only use // for single-line and /* ... */ for multi-line comments.
- Do not include markdown code blocks or any text except valid OpenSCAD code.
- Do not include any echo(), assert(), or other debugging statements.
- The top-level object must always be a 3D object.

Examples:
// Good:
cube([10, 10, 10]);
sphere(r=5);
cylinder(h=20, r=5);
linear_extrude(height=5) circle(r=10);

// Bad (do not do this):
circle(r=10);
square([10,10]);
polygon(points=[[0,0],[1,0],[0,1]]);

If the user requests a 2D shape, always wrap it in linear_extrude() or another 3D operation.";

        public OpenAIService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string?> GenerateOpenScadCodeAsync(string prompt)
        {
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = prompt }
                },
                max_tokens = 1024,
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            return result == null ? null : CleanOpenScadCommentsAndMarkdown(result);
        }

        // Post-processing: ensure only // and /* ... */ comments are present, and remove markdown code blocks
        private string CleanOpenScadCommentsAndMarkdown(string code)
        {
            // Remove markdown code block markers (``` and ```scad)
            code = Regex.Replace(code, @"```[a-zA-Z]*", string.Empty);
            code = code.Replace("```", string.Empty);
            // Remove any # comments (not valid in OpenSCAD)
            code = Regex.Replace(code, @"(^|\s)#.*$", string.Empty, RegexOptions.Multiline);
            // Trim leading/trailing whitespace
            return code.Trim();
        }
    }
}


