namespace Service.Rest.Extensions
{
    public static class HttpResponseExtension
    {
        public static async Task<string> ReadAsStringAsync(this HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body, true).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }
    }
}
